using System;
using System.Data.SqlClient;

namespace _06.Remove_Villain
{
	class StartUp
	{
		private static string connectionString = "Server=DESKTOP-5CAFU3F\\SQLEXPRESS;" +
												 "Database=MinionsDB;" +
												 "Integrated Security=true;";
		private static SqlConnection connection = new SqlConnection(connectionString);

		private static SqlTransaction transaction;
		static void Main(string[] args)
		{
			connection.Open();

			int ID = int.Parse(Console.ReadLine());

			using (connection)
			{
				transaction = connection.BeginTransaction();
				try
				{
					SqlCommand command = new SqlCommand();
					command.Connection = connection;
					command.Transaction = transaction;
					command.CommandText = "SELECT Name FROM Villains WHERE Id = @villainId";
					command.Parameters.AddWithValue("@villainId", ID);

					object value = command.ExecuteScalar();
					if (value == null)
					{
						throw new ArgumentNullException(nameof(ID), "No such villain was found.");
					}
					string villainName = (string)value;

					command.CommandText = @"DELETE FROM MinionsVillains 
											 WHERE VillainId = @villainId";
					int minionsDeleted = command.ExecuteNonQuery();

					command.CommandText = @"DELETE FROM Villains
											 WHERE Id = @villainId";
					command.ExecuteNonQuery();

					transaction.Commit();
					Console.WriteLine($"{villainName} was deleted.");
					Console.WriteLine($"{minionsDeleted} minions were released.");
				}
				catch (ArgumentNullException ane)
				{
					try
					{
						Console.WriteLine(ane.Message);
						transaction.Rollback();
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				catch (Exception e)
				{
					try
					{
						Console.WriteLine(e.Message);
						transaction.Rollback();
					}
					catch (Exception re)
					{
						Console.WriteLine(re.Message);
					}
				}
			}
		}
	}
}
