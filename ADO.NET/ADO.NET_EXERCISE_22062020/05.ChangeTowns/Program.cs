using System;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace _05.ChangeTownsNameCasing
{
    class Program
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true;";
        static void Main()
        {
            var sqlConnection = new SqlConnection(ConnectionString);

            sqlConnection.Open();

            string countryNameInput = Console.ReadLine();

            string result = UpperCaseToTownsInCountry(sqlConnection, countryNameInput);

            Console.WriteLine(result);
        }

        private static string UpperCaseToTownsInCountry(SqlConnection sqlConnection, string countryNameInput)
        {
            string getCountryIdQueryString = @"SELECT Id FROM Countries
                                              WHERE [Name] = @countryName";
            using var getCountryIdCmd = new SqlCommand(getCountryIdQueryString, sqlConnection);
            getCountryIdCmd.Parameters.AddWithValue("@countryName", countryNameInput);

            string countryId = getCountryIdCmd.ExecuteScalar()?.ToString();

            var sb = new StringBuilder();

            if (countryId == null)
            {
                sb.AppendLine("No town names were affected.");

                return sb.ToString().TrimEnd();
            }

            string updateTownsToUpperQueryString = @"UPDATE Towns
                                                             SET [Name] = UPPER([Name])
                                                             WHERE CountryCode = @countryId";
            using var updateTownsToUpper = new SqlCommand(updateTownsToUpperQueryString, sqlConnection);
            updateTownsToUpper.Parameters.AddWithValue("@countryId", countryId);

            int townsCount = updateTownsToUpper.ExecuteNonQuery();

            if (townsCount == 0)
            {
                sb.AppendLine("No town names were affected.");

                return sb.ToString().TrimEnd();
            }
            else
            {
                sb.AppendLine($"{townsCount} town names were affected");
            }

            string getTownsByCountryIdQueryString = @"SELECT [Name] FROM Towns
                                                     WHERE CountryCode = @countryId";
            using var getTownsByCountryIdCmd = new SqlCommand(getTownsByCountryIdQueryString, sqlConnection);
            getTownsByCountryIdCmd.Parameters.AddWithValue("@countryId", countryId);

            using var reader = getTownsByCountryIdCmd.ExecuteReader();
            var townsNames = new List<string>();

            while (reader.Read())
            {
                townsNames.Add(reader["Name"].ToString());
            }

            sb.AppendLine($"[{string.Join(", ", townsNames)}]");

            return sb.ToString().TrimEnd();
        }
    }
}
