namespace Prototype
{
	class Program
	{
		static void Main(string[] args)
		{
			SandwichMenu menu = new SandwichMenu();

			//Default sandwiches
			menu["BLT"] = new Sandwich("Wheat", "Bacon", "", "Lettuce, Tomato");
			menu["PB&J"] = new Sandwich("White", "", "", "Peanut Butte, Jelly");
			menu["Turkey"] = new Sandwich("Rye", "Turkey", "Swiss", "Lettuce, Onion, Tomato");

			//Custom sandwiches
			menu["LoadedBLT"] = new Sandwich("Wheat", "Turkey, Bacon", "American", "Lettuce, Tomato, Onion, Olives");
			menu["ThreeMeatCombo"] = new Sandwich("Rye", "Turkey, Ham, Salami", "Provolone", "Lettuce, Onion");
			menu["Vegetarian"] = new Sandwich("Wheat", "", "", "Lettuce, Onion, Tomato, Olives, Spinach");

			//We can clone those sandwiches
			Sandwich sandwich1 = menu["BLT"].Clone() as Sandwich;
			Sandwich sandwich2 = menu["ThreeMeatCombo"].Clone() as Sandwich;
			Sandwich sandwich3 = menu["Vegetarian"].Clone() as Sandwich;
		}
	}
}
