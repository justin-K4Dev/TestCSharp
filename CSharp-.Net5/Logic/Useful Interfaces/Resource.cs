using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace UsefulInterfaces;


public class Resource
{
    public class AsyncResource : IAsyncDisposable
    {
        public AsyncResource()
        {
            Console.WriteLine("AsyncResource created");
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("AsyncResource.DisposeAsync() started...");
            await Task.Delay(10); // 비동기 자원 해제 시뮬레이션
            Console.WriteLine("AsyncResource disposed (async)");
        }
    }

    static async Task override_IAsyncDisposable()
    {
        /*
            IHostedService (ASP.NET Core/Worker)

            ✅ 목적
              - 비동기 자원해제 (네트워크/DB/Stream 등)
              - await using 지원
        */

        Console.WriteLine("Before await using");
        await using (var r = new AsyncResource())
        {
            Console.WriteLine("Inside using block (resource in use)");
            // 자원 사용
        }
        Console.WriteLine("After await using");
    }

    public static void Test()
    {
        override_IAsyncDisposable().Wait();
    }
}

