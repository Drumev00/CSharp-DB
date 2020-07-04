using System;

namespace PetStore.Services.Models.Food
{
	public class AddFoodServiceModel
	{
		public string Name { get; set; }
		public double Weight { get; set; }
		public decimal Price { get; set; }
		public decimal DistributorPrice { get; set; }
		public DateTime ExpirationDate { get; set; }
		public int BrandId { get; set; }
		public int CategoryId { get; set; }
	}
}
