namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using ImportDto;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportBookDTO[]), new XmlRootAttribute("Books"));
           //FIRST
            //var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));

            //using StringReader stringReader = new StringReader(xmlString);
            using (StringReader stringReader = new StringReader(xmlString))
            {
                ImportBookDTO[] bookDTOs = (ImportBookDTO[])xmlSerializer.Deserialize(stringReader);

                List<Book> booksToAdd = new List<Book>();

                StringBuilder sb = new StringBuilder();

                foreach (var bookDto in bookDTOs)
                {
                    if (!IsValid(bookDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime publishedOn;
                   
                    bool isDateValid = DateTime.TryParseExact(bookDto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None,out publishedOn);

                    if (!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var book = new Book
                    {
                        Name = bookDto.Name,
                        Genre = (Genre)bookDto.Genre,
                        Pages = bookDto.Pages,
                        Price = bookDto.Price,
                        PublishedOn = publishedOn
                    };

                    booksToAdd.Add(book);
                    sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
                }

                context.Books.AddRange(booksToAdd);
                context.SaveChanges();

               return sb.ToString().TrimEnd();

            }


        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            ImportAuthorDto[] authorDtos = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            List<Author> authors = new List<Author>();

            foreach (var authorDto in authorDtos)
            {
                if (!IsValid(authorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (authors.Any(a=>a.Email==authorDto.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var author = new Author
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Email = authorDto.Email,
                    Phone = authorDto.Phone
                };

                //var uniqueBookIds = authorDto.Books.Distinct();

                foreach (var bookDto in authorDto.Books)
                {
                  
                    if (!bookDto.BookId.HasValue)
                    {
                        continue;
                    }

                    Book book = context.Books.FirstOrDefault(b => b.Id == bookDto.BookId);
                    
                    if (book == null)
                    {
                        continue;
                    }

                    author.AuthorsBooks.Add(new AuthorBook
                    {
                        Author = author,
                        Book = book
                    });
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                authors.Add(author);
                sb.AppendLine(string.Format(SuccessfullyImportedAuthor, (author.FirstName + " " + author.LastName), author.AuthorsBooks.Count));
            }

            context.Authors.AddRange(authors);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

            
        }

       // private static T[] DeserializeObject<T>(string rootElement, string //xmlString)
       // {
       //     var xmlSerializer = new XmlSerializer(typeof(T[]), new //XmlRootAttribute(rootElement));
       //     var deserializedDtos = (T[])xmlSerializer.Deserialize(new //StringReader(xmlString));
       //     return deserializedDtos;
       // }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}