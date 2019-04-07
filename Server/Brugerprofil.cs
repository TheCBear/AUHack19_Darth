
using System;
using System.IO;
using System.Text; //Bennyttes til decodning Encoding.ASCII.GetBytes 


namespace UserProfile
{
	class UserProfile
	{
		private string id_;
		private string firstName_;
		private string lastName_;
		private string pass_;

		public UserProfile(string id, string firstName, string lastName, string pass)
		{
			id_ = id;
			firstName_ = firstName;
			lastName_ = lastName;
			pass_ = pass;
		}

		public string getId( )
		{
			return id_;
		}

		public string getFirstName()
		{
			return firstName_;
		}

		public string getLastName()
		{
			return lastName_;
		}

		public string getPass()
		{
			return pass_;
		}

		public bool setId(string id)
		{
			id_ = id;
			return true;
		}

		public bool setFirstName(string firstName)
		{
			firstName_ = firstName;
			return true;
		}

		public bool setLastName(string lastName)
		{
			lastName_ = lastName;
			return true;
		}

		public bool setPass(string pass)
		{
			pass_ = pass;
			return true;
		}
	}
}

