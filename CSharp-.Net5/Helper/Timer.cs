using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Helper;

//============================================================================
// 심플 타이머	
// pause, resume 기능 추가
//============================================================================

public class Timer
{
	private readonly System.Diagnostics.Stopwatch m_stop_watch = new System.Diagnostics.Stopwatch();
	private readonly TimeSpan m_interval = default(TimeSpan);// 체크 주기
	private readonly bool m_is_manual_restart = false;  // 수동 재시작 여부
	private bool m_force_on = false;

	public Timer(TimeSpan intreval, bool is_pause = false, bool is_manual_restart = false)
	{
		m_interval = intreval;
		m_is_manual_restart = is_manual_restart;

		if (is_pause)
		{
			pause();
		}
		else
		{
			resume();
		}
	}

	public Timer(Int64 interval_milliseconds, bool is_pause = false, bool is_manual_restart = false)
		: this(new TimeSpan(interval_milliseconds * TimeSpan.TicksPerMillisecond), is_pause, is_manual_restart)
	{
	}

	public Timer(float intreval_seconds, bool is_pause = false, bool is_manual_restart = false)
		: this((Int64)(intreval_seconds * 1000.0f), is_pause, is_manual_restart)
	{
	}

	//=========================================================================
	// 지정된 시간 만큼 경과 되었는지 검사한다.
	//=========================================================================
	public bool isOn()
	{
		if (m_force_on)
		{
			m_force_on = false;
		}
		else
		{
			if (m_stop_watch.IsRunning == false)
			{
				return false;
			}

			if (Elapsed < m_interval)
			{
				return false;
			}
		}

		if (m_is_manual_restart)
		{
			// 한번만 실행되고 멈춘다, 다시 시작하려면, restart를 호출해야 한다.
			pause();
		}
		else
		{
			// 자동리셋이면 재시작
			restart();
		}

		return true;
	}

	// 강제로 타이머를 On상태로 만든다.
	public void forceOn()
	{
		m_force_on = true;
	}

	//=========================================================================
	// isOn의 기능과, 경과시간을 얻어온다.
	//=========================================================================
	public bool isOn(out TimeSpan elapsed)
	{
		elapsed = Elapsed;
		return isOn();
	}


	//=========================================================================
	// 타이머가 멈춰있는가?
	//=========================================================================
	public bool isPaused()
	{
		return m_stop_watch.IsRunning == false;
	}


	//=========================================================================
	// 타이머가 실행중인가?
	//=========================================================================
	public bool isRunning()
	{
		return m_stop_watch.IsRunning;
	}


	//=========================================================================
	// 일시정지
	//=========================================================================
	public void pause()
	{
		m_stop_watch.Stop();
	}

	//=========================================================================
	// 시작 또는 일시정지를 종료하고 다시 시작
	//=========================================================================
	public void resume()
	{
		m_stop_watch.Start();
	}

	//=========================================================================
	// 타이머를 초기화하고, 시작
	//=========================================================================
	public TimeSpan restart()
	{
		TimeSpan elapsed = m_stop_watch.Elapsed;
		m_stop_watch.Reset();
		m_stop_watch.Start();
		return elapsed;
	}

	//=========================================================================
	// 경과 시간 조회
	//=========================================================================       
	public TimeSpan Elapsed
	{
		get { return m_stop_watch.Elapsed; }
	}
	//=========================================================================
	// 남은 시간 조회
	//=========================================================================       
	public TimeSpan RemainTime
	{
		get { return Elapsed.TotalSeconds > 0 ? (m_interval > Elapsed ? m_interval - Elapsed : new TimeSpan(0)) : new TimeSpan(0); }
	}


}//Timer
