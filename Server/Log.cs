using System;
using System.IO;
using System.Text; //Bennyttes til decodning Encoding.ASCII.GetBytes 
using System.Text.RegularExpressions;
using Register;
namespace Log
{
	class Log
	{
		static string filePath = @"/root/PRJ4_GRP3/Server/"; //Your File Path;//Hvor ligger servermappen
		//private string filePath = @"/home/stud/Prj4/PRJ4_GRP3/Server/"; // morten jesper filepath
		private string fileName = "Log.txt";
		private System.Collections.Generic.IEnumerator<string> enumerator;
		private bool readingLog = false;
		private int eventCount = 0;
		public Log()
		{
			using (StreamReader r = new StreamReader(filePath + fileName))
			{
					while(r.ReadLine() != null)
						eventCount++;	
			}
		}


		public void setUserLogin( string id)
		{
			using (StreamWriter sw = new StreamWriter(filePath + fileName, true))
			{
				string toLog = "User Id: " + id + " logged in at " + DateTime.Now + "\n";
				sw.Write(toLog);
				eventCount++;
			}
		}

		public void setUserEdit( string id, string name, string editDetails)
		{
			using (StreamWriter sw = new StreamWriter(filePath + fileName, true))
			{
				string toLog = "User Id: " + id + " " + name + " edited to " + editDetails + " at " + DateTime.Now + "\n";
				sw.Write(toLog);
				eventCount++;
			}
		}

		public void setAlarm( string sensor)
		{
			using (StreamWriter sw = new StreamWriter(filePath + fileName, true))
			{
				string toLog = "Alarm triggered by sensor: " + sensor + " at " + DateTime.Now + "\n";
				sw.Write(toLog);
				eventCount++;
			}
		}

		public bool readLog(ref string line)
		{
			if(readingLog)
			{
				if (enumerator.MoveNext())
					line = enumerator.Current;
				else
					readingLog = false;

			}
			else
			{
				enumerator = File.ReadLines(filePath + fileName).GetEnumerator();
				if (enumerator.MoveNext())
				{
					line = enumerator.Current;
					readingLog = true;
				}
			}
			return readingLog;
		}

		public void resetRead()
		{
			readingLog = false;
		}

		public int getEventCount()
		{
			return eventCount;
		}
	}
}


