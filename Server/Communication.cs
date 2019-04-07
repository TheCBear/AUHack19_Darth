using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text; //Bennyttes til decodning Encoding.ASCII.GetBytes 



namespace Communication
{
	class Communication
	{

		/// <MINE VARIABLER, HUSK AT SÆTTE MIG>
		const int forbytes = 1; //Bit der ligger før modtagelsen i bufferen
		string filePath = @"/root/ikn/Oevelse6/C#/ServerFileFolder/"; //Your File Path;//Hvor ligger servermappen
		string fileName = "log.txt";
		/// <MINE VARIABLER, HUSK AT SÆTTE MIG>

		/// <summary>
		/// The PORT
		/// </summary>
		//const int PORT = 9003;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000; //Må ikke sættes over jombo buffer på lan er den 9000 på wifi er den efter IEEE på 7935


		public Communication()
		{
			
		}




		public Socket waitForClient(Socket Sock)
		{
			Socket serverSock = Sock.Accept();
			return serverSock;
		}

		public Socket openConnection(int PORT)
		{
			IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, PORT); //Kontroller alle ip-addresser på den port, fordi den er sat til any
			Socket Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp ); //InterNetwork means IPV4
			Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout,3000); //Timeout ved sendning
			Sock.Bind(ipEnd);
			Sock.Listen(100);
			return Sock;
		}

		public bool closeConnection(Socket sock)
		{
			try
			{
				sock.Close();
				return false;
			}
			catch 
			{
				return true;
			}
		}

		public byte[] recive(Socket serverSock)
		{
			serverSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout,5000); //Timeout ved modtagelse, Må ikke kaldes, før acceptet, da serveren skal vente på en mullig klient
			serverSock.ReceiveTimeout = 5000;
			byte[] bytes = new byte[BUFSIZE];
			int msgLength = 0;

				msgLength = serverSock.Receive (bytes);  
			
			return bytes;
		}

		public byte[] receive(Socket serverSock)
		{
			byte[] bytes = new byte[BUFSIZE];
			byte[] recBuf = new byte[1];
			int recLen = 0;
			char ch;
			do
			{
				serverSock.Receive(recBuf,1,0);
				ch = (char)recBuf[0];
				if(ch!=0)
					bytes[recLen++]=recBuf[0];
			}while(ch != 0 && recLen < BUFSIZE);
			return bytes;
		}

		public bool transmit(Socket serverSock, byte[] msg)
		{
			serverSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout,5000); //Timeout ved sendning
			try
			{ 
				serverSock.Send (msg); 
				return false;
			}
			catch
			{
				return true;
			}
		}

		public byte[] convertMessageString(string msg)
		{
			byte[] convertet = Encoding.ASCII.GetBytes (msg); 
			return convertet; 
		}

		public byte[] convertMessageInt(int msg)
		{
			byte[] convertet = BitConverter.GetBytes(msg);
			return convertet;
		}

		public string decodeMessageString(byte[] msg)
		{
			string decoded = Encoding.ASCII.GetString (msg, 0, msg.Length);
			return decoded;
		}

		public int decodeMessageInt(byte[] msg)
		{
			int decoded = BitConverter.ToInt32(msg, 0);
			return decoded;
		}

		private void sendLog (Socket io) //Jeg har fjernet long fileSize(Unødvendig), netstream gav ikke menning da det er en socket
		{
			// TO DO Your own code

			io.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout,5000); //Timeout ved modtagelse
			io.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout,5000); //Timeout ved sendning

			byte[] fileData = new byte[BUFSIZE-forbytes]; //Benyttes til at lagre dele af filen, som skal sendes
			using(FileStream fs = new FileStream(filePath + fileName, FileMode.Open, FileAccess.Read)) //Åbner en filestream, til at læse fra en file
			{

				int procentBuf = 0; //Disse 3 funktioner benyttes til at finde procent
				double procentKontrol = 0;
				bool disp = true;

				long count = new System.IO.FileInfo((filePath + fileName)).Length; //Giver mig file størrelsen uden at læse filen
				byte[] buffer1 = new byte[BUFSIZE]; //Jeg plusser buffersize på for at have en længere, så jeg ikke ender på eksempel 2999, hvor jeg skulle have 3000
				int buffer2 = 0;
				byte[] emptyData = new byte[1]; //Benyttes til respons fra client
				long sendCount = (count / (BUFSIZE-forbytes)) + 1 ; //Devider med buffer, for at få antal gange og da vi runder ned, skal vi have 1+

				if(sendCount >= (BUFSIZE-forbytes))
				{
					//sendCount = sendCount + (sendCount / (BUFSIZE-forbytes)) ; //Sikre at overførslen virker ved over 100kB
				}

				byte[] countBytes = BitConverter.GetBytes(sendCount);

				Console.WriteLine("\n Antal gange der skal sendes: {0}", sendCount);

				io.Send(countBytes);

				for (int i = 1; i <= sendCount; i = i + 1) 	//Begynder sendelse af filen i denne for løkke efter antal af pakker
				{
					io.Receive (emptyData);//Venter på clienten er klar

					if (i > procentKontrol && disp == true) //Fortæl hvor mange % der er tilbage af overførslen
					{
						Console.Clear();
						Console.WriteLine("Venter: {0} procent ud af 100 procent eller {1} pakker ud af {2}.", procentBuf, i, sendCount);
						procentBuf++; //Tæller 1 procent op til næste udskrivning
						procentKontrol = procentBuf * sendCount / 100; //Finder værdi for næste procent i antal pakker
					}

					if (sendCount <= 1) //Ved kun 1 pakke
					{
						Array.Resize(ref buffer1, unchecked((int)(count+forbytes)));
						fs.Read (fileData, buffer2, unchecked((int)count));
						Array.Copy(fileData, buffer2, buffer1, forbytes, count);
					}
					else if (i >= sendCount)//Ved sidste pakke ved flere pakker
					{	
						Console.WriteLine("NO MORE DATA");
						int x = unchecked((int)(count - buffer2));
						Console.WriteLine("Reste data: {0}",x);

						fs.Read (fileData, 0, x);
						Array.Resize(ref buffer1, unchecked((int)(x+forbytes)));
						Array.Copy(fileData, 0, buffer1, forbytes, x);
					}	
					else if (buffer2 <= count - BUFSIZE) //Ved flere pakker
					{
						fs.Read (fileData, 0, BUFSIZE-forbytes);
						Array.Copy(fileData, 0, buffer1, forbytes, BUFSIZE-forbytes);
						buffer2 = buffer2 + BUFSIZE-forbytes;
					}

					io.Send (buffer1);
				}
			}
		}
	}
}

