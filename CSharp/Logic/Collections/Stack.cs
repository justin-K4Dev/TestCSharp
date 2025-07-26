using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Concurrent;
using System.Threading;


namespace Collections
{
    public class Stack
    {
        static void stack_what()
        {
            /*
                자료구조 : 스택 (Stack)

                스택 (Stack)은 가장 나중에 추가된 데이타가 먼저 출력 처리되는(LIFO, Last In First Out) 자료 구조로서
                가장 최신 입력된 순서대로 처리해야 하는 상황에 이용된다.
                스택은 개념적으로 한 쪽 끝에서만 자료를 넣거나 뺄 수 있는 구조로 되어 있다.
                자료는 스택에 저장하는 것은 Push라 하고, 가장 최근 것부터 꺼내는 것은 Pop이라 한다.
                Stack의 Push와 Pop은 일반적으로 O(1)의 시간이 소요되지만,
                Push의 경우 만약 스택이 Full되어 동적 확장이 일어난다면 O(n)의 시간이 소요된다. 
            */
            {
                Console.ReadLine();
            }
        }


        static void Stack_use()
        {
            /*
                .NET에는 스택을 구현한 Non-generic인 Stack클래스와 이의 Generic 형태인 Stack<T> 클래스가 있다.
                Queue와 마찬가지로 .NET의 Stack클래스는 내부적으로 순환 배열 (Circular Array)으로 구현되어 있으며,
                스택이 가득 차면 자동으로 배열을 동적으로 확장하게 된다. 
            */
            {
                Stack<double> s = new Stack<double>();
                s.Push(10.5);
                s.Push(3.54);
                s.Push(4.22);

                double val = s.Pop(); //4.22

                Console.ReadLine();
            }
        }


        static void ConcurrentStack_use()
        {
            /*
                .NET 4.0 부터 멀티쓰레딩 환경에서 스택을 보다 간편하게 사용할 수 있는
                새로운 클래스인 ConcurrentStack<T> 가 제공되었다.
                Stack 클래스와 비슷하게 ConcurrentStack 의 기본 동작 메서드는 Push() 와 TryPop() 인데,
                ConcurrentStack 에는 Pop() 메서드가 없고 대신 TryPop() 메서드를 사용한다.

                아래 예제는 하나의 쓰레드가 ConcurrentStack 에 0부터 99까지 계속 집어 넣을 때,
                동시에 다른 쓰레드에서는 계속 그 스택에서 데이타를 빼내 읽어 오는 작업을 하는 샘플 코드이다.
                스택을 Pop 하는 속도를 약간 늦춤으로 해서 0 부터 99까지
                순차적으로 출력하지 않을 가능성이 더 커지게 하였다.
            */
            {
                var s = new ConcurrentStack<int>();

                // 데이타를 스택에 넣는 쓰레드
                Task tPush = Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        s.Push(i);
                        Thread.Sleep(100);
                    }
                });

                // 데이타를 스택에서 읽는 쓰레드
                Task tPop = Task.Factory.StartNew(() =>
                {
                    int n = 0;
                    int result;
                    while (n < 100)
                    {
                        if (s.TryPop(out result))
                        {
                            Console.WriteLine(result);
                            n++;
                        }
                        Thread.Sleep(150);
                    }
                });

                // 두 쓰레드가 끝날 때까지 대기
                Task.WaitAll(tPush, tPop);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //ConcurrentStack_use();

            //Stack_use();

            //stack_what();
        }
    }
}
