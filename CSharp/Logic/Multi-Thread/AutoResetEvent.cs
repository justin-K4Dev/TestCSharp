using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace MultiThread
{
    public class AutoResetEvent
    {
        static void thread_synch_by_signal_what()
        {
            /*
                신호(Signal)에 의한 쓰레드 동기화 

                쓰레드 동기화에는 Locking과 같이 공유 리소스에 락을 걸어 쓰레드 접근을 제한하는 방식
                (Monitor, Mutex, Semaphore 등)
                이외에 대기 중인 쓰레드에게 외부에서 신호(Signal)을 보냄으로써 쓰레드 흐름을 통제하는 방식도 있다.
                이러한 신호(Signaling) 방식으로 많이 사용되는 것으로
                AutoResetEvent, ManualResetEvent, CountdownEvent, Wait/Pulse 등이 있다.
                (NOTE: 여기서의 Event는 윈도우 프로그래밍에서 말하는 이벤트와 전혀 다른 개념이다.
                여기서의 Event는 쓰레드 동기화에 사용되는 OS 리소스이다) 
            */
            {
                Console.ReadLine();
            }
        }


        class Worker
        {
            // AutoResetEvent 객체 필드
            public static System.Threading.AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(false);

            public static void Run()
            {
                string name = System.Threading.Thread.CurrentThread.Name;
                Console.WriteLine("{0}: Run Start", name);

                // AutoResetEvent 신호 대기
                autoEvent.WaitOne();
                Console.WriteLine("{0} : DoWork", name);

                Console.WriteLine("{0}: Run End", name);
            }
        }

        static void AutoResetEvent_concept()
        {
            /*
                AutoResetEvent는 이 이벤트를 기다리는 쓰레드들에게 신호를 보내 하나의 쓰레드만 통과시키고
                나머지 쓰레드들은 다음 신호를 기다리게 한다.
                이는 흡사 유료 주차장 자동 게이트와 같이 한 차량이 통과하면
                자동으로 게이트가 닫히는 것과 같다.
                쓰레드 A가 AutoResetEvent 객체의 WaitOne() 메소드를 써서 대기하고 있다가,
                다른 쓰레드 B에서 이 AutoResetEvent 객체의 Set() 메서드를 호출하면,
                쓰레드 A는 대기 상태를 해제하고 계속 다음 문장을 실행할 수 있게 된다.
            */
            {
                // 쓰레드 A 생성
                System.Threading.Thread A = new System.Threading.Thread(Worker.Run);
                A.Name = "Thread A";
                A.Start();

                // 메인쓰레드            
                System.Threading.Thread.Sleep(3000); //3초 대기
                Worker.autoEvent.Set(); // 쓰레드 A에 신호

                Console.ReadLine();
            }
        }


        class Traffic
        {
            private bool _running = true;

            // 상하, 좌우 통행 신호 역활을 하는 AutoResetEvent 이벤트들
            private System.Threading.AutoResetEvent _evtVert = new System.Threading.AutoResetEvent(true);
            private System.Threading.AutoResetEvent _evtHoriz = new System.Threading.AutoResetEvent(false);

            private Queue<int> _Qvert = new Queue<int>();
            private Queue<int> _Qhoriz = new Queue<int>();

            // 상하방향의 큐 데이타 처리
            // Vertical 방향의 처리 신호(_evtVert)를 받으면
            // Vertical 큐의 모든 큐 아이템을 처리하고
            // 좌우방향 처리 신호(_evtHoriz)를 보냄
            public void ProcessVertical()
            {
                while (_running)
                {
                    // Vertical 방향 처리 신호 기다림
                    _evtVert.WaitOne();

                    // Vertical 큐의 모든 데이타 처리
                    // 큐는 다른 쓰레드에서 엑세스 가능하므로 lock을 건다
                    lock (_Qvert)
                    {
                        while (_Qvert.Count > 0)
                        {
                            int val = _Qvert.Dequeue();
                            Console.WriteLine("Vertical : {0}", val);
                        }
                    }

                    // Horizontal 방향 처리 신호 보냄
                    _evtHoriz.Set();
                }

                Console.WriteLine("ProcessVertical : Done");
            }

            // 좌우방향의 큐 데이타 처리
            // Horizontal 방향의 처리 신호(_evtHoriz)를 받으면
            // Horizontal 큐의 모든 큐 아이템을 처리하고
            // 상하방향 처리 신호(_evtHoriz)를 보냄
            public void ProcessHorizontal()
            {
                while (_running)
                {
                    _evtHoriz.WaitOne();

                    lock (_Qhoriz)
                    {
                        while (_Qhoriz.Count > 0)
                        {
                            int val = _Qhoriz.Dequeue();
                            Console.WriteLine("Horizontal : {0}", val);
                        }
                    }

                    _evtVert.Set();
                }

                Console.WriteLine("ProcessHorizontal : Done");
            }

            public bool Running
            {
                get { return _running; }
                set { _running = value; }
            }

            public void AddVertical(int[] data)
            {
                lock (_Qvert)
                {
                    foreach (var item in data)
                    {
                        _Qvert.Enqueue(item);
                    }
                }
            }

            public void AddHorizontal(int[] data)
            {
                lock (_Qhoriz)
                {
                    foreach (var item in data)
                    {
                        _Qhoriz.Enqueue(item);
                    }
                }
            }
        }

        static void AutoResetEvent_use()
        {
            /*
                AutoResetEvent 활용한 한 예제로 교통 신호를 통제하는 예를 들어 보자.
                실제 교통 신호보다 좀 단순화하여 상하방향과 좌우방향만을 각각 통제하는 2개의 신호만이 있다고 가정하자.
                상하방향 신호가 켜지면 상하방향 차량들이 모두 통과되고
                (즉, 상하방향 Queue에 있는 모두 데이타 처리),
                더 이상 차량이 없으면 좌우방향에 신호를 보내 좌우방향에 밀려 있는 차량들을 통과한다
                (즉, 좌우방향 Queue에 있는 모두 데이타 처리).
                아래 예제는 2개의 쓰레드가 각각 상하, 좌우 데이타 처리를 위한 메서드들을 실행하고,
                2개의 AutoResetEvent 객체를 신호로 사용하고 있다.
                각 쓰레드의 메서드는 먼저 자신의 신호 이벤트를 기다렸다가 이벤트가 Set되면
                큐의 있는 모든 데이타를 처리한 후 상대편 이벤트를 Set하여 상대편에게 실행 제어권을 넘긴다.
                메인쓰레드에서는 계속 데이타를 큐에 넣는 역활을 하고 있다.
            */
            {
                Traffic traffic = new Traffic();

                // 2개의 쓰레드 구동
                System.Threading.Thread v = new System.Threading.Thread(traffic.ProcessVertical);
                System.Threading.Thread h = new System.Threading.Thread(traffic.ProcessHorizontal);
                v.Start();
                h.Start();

                // 메인쓰레드에서 데이타 전송
                for (int i = 0; i < 30; i += 3)
                {
                    traffic.AddVertical(new int[] { i, i + 1, i + 2 });
                    traffic.AddHorizontal(new int[] { i, i + 1, i + 2 });
                    System.Threading.Thread.Sleep(10);
                }

                System.Threading.Thread.Sleep(1000);
                traffic.Running = false;

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //AutoResetEvent_use();

            //AutoResetEvent_concept();

            //thread_synch_by_signal_what();
        }
    }
}
