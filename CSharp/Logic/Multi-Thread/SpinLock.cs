using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;




namespace MultiThread
{
	public class SpinLock
	{
		// 잠김 횟수
		const int try_lock_count = 1000000;
		static Int32 thread_count = 16;
		static ConcurrentQueue<LockDataObject> data_queue = new ConcurrentQueue<LockDataObject>();

		// 잠김에 사용할 객체
		static object _lock = new Object();
		// 잠김에 사용할 SpinLock 객체
		static System.Threading.SpinLock _spinlock = new System.Threading.SpinLock();

		// 수행에 필요한 데이터 객체
		class LockDataObject
		{
			public string Name { get; set; }
			public double Number { get; set; }
		}

		private static void updateWithSpinLock(LockDataObject d, Int32 i)
		{
			// false로 세팅하고 spinLock 객채에 Enter 해야 한다.
			bool lockTaken = false;
			try
			{
				_spinlock.Enter(ref lockTaken);

				data_queue.Enqueue(d);
			}
			catch (LockRecursionException e)
			{
				Console.WriteLine($"Exception !!! : errMsg:{e.Message} - ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			}
			finally
			{
				// 잠김 풀기
				if (lockTaken)
				{
					_spinlock.Exit(false);
				}
			}
		}

		private static void useSpinLock()
		{
			Stopwatch sw = Stopwatch.StartNew();

			// 병렬 실행
			System.Threading.Tasks.Parallel.For(0, thread_count, (i) => 
				{
					for (int c = 0; c < try_lock_count; c++)
					{
						updateWithSpinLock(new LockDataObject() { Name = c.ToString(), Number = c }, c);
					}
				}
			);

			sw.Stop();
			Console.WriteLine($"elapsed ms with SpinLock: {sw.ElapsedMilliseconds}");
		}

		static void updateWithLock(LockDataObject d, Int32 i)
		{
			lock (_lock)
			{
				data_queue.Enqueue(d);
			}
		}

		private static void useLock()
		{
			Stopwatch sw = Stopwatch.StartNew();

			// 병렬 실행
			System.Threading.Tasks.Parallel.For(0, thread_count, (i) =>
				{
					for (int c = 0; c < try_lock_count; c++)
					{
						updateWithLock(new LockDataObject() { Name = c.ToString(), Number = c }, c);
					}
				}
			);

			sw.Stop();
			Console.WriteLine($"elapsed ms with ExclusiveLock: {sw.ElapsedMilliseconds}");
		}

		static void SpinLock_vs_lock()
		{
			Console.WriteLine($"Try Lock Max Count : {try_lock_count}");
			Console.WriteLine($"Thread Count : {thread_count}");

			useLock();
			useSpinLock();

			Console.ReadLine();
		}

		public static void Test()
		{
			//SpinLock_vs_lock();
		}
	}
}
