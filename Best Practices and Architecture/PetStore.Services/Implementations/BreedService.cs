using System;
using System.Linq;
using PetStore.Data;
using PetStore.Data.Models;

namespace PetStore.Services.Implementations
{
	public class BreedService : IBreedService
	{
		private readonly PetStoreDbContext data;

		public BreedService(PetStoreDbContext db)
		{
			this.data = db;
		}

		public void Add(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Breed name cannot be null or whitespace");
			}

			var breed = new Breed
			{
				Name = name
			};

			this.data.Breeds.Add(breed);
			this.data.SaveChanges();
		}

		public bool Exists(int Id)
		{
			return this.data
				.Breeds
				.Any(b => b.Id == Id);
		}
	}
}
