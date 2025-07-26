using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThread
{
	public class Toast { }

	public class Bacon { }

	public class Egg { }

	public class Juice { }

	public class Coffee { }

	public class Sync
	{
		public static Juice PourOJ()
		{
			Console.WriteLine("Pouring orange juice");
			return new Juice();
		}

		public static void ApplyJam(Toast toast) =>
			Console.WriteLine("Putting jam on the toast");

		public static void ApplyButter(Toast toast) =>
			Console.WriteLine("Putting butter on the toast");

		public static Toast ToastBread(int slices)
		{
			for (int slice = 0; slice < slices; slice++)
			{
				Console.WriteLine("Putting a slice of bread in the toaster");
			}
			Console.WriteLine("Start toasting...");
			Task.Delay(3000).Wait();
			Console.WriteLine("Remove toast from toaster");

			return new Toast();
		}

		public static Bacon FryBacon(int slices)
		{
			Console.WriteLine($"putting {slices} slices of bacon in the pan");
			Console.WriteLine("cooking first side of bacon...");
			Task.Delay(3000).Wait();
			for (int slice = 0; slice < slices; slice++)
			{
				Console.WriteLine("flipping a slice of bacon");
			}
			Console.WriteLine("cooking the second side of bacon...");
			Task.Delay(3000).Wait();
			Console.WriteLine("Put bacon on plate");

			return new Bacon();
		}

		public static Egg FryEggs(int howMany)
		{
			Console.WriteLine("Warming the egg pan...");
			Task.Delay(3000).Wait();
			Console.WriteLine($"cracking {howMany} eggs");
			Console.WriteLine("cooking the eggs ...");
			Task.Delay(3000).Wait();
			Console.WriteLine("Put eggs on plate");

			return new Egg();
		}

		public static Coffee PourCoffee()
		{
			Console.WriteLine("Pouring coffee");
			return new Coffee();
		}

		public static async void Test()
		{
			var stop_watch = new Stopwatch();
			stop_watch.Start();

			Coffee cup = PourCoffee();
			Console.WriteLine("coffee is ready");

			Egg eggs = FryEggs(3);
			Console.WriteLine("eggs are ready");

			Bacon bacon = FryBacon(3);
			Console.WriteLine("bacon is ready");

			Toast toast = ToastBread(3);
			ApplyButter(toast);
			ApplyJam(toast);
			Console.WriteLine("toast is ready");

			Juice oj = PourOJ();
			Console.WriteLine("oj is ready");
			Console.WriteLine("Breakfast is ready!");

			stop_watch.Stop();
			Console.WriteLine($"{typeof(Sync)} elapsed MS:{stop_watch.ElapsedMilliseconds}");

			Console.ReadLine();
		}
	}

	namespace Aysnc
	{
		public class Pattern1
		{
			public static async Task<Toast> ToastBreadAsync(int slices)
			{
				for (int slice = 0; slice < slices; slice++)
				{
					Console.WriteLine("Putting a slice of bread in the toaster");
				}
				Console.WriteLine("Start toasting...");
				await Task.Delay(3000);
				Console.WriteLine("Remove toast from toaster");

				return new Toast();
			}

			public static async Task<Bacon> FryBaconAsync(int slices)
			{
				Console.WriteLine($"putting {slices} slices of bacon in the pan");
				Console.WriteLine("cooking first side of bacon...");
				await Task.Delay(3000);
				for (int slice = 0; slice < slices; slice++)
				{
					Console.WriteLine("flipping a slice of bacon");
				}
				Console.WriteLine("cooking the second side of bacon...");
				await Task.Delay(3000);
				Console.WriteLine("Put bacon on plate");

				return new Bacon();
			}

			public static async Task<Egg> FryEggsAsync(int howMany)
			{
				Console.WriteLine("Warming the egg pan...");
				await Task.Delay(3000);
				Console.WriteLine($"cracking {howMany} eggs");
				Console.WriteLine("cooking the eggs ...");
				await Task.Delay(3000);
				Console.WriteLine("Put eggs on plate");

				return new Egg();
			}

			public static async void Test()
			{
				var stop_watch = new Stopwatch();
				stop_watch.Start();

				Coffee cup = Sync.PourCoffee();
				Console.WriteLine("coffee is ready");

				Egg eggs = await FryEggsAsync(3);
				Console.WriteLine("eggs are ready");

				Bacon bacon = await FryBaconAsync(3);
				Console.WriteLine("bacon is ready");

				Toast toast = await ToastBreadAsync(3);
				Sync.ApplyButter(toast);
				Sync.ApplyJam(toast);
				Console.WriteLine("toast is ready");

				Juice oj = Sync.PourOJ();
				Console.WriteLine("oj is ready");
				Console.WriteLine("Breakfast is ready!");

				stop_watch.Stop();
				Console.WriteLine($"{typeof(Pattern1)} elapsed MS:{stop_watch.ElapsedMilliseconds}");

				Console.ReadLine();
			}
		}

		public class Pattern2
		{
			public static async void Test()
			{
				var stop_watch = new Stopwatch();
				stop_watch.Start();

				Coffee cup = Sync.PourCoffee();
				Console.WriteLine("coffee is ready");

				Task<Egg> eggsTask = Pattern1.FryEggsAsync(3);
				Task<Bacon> baconTask = Pattern1.FryBaconAsync(3);
				Task<Toast> toastTask = Pattern1.ToastBreadAsync(3);

				Toast toast = await toastTask;
				Sync.ApplyButter(toast);
				Sync.ApplyJam(toast);
				Console.WriteLine("toast is ready");
				Juice oj = Sync.PourOJ();
				Console.WriteLine("oj is ready");

				Egg eggs = await eggsTask;
				Console.WriteLine("eggs are ready");
				Bacon bacon = await baconTask;
				Console.WriteLine("bacon is ready");

				Console.WriteLine("Breakfast is ready!");

				stop_watch.Stop();
				Console.WriteLine($"{typeof(Pattern2)} elapsed MS:{stop_watch.ElapsedMilliseconds}");

				Console.ReadLine();
			}
		}

		public class Pattern3
		{
			public static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
			{
				var toast = await Pattern1.ToastBreadAsync(number);
				Sync.ApplyButter(toast);
				Sync.ApplyJam(toast);

				return toast;
			}

			public static async void Test()
			{
				var stop_watch = new Stopwatch();
				stop_watch.Start();

				Coffee cup = Sync.PourCoffee();
				Console.WriteLine("coffee is ready");

				var eggsTask = Pattern1.FryEggsAsync(3);
				var baconTask = Pattern1.FryBaconAsync(3);
				var toastTask = MakeToastWithButterAndJamAsync(3);

				var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
				while (breakfastTasks.Count > 0)
				{
					var finishedTask = await Task.WhenAny(breakfastTasks);
					if (finishedTask == eggsTask)
					{
						Console.WriteLine("eggs are ready");
					}
					else if (finishedTask == baconTask)
					{
						Console.WriteLine("bacon is ready");
					}
					else if (finishedTask == toastTask)
					{
						Console.WriteLine("toast is ready");
					}
					breakfastTasks.Remove(finishedTask);
				}

				Juice oj = Sync.PourOJ();
				Console.WriteLine("oj is ready");
				Console.WriteLine("Breakfast is ready!");

				stop_watch.Stop();
				Console.WriteLine($"{typeof(Pattern3)} elapsed MS:{stop_watch.ElapsedMilliseconds}");

				Console.ReadLine();
			}
		}

		public class Pattern4
		{

			public static async void Test()
			{
				var stop_watch = new Stopwatch();
				stop_watch.Start();

				Coffee cup = Sync.PourCoffee();
				Console.WriteLine("coffee is ready");

				var eggsTask = Pattern1.FryEggsAsync(3);
				var baconTask = Pattern1.FryBaconAsync(3);
				var toastTask = Pattern3.MakeToastWithButterAndJamAsync(3);

				Task.WaitAll(eggsTask, baconTask, toastTask);

				stop_watch.Stop();
				Console.WriteLine($"{typeof(Pattern4)} elapsed MS:{stop_watch.ElapsedMilliseconds}");

				Console.ReadLine();
			}
		}

	}//Async

	public class AsyncAwaitExample
	{
		public static void Test()
		{
			//Aysnc.Pattern4.Test();
			//Aysnc.Pattern3.Test();
			//Aysnc.Pattern2.Test();
			//Aysnc.Pattern1.Test();
			//Sync.Test();
		}
	}
}
