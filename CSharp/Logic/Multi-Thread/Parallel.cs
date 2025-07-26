using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;





namespace MultiThread
{
    public class Parallel
    {
        static void Parallel_what()
        {
            /*
                Parallel Programming

                CPU 가 하나였던 시대에서 2개, 4개의 CPU를 장착하는 것이 보편화 됨에 따라,
                이러한 복수 CPU를 충분히 활용하기 위한 프로그래밍 기법에 대한 요구가 증가하였다.
                .NET 4.0에서는 이러한 요구에 부합하기 위해
                Parallel Framework (PFX) 라 불리우는 병렬 프로그래밍 프레임워크를 추가하였다.
                병렬처리는 큰 일거리를 분할하는 단계, 분할된 작업들을 병렬로 실행하는 단계,
                그리고 결과를 집계하는 단계로 진행된다.
                일거리를 분할하는 방식은 크게 Data Parallelism과 Task Parallelism 으로 나누어 진다.
                Data Parallelism 은 대량의 데이타를 처리하는데 있어 각 CPU에 일감을 나눠서 주고
                동시에 병렬로 처리하는 것을 말한다.
                즉, 대량의 데이타를 분할하여 다중 CPU를 사용하여 다중 쓰레드들이 각각 할당된 데이타를 처리하는데
                일반적으로 쓰레드 당 처리 내용은 동일하다.
                Task Parallelism 은 큰 작업 Task 를 분할하여 각 쓰레드들이 나눠서 다른 작업 Task들을 실행하는 것이다.
                Parallel Framework (PFX) 은 크게 
                    (1) PLINQ (Parallel LINQ)와 Parallel 클래스 (For/Foreach 메서드)를 중심으로 하는 Data Parallelism 지원 클래스들과
                    (2) Task, TaskFactory 클래스 등 Task Parallelism 을 지원하는 클래스들로 구별할 수 있다.
                Parallel.Invoke() 는 Task Parallelism 을 지원한다.
            */
            {
                Console.ReadLine();
            }
        }


        static void Parallel_basic()
        {
            /*
                Parallel 클래스는 Parallel.For()와 Parallel.ForEach() 메서드를 통해
                다중 CPU에서 다중 쓰레드가 병렬로 데이타를 분할하여 처리하는 기능을 제공한다.
                아래 예제의 첫번째 for 루프는 하나의 쓰레드가 0부터 999까지 순차적으로 처리하게 되는 반면,
                두번째 Parallel.For() 문은 시스템이 다중 쓰레드들 생성하여
                각 쓰레드가 처리할 데이타를 분할하여 병렬로 실행하게 되기 때문에,
                0~999 번호 출력이 뒤죽박죽으로 표시된다.
                하지만 0~999까지 각 숫자는 단 한번만 출력된다.
            */
            {
                // 1. 순차적 실행
                // 동일쓰레드가 0~999 출력
                //
                for (int i = 0; i < 1000; i++)
                {
                    Console.WriteLine("{0}: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, i);
                }

                Console.ReadLine();
            }

            { 
                // 2. 병렬 처리
                // 다중쓰레드가 병렬로 출력
                //
                System.Threading.Tasks.Parallel.For(0, 1000, (i) => {
                    Console.WriteLine("{0}: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, i);
                });

				Console.ReadLine();
            }
		}


        static void Parallel_with_Stop()
        {
			var stack = new ConcurrentStack<long>();
			System.Threading.Tasks.Parallel.For(0, 100000, (index, state) =>
			{
                if (index < 50000)
                {
                    stack.Push(index);
                }
                else
                {
                    state.Stop();
                }
			});

			Console.WriteLine("개수: " + stack.Count);

            Console.ReadLine();
		}

		static void Parallel_with_Break()
		{
			var stack = new ConcurrentStack<long>();
			System.Threading.Tasks.Parallel.For(0, 100000, (index, state) =>
			{
				stack.Push(index);
				if (index >= 50000)
				{
					state.Break();

					Console.WriteLine("Index : " + index);
					Console.WriteLine("LowestBreakIteration : " + state.LowestBreakIteration);
				}
			});

			Console.WriteLine("개수: " + stack.Count);

			Console.ReadLine();
		}


        static void Parallel_with_ParallelLoopResult()
        {
			var result = System.Threading.Tasks.Parallel.For(0, 100000, (index, state) =>
			{
				//----- Do Something...
			});

			if (result.IsCompleted)
			{
				//----- 모든 루프가 실행 되었다
			}
			else if (result.LowestBreakIteration.HasValue)
			{
				//----- Break 되었다
			}
			else
			{
				//----- Stop 되었다
			}

			Console.ReadLine();
		}


        static void Parallel_with_ThreadLocalVariables()
        {
			int total = 0;
			System.Threading.Tasks.Parallel.ForEach(
				Enumerable.Range(1, 100)                    // 병렬 처리를 할 소스
			,	() => 0                                     // 스레드 로컬 변수 초기화 값
			,	(value, state, local) => local += value     // 루프 작업
			,	local => Interlocked.Add(ref total, local)  // 루프 작업 완료 후 처리
			);

			Console.WriteLine(total);


		}


		const int MAX = 10000000;
        const int SHIFT = 3;

        static void SequentialEncryt()
        {
            // 테스트 데이타 셋업
            // 1000 만개의 스트링
            string text = "I am a boy. My name is Tom.";
            List<string> textList = new List<string>(MAX);
            for (int i = 0; i < MAX; i++)
            {
                textList.Add(text);
            }

            // 순차 처리 (Test run: 8.7 초)
            var stop_watch = new System.Diagnostics.Stopwatch();
			stop_watch.Start();
            for (int i = 0; i < MAX; i++)
            {
                char[] chArr = textList[i].ToCharArray();

                // 모든 문자를 시저 암호화
                for (int x = 0; x < chArr.Length; x++)
                {
                    // 시저 암호
                    if (chArr[x] >= 'a' && chArr[x] <= 'z')
                    {
                        chArr[x] = (char)('a' + ((chArr[x] - 'a' + SHIFT) % 26));
                    }
                    else if (chArr[x] >= 'A' && chArr[x] <= 'Z')
                    {
                        chArr[x] = (char)('A' + ((chArr[x] - 'A' + SHIFT) % 26));
                    }
                }

                // 변경된 암호로 치환
                textList[i] = new String(chArr);
            };
			stop_watch.Stop();
            Console.WriteLine(stop_watch.Elapsed.ToString());
        }

        static void ParallelEncryt()
        {
            // 테스트 데이타 셋업
            // 1000 만개의 스트링
            string text = "I am a boy. My name is Tom.";
            List<string> textList = new List<string>(MAX);
            for (int i = 0; i < MAX; i++)
            {
                textList.Add(text);
            }

            // 병렬 처리 (Test run: 6.1 초)
            var stop_watch = new System.Diagnostics.Stopwatch();
			stop_watch.Start();
            System.Threading.Tasks.Parallel.For(0, MAX, i =>
            {
                char[] chArr = textList[i].ToCharArray();

                // 모든 문자를 시저 암호화
                for (int x = 0; x < chArr.Length; x++)
                {
                    // 시저 암호
                    if (chArr[x] >= 'a' && chArr[x] <= 'z')
                    {
                        chArr[x] = (char)('a' + ((chArr[x] - 'a' + SHIFT) % 26));
                    }
                    else if (chArr[x] >= 'A' && chArr[x] <= 'Z')
                    {
                        chArr[x] = (char)('A' + ((chArr[x] - 'A' + SHIFT) % 26));
                    }
                }

                // 변경된 암호로 치환
                textList[i] = new String(chArr);
            });
			stop_watch.Stop();
            Console.WriteLine(stop_watch.Elapsed.ToString());
        }

        static void sequence_vs_parallel()
        {
            /*
                대량의 데이타를 여러 쓰레드가 나눠서 처리하는 병렬 처리는 많은 경우
                순차적으로 처리하는 것보다 빠를 가능성이 크다.
                아래 예제는 대량의 데이타를 암호화(여기서는 초보적인 암호방식인 시저암호를 사용)하는데 있어서
                순차적으로 처리한 경우와 Parallel 클래스를 이용해서 병렬처리한 경우를 보여준다.
                4 CPU 머신에서 테스트한 결과, 순차처리는 8.7초, 병렬처리는 6.1초로 차이를 보였다
            */
            {
                SequentialEncryt();

                ParallelEncryt();

                Console.ReadLine();
            }
        }


        static void Update()
        {
            int count = 0;
            while(true)
            {
                count++;
                Console.WriteLine("Called Update() - count:{0}", count);
                System.Threading.Thread.Sleep(3000);
            }
        }

        static void Parallel_Invoke()
        {
            /*
                Parallel.Invoke() 메서드는 여러 작업들을 병렬로 처리하는 기능을 제공한다.
                즉, 다수의 작업 내용을 Action delegate로 받아 들여
                다중 쓰레드들로 동시에 병렬로 Task를 나눠서 실행하게 된다.
                Task 클래스와 다른 점은, 만약 1000개의 Action 델리게이트가 있을 때,
                Task 클래스는 보통 1000개의 쓰레드를 생성하여 실행하지만(물론 사용자 다르게 지정할 수 있지만),
                Parallel.Invoke는 1000개를 일정한 집합으로 나눠
                내부적으로 상대적으로 적은(적절한) 수의 쓰레드들에게 자동으로 할당해서 처리한다.
            */
            {
                // 5개의 다른 Task들을 병렬로 실행
                System.Threading.Tasks.Parallel.Invoke(
                    () => { Update(); },
                    () => { Update(); },
                    () => { Update(); },
                    () => { Update(); },
                    () => { Update(); }
                );

                Console.ReadLine();
            }
        }


        static void Parallel_with_CancellationToken()
        {
			int[] nums = Enumerable.Range(0, 1).ToArray();
			var cts = new System.Threading.CancellationTokenSource();

			// Use ParallelOptions instance to store the CancellationToken
			ParallelOptions po = new ParallelOptions();
			po.CancellationToken = cts.Token;
			cts.Token.Register(
			   () => Console.WriteLine("Cancelling....")
		    );
			po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
			Console.WriteLine("Press any key to start");
			Console.ReadLine();

			// Run a task so that we can cancel from another thread.
			Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Press 'c' + 'Enter' to cancel");
				if (Console.ReadLine() == "c")
                {
                    cts.Cancel();
                }
			});

			try
			{
				System.Threading.Tasks.Parallel.ForEach(nums, po, (num, loop_state) =>
				{
					double d = Math.Sqrt(num);

					Console.WriteLine("{0} on {1}", d, System.Threading.Thread.CurrentThread.ManagedThreadId);
				});
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				cts.Dispose();
			}

            Console.ReadLine();
		}

        public class MyResult
        {
            public string Value { get; set; }

			public MyResult() { Value = string.Empty; }
		}


		static async Task<MyResult> callAsyncFunc2(Int32 num)
		{
			var result = new MyResult();

			Console.WriteLine($"callAsyncFunc2 Index:{num}, ThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			result = await Task.Factory.StartNew( delegate
			{
				Console.WriteLine($"callAsyncFunc2 Task.Factory.StartNew Index:{num}, ThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                return result;
			});

			return result;
		}

		static async Task<MyResult> callAsyncFunc1(Int32 num)
        {
            var result = new MyResult();

			Console.WriteLine($"callAsyncFunc1 Index:{num}, ThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			result = await Task<MyResult>.Run( async delegate
			{
				Console.WriteLine($"callAsyncFunc1 Task.Run Index:{num}, ThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

                result = await callAsyncFunc2(num);
                return result;
			});

            return result;
		}
        
		static void Parallel_with_async()
		{
			int[] nums = Enumerable.Range(0, 1).ToArray();
			var cts = new System.Threading.CancellationTokenSource();

			// Use ParallelOptions instance to store the CancellationToken
			ParallelOptions po = new ParallelOptions();
			po.CancellationToken = cts.Token;
			cts.Token.Register(
			   () => Console.WriteLine("Cancelling....")
			);
			po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
			Console.WriteLine("Press any key to start");
			Console.ReadLine();

			// Run a task so that we can cancel from another thread.
			Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Press 'c' + 'Enter' to cancel");
				if (Console.ReadLine() == "c")
				{
					cts.Cancel();
				}
			});

			try
			{
				System.Threading.Tasks.Parallel.ForEach(nums, po, async (num, loop_state) =>
				{
					await callAsyncFunc1(num);
					Console.WriteLine($"Parallel Inddex:{num}, ThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				});
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				cts.Dispose();
			}

            // 주의 - Parallel.ForEach() 는 awiat 의 완료를 기다리지 않는다. !!!
            //        .NET 5.0 이상 부터는 ForEachAsync() 를 제공하여 await 의 완료를 기다릴 수 있도록 추가 하였다.

            Console.WriteLine($"Terminate Parallel !!! - ThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Console.ReadLine();
		}

		public static void Test()
        {
			//Parallel_with_async();

			//Parallel_with_CancellationToken();

			//Parallel_Invoke();

			//sequence_vs_parallel();

			//Parallel_with_ThreadLocalVariables();

			//Parallel_with_ParallelLoopResult();

			//Parallel_with_Break();

			//Parallel_with_Stop();

			//Parallel_basic();

			//Parallel_what();
		}
	}
}
