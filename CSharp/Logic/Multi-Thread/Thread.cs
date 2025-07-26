using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.ObjectModel;


namespace MultiThread
{
    public class Thread
    {
        static void Run()
        {
            Console.WriteLine("Thread#{0}: Begin", System.Threading.Thread.CurrentThread.ManagedThreadId);
            // Do Something
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("Thread#{0}: End", System.Threading.Thread.CurrentThread.ManagedThreadId);
        }


        static void Thread_what()
        {
			/*
                C#에서 쓰레드를 만드는 기본적인 클래스로 System.Threading.Thread라는 클래스가 있다.
                이 클래스의 생성자(Constructor)에 실행하고자 하는 메서드를 델리게이트로 지정한 후,
                Thread클래스 객체에서 Start() 메서드를 호출하면 새로운 쓰레드가 생성되어 실행되게 된다.
                아래 예는 동일 클래스 안의 Run() 메서드를 실행하는 쓰레드를 하나 생성한 후 실행시키는 예제이다.
                예제에서는 기본적으로 생성된 메인 쓰레드에서도 동일하게 Run()메서드를 호출하고 있으므로,
                Begin/End문장이 2번 출력되고 있는데,
                이는 2개의 쓰레드가 동시에 한 메서드를 실행하고 있기 때문이다.
            */
			{
				// 새로운 쓰레드에서 Run() 실행
				System.Threading.Thread t1 = new System.Threading.Thread(new ThreadStart(Run));
				t1.Start();

				// 메인쓰레드에서 Run() 실행
				Run();
			}

            Console.ReadLine();
        }

        static Int32 calculate_active_thread_count()
        {
			return ((IEnumerable)Process.GetCurrentProcess().Threads)
				   .OfType<System.Diagnostics.ProcessThread>()
				   .Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running)
				   .Count();
		}


		static ConcurrentDictionary<Int32, string> thread_ids = new ConcurrentDictionary<Int32, string>();

		static Int32 calculate_thread_count(Int32 sleepMS)
        {
		    Int32[] to_load_datas =
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35
			};

			var main_thread_id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            var thread_id_string = $"MainTID:{main_thread_id}, Main:{"nothing"}";
            thread_ids.TryAdd(main_thread_id, thread_id_string);

			System.Threading.Tasks.Parallel.ForEach(to_load_datas, x =>
			{
                var curr_thread_id = System.Threading.Thread.CurrentThread.ManagedThreadId;
				thread_id_string = $"ParaTID:{curr_thread_id}, Value:{x}";
				thread_ids.TryAdd(curr_thread_id, thread_id_string);

                Console.WriteLine($"Curr Count by ActiveThread: {calculate_active_thread_count()}");

				Console.WriteLine($"Curr Count by CurrProc: {Process.GetCurrentProcess().Threads.Count}");

				Console.WriteLine($"Curr Count by CurrProcSum: {Process.GetProcesses().Sum(p => p.Threads.Count)}");

				System.Threading.Thread.Sleep(sleepMS);
			});

            return thread_ids.Count;
		}

		static void Thread_optimal_count()
        {
			// 적절한 스레드 개수 산정 방법
			var thread_count = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0));
			Console.WriteLine($"Calculate thread count:{thread_count}");

            // 실행중에 스레드 개수 얻기
			var sleep_ms_per_thread = 5000;
			var calculated_count = calculate_thread_count(sleep_ms_per_thread);
            Console.WriteLine($"Mount thread count:{calculated_count}, sleepPerThread:{sleep_ms_per_thread}");

			Console.ReadLine();
		}

        static void Thread_info()
        {
			var curr_process = Process.GetCurrentProcess();
			var curr_threads = curr_process.Threads;

            {
				var i = 0;
				foreach (ProcessThread pt in curr_threads)
				{
					Console.WriteLine("******* {0} 번째 스레드 정보 *******", i++);
					Console.WriteLine("ThreadId : {0}", pt.Id);           //스레드 ID
					Console.WriteLine("시작시간 : {0}", pt.StartTime);    //스레드 시작시간
					Console.WriteLine("우선순위 : {0}", pt.BasePriority); //스레드 우선순위
					Console.WriteLine("상태 : {0}", pt.ThreadState);      //스레드 상태
					Console.WriteLine();
				}
			}

			{
				int curr_tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
				var found_thread = (from ProcessThread entry in curr_threads
									where entry.Id == curr_tid
									select entry).FirstOrDefault();
				if (null != found_thread)
				{
					Console.WriteLine("Found ThreadId : {0}", found_thread.Id);           //스레드 ID
					Console.WriteLine("시작시간 : {0}", found_thread.StartTime);    //스레드 시작시간
					Console.WriteLine("우선순위 : {0}", found_thread.BasePriority); //스레드 우선순위
					Console.WriteLine("상태 : {0}", found_thread.ThreadState);      //스레드 상태
					Console.WriteLine();
				}
			}

			Console.ReadLine();
		}

        static void Thread_with_delegate()
        {
            /*
                이 섹션은 .NET의 Thread 클래스를 이용해 쓰레드를 만드는 다양한 예를 들고 있다.
                Thread클래스의 생성자가 받아들이는 파라미터는
                ThreadStart 델리게이트와 ParameterizedThreadStart 델리게이트가 있는데,
                이 섹션은 파라미터를 직접 전달하지 않는 메서드들에 사용하는 ThreadStart 델리게이트 사용 예제를 보여준다.
                ThreadStart 델리게이트는 public delegate void ThreadStart();와 같이 정의되어 있는데,
                리턴값과 파라미터 모두 void임을 알 수 있다.
                따라서 파라미터와 리턴값이 없는 메서드는 델리게이트 객체로 생성될 수 있다.
                아래 예에서 보이듯이, ThreadStart 델리게이트를 만족하는 다른 방식들
                즉, 익명 메서드, 람다식 등도 모두 사용할 수 있다.
            */
            {
                // Run 메서드를 입력받아
                // ThreadStart 델리게이트 타입 객체를 생성한 후
                // Thread 클래스 생성자에 전달
                System.Threading.Thread t1 = new System.Threading.Thread(new ThreadStart(Run));
                t1.Start();

                // 컴파일러가 Run() 메서드의 함수 프로토타입으로부터
                // ThreadStart Delegate객체를 추론하여 생성함
                System.Threading.Thread t2 = new System.Threading.Thread(Run);
                t2.Start();

                // 익명메서드(Anonymous Method)를 사용하여
                // 쓰레드 생성
                System.Threading.Thread t3 = new System.Threading.Thread(delegate ()
                {
                    Run();
                });
                t3.Start();

                // 람다식 (Lambda Expression)을 사용하여
                // 쓰레드 생성
                System.Threading.Thread t4 = new System.Threading.Thread(() => Run());
                t4.Start();

                // 간략한 표현
                new System.Threading.Thread(() => Run()).Start();

                Console.ReadLine();
            }
        }


        class Helper
        {
            public void Run()
            {
                Console.WriteLine("Helper.Run");
            }
        }

        static void Thread_with_other_class()
        {
            /*
                동일 클래스가 아닌 다른 클래스의 메서드를 쓰레드에 호출하기 위해서는
                해당 클래스의 객체를 생성한 후 (혹은 외부로부터 전달 받은 후) 그 객체의 메서드를
                델리게이트로 Thread에 전달하면 된다. 
            */

            // Helper클래스의 Run메서드 호출
            Helper obj = new Helper();
            System.Threading.Thread t = new System.Threading.Thread(obj.Run);
            t.Start();

            Console.ReadLine();
        }

        // radius라는 파라미터를 object 타입으로 받아들임
        static void Calc(object radius)
        {
            double r = (double)radius;
            double area = r * r * 3.14;
            Console.WriteLine("r={0},area={1}", r, area);
        }

        static void Sum(int d1, int d2, int d3)
        {
            int sum = d1 + d2 + d3;
            Console.WriteLine(sum);
        }

        static void Thread_parameter_passing()
        {
            /*
                Thread 클래스는 파라미터를 전달하지 않는 ThreadStart 델리게이트와 파라미터를 직접 전달하는
                ParameterizedThreadStart 델리게이트를 사용할 수 있다.
                ThreadStart 델리게이트는 public delegate void ThreadStart(); 프로토타입에서 알 수 있듯이,
                파라미터를 직접 전달 받지 않는다.
                (물론 파라미터를 전달하는 방식은 있다. 아래 참조)
                ParameterizedThreadStart 델리게이트는
                public delegate void ParameterizedThreadStart(object obj);로 정의되어 있는데,
                하나의 object 파라미터를 전달하고 리턴 값이 없는 형식이다.
                하나의 파라미터를 object 형식으로 전달하기 때문에,
                여러 개의 파라미터를 전달하기 위해서는 클래스나 구조체를 만들어 객체를 생성해서 전달할 수 있다.
                파라미터의 전달은 Thread.Start() 메서드를 호출할 때 파라미터를 전달한다.
                ThreadStart를 이용해 파라미터를 전달하는 방법은
                일단 델리게이트 메서드는 파라미터를 받아들이지 않으므로
                그 메서드 안에서 다른 메서드를 호출하면서 파라미터를 전달하는 방식을 사용할 수 있다.
                이렇게 하면 파라미터를 아래 t3의 예처럼 여러 개 전달할 수도 있다. 
            */

            // 파라미터 없는 ThreadStart 사용
            System.Threading.Thread t1 = new System.Threading.Thread(new ThreadStart(Run));
            t1.Start();

            // ParameterizedThreadStart 파라미터 전달
            // Start()의 파라미터로 radius 전달
            System.Threading.Thread t2 = new System.Threading.Thread(new ParameterizedThreadStart(Calc));
            t2.Start(10.00);

            // ThreadStart에서 파라미터 전달
            System.Threading.Thread t3 = new System.Threading.Thread(() => Sum(10, 20, 30));
            t3.Start();

            Console.ReadLine();
        }

		static void background_and_foreground_Thread()
		{
			/*
                Thread 클래스 객체를 생성한 후 Start()를 실행하기 전에
                IsBackground 속성을 true/false로 지정할 수 있는데,
                만약 true로 지정하면 이 쓰레드는 백그라운드 쓰레드가 된다.
                디폴트 값은 false 즉 Foreground 쓰레드이다.
                백그라운드와 Foreground 쓰레드의 기본적인 차이점은
                Foreground 쓰레드는 메인 쓰레드가 종료되더라도 Foreground 쓰레드가 살아 있는 한,
                프로세스가 종료되지 않고 계속 실행되고, 백그라운드 쓰레드는 메인 쓰레드가 종료되면
                바로 프로세스를 종료한다는 점이다.
            */

			// Foreground Thread
			System.Threading.Thread t1 = new System.Threading.Thread(new ThreadStart(Run));
			t1.Start();

			// Backgroud Thread
			System.Threading.Thread t2 = new System.Threading.Thread(new ThreadStart(Run));
			t2.IsBackground = true;
			t2.Start();

			Console.ReadLine();
		}

		static void Thread_with_Join()
        {
			// Thread 생성 (생성된 Thread가 동작시킬 함수)
			var t = new System.Threading.Thread(new ThreadStart(Run));
			t.IsBackground = true;

			// Thread 시작
			t.Start();
			Console.WriteLine("Waiting for Thread");

			// background Thread가 종료될때 까지 대기
			t.Join();

			Console.ReadLine();
		}

		// Thread 2: The listener
		static void DoWork(object obj)
		{
			CancellationToken token = (CancellationToken)obj;

			while(true)
			{
				if (true == token.IsCancellationRequested)
				{
					Console.WriteLine("Cancellation has been requested...");
					// Perform cleanup if necessary.
					//...
					// Terminate the operation.
					break;
				}

				// Simulate some work.
				System.Threading.Thread.SpinWait(1000);
			}
		}

		static void Thread_with_CancellationTokenSource()
        {
			// Create the token source.
			CancellationTokenSource cts = new CancellationTokenSource();

			// Pass the token to the cancelable operation.
			System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), cts.Token);
			System.Threading.Thread.Sleep(3000);

			// Request cancellation.
			cts.Cancel();
			Console.WriteLine("Cancellation set in token source...");
			System.Threading.Thread.Sleep(5000);
			// Cancellation should have happened, so call Dispose.
			cts.Dispose();

			Console.ReadLine();
		}

		class CancelableObject
		{
			public string id;

			public CancelableObject(string id)
			{
				this.id = id;
			}

			public void Cancel()
			{
				Console.WriteLine("Object {0} Cancel callback", id);
				// Perform object cancellation here.
			}
		}

		static void Thread_with_cancel_by_object()
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationToken token = cts.Token;

			// User defined Class with its own method for cancellation
			var obj1 = new CancelableObject("1");
			var obj2 = new CancelableObject("2");
			var obj3 = new CancelableObject("3");

			// Register the object's cancel method with the token's
			// cancellation request.
			token.Register(() => obj1.Cancel());
			token.Register(() => obj2.Cancel());
			token.Register(() => obj3.Cancel());

			// Request cancellation on the token.
			cts.Cancel();
			// Call Dispose when we're done with the CancellationTokenSource.
			cts.Dispose();

			Console.ReadLine();
		}


		public static void Test()
        {
			//Thread_with_cancel_by_object();

			//Thread_with_CancellationTokenSource();

			//Thread_with_Join();

			//background_and_foreground_Thread();

			//Thread_parameter_passing();

			//Thread_with_other_class();

			//Thread_with_delegate();

			//Thread_info();

			//Thread_optimal_count();
    
			//Thread_what();
		}
	}
}
