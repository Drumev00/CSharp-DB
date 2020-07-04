using System;

using PetStore.Data.Models.Enumerations;

namespace PetStore.Services
{
	public interface IPetService
	{
		void BuyPet(Gender gender, DateTime birthDate, decimal price, int breedId, int categoryId, string description = null);

		void SellPet(int petId, int userId);

		bool Exists(int petId);
	}
}
