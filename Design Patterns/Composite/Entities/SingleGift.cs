using System;

namespace Composite.Entities
{
	public class SingleGift : GiftBase
	{
		public SingleGift(string name, decimal price)
			:base(name, price)
		{

		}
		public override decimal CalculateTotalPrice()
		{
			Console.WriteLine($"{name} with the price {price}");

			return price;
		}
	}
}
