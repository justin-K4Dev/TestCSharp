using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace UsefulInterfaces;


public class Option
{
    public class MyConfig
    {
        public string Greeting { get; set; }
        public int Count { get; set; }
    }

    static void override_IOptionsT()
    {
        /*
            IOptions<T>, IOptionsMonitor<T>

            ✅ 목적
              - DI(의존성 주입)로 서비스/클래스에 편하게 주입
              - IOptions<T>는 앱 시작 시 값만 필요할 때
              - IOptionsMonitor<T>는 실행 중에도 값이 바뀔 수 있고, 변화도 감지하고 싶을 때
        */

        {
            // 서비스 등록 및 옵션 바인딩
            var services = new ServiceCollection();
            services.Configure<MyConfig>(cfg =>
            {
                cfg.Greeting = "Hello";
                cfg.Count = 5;
            });

            var provider = services.BuildServiceProvider();

            // IOptions<T>로 설정 가져오기
            var options = provider.GetRequiredService<IOptions<MyConfig>>();

            Console.WriteLine($"[IOptions<T>] Greeting: {options.Value.Greeting}, Count: {options.Value.Count}");

        }

        {
            var services = new ServiceCollection();
            services.Configure<MyConfig>(cfg =>
            {
                cfg.Greeting = "Hi";
                cfg.Count = 10;
            });

            var provider = services.BuildServiceProvider();
            var monitor = provider.GetRequiredService<IOptionsMonitor<MyConfig>>();

            // 값 읽기
            Console.WriteLine($"[IOptionsMonitor<T>] Greeting: {monitor.CurrentValue.Greeting}, Count: {monitor.CurrentValue.Count}");

            // 변경 감지(실전에서는 파일, DB 등에서 바뀌면 자동 감지)
            monitor.OnChange(newCfg =>
            {
                Console.WriteLine($"Config changed! Greeting: {newCfg.Greeting}, Count: {newCfg.Count}");
            });

            // 강제로 옵션 재설정/바꿔보기 (실전에서는 파일, 환경변수 등에서 자동 적용)
            var optionsChangeToken = provider.GetRequiredService<IOptionsChangeTokenSource<MyConfig>>();
            // 직접 바꿔주는 예시는 DI 및 파일, 환경변수 등으로 할 수 있지만
            // 이 코드는 옵션값을 바꿀 때 OnChange가 작동함을 설명하는 예시임
        }
    }

    public static void Test()
    {
        override_IOptionsT();
    }
}
