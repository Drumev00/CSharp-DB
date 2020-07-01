using System;

namespace Template
{
	public class SourDough : Bread
	{
		public override void Bake()
		{
			Console.WriteLine("Baking the Sourdough Bread. (20 minutes)");
		}

		public override void MixIngredients()
		{
			Console.WriteLine("Gathering Ingredients for Sourdough Bread.");
		}
	}
}
