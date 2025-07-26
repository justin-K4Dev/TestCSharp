using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Loop
    {
        static void loop_for()
        {
            /*
                C# for 문은 루프 안에 있는 문장들을 반복적으로 실행할 때 사용한다.
                for 루프는 일반적으로 카운터 변수를 이용해 일정 범위 동안 for 루프 안의 블럭을 실행한다.

                다음 예제는 0부터 9까지 총 10번 콘솔 출력을 반복하는 코드이다.
            */
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Loop {0}", i);
                }

                Console.ReadLine();
            }
        }


        static void loop_foreach()
        {
            /*
                 C# foreach 문은 배열이나 컬렉션에 주로 사용하는데,
                 컬렉션의 각 요소를 하나씩 꺼내와서 foreach 루프 내의 블럭을 실행할 때 사용된다.
                 다음 예제는 문자열 배열을 foreach를 사용하여 각 문자열 요소를 하나씩 출력하는 코드이다. 
            */
            {
                string[] array = new string[] { "AB", "CD", "EF" };
                foreach (string s in array)
                {
                    Console.WriteLine(s);
                }

                Console.ReadLine();
            }
        }


        static void for_vs_foreach()
        {
            /*
                C# foreach는 for,while 등 다른 루프 문장보다 내부적으로 보다 최적화있는데,
                따라서 가능하면 foreach 를 사용할 것을 권장한다.
                특히 2차배열, 3차배열 등의 다중 배열을 처리할 경우, for루프는 배열 차수만큼 여러번 루프를 돌려야 하지만,
                foreach는 아래와 같이 단순히 한 루프 문장으로 이를 처리할 수 있어 편리하다. 
            */
            {
                // 3차배열 선언
                string[,,] arr = new string[,,] {
                            { {"1", "2"}, {"11","22"} }
                        ,   { {"3", "4"}, {"33", "44"} }
                    };

                //for 루프 : 3번 루프를 만들어 돌림
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        for (int k = 0; k < arr.GetLength(2); k++)
                        {
                            Console.WriteLine(arr[i, j, k]);
                        }
                    }
                }

                //foreach 루프 : 한번에 3차배열 모두 처리
                foreach (var s in arr)
                {
                    Console.WriteLine(s);
                }

                Console.ReadLine();
            }
        }


        static void loop_while()
        {
            /*
                C# while 문은 while 조건식이 참인 동안 계속 while 블럭을 실행할 때 사용한다.
                다음 예제는 while문을 사용하여 1부터 10까지 번호를 콘솔에 출력하는 코드이다.
                i 가 11 이 되면 while 조건식이 false가 되어 while 루프를 빠져나오게 된다.
            */
            {
                int i = 1;
                while (i <= 10)
                {
                    Console.WriteLine(i);
                    i++;
                }

                Console.ReadLine();
            }
        }


        static void example()
        {
            /*
                아래 예제는 콘솔로부터 Q키가 입력되지 전까지 계속 키 입력을 받아들인 후,
                그동안 입력된 키들을 foreach 루프를 써서 출력해 본 예이다.    
            */
            {
                List<char> keyList = new List<char>();
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey();
                    keyList.Add(key.KeyChar);
                } while (key.Key != ConsoleKey.Q); // Q가 아니면 계속

                Console.WriteLine();
                foreach (char ch in keyList) // 리스트 루프
                {
                    Console.Write(ch);
                }

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //example();

            //loop_while();

            //for_vs_foreach();

            //loop_foreach();

            //loop_for();
        }
    }
}
