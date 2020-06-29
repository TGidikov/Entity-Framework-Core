using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace _04.AddMinion
{
   public  class StartUp
    {
        private const string ConnectionString = @"Server=.;
                                                Database=MinionsDB;
                                                Integrated Security=true;";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnction = new SqlConnection(ConnectionString);

            sqlConnction.Open();

            string[] minionsInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] minionsInfo = minionsInput[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] villianInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string[] villianInfo = villianInput[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string result = AddMinnionToDatabase(sqlConnction, minionsInfo, villianInfo);

            Console.WriteLine(result);


        }

        private static string AddMinnionToDatabase(SqlConnection sqlConnction, string[] minionsInfo, string[] villianInfo)
        {
            StringBuilder output = new StringBuilder();
            
            string minionName = minionsInfo[0];
            string minionAge = minionsInfo[1];
            string minionTown = minionsInfo[2];
            
            string villainName = villianInfo[0];

            string townId = EnsureTownExist(sqlConnction, minionTown,output);

            string villainId = EnsureVillainExists(sqlConnction, villainName, output);

            string insertMinionQueryText = @"INSERT INTO Minions([Name],Age,TownId)
                                            VALUES (@minionName,@minionAge,@townId)";
            using SqlCommand insertMinionCmd = new SqlCommand(insertMinionQueryText, sqlConnction);

            insertMinionCmd.Parameters.AddRange(new[]
            {
                new SqlParameter(@"minionName",minionName),
                new SqlParameter(@"minionAge",minionAge),
                new SqlParameter(@"townId",townId)

            });

            insertMinionCmd.ExecuteNonQuery();
            string getMinionIdQueryText = @"SELECT Id FROM Minions WHERE [Name]=@minionName";

            using SqlCommand getMinionIdCmd = new SqlCommand(getMinionIdQueryText, sqlConnction);

            getMinionIdCmd.Parameters.AddWithValue("@minionName", minionName);
            string minionId = getMinionIdCmd.ExecuteScalar().ToString();

            string insertIntoMappingQueryText = @"INSERT INTO MinionsVillains(minionId,villainId)
                                                    VALUES(@minionId,@villainId)";

            using SqlCommand insertIntoMappingCmd = new SqlCommand(insertIntoMappingQueryText, sqlConnction);
            insertIntoMappingCmd.Parameters.AddRange(new[]
            {
                new SqlParameter("@minionId",minionId),
                new SqlParameter("@villainId",villainId)
            });

            insertIntoMappingCmd.ExecuteNonQuery();
            output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

            return output.ToString().TrimEnd();
        }

        private static string EnsureVillainExists(SqlConnection sqlConnction, string villainName, StringBuilder output)
        {
            string getVillainIdQueryText = @"SELECT Id FROM Villains WHERE [Name]= @name";

            using SqlCommand getVillainIdCmd = new SqlCommand(getVillainIdQueryText, sqlConnction);
            getVillainIdCmd.Parameters.AddWithValue("@name", villainName);

            string villainId = getVillainIdCmd.ExecuteScalar()?.ToString();

            if (villainId== null)
            {
                string getFactroIdQueryText = @"SELECT Id FROM EvilnessFactors WHERE [Name]='Evil'";

                using SqlCommand getFactorIdCmd = new SqlCommand(getFactroIdQueryText, sqlConnction);

                string factorId = getFactorIdCmd.ExecuteScalar()?.ToString();

                string insertVillainQueryText = @"INSERT INTO Villains([Name],EvilnessFactorId)
                                                 VALUES(@VillainName,@factorId)";

                using SqlCommand insertVillianCmd = new SqlCommand(insertVillainQueryText, sqlConnction);
                insertVillianCmd.Parameters.AddWithValue("@villainName", villainName);
                insertVillianCmd.Parameters.AddWithValue("@factorId", factorId);

                insertVillianCmd.ExecuteNonQuery();

                villainId = getVillainIdCmd.ExecuteScalar().ToString();
                output.AppendLine($"Villain {villainName} was added to the database.");
            }
            return villainId;
        }

        private static string EnsureTownExist(SqlConnection sqlConnction, string minionTown,StringBuilder output)
        {
            string getTownIdQueryText= @"SELECT Id FROM Towns WHERE [Name]=@townName";

            using SqlCommand getTownIdCmd = new SqlCommand(getTownIdQueryText, sqlConnction);

            getTownIdCmd.Parameters.AddWithValue("@townName", minionTown);

            string townId = getTownIdCmd.ExecuteScalar().ToString();

            if (townId==null)
            {
                string insertTownQueryText = @"INSERT INTO Towns([Name],CountryCode)
                                             VALUES (@townName,1)";

                using SqlCommand insertTownCmd = new SqlCommand(insertTownQueryText, sqlConnction);

                insertTownCmd.Parameters.AddWithValue("@townName", minionTown);

                insertTownCmd.ExecuteNonQuery();

                townId = getTownIdCmd.ExecuteScalar()?.ToString();

                output.AppendLine($"Town {minionTown} was added to the database.");

            }
            return townId;
        }
    }
}
