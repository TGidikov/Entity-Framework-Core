namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
           // DbInitializer.ResetDatabase(db);


            //string input = Console.ReadLine();

            string result = GetTotalProfitByCategory(db);
            Console.WriteLine(result);
        }
       
        //PROBLEM 02:
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            var bookTitles = context
                .Books
                .AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(bt => bt)
                .ToList();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }
            return sb.ToString().TrimEnd();
            //return string.Join(Environment.NewLine, bookTitles);
        }
       
        //PROBLEM 03:
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var goldenBooks = context
                .Books
                .Where(b => b.Copies < 5000 && b.EditionType==EditionType.Gold)   
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();
            foreach (var b in goldenBooks)
            {
                sb.AppendLine(b.Title);
            }
            return sb.ToString().TrimEnd();

        }
       
        //PROBLEM 04:
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var booksByPrice = context
                 .Books
                 .Where(b => b.Price > 40)
                 .Select(b => new
                 {
                     b.Price,
                     b.Title

                 })
                 .OrderByDescending(b => b.Price)
                 .ToList();

           
            foreach (var b in booksByPrice)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");
            }
            return sb.ToString().TrimEnd().TrimStart();
        }
       
        //PROBLEM 05:
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var booksCollection = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                    
                })
                .OrderBy(b=>b.BookId)
                .ToList();

            foreach (var b in booksCollection)
            {
                sb.AppendLine(b.Title);
            }
            
            return sb.ToString().TrimEnd();
        }
        
        //PROBLEM 06:
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var categories = input.ToLower()
                .Split(' ',StringSplitOptions.RemoveEmptyEntries)               
                .ToList();
            var result = new List<string>();
            foreach (var c in categories)
            {
                var booksByCategory = context
               .Books
               .Where(b => b.BookCategories.Any(b => b.Category.Name.ToLower() == c))
               .Select(b => b.Title)
               .ToList();

                result.AddRange(booksByCategory);

            };

            var finalResult=result.OrderBy(r => r).ToList();
      
            foreach (var b in finalResult)
            {
                sb.AppendLine(b);
            }
            return sb.ToString().TrimEnd();
        }
        
        //PROBLEM 07:
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            var  dt = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context
                .Books
                .Where(b => b.ReleaseDate < dt)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();



            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }
            return sb.ToString().TrimEnd().TrimStart();
        }

        //PROBLEM 08:
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + ' ' + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var a in authors)
            {
                sb.AppendLine(a.FullName);
            }
            return sb.ToString().TrimEnd().TrimStart();



        }

        //PROBLEM 09:
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            var sb = new StringBuilder();
            foreach (var b in books)
            {
                sb.AppendLine(b);
            }
            return sb.ToString().TrimEnd().TrimStart();
        }

        //PROBLEM 10:
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    AuthorName = b.Author.FirstName + ' ' + b.Author.LastName,

                })
                .OrderBy(b=>b.BookId)
                .ToList();



            var sb = new StringBuilder();
            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} ({b.AuthorName})");
            }
            return sb.ToString().TrimEnd().TrimStart();

        }

        //PROBLEM 11:
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();
            return books;
        }

        //PROBLEM 12:
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsCopies = context.Authors
                .Select(a => new
                {

                    FullName = a.FirstName + ' ' + a.LastName,
                    BookCopies = a.Books
                    .Select(b => b.Copies)
                    .Sum()
                })
                .OrderByDescending(a => a.BookCopies)
                .ToList();

            var sb = new StringBuilder();
            foreach (var a in authorsCopies)
            {
                sb.AppendLine($"{a.FullName} - {a.BookCopies}");
            }
            return sb.ToString().TrimEnd().TrimStart();
        }

        //PROBLEM 13:
        public static string GetTotalProfitByCategory(BookShopContext context)
        {

            var categoryProfits = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Select(cb => new
                    {

                        BookProfit = cb.Book.Copies * cb.Book.Price
                    }).Sum(cb=>cb.BookProfit)

                })
                .OrderByDescending(c=>c.TotalProfit)
                .ThenBy(c=>c.Name)
                .ToList();


            var sb = new StringBuilder();
            foreach (var c in categoryProfits)
            {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:f2}");
            }
            return sb.ToString().TrimEnd().TrimStart();

        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var sb = new StringBuilder();
            var categoriesWithMostRecentBooks = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    MostRB = c.CategoryBooks
                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(cb => new
                    {
                        BookTitle = cb.Book.Title,
                        ReleaseYEare = cb.Book.ReleaseDate.Value.Year

                    }).ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            foreach (var c in categoriesWithMostRecentBooks)
            {
                sb.AppendLine($"--{c.CategoryName}");
                foreach (var b in c.MostRB)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.ReleaseYEare})");
                }
            }

            return sb.ToString().TrimEnd();
        }





    }
}
