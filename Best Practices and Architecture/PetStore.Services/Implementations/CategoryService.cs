using System.Linq;

using PetStore.Data;

namespace PetStore.Services.Implementations
{
	public class CategoryService : ICategoryService
	{
		private readonly PetStoreDbContext data;

		public CategoryService(PetStoreDbContext db)
		{
			this.data = db;
		}

		public bool Exists(int categoryId)
		{
			return this.data
				.Categories
				.Any(c => c.Id == categoryId);
		}
	}
}
