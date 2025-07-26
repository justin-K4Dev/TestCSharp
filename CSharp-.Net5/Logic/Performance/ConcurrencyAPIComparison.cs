using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Performance;

public class ConcurrencyAPIComparison
{
	static async Task startEventTimer(Int32 intervalMS)
	{
		_ = await Task.Factory.StartNew(async () =>
		{
			Console.WriteLine($"Start Thread !!! : TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}"
							 + $" - CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");
			while (true)
			{
				await Task.Delay(intervalMS);

				Console.WriteLine($"Wake Up Thread !!! : TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}"
								 + $" - CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");

				while (true)
				{
					var curr_tick = Environment.TickCount;
					if ((curr_tick % 10) == 0)
					{
						break;
					}
				}
			}
		}, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
	}

	static async Task onStartEventTimer(Int32 delayMS, Int32 intervalMS)
	{
		await Task.Delay(delayMS);

		await startEventTimer(intervalMS);
	}

	static void Task_StartNew_with_ThreadPool()
	{
		Console.WriteLine($"Start EventTimer - CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");

		System.Threading.Tasks.Parallel.For(0, 2000, async (i) =>
		{
			await onStartEventTimer(1, 5000);

			Console.WriteLine($"StartCount:{i}, CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");
		});

		Console.WriteLine($"Start MainLoop - CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");

		while (true)
		{
			var stop_watch = new Stopwatch();
			stop_watch.Start();

			var task = Task.Delay(100);
			task.Wait();

			stop_watch.Stop();

			if (130 <= stop_watch.ElapsedMilliseconds)
			{
				Console.WriteLine($"################################### MainLoop Delay !!! : 130(ms) <= elapsedMS:{stop_watch.ElapsedMilliseconds}");
			}
		}

		Console.ReadLine();
	}

	private const Int32 read_task_count = 24;
	private const Int32 write_task_count = 24;
	private static ConcurrentQueue<LockDataObject> data_queue = null;

	private static Int32 max_task_count = 0;
	private static List<Int32> call_count_per_task = null;

	private const Int32 thread_delay_ms = 10;
	private bool is_thread_stop = false;

	// 잠김에 사용할 SpinLock 객체
	static System.Threading.SpinLock spin_lock = new System.Threading.SpinLock();

	// 수행에 필요한 데이터 객체
	class LockDataObject
	{
		public string Name { get; set; }
		public double Number { get; set; }
	}

	static void use_spin_lock_with_task()
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

		Console.WriteLine($"\nStart SpinLock with Task Test");

		var started_time = DateTime.Now;

		for (var i = 0; i < write_task_count; i++)
		{
			var task_index = i;

			write_tasks[task_index] = Task.Run(() =>
			{
				while (is_stop == false)
				{
					bool lockTaken = false;

					try
					{
						spin_lock.Enter(ref lockTaken);

						call_count_per_task[task_index]++;

						var obj = new LockDataObject() { Name = task_index.ToString(), Number = task_index };

						data_queue.Enqueue(obj);

						System.Threading.Thread.Sleep(thread_delay_ms);
					}
					finally
					{
						// 잠김 풀기
						if (lockTaken)
						{
							spin_lock.Exit(false);
						}
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
					bool lockTaken = false;

					try
					{
						spin_lock.Enter(ref lockTaken);

						call_count_per_task[task_index]++;

						if (false == data_queue.IsEmpty)
						{
							data_queue.TryDequeue(out var param);
						}

						System.Threading.Thread.Sleep(thread_delay_ms);
					}
					finally
					{
						if (lockTaken)
						{
							spin_lock.Exit(false);
						}
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
							data_queue.TryDequeue(out var param);
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
					lock (lock_obj)
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

						if (false == data_queue.IsEmpty)
						{
							data_queue.TryDequeue(out var param);
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
					data_queue.TryDequeue(out var param);
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

	static void SpinLock_vs_ReadWriteLockSlim_vs_ExclusiveLock()
	{
		max_task_count = Math.Max(read_task_count, write_task_count);
		Console.WriteLine($"Task MaxCount : {max_task_count}");

		use_exclusive_lock_with_thread();

		use_exclusive_lock_with_task();

		use_read_write_lock_slim_with_task();

		use_spin_lock_with_task();

		Console.ReadLine();
	}


	public static void Test()
	{
		//PerformanceMonitor.start(10, Performance.ForCheck.print_profile_all);

		//SpinLock_vs_ReadWriteLockSlim_vs_ExclusiveLock();

		//Task_StartNew_with_ThreadPool();

		//PerformanceMonitor.stop();
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
