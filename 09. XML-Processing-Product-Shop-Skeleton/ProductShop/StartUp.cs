using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XmlHelper;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Collections.Immutable;
using ProductShop.Dtos.Export;
using System.Runtime.CompilerServices;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var Xml = File.ReadAllText("../../../Datasets/categories-//products.xml");
            //
            //var result = ImportCategoryProducts(context,Xml);
            //
            //Console.WriteLine(result);

            var result = GetUsersWithProducts(context);
            File.WriteAllText("../../../Results/users-and-products.xml", result);
        }

        //PROBLEM 01:
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Users";
            var usersResults = XMLConverter.Deserializer<ImportUserDto>(inputXml, rootElement);

            //List<User> users = new List<User>();
            //
            //foreach (var importUser in usersResults)
            //{
            //    var user = new User
            //    {
            //        FirstName = importUser.FirstName,
            //        LastName = importUser.Lastname,
            //        Age = importUser.Age
            //    };
            //
            //    users.Add(user);
            //}

            var users = usersResults
                 .Select(u => new User
                 {
                     FirstName = u.FirstName,
                     LastName = u.Lastname,
                     Age = u.Age
                 })
                 .ToList();


            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //PROBLEM 02:
        public static string ImportProducts(ProductShopContext context, string inputXml)

        {
            const string rootElement = "Products";

            var productsResult = XMLConverter.Deserializer<ImportProductDto>(inputXml, rootElement);

            var products = productsResult.Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price,
                SellerId = p.SellerId,
                BuyerId = p.BuyerId

            })
             .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //PROBLEM 03:
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Categories";

            var categoriesResult = XMLConverter.Deserializer<ImportCategoryDto>(inputXml, rootElement);

            var categories = categoriesResult
                .Select(c => new Category
                {
                    Name = c.Name
                })
                .Where(c => c.Name != null)
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //PROBLEM 04:
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "CategoryProducts";

            var result = XMLConverter.Deserializer<ImportCategoryProductsDTO>(inputXml, rootElement);

            var categoriesAndProducts = result
                .Where(i =>
                context.Categories.Any(s => s.Id == i.CategoryId) &&
                context.Products.Any(s => s.Id == i.ProductId))
                .Select(cp => new CategoryProduct
                {
                    CategoryId = cp.CategoryId,
                    ProductId = cp.ProductId

                })
                .ToList();

            context.CategoryProducts.AddRange(categoriesAndProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesAndProducts.Count}";

        }

        //PROBLEM 05:
        public static string GetProductsInRange(ProductShopContext context)
        {
            const string rootElement = "Products";
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
               .Select(x => new ExportProductsDTO
               {
                   Name = x.Name,
                   Price = x.Price,
                   Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
               })
               .OrderBy(p => p.Price)
               .Take(10)
               .ToList();

            var result = XMLConverter.Serialize(products, rootElement);

            return result;
        }

        //PROBLEM 06:
        public static string GetSoldProducts(ProductShopContext context)
        {
            const string rootElement = "Users";

            var usersWithProducts = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(x => new ExportUserSoldProductDTO
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new UserProductDTO
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .Take(5)
                .ToList();



            var result = XMLConverter.Serialize(usersWithProducts, rootElement);

            return result;

        }

        //PROBLEM 07:
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {

            const string rootElement = "Categories";

            var categories = context.Categories
                  .Select(c => new ExportCategoriesByProductsCountDTO
                  {
                      Name = c.Name,
                      Count = c.CategoryProducts.Count,
                      AveragePrice =
                      c.CategoryProducts.Select(x => x.Product).Average(p => p.Price),
                      TotalRevenue =
                      c.CategoryProducts.Select(x => x.Product).Sum(p => p.Price)
                  })
                  .OrderByDescending(c => c.Count)
                  .ThenBy(c => c.TotalRevenue)
                  .ToList();




            var result = XMLConverter.Serialize(categories, rootElement);

            return result;

        }

        //PROBLEM 08:
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            const string rootElement = "Users";

            var usersAndProducts = context.Users
                .ToArray()
                .Where(u => u.ProductsSold.Any())
                .Select(ex => new ExportUserDTO
                {
                    FirstName = ex.FirstName,
                    LastName = ex.LastName,
                    Age = ex.Age,
                    SoldProduct = new ExportProductCountDTO
                    {
                        Count = ex.ProductsSold.Count,
                        Products = ex.ProductsSold.Select(p => new ExportProductDTO
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }

                })
                .OrderByDescending(p => p.SoldProduct.Count)
                .Take(10)
                .ToArray();

            var resultsDTO = new ExportUserCountDTO
            {
                Count = context.Users.Count(p => p.ProductsSold.Any()),
                Users = usersAndProducts
            };


            var result = XMLConverter.Serialize(resultsDTO, rootElement);

            return result;


        }

    }


}
