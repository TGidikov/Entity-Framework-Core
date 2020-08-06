namespace Cinema.DataProcessor
{
    using System;
    using System.Linq;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using XmlHelper;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {

            var topMovies = context
                .Movies
                .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Count > 0))
                 .OrderByDescending(r => r.Rating)
                .ThenByDescending
                (p => p.Projections
                .Sum(t => t.Tickets
                .Sum(pt => pt.Price)))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = m
                    .Projections
                    .SelectMany(t => t.Tickets).Select(c => new
                    {
                        FirstName = c.Customer.FirstName,
                        LastName = c.Customer.LastName,
                        Balance=c.Customer.Balance.ToString("F2")
                    })

                    .OrderByDescending(c => c.Balance)
                    .ThenBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)  
                    .ToArray()

                })
                .Take(10)
                .ToArray();


            string json = JsonConvert.SerializeObject(topMovies, Formatting.Indented);
            return json;
        }
        
  //{
  //  "MovieName": "SIS",
  //  "Rating": "10.00",
  //  "TotalIncomes": "184.04",
  //  "Customers": [
  //    {
  //      "FirstName": "Davita",
  //      "LastName": "Lister",
  //      "Balance": "279.76"
  //    },
  //    {
  //      "FirstName": "Arluene",
  //      "LastName": "Farman",
  //      "Balance": "118.33"
  //    {


        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var topCustomers = context
                .Customers
                .Where(c => c.Age >= age)                  
                 .OrderByDescending(c => c.Tickets.Sum(p=>p.Price))
                 .Take(10)
                .Select(c => new ExportTopCustomers
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromSeconds(c.Tickets.Sum(t=>t.Projection.Movie.Duration.TotalSeconds)).ToString(@"hh\:mm\:ss")

                })               
                .ToArray();


            


            var result = XMLConverter.Serialize(topCustomers, "Customers");

            return result;
        }
    }
}
//<Customers>
//  <Customer FirstName = "Marjy" LastName="Starbeck">
//    <SpentMoney>82.65</SpentMoney>
//    <SpentTime>17:04:00</SpentTime>
//  </Customer>
//  <Customer FirstName = "Jerrie" LastName="O\'Carroll">
//    <SpentMoney>67.13</SpentMoney>
//    <SpentTime>13:40:00</SpentTime>
//  </Customer>
//  <Customer FirstName = "Randi" LastName="Ferraraccio">
//    <SpentMoney>63.39</SpentMoney>
//    <SpentTime>17:42:00</SpentTime>
//  </Customer>...
//</Customers>
