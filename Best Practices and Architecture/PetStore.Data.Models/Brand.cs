using System.Collections.Generic;

namespace PetStore.Data.Models
{
	public class Brand
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();
		public ICollection<Food> Food { get; set; } = new HashSet<Food>();
	}
}
