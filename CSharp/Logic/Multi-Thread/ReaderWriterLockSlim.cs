using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MultiThread
{
	public class ReaderWriterLockSlim
	{
		static void ReaderWriterLockSlim_what()
		{
            /*
				📚 ReaderWriterLockSlim

				  1. 목적
				    - 다수의 "읽기" 스레드가 동시에 들어오는 건 허용하면서,
					  "쓰기" 스레드는 단 하나만 들어오도록 만들기 위한 락.
				    - 읽기 작업이 훨씬 많은 자료구조(캐시, 설정 조회, 게임 월드 조회 등)에 유리하다.
				    - 일반 lock(=Monitor)처럼 무조건 한 번에 하나만 들어오는 구조보다 동시성이 높다.

				  2. 주요 모드
				    - Read      : 읽기 전용 진입. 여러 스레드가 동시에 Read 가능.
				    - Write     : 쓰기 전용 진입. 오직 1개만 가능, 그리고 Read도 모두 비워져 있어야 들어감.
				    - Upgradeable Read : "일단 읽기 락으로 들어오지만, 나중에 쓰기로 바꿀 수도 있음" 이라는 특수 모드.
									   동시에 여러 개 들어갈 수 없음(1개만 허용). 내부에서 Write로 승격 가능.

				  3. 대표 메서드
				    - EnterReadLock() / ExitReadLock()
				    - EnterWriteLock() / ExitWriteLock()
				    - EnterUpgradeableReadLock() / ExitUpgradeableReadLock()
				    - TryEnterXXX(...) 도 있음 (타임아웃 등)

				  4. 왜 Upgradeable 이 필요한가?
				    - 이미 여러 스레드가 Read로 들어와 있는 상태에서
					  "나도 읽으려 했는데… 읽어보니 이걸 수정해야겠네?" 하는 패턴이 있다.
				    - 그냥 Read로 들어온 상태에서 다시 Write를 걸려고 하면 데드락 위험이 생긴다.
				    - 그래서 "업그레이드 가능(Read + 나중에 Write로 승격)"한 모드를 따로 둔 것.

				  5. 진입 가능 여부 도표
				    - 아래는 "현재 락 안에 누가 들어와 있을 때", "새로 들어오려는 락 모드"가 가능한지 표로 나타낸 것.
				    - 실제 구현은 대기 큐, 공정성 옵션 등 영향이 있지만, 기본적인 허용 관계만 본다.

				    | 현재 상태 ↓  |  새로 진입하려는 모드 → | Read            | Upgradeable | Write
				    |---------------|--------------------------|-----------------|-------------|-------------------------------------------
				    | (아무도 없음) |                          |        O        |      O      |      O
				    |---------------|--------------------------|-----------------|-------------|-------------------------------------------
				    | Read 1개 이상 |                          |        O        |      X      |      X
					|			    |                          | (여러 Read OK)  |             |
				    |---------------|--------------------------|-----------------|-------------|-------------------------------------------
				    | Upgradeable   |                          |        O        |      X      |      O
				    | 1개만 존재    |                          |   (Read는 OK)   |             |
					|			    |                          |                 |             |
					|			    |                          |                 |             | - Upgradeable 1개만 있고,
			        |               |                          |                 |             |   다른 Read가 없을 때만
			        |               |                          |                 |             |   Write가 들어올 수 있다.
					|			    |                          |                 |             | - 즉 업그레이더블이 들어와 있는 상태에서
			        |               |                          |                 |             |   다른 Read가 또 들어오는
			        |               |                          |                 |             |   그 순간부터는 Write 진입이 막힌다.
					|			    |                          |                 |             | 
				    |---------------|--------------------------|-----------------|-------------|-------------------------------------------
				    | Write 1개     |                          |        X        |      X      |      X
				    
			        (1) Read는 Read끼리는 자유롭게 겹친다.
                    (2) Write는 혼자만 들어간다.
                    (3) Upgradeable은 1명만 들어갈 수 있고, 들어와 있는 동안
                      - 다른 Read는 들어올 수 있지만
                      - 또 다른 Upgradeable은 못 들어오고
                      - Write는 "다른 Read가 하나도 없을 때"만 들어올 수 있다
                        (즉, Upgradeable이 실제로 Write로 승격하려면 주변이 깨끗해야 한다)
			*/
        }

        //-----------------------------------------------------------------------------------------------

        static List<int> _numbers = new List<int>();
        static System.Threading.ReaderWriterLockSlim _rw = new System.Threading.ReaderWriterLockSlim();

        static void reader(int id)
        {
            for (int i = 0; i < 3; i++)
            {
                _rw.EnterReadLock();
                try
                {
                    Console.WriteLine($"[R{id}] count={_numbers.Count}");
                }
                finally
                {
                    _rw.ExitReadLock();
                }
                System.Threading.Thread.Sleep(150);
            }
        }

        static void writer()
        {
            for (int i = 0; i < 3; i++)
            {
                System.Threading.Thread.Sleep(200); // 일부러 섞이게
                _rw.EnterWriteLock();
                try
                {
                    int v = _numbers.Count + 10;
                    _numbers.Add(v);
                    Console.WriteLine($"[W] add {v}");
                }
                finally
                {
                    _rw.ExitWriteLock();
                }
            }
        }

        static void upgradeable()
        {
            for (int i = 0; i < 2; i++)
            {
                System.Threading.Thread.Sleep(300);
                _rw.EnterUpgradeableReadLock();
                try
                {
                    Console.WriteLine("[U] in, count=" + _numbers.Count);

                    bool needWrite = _numbers.Count < 10;
                    if (needWrite)
                    {
                        _rw.EnterWriteLock();
                        try
                        {
                            int v = _numbers.Count + 1000;
                            _numbers.Add(v);
                            Console.WriteLine("[U] upgrade -> add " + v);
                        }
                        finally
                        {
                            _rw.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _rw.ExitUpgradeableReadLock();
                }
            }
        }

        static async void ReaderWriterLockSlim_use()
        {
            // 초기 데이터는 Write로 한번에 넣기
            _rw.EnterWriteLock();
            try
            {
                _numbers.Add(1);
                _numbers.Add(2);
            }
            finally
            {
                _rw.ExitWriteLock();
            }

            // 읽기 여러 개
            var r1 = Task.Run(() => reader(1));
            var r2 = Task.Run(() => reader(2));

            // 쓰기 하나
            var w1 = Task.Run(() => writer());

            // 업그레이더블 하나
            var u1 = Task.Run(() => upgradeable());

            await Task.WhenAll(r1, r2, w1, u1);

            Console.WriteLine("끝");

            Console.ReadLine();
        }

		//-----------------------------------------------------------------------------------------------

        private const Int32 read_task_count = 16;
        private const Int32 write_task_count = 16;
        private static ConcurrentQueue<LockDataObject> data_queue = null;

        private static Int32 max_task_count = 0;
        private static List<Int32> call_count_per_task = null;

        private const Int32 thread_delay_ms = 10;
        private bool is_thread_stop = false;

        // 수행에 필요한 데이터 객체
        class LockDataObject
        {
            public string Name { get; set; }
            public double Number { get; set; }
        }

        static void use_read_write_lock_slim_with_task()
		{
			data_queue = new ConcurrentQueue<LockDataObject>();

			call_count_per_task = new List<int>(max_task_count);
			for (var i = 0; i < max_task_count; i++)
			{
				call_count_per_task.Add(0);
			}

			bool is_stop = false;

			var read_tasks = new Task[read_task_count];
			var write_tasks = new Task[write_task_count];

			var rw_lock = new System.Threading.ReaderWriterLockSlim();

			Console.WriteLine($"\nStart ReaderWriterLockSlim with Task Test");

			var started_time = DateTime.Now;

			for (var i = 0; i < write_task_count; i++)
			{
				var task_index = i;

				write_tasks[task_index] = Task.Run(() =>
				{
					while (is_stop == false)
					{
						rw_lock.EnterWriteLock();

						try
						{
							call_count_per_task[task_index]++;

							var obj = new LockDataObject() { Name = task_index.ToString(), Number = task_index };

							data_queue.Enqueue(obj);

							System.Threading.Thread.Sleep(thread_delay_ms);
						}
						finally
						{
							rw_lock.ExitWriteLock();
						}
					}
				});
			}

			for (var i = 0; i < read_task_count; i++)
			{
				var task_index = i;

				read_tasks[task_index] = Task.Run(() =>
				{
					while (is_stop == false)
					{
						rw_lock.EnterReadLock();

						try
						{
							call_count_per_task[task_index]++;

							if (false == data_queue.IsEmpty)
							{
								LockDataObject param = new LockDataObject();
								data_queue.TryDequeue(out param);
							}

							System.Threading.Thread.Sleep(thread_delay_ms);
						}
						finally
						{
							rw_lock.ExitReadLock();
						}
					}
				});
			}

			Console.WriteLine("Press any key to exit");
			Console.ReadLine();

			is_stop = true;
			Task.WaitAll(read_tasks);
			Task.WaitAll(write_tasks);

			double total_seconds = (DateTime.Now - started_time).TotalSeconds;

			Int32 total_access = 0;
			foreach (var count in call_count_per_task)
			{
				total_access += count;
			}
			call_count_per_task.Clear();

			Int32 access_per_sec = (Int32)(total_access / total_seconds);

			Console.WriteLine($"Total Test Time : {total_seconds} sec");
			Console.WriteLine($"Remain Queue Count : {data_queue.Count()}");
			Console.WriteLine($"Total Access : {total_access}");
			Console.WriteLine($"Access/Sec : {access_per_sec}/s");
		}

		static void use_exclusive_lock_with_task()
		{
			data_queue = new ConcurrentQueue<LockDataObject>();

			call_count_per_task = new List<int>(max_task_count);
			for (var i = 0; i < max_task_count; i++)
			{
				call_count_per_task.Add(0);
			}

			bool is_stop = false;

			var read_tasks = new Task[read_task_count];
			var write_tasks = new Task[write_task_count];

			object lock_obj = new object();

			Console.WriteLine($"\nStart Exclusive Lock with Task Test");

			var started_time = DateTime.Now;

			for (var i = 0; i < write_task_count; i++)
			{
				var task_index = i;

				write_tasks[task_index] = Task.Run(() =>
				{
					while (is_stop == false)
					{
						lock(lock_obj)
						{
							call_count_per_task[task_index]++;

							var obj = new LockDataObject() { Name = task_index.ToString(), Number = task_index };

							data_queue.Enqueue(obj);

							System.Threading.Thread.Sleep(thread_delay_ms);
						}
					}
				});
			}

			for (var i = 0; i < read_task_count; i++)
			{
				var task_index = i;

				read_tasks[task_index] = Task.Run(() =>
				{
					while (is_stop == false)
					{
						lock (lock_obj)
						{
							call_count_per_task[task_index]++;

							if(false == data_queue.IsEmpty)
							{
								LockDataObject param = new LockDataObject();
								data_queue.TryDequeue(out param);
							}

							System.Threading.Thread.Sleep(thread_delay_ms);
						}
					}
				});
			}

			Console.WriteLine("Press any key to exit");
			Console.ReadLine();

			is_stop = true;
			Task.WaitAll(read_tasks);
			Task.WaitAll(write_tasks);

			double total_seconds = (DateTime.Now - started_time).TotalSeconds;

			Int32 total_access = 0;
			foreach (var count in call_count_per_task)
			{
				total_access += count;
			}
			call_count_per_task.Clear();

			Int32 access_per_sec = (Int32)(total_access / total_seconds);

			Console.WriteLine($"Total Test Time : {total_seconds} sec");
			Console.WriteLine($"Remain Queue Count : {data_queue.Count()}");
			Console.WriteLine($"Total Access : {total_access}");
			Console.WriteLine($"Access/Sec : {access_per_sec}/s");
		}

		private class IsStop
		{ 
			public bool Ok { get; set; }

			public IsStop(bool isStop)
			{
				Ok = isStop;
			}
		}

		private class ThreadParams
		{
			public Int32 ThreadNo { get; set; }
			public object LockObject { get; set; }
			public IsStop IsStop { get; set; }
		}

		static void doWriteThread(object state)
		{
			var thread_params = (ThreadParams)state;

			while (thread_params.IsStop.Ok == false)
			{
				lock (thread_params.LockObject)
				{
					call_count_per_task[thread_params.ThreadNo]++;

					var obj = new LockDataObject() { Name = thread_params.ThreadNo.ToString(), Number = thread_params.ThreadNo };

					data_queue.Enqueue(obj);

					System.Threading.Thread.Sleep(thread_delay_ms);
				}
			}
		}

		static void doReadThread(object state)
		{
			var thread_params = (ThreadParams)state;

			while (thread_params.IsStop.Ok == false)
			{
				lock (thread_params.LockObject)
				{
					call_count_per_task[thread_params.ThreadNo]++;

					if (false == data_queue.IsEmpty)
					{
						LockDataObject param = new LockDataObject();
						data_queue.TryDequeue(out param);
					}

					System.Threading.Thread.Sleep(thread_delay_ms);
				}
			}
		}

		static void use_exclusive_lock_with_thread()
		{
			data_queue = new ConcurrentQueue<LockDataObject>();

			call_count_per_task = new List<int>(max_task_count);
			for (var i = 0; i < max_task_count; i++)
			{
				call_count_per_task.Add(0);
			}

			var threads = new List<System.Threading.Thread>();
			object lock_obj = new object();

			Console.WriteLine($"\nStart Exclusive Lock with Thread Test");

			var is_stop = new IsStop(false);

			var started_time = DateTime.Now;

			for (var i = 0; i < write_task_count; i++)
			{
				var thread_params = new ThreadParams();
				thread_params.ThreadNo = i;
				thread_params.LockObject = lock_obj;
				thread_params.IsStop = is_stop;

				var new_thread = new System.Threading.Thread(doWriteThread);
				new_thread.Start(thread_params);

				threads.Add(new_thread);
			}

			for (var i = 0; i < read_task_count; i++)
			{
				var thread_params = new ThreadParams();
				thread_params.ThreadNo = i;
				thread_params.LockObject = lock_obj;
				thread_params.IsStop = is_stop;

				var new_thread = new System.Threading.Thread(doReadThread);
				new_thread.Start(thread_params);

				threads.Add(new_thread);
			}

			Console.WriteLine("Press any key to exit");
			Console.ReadLine();

			is_stop.Ok = true;
			threads.waitAll();

			double total_seconds = (DateTime.Now - started_time).TotalSeconds;

			Int32 total_access = 0;
			foreach (var count in call_count_per_task)
			{
				total_access += count;
			}
			call_count_per_task.Clear();

			Int32 access_per_sec = (Int32)(total_access / total_seconds);

			Console.WriteLine($"Total Test Time : {total_seconds} sec");
			Console.WriteLine($"Remain Queue Count : {data_queue.Count()}");
			Console.WriteLine($"Total Access : {total_access}");
			Console.WriteLine($"Access/Sec : {access_per_sec}/s");
		}

		static void ReaderWriterLockSlim_vs_lock()
		{
			max_task_count = Math.Max(read_task_count, write_task_count);
			Console.WriteLine($"Task MaxCount : {max_task_count}");

			use_exclusive_lock_with_thread();

			use_exclusive_lock_with_task();

			use_read_write_lock_slim_with_task();

			Console.ReadLine();
		}

		public static void Test()
		{
			ReaderWriterLockSlim_vs_lock();

			ReaderWriterLockSlim_use();

            ReaderWriterLockSlim_what();
        }
	}

	public static class ThreadExtension
	{
		public static void waitAll(this IEnumerable<System.Threading.Thread> threads)
		{
			foreach (var thread in threads)
			{
				thread.Join();
			}
		}
	}
}
