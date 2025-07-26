using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace MultiThread
{
    public class CountdownEvent
    {
        class Worker
        {
            // CountdownEvent 객체 필드
            public static System.Threading.CountdownEvent countEvent = new System.Threading.CountdownEvent(5);

            public static void Vote(object id)
            {
                if (countEvent.CurrentCount > 0)
                {
                    // CountdownEvent 신호. -1씩 카운트다운.
                    countEvent.Signal();

                    Console.WriteLine("{0}: Vote", id);
                }
                else
                {
                    Console.WriteLine("{0}: No vote", id);
                }
            }
        }

        static void CountdownEvent_what()
        {
            /*
                ManualResetEvent가 한 쓰레드에서 신호(Signal)을 보내 복수 쓰레드들을 통제하는데 사용되는 반면,
                .NET 4.0에 소개된 CountdownEvent는 한 쓰레드에서 복수 쓰레드들로부터의 신호들을 기다리는데 사용된다.
                아래는 10개의 쓰레드를 시작한 후,
                이 쓰레드들로부터 처음 5개의 신호가 (CountdownEvent.Signal() 메서드) 먼저 도착하는 대로
                메인쓰레드는 Wait 대기 상태를 해제하고 다음 문장을 실행하게 된다.
            */
            {
                // 10개의 쓰레드 시작
                // 10개중 5개만 Vote만 끝내면 중지            
                for (int i = 0; i < 10; i++)
                {
                    new System.Threading.Thread(Worker.Vote).Start(i);
                }

                // 메인쓰레드 첫 5개 신호를 기다림
                Worker.countEvent.Wait();

                Console.WriteLine("Vote is done!");
            }
        }

        public static void Test()
        {
            //CountdownEvent_what();
        }
    }
}
