namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Microsoft.SqlServer.Server;
    using Newtonsoft.Json;
    using XmlHelper;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDto = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            List<Movie> movies = new List<Movie>();

            foreach (var movie in moviesDto)
            {
                if (!IsValid(movie))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (movies.Any(m => m.Title == movie.Title))
                {

                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidEnum = Enum.TryParse(typeof(Genre), movie.Genre, out object genre);


                movies.Add(new Movie
                {
                    Title = movie.Title,
                    Genre = (Genre)genre,
                    Duration = movie.Duration,
                    Rating = movie.Rating,
                    Director = movie.Director
                });


                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre.ToString(), movie.Rating.ToString("F2")));
            }

            context.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();


        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var hallDtos = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);

            var halls = new List<Hall>();


            foreach (var hallDto in hallDtos)
            {
                if (!IsValid(hallDto)
                    || hallDto.Seats <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var validHall = new Hall
                {
                    Name = hallDto.Name,
                    Is4Dx = hallDto.Is4Dx,
                    Is3D = hallDto.Is3D,
                };
                for (int i = 0; i < hallDto.Seats; i++)
                {
                    validHall.Seats.Add(new Seat());
                }

                halls.Add(validHall);

                string status = "";

                if (validHall.Is3D)
                {
                    status = "3D";
                   
                    if (validHall.Is4Dx)
                    {
                        status = "4Dx/3D";
                    }

                }
                else if (validHall.Is4Dx)
                {
                    status = "4Dx";
                }
                else
                {
                    status = "Normal";
                }

                sb.AppendLine(String.Format(SuccessfulImportHallSeat, validHall.Name, status, validHall.Seats.Count));

            }
            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            const string rootElement = "Projections";

            var result = XMLConverter.Deserializer<ImportProjectionDto>(xmlString, rootElement);

            var projections = new List<Projection>();

            foreach (var projectionDto in result)
            {
                var validMovie = context.Movies.FirstOrDefault(m => m.Id == projectionDto.MovieId);

                var validHall = context.Halls.FirstOrDefault(h => h.Id == projectionDto.HallId);

                if (validHall==null || validMovie==null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
               
                var dateTime = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var projection = new Projection
                {
                    Movie = validMovie,
                    Hall = validHall,
                    DateTime = dateTime
                };

                projections.Add(projection);
                sb.AppendLine(String.Format(SuccessfulImportProjection, validMovie.Title, projection.DateTime.ToString("MM/dd/yyyy")));

            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            return sb.ToString().TrimEnd();


        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            const string rootElement = "Customers";

            var result = XMLConverter.Deserializer<ImportCustomerTDto>(xmlString, rootElement);

            var customers = new List<Customer>();

            foreach (var customerDto in result)
            {
                if (!IsValid(customerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = new Customer
                {
                    FirstName=customerDto.FirstName,
                    LastName=customerDto.LastName,
                    Age=customerDto.Age,
                    Balance=customerDto.Balance
                };

                foreach (var ticketDto in customerDto.Tickets)
                {
                    var projection = context.Projections.FirstOrDefault(p => p.Id == ticketDto.ProjectionId);

                    var ticket = new Ticket
                    {
                        Projection = projection,
                        Price = ticketDto.Price
                    };

                    customer.Tickets.Add(ticket);

                }
                customers.Add(customer);
                sb.AppendLine(String.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            var result = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return result;
        }
    }
}