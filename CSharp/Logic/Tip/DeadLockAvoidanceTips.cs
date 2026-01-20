using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace Tip
{
    public class DeadLockAvoidanceTips
    {
        static async Task<string> getDataAsync()
        {
            await Task.Delay(200);
            return "OK";
        }

        // async는 끝까지 async로
        static async Task deadlock_AsyncAllTheWay()
        {
            /*
                async는 끝까지 async로

                  - 의미: 중간에 .Result, .Wait(), .GetAwaiter().GetResult()로 “동기화”하지 말고 위로 다 await 올려라.
                  - 이유: 컨텍스트 있는 환경(UI, ASP.NET 옛)에서 중간에 동기 대기하면 데드락 잘 남.            
            */
            {
                string s = await getDataAsync();  // 끝까지 async
                Console.WriteLine(s);
            }

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        static string getStringDataSync()
        {
            // 동기로 기다려야만 하는 구석진 곳이라고 가정
            return getStringDataAsync().GetAwaiter().GetResult();
        }

        static async Task<string> getStringDataAsync()
        {
            await Task.Delay(200).ConfigureAwait(false);  // ★ 복귀 안 함
            return "OK";
        }

        static void deadlock_ConfigureAwaitFalse()
        {
            /*
                ConfigureAwait(false) 쓰기

                  - 의미:“다시 UI/요청 컨텍스트로 돌아올 필요 없다”면 붙여서 컨텍스트 복귀를 막는다.
                  - 이유: 아래처럼 동기 대기하는 코드가 있어도 데드락 가능성을 줄여줌.            
            */
            {
                string result = getStringDataSync();

                Console.WriteLine($"result = {result}");
                Console.WriteLine("끝");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static Task initAsync()
        {
            return Task.CompletedTask;  // 즉시 완료, 동기 Wait 해도 안전
        }

        static async Task deadlock_ReturnCompletedTaskForSyncCall()
        {
            /*
                "이미 끝난 Task”를 돌려주기

                  - 의미: 동기 자리(생성자 이후 init, Dispose 전 후크 등)에서 굳이 진짜 비동기 안 돌고 
                         “끝난 Task”를 주면, 호출자가 .Wait() 해도 안 막힌다.
                  - 이유: 안에서 대기할 게 없으니 데드락이 안 생김.          
            */
            {
                // 여기서는 동기로 초기화를 끝내야 한다고 가정
                initAsync().GetAwaiter().GetResult();

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

        static async Task doWorkAsync()
        {
            await _gate.WaitAsync();
            try
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} in");
                // 보호구간
                await Task.Delay(200);    // 여기서 await 해도 안전
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} out");
            }
            finally
            {
                _gate.Release();
            }
        }

        static async Task deadlock_AsyncSemaphoreInsteadOfLock()
        {
            /*
                lock 대신 SemaphoreSlim.WaitAsync()

                  - 의미: 비동기 코드에서 lock 안에 await 넣지 말고, 비동기 대기 가능한 세마포어를 쓰자.
                  - 이유: lock은 스레드를 점유한 채 기다리니까 교착이 쉬운데, WaitAsync는 스레드 비워둬서 여유가 생김.          
            */
            {
                // 동시에 여러 번 호출해도 _gate가 1개씩만 들어가게 해줌
                var t1 = doWorkAsync();
                var t2 = doWorkAsync();
                var t3 = doWorkAsync();

                await Task.WhenAll(t1, t2, t3);

                Console.WriteLine("all done");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        private static readonly object _lockA = new object();
        private static readonly object _lockB = new object();

        // 항상 A → B 순서
        static void doJob1()
        {
            lock (_lockA)
            {
                lock (_lockB)
                {
                    // ...
                }
            }
        }

        // 여기서도 A → B 순서 유지
        static void doJob2()
        {
            lock (_lockA)
            {
                lock (_lockB)
                {
                    // ...
                }
            }
        }

        static async Task deadlock_LockOrderingFixed()
        {
            /*
                락 순서(lock ordering) 고정

                  - 의미: 여러 리소스를 잠글 때 순서를 항상 같게 해서 “A가 B 기다리고 B가 A 기다리는” 상황을 방지.
                  -       이건 동기/비동기 둘 다 해당.          
            */
            {
                // 두 작업을 동시에 돌려서도 데드락이 안 나는지 보자
                var t1 = new Thread(doJob1);
                var t2 = new Thread(doJob2);

                t1.Start();
                t2.Start();

                t1.Join();
                t2.Join();

                Console.WriteLine("끝");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static async Task doWithTimeoutAsync()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            await someAsync(cts.Token);   // 3초 안에 안 끝나면 취소
        }

        static async Task someAsync(CancellationToken token)
        {
            await Task.Delay(5000, token);  // 5초지만 위에서 3초 제한
        }

        static async Task tryEnterGateAsync()
        {
            // 2초 안에 못 들어가면 포기
            if (await _gate.WaitAsync(2000) == false)
            {
                Console.WriteLine("2초 안에 락 못 잡아서 포기");
                return;
            }

            try
            {
                Console.WriteLine("락 잡았음, 작업 중...");
                await Task.Delay(500); // 보호구간
            }
            finally
            {
                _gate.Release();
            }
        }

        static async Task deadlock_TimeoutOrCancellation()
        {
            /*
                타임아웃 / 취소 토큰 걸기

                  - 의미: 혹시나 교착이 생겨도 영원히 안 기다리게 탈출구를 만들어 둔다.
                  -       진짜 원천 방지책은 아니지만 “프로세스 멈춰버림”을 막음.          
            */
            {
                // 1) 토큰으로 3초 제한 두는 예제 호출
                await doWithTimeoutAsync();
                Console.WriteLine("doWithTimeoutAsync 끝");

                // 2) 세마포어에 2초 안에 못 들어가면 포기하는 예제 호출
                await tryEnterGateAsync();
                Console.WriteLine("tryEnterGateAsync 끝");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        private class MyForm : Form
        {
            private TextBox textBox1;
            private Button button1;

            public MyForm()
            {
                textBox1 = new TextBox { Left = 10, Top = 10, Width = 200 };
                button1 = new Button { Left = 10, Top = 40, Width = 80, Text = "Load" };

                // 좋은 예: 이벤트 핸들러 자체를 async 로
                button1.Click += button1_Click;

                Controls.Add(textBox1);
                Controls.Add(button1);
            }

            // ✅ 끝까지 async 로 간다
            private async void button1_Click(object sender, EventArgs e)
            {
                textBox1.Text = "loading...";
                var data = await getDataAsync();   // UI 스레드를 막지 않음
                textBox1.Text = data;
            }

            private async Task<string> getDataAsync()
            {
                await Task.Delay(500);  // 네트워크/IO 대기라고 가정
                return "OK";
            }
        }

        static async Task deadlock_NoBlockingOnContext()
        {
            /*
                "컨텍스트 있는 곳에서는 동기 호출 금지" 정책화

                  - 의미: UI 스레드 / ASP.NET 요청 스레드에서는 .Result / .Wait()을 아예 팀 룰로 막아버린다.
                  - 이유: 이 환경은 SynchronizationContext가 있어서 “다시 여기로 돌아올게”가 기본인데,
                          거기서 스레드를 막으면 바로 데드락 패턴.
            */
            {
                Application.EnableVisualStyles();
                Application.Run(new MyForm());

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            deadlock_NoBlockingOnContext().Wait();

            deadlock_TimeoutOrCancellation().Wait();

            deadlock_LockOrderingFixed().Wait();

            deadlock_AsyncSemaphoreInsteadOfLock().Wait();

            deadlock_ReturnCompletedTaskForSyncCall().Wait();

            deadlock_ConfigureAwaitFalse();

            deadlock_AsyncAllTheWay().Wait();
        }
    }
}
