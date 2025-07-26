using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace MultiThread;



public class AsyncAwait
{
    // 시나리오 1: await + Task.Run
    static async Task runAwaitableTask()
    {
        Console.WriteLine($"\n[RunAwaitableTask] Before await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        await Task.Run(() =>
        {
            Console.WriteLine($"[RunAwaitableTask] Inside Task.Run - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            System.Threading.Thread.Sleep(500); // simulate work
        });

        Console.WriteLine($"[RunAwaitableTask] After await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    // 시나리오 2: await + 이미 완료된 Task
    static async Task runCompletedTask()
    {
        Console.WriteLine($"\n[RunCompletedTask] Before await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        await Task.CompletedTask;

        Console.WriteLine($"[RunCompletedTask] After await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    // 시나리오 3: await 없이 fire-and-forget
    static void fireAndForget()
    {
        Console.WriteLine($"\n[FireAndForget] Before call - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        _ = fireAndForgetAsync();

        Console.WriteLine($"[FireAndForget] After call - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    static async Task fireAndForgetAsync()
    {
        Console.WriteLine($"[FireAndForgetAsync] Start - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        await Task.Delay(300);

        Console.WriteLine($"[FireAndForgetAsync] After await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    // 시나리오 4: ConfigureAwait(false)
    static async Task runConfigureAwaitFalse()
    {
        Console.WriteLine($"\n[RunConfigureAwaitFalse] Before await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        await Task.Delay(300).ConfigureAwait(false);

        Console.WriteLine($"[RunConfigureAwaitFalse] After await - Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    }

    static async Task flow_Task()
    {
        Console.WriteLine($"[flow_Task] Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        await runAwaitableTask();         // Task.Run 내부 await

        await runCompletedTask();         // 이미 완료된 Task

        fireAndForget();                  // await 없이 호출

        await runConfigureAwaitFalse();   // ConfigureAwait(false)

        Console.WriteLine($"[flow_Task] Done !!!, Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        Console.ReadLine();
    }


    public class MyAsyncResource : IAsyncDisposable
    {
        public async System.Threading.Tasks.ValueTask DisposeAsync()
        {
            await Task.Delay(100); // 비동기 정리 작업
            Console.WriteLine("비동기 해제 완료");
        }
    }

    static async void use_async_void()
    {
        await using var res = new MyAsyncResource();
        Console.WriteLine("리소스 사용");
    }

    static async Task use_async_Task()
    {
        await using var res = new MyAsyncResource();
        Console.WriteLine("리소스 사용");
    }

    static async Task async_void_n_Task()
    {
        /*
            ✅ 기본 차이 요약
            항목					async void						async Task
            반환 타입				void							Task
            예외 전파				❌ 호출자가 try/catch 불가	    ✅ 호출자가 try/catch 가능
            await 대상 여부		    ❌ await 불가					✅ await 가능
            사용 용도				이벤트 핸들러 전용				일반 비동기 메서드 권장 방식
            흐름 제어				호출 후 "Fire and Forget"	    호출 후 대기 (await 가능)
            디버깅				    어렵고 위험					    예외 추적 가능	
        

            📌 결론
            항목	                       async void	                         async Task
            예외 처리	                   ❌ 호출자 catch 불가	                 ✅ catch 가능
            await 대상	                   ❌ 불가능	                         ✅ 가능
            비동기 using 가능 여부	       ✅ 가능하지만 위험	                 ✅ 안정적
            추천 여부	                   ❌ 사용 금지 (이벤트 핸들러 제외)	 ✅ 일반 비동기 메서드는 항상 Task
       */
        {
            try
            {
                use_async_void(); // ❌ 이건 await할 수 없고, 예외도 catch 안됨
            }
            catch (Exception e)
            {
                // 절대 도달하지 않음
            }
        }
        {
            try
            {
                await use_async_Task(); // ✅ 정상 await 가능
            }
            catch (Exception e)
            {
                Console.WriteLine($"예외 처리됨: {e}");
            }
        }

        Console.ReadLine();
    }

    static async Task Task_with_ConfigureAwait()
    {
        /*
            🖥️ SP.NET Core (중요!)

              - ASP.NET Core는 기본적으로 SynchronizationContext가 없다 !!!
              - 따라서, ConfigureAwait(false)를 써도 성능상 큰 차이는 없지만,
                라이브러리 일관성을 위해 계속 사용하는 것이 좋음
                (코드 이식성, 통일된 패턴을 위해)

            ✅ .NET 8 이후: ConfigureAwait의 변화와 추가 옵션

              1. 새로운 플래그: ConfigureAwaitOptions
                - .NET 8에서는 ConfigureAwait에 다양한 옵션을 지정할 수 있게 됨

                await SomeAsync().ConfigureAwait(ConfigureAwaitOptions.None);           // 기존과 동일
                await SomeAsync().ConfigureAwait(ConfigureAwaitOptions.ForceYielding);  // 추가됨!
            
              2. ConfigureAwaitOptions.ForceYielding
                - 항상 다음 스케줄러(스레드풀 등)로 실행을 "강제"

                2.1. 기존 ConfigureAwait(false)는, 작업이 이미 완료된 경우 "동기적으로 바로 이어서 실행"될 수 있었음
                2.2. ForceYielding은 무조건 '한 번 더 점프'
                  → 동기 연속 실행을 막고, 데드락이나 실행 순서 문제 방지에 도움
         
                  // 기존 방식 (.NET 5~7)
                  await SomeAsync().ConfigureAwait(false); // 스케줄 점프가 보장되진 않음
                  // .NET 8 방식
                  await SomeAsync().ConfigureAwait(ConfigureAwaitOptions.ForceYielding); // 반드시 다음 스케줄러로 점프

            🧩 주요 내용 요약
            
               1. ConfigureAwaitOptions.ForceYielding
                 - .NET 8부터 추가된 옵션으로, ConfigureAwait(false)와 비슷하지만, 더 강제로 비동기로 전환합니다. 
                   즉, await 지점 이후 반드시 yield(다음 스케줄) 과정을 거칩니다.

               2. 기존 ConfigureAwait(false)와 비교
                 - 사실 ConfigureAwait(false)는 단순하게 "기존 SynchronizationContext나 UI context로 돌아가지 마라"는 의미입니다.
                 - 하지만 .NET 8에서는 이 옵션을 보다 명시적으로 통제할 수 있어, 비동기 흐름의 제어가 좀 더 확실해졌습니다.

               3. 사용 시점
                 - 라이브러리 코드: 여전히 ConfigureAwait(false) 권장. (컨텍스트에 의존하지 않는 로직이라면 안전)
                 - 애플리케이션 코드: UI나 ASP.NET Core와 같이 특정 컨텍스트에서 작동해야 한다면 신중히 사용. 
                   특히 ASP.NET Core는 기본적으로 SynchronizationContext가 없기 때문에 큰 영향은 없지만, 
                   UI 앱에서는 ForceYielding 옵션이 유용할 수 있습니다 
.
            📊 결론
              - .NET 8: ConfigureAwaitOptions.ForceYielding 도입으로 비동기 처리 흐름 제어가 더 유연해짐.
              - 라이브러리: 여전히 ConfigureAwait(false) (또는 ForceYielding) 권장.
              - 애플리케이션/UI 코드: 기존 Context 유지가 중요하면 (ConfigureAwait(true)나 default), 변경시 유의해서 사용 !!!
       */

        Console.ReadLine();
    }


    public static async Task Test()
    {
        //await Task_with_ConfigureAwait();

        //await async_void_n_Task();

        //await flow_Task();
    }

}

