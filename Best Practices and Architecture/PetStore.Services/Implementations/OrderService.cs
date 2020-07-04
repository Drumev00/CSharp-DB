using PetStore.Data;

namespace PetStore.Services.Implementations
{
	public class OrderService : IOrderService
	{
		private readonly PetStoreDbContext data;

		public OrderService(PetStoreDbContext db)
		{
			this.data = db;
		}

		public void CompleteOrder(int orderId)
		{
			var order = this.data
				.Orders
				.Find(orderId);

			order.Status = Data.Models.Enumerations.OrderStatus.Sold;

			this.data.SaveChanges();
		}
	}
}
