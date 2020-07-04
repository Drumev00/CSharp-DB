using PetStore.Services.Models.Toy;

namespace PetStore.Services
{
	public interface IToyService
	{
		void BuyFromDistributor(string name, decimal disPrice, int brandId, int categoryId, string description = null);

		void BuyFromDistributor(AddToyServiceModel model);

		void SellToy(int toyId, int userId);

		bool Exists(int toyId);
	}
}
