using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace MultiThread
{
	public class TaskCompletionSourceExample
	{
		static async Task TaskCompletionSource_basic()
		{
			var stopwatch = Stopwatch.StartNew();

			var tcs = new TaskCompletionSource<bool>();

			Console.WriteLine($"Starting... (after {stopwatch.ElapsedMilliseconds}ms)");

			var t = Task.Delay(5000);
			await t.ContinueWith(task => tcs.SetResult(true));

			Console.WriteLine($"Waiting...  (after {stopwatch.ElapsedMilliseconds}ms)");

			await tcs.Task;

			Console.WriteLine($"Done.       (after {stopwatch.ElapsedMilliseconds}ms)");

			stopwatch.Stop();

			Console.ReadLine();
		}


		public class SdkProxy
		{
			private ConcurrentDictionary<Guid, TaskCompletionSource<bool>> pendingOrders;
			private MockSdk sdk;

			public SdkProxy()
			{
				pendingOrders = new ConcurrentDictionary<Guid, TaskCompletionSource<bool>>();

				sdk = new MockSdk();
				sdk.OnOrderCompleted += Sdk_OnOrderCompleted;
			}

			public Task SubmitOrderAsync(decimal price)
			{
				var orderId = sdk.SubmitOrder(price);

				Console.WriteLine($"OrderId {orderId} submitted with price {price}");

				var tcs = new TaskCompletionSource<bool>();
				pendingOrders.TryAdd(orderId, tcs);

				return tcs.Task;
			}

			private void Sdk_OnOrderCompleted(object sender, OrderOutcome e)
			{
				string successStr = e.Success ? "was successful" : "failed";
				Console.WriteLine($"OrderId {e.OrderId} {successStr}");

				var tcs = new TaskCompletionSource<bool>();
				pendingOrders.TryRemove(e.OrderId, out tcs);
				tcs.SetResult(e.Success);
			}
		}

		public class OrderOutcome
		{
			public Guid OrderId { get; set; }
			public bool Success { get; set; }

			public OrderOutcome(Guid orderId, bool success)
			{
				this.OrderId = orderId;
				this.Success = success;
			}
		}

		public class MockSdk
		{
			public event EventHandler<OrderOutcome> OnOrderCompleted;

			public Guid SubmitOrder(decimal price)
			{
				var orderId = Guid.NewGuid();

				// do a REST call over the network or something
				Task.Delay(3000).ContinueWith(task => OnOrderCompleted(this, new OrderOutcome(orderId, true)));

				return orderId;
			}
		}

		static async Task TaskCompletionSource_with_Proxy()
		{
			var sdkProxy = new SdkProxy();

			await sdkProxy.SubmitOrderAsync(10);
			await sdkProxy.SubmitOrderAsync(20);
			await sdkProxy.SubmitOrderAsync(5);
			await sdkProxy.SubmitOrderAsync(15);
			await sdkProxy.SubmitOrderAsync(4);

			Console.ReadLine();
		}

		public static async void Test()
		{
			//TaskCompletionSource_with_Proxy().Wait();

			//TaskCompletionSource_basic().Wait();
		}
	}
}
