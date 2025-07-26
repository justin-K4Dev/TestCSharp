using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace MultiThread
{
    public class ManualResetEvent
    {
        class Worker
        {
            // ManualResetEvent 객체 필드
            public static System.Threading.ManualResetEvent manualEvent = new System.Threading.ManualResetEvent(false);

            public static void Run(object id)
            {
                Console.WriteLine("{0} in Wait", id);

                // ManualResetEvent 신호 대기
                manualEvent.WaitOne();

                Console.WriteLine("{0}: Done", id);
            }
        }

        static void ManualResetEvent_what()
        {
            /*
                ManualResetEvent는 하나의 쓰레드만 통과시키고 닫는 AutoResetEvent와 달리,
                한번 열리면 대기중이던 모든 쓰레드를 실행하게 하고 코드에서 수동으로 Reset()을 호출하여
                문을 닫고 이후 도착한 쓰레드들을 다시 대기토록 한다.
                아래는 여러 쓰레드의 실행을 중지시킨 후,
                ManualResetEvent로 신호를 보내 대기중이던 모든 쓰레들들을 한꺼번에 실행시키는 예제이다.
            */
            {
                // 10개의 쓰레드 생성
                // 10개 쓰레드 모두 manualEvent.WaitOne(); 에서
                // 실행 중지후 대기중
                for (int i = 0; i < 10; i++)
                {
                    new System.Threading.Thread(Worker.Run).Start(i);
                }

                // 메인쓰레드            
                System.Threading.Thread.Sleep(3000);

                // ManualResetEvent 객체 Set() 호출
                // 10개 쓰레드 모두 실행 계속함.
                Worker.manualEvent.Set();

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //ManualResetEvent_what();
        }
    }
}
