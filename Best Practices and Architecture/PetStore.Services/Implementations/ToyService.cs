using System;
using System.Linq;

using PetStore.Data;
using PetStore.Data.Models;
using PetStore.Services.Models.Toy;

namespace PetStore.Services.Implementations
{
	public class ToyService : IToyService
	{
		private readonly PetStoreDbContext data;
		private readonly IUserService userService;

		public ToyService(PetStoreDbContext db, IUserService userService)
		{
			this.data = db;
			this.userService = userService;
		}

		public void BuyFromDistributor(string name, decimal disPrice, int brandId, int categoryId, string description = null)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Name cannot be null or whitespace!");
			}

			var toy = new Toy
			{
				Name = name,
				DistributorPrice = disPrice,
				Price = disPrice * 1.2m,
				BrandId = brandId,
				CategoryId = categoryId,
				Description = description
			};

			this.data.Toys.Add(toy);
			this.data.SaveChanges();
		}

		public void BuyFromDistributor(AddToyServiceModel model)
		{
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new ArgumentException("Name cannot be null or whitespace!");
			}

			var toy = new Toy
			{
				Name = model.Name,
				DistributorPrice = model.DistributorPrice,
				Price = model.DistributorPrice * 1.2m,
				BrandId = model.BrandId,
				CategoryId = model.CategoryId,
				Description = model.Description
			};

			this.data.Toys.Add(toy);
			this.data.SaveChanges();
		}

		public bool Exists(int toyId)
		{
			return this.data
				.Toys
				.Any(t => t.Id == toyId);
		}

		public void SellToy(int toyId, int userId)
		{
			if (!Exists(toyId))
			{
				throw new ArgumentException("No such a toy exists.");
			}
			if (!this.userService.Exists(userId))
			{
				throw new ArgumentException("User not found.");
			}

			var order = new Order
			{
				UserId = userId,
				PurchaseDate = DateTime.Now,
				Status = Data.Models.Enumerations.OrderStatus.Sold,
			};

			var toyOrder = new ToyOrder
			{
				Order = order,
				ToyId = toyId
			};

			this.data.Orders.Add(order);
			this.data.ToyOrders.Add(toyOrder);
			this.data.SaveChanges();
		}
	}
}
