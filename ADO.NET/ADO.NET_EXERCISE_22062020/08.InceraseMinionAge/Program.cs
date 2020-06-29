using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _08IncreaseMinionAge
{
    class StartUp
    {
        const string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";

        static void Main()
        {
            int[] minionsIDs = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                UpdateMinionsInfo(connection, minionsIDs);

                PrintMinions(connection);
            }
        }

        private static void PrintMinions(SqlConnection connection)
        {
            string selectQuery = @"SELECT Name, Age FROM Minions";

            using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
            {
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
                    }
                }
            }
        }

        private static void UpdateMinionsInfo(SqlConnection connection, int[] minionsIDs)
        {
            string updateQuery = @" UPDATE Minions
                                       SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                     WHERE Id = @Id";

            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
            {
                foreach (var minion in minionsIDs)
                {
                    updateCommand.Parameters.AddWithValue("@Id", minion);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}