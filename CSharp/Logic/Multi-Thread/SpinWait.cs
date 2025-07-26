using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace MultiThread
{
	public class SpinWait
	{
		public class LockFreeStack<T>
		{
			private volatile Node m_head;

			private class Node { public Node Next; public T Value; }

			public void push(T item)
			{
				var spin = new System.Threading.SpinWait();
				Node node = new Node { Value = item }, head;
				while (true)
				{
					head = m_head;
					node.Next = head;
					if (Interlocked.CompareExchange(ref m_head, node, head) == head)
					{
						break;
					}

					spin.SpinOnce();
				}
			}

			public bool tryPop(out T result)
			{
				result = default(T);
				var spin = new System.Threading.SpinWait();

				Node head;
				while (true)
				{
					head = m_head;

					if (head == null)
					{
						return false;
					}

					if (Interlocked.CompareExchange(ref m_head, head.Next, head) == head)
					{
						result = head.Value;
						return true;
					}

					spin.SpinOnce();
				}
			}

			public bool isEmpty() { return m_head == null; }
		}

		static void SpinWait_what()
		{
			/*
				.Net 에서는 SpinWait을 할 수 있는 두가지 방법을 제공 한다.
				
				첫번째는 Thread.SpinWait() 메소드
				=>	Thread.SpinWait(100);
					Thread.SpinWait()는 주어진 iterationCount 만큼만 스핀 한다.
					Thread.SpinWait는 스핀 횟수와 관계 없이 스케쥴링된 타임을 절대 양보하지 않는다.
					딱 한가지 양보하는 것은 같은 코어에서 동시에 돌아가고 있는 쓰레드에게는 양보한다. (Yield Processor)
					
				두번째는 Thread.SpinWait 구조체
				=> 	var wait = new SpinWait();
					for(var i = 0; i < 100 ; i++)
					{
						wait.SpinOnce();
					}

					SpinWait.SpinOnce() 는 딱 1회 스핀을 수행하는 것이 아니다.
					지금까지 얼마나 돌았는지 보고 적절한 스핀 횟수만큼 스핀하게 된다.
					(따라서 SpinWait를 미리 만들어놓고 재사용하는 건 좋지 않다)
					SpinWait 는 때때로 스케쥴링을 양보하기도 한다.
					(SwitchToThread), NextSpinWillYield 프로퍼티를 이용해 다음 스핀이 스케쥴링을 양보할것인지 조사할 수 있다.

				상황에 따라 적절한 것을 사용하면 되겠지만, 대부분의 경우에 SpinWait가 더 좋은 선택이라고 볼 수 있다.
				SpinWait는 Thread.SpinWait()를 래핑하여 알아서 돌만큼 돌고, 쉴만큼 쉬는것이 구현된 좀더 고수준의 스핀 이다.
			*/
			{
				var stack_with_lockfree = new LockFreeStack<Int32>();

				var write_task_count = 100;
				var read_task_count = 100;

				var write_tasks = new Task[write_task_count];
				var read_tasks = new Task[read_task_count];

				var started_time = DateTime.Now;

				for (var i = 0; i < write_task_count; i++)
				{
					var task_index = i;
					
					write_tasks[task_index] = Task.Run(() =>
					{
						stack_with_lockfree.push(i);
					});
				}

				for (var i = 0; i < read_task_count; i++)
				{
					var task_index = i;

					read_tasks[task_index] = Task.Run(() =>
					{
						Int32 value;
						stack_with_lockfree.tryPop(out value);
					});
				}


				Console.WriteLine($"Stack Empty:{stack_with_lockfree.isEmpty().ToString()}");

				Task.WaitAll(read_tasks);
				Task.WaitAll(write_tasks);					
			}

			Console.ReadLine();
		}

		// IMPORTANT:
		// Use this only for very short sleep intervals. 
		// One CPU core will run with 100% load during this interval.
		static void SleepPrecise(Int32 intervalMS) // in ms
		{
			if (intervalMS <= 0)
			{
				return;
			}

			// System.Diagnostics.Stopwatch uses the performance counter in the processor 
			// which has a precision of micro seconds or even nano seconds.
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			while (stopwatch.ElapsedMilliseconds < intervalMS)
			{
				// SpinWait(80000) --> 1 ms on a 3.6 GHz AMD Radeon processor
				System.Threading.Thread.SpinWait(1000);
			}

			Console.WriteLine($"Sleep TimeMS:{stopwatch.ElapsedMilliseconds}");
		}

		static void SpinWait_for_sleep()
		{
			System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;

			try
			{
				SleepPrecise(5000);
			}
			finally
			{
				System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Normal;
			}

			Console.ReadLine();
		}

		class Latch
		{
			private object latchLock = new object();
			// 0 = unset, 1 = set.
			private int m_state = 0;
			private volatile Int32 totalKernelWaits = 0;
			private volatile Int32 maxSpinCount = 0;

			// Block threads waiting for ManualResetEvent.
			private System.Threading.ManualResetEvent m_ev = new System.Threading.ManualResetEvent(false);

			// For fast logging with minimal impact on latch behavior.
			// Spin counts greater than 20 might be encountered depending on machine config.
			private long[] spinCountLog = new long[20];

			public void DisplayLog()
			{
				Console.WriteLine($"Max Spin Count:{maxSpinCount}");
				for (int i = 0; i < spinCountLog.Length; i++)
				{
					Console.WriteLine( "Wait succeeded with spin count of {0} on {1:N0} attempts"
									 , i, spinCountLog[i] );
				}
				Console.WriteLine("Wait used the kernel event on {0:N0} attempts.", totalKernelWaits);
				Console.WriteLine("Logging complete");
			}


			public void Set()
			{
				lock (latchLock)
				{
					m_state = 1;
					m_ev.Set();
				}
			}

			public void Wait()
			{
				Trace.WriteLine("Wait timeout infinite");
				Wait(Timeout.Infinite);
			}

			public bool Wait(int timeout)
			{
				var spinner = new System.Threading.SpinWait();
				Stopwatch watch;

				while (m_state == 0)
				{
					// Lazily allocate and start stopwatch to track timeout.
					watch = Stopwatch.StartNew();

					// Spin only until the SpinWait is ready
					// to initiate its own context switch.
					if (!spinner.NextSpinWillYield)
					{
						spinner.SpinOnce();
					}
					// Rather than let SpinWait do a context switch now,
					//  we initiate the kernel Wait operation, because
					// we plan on doing this anyway.
					else
					{
						Interlocked.Increment(ref totalKernelWaits);
						// Account for elapsed time.
						long realTimeout = timeout - watch.ElapsedMilliseconds;

						// Do the wait.
						if (realTimeout <= 0 || !m_ev.WaitOne((int)realTimeout))
						{
							Trace.WriteLine("wait timed out.");
							return false;
						}
					}
				}

				var curr_spin_count = spinner.Count;
				Interlocked.Increment(ref spinCountLog[curr_spin_count]);

				if(maxSpinCount < curr_spin_count)
				{
					maxSpinCount = curr_spin_count;
				}

				// Take the latch.
				Interlocked.Exchange(ref m_state, 0);

				return true;
			}
		}

		static void IncreaseCount(Latch latch, Int32 count, CancellationTokenSource cts)
		{
			while (!cts.IsCancellationRequested)
			{
				// Obtain the latch.
				if (latch.Wait(50))
				{
					// Do the work. Here we vary the workload a slight amount
					// to help cause varying spin counts in latch.
					double d = 0;
					if (count % 2 != 0)
					{
						d = Math.Sqrt(count);
					}
					Interlocked.Increment(ref count);

					// Release the latch.
					latch.Set();
				}
			}
		}

		static void SpinWait_with_two_phase_wait_operation()
		{
			var latch = new Latch();
			var count = 2;
			CancellationTokenSource cts = new CancellationTokenSource();

			// Demonstrate latch with a simple scenario: multiple
			// threads updating a shared integer. Both operations
			// are relatively fast, which enables the latch to
			// demonstrate successful waits by spinning only.
			latch.Set();

			// UI thread. Press 'c' to cancel the loop.
			Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Press 'c' to cancel.");
				if (Console.ReadKey(true).KeyChar == 'c')
				{
					cts.Cancel();
				}
			});

			System.Threading.Tasks.Parallel.Invoke( () => IncreaseCount(latch, count, cts)
												  ,	() => IncreaseCount(latch, count, cts)
												  ,	() => IncreaseCount(latch, count, cts) );


			latch.DisplayLog();
			if (cts != null)
			{
				cts.Dispose();
			}

			Console.ReadLine();
		}

		public static void Test()
		{
			//SpinWait_with_two_phase_wait_operation();

			//SpinWait_for_sleep();

			//SpinWait_what();
		}
	}
}
