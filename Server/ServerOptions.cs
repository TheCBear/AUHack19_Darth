using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text; //Bennyttes til decodning Encoding.ASCII.GetBytes 
using System.Diagnostics;
using System.Threading;


using Communication;



namespace ServerOptions
{
	class ServerOptions
	{
		const int boxPORT = 4952;
		const int BUFSIZE = 1000;

        string[] addr = { "B4:E6:2D:45:18:C0", "B4:E6:2D:45:68:79", "B4:E6:2D:45:6F:B4" };
        string[] stades = { "available", "available", "available" };


        const int forbytes = 1; //Bit der ligger før modtagelsen i bufferen


        private ServerOptions()
		{


			while (true) 
			{
				var thBox = new Thread (boxTasker);

				try
				{
					if (thBox.IsAlive == false) {
						thBox.Start ();
					}


					while (thBox.IsAlive)
                    {
                        string[] home = {
                            "<!DOCTYPE html>",
                            "<html>",
                            "<head>",
                                "<meta http-equiv=\"refresh\" content=\"5\" />",
                                "<meta charset=\"utf-8\" />",
                                "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />",
                                "<title>@ViewData[\"Title\"] - socialPicView</title>",
                                "<link rel=\"stylesheet\" href=\"ground_view.css\" type=\"text/css\" />",
                            "</head>",
                            "<body>",
                                "<div id=\"titleBar\">",
                                    "<h1 id=title>",
                                        "Workspace Finder",
                                    "</h1>",
                                    "<hr></hr>",
                                "</div>",
                                "<div id=\"imgDiv\">",
                                    "<img src=\"ground_view.PNG\" alt=\"Ground plane\"></img>",
                                    "<div id=\"table1\">&#10006</div>",
		                            "<div id=\"table2\">&#10003</div>",
		                            "<div id=\"table3\">&#10006</div>",
	                            "</div>",
                            "</body>",
                            "</html>"
                        };

                        if(stades[0] == "occupied")
                        {
                            home[18] = "<div id=\"table1\">&#10006</div>";
                        }
                        else
                        {
                            home[18] = "<div id=\"table1\">&#10003</div>";
                        }

                        if (stades[1] == "occupied")
                        {
                            home[19] = "<div id=\"table2\">&#10006</div>";
                        }
                        else
                        {
                            home[19] = "<div id=\"table2\">&#10003</div>";
                        }

                        if (stades[2] == "occupied")
                        {
                            home[20] = "<div id=\"table3\">&#10006</div>";
                        }
                        else
                        {
                            home[20] = "<div id=\"table3\">&#10003</div>";
                        }

                        System.IO.File.WriteAllLines(@"C:\Users\andre\Dropbox\Public\home.html", home);


                        string[] ground_view = {
                            "#table1{",

                                 "   color: red;",
                                 "                   position: absolute;",
                                 "                   top: 540px;",
                                 "                   left: 340px;",
                                 "                   font-size: 50px;",
                                 "                   font-weight: bold;",
                                 "                   }",

                                "#table2{",
                                 "               color: green; /* red/green */",
                                 "               position: absolute;",
                                 "               top: 545px;",
                                 "               left: 425px;",
                                 "               font-size: 50px;",
                                 "               font-weight: bold;",
                                 "               }",

                                "#table3{",
                                "            color: red;",
                                "            position: absolute;",
                                "            top: 550px;",
                                "            left: 505px;",
                                "                font-size: 50px;",
                                "                font-weight: bold;",
                                "            }",

                                "#titleBar{",
                                "        position: absolute;",
                                "        top: 300 px;",
                                "        left: -20px;",
                                "        background: lightgrey;",
                                "        width: 120%;",
                                "        }",
                                "#title{",
                                "        position: relative;",
                                 "   left: 10%;",
                                 "   font-size: 40px;",
                                "}",

                                "#imgDiv{",
                                "    position: relative;",
                                "    top:100px;",
                                "    margin-left: auto;",
                                "    margin-right: auto;",
                                "    z-index: -1;",
                                "}"
                        };
                        System.IO.File.WriteAllLines(@"C:\Users\andre\Dropbox\Public\ground_view.css", ground_view);
                        System.Threading.Thread.Sleep(15 * 100);
                    }

                    Console.WriteLine("BoxThread is dead");
                    thBox.Abort();
                }
				catch 
				{
					Console.WriteLine ("Restarting server");
					thBox.Abort ();
				}
			}
		}


		private void boxTasker()
		{
			Console.WriteLine ("BoxTasker started");
			Communication.Communication boxCommunication = new Communication.Communication ();

			Console.WriteLine ("Waiting for connection to box");
			Socket boks = boxCommunication.openConnection (boxPORT);

			while (true) {

				Socket boks_connected = boxCommunication.waitForClient (boks);
				//boks_connected.SendTimeout = 1000;
				//boks_connected.reciveTimeout = 5000;
				Console.WriteLine ("BOX Connected");

				try {
					int timeRemember = 0;
					int timeNow = 0;
					string boxState = "0";
					int tjekCounter = 0;
					bool firstStart = true;


					timeRemember = timeNow;

					while (boks_connected.Connected) 
					{
                        byte[] buf = new byte[BUFSIZE];
						string menuBox = "0";
						DateTime time = DateTime.Now;
						timeNow = time.Second;
                        

                        if (boks_connected.Available > 0)
						{
							buf = boxCommunication.recive (boks_connected);
							menuBox = boxCommunication.decodeMessageString (buf);
							timeRemember = timeNow;
							Console.WriteLine ("menuBox = {0}", menuBox);
						}
						//else if (timeRemember + 5 == timeNow )
						//{
      //                      Console.WriteLine("Time: {0} ", timeNow);
      //                      Console.WriteLine("States: {0} , {1} , {2}", stades[0], stades[1], stades[2]);
      //                      //menuBox = "2";
      //                      timeRemember = timeNow;
						//}
						//else if (5 > timeNow )
						//{
      //                      timeRemember = 5;
						//}
						//else if(firstStart == true)
						//{
						//	//menuBox = "2";
						//	firstStart = false;
						//}

						switch (menuBox.Substring (0, Math.Min (1, menuBox.Length))) 
						{ //Menu for the box
						case "1": //login/unlock 
							Console.WriteLine("Getting table data");
                            buf = boxCommunication.convertMessageString("1");
                            boxCommunication.transmit(boks_connected, buf);
                            System.Threading.Thread.Sleep(500);
                            buf = boxCommunication.receive(boks_connected);
							string id = boxCommunication.decodeMessageString (buf);
                            boxCommunication.transmit(boks_connected, buf);
                            System.Threading.Thread.Sleep(500);
                            Console.WriteLine("ID: {0}", id);
                            buf = boxCommunication.receive(boks_connected);
                            string state = boxCommunication.decodeMessageString (buf);
                            Console.WriteLine("State: {0}", state);
                            
                            if(addr[0] == id)
                            {
                                stades[0] = state;
                            }
                            else if (addr[1] == id)
                            {
                                stades[1] = state;
                            }
                            else if(addr[2] == state)
                            {
                                stades[2] = state;
                            }
                        break;


						case "2": //Ping
                                Console.WriteLine("Asking for table data");
                                buf = boxCommunication.convertMessageString("1");
                                //boxCommunication.transmit(boks_connected, buf);
                                System.Threading.Thread.Sleep(100);
                         break;
							
						}
					}
				} 
				catch 
				{									
					Console.Write ("Boks lost connection");
                    boxCommunication.closeConnection(boks_connected);

                }


			}
		}




		public static void Main (string[] args)
		{
			while (true) 
			{
				try
				{
					Console.WriteLine ("Server starts...");
					new ServerOptions (); //Start class
				}
				catch 
				{
					Console.WriteLine ("The hole server has chrash restarting");
				}
			}
		}
	}
}


