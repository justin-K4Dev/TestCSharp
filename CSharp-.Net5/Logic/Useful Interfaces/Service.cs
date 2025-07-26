
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace UsefulInterfaces;


public class Service
{
    public interface IScopedService
    {
        Guid Id { get; }
    }

    public class ScopedService : IScopedService, IDisposable
    {
        public Guid Id { get; } = Guid.NewGuid();
        public void Dispose() => Console.WriteLine($"ScopedService ({Id}) disposed!");
    }

    static void override_IServiceScope()
    {
        /*
            IServiceScope

            ✅ 목적
              - DI(의존성 주입) 컨테이너에서 "스코프" 단위의 서비스 생명주기 관리를 위한 인터페이스
              - .ServiceProvider 속성으로 스코프 내에서만 살아있는 서비스를 제공
              - IDisposable을 구현: 스코프가 끝나면 자원/서비스가 Dispose됨

            IServiceScopeFactory

            ✅ 목적
              - 새로운 IServiceScope(스코프 컨텍스트) 인스턴스를 생성해주는 팩토리 역할.
              - await MoveNextAsync()로 다음 데이터, Current로 현재 데이터, DisposeAsync()로 해제

            ✅ **스코프(범위)**란?
              - DI 루트 컨테이너에서 원하는 시점에 임시 스코프 컨테이너 생성 가능
        */

        // 1. 루트 서비스 컬렉션/컨테이너 구성
        var services = new ServiceCollection();
        services.AddScoped<IScopedService, ScopedService>();
        var provider = services.BuildServiceProvider();

        // 2. IServiceScopeFactory로 스코프 생성
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        // 첫번째 스코프
        using (var scope1 = scopeFactory.CreateScope())
        {
            var svc1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
            Console.WriteLine($"Scope1 Service ID: {svc1.Id}");
        } // scope1 끝나면 ScopedService Dispose 호출

        // 두번째 스코프
        using (var scope2 = scopeFactory.CreateScope())
        {
            var svc2a = scope2.ServiceProvider.GetRequiredService<IScopedService>();
            var svc2b = scope2.ServiceProvider.GetRequiredService<IScopedService>();
            Console.WriteLine($"Scope2 Service ID (a): {svc2a.Id}");
            Console.WriteLine($"Scope2 Service ID (b): {svc2b.Id} (동일 객체)");
        } // scope2 끝나면 ScopedService Dispose 호출

        /*
            출력:
            Scope1 Service ID: aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee
            ScopedService (aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee) disposed!
            Scope2 Service ID (a): ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj
            Scope2 Service ID (b): ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj (동일 객체)
            ScopedService (ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj) disposed!
        */
    }

    //=============================================================================================

    public class MyWorker : Microsoft.Extensions.Hosting.IHostedService
    {
        private Task? _task;
        private CancellationTokenSource? _cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MyWorker: StartAsync called");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _task = Task.Run(() => WorkLoop(_cts.Token), _cts.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MyWorker: StopAsync called");
            if (_cts != null)
                _cts.Cancel();

            // 만약 백그라운드 작업의 완료를 기다리고 싶다면:
            return _task ?? Task.CompletedTask;
        }

        private async Task WorkLoop(CancellationToken token)
        {
            int count = 0;
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"MyWorker: Working... {++count}");
                await Task.Delay(1000, token);
            }
            Console.WriteLine("MyWorker: WorkLoop cancelled");
        }
    }

    static async Task override_IHostedService()
    {
        /*
            IHostedService (ASP.NET Core/Worker)

            ✅ 목적
              - 백그라운드 서비스(스케줄러/메시지큐 등) 구현
        */

        // HostBuilder로 백그라운드 서비스 구성
        using var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHostedService<MyWorker>();
            })
            .Build();

        Console.WriteLine("Host starting...");
        await host.StartAsync();

        Console.WriteLine("Host running... (press Enter to stop)");
        Console.ReadLine();

        Console.WriteLine("Host stopping...");
        await host.StopAsync();

        Console.WriteLine("Host stopped!");
    }


    public static void Test()
    {
        override_IHostedService().Wait();

        override_IServiceScope();
    }
}
