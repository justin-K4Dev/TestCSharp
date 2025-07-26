using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Collections;


namespace MultiThread
{
    public class Monitor
    {
        class Caculator
        {
            private int counter = 1000;

            // lock 문에 사용될 객체
            private object lockObject = new object();

            public void Run()
            {
                // 10개의 쓰레드가 동일 메서드 실행
                for (int i = 0; i < 10; i++)
                {
                    new System.Threading.Thread(SafeCalc).Start();
                }
            }

            // Thread-Safe 메서드 
            private void SafeCalc()
            {
                // 한번에 한 쓰레드만 lock블럭 실행
                System.Threading.Monitor.Enter(lockObject);
                try
                {
                    // 필드값 변경
                    counter++;

                    // 가정 : 다른 복잡한 일을 한다
                    for (int i = 0; i < counter; i++)
                        for (int j = 0; j < counter; j++) ;

                    // 필드값 읽기
                    Console.WriteLine(counter);
                }
                finally
                {
                    System.Threading.Monitor.Exit(lockObject);
                }
            }
        }

        static void Monitor_what()
        {
            /*
                Monitor 클래스는 C#의 lock과 같이 특정 코드 블럭(Critiacal Section)을 배타적으로 Locking하는 기능을 가지고 있다.
                Monitor.Enter() 메서드는 Critiacal Section 블럭을 시작하여 한 쓰레드만 블럭으로 들어가게 하며,
                Monitor.Exit()은 락킹을 해제하여 다음 쓰레드가 Critiacal Section 블럭을 실행하게 한다.
                C# lock은 실제로는 Monitor.Enter()와 Monitor.Exit()을 사용하여
                Critiacal Section을 try...finally 문으로 감싼 문장들로 컴파일시 코드를 변경하는 것이다.
                즉, 자주 사용되는 Monitor.Enter - Exit의 간략한 표현으로 볼 수 있다. 
            */
            {
                Caculator c = new Caculator();
                c.Run();

                Console.ReadLine();
            }

        }


        class MessageUpdator
        {
            static Queue Q = new Queue();
            static object lockObj = new object();
            public static bool running = true;

            public static void WriteQueue(object val)
            {
                lock (lockObj)
                {
                    Q.Enqueue(val);
                    Console.WriteLine("W:{0}", val);
                    System.Threading.Monitor.Pulse(lockObj);
                }
            }

            public static void ReadQueue()
            {
                while (running)
                {
                    lock (lockObj)
                    {
                        while (Q.Count == 0)
                        {
                            System.Threading.Monitor.Wait(lockObj);
                        }

                        for (int i = 0; i < Q.Count; i++)
                        {
                            int val = (int)Q.Dequeue();
                            Console.WriteLine("R:{0}", val);
                        }
                    }
                }
            }
        }


        static void Monitor_Wait_and_Pulse()
        {
            /*
                Monitor 클래스의 또 다른 중요한 메서드로 Wait()와 Pulse() / PulseAll()이 있다.

                Wait() 메서드는 현재 쓰레드를 잠시 중지하고, lock을 Release한 후,
                다른 쓰레드로부터 Pulse 신호가 올 때까지 대기한다.
                Wait에서 lock 이 Release 되었으므로 다른 쓰레드가 lock을 획득하고 작업을 실행한다.
                다른 쓰레드가 자신의 작업을 마치고 Pulse() 메서드를 호출하면
                대기중인 쓰레드는 lock을 획득하고 계속 다음 작업을 실행한다.

                Pulse() 메서드가 호출되었을 때, 만약 대기중인 쓰레드가 있다면,
                그 쓰레드가 계속 실행하게 되지만,
                만약 대기중인 쓰레드가 없다면, Pulse 신호는 없어진다.

                (이러한 Wait/Pulse는 개념적으로 AutoResetEvent과 같은 이벤트 개념과 비슷하다.
                하지만 AutoResetEvent는 Set() 메서드로 펄스 신호를 보내는데 대기중인 쓰레드가 없는 경우
                하나의 Pulse 신호가 있었다는 것을 계속 가지고 있게 된다.)

                Pulse() 메서드는 다음 한 개의 쓰레드만 계속 실행하게 하지만,
                PulseAll() 메서드는 현재 대기중인 모든 쓰레드를 풀어 실행하게 한다.

                Monitor 클래스의 Wait(), Pulse() 메서드를 사용하기 위해 한가지 전제 조건이 있는데,
                이는 이 메서드들이 lock 블럭 안에서 호출되어야 한다는 것이다.

                아래 코드는 10개의 작업 쓰레드들이 데이타를 Queue에 집어 넣고,
                하나의 읽기 쓰레드가 데이타를 계속 Queue에서 꺼내오는 샘플 예제이다.
            */
            {
                // reader 쓰레드 시작
                System.Threading.Thread reader = new System.Threading.Thread(MessageUpdator.ReadQueue);
                reader.Start();

                // writer 쓰레드들 시작
                List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
                for (int i = 0; i < 10; i++)
                {
                    var t = new System.Threading.Thread(new ParameterizedThreadStart(MessageUpdator.WriteQueue));
                    t.Start(i);
					threads.Add(t);
                }
				// 모든 writer가 종료될 때까지 대기
				threads.ForEach(p => p.Join());

                // reader 종료
                MessageUpdator.running = false;

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //Monitor_Wait_and_Pulse();

            //Monitor_what();
        }
    }
}
