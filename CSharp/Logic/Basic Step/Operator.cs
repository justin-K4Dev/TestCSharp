using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace BasicStep
{
    public class Operator
    {
        static void operator_what()
        {
            /*
                C#은 다른 프로그래밍 언어와 비슷하게 수식 연산자, 논리 연산자, 조건 연산자등 다양한 연산자들을 제공하고 있다.
                아래 표는 각 카테코리별 연산자 및 그 샘플을 예시한 것이다. 

                OperatorType        Operator                        Example
                수식 연산자          +, -, *, /, %	                int a = (x + y - z) * (b / c) % d;
                할당 연산자	        =, +=, -=, *=, /=, %=           int a = 100;
                                                                    sum += a;
                                                                    [설명] sum += a 는 sum = sum + a 를 축약한 표현이다.
                증가/감소 연산자     ++, --	                        int i = 1;
                                                                    i++;
                                                                    [설명] i++ 는 i = i + 1 를 축약한 표현이다.
                논리 연산자	        && (And), || (Or), ! (Not)	    if ((a > 1 && b < 0) || c == 1 || !d)
                관계/비교 연산자	    <, >, ==, !=, >=, <=	        if (a <= b)
                비트 연산자	        & (AND), | (OR), ^ (XOR)	    byte a=7;
                                                                    byte b=(a & 3) | 4;
                                                                    [설명] 비트 연산에서 & 는 둘이 1인 경우만 1이 되고 (예: 1 & 1 = 1),
                                                                    | 는 둘 중에 하나라도 1인 경우 1이 되며,
                                                                    ^ 는 둘 중에 하나만 1 인 경우 1이 된다.
                Shift 연산자	        >>, <<	                        int i=2;
                                                                    i = i << 5;
                                                                    [설명] i의 값을 왼쪽으로 5 비트 이동한다.
                                                                    결과값은 2의 6승 즉 64가 된다.
                조건 연산자	        ?
                                    ?? (C# 3.0 이상만 지원)	        int val = (a > b) ? a : b;
                                                                    [설명] a가 b보다 크면 val에 a 값을 대입하고,
                                                                    같거나 작으면 b 값을 대입한다 
                                                                    string s = str ?? "(널)";
                                                                    [설명] 변수 str가 null 이면 "(널)" 이라는 문자열을 s 에 대입한다.
                                                                    null 이 아니면 str의 값을 s 에 대입.
            */
            {
                Console.ReadLine();
            }
        }


        static void null_coalescing_operator()
        {
            /*
                ?? 연산자는 Null-coalescing operator라고 불리우는 특별한 연산자로서 C# 3.0 이상에서 지원하는 연산자이다.
                ?? 연산자는 ?? 왼쪽 피연산자의 값이 NULL인 경우 ?? 뒤의 피연산자 값을 리턴하고,
                아니면 그냥 ?? 앞의 피연산자 값을 리턴한다.
                ?? 연산자는 왼쪽 피연산자가 NULL이 허용되는 데이타 타입인 경우에만 사용된다.
                예를 들어, int 타입은 NULL을 가질 수 없으므로 허용되지 않지만,
                Nullable<int> 즉 int? 타입은 허용된다.

                    int? i = null;
                    i = i ?? 0;

                    string s = null;
                    s = s ?? string.Empty;
            */
            {
                Console.ReadLine();
            }
        }


        static void bitwise_operator()
        {
            /*
                C# 비트 연산자(bitwise operator)는 AND (&), OR (|), XOR (^), NOT (~), Left Shift (<<), Right Shift (>>) 가 있으며,
                byte 간 비트 연산도 이러한 비트 연산자를 사용하는데,
                바이트 간 비트 연산 수행후 결과를 다시 byte 에 담기 위해서는 비트 연산 결과를 byte로 캐스팅 해 주어야 한다.
                (주: 비트 연산 후 결과를 int로 리턴하기 때문에, 이를 다시 byte에 담기 위해서는 (byte)로 캐스팅해야 한다)

                아래 예제는 2개의 바이트 간에 다섯가지 비트 연산을 수행한 예이다.
            */
            {
                byte x = 0xF1;
                byte y = 0x1F;

                byte a = (byte)(x & y);  // 0x11
                byte o = (byte)(x | y);  // 0xFF
                byte c = (byte)(x ^ y);  // 0xEE
                byte d = (byte)~x;      // 0x0E

                byte e = (byte)(x << 2); // 0xC4
                byte f = (byte)(y >> 2); // 0x07

                Console.ReadLine();
            }
        }


        static void PrintBits(BitArray ba)
        {
            for (int i = 0; i < ba.Count; i++)
            {
                Console.Write(ba[i] ? "1" : "0");
            }

            Console.WriteLine();
        }

        static void BitArray_use()
        {
            /*
                use BitArray

                C# 데이타 타입의 가장 작은 단위는 byte로서 한 바이트는 8 비트를 가지고 있다.
                바이트 내의 각 비트 단위로 처리하기 위해서는 일반적으로 Shift 와 비트 연산자를 사용하여 비트별 값들을 읽거나 쓰게 된다.
                이러한 불편함을 해소시키기 위해 .NET Framework에서 BitArray 클래스를 제공하고 있다.

                BitArray 클래스는 특정 비트를 읽거나 쓰기에 편리한 기능을 제공하고 있으며,
                개발자가 지정한 (임의의) 숫자의 비트들을 핸들링할 수 있도록 필요한 공간들을 내부에서 자동으로 처리해 준다.

                BitArray 생성자에서 비트수를 파라미터로 지정하면, 해당 비트수만큼의 비트들을 사용할 수 있는 객체를 생성해 준다.
                즉, new BitArray(8) 은 8비트를 갖는 객체를 생성한다. 비트인덱스는 첫번째 비트가 0, 두번째 비트가 1, ... 등과 같이 붙여진다.
                BitArray 생성자는 또한 bool[] 배열 혹은 byte[] 배열을 받아들여 비트들을 초기화할 수 있다. 

                비트를 읽을 때는 Get(비트인덱스) 메서드를 사용하고, 쓸 때는 Set(비트인덱스, true/false) 메서드를 사용한다.
                즉, 3번째 비트를 1로 하기 위해서는 Set(2, true) 와 같이 사용하며, 다시 3번째 비트를 읽을 때는 Get(2) 를 사용한다.
                BitArray 클래스는 또한 Indexer 를 지원하고 있는데, 이를 사용하면 Get과 Set 기능을 인덱서를 통해 쉽게 사용할 수 있다.

                그리고 BitArray는 비트 연산을 쉽게 하기 위해 And(), Or(), Xor(), Not() 같은 Bitwise 연산 메서드들을 제공하고 있다. 
            */
            {
                // 8개의 비트를 갖는 BitArray 객체 생성
                BitArray ba1 = new BitArray(8);

                // ## 비트 쓰기 ##
                ba1.Set(0, true);
                ba1.Set(1, true);

                PrintBits(ba1);  // 11000000

                BitArray ba2 = new BitArray(8);
                ba2[1] = true;
                ba2[2] = true;
                ba2[3] = true;
                PrintBits(ba2);  // 01110000

                // ## 비트 읽기 ##
                bool b1 = ba1.Get(0); // true
                bool b2 = ba2[4];     // false


                // ## BitArray 비트 연산 ## 
                // OR (ba1 | ba2) 결과를 ba1 에 
                ba1.Or(ba2);
                PrintBits(ba1);  // 11110000

                // AND (ba1 & ba2) 결과를 ba1 에 
                ba1.And(ba2);
                PrintBits(ba1);  // 01110000

                ba1.Xor(ba2);
                ba1.Not();

                // ## 기타 BitArray 생성 방법 ##
                // bool[] 로 생성
                var bools = new bool[] { true, true, false, false };
                BitArray ba3 = new BitArray(bools);

                // byte[] 로 생성
                var bytes = new byte[] { 0xFF, 0x11 };
                BitArray ba4 = new BitArray(bytes);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //BitArray_use();

            //bitwise_operator();

            //null_coalescing_operator();

            //operator_what();
        }
    }
}
