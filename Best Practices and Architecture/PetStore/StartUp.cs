using PetStore.Data;
using PetStore.Services.Implementations;
using System;

namespace PetStore
{
	public class StartUp
	{
		public static void Main()
		{
			using var data = new PetStoreDbContext();

			var userService = new UserService(data);
			var toyServie = new ToyService(data, userService);
			var breedService = new BreedService(data);
			var categoryService = new CategoryService(data);

			var petService = new PetService(data, breedService, categoryService, userService);

			//petService.BuyPet(Data.Models.Enumerations.Gender.Male, DateTime.Now, 35.5m, 1, 3, "Cute");
			petService.SellPet(1, 1);


		}
	}
}
