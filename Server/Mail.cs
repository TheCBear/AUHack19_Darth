using System;
using System.IO;
using System.Text; //Bennyttes til decodning Encoding.ASCII.GetBytes 
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;


namespace Mail
{
	class Mail
	{
		const string pass = "fjz46tus";
		const string server = "au570003@gmail.com";

		public Mail()
		{


		}


		public static bool sendMailAdmin(string notifier, DateTime time)
		{
			// Command line argument must the the SMTP host.

			// Specify the e-mail sender.
			// Create a mailing address that includes a UTF8 character
			// in the display name.

			// Set destinations for the e-mail message.
			//MailAddress to = new MailAddress("asbjoern@lybker.dk");
			//MailAddress from = new MailAddress ("root@84.238.30.243");
			// Command line argument must the the SMTP host.
			var serverAddress = new MailAddress(server, "Værdi-Box_Server");
			var admin1Address = new MailAddress("andreas.hervert@gmail.com", "Admin1");
			const string subject = "Værdi-Box Start!";
			string body = "Værdi-Box start: " + notifier + " At: " + time;
			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(serverAddress.Address, pass)
			};
			using (var message = new MailMessage(serverAddress, admin1Address)
				{
					Subject = subject,
					Body = body
				})
				//message.To.Add(admin2Address);  
				//message.To.Add(admin3Address);  

			{
				smtp.Send(message);
			}
				return true;
		
		}



		public static bool sendMailAlert(string alarmId, DateTime time)
		{
			var serverAddress = new MailAddress(server, "Værdi-Box_Server");
			var admin1Address = new MailAddress("andreas.hervert@gmail.com", "Admin1");
			const string subject = "Værdi-Box ALERT!";
			string body = "Værdi-Box alarm aktivatet: " + alarmId + " At: " + time;
			var smtp = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(serverAddress.Address, pass)
			};
			using (var message = new MailMessage(serverAddress, admin1Address)
				{
					Subject = subject,
					Body = body
				})
				//message.To.Add(admin2Address);  
				//message.To.Add(admin3Address);  

			{
				smtp.Send(message);
			}
			return true;

			return true;
		}



		static bool mailSent = false;
		private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
		{
			// Get the unique identifier for this asynchronous operation.
			String token = (string) e.UserState;

			if (e.Cancelled)
			{
				Console.WriteLine("[{0}] Send canceled.", token);
			}
			if (e.Error != null)
			{
				Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
			} else
			{
				Console.WriteLine("Message sent.");
			}
			mailSent = true;
		}
	}
}

