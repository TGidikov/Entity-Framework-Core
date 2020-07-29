using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Implort;
using CarDealer.Models;
using CarDealer.XmlHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            ///var Xml = File.ReadAllText("../../../Datasets/sales.xml");
            ///
            ///var result = ImportSales(context, Xml);
            ///
            ///Console.WriteLine(result);

            var result = GetSalesWithAppliedDiscount(context);
            File.WriteAllText("../../../Results/sales-discounts.xml", result);

        }
        //PROBLEM 09:
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            const string root = "Suppliers";

            var suppliersDTO = XMLConverter.Deserializer<ImportSupplierDTO>(inputXml, root);

            var result = suppliersDTO.Select(s => new Supplier
            {
                Name = s.Name,
                IsImporter = s.IsImporter
            })
            .ToList();


            context.Suppliers.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Count}";

        }

        //PROBLEM 10:
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string root = "Parts";

            var partsDTO = XMLConverter.Deserializer<ImportPartsDTO>(inputXml, root);

            var supplierIDs = context.Suppliers.Select(s => s.Id).ToArray();

            var result = partsDTO
                .Where(i => supplierIDs.Contains(i.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToArray();


            context.Parts.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Length}";

        }

        //PROBLEM 11:
        public static string ImportCars(CarDealerContext context, string inputXml)
        {

            const string root = "Cars";

            var carsDTO = XMLConverter.Deserializer<ImportCarsDTO>(inputXml, root);

            var realParts = context.Parts.Select(p => p.Id).ToArray();

            var cars = new List<Car>();

            foreach (var carDTO in carsDTO)
            {
                Car car = new Car();
                car.Make = carDTO.Make;
                car.Model = carDTO.Model;
                car.TravelledDistance = carDTO.TraveledDistance;


                var partsIds = carDTO.Parts.Select(p => p.Id).ToArray();

                foreach (var partId in partsIds.Distinct())
                {
                    if (realParts.Contains(partId))
                    {
                        PartCar partCar = new PartCar();
                        partCar.PartId = partId;
                        partCar.Car = car;
                        car.PartCars.Add(partCar);
                    }


                }
                cars.Add(car);

            }
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";

        }

        //PROBLEM 12:
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string root = "Customers";

            var customersDTO = XMLConverter.Deserializer<ImportCustomers>(inputXml, root);

            var customers = customersDTO.Select(c => new Customer
            {
                Name = c.Name,
                BirthDate = c.Birthdate,
                IsYoungDriver = c.IsYoungDriver
            })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        //PROBLEM 13:
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string root = "Sales";

            var salesDTO = XMLConverter.Deserializer<ImportSalesDTO>(inputXml, root);

            var existingCars = context.Cars.Select(c => c.Id).ToArray();

            var sales = new List<Sale>();

            foreach (var sale in salesDTO)
            {
                if (existingCars.Contains(sale.CarId))
                {
                    Sale realSale = new Sale();

                    realSale.CarId = sale.CarId;
                    realSale.CustomerId = sale.CustomerId;
                    realSale.Discount = sale.Discount;

                    sales.Add(realSale);

                }
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";

        }


        //PROBLEM 14:
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            const string rootElement = "cars";

            var cars = context.Cars
                .Select(c => new ExportCarsWithDistanceDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToList();

            var result = XMLConverter.Serialize(cars, rootElement);

            return result;
        }

        //PROBLEM 15:
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            const string rootElement = "cars";

            var cars = context.Cars
                .Where(c => c.Make == "BMW")
               .Select(c => new ExportBMWCarsDTO
               {
                   Id = c.Id,
                   Model = c.Model,
                   TravelledDistance = c.TravelledDistance
               })
               .OrderBy(c => c.Model)
               .ThenByDescending(c => c.TravelledDistance)
               .ToList();

            var result = XMLConverter.Serialize(cars, rootElement);

            return result;
        }

        //PROBLEM 16:
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            const string rootElement = "suppliers";

            var suppliers = context.Suppliers               
                .Where(s => !s.IsImporter)
                .Select(s => new ExportLocalSuplliersDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()

                })
                .ToList();



            var result = XMLConverter.Serialize(suppliers, rootElement);

            return result;

        }

        //PROBLEM 17:
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            const string rootElement = "cars";

            var cars = context.Cars
                .Select(c => new ExportCarsWithPartsDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts =c.PartCars.Select(pc => new ExportListWithParts
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(pc=>pc.Price)
                    .ToArray()
                })
                .OrderByDescending(c=>c.TravelledDistance)
                .ThenBy(c=>c.Model)
                .Take(5)
                .ToList();

            var result = XMLConverter.Serialize(cars, rootElement);

            return result;
        }

        //PROBLEM 18:
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {

            const string rootElement = "customers";

            var customers = context.Sales
                .Where(c => c.Customer.Sales.Count > 0)
                .Select(c => new ExportTotalSalesByCustomerDTO
                {
                    FullName = c.Customer.Name,
                    BoughtCars = c.Customer.Sales.Count,
                    SpendMoney = c.Car.PartCars.Sum(pc=>pc.Part.Price)
                })
                .OrderByDescending(c => c.SpendMoney)
                .ToList();
               


            var result = XMLConverter.Serialize(customers, rootElement);

            return result;
        }
        
        //PROBLEM 19:
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            const string rootElement = "sales";

            var sales = context.Sales
                .Select(s => new ExportSalesDTO
                {
                    Car = new ExportCarDTO
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount =s.Car.PartCars.Sum(pc =>    pc.Part.Price)- (s.Car.PartCars.Sum(pc => pc.Part.Price) * (s.Discount / 100))

                }).ToArray();
                





            var result = XMLConverter.Serialize(sales, rootElement);

            return result;


        }

    }
}