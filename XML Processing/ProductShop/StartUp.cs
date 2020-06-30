using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;

namespace ProductShop
{
	public class StartUp
	{
		public static void Main(string[] args)
		{
			using (var db = new ProductShopContext())
			{
				Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

				//var inputXml = File.ReadAllText("./../../../Datasets/categories-products.xml");

				Console.WriteLine(GetUsersWithProducts(db));
			}
		}
		//Problem 01
		public static string ImportUsers(ProductShopContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

			ImportUserDto[] userDtos;
			using (var reader = new StringReader(inputXml))
			{
				userDtos = (ImportUserDto[])xmlSerializer.Deserialize(reader);
			}

			var users = Mapper.Map<User[]>(userDtos);

			context.Users.AddRange(users);
			context.SaveChanges();

			return $"Successfully imported {users.Length}";
		}
		//Problem 02
		public static string ImportProducts(ProductShopContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

			ImportProductDto[] productDtos;
			using (var reader = new StringReader(inputXml))
			{
				productDtos = (ImportProductDto[])xmlSerializer.Deserialize(reader);
			}

			var products = Mapper.Map<Product[]>(productDtos);

			context.Products.AddRange(products);
			context.SaveChanges();

			return $"Successfully imported {products.Length}";
		}
		//Problem 03
		public static string ImportCategories(ProductShopContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));

			ImportCategoryDto[] categoryDtos;
			using (var reader = new StringReader(inputXml))
			{
				categoryDtos = ((ImportCategoryDto[])xmlSerializer.Deserialize(reader))
					.Where(c => c.Name != null)
					.ToArray();
			}

			var categories = Mapper.Map<Category[]>(categoryDtos);

			context.Categories.AddRange(categories);
			context.SaveChanges();

			return $"Successfully imported {categories.Length}";
		}
		//Problem 04
		public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
		{
			var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductsDto[]), new XmlRootAttribute("CategoryProducts"));

			ImportCategoryProductsDto[] categoryProductsDtos;
			using (var reader = new StringReader(inputXml))
			{
				categoryProductsDtos = ((ImportCategoryProductsDto[])xmlSerializer.Deserialize(reader))
					.Where(dto => context.Categories.Any(c => c.Id == dto.CategoryId) && context.Products.Any(p => p.Id == dto.ProductId))
					.ToArray();
			}

			var categoryProducts = Mapper.Map<CategoryProduct[]>(categoryProductsDtos);

			context.CategoryProducts.AddRange(categoryProducts);
			context.SaveChanges();

			return $"Successfully imported {categoryProducts.Length}";
		}
		//Problem 05
		public static string GetProductsInRange(ProductShopContext context)
		{
			var xmlSerializer = new XmlSerializer(typeof(ExportProductsInRange[]), new XmlRootAttribute("Products"));

			var products = context.Products
				.Where(p => p.Price >= 500 && p.Price <= 1000)
				.OrderBy(p => p.Price)
				.ProjectTo<ExportProductsInRange>()
				.Take(10)
				.ToArray();

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, products, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 06
		public static string GetSoldProducts(ProductShopContext context)
		{
			var xmlSerializer = new XmlSerializer(typeof(ExportUsersSoldProductsDto[]), new XmlRootAttribute("Users"));

			var users = context.Users
				.Where(u => u.ProductsSold.Count >= 1)
				.OrderBy(u => u.LastName)
				.ThenBy(u => u.FirstName)
				.Take(5)
				.Select(u => new ExportUsersSoldProductsDto()
				{
					FirstName = u.FirstName,
					LastName = u.LastName,
					SoldProducts = u.ProductsSold
						.Where(p => p.BuyerId != null)
						.Select(p => new ExportProductsInRange()
						{
							Name = p.Name,
							Price = p.Price
						})
						.ToArray()
				})
				.ToArray();

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, users, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 07
		public static string GetCategoriesByProductsCount(ProductShopContext context)
		{
			var xmlSerializer = new XmlSerializer(typeof(ExportCategoriesByProductsCountDto[]), new XmlRootAttribute("Categories"));

			var categoriesWithCosts = context.Categories
				.Select(c => new ExportCategoriesByProductsCountDto()
				{
					Name = c.Name,
					Count = c.CategoryProducts.Count,
					AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
					TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
				})
					.OrderByDescending(c => c.Count)
					.ThenBy(c => c.TotalRevenue)
				.ToArray();

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, categoriesWithCosts, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 08
		public static string GetUsersWithProducts(ProductShopContext context)
		{
			var xmlSerializer = new XmlSerializer(typeof(ExportUsersAndProductsFinalResultDto));

			var users = context.Users
				.Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
				.OrderByDescending(x => x.ProductsSold.Count)
				.Select(u => new ExportUsersAndProductsUserDto()
				{
					FirstName = u.FirstName,
					LastName = u.LastName,
					Age = u.Age,
					SoldProducts = new ExportSoldProductsUsersAndProductsDto()
					{
						Count = u.ProductsSold.Count,
						Products = u.ProductsSold
							.Select(p => new ExportUsersAndProductsProductDto()
							{
								Name = p.Name,
								Price = p.Price
							})
							.OrderByDescending(dto => dto.Price)
							.ToArray()
					}
				})
				.Take(10)
				.ToArray();

			var result = new ExportUsersAndProductsFinalResultDto
			{
				Count = context.Users.Count(p => p.ProductsSold.Any()),
				Users = users
			};

			StringBuilder sb = new StringBuilder();

			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			using (var writer = new StringWriter(sb))
			{
				xmlSerializer.Serialize(writer, result, namespaces);
			}

			return sb.ToString().TrimEnd();
		}
	}
}