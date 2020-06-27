using System;
using System.Collections.Generic;
using System.Globalization;
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
		public static void Main(string[] args)
		{
			var db = new CarDealerContext();

			//string inputJson = File.ReadAllText("./../../../Datasets/sales.json");

			Console.WriteLine(GetSalesWithAppliedDiscount(db));
		}
		//Problem 09
		public static string ImportSuppliers(CarDealerContext context, string inputJson)
		{
			var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

			context.Suppliers.AddRange(suppliers);
			context.SaveChanges();

			return $"Successfully imported {suppliers.Length}.";
		}
		//Problem 10
		public static string ImportParts(CarDealerContext context, string inputJson)
		{
			var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
				.Where(p => p.SupplierId <= 31)
				.ToArray();

			context.Parts.AddRange(parts);
			context.SaveChanges();

			return $"Successfully imported {parts.Length}.";
		}
		//Problem 11
		public static string ImportCars(CarDealerContext context, string inputJson)
		{
			var carsinput = JsonConvert.DeserializeObject<CarDTO[]>(inputJson);

			foreach (var carDto in carsinput)
			{
				Car car = new Car
				{
					Id = carDto.Id,
					Make = carDto.Make,
					Model = carDto.Model,
					TravelledDistance = carDto.TravelledDistance
				};

				context.Cars.Add(car);

				foreach (var partId in carDto.PartsId)
				{
					var partCar = new PartCar
					{
						PartId = partId,
						CarId = car.Id
					};

					if (car.PartCars.FirstOrDefault(c => c.PartId == partId) == null)
					{
						context.PartCars.Add(partCar);
					}
				}
			}
			context.SaveChanges();

			return $"Successfully imported {carsinput.Length}.";
		}
		//Problem 12
		public static string ImportCustomers(CarDealerContext context, string inputJson)
		{
			var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

			context.Customers.AddRange(customers);
			context.SaveChanges();

			return $"Successfully imported {customers.Length}.";
		}
		//Problem 13
		public static string ImportSales(CarDealerContext context, string inputJson)
		{
			var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

			context.Sales.AddRange(sales);
			context.SaveChanges();

			return $"Successfully imported {sales.Length}.";
		}
		//Problem 14
		public static string GetOrderedCustomers(CarDealerContext context)
		{
			var customers = context.Customers
				.OrderBy(c => c.BirthDate.Year)
				.ThenBy(c => c.BirthDate.Month)
				.ThenBy(c => c.BirthDate.Day)
				.ThenBy(c => c.IsYoungDriver)
				.Select(c => new
				{
					Name = c.Name,
					BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
					IsYoungDriver = c.IsYoungDriver
				})
				.ToList();

			var jsonCustomers = JsonConvert.SerializeObject(customers, Formatting.Indented);

			return jsonCustomers;
		}
		//Problem 15
		public static string GetCarsFromMakeToyota(CarDealerContext context)
		{
			var cars = context.Cars
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

			var jsonCars = JsonConvert.SerializeObject(cars, Formatting.Indented);

			return jsonCars;
		}
		//Problem 16
		public static string GetLocalSuppliers(CarDealerContext context)
		{
			var suppliers = context.Suppliers
				.Where(s => !s.IsImporter)
				.Select(s => new
				{
					Id = s.Id,
					Name = s.Name,
					PartsCount = s.Parts.Count
				})
				.ToList();

			var jsonSuppliers = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

			return jsonSuppliers;
		}
		//Problem 17
		public static string GetCarsWithTheirListOfParts(CarDealerContext context)
		{
			var carsAndParts = context.Cars
				.Select(c => new
				{
					car = new
					{
						Make = c.Make,
						Model = c.Model,
						TravelledDistance = c.TravelledDistance
					},

					parts = c.PartCars
					.Select(pc => new
					{
						Name = pc.Part.Name,
						Price = pc.Part.Price.ToString("f2")
					})
					.ToList()
				})
				.ToList();

			var jsonCars = JsonConvert.SerializeObject(carsAndParts, Formatting.Indented);

			return jsonCars;
		}
		//Problem 18
		public static string GetTotalSalesByCustomer(CarDealerContext context)
		{
			var customers = context.Customers
				.Where(c => c.Sales.Count > 0)
				.Select(c => new
				{
					fullName = c.Name,
					boughtCars = c.Sales.Count,
					spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
				})
				.OrderByDescending(c => c.spentMoney)
				.ThenByDescending(c => c.boughtCars)
				.ToList();

			var jsonCustomers = JsonConvert.SerializeObject(customers, Formatting.Indented);

			return jsonCustomers;
		}
		//Problem 19
		public static string GetSalesWithAppliedDiscount(CarDealerContext context)
		{
			var sales = context.Sales
				.Select(s => new
				{
					car = new
					{
						Make = s.Car.Make,
						Model = s.Car.Model,
						TravelledDistance = s.Car.TravelledDistance
					},

					customerName = s.Customer.Name,
					Discount = s.Discount.ToString("f2"),
					price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString("f2"),
					priceWithDiscount = s.Discount == 0 ? Math.Round(s.Car.PartCars.Sum(pc => pc.Part.Price), 2).ToString() : Math.Round(s.Car.PartCars.Sum(pc => pc.Part.Price) - (s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100), 2).ToString()
				})
				.Take(10)
				.ToList();

			var jsonSales = JsonConvert.SerializeObject(sales, Formatting.Indented);

			return jsonSales;
		}
	}
}