using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Condition
    {
        static void condition_if()
        {
            /*
                if 문은 조건식이 참, 거짓인지에 따라 서로 다른 블럭의 코드를 실행하게 한다.
                예를 들면, if (조건식) { 블럭1 } else { 블럭2 } 문장의 경우,
                조건식이 참이면 블럭1을 실행하고, 거짓이면 블럭2를 실행한다.

                아래 예제는 a값이 0 이상이면 val는 a 값을 그대로 갖고, 0보다 작으면 -a 값을 갖는 표현이다.
            */
            {
                int val;
                int a = -11;

                if (a >= 0)
                {
                    val = a;
                }
                else
                {
                    val = -a;
                }

                //출력값 : 11
                Console.Write(val);

                Console.ReadLine();
            }
        }


        static void condition_switch()
        {
            /*
                switch 문은 조건값이 여러 값들을 가질 경우 각 case 별 다른 문장들을 실행할 때 사용된다.
                각각의 경우에 해당하는 값을 case 문 뒤에 지정하며,
                어떤 경우에도 속하지 않는 경우는 default 문을 사용해 지정한다.
                각 case문 내에서 break 문을 사용하게 되면 해당 case 블럭의 문장들을 실행하고 switch 문을 빠져 나오게 된다.

                아래 예제에서 만약 category값이 딸기라면 price는 1100원이 된다.
            */
            {
                int price = 0;
                var category = "딸기";

                switch (category)
                {
                    case "사과":
                        price = 1000;
                        break;
                    case "딸기":
                        price = 1100;
                        break;
                    case "포도":
                        price = 900;
                        break;
                    default:
                        price = 0;
                        break;
                }

                Console.ReadLine();
            }
        }


        static void example()
        {
            /*
                다음 예제는 조건문을 사용하는 예제로서,
                콘솔로부터 파라미터 1개를 받아들여 각 옵션별로 해당 필드의 값을 설정하는 코드이다. 
            */
            {
                bool verbose = false;
                bool continueOnError = false;
                bool logging = false;

                string[] args = new string[] { "/c" };

                if (args.Length != 1)
                {
                    Console.WriteLine("Usage: MyApp.exe option");
                    return;
                }

                string option = args[0];
                switch (option.ToLower())
                {
                    case "/v":
                    case "/verbose":
                        verbose = true;
                        break;
                    case "/c":
                        continueOnError = true;
                        break;
                    case "/l":
                        logging = true;
                        break;
                    default:
                        Console.WriteLine("Unknown argument: {0}", option);
                        break;
                }

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //example();

            //condition_switch();

            //condition_if();
        }
    }
}
