using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using System.Net;
using System.IO;
using System.Windows.Forms;


namespace MultiThread
{
    public class Timer
    {
        class Updater
        {
            public void Run()
            {
                // 윈폼 타이머 사용
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000; // 1초
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }

            // 매 1초마다 Tick 이벤트 핸들러 실행
            void timer_Tick(object sender, EventArgs e)
            {
                // UI 쓰레드에서 실행. 
                // UI 컨트롤 직접 엑세스 가능
                Console.WriteLine("Tick - Arg:{0}", DateTime.Now.ToLongTimeString());
            }
        }

        // 쓰레드풀의 작업쓰레드가 지정된 시간 간격으로
        // 아래 이벤트 핸들러 실행
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 웹페이지 html문을 다운로드
            WebClient web = new WebClient();
            string webpage = web.DownloadString("http://mssql.tools");

            // 다운로드 내용을 파일에 저장
            string time = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            string outputFile = string.Format("page_{0}.html", time);
            File.WriteAllText(outputFile, webpage);
        }


        static void Timer_with_multi_thread()
        {
            /*
                .NET의 타이머는 크게 멀티쓰레딩을 지원하는
                System.Threading.Timer클래스, System.Timers.Timer 클래스와
                싱글쓰레드만을 지원하는
                System.Windows.Forms.Timer클래스, System.Windows.Threading.DispatcherTimer 클래스가 있다.
                멀티쓰레딩을 지원하는 Timer클래스들은 특정 간격으로 실행되는 이벤트 핸들러를
                쓰레드풀에서 할당된 작업쓰레드를 이용하여 실행하게 되고,
                항상 같은 작업쓰레드가 이벤트 핸들러를 실행한다는 보장이 없다.
                또한 만약 이벤트 핸들러가 다음 Interval 보다 오래 실행된다면,
                다른 작업쓰레드가 핸들러를 실행하게 되기 때문에, Thread Safe하게 작성해야 한다.
                아래 예제는 1시간마다 특정 웹페이지를 읽어와서 파일로 저장하는 예이다.
            */
            {
                // 타이머 생성 및 시작
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 60 * 60 * 1000; // 1 시간
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();

                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
            }

        }


        static void Timer_with_single_thread()
        {
            /*
                 Timer를 UI 프로그램에서 보다 편리한 사용을 위해,
                 윈폼 (WinForms)에는 System.Windows.Forms.Timer라는 클래스가 있으며,
                 WPF (Windows Presentation Foundation)에는 System.Windows.Threading.DispatcherTimer 클래스가 있다.
                 이들 타이머 클래스들은 Tick 이벤트 핸들러를 실행하기 위해
                 별도의 작업쓰레드를 생성하지 않고 UI 쓰레드에서 실행하기 때문에,
                 UI 컨트롤이나 UI Element들을 직접 이벤트 핸들러 안에서 마샬링 없이 엑세스할 수 있다.
                 타이머가 UI 쓰레드를 이용하기 때문에 만약 이벤트 핸들러가 긴 작업을 수행할 경우
                 UI Hang과 같은 현상이 있을 수 있다.
             */
            {
                Updater u = new Updater();
                u.Run();

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //Timer_with_single_thread();

            //Timer_with_multi_thread();
        }
    }
}
