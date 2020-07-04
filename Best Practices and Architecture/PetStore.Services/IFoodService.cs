using System;

using PetStore.Services.Models.Food;

namespace PetStore.Services
{
	public interface IFoodService
	{
		void BuyFromDistributor(string name, double weight, decimal price, DateTime expDate, int brandId, int categoryId);

		void BuyFromDistributor(AddFoodServiceModel model);

		void SellFood(int foodId, int userId);

		bool Exists(int foodId);
	}
}
