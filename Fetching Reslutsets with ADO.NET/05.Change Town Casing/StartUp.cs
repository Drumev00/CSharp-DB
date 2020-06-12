using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _05.Change_Town_Casing
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
			string countryName = Console.ReadLine();
			using (connection)
			{
				string findTowns = @"SELECT t.Name 
									   FROM Towns as t
									   JOIN Countries AS c ON c.Id = t.CountryCode
									  WHERE c.Name = @countryName";
				SqlCommand command = new SqlCommand(findTowns, connection);
				command.Parameters.AddWithValue("@countryName", countryName);
				SqlDataReader reader = command.ExecuteReader();
				List<string> townNames = new List<string>();
				using (reader)
				{
					while (reader.Read())
					{
						townNames.Add((string)reader["Name"]);
					}
				}
				string townsToUpper = @"UPDATE Towns
											   SET Name = UPPER(Name)
											 WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";
				command.CommandText = townsToUpper;


				int rowsAffected = command.ExecuteNonQuery();

				Console.WriteLine($"{rowsAffected} town names were affected.");
				Console.Write("[" + string.Join(", ", townNames) + "]");
			}
		}
	}
}
