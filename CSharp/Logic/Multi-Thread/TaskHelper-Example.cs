using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread
{
	public class TaskHelperExample
	{

        private static async System.Threading.Tasks.Task testThenAsync()
        {
            Console.WriteLine("[thenAsync] Start");

            var result = await System.Threading.Tasks.Task
                .FromResult(10)
                .thenAsync(value => value * 2);

            Console.WriteLine($"[thenAsync] Result: {result}");
        }

        private static async System.Threading.Tasks.Task testContinueByStatusSuccess()
        {
            Console.WriteLine("[continueByStatus Success] Start");

            await System.Threading.Tasks.Task
                .FromResult("OK")
                .continueByStatus(
                    onSuccess: result => Console.WriteLine($"Success: {result}"),
                    onFaulted: ex => Console.WriteLine($"Faulted: {ex.Message}"),
                    onCanceled: () => Console.WriteLine("Canceled"));
        }

        private static async System.Threading.Tasks.Task testContinueByStatusFaulted()
        {
            Console.WriteLine("[continueByStatus Faulted] Start");

            await System.Threading.Tasks.Task
                .FromException(new InvalidOperationException("Test exception"))
                .continueByStatus(
                    onSuccess: () => Console.WriteLine("Success"),
                    onFaulted: ex => Console.WriteLine($"Faulted: {ex.Message}"),
                    onCanceled: () => Console.WriteLine("Canceled"));
        }

        private static async System.Threading.Tasks.Task testContinueByStatusCanceled()
        {
            Console.WriteLine("[continueByStatus Canceled] Start");

            await System.Threading.Tasks.Task
                .FromCanceled(new System.Threading.CancellationToken(true))
                .continueByStatus(
                    onSuccess: () => Console.WriteLine("Success"),
                    onFaulted: ex => Console.WriteLine($"Faulted: {ex.Message}"),
                    onCanceled: () => Console.WriteLine("Canceled"));
        }



        private static async System.Threading.Tasks.Task testTaskHelper()
        {
            await testThenAsync();
            await testContinueByStatusSuccess();
            await testContinueByStatusFaulted();
            await testContinueByStatusCanceled();
        }

        public static void Test()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("TaskHelperTest Start");
            Console.WriteLine("========================================");

            testTaskHelper().Wait();

            Console.WriteLine("========================================");
            Console.WriteLine("TaskHelperTest End");
            Console.WriteLine("========================================");
        }
    }
}
