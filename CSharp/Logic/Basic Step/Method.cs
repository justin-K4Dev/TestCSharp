using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Method
    {
        static void method_what()
        {
            /*
                클래스내에서 일련의 코드 블럭을 실행시키는 함수를 메서드라 부른다.
                메서드는 0 ~ N개의 인수를 갖을 수 있으며, 하나의 리턴 값을 갖는다.
                리턴 값이 없으면 리턴 타입을 void로 표시한다.
                또한 public, private 같은 접근 제한자를 리턴 타입 앞에 둘 수 있다.
                아래는 전형적인 메서드의 예이다. 이는 a,b,c 라는 3개의 인수를 받아 들이고,
                int 타입의 데이타를 리턴하는 public 메서드이다.

                    public int GetData(int a, string b, bool c)
                    {
                    }
            */
            {
                Console.ReadLine();
            }
        }


        private void Calculate(int a)
        {
            a *= 2;
        }

        static void pass_by_value()
        {
            /*
                C#은 메서드에 인수를 전달할 때, 디폴트로 값을 복사해서 전달하는 Pass by Value 방식을 따른다.
                만약 전달된 인수를 메서드 내에서 변경한다해도 메서드가 끝나고 함수가 리턴된 후,
                전달되었던 인수의 값은 호출자에서 원래 값 그대로 유지된다. 
            */
            {
                Method m = new Method();

                int val = 100;

                m.Calculate(val);
                //val는 그대로 100

                Console.ReadLine();
            }
        }


        //ref 정의
        static double GetData(ref int a, ref double b)
        { return ++a * ++b; }

        //out 정의
        static bool GetData(int a, int b, out int c, out int d)
        {
            c = a + b;
            d = a - b;
            return true;
        }

        static void pass_by_reference()
        {
            /*
                메서드에 파라미터를 전달할 때, 만약 레퍼런스(참조)로 전달하고자 한다면 C# 키워드 ref를 사용한다.
                ref를 사용할 경우 메서드 내에서 변경된 값은 리턴 후에도 유효하다.
                ref를 사용하기 위해서는 ref로 전달되는 변수가 사전에 초기화되어져야 한다.

                C#의 ref와 비슷한 기능을 하는 것으로 C# out 키워드가 있다.
                out을 사용하는 파라미터는 메서드 내에서 그 값을 반드시 지정하여 전달하게 되어 있다.
                C#의 ref는 해당 변수가 사전에 초기화되어야 하지만, C# out은 사전에 변수를 초기화할 필요는 없다. 
            */
            {
                // ref 사용. 초기화 필요.
                int x = 1;
                double y = 1.0;
                double ret = GetData(ref x, ref y);

                // out 사용. 초기화 불필요.
                int c, d;
                bool bret = GetData(10, 20, out c, out d);

                Console.ReadLine();
            }
        }


        static void named_parameter()
        {
            /*
                메서드에 파라미터를 전달할 때, 일반적으로 파라미터 위치에 따라 순차적으로 파라미터가 넘겨지는데,
                C# 4.0부터는 위치와 상관없이 파라미터명을 지정하여 파라미터를 전달할 수 있게 하였다.
                이러한 파라미터를 Named Parameter라 부른다.

                    Method1(name: "John", age: 10, score: 90);
            */
            {
                Console.ReadLine();
            }
        }


        //Optional 파라미터: calcType
        int Calc(int a, int b, string calcType = "+")
        {
            switch (calcType)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    return a / b;
                default:
                    throw new ArithmeticException();
            }
        }

        static void optional_parameter()
        {
            /*
                C# 4.0에서부터 어떤 메서드의 파라미터가 디폴트 값을 갖고 있다면, 메서드 호출시 이러한 파라미터를 생략하는 것을 허용하였다.
                이렇게 디폴트 값을 가진 파라미터를 Optional 파라미터라 부른다.
                Optional 파라미터는 반드시 파라미터들 중 맨 마지막에 놓여져야 한다.
                복수개의 Optional 파라미터가 있는 경우 반드시 Optional 이 아닌 파라미터들 뒤에 위치해야 한다.
            */
            {
                Method m = new Method();
                int ret = m.Calc(1, 2);
                ret = m.Calc(1, 2, "*");

                Console.ReadLine();
            }

        }


        static void jagged_parameter()
        {
            /*
                일반적으로 메서드의 파라미터 갯수는 고정되어 있다.
                하지만 어떤 경우는 파라미터의 갯수를 미리 알 수 없는 경우도 있는데,
                이런 경우 C# 키워드 params를 사용한다.
                이 params 키워드는 가변적인 배열을 인수로 갖게 해주는데,
                파라미터들 중 반드시 하나만 존재해야 하며, 맨 마지막에 위치해야 한다.

                    //메서드
                    int Calc(params int[] values)

                    //사용
                    int s = Calc(1,2,3,4);
                    s = Calc(6,7,8,9,10,11);
            */
            {
                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //jagged_parameter();

            //optional_parameter();

            //named_parameter();

            //pass_by_reference();

            //pass_by_value();

            //method_what();
        }

    }
}
