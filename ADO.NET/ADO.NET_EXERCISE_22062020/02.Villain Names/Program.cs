using Microsoft.Data.SqlClient;
using System;

namespace _02.VillainNames
{
    class Program
    {
        private static string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";
       
        static void Main()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                string selectQuery = @"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                           FROM Villains AS v 
                                           JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                       GROUP BY v.Id, v.Name 
                                         HAVING COUNT(mv.VillainId) > 3 
                                       ORDER BY COUNT(mv.VillainId)";

                SqlCommand command = new SqlCommand(selectQuery, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

    }
    }
}
