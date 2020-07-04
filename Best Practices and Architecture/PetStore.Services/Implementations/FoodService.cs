using System;
using System.Linq;

using PetStore.Data;
using PetStore.Data.Models;
using PetStore.Services.Models.Food;

namespace PetStore.Services.Implementations
{
	public class FoodService : IFoodService
	{
		private readonly PetStoreDbContext data;
		private readonly IUserService userService;
		public FoodService(PetStoreDbContext db, IUserService userService)
		{
			this.data = db;
			this.userService = userService;


		}
		public void BuyFromDistributor(string name, double weight, decimal disPrice, DateTime expDate, int brandId, int categoryId)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Name cannot be null or whitespace!");
			}

			var food = new Food
			{
				Name = name,
				Weight = weight,
				DistributorPrice = disPrice,
				Price = disPrice * 1.2m,
				ExpirationDate = expDate,
				BrandId = brandId,
				CategoryId = categoryId
			};

			this.data.Foods.Add(food);
			this.data.SaveChanges();
		}

		public void BuyFromDistributor(AddFoodServiceModel model)
		{
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new ArgumentException("Name cannot be null or whitespace!");
			}

			var food = new Food
			{
				Name = model.Name,
				Weight = model.Weight,
				DistributorPrice = model.DistributorPrice,
				Price = model.DistributorPrice * 1.2m,
				ExpirationDate = model.ExpirationDate,
				BrandId = model.BrandId,
				CategoryId = model.CategoryId
			};

			this.data.Foods.Add(food);
			this.data.SaveChanges();
		}

		public bool Exists(int foodId)
		{
			return this.data
				.Foods
				.Any(f => f.Id == foodId);
		}

		public void SellFood(int foodId, int userId)
		{
			if (!Exists(foodId))
			{
				throw new ArgumentException("No such food exists.");
			}
			if (!this.userService.Exists(userId))
			{
				throw new ArgumentException("User not found.");
			}
			
			var order = new Order
			{
				Status = Data.Models.Enumerations.OrderStatus.Sold,
				PurchaseDate = DateTime.Now,
				UserId = userId
			};

			var foodOrder = new FoodOrder
			{
				FoodId = foodId,
				Order = order
			};

			this.data.Orders.Add(order);
			this.data.FoodOrders.Add(foodOrder);

			this.data.SaveChanges();
		}
	}
}
