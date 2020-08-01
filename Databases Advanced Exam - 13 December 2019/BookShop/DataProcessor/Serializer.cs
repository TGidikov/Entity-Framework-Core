namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Castle.DynamicProxy.Generators;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var mostCraziestAuthor = context
                .Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + ' ' + a.LastName,
                    Books = a.AuthorsBooks
                    .OrderByDescending(ab => ab.Book.Price)
                    .Select(b => new
                    {
                        BookName = b.Book.Name,
                        BookPrice = b.Book.Price.ToString("f2")
                    })
                })
                .ToArray()
                .OrderByDescending(a => a.Books.Count())
                .ThenBy(a => a.AuthorName);

            string json = JsonConvert.SerializeObject(mostCraziestAuthor, Formatting.Indented);
            return json;


        }
        //      [
        //{
        //  "AuthorName": "Angelina Tallet",
        //  "Books": [
        //    {
        //      "BookName": "Allen Fissidens Moss",
        //      "BookPrice": "78.44"
        //    },
        //    {
        //      "BookName": "Earlyleaf Brome",
        //      "BookPrice": "63.66"
        //    },
        //    {
        //      "BookName": "Sky Mousetail",
        //      "BookPrice": "13.14"
        //    },
        //    {
        //      "BookName": "Arrowleaf Clover",
        //      "BookPrice": "1.71"
        //    }
        //  ]
        //},

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var sb = new StringBuilder();
            var books = context
                .Books
                .Where(b => b.PublishedOn < date && b.Genre == Data.Models.Enums.Genre.Science)
                .ToArray()
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn)
                .Take(10)
                .Select(b => new ExportOldestBooks
                {
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = b.Pages
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportOldestBooks[]), new XmlRootAttribute("Books"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter stringWriter = new StringWriter(sb))
            {
                xmlSerializer.Serialize(stringWriter, books, namespaces);

            }
            return sb.ToString().TrimEnd();
        }
    }
}
//<? xml version="1.0" encoding="utf-16"?>
//<Books>
//<Book Pages = "4881" >
//< Name > Sierra Marsh Fern</Name>
//<Date>03/18/2016</Date>
//</Book>
