using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace MultiThread
{
    public class TaskScheduler
    {
        //첫번째 실행
        private static async Task FirstCoroutine()
        {
            Console.WriteLine("Starting - Zone #1");
            Console.WriteLine($"CurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("Yielding - Zone #1");

            //배타 실행 스케줄에서 Task가 실행 됨으로 다음 Task에 실행 수행을 넘긴다.
            await Task.Yield();

            Console.WriteLine("Returned - Zone #1");
            Console.WriteLine($"CurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("Yielding again - Zone #1");

            //배타 실행 스케줄에서 Task가 실행 됨으로 다음 Task에 실행 수행을 넘긴다.
            await Task.Yield();

            Console.WriteLine("Returned - Zone #1");
            Console.WriteLine($"CurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("Finished - Zone #1");
        }

        //두번째 실행
        private static async Task SecondCoroutine()
        {
            Console.WriteLine("\tStarting - Zone #2");
            Console.WriteLine($"\tCurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("\tYielding - Zone #2");

            //배타 실행 스케줄에서 Task가 실행 됨으로 다음 Task에 실행 수행을 넘긴다.
            await Task.Yield();

            Console.WriteLine("\tReturned - Zone #2");
            Console.WriteLine($"\tCurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("\tYielding again - Zone #2");

            //배타 실행 스케줄에서 Task가 실행 됨으로 다음 Task에 실행 수행을 넘긴다.
            await Task.Yield();

            Console.WriteLine("\tReturned - Zone #2");
            Console.WriteLine($"\tCurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("\tYielding again - Zone #2");

            //배타 실행 스케줄에서 Task가 실행 됨으로 다음 Task에 실행 수행을 넘긴다.
            await Task.Yield();

            Console.WriteLine("\tReturned again - Zone #2");
            Console.WriteLine($"\tCurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("\tFinished - Zone #2");
        }

        //세번째 실행
        private static async Task ThirdCoroutine()
        {
            Console.WriteLine("\t\tStarting - Zone #3");
            Console.WriteLine($"\t\tCurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("\t\tYielding - Zone #3");

            //배타 실행 스케줄에서 Task가 실행 됨으로 다음 Task에 실행 수행을 넘긴다.
            await Task.Yield();

            Console.WriteLine("\t\tReturned - Zone #3");
            Console.WriteLine($"\t\tCurrentThreadId:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("\t\tFinished - Zone #3");
        }

        private static async Task runCoroutineAsync(TaskFactory taskFactory, Func<Task> coroutine)
        {
            //배타 실행 스케줄로 세팅된 TaskFactory
            //StartNew로 만들어진 Task를 배타 실행되는 스케줄로 관리가 되도록 컨트롤 한다.
            await await taskFactory.StartNew(coroutine);
        }

        static async Task doCoroutinesAsync(TaskFactory taskFactory)
        {
            var coroutines = new[]
            {
                runCoroutineAsync(taskFactory, FirstCoroutine)
            ,   runCoroutineAsync(taskFactory, SecondCoroutine)
            ,   runCoroutineAsync(taskFactory, ThirdCoroutine)
            };

            //모두 완료가 될 때가지 대기 한다.
            await Task.WhenAll(coroutines);
        }

        static async void Task_with_yield()
        {
            /*
                ======================================================================================================
				Coroutine 함수와 Task.Yield() 함수의 처리 과정을 사용된 스케쥴러와 함께 도식화 !!!
			    ======================================================================================================
             
				[STEP 0] 시스템 준비
				───────────────────────────────────────────────────
				- 생성: new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler
				- 등록: Coroutine 3개 TaskFactory로 실행

				[ExclusiveScheduler Queue]                 [ThreadPool Queue (Default)]             
				┌───────────┐
				│ FirstCoroutine()     │
				├───────────┤
				│ SecondCoroutine()    │                  (비어 있음)
				├───────────┤
				│ ThirdCoroutine()     │
				└───────────┘                  


				[STEP 1] FirstCoroutine 실행
				───────────────────────────────────────────────────
				- 실행 중 "await Task.Yield()" 도달
				- 상태 저장
				- FirstCoroutine.MoveNext #1 생성 → ThreadPool.QueueUserWorkItem 등록

				[ExclusiveScheduler Queue]                [ThreadPool Queue (Default)]             
				┌───────────┐
				│ SecondCoroutine()    │
				├───────────┤
				│ ThirdCoroutine()     │
				└───────────┘                ┌──────────┐
												          │ MoveNext #1        │
												          └──────────┘


				[STEP 2] SecondCoroutine 실행
				───────────────────────────────────────────────────
				- 실행 중 "await Task.Yield()" 도달
				- 상태 저장
				- SecondCoroutine.MoveNext #2 등록됨

				[ExclusiveScheduler Queue]                [ThreadPool Queue (Default)]             
				┌───────────┐
				│ ThirdCoroutine()     │
				└───────────┘                ┌──────────┐
												          │ MoveNext #1        │
												          ├──────────┤
												          │ MoveNext #2        │
												          └──────────┘


				[STEP 3] ThirdCoroutine 실행
				───────────────────────────────────────────────────
				- 실행 중 "await Task.Yield()" 도달
				- ThirdCoroutine.MoveNext #3 등록됨

				[ExclusiveScheduler Queue]                [ThreadPool Queue (Default)]             
				                                          ┌──────────┐
												          │ MoveNext #1        │
												          ├──────────┤
				(비어 있음)								  │ MoveNext #2        │
												          ├──────────┤
												          │ MoveNext #3        │
												          └──────────┘

				[STEP 4] Wakeup 발생 (ThreadPool 작동) <- 스케쥴러의 함수 콜 타임라인에 의해 작동                        
				───────────────────────────────────────────────────
				📌 [ThreadPool Wakeup 트리거]
				   - 이전 Task 완료됨
				   - 워커 스레드가 유휴 상태
				   - .NET의 자동 워커 확장 감지

				→ TaskScheduler.Default 가 큐를 체크함

				📤 MoveNext #1 실행
				📤 MoveNext #2 실행
				📤 MoveNext #3 실행

				[ExclusiveScheduler Queue]                [ThreadPool Queue (Default)]             
				                                          ┌──────────┐
				(비어 있음)								  │ 실행 중 없음       │
												          └──────────┘


				[STEP 5] 각 MoveNext() 내부에서 await 이후 코드 재개됨
				───────────────────────────────────────────────────
				- FirstCoroutine "Returned - Zone #1"
				- SecondCoroutine "Returned - Zone #2"
				- ThirdCoroutine "Returned - Zone #3"
			

                ======================================================================================================
				[STEP 4] 단계에서 ThreadPool 처리에 의한 MoveNext() 호출 과정
			    ======================================================================================================
			    
				[Caller]							[ThreadPool]
				   │                                   │
				   ├── QueueUserWorkItem(action) ─▶│  <-- 요청 들어감
				   │									│
				   │									├── 큐에 Task 등록
				   │									├── 워커 부족 시 확장
				   │									└── 유휴 스레드 감지됨
				   │                                   ↓
				   │                             [Thread Assigned]
				   │                                   ↓
				   │                          action.Invoke() == MoveNext()
			
                ======================================================================================================
				내부적으로 C# 컴파일러가 변환하는 흐름
			    ======================================================================================================

				var awaiter = Task.Yield().GetAwaiter();         // => YieldAwaiter
				if (!awaiter.IsCompleted)
				{
					state = ...;                                 // 상태 저장
					awaiter.OnCompleted(MoveNext);               // ❗ 여기서 호출됨
					return;
				}
				
				// 관련 인터페이스 !!!
				public interface INotifyCompletion
				{
					void OnCompleted(Action continuation);
				}
			*/
            {
                var coroutineFactory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);

                var task = doCoroutinesAsync(coroutineFactory);
                task.Wait();

                Console.ReadLine();
            }
        }
    
        //-----------------------------------------------------------------------------------------------

        public class CustomTaskScheduler : System.Threading.Tasks.TaskScheduler, IDisposable
        {
            private readonly BlockingCollection<Task> _queue;
            private readonly ConcurrentDictionary<Task, byte> _scheduledTasks;
            private readonly System.Threading.Thread[] _workers;
            private readonly CancellationTokenSource _cts;
            private readonly object _sync;

            // static 없이, 이 scheduler 인스턴스가 소유한 worker thread id 집합
            private readonly ConcurrentDictionary<int, byte> _workerThreadIds;

            private bool _started;
            private bool _disposed;

            private int _inlineCount;
            private int _queuedCount;
            private int _executedCount;

            private readonly bool _allowInline;

            public CustomTaskScheduler(int workerCount, string threadNamePrefix = "CustomTaskScheduler", bool allowInline = false)
            {
                if (workerCount <= 0)
                    throw new ArgumentOutOfRangeException("workerCount");

                _queue = new BlockingCollection<Task>(new ConcurrentQueue<Task>());
                _scheduledTasks = new ConcurrentDictionary<Task, byte>();
                _workerThreadIds = new ConcurrentDictionary<int, byte>();
                _cts = new CancellationTokenSource();
                _sync = new object();

                _workers = new System.Threading.Thread[workerCount];

                for (int i = 0; i < workerCount; i++)
                {
                    int workerIndex = i;
                    _workers[i] = new System.Threading.Thread(() => WorkerLoop(_cts.Token))
                    {
                        IsBackground = true,
                        Name = threadNamePrefix + "-" + workerIndex
                    };
                }

                _allowInline = allowInline;
            }

            public int WorkerCount
            {
                get { return _workers.Length; }
            }

            public int PendingCount
            {
                get { return _queue.Count; }
            }

            public int InlineCount
            {
                get { return Volatile.Read(ref _inlineCount); }
            }

            public int QueuedCount
            {
                get { return Volatile.Read(ref _queuedCount); }
            }

            public int ExecutedCount
            {
                get { return Volatile.Read(ref _executedCount); }
            }

            public void Start()
            {
                lock (_sync)
                {
                    ThrowIfDisposed();

                    if (_started)
                        return;

                    _started = true;

                    foreach (var worker in _workers)
                    {
                        worker.Start();
                    }
                }
            }

            public void Stop()
            {
                lock (_sync)
                {
                    if (_disposed)
                        return;

                    if (!_started)
                        return;

                    if (!_queue.IsAddingCompleted)
                        _queue.CompleteAdding();

                    _cts.Cancel();
                }
            }

            public void Join()
            {
                foreach (var worker in _workers)
                {
                    if (worker.IsAlive)
                        worker.Join();
                }
            }

            /*	
                📚 작업 대기 등록

			      1. 의도
			        - 작업을 즉시 실행하지 않고, 스케줄러가 정한 규칙(우선순위, 스레드 개수 등)에 따라 차례를 기다리게 하기 위함이다.

			      2. 기능
			        - Task가 생성되어 실행될 준비가 되었을 때, 이를 스케줄러의 관리 목록(큐)에 추가한다.			      

                  3. 호출 시점
				    - 작업(Task)이 생성되어 실행될 준비가 되었지만, 지금 당장 실행하지 않고 스케줄러에게 처리를 맡길 때이며,

                    3.1. Task.Start() 또는 TaskFactory.StartNew()를 호출할 때,
                      - 가장 명시적인 호출 시점, 새로운 Task를 만들고 실행 버튼을 누르는 순간,
                        시스템은 내부적으로 QueueTask를 호출하여 해당 작업을 스케줄러의 대기열에 집어넣는다. !!!

			        3.2. await 이후의 후속 작업(Continuation)이 실행될 때
			          - await 문장에서 비동기 작업이 끝난 후, 그다음에 이어질 코드들(Continuation)을 누가 실행할지 결정할 때 호출된다.
			            
						async Task MyMethod()
						{
							await Task.Delay(1000); 
							// <--- 이 지점!
							// 1초 뒤 작업이 재개될 때, 나머지 코드를 실행하기 위해 
							// 현재 스케줄러의 QueueTask에 이 지점 이후의 로직이 전달 된다. !!!
							Console.WriteLine("작업 완료");
						}

				    3.3. 부모-자식 Task 관계에서 자식 Task가 생성될 때
                      - 부모 Task 안에서 AttachedToParent 옵션으로 자식 Task를 만들면,
			            자식 Task가 시작될 때 부모가 사용 중인 스케줄러의 QueueTask로 전달된다.

						public static void Run(TaskScheduler customScheduler)
						{
							// 1. 부모 Task를 커스텀 스케줄러를 사용하여 시작
							Task parent = Task.Factory.StartNew(() =>
							{
								Console.WriteLine($"부모 Task 실행 중... (Thread ID: {Environment.CurrentManagedThreadId})");

								// 2. 자식 Task 생성 (AttachedToParent 옵션 사용)
								// 별도의 스케줄러를 지정하지 않으면 부모의 스케줄러(customScheduler)를 상속받음
								Task child = Task.Factory.StartNew(() =>
								{
									Console.WriteLine($"자식 Task 실행 중... (Thread ID: {Environment.CurrentManagedThreadId})");
									// 여기서 이 작업은 customScheduler의 QueueTask를 통해 실행됩니다.
								}, TaskCreationOptions.AttachedToParent);

							}, System.Threading.CancellationToken.None, TaskCreationOptions.None, customScheduler);

							parent.Wait(); // 부모는 자식이 끝날 때까지 자동으로 기다립니다 (AttachedToParent의 특징)
							Console.WriteLine("모든 작업 종료");
						}

				✅ 파라메터
				  - task : 큐에 대기할 Task입니다.			
            */
            protected override void QueueTask(Task task)
            {
                if (task == null)
                    throw new ArgumentNullException("task");

                int currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                string currentThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                Console.WriteLine(
                    "[QueueTask] Enter | ThreadId={0}, ThreadName={1}, TaskId={2}",
                    currentThreadId,
                    currentThreadName,
                    task.Id);

                lock (_sync)
                {
                    Console.WriteLine(
                        "[QueueTask] State Check | Started={0}, IsAddingCompleted={1}, PendingCount={2}, TaskId={3}",
                        _started,
                        _queue.IsAddingCompleted,
                        _queue.Count,
                        task.Id);

                    ThrowIfDisposed();

                    if (!_started)
                    {
                        Console.WriteLine(
                            "[QueueTask] Reject | Reason=Scheduler not started | TaskId={0}",
                            task.Id);

                        throw new InvalidOperationException("CustomTaskScheduler is not started.");
                    }

                    if (_queue.IsAddingCompleted)
                    {
                        Console.WriteLine(
                            "[QueueTask] Reject | Reason=Scheduler is stopping | TaskId={0}",
                            task.Id);

                        throw new InvalidOperationException("CustomTaskScheduler is stopping.");
                    }

                    _scheduledTasks[task] = 0;
                    Console.WriteLine(
                        "[QueueTask] Registered in scheduled task set | TaskId={0}",
                        task.Id);

                    _queue.Add(task);
                    Console.WriteLine(
                        "[QueueTask] Enqueued | TaskId={0}, PendingCount(after enqueue)={1}",
                        task.Id,
                        _queue.Count);

                    int queuedCount = Interlocked.Increment(ref _queuedCount);
                    Console.WriteLine(
                        "[QueueTask] Queue count incremented | QueuedCount={0}, TaskId={1}",
                        queuedCount,
                        task.Id);
                }
            }

            /*	
                📚 현재 스레드 내 즉시 실행(Inlining) 결정

                1. 의도
                  - 새 작업을 큐에 넣지 않고, 현재 이 메서드를 호출한 스레드에서 바로 실행해도 되는지를 판단하기 위함이다.

                2. 기능
                  - 스케줄러는 상황에 따라 Task를 별도 worker queue로 보내지 않고,
                    호출한 현재 스레드에서 즉시 실행(inline execution)할 수 있다.
                  - 이렇게 하면 큐잉 비용과 context switch를 줄일 수 있지만,
                    잘못 허용하면 재진입(reentrancy), 순서 꼬임, 예상치 못한 스레드 문맥 실행 문제가 생길 수 있다.

                3. 호출 시점                  
                  3.1. Wait() 호출 할때

                    - TPL은 현재 스레드가 어떤 Task의 완료를 기다릴 때,
                      그 Task가 아직 실행되지 않았다면 “굳이 큐에 넣고 다른 worker가 처리할 때까지 기다리지 말고,
                      지금 기다리는 스레드에서 바로 실행할 수 있나?” 를 검사할 수 있다.
              
                        static void InlineExample_Wait(TaskScheduler scheduler)
                        {
                            var factory = new TaskFactory(
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                TaskContinuationOptions.None,
                                scheduler);

                            Task task = factory.StartNew(() =>
                            {
                                Console.WriteLine("[Task] Running on ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                            });

                            Console.WriteLine("[Main] Waiting on ThreadId = " + Thread.CurrentThread.ManagedThreadId);

                            // 여기서 TPL은 task가 아직 실행되지 않았다면
                            // "현재 스레드에서 바로 실행할 수 있나?"를 검토하면서
                            // TryExecuteTaskInline(...)를 호출할 수 있다.
                            task.Wait();

                            Console.WriteLine("[Main] Task completed.");
                        }

                  3.2. Result 호출 할때
                    - Result도 내부적으로는 완료를 기다리므로 비슷한 상황이 생긴다.

                        static void InlineExample_Result(TaskScheduler scheduler)
                        {
                            var factory = new TaskFactory(
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                TaskContinuationOptions.None,
                                scheduler);

                            Task<int> task = factory.StartNew(() =>
                            {
                                Console.WriteLine("[Task<int>] Running on ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                                return 123;
                            });

                            Console.WriteLine("[Main] Reading Result on ThreadId = " + Thread.CurrentThread.ManagedThreadId);

                            // Result를 읽는 시점에 아직 task가 실행되지 않았다면
                            // TPL이 현재 스레드에서 바로 실행 가능한지 검토하면서
                            // TryExecuteTaskInline(...)를 호출할 수 있다.
                            int value = task.Result;

                            Console.WriteLine("[Main] Result = " + value);
                        }

                  3.3. Continuation 실행 시점
                    - 어떤 Task가 끝난 뒤 이어지는 continuation이 있을 때,
                      TPL은 continuation을 굳이 다시 큐에 넣지 않고 지금 이 스레드에서 바로 실행 가능한지를 볼 수 있다.

                        static void InlineExample_Continuation(TaskScheduler scheduler)
                        {
                            var factory = new TaskFactory(
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                TaskContinuationOptions.None,
                                scheduler);

                            Task first = factory.StartNew(() =>
                            {
                                Console.WriteLine("[First] ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                            });

                            // continuation 실행이 필요해지면,
                            // ExecuteSynchronously 옵션 때문에 TPL은 먼저
                            // "이 continuation을 현재 스레드에서 즉시 실행할 수 있는가?"를 검토할 수 있다.
                            // 이 과정에서 scheduler.TryExecuteTaskInline(...)가 호출될 수 있다.
                            Task continuation = first.ContinueWith(
                                t =>
                                {
                                    Console.WriteLine("[Continuation] ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                                },
                                CancellationToken.None,
                                TaskContinuationOptions.ExecuteSynchronously,
                                scheduler);
                            
                            // 아직 완료되지 않았다면 기다리는 스레드에서 inline 검토 가능
                            // 3.1 Wait() 호출 시점 예제
                            continuation.Wait();
                        }
            
                  3.4. 부모-자식 Task 구조에서 대기 시점
                    - 부모 Task가 자식 Task를 만들고,
                      그 부모나 외부 코드가 자식 완료를 기다릴 때도 유사한 상황이 생길 수 있다.

                        static void InlineExample_ParentChild(TaskScheduler scheduler)
                        {
                            var factory = new TaskFactory(
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                TaskContinuationOptions.None,
                                scheduler);

                            Task parent = factory.StartNew(() =>
                            {
                                Console.WriteLine("[Parent] ThreadId = " + Thread.CurrentThread.ManagedThreadId);

                                Task child = factory.StartNew(() =>
                                {
                                    Console.WriteLine("[Child] ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                                });

                                // 여기서 child가 아직 큐에만 있고 실행되지 않았다면,
                                // 현재 스레드에서 child를 인라인 실행할 수 있을지 TPL이 검토하면서
                                // TryExecuteTaskInline(...)를 호출할 수 있다.
                                child.Wait();
                            });

                            parent.Wait();
                        }

                  3.5. async/await 재개 시점
                    - async/await도 내부적으로 continuation(재개 코드)을 만든다.
                    - 따라서 await 이후의 코드가 재개될 때 scheduler가 실행 경로에 관여할 수 있다.
                    - 다만 이 경우는 Wait(), Result(), ExecuteSynchronously continuation 예제보다 설명이 더 조심스러워야 한다.
                    - 이유는 await 이후 재개 문맥이 단순히 TaskScheduler 하나만으로 결정되지 않고,
                      SynchronizationContext, TaskScheduler.Current, ConfigureAwait(false), await 대상의 완료 상태 등에 함께 영향을 받기 때문이다.
                    - 따라서 async/await는 "TryExecuteTaskInline(...)가 반드시 여기서 호출된다"는 예제로 보기보다,
                      "await 이후 코드도 continuation이며 scheduler가 관여할 수 있다"는 보조 설명으로 이해하는 것이 더 정확하다.

                        static async Task InlineExample_AsyncAwait(TaskScheduler scheduler)
                        {
                            var factory = new TaskFactory(
                                CancellationToken.None,
                                TaskCreationOptions.None,
                                TaskContinuationOptions.None,
                                scheduler);

                            Console.WriteLine("[Async] Before await. ThreadId = " + Thread.CurrentThread.ManagedThreadId);

                            await factory.StartNew(() =>
                            {
                                Console.WriteLine("[Async Scheduled Work] ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                            });

                            // 이 코드는 await 이후 continuation(재개 코드)이다.
                            // 이 continuation 실행 과정에 scheduler가 관여할 수는 있지만,
                            // Wait()/Result()/ExecuteSynchronously continuation 예제처럼
                            // TryExecuteTaskInline(...) 호출을 단정적으로 설명하긴 어렵다.
                            Console.WriteLine("[Async] After await. ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                        }

                ✅ 파라메터
                  - task : 즉시 실행 가능 여부를 판단할 Task
                  - taskWasPreviouslyQueued : 이미 QueueTask를 통해 큐에 들어갔던 Task인지 여부

                ✅ 반환값 (Returns)
                  - true  : 현재 스레드에서 즉시 실행 허용
                  - false : 즉시 실행 불허, 일반 큐/worker 경로로 처리
            */
            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                if (!_allowInline)
                    return false;

                if (task == null)
                    throw new ArgumentNullException("task");

                int currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                string currentThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                Console.WriteLine(
                    "[TryExecuteTaskInline] Enter | ThreadId={0}, ThreadName={1}, TaskId={2}, PreviouslyQueued={3}",
                    currentThreadId,
                    currentThreadName,
                    task.Id,
                    taskWasPreviouslyQueued);

                // 현재 스레드가 이 scheduler의 worker thread인지 검사
                byte ignoredThread;
                if (!_workerThreadIds.TryGetValue(currentThreadId, out ignoredThread))
                {
                    Console.WriteLine(
                        "[TryExecuteTaskInline] Reject | Reason=Current thread is not this scheduler's worker thread | ThreadId={0}, TaskId={1}",
                        currentThreadId,
                        task.Id);

                    return false;
                }

                Console.WriteLine(
                    "[TryExecuteTaskInline] Worker thread confirmed | ThreadId={0}, TaskId={1}",
                    currentThreadId,
                    task.Id);

                // 이미 queue에 들어가 있었다면, queue 쪽 등록 상태를 먼저 제거해야 함
                if (taskWasPreviouslyQueued)
                {
                    Console.WriteLine(
                        "[TryExecuteTaskInline] Task was previously queued | Trying dequeue | TaskId={0}",
                        task.Id);

                    if (!TryDequeue(task))
                    {
                        Console.WriteLine(
                            "[TryExecuteTaskInline] Reject | Reason=TryDequeue failed | ThreadId={0}, TaskId={1}",
                            currentThreadId,
                            task.Id);

                        return false;
                    }

                    Console.WriteLine(
                        "[TryExecuteTaskInline] TryDequeue succeeded | ThreadId={0}, TaskId={1}",
                        currentThreadId,
                        task.Id);
                }

                bool executed = false;

                try
                {
                    executed = TryExecuteTask(task);

                    Console.WriteLine(
                        "[TryExecuteTaskInline] TryExecuteTask finished | Executed={0} | ThreadId={1}, TaskId={2}",
                        executed,
                        currentThreadId,
                        task.Id);

                    if (executed)
                    {
                        Interlocked.Increment(ref _inlineCount);
                        Interlocked.Increment(ref _executedCount);

                        Console.WriteLine(
                            "[TryExecuteTaskInline] Inline execution success | InlineCount={0}, ExecutedCount={1}, TaskId={2}",
                            _inlineCount,
                            _executedCount,
                            task.Id);
                    }
                    else
                    {
                        Console.WriteLine(
                            "[TryExecuteTaskInline] Inline execution failed | TaskId={0}",
                            task.Id);
                    }

                    return executed;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "[TryExecuteTaskInline] Exception | ThreadId={0}, TaskId={1}, Exception={2}",
                        currentThreadId,
                        task.Id,
                        ex);

                    throw;
                }
            }

            /*	
                📚 스케줄된 Task 목록 조회

                1. 의도
                  - 현재 스케줄러에 등록되어 대기 중인 Task들을 디버거/진단 도구가 볼 수 있게 하기 위함이다.

                2. 기능
                  - 스케줄러 내부 큐에 들어 있으나 아직 실행되지 않은 Task들의 스냅샷을 반환한다.
                  - 이 메서드는 주로 디버깅, 진단, 시각화 목적이다.

                3. 호출 시점
                  - Visual Studio 디버거, 진단 도구, TPL 관련 내부 점검 로직이 현재 대기 중인 작업을 확인하려 할 때 호출될 수 있다.

                ✅ 파라메터
                  - 없음

                ✅ 반환값 (Returns)
                  - 현재 스케줄러 큐에 대기 중인 Task들의 열거 가능 컬렉션
            */
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return _scheduledTasks.Keys.ToArray();
            }

            /*	
                📚 최대 동시 실행 수준

                1. 의도
                  - 이 스케줄러가 동시에 몇 개의 작업을 병렬 실행할 수 있는지 외부에 알려주기 위함이다.

                2. 기능
                  - TPL이나 외부 호출자가 현재 스케줄러의 병렬 실행 한도를 참고할 수 있게 한다.
                  - 일반적으로 내부 worker thread 개수와 같은 값을 반환한다.

                3. 호출 시점
                  - TaskScheduler의 특성을 조회할 때
                  - 디버거나 진단 도구가 스케줄러 능력을 확인할 때
                  - 일부 내부 TPL 로직에서 참고할 수 있다

                ✅ 파라메터
                  - 없음

                ✅ 반환값 (Returns)
                  - 동시에 실행 가능한 최대 Task 개수
            */
            public override int MaximumConcurrencyLevel
            {
                get { return _workers.Length; }
            }

            protected override bool TryDequeue(Task task)
            {
                if (task == null)
                    return false;

                byte ignoredTask;
                return _scheduledTasks.TryRemove(task, out ignoredTask);
            }

            private void WorkerLoop(CancellationToken token)
            {
                int currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                string currentThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                Console.WriteLine(
                    "[WorkerLoop] Start | ThreadId={0}, ThreadName={1}",
                    currentThreadId,
                    currentThreadName);

                _workerThreadIds[currentThreadId] = 0;

                Console.WriteLine(
                    "[WorkerLoop] Registered worker thread | ThreadId={0}, WorkerThreadCount={1}",
                    currentThreadId,
                    _workerThreadIds.Count);

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        Task task;

                        Console.WriteLine(
                            "[WorkerLoop] Waiting for task | ThreadId={0}, PendingCount(before take)={1}",
                            currentThreadId,
                            _queue.Count);

                        try
                        {
                            task = _queue.Take(token);

                            Console.WriteLine(
                                "[WorkerLoop] Dequeued task | ThreadId={0}, TaskId={1}, PendingCount(after take)={2}",
                                currentThreadId,
                                task.Id,
                                _queue.Count);
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine(
                                "[WorkerLoop] Stop | Reason=Cancellation requested | ThreadId={0}",
                                currentThreadId);

                            break;
                        }
                        catch (InvalidOperationException)
                        {
                            Console.WriteLine(
                                "[WorkerLoop] Stop | Reason=Queue completed and empty | ThreadId={0}",
                                currentThreadId);

                            // CompleteAdding 이후 queue가 비면 종료
                            break;
                        }

                        byte ignoredTask;
                        bool removed = _scheduledTasks.TryRemove(task, out ignoredTask);

                        Console.WriteLine(
                            "[WorkerLoop] Remove from scheduled set | ThreadId={0}, TaskId={1}, Removed={2}",
                            currentThreadId,
                            task.Id,
                            removed);

                        try
                        {
                            // Task를 직접 실행하지 말고, 반드시 TryExecuteTask(task)를 사용해야 한다.
                            // 그래야 Task 상태 전이, 예외 처리, continuation 연결 등 TPL 내부 규약이 유지된다.
                            bool executed = TryExecuteTask(task);

                            Console.WriteLine(
                                "[WorkerLoop] TryExecuteTask finished | ThreadId={0}, TaskId={1}, Executed={2}",
                                currentThreadId,
                                task.Id,
                                executed);

                            if (executed)
                            {
                                int executedCount = Interlocked.Increment(ref _executedCount);

                                Console.WriteLine(
                                    "[WorkerLoop] ExecutedCount incremented | ThreadId={0}, TaskId={1}, ExecutedCount={2}",
                                    currentThreadId,
                                    task.Id,
                                    executedCount);
                            }
                            else
                            {
                                Console.WriteLine(
                                    "[WorkerLoop] Execution skipped/failed | ThreadId={0}, TaskId={1}",
                                    currentThreadId,
                                    task.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                "[WorkerLoop] Task execution exception | ThreadId={0}, TaskId={1}, Exception={2}",
                                currentThreadId,
                                task.Id,
                                ex);
                        }
                    }
                }
                finally
                {
                    byte ignoredThread;
                    bool removedThread = _workerThreadIds.TryRemove(currentThreadId, out ignoredThread);

                    Console.WriteLine(
                        "[WorkerLoop] Worker thread removed | ThreadId={0}, Removed={1}, WorkerThreadCount(after remove)={2}",
                        currentThreadId,
                        removedThread,
                        _workerThreadIds.Count);

                    Console.WriteLine(
                        "[WorkerLoop] Exit | ThreadId={0}, ThreadName={1}",
                        currentThreadId,
                        currentThreadName);
                }
            }

            private void ThrowIfDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException("CustomTaskScheduler");
            }

            public void Dispose()
            {
                bool shouldJoin = false;

                lock (_sync)
                {
                    if (_disposed)
                        return;

                    _disposed = true;

                    if (_started)
                    {
                        if (!_queue.IsAddingCompleted)
                            _queue.CompleteAdding();

                        _cts.Cancel();
                        shouldJoin = true;
                    }
                }

                if (shouldJoin)
                {
                    Join();
                }

                _queue.Dispose();
                _cts.Dispose();
            }
        }

        public static class CustomTask
        {
            private static System.Threading.Tasks.TaskScheduler _scheduler;
            private static readonly object _sync = new object();

            public static void Initialize(System.Threading.Tasks.TaskScheduler scheduler)
            {
                if (scheduler == null)
                    throw new ArgumentNullException("scheduler");

                lock (_sync)
                {
                    _scheduler = scheduler;

                    Console.WriteLine(
                        "[CustomTask] " + string.Format(
                            "Initialize | SchedulerType={0}, MaximumConcurrencyLevel={1}",
                            scheduler.GetType().FullName,
                            scheduler.MaximumConcurrencyLevel));
                }
            }

            public static Task StartNew(Action action)
            {
                if (action == null)
                    throw new ArgumentNullException("action");

                System.Threading.Tasks.TaskScheduler scheduler;
                lock (_sync)
                {
                    scheduler = _scheduler;
                }

                if (scheduler == null)
                    throw new InvalidOperationException("CustomTask is not initialized.");

                int currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                string currentThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                Console.WriteLine(
                    "[CustomTask] " + string.Format(
                        "StartNew<Action> Request | CallerThreadId={0}, CallerThreadName={1}, SchedulerType={2}",
                        currentThreadId,
                        currentThreadName,
                        scheduler.GetType().Name));

                return Task.Factory.StartNew(
                    () =>
                    {
                        int workerThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                        string workerThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                        Console.WriteLine(
                            "[CustomTask] " + string.Format(
                                "StartNew<Action> Execute Begin | WorkerThreadId={0}, WorkerThreadName={1}",
                                workerThreadId,
                                workerThreadName));

                        try
                        {
                            action();

                            Console.WriteLine(
                                "[CustomTask] " + string.Format(
                                    "StartNew<Action> Execute Success | WorkerThreadId={0}, WorkerThreadName={1}",
                                    workerThreadId,
                                    workerThreadName));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                "[CustomTask] " + string.Format(
                                    "StartNew<Action> Execute Exception | WorkerThreadId={0}, WorkerThreadName={1}, Exception={2}",
                                    workerThreadId,
                                    workerThreadName,
                                    ex));
                            throw;
                        }
                    },
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    scheduler);
            }

            public static Task<TResult> StartNew<TResult>(Func<TResult> func)
            {
                if (func == null)
                    throw new ArgumentNullException("func");

                System.Threading.Tasks.TaskScheduler scheduler;
                lock (_sync)
                {
                    scheduler = _scheduler;
                }

                if (scheduler == null)
                    throw new InvalidOperationException("CustomTask is not initialized.");

                int currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                string currentThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                Console.WriteLine(
                    "[CustomTask] " + string.Format(
                        "StartNew<TResult> Request | CallerThreadId={0}, CallerThreadName={1}, SchedulerType={2}",
                        currentThreadId,
                        currentThreadName,
                        scheduler.GetType().Name));

                return Task.Factory.StartNew(
                    () =>
                    {
                        int workerThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                        string workerThreadName = System.Threading.Thread.CurrentThread.Name ?? "(null)";

                        Console.WriteLine(
                            "[CustomTask] " + string.Format(
                                "StartNew<TResult> Execute Begin | WorkerThreadId={0}, WorkerThreadName={1}",
                                workerThreadId,
                                workerThreadName));

                        try
                        {
                            TResult result = func();

                                Console.WriteLine(
                                    "[CustomTask] " + string.Format(
                                        "StartNew<TResult> Execute Success | WorkerThreadId={0}, WorkerThreadName={1}, ResultType={2}",
                                        workerThreadId,
                                        workerThreadName,
                                        typeof(TResult).FullName));

                            return result;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                "[CustomTask] " + string.Format(
                                    "StartNew<TResult> Execute Exception | WorkerThreadId={0}, WorkerThreadName={1}, Exception={2}",
                                    workerThreadId,
                                    workerThreadName,
                                    ex));
                            throw;
                        }
                    },
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    scheduler);
            }

            public static bool IsInitialized
            {
                get
                {
                    lock (_sync)
                    {
                        return _scheduler != null;
                    }
                }
            }

            public static System.Threading.Tasks.TaskScheduler CurrentScheduler
            {
                get
                {
                    lock (_sync)
                    {
                        return _scheduler;
                    }
                }
            }
        }

        private static void BenchmarkScheduler(string title, CustomTaskScheduler scheduler)
        {
            using (scheduler)
            {
                scheduler.Start();

                var factory = new TaskFactory(
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskContinuationOptions.None,
                    scheduler);

                Stopwatch sw = Stopwatch.StartNew();

                var tasks = new List<Task>();

                for (int i = 0; i < 500; i++)
                {
                    int index = i;

                    Task first = factory.StartNew(() =>
                    {
                        double x = 0;
                        for (int k = 0; k < 1000; k++)
                        {
                            x += Math.Sqrt(k + index);
                        }
                    });

                    Task continuation = first.ContinueWith(
                        t =>
                        {
                            double y = 0;
                            for (int k = 0; k < 1000; k++)
                            {
                                y += Math.Sqrt(k + index);
                            }
                        },
                        CancellationToken.None,
                        TaskContinuationOptions.ExecuteSynchronously,
                        scheduler);

                    tasks.Add(continuation);
                }

                Task.WaitAll(tasks.ToArray());

                sw.Stop();

                Console.WriteLine();
                Console.WriteLine("=== " + title + " ===");
                Console.WriteLine("Elapsed(ms)   : " + sw.ElapsedMilliseconds);
                Console.WriteLine("WorkerCount   : " + scheduler.WorkerCount);
                Console.WriteLine("QueuedCount   : " + scheduler.QueuedCount);
                Console.WriteLine("ExecutedCount : " + scheduler.ExecutedCount);
                Console.WriteLine("InlineCount   : " + scheduler.InlineCount);
                Console.WriteLine("PendingCount  : " + scheduler.PendingCount);

                Console.ReadLine();
            }
        }

        static void Task_with_Thread_and_CustomTaskScheduler()
	    {
            //-------------------------------------------------------------------------------------
            // 1. 고정 worker 개수로 Task 실행 테스트
            //-------------------------------------------------------------------------------------
            {
                // CustomTaskScheduler(4) 생성
                using (var scheduler = new CustomTaskScheduler(4))
                {
                    scheduler.Start();

                    var factory = new TaskFactory(
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        scheduler);

                    var tasks = new List<Task>();

                    // 20개 Task 등록
                    for (int i = 0; i < 20; i++)
                    {
                        int taskIndex = i;

                        // 실제로 worker 4개 안에서만 돌아가는지 확인
                        Task task = factory.StartNew(() =>
                        {
                            Console.WriteLine(
                                "[Task {0}] Start - ThreadId={1}, Name={2}",
                                taskIndex,
                                System.Threading.Thread.CurrentThread.ManagedThreadId,
                                System.Threading.Thread.CurrentThread.Name);

                            System.Threading.Thread.Sleep(200);

                            Console.WriteLine(
                                "[Task {0}] End   - ThreadId={1}, Name={2}",
                                taskIndex,
                                System.Threading.Thread.CurrentThread.ManagedThreadId,
                                System.Threading.Thread.CurrentThread.Name);
                        });

                        tasks.Add(task);
                    }

                    Task.WaitAll(tasks.ToArray());

                    Console.WriteLine();
                    Console.WriteLine("=== CustomTaskSchedulerTest ===");
                    Console.WriteLine("WorkerCount   : " + scheduler.WorkerCount);
                    Console.WriteLine("QueuedCount   : " + scheduler.QueuedCount);
                    Console.WriteLine("ExecutedCount : " + scheduler.ExecutedCount);
                    Console.WriteLine("InlineCount   : " + scheduler.InlineCount);
                    Console.WriteLine("PendingCount  : " + scheduler.PendingCount);

                    Console.ReadLine();
                }
            }

            //-------------------------------------------------------------------------------------
            // 2. inline 최적화 테스트
            //-------------------------------------------------------------------------------------
            {
                using (var scheduler = new CustomTaskScheduler(4))
                {
                    scheduler.Start();

                    var factory = new TaskFactory(
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        scheduler);

                    var tasks = new List<Task>();

                    for (int i = 0; i < 50; i++)
                    {
                        int index = i;

                        // 선행 Task 실행
                        Task first = factory.StartNew(() =>
                        {
                            Console.WriteLine(
                                "[First {0}] ThreadId={1}, Name={2}",
                                index,
                                System.Threading.Thread.CurrentThread.ManagedThreadId,
                                System.Threading.Thread.CurrentThread.Name);
                        });

                        // continuation을 ExecuteSynchronously로 등록
                        Task continuation = first.ContinueWith(
                            t =>
                            {
                                Console.WriteLine(
                                    "[Continuation {0}] ThreadId={1}, Name={2}",
                                    index,
                                    System.Threading.Thread.CurrentThread.ManagedThreadId,
                                    System.Threading.Thread.CurrentThread.Name);
                            },
                            CancellationToken.None,
                            TaskContinuationOptions.ExecuteSynchronously,
                            scheduler);

                        // scheduler가 inline 허용시 이어서 실행 가능한지 확인

                        tasks.Add(continuation);
                    }

                    Task.WaitAll(tasks.ToArray());

                    Console.WriteLine();
                    Console.WriteLine("=== Inline Continuation Test ===");
                    Console.WriteLine("WorkerCount   : " + scheduler.WorkerCount);
                    Console.WriteLine("QueuedCount   : " + scheduler.QueuedCount);
                    Console.WriteLine("ExecutedCount : " + scheduler.ExecutedCount);
                    Console.WriteLine("InlineCount   : " + scheduler.InlineCount);
                    Console.WriteLine("PendingCount  : " + scheduler.PendingCount);

                    Console.ReadLine();
                }
            }

            //-------------------------------------------------------------------------------------
            // 3. Wait() 시점 inline 검토 테스트
            //-------------------------------------------------------------------------------------
            {
                using (var scheduler = new CustomTaskScheduler(2))
                {
                    scheduler.Start();

                    var factory = new TaskFactory(
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        scheduler);

                    Task task = factory.StartNew(() =>
                    {
                        Console.WriteLine(
                            "[Task] Running on ThreadId={0}, Name={1}",
                            System.Threading.Thread.CurrentThread.ManagedThreadId,
                            System.Threading.Thread.CurrentThread.Name);

                        System.Threading.Thread.Sleep(300);
                    });

                    Console.WriteLine(
                        "[Main] Before Wait. ThreadId={0}",
                        System.Threading.Thread.CurrentThread.ManagedThreadId);

                    task.Wait();

                    Console.WriteLine(
                        "[Main] After Wait. ThreadId={0}",
                        System.Threading.Thread.CurrentThread.ManagedThreadId);

                    Console.WriteLine();
                    Console.WriteLine("=== Wait Inline Test ===");
                    Console.WriteLine("WorkerCount   : " + scheduler.WorkerCount);
                    Console.WriteLine("QueuedCount   : " + scheduler.QueuedCount);
                    Console.WriteLine("ExecutedCount : " + scheduler.ExecutedCount);
                    Console.WriteLine("InlineCount   : " + scheduler.InlineCount);
                    Console.WriteLine("PendingCount  : " + scheduler.PendingCount);

                    Console.ReadLine();
                }
            }

            //-------------------------------------------------------------------------------------
            // 4. Task.Result 테스트
            //-------------------------------------------------------------------------------------
            {
                using (var scheduler = new CustomTaskScheduler(2))
                {
                    scheduler.Start();

                    var factory = new TaskFactory(
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        scheduler);

                    Task<int> task = factory.StartNew(() =>
                    {
                        Console.WriteLine(
                            "[Task<int>] Running on ThreadId={0}, Name={1}",
                            System.Threading.Thread.CurrentThread.ManagedThreadId,
                            System.Threading.Thread.CurrentThread.Name);

                        System.Threading.Thread.Sleep(200);
                        return 12345;
                    });

                    Console.WriteLine("[Main] Before Result");

                    int value = task.Result;

                    Console.WriteLine("[Main] Result = " + value);

                    Console.WriteLine();
                    Console.WriteLine("=== Result Inline Test ===");
                    Console.WriteLine("WorkerCount   : " + scheduler.WorkerCount);
                    Console.WriteLine("QueuedCount   : " + scheduler.QueuedCount);
                    Console.WriteLine("ExecutedCount : " + scheduler.ExecutedCount);
                    Console.WriteLine("InlineCount   : " + scheduler.InlineCount);
                    Console.WriteLine("PendingCount  : " + scheduler.PendingCount);

                    Console.ReadLine();
                }
            }

            //-------------------------------------------------------------------------------------
            // 5. 성능 비교 테스트
            //-------------------------------------------------------------------------------------
            {
                BenchmarkScheduler(
                    "InlineEnabled",
                    new CustomTaskScheduler(4, "InlineEnabled", true));

                BenchmarkScheduler(
                    "InlineDisabled",
                    new CustomTaskScheduler(4, "InlineDisabled", false));

                Console.WriteLine();
                Console.WriteLine("Benchmark compare completed.");
                Console.ReadLine();

                Console.ReadLine();
            }

            //-------------------------------------------------------------------------------------
            // 6. 여러개의 CustomTaskScheduler 동시 사용 테스트
            //-------------------------------------------------------------------------------------
            {
                using (var schedulerA = new CustomTaskScheduler(2, "SchedulerA"))
                using (var schedulerB = new CustomTaskScheduler(2, "SchedulerB"))
                {
                    schedulerA.Start();
                    schedulerB.Start();

                    var factoryA = new TaskFactory(
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        schedulerA);

                    var factoryB = new TaskFactory(
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        schedulerB);

                    var tasks = new List<Task>();

                    for (int i = 0; i < 10; i++)
                    {
                        int index = i;

                        tasks.Add(factoryA.StartNew(() =>
                        {
                            Console.WriteLine(
                                "[A-{0}] ThreadId={1}, Name={2}",
                                index,
                                System.Threading.Thread.CurrentThread.ManagedThreadId,
                                System.Threading.Thread.CurrentThread.Name);
                            System.Threading.Thread.Sleep(100);
                        }));

                        tasks.Add(factoryB.StartNew(() =>
                        {
                            Console.WriteLine(
                                "[B-{0}] ThreadId={1}, Name={2}",
                                index,
                                System.Threading.Thread.CurrentThread.ManagedThreadId,
                                System.Threading.Thread.CurrentThread.Name);
                            System.Threading.Thread.Sleep(100);
                        }));
                    }

                    Task.WaitAll(tasks.ToArray());

                    Console.WriteLine();
                    Console.WriteLine("=== Multiple Scheduler Test ===");
                    Console.WriteLine("[SchedulerA] ExecutedCount = " + schedulerA.ExecutedCount + ", InlineCount = " + schedulerA.InlineCount);
                    Console.WriteLine("[SchedulerB] ExecutedCount = " + schedulerB.ExecutedCount + ", InlineCount = " + schedulerB.InlineCount);

                    Console.ReadLine();
                }
            }

            //-------------------------------------------------------------------------------------
            // 7. CustumTask 테스트
            //-------------------------------------------------------------------------------------
            {
                using (var scheduler = new CustomTaskScheduler(4))
                {
                    scheduler.Start();
                    CustomTask.Initialize(scheduler);

                    Task[] tasks = new Task[10];

                    for (int i = 0; i < tasks.Length; i++)
                    {
                        int index = i;

                        tasks[i] = CustomTask.StartNew(() =>
                        {
                            Console.WriteLine("Task " + index + " started on " + System.Threading.Thread.CurrentThread.Name);
                            System.Threading.Thread.Sleep(300);
                            Console.WriteLine("Task " + index + " finished on " + System.Threading.Thread.CurrentThread.Name);
                        });
                    }

                    Task.WaitAll(tasks);

                    Console.WriteLine("PendingCount = " + scheduler.PendingCount);
                    Console.WriteLine("MaximumConcurrencyLevel = " + scheduler.MaximumConcurrencyLevel);
                    Console.WriteLine("Done.");

                    Console.ReadLine();
                }
            }
        }

        //-----------------------------------------------------------------------------------------------

        internal sealed class SingleThreadTaskScheduler : System.Threading.Tasks.TaskScheduler
	    {
		    static System.Threading.Thread _thread;
		    static BlockingCollection<WorkItem> _workItems;

		    class WorkItem
		    {
			    Task _task;
			    public Task Task => _task;
			    Func<Task, bool> _func;
			    public Func<Task, bool> Func => _func;

			    internal WorkItem(Func<Task, bool> func, Task task)
			    {
				    _task = task;
				    _func = func;
			    }
		    }

		    static SingleThreadTaskScheduler()
		    {
			    _workItems = new BlockingCollection<WorkItem>();

			    _thread = new System.Threading.Thread(doThreadFunc);
			    _thread.IsBackground = true;
			    _thread.Start();
		    }

		    static void doThreadFunc()
		    {
			    while (true)
			    {
				    var item = _workItems.Take();
				    item.Func(item.Task); // Task.Factory.StartNew() 에 등록 함수가 호출 된다 !!!
			    }
		    }

		    protected override void QueueTask(Task task)
		    {
			    _workItems.Add(new WorkItem(TryExecuteTask, task));
		    }

		    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		    {
                // 인라인 실행 로직 (보통 동기화 이슈가 없다면 TryExecuteTask 호출)
                return false;
		    }

		    protected override IEnumerable<Task> GetScheduledTasks() => null;
		    public override int MaximumConcurrencyLevel => 1;
	    }

		static void Task_with_Thread_and_SingleThreadTaskScheduler()
		{
			var single_thread_task_scheduler = new SingleThreadTaskScheduler();

			Console.WriteLine($"Main Thread !!! - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			Task.Factory.StartNew(() =>
			{
				Console.WriteLine($"SingleThreadTaskScheduler StartNew !!! - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			}, CancellationToken.None, TaskCreationOptions.None, single_thread_task_scheduler);

			Console.ReadLine();
		}

		//-----------------------------------------------------------------------------------------------

        public class LimitedConcurrencyLevelTaskScheduler : System.Threading.Tasks.TaskScheduler
        {
            private readonly LinkedList<Task> _tasks = new LinkedList<Task>();
            private readonly int _maxDegreeOfParallelism;
            private int _delegatesQueuedOrRunning = 0;

            public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
            {
                _maxDegreeOfParallelism = maxDegreeOfParallelism;
            }

            protected override void QueueTask(Task task)
            {
                lock (_tasks)
                {
                    _tasks.AddLast(task);
                    if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                    {
                        _delegatesQueuedOrRunning++;
                        NotifyThreadPoolOfPendingWork();
                    }
                }
            }

            private void NotifyThreadPoolOfPendingWork()
            {
                System.Threading.ThreadPool.UnsafeQueueUserWorkItem(_ =>
                {
                    Task item;
                    lock (_tasks)
                    {
                        if (_tasks.Count == 0)
                        {
                            _delegatesQueuedOrRunning--;
                            return;
                        }
                        item = _tasks.First.Value;
                        _tasks.RemoveFirst();
                    }

                    // 실제 Task 실행
                    base.TryExecuteTask(item);

                    // 다음 작업 처리 시도
                    NotifyThreadPoolOfPendingWork();
                }, null);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                // 인라인 실행 로직 (보통 동기화 이슈가 없다면 TryExecuteTask 호출)
                return false;
            }

            protected override IEnumerable<Task> GetScheduledTasks() => _tasks;

            public override int MaximumConcurrencyLevel => _maxDegreeOfParallelism;
        }

        static void Task_with_LimitedConcurrencyLevelTaskScheduler()
		{
            var scheduler = new LimitedConcurrencyLevelTaskScheduler(2); // 동시에 2개만 실행
            var factory = new TaskFactory(scheduler);

            factory.StartNew(() =>
            {
                Console.WriteLine("LimitedConcurrencyLevelTaskScheduler 실행중 !!!");
            });

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------------

        public static void Test()
		{
			//Task_with_LimitedConcurrencyLevelTaskScheduler();

            //Task_with_Thread_and_SingleThreadTaskScheduler();

            Task_with_Thread_and_CustomTaskScheduler();

            //Task_with_yield();
        }
	}
}
