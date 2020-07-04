using System.Collections.Generic;

namespace PetStore.Data.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();
		public ICollection<Food> Food { get; set; } = new HashSet<Food>();
		public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();
	}
}
