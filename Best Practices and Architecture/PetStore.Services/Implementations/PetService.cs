using System;
using System.Linq;

using PetStore.Data;
using PetStore.Data.Models;
using PetStore.Data.Models.Enumerations;

namespace PetStore.Services.Implementations
{
	public class PetService : IPetService
	{
		private readonly PetStoreDbContext data;
		private readonly IBreedService breedService;
		private readonly ICategoryService categoryService;
		private readonly IUserService userService;

		public PetService(PetStoreDbContext db, IBreedService breedService, ICategoryService categoryService, IUserService userService)
		{
			this.data = db;
			this.breedService = breedService;
			this.categoryService = categoryService;
			this.userService = userService;
		}

		public void BuyPet(Gender gender, DateTime birthDate, decimal price, int breedId, int categoryId, string description = null)
		{
			if (price < 0)
			{
				throw new ArgumentException("Pet's price cannot be negative number");
			}
			if (!this.breedService.Exists(breedId))
			{
				throw new ArgumentException("Breed doesn't exists.");
			}
			if (!this.categoryService.Exists(categoryId))
			{
				throw new ArgumentException("Category doesn't exists.");
			}

			var pet = new Pet
			{
				Gender = gender,
				BirthDate = DateTime.Now,
				Price = price,
				BreedId = breedId,
				CategoryId = categoryId,
				Description = description
			};

			this.data.Pets.Add(pet);
			this.data.SaveChanges();
		}

		public bool Exists(int petId)
		{
			return this.data
				.Pets
				.Any(p => p.Id == petId);
		}

		public void SellPet(int petId, int userId)
		{
			if (!this.userService.Exists(userId))
			{
				throw new ArgumentException("User not found.");
			}
			if (!this.Exists(petId))
			{
				throw new ArgumentException("Pet not found.");
			}

			var pet = this.data.Pets.Find(petId);

			var order = new Order
			{
				PurchaseDate = DateTime.Now,
				Status = OrderStatus.Sold,
				UserId = userId,
			};
			pet.Order = order;

			this.data.Orders.Add(order);
			this.data.SaveChanges();
		}
	}
}
