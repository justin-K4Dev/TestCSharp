using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace MultiThread
{
    public class Semaphore
    {
        class ThreadManager
        {
            private System.Threading.Semaphore sema;

            public ThreadManager()
            {
                // 5개의 쓰레드만 허용
                sema = new System.Threading.Semaphore(5, 5);
            }

            public void Run(object seq)
            {
                // 쓰레드가 가진 데이타(일련번호)
                Console.WriteLine(seq);

                // 최대 5개 쓰레드만 아래 문장 실행
                sema.WaitOne();

                Console.WriteLine("Running#" + seq);
                System.Threading.Thread.Sleep(500);

                // Semaphore 1개 해체. 
                // 이후 다음 쓰레드 WaitOne()에서 진입 가능
                sema.Release();

            }
        }

        static void Semaphore_what()
        {
            /*
                Semaphore 클래스는 공유된 리소스를 지정된 수의 쓰레드들만 엑세스할 수 있게 허용하는데,
                예를 들어 최대 10개의 쓰레드들이 엑세스하도록 허용하였다면,
                11번째 쓰레드는 현재 사용 중인 10개의 쓰레드중 누군가가 리소스 사용을 마쳐야지만,
                그 리소스를 사용할 수 있게 된다.
                lock, Monitor, Mutex가 한번에 한 쓰레드만을 허용하는 반면,
                Semaphore는 복수 개의 쓰레드가 동시에 리소스를 엑세스하는 것을 허용한다.
            */
            {
                ThreadManager c = new ThreadManager();

                // 10개 쓰레드들 실행
                // 처음 5개만 먼저 실행되고 하나씩 해제와 함께
                // 실행될 것임.
                for (int i = 1; i <= 10; i++)
                {
                    new System.Threading.Thread(c.Run).Start(i);
                }

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //Semaphore_what();
        }
    }
}
