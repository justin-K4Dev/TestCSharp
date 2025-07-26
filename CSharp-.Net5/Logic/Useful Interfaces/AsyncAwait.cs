using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace UsefulInterfaces;

public class AsyncAwait
{
    public static async IAsyncEnumerable<int> GetNumbersAsync()
    {
        for (int i = 1; i <= 5; i++)
        {
            await Task.Delay(1000); // 1초 대기
            yield return i;
        }
    }

    static async Task override_IAsyncEnumerableT()
    {
        /*
            IAsyncEnumerable<T>

            ✅ 목적
              - 비동기(Async) 스트림 처리를 위한 .NET 표준 인터페이스
              - 비동기 시퀀스(스트림) 데이터를 하나씩 await로 순회할 수 있게 하는 인터페이스
              - 비동기 foreach (await foreach) 구문으로, 데이터를 하나씩 "지연"해서,
                파일, 네트워크, DB 등 느리거나 대량 데이터를 효율적으로 처리할 때 사용
              - 비동기 메서드에서 yield return과 유사하게 하나씩 값을 "비동기적으로" 내보낼 수 있음.

            IAsyncEnumerator<T>

            ✅ 목적
              - IAsyncEnumerable<T>에서 비동기적으로 하나씩 요소를 꺼내는 열거자(Enumerator) 인터페이스
              - await MoveNextAsync()로 다음 데이터, Current로 현재 데이터, DisposeAsync()로 해제
        */

        {
            Console.WriteLine("Start await foreach...");
            await foreach (var num in GetNumbersAsync())
            {
                Console.WriteLine($"Received: {num}");
            }
            Console.WriteLine("Done!");
            /*
                Start await foreach...
                Received: 1
                Received: 2
                Received: 3
                Received: 4
                Received: 5
                Done! 
            */
        }

        {
            Console.WriteLine("Start await foreach...");
            // IAsyncEnumerator<T> 직접 사용 예제
            await using (var enumerator = GetNumbersAsync().GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    Console.WriteLine($"Current: {enumerator.Current}");
                }
            }
            Console.WriteLine("Done!");
            /*
                Current: 1
                Current: 2
                Current: 3
                Done!     
            */
        }
    }

    public static void Test()
    {
        override_IAsyncEnumerableT().Wait();
    }
}
