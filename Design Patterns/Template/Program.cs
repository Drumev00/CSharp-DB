namespace Template
{
	class Program
	{
		static void Main(string[] args)
		{
			var twelveGrain = new TwelveGrain();
			twelveGrain.Make();
			var sourDough = new SourDough();
			sourDough.Make();
			var wholeWheat = new WholeWheat();
			wholeWheat.Make();
		}
	}
}
