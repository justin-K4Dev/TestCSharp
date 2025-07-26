using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Threading;
using System.Diagnostics;


namespace MultiThread
{
    public class ThreadPool
    {
        static void Calc(object radius)
        {
            if (radius == null) return;

            double r = (double)radius;
            double area = r * r * 3.14;
            Console.WriteLine("r={0}, area={1}", r, area);
        }

        static void ThreadPool_what()
        {
			/*
                .NET의 Thread 클래스를 이용하여 쓰레드를 하나씩 만들어 사용하는 것이 아니라,
                이미 존재하는 쓰레드풀에서 사용가능한 작업 쓰레드를 할당 받아 사용하는 방식이 있는데,
                이는 다수의 쓰레드를 계속 만들어 사용하는 것보다 효율적이다.
                이렇게 시스템에 존재하는 쓰레드풀에 있는 쓰레드를 사용하기 위해서는
                     (1) ThreadPool 클래스,
                     (2) 비동기 델리게이트(Asynchronous delegate),
                     (3) .NET 4 Task 클래스,
                     (4) .NET 4 Task<T> 클래스,
                     (5) BackgroundWorker 클래스 등을 사용할 수 있다.
                이 중 ThreadPool 클래스의 경우, ThreadPool.QueueUserWorkItem() 를 사용하여
                실행하고자 하는 메서드 델리게이트를 지정하면 시스템풀에서 쓰레드를 할당하여 실행하게 된다.
                이 방식은 실행되는 메서드로부터 리턴 값을 돌려받을 필요가 없는 곳에 주로 사용된다.
                리턴값이 필요한 경우는 비동기 델리게이트(Asynchronous delegate)를 사용한다.

                쓰레드 생성시 요청되는 쓰레드수가 해당 컴퓨터의 CPU 수보다 많아지면,
                CLR 시스템은 쓰레드를 즉시 생성하지 않고 초당 2개의 쓰레드를 새로 생성되도록 지연하게 된다. (Thread Throttling)
            
                C# 에서 미리 Poolling 해둔 Thread를 가져다가 사용하고 해당 Thread가 종료되면 자동으로 Pool로 반납 된다.
                대신 ThreadPool에서 가져온 Thread를 오래 사용할 경우 ThreadPool로 반납이 안되어,
                ThreadPool의 Thread가 부족해질 수 있고, 그럴 경우 ThreadPool이 동작하지 않는 상황 발생 한다.
                따라서 간단한 작업을 할때 사용 하도록 한다. !!!
            */

			{
				// 쓰레드풀에 있는 쓰레드를 이용하여
				// Calc() 메서드 실행. 
				// 리턴값 없을 경우 사용.
				System.Threading.ThreadPool.QueueUserWorkItem(Calc); // radius=null
                System.Threading.ThreadPool.QueueUserWorkItem(Calc, 10.0); // radius=10
                System.Threading.ThreadPool.QueueUserWorkItem(Calc, 20.0);
            }

			Console.ReadLine();
		}

        static void ThreadPool_size()
        {
			// 스레드 풀에서 스레드 개수 얻기
			int workerThreads;
			int portThreads;
			System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
			Console.WriteLine($"GetMaxThreads() count - workerThreadCount:{workerThreads}, completionPortThreadCount:{portThreads}");
			System.Threading.ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);
			Console.WriteLine($"GetAvailableThreads() count - workerThreadCount:{workerThreads}, completionPortThreadCount:{portThreads}");

			Console.ReadLine();
		}


		static void Producer()
		{
			while (true)
			{
				// Process 작업을 Local Queue에 추가
				System.Threading.Tasks.Task.Factory.StartNew(Process);

				System.Threading.Thread.Sleep(200);
			}
		}

		static async Task Process()
		{
			var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();
			Guid guid = Guid.NewGuid();

			Task.Run(() =>
			{
				System.Threading.Thread.Sleep(1000);
				tcs.SetResult(true);
			});

			tcs.Task.Wait();

			Console.WriteLine($"Ended - {guid} {DateTime.Now} {tid} {mid}");
		}

		static int tid => AppDomain.GetCurrentThreadId();
		static int mid => System.Threading.Thread.CurrentThread.ManagedThreadId;

		static void ThreadPool_and_thread_queue()
        {
            /*
                .NET ThreadPool starvation, and how queuing makes it worse
                ; https://medium.com/criteo-labs/net-threadpool-starvation-and-how-queuing-makes-it-worse-512c8d570527			
			
                쓰레드 풀은 1개의 Global Queue와 스레드 풀 내의 쓰레드 별로 1개씩의 Local Queue 를 가집니다.

                ThreadPool - Global Queue
                    Thread #1 - LocalQueue
                    Thread #2 - LocalQueue
                    ...
                
                쓰레드 풀에 할당(Enqueue)할 때의 규칙
                
                - Global Queue에 추가하는 규칙
                  + 쓰레드 풀 외부의 스레드가 작업을 할당하는 경우 : 메인 스레드가 대표적...
                  + 쓰레드 풀 내부의 스레드가 작업을 할당하는 경우
                    * ThreadPool.QueueUserWorkItem or ThreadPool.UnsafeQueueUserWorkItem
                    * Task.Factory.StartNew with the TaskCreationOptions.PreferFairness
                    * Task.Yield on the default task scheduler
                  + 어차피 쓰레드 풀에 속하지 않은 외부의 스레드는 LocalQueue 를 소유하고 있지 않기 때문에 GlobalQueue 에 넣을 수밖에 없다.
              

                - Local Queue 에 추가하는 규칙
                  + 그 외의 모든 경우
                  + 특별히 LocalQueue 를 가지고 있는 ThreadPool 내의 스레드일지라도 위의 3가지 규칙을 제외하고는 모두 LocalQueue 에 넣는 것으로 이해 하자.
                
                큐에 할당된 작업을 스레드 풀의 여유 스레드가 가져가는(Dequeue) 규칙

                - ThreadPool 내의 스레드가 자유롭게 되면,
                  + 해당 스레드의 LocalQueue 에서 마지막 추가된(LIFO) 항목, 즉 큐의 tail에 있는 작업을 꺼내서 실행
                    ; 마지막에 추가된 항목, 즉 최근 추가된 항목을 처리하는 이유는 cache 의 locality 에 따른 적중률을 높이기 위해서...
                  + LocalQueue 가 비었으면 GlobalQueue 에서 오래된 항목(FIFO), 즉 큐의 head에 있는 작업을 꺼내서 실행
                    ; 어차피 GlobalQueue 라면 현재 스레드가 실행 중인 CPU의 cache 적중률이 높지 않을 것이므로 FIFO 로 처리...
                  + GlobalQueue도 비었으면, 다른 스레드의 LocalQueue에서 오래된 항목(FIFO)을 꺼내서 실행
                    ; 어차피 다른 스레드의 작업 항목이라면 마찬가지로 cache 적중률이 높지 않을 것이므로 FIFO 처리...
            */
            {
				System.Threading.ThreadPool.SetMinThreads(8, 8);

				Task.Factory.StartNew( Producer
					                 , TaskCreationOptions.None );

				Console.ReadLine();
			}

            Console.ReadLine();
		}


		static void threadPoolThreadFunc(object param)
		{
			var hashtable = param as System.Collections.Hashtable;

			int data = (int)hashtable["data"];

			// 인자로 받은 data를 처리하고 EventWaitHandle 을 Set
			data += 5;
			hashtable["data"] = data;

			(hashtable["eventwaithandle"] as EventWaitHandle).Set();
		}

		static void ThreadPool_with_EventWaitHandle()
		{
            var hashtable = new System.Collections.Hashtable();
			EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
			hashtable["data"] = 1;
			hashtable["eventwaithandle"] = ewh;

			System.Threading.ThreadPool.QueueUserWorkItem(threadPoolThreadFunc, hashtable);

			// 인자로 보낸 EventWaitHandle 이 set 될 때까지 대기
			ewh.WaitOne();

			Console.WriteLine("result: " + hashtable["data"]);

			Console.ReadLine();
		}

		public static void Test()
        {
			//ThreadPool_with_EventWaitHandle();

			//ThreadPool_and_thread_queue();

			//ThreadPool_size();

			//ThreadPool_what();
		}
	}
}
