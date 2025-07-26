using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Diagnostics;

namespace MultiThread
{
    public class Mutex
    {
        class NumberList
        {
            // MutexName1 이라는 뮤텍스 생성
            private static System.Threading.Mutex mtx = new System.Threading.Mutex(false, "MutexName1");

            // 데이타 멤버
            public static List<int> MyList = new List<int>();

            // 데이타를 리스트에 추가
            public static void AddList(int val)
            {
                // 먼저 뮤텍스를 취득할 때까지 대기
                mtx.WaitOne();

                // 뮤텍스 취득후 실행 블럭
                MyList.Add(val);

                // 뮤텍스 해제
                mtx.ReleaseMutex();
            }

            // 리스트 출력
            public static void ShowList()
            {
                MyList.ForEach(p => Console.WriteLine(p));
            }
        }


        static void Mutex_what()
        {
            /*
                Mutex 클래스는 Monitor클래스와 같이
                특정 코드 블럭(Critiacal Section)을 배타적으로 Locking하는 기능을 가지고 있다.
                단, Monitor클래스는 하나의 프로세스 내에서만 사용할 수 있는 반면,
                Mutex 클래스는 해당 머신의 프로세스간에서도 배타적 Locking을 하는데 사용된다.
                Mutex 락킹은 Monitor 락킹보다 약 50배 정도 느리기 때문에
                한 프로세스내에서만 배타적 Lock이 필요한 경우는 C#의 lock이나 Monitor 클래스를 사용한다.
                아래 예제는 MyClass가 외부 프로세스에서도 사용할 수 있다는 가정하에
                배타적 Lock으로 뮤텍스를 사용하는 예이다.
                (실제로는 한 프로세스 내에서 사용할 경우, Monitor를 사용하는 것이 빠른 방식이다)
            */
            {
                // 2개의 쓰레드 실행
                System.Threading.Thread t1 = new System.Threading.Thread(() => NumberList.AddList(10));
                System.Threading.Thread t2 = new System.Threading.Thread(() => NumberList.AddList(20));
                t1.Start();
                t2.Start();

                // 2개의 쓰레드 실행완료까지 대기
                t1.Join();
                t2.Join();

                // 메인쓰레드에서 뮤텍스 사용
                using (System.Threading.Mutex m = new System.Threading.Mutex(false, "MutexName1"))
                {
                    // 뮤텍스를 취득하기 위해 10 ms 대기
                    if (m.WaitOne(10))
                    {
                        // 뮤텍스 취득후 MyList 사용
                        NumberList.MyList.Add(30);
                    }
                    else
                    {
                        Console.WriteLine("Cannot acquire mutex");
                    }
                }

                NumberList.ShowList();

                Console.ReadLine();
            }
        }


        static void Mutex_with_Process()
        {
            /*
                Mutex 활용에 적합한 예로서 흔히 한 머신 내에서
                오직 한 응용프로그램(Application)만이 실행되도록 하는 테크닉을 든다.
                한 컴퓨터 내 한 프로세스만 뜨게 하기 위해 고유의 Mutex명을 지정할 필요가 있는데,
                일반적으로 이를 위해 GUID (globally unique identifier)를 사용한다.
                처음 프로세스가 먼저 Mutex를 획득하면 다른 프로세스는 Mutex를 획득할 수 없기 때문에
                오직 하나의 프로그램만 머신 내에서 실행되는 것이다.
            */
            {
                // Unique한 뮤텍스명을 위해 주로 GUID를 사용한다.
                string mtxName = "60C3D9CA-5957-41B2-9B6D-419DC9BE77DF";

                // 뮤텍스명으로 뮤텍스 객체 생성 
                // 만약 뮤텍스를 얻으면, createdNew = true
                bool createdNew;
                System.Threading.Mutex mtx = new System.Threading.Mutex(true, mtxName, out createdNew);

                // 뮤텍스를 얻지 못하면 에러
                if (!createdNew)
                {
                    Console.WriteLine("에러: 프로그램 이미 실행중");
                    return;
                }

                // 성공하면 본 프로그램 실행
                System.Diagnostics.Process.Start("");
            }
        }


        public static void Test()
        {
            //Mutex_with_Process();

            //Mutex_what();
        }
    }
}
