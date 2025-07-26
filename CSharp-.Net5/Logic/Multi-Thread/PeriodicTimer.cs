using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread;


public class PeriodicTimer
{
	class MyEventPeriodicTimer : IAsyncDisposable
	{	
		private System.Threading.PeriodicTimer m_timer;
		private Task m_timerTask;
		private CancellationTokenSource m_cts;

		public MyEventPeriodicTimer()
		{
			m_cts = new CancellationTokenSource();

			m_timer = new System.Threading.PeriodicTimer(TimeSpan.FromSeconds(3));
			m_timerTask = handleTimerAsync(m_timer, m_cts.Token);
		}

		public void cancel() => m_cts.Cancel();

		async Task handleTimerAsync(System.Threading.PeriodicTimer timer, CancellationToken cancel = default)
		{
			try
			{
				while (await timer.WaitForNextTickAsync(cancel))
				{
					await Task.Run(() => onEventPeriodicTimer(cancel), cancel);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception exc)
			{
				//Handle the exception but don't propagate it
				Console.WriteLine(exc.Message);
			}

			Console.WriteLine("end handleTimerAsync()");
		}

		public async System.Threading.Tasks.ValueTask DisposeAsync()
		{
			m_timer.Dispose();
			await m_timerTask;

			GC.SuppressFinalize(this);
		}

		void onEventPeriodicTimer(CancellationToken cancel)
		{
			Console.WriteLine("Called onEventPeriodicTimer() !!!");
		}
	}

	static void PeriodicTimer_what()
	{
		/*
			.NET 6 에서 사용할 수 있는 PeriodicTimer 는 비동기 방식으로 타이머 틱을 처리하는 최신 타이머 API 입니다.			 
		*/
		{

			Console.WriteLine("Press any key to start");
			Console.ReadLine();

			var timer = new MyEventPeriodicTimer();

			var update_task = Task.Run(() =>
			{
				while (true)
				{
					Console.WriteLine("Press 'c' + 'Enter' to cancel");
					if (Console.ReadLine() == "c")
					{
						Console.WriteLine("call Timer.canel()");
						timer.cancel();
						break;
					}

					System.Threading.Thread.Sleep(1000);
				}
			});

			update_task.Wait();

			Console.ReadLine();
		}
	}

	public static void Test()
	{
		//PeriodicTimer_what();
	}
}
