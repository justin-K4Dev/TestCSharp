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


		static void read_write_lock_slim_vs_lock()
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
			//read_write_lock_slim_vs_lock();
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
