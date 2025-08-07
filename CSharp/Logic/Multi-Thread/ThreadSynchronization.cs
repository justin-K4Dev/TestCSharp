using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread
{
    public class ThreadSynchronization
    {
        static int counter = 0;
        static object lockObj = new object();
        static System.Threading.Mutex mutex = new System.Threading.Mutex();
        static System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(2); // 최대 2개 동시 허용
        static System.Threading.AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(false);
        static System.Threading.ManualResetEvent manualEvent = new System.Threading.ManualResetEvent(false);
        static System.Threading.SpinLock spinLock = new System.Threading.SpinLock();
        static System.Threading.ReaderWriterLockSlim rwLock = new System.Threading.ReaderWriterLockSlim();

        static void thread_synch_what()
        {
            /*
                Thread Synchronization - Thread Safe & Unsafe
                
                한 메서드를 복수의 쓰레드가 동시에 실행하고 그 메서드에서 클래스 객체의 필드들을 읽거나 쓸 때,
                복수의 쓰레드가 동시에 필드값들은 변경할 수 있게 된다.
                객체의 필드값은 모든 쓰레드가 자유롭게 엑세스할 수 있기 때문에,
                메서드 실행 결과가 잘못될 가능성이 크다.
                이렇게 쓰레드들이 공유된 자원(예를 들어 객체 필드)을 동시에 접근하는 것을 막고,
                각 쓰레드들이 순차적으로 혹은 제한적으로 접근하도록 하는 것이
                쓰레드 동기화 (Thread Synchronization)이다.
                또한 이렇게 쓰레드 동기화를 구현한 메서드나 클래스를 Thread-Safe하다고 한다.
                .NET의 많은 클래스들은 Thread-Safe하지 않는데,
                이는 Thread-Safe를 구현하려면 Locking 오버헤드와 보다 많은 코딩을 요구하는데,
                실제 실무에서 이러한 Thread-Safe를 필요로 하지 않는 경우가 더 많기 때문이다.
            */
            {
                Console.ReadLine();
            }
        }


        static void LockWorker(object tag)
        {
            for (int i = 0; i < 5; i++)
            {
                lock (lockObj)
                {
                    int tmp = counter;
                    System.Threading.Thread.Sleep(10);
                    counter = tmp + 1;
                    Console.WriteLine("{0} (lock): {1}", tag, counter);
                }
            }
        }

        static void MutexWorker(object tag)
        {
            for (int i = 0; i < 5; i++)
            {
                mutex.WaitOne();
                try
                {
                    int tmp = counter;
                    System.Threading.Thread.Sleep(10);
                    counter = tmp + 1;
                    Console.WriteLine("{0} (mutex): {1}", tag, counter);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        static void SemaphoreWorker(object tag)
        {
            Console.WriteLine("{0}: 세마포어 대기", tag);
            semaphore.Wait();
            try
            {
                Console.WriteLine("{0}: 세마포어 진입", tag);
                System.Threading.Thread.Sleep(100);
            }
            finally
            {
                Console.WriteLine("{0}: 세마포어 퇴장", tag);
                semaphore.Release();
            }
        }

        static void AutoResetWorker(object state)
        {
            Console.WriteLine("AutoResetEvent 대기중...");
            autoEvent.WaitOne();
            Console.WriteLine("AutoResetEvent 신호 받음!");
        }

        static void ManualResetWorker(object state)
        {
            Console.WriteLine("ManualResetEvent 대기중...");
            manualEvent.WaitOne();
            Console.WriteLine("ManualResetEvent 신호 받음!");
        }

        static void SpinLockWorker(object tag)
        {
            for (int i = 0; i < 5; i++)
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    int tmp = counter;
                    System.Threading.Thread.Sleep(10);
                    counter = tmp + 1;
                    Console.WriteLine("{0} (spinlock): {1}", tag, counter);
                }
                finally
                {
                    if (lockTaken) spinLock.Exit();
                }
            }
        }

        static void ReadWorker(object state)
        {
            rwLock.EnterReadLock();
            try
            {
                Console.WriteLine("Reader {0}: counter={1}", System.Threading.Thread.CurrentThread.ManagedThreadId, counter);
                System.Threading.Thread.Sleep(50);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        static void WriteWorker(object state)
        {
            rwLock.EnterWriteLock();
            try
            {
                counter++;
                Console.WriteLine("Writer: counter={0}", counter);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        static void dotnet_thread_synch()
        {
            /*
                .NET classes for thread synchronization 

                쓰레드 동기화를 위하여 .NET에는 (Version에 따라 다르지만) 많은 클래스와 메서드들이 있다.
                이 중 중요한 것들로서 Monitor, Mutex, Semaphore, SpinLock, ReaderWriterLock, AutoResetEvent, ManualResetEvent 등이 있으며,
                C# 키워드로는 lock, await 등이 있다.
                쓰레드 동기화를 위해 자주 사용되는 방식으로서,
                (1) Locking으로 공유 리소스에 대한 접근을 제한하는 방식으로 C# lock, Monitor, Mutex, Semaphore, SpinLock, ReaderWriterLock 등이 사용되며,
                (2) 타 쓰레드에 신호(Signal)을 보내 쓰레드 흐름을 제어하는 방식으로 AutoResetEvent, ManualResetEvent, CountdownEvent 등이 있다.
            */
            {
                Console.WriteLine("=== [lock] ===");
                counter = 0;
                var t1 = new System.Threading.Thread(LockWorker);
                var t2 = new System.Threading.Thread(LockWorker);
                t1.Start("T1"); t2.Start("T2");
                t1.Join(); t2.Join();

                Console.WriteLine("\n=== [Mutex] ===");
                counter = 0;
                var m1 = new System.Threading.Thread(MutexWorker);
                var m2 = new System.Threading.Thread(MutexWorker);
                m1.Start("M1"); m2.Start("M2");
                m1.Join(); m2.Join();

                Console.WriteLine("\n=== [SemaphoreSlim] ===");
                var s1 = new System.Threading.Thread(SemaphoreWorker);
                var s2 = new System.Threading.Thread(SemaphoreWorker);
                var s3 = new System.Threading.Thread(SemaphoreWorker);
                s1.Start("S1"); s2.Start("S2"); s3.Start("S3");
                s1.Join(); s2.Join(); s3.Join();

                Console.WriteLine("\n=== [AutoResetEvent] ===");
                var autoWaiter = new System.Threading.Thread(AutoResetWorker);
                autoWaiter.Start();
                System.Threading.Thread.Sleep(500);
                autoEvent.Set();
                autoWaiter.Join();

                Console.WriteLine("\n=== [ManualResetEvent] ===");
                var manualWaiter = new System.Threading.Thread(ManualResetWorker);
                manualWaiter.Start();
                System.Threading.Thread.Sleep(500);
                manualEvent.Set();
                manualWaiter.Join();
                manualEvent.Reset();

                Console.WriteLine("\n=== [SpinLock] ===");
                counter = 0;
                var sp1 = new System.Threading.Thread(SpinLockWorker);
                var sp2 = new System.Threading.Thread(SpinLockWorker);
                sp1.Start("SP1"); sp2.Start("SP2");
                sp1.Join(); sp2.Join();

                Console.WriteLine("\n=== [ReaderWriterLockSlim] ===");
                counter = 0;
                var rw1 = new System.Threading.Thread(ReadWorker);
                var rw2 = new System.Threading.Thread(ReadWorker);
                var rw3 = new System.Threading.Thread(WriteWorker);
                rw1.Start(); rw2.Start();
                System.Threading.Thread.Sleep(100); // Read 우선
                rw3.Start();
                rw1.Join(); rw2.Join(); rw3.Join();

                Console.WriteLine("\n=== 모든 테스트 종료 ===");
                Console.ReadLine();

                Console.ReadLine();
            }
        }


        class Calculator
        {
            private int counter = 1000;

            // lock문에 사용될 객체
            private object lockObject = new object();

            public void RunThreadUnsafe()
            {
                // 10개의 쓰레드가 동일 메서드 실행
                for (int i = 0; i < 10; i++)
                {
                    new System.Threading.Thread(UnsafeCalc).Start();
                }
            }

            // Thread-Safe하지 않은 메서드 
            private void UnsafeCalc()
            {
                // 객체 필드를 모든 쓰레드가 
                // 자유롭게 변경
                counter++;

                // 가정 : 다른 복잡한 일을 한다
                for (int i = 0; i < counter; i++)
                {
                    for (int j = 0; j < counter; j++) ;
                }

                // 필드값 읽기
                Console.WriteLine(counter);
            }

            public void RunThreadSafe()
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
                lock (lockObject)
                {
                    // 필드값 변경
                    counter++;

                    // 가정 : 다른 복잡한 일을 한다
                    for (int i = 0; i < counter; i++)
                        for (int j = 0; j < counter; j++) ;

                    // 필드값 읽기
                    Console.WriteLine(counter);
                }
            }
        }

        static void thread_unsafe()
        {
            /*
                멀티쓰레드는 필드의 내용을 동시에 엑세스하여 잘못된 결과를 만들거나 출력할 수 있는데,
                이를 Thread Unsafe 하다고 한다.
                아래 예제는 여러 개의 쓰레드가 Thread-Safe하지 않은 메서드를 호출하는 예를 보여주고 있다.
                10개의 쓰레드가 counter라는 필드를 동시에 쓰거나 읽는 샘플로서
                한 쓰레드가 counter변수를 변경하고 읽기 전에 다른 쓰레드가 다시 counter변수를 변경할 수 있기 때문에
                불확실한 결과를 출력하게 된다. 
            */
            {
                Calculator c = new Calculator();
                c.RunThreadUnsafe();

                Console.ReadLine();
            }
        }


        static void thread_safe()
        {
            /*
                C#의 lock 키워드는 특정 블럭의 코드(Critical Section이라 부른다)를
                한번에 하나의 쓰레드만 실행할 수 있도록 해준다.
                lock()의 파라미터에는 임의의 객체를 사용할 수 있는데,
                주로 object 타입의 private 필드를 지정한다.
                즉, private object obj = new object() 와 같이 private 필드를 생성한 후, lock(obj)와 같이 지정한다.

                특히, 클래스 객체를 의미하는 this를 사용하여 lock(this)와 같이 잘못 사용할 수 있는데,
                이는 의도치 않게 데드락을 발생시키거나 Lock Granularity를 떨어뜨리는 부작용을 야기할 수 있다.
                즉, 불필요하게 객체 전체 범위에서 lock을 걸어
                다수의 메서드가 공유 리소스를 접근하는데 Lock Granularity를 떨어뜨리는 효과가 있으며
                (주: 이 경우 필드나 속성들이 잠긴다는 의미는 아니며, 이들 필드나 속성을 계속 읽고 쓸 수 있음),
                또한 만약 외부에서 해당 클래스 객체를 lock 하고 그 객체 안의 메서드를 호출한 경우
                그리고 그 메서드안에서 다시 lock(this)를 하는 경우,
                이미 외부에서 잡혀있는 lock이 풀리기를 메서드 내에서 계속 기다릴 것이므로 데드락이 발생할 수 있다.

                그리고 Critical Section 코드 블록은 가능한 한 범위를 작게하는 것이 좋은데,
                이는 필요한 부분만 Locking한다는 원칙에 따른 것이다.
            */
            {
                Calculator c = new Calculator();
                c.RunThreadSafe();

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //thread_safe();

            //thread_unsafe();

            //dotnet_thread_synch();

            //thread_synch_what();
        }
    }
}
