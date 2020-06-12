using System;
using System.Data.SqlClient;

namespace _03.Minion_Names
{
	class StartUp
	{
		private static string connectionString = "Server=DESKTOP-5CAFU3F\\SQLEXPRESS;" +
												 "Database=MinionsDB;" +
												 "Integrated Security=true;";
		static SqlConnection connection = new SqlConnection(connectionString);
		static void Main(string[] args)
		{
			connection.Open();
			int id = int.Parse(Console.ReadLine());
			string findVillain = "SELECT Name FROM Villains WHERE Id = @Id";

			string findMinions = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

			using(connection)
			{
				try
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.Parameters.AddWithValue("@Id", id);
					command.CommandText = findVillain;

					string villainName = (string)command.ExecuteScalar();
					if (villainName == null)
					{
						Console.WriteLine($"No villain with ID {id} exists in the database.");
						Environment.Exit(0);
					}
					else
					{
						Console.WriteLine($"Villain: {villainName}");
					}

					command.CommandText = findMinions;
					int minionsServeToVillain = command.ExecuteNonQuery();
					if (minionsServeToVillain == 0)
					{
						Console.WriteLine("(no minions)");
						Environment.Exit(0);
					}

					SqlDataReader reader = command.ExecuteReader();
					using(reader)
					{
						while(reader.Read())
						{
							Console.WriteLine($"{reader["RowNum"]}. {reader["Name"]} {reader["Age"]}");
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
}
