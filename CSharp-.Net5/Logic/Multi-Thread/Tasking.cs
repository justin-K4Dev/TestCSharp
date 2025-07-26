using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MultiThread;

public class Tasking
{
	static void UserClicksTheCancelButton(CancellationTokenSource cts)
	{
		// Wait between 150 and 500 ms, then cancel.
		// Adjust these values if necessary to make
		// cancellation fire while query is still executing.
		Random rand = new();
		System.Threading.Thread.Sleep(rand.Next(150, 500));
		cts.Cancel();
	}

	static void Task_with_cancel_with_PLINQ_query()
	{
		int[] source = Enumerable.Range(1, 10000000).ToArray();
		using CancellationTokenSource cts = new();

		// Start a new asynchronous task that will cancel the
		// operation from another thread. Typically you would call
		// Cancel() in response to a button click or some other
		// user interface event.
		Task.Factory.StartNew(() =>
		{
			UserClicksTheCancelButton(cts);
		});

		int[] results = null;
		try
		{
			results = ( from num in source.AsParallel().WithCancellation(cts.Token)
				        where num % 3 == 0
				        orderby num descending
				        select num ).ToArray();
		}
		catch (OperationCanceledException e)
		{
			Console.WriteLine(e.Message);
		}
		catch (AggregateException ae)
		{
			if (ae.InnerExceptions != null)
			{
				foreach (Exception e in ae.InnerExceptions)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		foreach (var item in results ?? Array.Empty<int>())
		{
			Console.WriteLine(item);
		}

		Console.WriteLine();

		Console.ReadLine();
	}


    public class Packet
    {
        public string Data { get; set; }
    }

    static async Task<Packet> receivePacketAsync()
    {
        await Task.Delay(3000);
        return new Packet { Data = "Hello Packet" };
    }

    static async Task Task_with_WaitAsync()
    {
        try
        {
            Task<Packet> receiveTask = receivePacketAsync();

            // 타임아웃: 3초 안에 응답이 없으면 예외 발생
            Packet packet = await receiveTask.WaitAsync(TimeSpan.FromSeconds(3));

            Console.WriteLine($"패킷 수신 성공: {packet.Data}");
        }
        catch (TimeoutException)
        {
            Console.WriteLine("패킷 수신 타임아웃 발생");
        }
    }


    public static void Test()
	{
		//Task_with_WaitAsync().Wait();

		//Task_with_cancel_with_PLINQ_query();
	}
}
