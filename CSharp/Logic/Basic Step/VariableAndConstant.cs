using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace BasicStep
{
    public class VariableAndConstant
    {
        //필드 (전역 변수)
        int globalVar;
        const int MAX = 1024;

        public void TestMethod()
        {
            //로컬변수
            int localVar;

            //아래 할당이 없으면 에러 발생
            localVar = 100;

            Console.WriteLine(globalVar);
            Console.WriteLine(localVar);
        }

        static void variable_use()
        {
            /*
                C# 변수는 메서드 안에서 해당 메서드의 로컬변수로 선언되거나,
                혹은 클래스 안에서 클래스 내의 전역변수(이를 필드(Field) 라고 부른다)로 선언될 수 있다.
                로컬변수는 해당 메서드내에서만 사용되며, 메서드 호출이 끝나면 사용되지 못한다.
                반면 필드는 클래스 객체가 살아있는 한 계속 존속하며
                또한 다른 메서드들에서 참조할 수 있다
                (주: 만약 필드가 static 정적 필드이면 클래스 Type이 처음으로 런타임에 의해 로드될 때
                해당 Type object (타입 메타정보를 갖는 객체)에 생성되서 프로그램이 종료 때까지 유지된다).

                로컬변수는 기본값을 할당받지 못하기 때문에 반드시 사용 전에 값을 할당해야 하는 반면,
                필드는 값을 할당하지 않으면, 기본값이 자동으로 할당된다. 예를 들어, int 타입 필드인 경우 0 이 할당된다.

                모든 C# 변수는 Case-Sensitive 즉 대소문자를 구별한다. 즉, var1 과 Var1은 서로 다른 변수이다.

                * 필드 globalVar는 값을 명시적으로 할당하지 않은 경우 기본값 0이 할당된다.
                  여기서 전역(Global)의 의미는 객체 (혹은 클래스) 내에서의 전역을 의미한다.
                * 지역변수 localVar는 값을 할당하지 않고 사용하게 되면, 컴파일러 에러가 발생한다.
            */
            {
                Console.ReadLine();
            }
        }


        static void constant_use()
        {
            /*
                C# 상수는 C# 키워드 const를 사용하여 정의한다.
                C# 변수와 비슷하게 선언하는데, 다만 앞에 const를 붙여 상수임을 나타낸다.
                const는 필드 선언부에서 사용되거나 메서드 내에서 사용될 수 있으며, 컴파일시 상수값이 결정된다.

                (주: C# const 대신 readonly 키워드를 사용하여 읽기전용 (개념적으로 상수와 비슷한) 필드를 만들 수 있다.
                readonly는 필드 선언부나 클래스 생성자에서 그 값을 지정할 수 있고, 런타임시 값이 결정된다)

                    const int MAX_VALUE = 1024;
                    readonly int Max;
                    public Class1() {
                        Max = 1;
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void base_convert()
        {
            /*
                2진수, 10진수, 16진수 간의 변환은 흔히 2진수 문자열, 10진수 숫자, 16진수 문자열간의 변환을 말하는데,
                상호 변환은 모두 10진수 숫자를 기본으로 한다.
                즉, 2진수에서 16진수로의 변환을 위해 먼저 2진수문자열을 10진수로 변환한 다음 다시 16진수 문자열로 변환하게 된다.
                10진수로의 변환은 Convert.Int32(strVal, base) 함수를 사용하는데,
                첫번째 파라미터인 해당 문자열의 값이 두번째 파라미터인 base(진수)로 표현되어 있으니 이를 파싱해서 정수로 변환하라는 명령이다.
                10진수로부터 타 진수로의 변환은 Convert.ToString(intVal, base) 함수를 사용하는데,
                이는 첫번째 파라미터인 정수로부터 두번째 파라미터인 base 진수로 변환하여 문자열을 리턴하게 된다.
            */
            {
                // 2진수 문자열을 10진수 숫자로
                string strBase2 = "0000011011101010"; // 0x06EA
                int iBase10 = Convert.ToInt32(strBase2, 2);

                // 10진수 숫자를 16진수 문자열 (영문소문자)
                string strHex = Convert.ToString(iBase10, 16);

                // 10진수 숫자를 16진수 문자열 (영문대문자)  
                // (X4: Hexa 4자리, 영문은 대문자로)
                string strHex2 = iBase10.ToString("X4");

                // 위의 ToString과 동일한 표현
                string strHex3 = string.Format("{0:X4}", iBase10);

                // 16진수 문자열을 10진수로
                int iBase10_2 = Convert.ToInt32(strHex3, 16);

                // 10진수를 2진수 문자열로
                string strBase2_2 = Convert.ToString(iBase10, 2);

                Console.WriteLine(" 2진수: {0}", strBase2); // 0000011011101010
                Console.WriteLine("10진수: {0}", iBase10);  // 1770
                Console.WriteLine("16진수: {0}", strHex);   // 6ea
                Console.WriteLine("16진수: {0}", strHex2);  // 06EA
                Console.WriteLine("10진수: {0}", iBase10_2);  // 1770

                // Hex 문자열을 바이트로 
                string hexStr = "5A";
                int iVal = Convert.ToInt32(hexStr, 16);
                byte b = (byte)iVal;
                Console.WriteLine("{0:X}", b);

                string s = "9E";
                byte c = byte.Parse(s, NumberStyles.HexNumber);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //base_convert();

            //constant_use();

            //variable_use();
        }
    }
}
