using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _07.Print_All_Names
{
	class StartUp
	{
		private static string connectionString = "Server=DESKTOP-5CAFU3F\\SQLEXPRESS;" +
												 "Database=MinionsDB;" +
												 "Integrated Security=true;";
		private static SqlConnection connection = new SqlConnection(connectionString);
		static void Main(string[] args)
		{
			connection.Open();
			string queryText = "SELECT Name FROM Minions";
			using(connection)
			{
				SqlCommand command = new SqlCommand(queryText, connection);
				SqlDataReader reader = command.ExecuteReader();
				List<string> minionNames = new List<string>();
				using(reader)
				{
					while(reader.Read())
					{
						minionNames.Add((string)reader["Name"]);
					}
				}
				for (int i = 0; i < minionNames.Count / 2; i++)
				{
					Console.WriteLine(minionNames[i]);
					Console.WriteLine(minionNames[minionNames.Count-1-i]);
					if (i == minionNames.Count / 2 - 1 && minionNames.Count % 2 != 0)
					{
						Console.WriteLine(minionNames[i + 1]);
					}
				}
			}
		}
	}
}
