using System;
using System.IO;
using System.Text; //Bennyttes til decodning Encoding.ASCII.GetBytes 
using System.Diagnostics;
using System.Threading;




namespace picCatchProgram
{
	class pictureSynciniser
    {
        bool runner = true;
        string tag_;
        string service_;

		private pictureSynciniser(string tag, string service)
		{
            tag_ = tag;
            service_ = service;
		}


        //With this thread use tag_ and service_.
		private void picGet() //////////////////////////////YOUR CLASS////////////////////////////////////////
		{
			Console.WriteLine ("Social get pic Started");

			while (runner)
            {

                //Write code here

			}
		}
	}


    public static void Main(string[] args)
    {
        while (true)
        {
            try
            { 
                var tester = new pictureSynciniser("pubc15gr7", "instagram"); //Start class

                while (true)
                {
                    var thPicGet = new Thread(tester.picGet);

                    try
                    {
                        if (thPicGet.IsAlive == false)
                        {
                            thPicGet.Start();
                        }


                        while (thPicGet.IsAlive)
                        {
                            Thread.Sleep(120000); //2min before try end
                            thPicGet.runner = false;
                        }
                        Console.WriteLine("thPicGet is dead");
                        thPicGet.Abort();
                    }
                    catch
                    {
                        Console.WriteLine("Restarting server");
                        thPicGet.Abort();
                    }
                }

                s.runner = false;
            }
            catch
            {
                Console.WriteLine("The hole program has chrash restarting");
            }
        }
    }
}


