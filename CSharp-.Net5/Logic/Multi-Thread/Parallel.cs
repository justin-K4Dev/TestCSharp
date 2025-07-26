using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MultiThread;

public class Parallel
{
	static async Task mathSQRT(Int32 num)
	{
		await Task.Run(() =>
		{
			double d = Math.Sqrt(num);
			Console.WriteLine("{0} on {1}", d, System.Threading.Thread.CurrentThread.ManagedThreadId);
		});
	}


	static async Task Parallel_with_Async()
	{
		int[] nums = Enumerable.Range(0, 100).ToArray();
		var cts = new System.Threading.CancellationTokenSource();
		var token = cts.Token;

		// Use ParallelOptions instance to store the CancellationToken
		ParallelOptions po = new ParallelOptions();
		po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
		Console.WriteLine("Press any key to start");
        Console.ReadLine();

        await System.Threading.Tasks.Parallel.ForEachAsync(nums, po, async (num, cts) => {
            await mathSQRT(num);
        });


        // Parallel.ForEachAsync() 는 await의 완료를 기다린다. !!!

        Console.WriteLine($"Terminate Parallel - ThreadId : {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        Console.ReadLine();
	}

	public static void Test()
	{
		//Parallel_with_Async().Wait();
	}
}
