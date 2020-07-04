using System;
using System.Linq;
using System.Collections.Generic;

using PetStore.Data;
using PetStore.Data.Models;
using PetStore.Services.Models.Brand;

namespace PetStore.Services.Implementations
{
	public class BrandService : IBrandService
	{
		private readonly PetStoreDbContext data;

		public BrandService(PetStoreDbContext db)
		{
			this.data = db;
		}


		public int Create(string name)
		{
			if (name.Length > DataSettings.MaxNameLength)
			{
				throw new InvalidOperationException($"Cannot create brand with more than {DataSettings.MaxNameLength} characters.");
			}

			var brand = new Brand
			{
				Name = name
			};

			this.data.Brands.Add(brand);
			this.data.SaveChanges();

			return brand.Id;
		}

		public IEnumerable<BrandListingServiceModel> SearchByName(string name)
		{
			return this.data
					.Brands
					.Where(b => b.Name.ToLower().Contains(name.ToLower()))
					.Select(b => new BrandListingServiceModel
					{
						Id = b.Id,
						Name = b.Name
					})
					.ToList();
		}
	}
}
