using System;
using System.Data.SqlClient;

namespace _02.Villain_Names
{
	class StartUp
	{
		private static string connectionString = "Server=DESKTOP-5CAFU3F\\SQLEXPRESS;" +
												 "Database=MinionsDB;" +
												 "Integrated Security=true;";
		static SqlConnection connection = new SqlConnection(connectionString);
		static void Main(string[] args)
		{
			try
			{
				connection.Open();
				string queryText = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
								    FROM Villains AS v 
								    JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
								GROUP BY v.Id, v.Name 
								  HAVING COUNT(mv.VillainId) > 3 
								ORDER BY COUNT(mv.VillainId)";
				using (connection)
				{
					SqlCommand command = new SqlCommand(queryText, connection);
					SqlDataReader reader = command.ExecuteReader();

					using(reader)
					{
						while(reader.Read())
						{
							Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
						}
					}
				}
				
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			
		}
	}
}
