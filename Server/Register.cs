using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UserProfile;

namespace Register
{
	class Register
	{
		private List<UserProfile.UserProfile> users = new List<UserProfile.UserProfile>();
		private int userCount;
		private List<UserProfile.UserProfile>.Enumerator enumerator;
		private bool getNextUser = false;
		private string filePath = @"/root/PRJ4_GRP3/Server/"; //Your File Path;//Hvor ligger servermappen
		//private string filePath = @"/home/stud/Prj4/PRJ4_GRP3/Server/"; // morten jesper filepath
		private string fileName = "Brugerprofiler.txt";

		private string sRegId = @"(\d|[a-z]|[A-Z]){10}";
		private string sRegFirstName = @"\:([a-z]|[A-Z]|\s|\d)+";
		private string sRegLastName = @"\.:([a-z]|[A-Z]|\s|\d)+";
		private string sRegPass = @"\.\d{4}";
		Regex regUser;
		Regex regId;
		Regex regFirstName;
		Regex regLastName;
		Regex regPass;
		public Register()
		{
			regId = new Regex(sRegId);
			regFirstName = new Regex(sRegFirstName);
			regLastName = new Regex(sRegLastName);
			regPass = new Regex(sRegPass);
			regUser = new Regex(sRegId + sRegFirstName + sRegLastName + sRegPass);
			using (StreamReader sr = new StreamReader(filePath + fileName))
			{
				string s = sr.ReadToEnd();
				foreach (Match m in regUser.Matches(s))
				{
					string id = regId.Match(m.Value).Value;
					string firstName = regFirstName.Match(m.Value).Value.Substring(1);
					string lastName = regLastName.Match(m.Value).Value.Substring(2);
					string pass = regPass.Match(m.Value).Value.Substring(1);
					UserProfile.UserProfile up = new UserProfile.UserProfile(id, firstName, lastName, pass);
					users.Add(up);
				}
				userCount = users.Count();
			}
		}

		public bool addUser( string id, string firstName, string lastName, string pass)
		{
			if (regUser.IsMatch(id + ":" + firstName + ".:" + lastName + "." + pass))
			{
				foreach (UserProfile.UserProfile u in users)
				{
					if (id == u.getId())
						return false;
				}
				UserProfile.UserProfile up = new UserProfile.UserProfile(id, firstName, lastName, pass);
				users.Add(up);
				userCount = users.Count();
				using (StreamWriter sw = new StreamWriter(filePath + fileName, true))
				{
					sw.Write(id + ":" + firstName + ".:" + lastName + "." + pass);
					sw.Write(":\n");
				}
				return true;
			}
			return false;
		}

		public bool editUser(string origId, string id, string firstName, string lastName, string pass)
		{
			if (regUser.IsMatch(id + ":" + firstName + ".:" + lastName + "." + pass))
			{
				var u = users.SingleOrDefault(up => up.getId() == origId);
				if (u != null)
				{
					foreach (UserProfile.UserProfile ou in users)
					{
						if (id == ou.getId())
							return false;
					}
					string s;
					using (StreamReader sr = new StreamReader(filePath + fileName))
					{
						s = sr.ReadToEnd();
						Regex regOrigUser = new Regex(origId + sRegFirstName + sRegLastName + sRegPass);
						s = regOrigUser.Replace(s, id + ":" + firstName + ".:" + lastName + "." + pass);  
					}
					using (StreamWriter sw = new StreamWriter(filePath + fileName, false))
					{
						sw.Write(s);
					}
					u.setId(id);
					u.setFirstName(firstName);
					u.setLastName(lastName);
					u.setPass(pass);
					return true;
				}
			}
			return false;
		}

		public bool deleteUser( string id)
		{
			var up = users.SingleOrDefault(u => u.getId() == id);
			if (up != null)
			{
				string s;
				using (StreamReader sr = new StreamReader(filePath + fileName))
				{
					s = sr.ReadToEnd();
					Regex regOrigUser = new Regex(id + sRegFirstName + sRegLastName + sRegPass + @"\D+");
					s = regOrigUser.Replace(s, "");
				}
				using (StreamWriter sw = new StreamWriter(filePath + fileName, false))
				{
					sw.Write(s);
				}
				users.Remove(up);
				userCount = users.Count();
				return true;
			}
			return false;
		}

		public bool logIn(string id, string pass)
		{
			foreach(UserProfile.UserProfile u in users)
			{
				if(id.Substring (0, Math.Min (10, id.Length)) == u.getId().Substring (0, Math.Min (10, u.getId().Length)))
				{
					if (pass.Substring (0, Math.Min (4, pass.Length)) == u.getPass().Substring (0, Math.Min (4, u.getPass().Length)))
						return true;
					else
						return false;
				}
			}
			return false;
		}

		public int getUserCount()
		{
			return userCount;
		}

		public bool getUsers(int no, ref string id, ref string firstName, ref string lastName, ref string pass) //random access, lineær tid, langsom
		{
			if (no < 0)
				return false;
			if (no > userCount)
				return false;
			enumerator = users.GetEnumerator();
			enumerator.MoveNext();
			for (int i = 0; i < no; i++)
			{
				if (!enumerator.MoveNext())
					return false;
			}
			id = enumerator.Current.getId();
			firstName = enumerator.Current.getFirstName();
			lastName = enumerator.Current.getLastName();
			pass = enumerator.Current.getPass();
			return true;	
		}

		public bool getUsers(ref string id, ref string firstName, ref string lastName, ref string pass) //sequence access, konst tid, hurtig
		{
			if(getNextUser)
			{
				if (enumerator.MoveNext())
				{
					id = enumerator.Current.getId();
					firstName = enumerator.Current.getFirstName();
					lastName = enumerator.Current.getLastName();
					pass = enumerator.Current.getPass();
				}
				else
					getNextUser = false;
			}
			else
			{
				enumerator = users.GetEnumerator();
				if(enumerator.MoveNext())
				{
					id = enumerator.Current.getId();
					firstName = enumerator.Current.getFirstName();
					lastName = enumerator.Current.getLastName();
					pass = enumerator.Current.getPass();
					getNextUser = true;
				}
			}
			return getNextUser;
		}

		public void resetGetUsers()
		{
			getNextUser = false;
		}
	}
}

