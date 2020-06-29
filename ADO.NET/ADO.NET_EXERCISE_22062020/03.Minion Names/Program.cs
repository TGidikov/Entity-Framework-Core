using Microsoft.Data.SqlClient;
using System;
using System.Text;

namespace _03.MinionNames
{
    class Program
    {
        private const string ConnectionString = @"Server=.;
                                                Database=MinionsDB;
                                                Integrated Security=true;";

        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            int Id = int.Parse(Console.ReadLine());

            string result = GetMinionsInfoAboutVillian(sqlConnection, Id);

        }

        private static string GetMinionsInfoAboutVillian(SqlConnection sqlConnection, int Id)
        {
            StringBuilder sb = new StringBuilder();

            var getVillianNameQueryText = @"SELECT Name FROM Villains WHERE Id = @Id";

            using SqlCommand getVillianName = new SqlCommand(getVillianNameQueryText, sqlConnection);

            getVillianName.Parameters.AddWithValue("@Id", Id);
            string villionName = getVillianName.ExecuteScalar()?.ToString();

            if (villionName == null)
            {
                sb.AppendLine($"No villain with ID {Id} exists in the database.");
            }
            else
            {
                sb.AppendLine($"Villain: {villionName}");

                string getMinisonsInfoQueryText = @"SELECT m.[Name],m.Age FROM Villains v
                                                    LEFT JOIN MinionsVillains mv
                                                    ON v.Id=mv.VillainId
                                                    LEFT JOIN Minions m
                                                    ON MV.MinionId=m.Id
                                                    WHERE v.[Name]= @villionName
                                                    Order BY M.[Name]";



                SqlCommand getMinionsInfoCommand = new SqlCommand
                    (getMinisonsInfoQueryText, sqlConnection);

                getMinionsInfoCommand.Parameters.AddWithValue("@villionName", villionName);

                using SqlDataReader reader = getVillianName.ExecuteReader();

                if (reader.HasRows)
                {
                    int rowNum = 1;
                    while (reader.Read())
                    {
                        string minionName = reader["Name"]?.ToString();
                        string minionAge = reader["Age"]?.ToString();

                        sb.AppendLine($"{rowNum}. {minionName} {minionAge}");
                        rowNum++;
                    }
                }
                else
                {
                    sb.AppendLine("(no minions)");
                }

            }
            return sb.ToString().TrimEnd();
        }
    }
}
