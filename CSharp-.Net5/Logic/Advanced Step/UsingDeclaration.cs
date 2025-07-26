using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;



namespace AdvancedStep;


public class UsingDeclaration
{
    public static void UsingDeclaration_what()
    {
        /*
            Using Declaration은 C# 8.0에서 추가된 기능으로,
            IDisposable 객체를 블록 없이 선언만으로 자동 해제되도록 만드는 기능입니다.

        
            ✅ 실제 작동 방식 (컴파일러 내부 동작)

                using var s = new MemoryStream();
                // 이 시점에 s 사용
                컴파일시 다음과 같이 변환됩니다:

                MemoryStream s = new MemoryStream();
                try
                {
                    // s 사용
                }
                finally
                {
                    if (s != null)
                        s.Dispose();
                }

            ➡ 즉, 컴파일러가 자동으로 try-finally를 만들어줍니다.
            ✅ 예외가 발생하든 안 하든, 무조건 Dispose()가 보장됩니다.
       */

        using var stream1 = new MemoryStream(); // C# 8.0부터 가능
        Console.WriteLine("Using stream1...");

        using var stream2 = new MemoryStream(); // C# 8.0부터 가능
        Console.WriteLine("Using stream2...");

        Console.ReadLine();

    } // 여기서 자동으로 stream1.Dispose(), stream2.Dispose() 호출됨


    public class MyDualResource : IDisposable, IAsyncDisposable
    {
        private bool _disposed;

        public void Use() => Console.WriteLine("리소스 사용 중...");

        public void Dispose()
        {
            Console.WriteLine("Dispose() 호출됨 (동기)");
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("DisposeAsync() 호출됨 (비동기)");
            await Task.Delay(100); // 예시: 비동기 리소스 해제
            _disposed = true;
        }
    }


    static async void using_with_sync_n_async()
    {
        // 동기
        using var r1 = new MyDualResource(); // override IDisposable
        r1.Use();

        // 비동기
        await using var r2 = new MyDualResource(); // override IAsyncDisposable
        r2.Use();

        Console.ReadLine();
    } // 여기서 자동으로 r1.Dispose(), r2.DisposeAsync() 호출됨


    public static void Test()
    {
        //using_with_sync_n_async();

        //UsingDeclaration_what();
    }
}
