using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
	public class StartUp
	{
		public static void Main(string[] args)
		{
			var db = new ProductShopContext();

			Console.WriteLine(GetUsersWithProducts(db));
		}
		//Problem 01
		public static string ImportUsers(ProductShopContext context, string inputJson)
		{
			var users = JsonConvert.DeserializeObject<User[]>(inputJson);

			context.Users.AddRange(users);
			context.SaveChanges();

			return $"Successfully imported {users.Length}";
		}
		//Problem 02
		public static string ImportProducts(ProductShopContext context, string inputJson)
		{
			var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

			context.Products.AddRange(products);
			context.SaveChanges();

			return $"Successfully imported {products.Length}";
		}
		//Problem 03
		public static string ImportCategories(ProductShopContext context, string inputJson)
		{
			var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
				.Where(c => c.Name != null)
				.ToArray();

			context.AddRange(categories);
			context.SaveChanges();

			return $"Successfully imported {categories.Length}";
		}
		//Problem 04
		public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
		{
			var entities = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

			context.AddRange(entities);
			context.SaveChanges();

			return $"Successfully imported {entities.Length}";
		}
		//Problem 05
		public static string GetProductsInRange(ProductShopContext context)
		{
			var products = context.Products
				.OrderBy(p => p.Price)
				.Where(p => p.Price >= 500 && p.Price <= 1000)
				.Select(p => new
				{
					name = p.Name,
					price = p.Price,
					seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
				})
				.ToList();

			var result = JsonConvert.SerializeObject(products, Formatting.Indented);

			return result;
		}
		//Problem 06
		public static string GetSoldProducts(ProductShopContext context)
		{
			var entities = context.Users
				.Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
				.OrderBy(u => u.LastName)
				.ThenBy(u => u.FirstName)
				.Select(u => new
				{
					firstName = u.FirstName,
					lastName = u.LastName,
					soldProducts = u.ProductsSold
					.Where(ps => ps.BuyerId != null)
					.Select(ps => new
					{
						name = ps.Name,
						price = ps.Price,
						buyerFirstName = ps.Buyer.FirstName,
						buyerLastName = ps.Buyer.LastName
					})
				})
				.ToList();

			var result = JsonConvert.SerializeObject(entities, Formatting.Indented);

			return result;
		}
		//Problem 07
		public static string GetCategoriesByProductsCount(ProductShopContext context)
		{
			var categories = context.Categories
				.OrderByDescending(c => c.CategoryProducts.Count)
				.Select(c => new
				{
					category = c.Name,
					productsCount = c.CategoryProducts.Count,
					averagePrice = c.CategoryProducts.Average(cp => cp.Product.Price).ToString("f2"),
					totalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price).ToString("f2")
				})
				.ToList();

			var result = JsonConvert.SerializeObject(categories, Formatting.Indented);

			return result;
		}
		//Problem 08
		public static string GetUsersWithProducts(ProductShopContext context)
		{
			JsonSerializerSettings jsonSerializer = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			var users = context.Users
				.Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
				.OrderByDescending(u => u.ProductsSold.Count(ps => ps.Buyer != null))
				.Select(u => new
				{
					firstName = u.FirstName,
					lastName = u.LastName,
					age = u.Age,
					soldProducts = new
					{
						count = u.ProductsSold
							.Where(p => p.Buyer != null)
							.Count(),
						products = u.ProductsSold
							.Where(p => p.Buyer != null)
							.Select(ps => new
							{
								name = ps.Name,
								price = ps.Price
							})
						.ToList()
					}
				})
				.ToList();

			var entities = new
			{
				usersCount = users.Count,
				users = users
			};

			var result = JsonConvert.SerializeObject(entities, Formatting.Indented, jsonSerializer);

			return result;
		}
	}
}