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
				📚 스레드(Thread)
             
                  - C#에서 쓰레드를 만드는 기본적인 클래스로 System.Threading.Thread라는 클래스가 있다.
                  - 이 클래스의 생성자(Constructor)에 실행하고자 하는 메서드를 델리게이트로 지정한 후,
                    Thread클래스 객체에서 Start() 메서드를 호출하면 새로운 쓰레드가 생성되어 실행되게 된다.
                  - 아래 예는 동일 클래스 안의 Run() 메서드를 실행하는 쓰레드를 하나 생성한 후 실행시키는 예제이다.
                  - 예제에서는 기본적으로 생성된 메인 쓰레드에서도 동일하게 Run()메서드를 호출하고 있으므로,
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

        static void Thread_ContextSwitcning()
        {
            /*
		        ✅ 스레드 컨텍스트 스위칭(Thread Context Switching)

                  - CPU가 "지금까지 이 스레드가 실행하던 상태"를 저장하고,
                    "다른 스레드의 상태"를 불러와서 실행을 넘기는 것.
                  - 여기서 저장/복원되는 상태 = 컨텍스트(Context)
                    * CPU 레지스터 값들 (RIP/EIP, RSP/ESP, 일반 레지스터들)
                    * 스택 포인터
                    * 일부 CPU 플래그
                    * 스레드가 사용 중이던 커널 구조체 정보 (스레드 정보 블록 등)
                  - 이 과정은 커널이 한다. (user-mode 코드가 직접 하는 게 아님)

                  1. 왜 발생하나? (스레드가 바뀌는 순간들)
                    (1) 타임 슬라이스(Quantum) 만료
                      - OS는 각 스레드에게 "너 15ms 동안만 실행해" 같은 실행 시간을 준다.
                      - 시간이 끝나면 스케줄러가 "다음 스레드로" 바꾸면서 컨텍스트 스위칭.
                      - 이건 "비자발적(involuntary) 컨텍스트 스위치".

                    (2) 스레드가 스스로 대기 상태로 들어갈 때
                      - 예: Thread.Sleep, lock 진입 대기, WaitHandle.WaitOne, I/O 동기 호출 등
                      - 스레드가 "난 할 일 없으니 재운 다음, 다른 애 실행해"라고 양보하는 것.
                      - 이건 "자발적(voluntary) 컨텍스트 스위치".

                    (3) 우선순위가 높은 스레드가 깨어날 때
                      - 높은 우선순위 스레드가 runnable 되면, OS가 당장 돌리려고 지금 스레드를 멈출 수 있음.

                    (4) CPU 코어 수보다 runnable 스레드 수가 많을 때
                      - 예: 코어 4개인데 runnable 스레드가 20개면 계속 돌아가면서 교대해야 함.

                  2. 컨텍스트 스위칭은 왜 비용(cost)이 있나?
                    - 현재 스레드의 레지스터/스택 포인터 등을 저장
                    - 새 스레드의 레지스터/스택 포인터 등을 복원
                    - 커널 모드에 진입했다가(user -> kernel) 다시 유저 모드로 돌아옴
                    - CPU 캐시/파이프라인이 깨질 수 있음 (다른 스레드의 메모리를 갑자기 보게 되니까)
                    - 그래서 "스레드가 많다고 항상 빠른 건 아니다" 라는 말이 나옴

                  3. .NET / C# 에서의 스레드와 컨텍스트 스위칭
                    - C#의 Thread, ThreadPool 스레드는 결국 "OS 스레드 위에 올라간" 관리되는 스레드다.
                    - 스레드가 OS에 의해 스케줄될 때마다 위 1~3에서 설명한 컨텍스트 스위칭이 그대로 일어난다.
                    - 즉, C#이라고 해서 컨텍스트 스위칭이 특별히 사라지진 않는다.
                    - 우리가 Thread.Sleep(0) 또는 Thread.Yield() 같은 걸 호출하면
                      "나 지금 당장 CPU 안 써도 돼"라고 말하는 것이고,
                      이때도 스케줄러가 다른 스레드로 넘기면서 컨텍스트 스위칭이 발생할 수 있다.

                  4. lock 과 컨텍스트 스위칭
                    - 아래 코드처럼 여러 스레드가 같은 lock을 두고 경쟁하면,
                      들어가지 못한 스레드는 대기 상태로 들어가고,
                      OS는 그 스레드를 잠깐 빼놓고 다른 스레드를 실행한다.
                    - 이때도 문맥이 바뀌기 때문에 컨텍스트 스위칭이 일어난다.

                        lock (_obj)
                        {
                            // 어떤 공유 작업
                        }

                    - 즉, "경쟁이 심한 lock"은 단순히 코드가 기다리는 것뿐 아니라
                      "스레드를 잠재웠다가 깨우는 오버헤드"까지 같이 들 수 있다.
                    - 그래서 병렬 코드에서는 "락을 짧게" 또는 "락 없는 구조"가 중요하다.

                  5. ThreadPool 과 컨텍스트 스위칭
                    - ThreadPool은 스레드 수를 알아서 조절하는 풀이다.
                    - 하지만 풀 안에 스레드가 여러 개라면, OS 입장에서는 여전히 "여러 스레드를 돌려가며 실행"해야 하므로
                      컨텍스트 스위칭은 그대로 존재한다.
                    - 단지 스레드 생성을 자주 안 해서 이 부분 비용을 줄일 뿐,
                      "스레드 간 교대"라는 현상 자체는 사라지지 않는다.

                  6. async/await 와 비교해서 보기
                    - async/await는 "대기하는 동안 스레드를 점유하지 않는다"는 점이 핵심이다.
                    - I/O 대기 중인 스레드를 빼놓고 다른 스레드를 실행시키므로 전체적으로 컨텍스트 스위칭 횟수를 줄이는 데 도움이 된다.
                    - 그러나 "대기가 끝났을 때 다른 스레드풀 스레드에서 이어서 실행"하면,
                      그 시점에서는 결국 또 다른 스레드가 실행하는 것이므로 "전환"이 일어났다고 볼 수 있다.
                    - 즉, async/await는 "불필요하게 CPU를 잡고있는 스레드"를 없애주지만,
                      "스레드가 여러 개면 OS는 여전히 교대 실행을 한다"는 사실은 변함없다.

                  7. 컨텍스트 스위칭을 관찰해볼 수 있는 아주 단순한 예시
                    - 아래 예시는 실제로 "지금 컨텍스트 스위칭이 몇 번 일어났다"를 정확히 찍어주는 건 아니지만,
                      스레드가 여러 개가 되면 출력이 섞이거나 순서가 왔다 갔다 하면서
                      "CPU가 스레드들을 번갈아 가며 실행하는구나"를 체감할 수 있다.

                        using System;
                        using System.Threading;

                        class Program
                        {
                            static void Main()
                            {
                                // 스레드를 여러 개 만들어서 CPU를 바쁘게 한다.
                                for (int i = 0; i < 4; i++)
                                {
                                    int id = i;
                                    var t = new Thread(() => Worker(id));
                                    t.IsBackground = true;
                                    t.Start();
                                }

                                Console.ReadLine();
                            }

                            static void Worker(int id)
                            {
                                // 바쁜 루프를 돌면서 현재 스레드ID를 찍어본다.
                                // 출력이 섞이거나 순서가 흐트러지는 부분에서
                                // OS가 스레드를 계속 번갈아 실행하고 있음을 볼 수 있다.
                                while (true)
                                {
                                    // ManagedThreadId는 .NET이 부여한 스레드 식별자
                                    Console.WriteLine($"[thread {id}] ManagedThreadId={Thread.CurrentThread.ManagedThreadId}");
                                    // 너무 정신없이 찍히지 않게 잠깐 슬립
                                    Thread.Sleep(10);
                                }
                            }
                        }

                    - 위 코드에서 Thread.Sleep(10)을 빼버리면, 한 스레드가 너무 공격적으로 돈 탓에
                      다른 스레드에게 CPU가 잘 안 가는 것처럼 보일 수도 있다.
                      Sleep/Wait 같은 호출이 "자발적 컨텍스트 스위치"를 유도한다는 점을 이렇게 관찰할 수 있다.

                  8. 줄이는 방법(원칙만)
                    - 스레드 수를 코어 수 이상으로 너무 많이 만들지 않는다.
                    - 락 경쟁을 줄인다 (lock 범위를 줄이고, 분리 가능한 데이터는 스레드마다 따로)
                    - 불필요한 blocking 호출(Thread.Sleep, 동기 I/O, lock 대기)을 줄인다.
                    - CPU-bound 작업은 Parallel/Task로 적절히 쪼개되, 너무 잘게 쪼개서 스케줄 오버헤드가 커지지 않게 한다.

                  9. 한 줄 요약
                    - "스레드가 여러 개면 OS는 반드시 교대로 실행한다 → 이게 컨텍스트 스위칭이다."
                    - "이 교대는 공짜가 아니다 → 너무 자주 일어나게 만들면 성능이 떨어진다."
                    - ".NET이라고 마법처럼 사라지는 게 아니다 → 우리가 락/스레드 개수/대기패턴을 잘 설계해야 한다."
            */
            {

            }
        }



        public static void Test()
        {
            //Thread_ContextSwitcning();

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
