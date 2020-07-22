using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static string ResultDirectoryPath = "../../../Datasets/Results";
        public static void Main(string[] args)
        {

            CarDealerContext db = new CarDealerContext();

            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //    Directory.CreateDirectory(ResultDirectoryPath);
            //}


            string json = File.ReadAllText("../../../Datasets/cars.json");
           
            Console.WriteLine(ImportCars(db, json));
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
            List<PartCar> partsCar = new List<PartCar>();

            foreach (var carDto in carsDTO)
            {

                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance

                };

                cars.Add(car);

                foreach (var carPartId in carDto.PartsId.Distinct())
                {
                    PartCar carParts = new PartCar()
                    {
                        PartId = carPartId,
                        Car = car
                    };

                    partsCar.Add(carParts);

                }


            }
            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partsCar);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";

        }
    }
}