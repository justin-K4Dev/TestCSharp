using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;




namespace Collections
{
    public class Queue
    {
        static void queue_what()
        {
            /*
                자료구조 : 큐 (Queue)

                큐 (Queue)는 먼저 추가된 데이타가 먼저 출력 처리되는(FIFO, First In First Out) 자료 구조로서
                입력된 순서대로 처리해야 하는 상황에 이용된다.
                Queue는 맨 뒤(tail)에 데이타를 계속 추가하고, 맨 앞(head)에서만 데이타를 읽기 때문에
                순차적으로 데이타를 처리하게 된다. 
            */
            {
                Console.ReadLine();
            }
        }


        static void Queue_use()
        {
            /*
                .NET에는 큐를 구현한 Queue클래스와 이의 Generic 형태인 Queue<T> 클래스가 있다.
                이 Queue클래스는 내부적으로 순환 배열 (Circular Array)로 구현되어 있는데,
                배열의 마지막 요소에 다다른 경우 다시 배열 처음 요소로 순환하는 구조(next % arrsize)를 가지고 있다.
                Queue는 내부적으로 head와 tail 포인터를 가지고 있는데,
                tail에 데이타를 추가하고(Enqueue) head에서 데이타를 읽고 제거(Dequeue)한다.
                만약 데이타 양이 많아 순환 배열이 모두 찰 경우,
                Queue는 Capacity를 2배로 (디폴트 Growth Factor가 2이다) 증가시키고,
                모든 배열 요소를 새 순환 배열에 자동으로 복사하여 큐를 확장한다.
            */
            {
                Queue<int> q = new Queue<int>();
                q.Enqueue(120);
                q.Enqueue(130);
                q.Enqueue(150);

                int next = q.Dequeue(); // 120
                next = q.Dequeue(); // 130

                Console.ReadLine();
            }
        }


        static void ConcurrentQueue_use()
        {
            /*
                멀티쓰레딩 환경에서 위의 Queue 클래스를 사용하기 위해서는
                전통적인 방식인 lock 을 사용하는 방법과 Queue.Synchronized() 를 사용하여
                Thread-safe한 Wrapper 큐를 사용하는 방법이 있다.

                .NET 4.0 부터 멀티쓰레딩 환경에서 큐를 보다 간편하게 사용할 수 있는
                새로운 클래스인 ConcurrentQueue<T> 가 제공되었다.

                Queue 클래스와 비슷하게 ConcurrentQueue 의 기본 동작 메서드는 Enqueue() 와 TryDequeue() 인데,
                ConcurrentQueue 에는 Dequeue() 메서드가 없고 대신 TryDequeue() 메서드를 사용한다.

                또한 마찬가지로 ConcurrentQueue에서는 Peek() 메서드 대신 TryPeek() 메서드를 사용한다.

                아래 예제는 하나의 쓰레드가 큐(ConcurrentQueue)에 0부터 99까지 계속 집어 넣을 때,
                동시에 다른 쓰레드에서는 계속 큐에서 데이타를 빼내 읽어 오는 작업을 하는 샘플 코드이다.
            */
            {
                var q = new ConcurrentQueue<int>();

                // 데이타를 큐에 넣는 쓰레드
                Task tEnq = Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        q.Enqueue(i);
                        Thread.Sleep(100);
                    }
                });

                // 데이타를 큐에서 읽는 쓰레드
                Task tDeq = Task.Factory.StartNew(() =>
                {
                    int n = 0;
                    int result;
                    while (n < 100)
                    {
                        if (q.TryDequeue(out result))
                        {
                            Console.WriteLine(result);
                            n++;
                        }
                        Thread.Sleep(100);
                    }
                });

                // 두 쓰레드가 끝날 때까지 대기
                Task.WaitAll(tEnq, tDeq);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //ConcurrentQueue_use();

            //Queue_use();

            //queue_what();
        }
    }
}
