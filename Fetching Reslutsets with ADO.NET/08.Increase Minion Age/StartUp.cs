using System;
using System.Data.SqlClient;
using System.Linq;

namespace _08.Increase_Minion_Age
{
	class StartUp
	{
		private static string connectionString = "Server=DESKTOP-5CAFU3F\\SQLEXPRESS;" +
												 "Database=MinionsDB;" +
												 "Integrated Security=true;";
		private static SqlConnection connection = new SqlConnection(connectionString);
		static void Main(string[] args)
		{
			string inputLine = Console.ReadLine();
			int[] minionsIDs = inputLine
				.Split(" ", StringSplitOptions.RemoveEmptyEntries)
				.Select(int.Parse)
				.ToArray();

			string updateQueryText = @"UPDATE Minions
										  SET Age += 1
										WHERE Id = @Id";
			connection.Open();
			using (connection)
			{
				SqlCommand command = new SqlCommand(updateQueryText, connection);
				for (int i = 0; i < minionsIDs.Length; i++)
				{
					command.Parameters.AddWithValue("@Id", minionsIDs[i]);
					command.ExecuteNonQuery();

					command.Parameters.Clear();

				}
				command.CommandText = "SELECT Name, Age FROM Minions";
				SqlDataReader reader = command.ExecuteReader();
				using(reader)
				{
					while(reader.Read())
					{
						Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
					}
				}
			}
		}
	}
}
