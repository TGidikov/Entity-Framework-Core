using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static string ResultDirectoryPath = "../../../Datasets/Results";
        public static void Main(string[] args)
        {

            ProductShopContext db = new ProductShopContext();
           

           // string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");
            
            


            string json =GetProductsInRange(db);

            if (!Directory.Exists(ResultDirectoryPath))
            {
                Directory.CreateDirectory(ResultDirectoryPath);
            }
            File.WriteAllText(ResultDirectoryPath + "/products-in-range.json", json);
        }
        private static void ResetDatabase(ProductShopContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("db was deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("db was created");
        }
        //PROBLEM 02:
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            List<User> users=JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //PROBLEM 03:
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(inputJson);
            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
        //PROBLEM 04:
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
           


            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson).Where(c=>c.Name!=null).ToList();
           
            context.AddRange(categories);
           
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        //PROBLEM 05:
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";

        }
        //PROBLEM 06:
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context                
                .Products               
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name =p.Name,
                    price =p.Price,
                    seller =p.Seller.FirstName+' '+p.Seller.LastName
                    
                }).ToArray();
           
           string json= JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

    }
}