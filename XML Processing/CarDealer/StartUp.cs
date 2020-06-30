using System;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

using System.Text;
using CarDealer.Data;
using CarDealer.Models;
using CarDealer.Dtos.Import;
using CarDealer.Dtos.Export;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
			using (var db = new CarDealerContext())
			{
				Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

				//string inputXml = File.ReadAllText("./../../../Datasets/sales.xml");

				Console.WriteLine(GetSalesWithAppliedDiscount(db));
			}
        }
		//Problem 09
		public static string ImportSuppliers(CarDealerContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportSuppliers[]), new XmlRootAttribute("Suppliers"));

			ImportSuppliers[] dtos;
			using (var reader = new StringReader(inputXml))
			{
				dtos = (ImportSuppliers[])xmlSerializer.Deserialize(reader);
			}

			var suppliers = Mapper.Map<Supplier[]>(dtos);

			context.Suppliers.AddRange(suppliers);
			context.SaveChanges();

			return $"Successfully imported {suppliers.Length}";
		}
		//Problem 10
		public static string ImportParts(CarDealerContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportParts[]), new XmlRootAttribute("Parts"));

			ImportParts[] dtos;
			using (var reader = new StringReader(inputXml))
			{
				dtos = ((ImportParts[])xmlSerializer.Deserialize(reader))
					.Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
					.ToArray();
			}

			var parts = Mapper.Map<Part[]>(dtos);

			context.Parts.AddRange(parts);
			context.SaveChanges();

			return $"Successfully imported {parts.Length}";
		}
		//Problem 11
		public static string ImportCars(CarDealerContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportCar[]), new XmlRootAttribute("Cars"));

			ImportCar[] dtos;
			using (var reader = new StringReader(inputXml))
			{
				dtos = (ImportCar[])xmlSerializer.Deserialize(reader);
			}

			List<Car> cars = new List<Car>();
			List<PartCar> partCars = new List<PartCar>();

			foreach (var dto in dtos)
			{
				var car = new Car
				{
					Make = dto.Make,
					Model = dto.Model,
					TravelledDistance = dto.TravelledDistance
				};

				var parts = dto.Parts
					.Where(partdto => context.Parts.Any(p => p.Id == partdto.PartId))
					.Select(partdto => partdto.PartId)
					.Distinct();

				foreach (var part in parts)
				{
					var partCar = new PartCar
					{
						PartId = part,
						Car = car
					};

					partCars.Add(partCar);
				}
				cars.Add(car);
			}

			context.Cars.AddRange(cars);
			context.PartCars.AddRange(partCars);
			context.SaveChanges();

			return $"Successfully imported {cars.Count}";
		}
		//Problem 12
		public static string ImportCustomers(CarDealerContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportCustomers[]), new XmlRootAttribute("Customers"));

			ImportCustomers[] dtos;
			using (var reader = new StringReader(inputXml))
			{
				dtos = (ImportCustomers[])xmlSerializer.Deserialize(reader);
			}

			var customers = Mapper.Map<Customer[]>(dtos);

			context.Customers.AddRange(customers);
			context.SaveChanges();

			return $"Successfully imported {customers.Length}";
		}
		//Problem 13
		public static string ImportSales(CarDealerContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportSales[]), new XmlRootAttribute("Sales"));

			ImportSales[] dtos;
			using (var reader = new StringReader(inputXml))
			{
				dtos = ((ImportSales[])xmlSerializer.Deserialize(reader))
					.Where(dto => context.Cars.Any(c => c.Id == dto.CarId))
					.ToArray();
			}

			var sales = Mapper.Map<Sale[]>(dtos);

			context.Sales.AddRange(sales);
			context.SaveChanges();

			return $"Successfully imported {sales.Length}";
		}
		//Problem 14
		public static string GetCarsWithDistance(CarDealerContext context)
		{
			var cars = context.Cars
				.Where(c => c.TravelledDistance > 2000000)
				.Select(c => new ExportCarsWithDistance
				{
					Make = c.Make,
					Model = c.Model,
					TravelledDistance = c.TravelledDistance
				})
				.OrderBy(c => c.Make)
				.ThenBy(c => c.Model)
				.Take(10)
				.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistance[]), new XmlRootAttribute("cars"));

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
				namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, cars, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 15
		public static string GetCarsFromMakeBmw(CarDealerContext context)
		{
			var bmws = context.Cars
				.Where(c => c.Make == "BMW")
				.OrderBy(c => c.Model)
				.ThenByDescending(c => c.TravelledDistance)
				.Select(c => new ExportBMWCars
				{
					Id = c.Id,
					Model = c.Model,
					TravelledDistance = c.TravelledDistance
				})
				.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(ExportBMWCars[]), new XmlRootAttribute("cars"));

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, bmws, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 16
		public static string GetLocalSuppliers(CarDealerContext context)
		{
			var suppliers = context.Suppliers
				.Where(s => !s.IsImporter)
				.Select(s => new ExportLocalSuppliers
				{
					Id = s.Id,
					Name = s.Name,
					PartsCount = s.Parts.Count
				})
				.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(ExportLocalSuppliers[]), new XmlRootAttribute("suppliers"));

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, suppliers, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 17
		public static string GetCarsWithTheirListOfParts(CarDealerContext context)
		{
			var carsWithParts = context.Cars
				.OrderByDescending(c => c.TravelledDistance)
				.ThenBy(c => c.Model)
				.Take(5)
				.Select(c => new ExportCarsWithParts
				{
					Make = c.Make,
					Model = c.Model,
					TravelledDistance = c.TravelledDistance,
					Parts = c.PartCars
					.Select(pc => new ExportPartsFromCar
					{
						Name = pc.Part.Name,
						Price = pc.Part.Price
					})
					.OrderByDescending(dto => dto.Price)
					.ToArray()
				})
				.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithParts[]), new XmlRootAttribute("cars"));

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, carsWithParts, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 18
		public static string GetTotalSalesByCustomer(CarDealerContext context)
		{
			var customers = context.Customers
				.Where(c => c.Sales.Count > 0)
				.Select(c => new ExportCustomersAndSales
				{
					FullName = c.Name,
					BoughtCars = c.Sales.Count,
					SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
				})
				.OrderByDescending(dto => dto.SpentMoney)
				.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(ExportCustomersAndSales[]), new XmlRootAttribute("customers"));

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, customers, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 19
		public static string GetSalesWithAppliedDiscount(CarDealerContext context)
		{
			var carsForSale = context.Sales
				.Select(s => new ExportSalesInformation
				{
					Car = new ExportCarsInformation
					{
						Make = s.Car.Make,
						Model = s.Car.Model,
						TravelledDistance = s.Car.TravelledDistance
					},
					Discount = s.Discount,
					CustomerName = s.Customer.Name,
					Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
					PriceWithDiscount = s.Discount == 0 ? s.Car.PartCars.Sum(pc => pc.Part.Price) : s.Car.PartCars.Sum(pc => pc.Part.Price) - (s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100)
				})
				.ToArray();

			var xmlSerializer = new XmlSerializer(typeof(ExportSalesInformation[]), new XmlRootAttribute("sales"));

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, carsForSale, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
	}
}