using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Helper
{
	//===========================================================================================
	// 주기적 작동 타이머 각종 지원 함수
	//
	// author : kangms
	// 
	//===========================================================================================

	public class LogicPeriodicTimer : IAsyncDisposable
	{
		private readonly System.Threading.PeriodicTimer m_timer;
		private readonly Task m_timer_task;
		private readonly CancellationTokenSource m_cts;

		public delegate void FnFunction();
		private readonly FnFunction m_fn_alarm_function;

		public LogicPeriodicTimer(Int32 intervalSec, FnFunction fnAlarmFunction)
		{
			m_cts = new CancellationTokenSource();

			m_timer = new System.Threading.PeriodicTimer(TimeSpan.FromSeconds(intervalSec));

			m_fn_alarm_function = fnAlarmFunction;
			m_timer_task = handleTimerAsync(m_timer, m_cts.Token);
		}

		private async Task handleTimerAsync(System.Threading.PeriodicTimer timer, CancellationToken cancel = default)
		{
			try
			{
				while (await timer.WaitForNextTickAsync(cancel))
				{
					await Task.Run(() => m_fn_alarm_function.Invoke());
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (System.Exception e)
			{
				var err_msg = $"LogicPeriodicTimer exception !!! : errMsg:{e.Message}";

				Console.WriteLine(err_msg);
			}
		}

		public void cancelTimer() => m_cts.Cancel();

		public async ValueTask DisposeAsync()
		{
			m_timer.Dispose();
			await m_timer_task;

			GC.SuppressFinalize(this);
		}

		public System.Threading.PeriodicTimer getPeriodicTimer() => m_timer;
	}
}
