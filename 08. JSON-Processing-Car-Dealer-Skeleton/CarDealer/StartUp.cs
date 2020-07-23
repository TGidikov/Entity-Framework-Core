using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static string ResultDirectoryPath = "../../../Datasets/Results";
        public static void Main(string[] args)
        {

            CarDealerContext db = new CarDealerContext();

            //Serialize:

            if (!Directory.Exists(ResultDirectoryPath))
            {
                Directory.CreateDirectory(ResultDirectoryPath);
            }
            string json = GetSalesWithAppliedDiscount(db);
            File.WriteAllText(ResultDirectoryPath + "/sales-discounts.json", json);

            //Deserialize:
            // string json = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(db, json));
        }
        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("db was deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("db was created");
        }

        //PROBLEM 10:
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";

        }

        //PROBLEM 11:
        public static string ImportParts(CarDealerContext context, string inputJson)
        {


            var parts = JsonConvert.DeserializeObject<List<Part>>
            (inputJson).Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId)).ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";

        }

        //PROBLEM 12:
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            List<CarDTO> carsDTO = JsonConvert.DeserializeObject<List<CarDTO>>(inputJson);

            List<Car> cars = new List<Car>();
            List<PartCar> carParts = new List<PartCar>();

            foreach (var carDto in carsDTO)
            {
                Car car = new Car
                {
                    Make = carDto.Make,

                    Model = carDto.Model,

                    TravelledDistance = carDto.TravelledDistance,

                };
                cars.Add(car);
                foreach (var carPartId in carDto.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar
                    {
                        PartId = carPartId,
                        Car = car

                    };
                    carParts.Add(partCar);
                }
            }
            context.Cars.AddRange(cars);
            context.PartCars.AddRange(carParts);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";

        }

        //PROBLEM 13:
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson)
               .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //PROBLEM 14:
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson)
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";


        }

        //PROBLEM 15:
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();


            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return json;
        }

        //PROBLEM 16:
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            return json;

        }

        //PROBLEM 17:
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suplliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count

                })
                .ToList();

            var json = JsonConvert.SerializeObject(suplliers, Formatting.Indented);
            return json;

        }

        //PROBLEM 18:
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    },
                    parts = c.PartCars
                     .Select(p => new
                     {
                         p.Part.Name,
                         Price = p.Part.Price.ToString("F2")
                     }).ToList()
                }).ToList();

            string json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }
        //PROBLEM 19:
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Sum(y => y.Car.PartCars.Sum(x => x.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }
        //PROBLEM 20:
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(c => new

                {
                    car = new
                    {
                        c.Car.Make,
                        c.Car.Model,
                        c.Car.TravelledDistance
                    },

                    customerName = c.Customer.Name,
                    Discount=c.Discount.ToString("F2"),
                    price = c.Car.PartCars.Sum(x => x.Part.Price).ToString("F2"),
                    priceWithDiscount = ((c.Car.PartCars.Sum(x => x.Part.Price) - (c.Car.PartCars.Sum(x => x.Part.Price) * c.Discount / 100))).ToString("F2")

                })
                .ToList();

            string json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;

        }

    }
}