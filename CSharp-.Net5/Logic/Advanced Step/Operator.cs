using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;



public class Operator
{
    // int 값을 32비트 2진수로 출력
    static void PrintInt(string name, int value)
    {
        string bin = Convert.ToString(value, 2).PadLeft(32, value < 0 ? '1' : '0');
        Console.WriteLine($"{name} = {value,12} (0b{bin})");
    }

    // uint 값을 32비트 2진수로 출력
    static void PrintUInt(string name, uint value)
    {
        string bin = Convert.ToString((int)value, 2).PadLeft(32, '0');
        Console.WriteLine($"{name} = {value,12} (0b{bin})");
    }

    static void use_UnsignedRightShiftOperator()
    {
        /*
            >>> 연산자 (Unsigned Right Shift Operator)

            비트를 오른쪽으로 밀면서, 왼쪽 빈 자리를 항상 0으로 채우는 연산자
        */
        {
            // 음수 값 준비 (-8)
            int neg = -8;

            Console.WriteLine("=== int (음수)에서 >> vs >>> 차이 ===");
            PrintInt("neg        ", neg);
            PrintInt("neg >> 1   ", neg >> 1);   // 산술 우측 시프트 (부호 비트 유지)
            PrintInt("neg >>> 1  ", neg >>> 1);  // 논리 우측 시프트 (왼쪽을 0으로 채움)
            Console.WriteLine();
            // neg = 11111111 11111111 11111111 11111000 (-8)
            // neg >> 1 = 11111111 11111111 11111111 11111100 (-4)  // >> : 부호비트 유지
            // neg >>> 1 = 01111111 11111111 11111111 11111100 (큰 양수) // >>> : 0 채움

            // 양수 int 에서는 >>, >>> 가 사실상 동일하게 보임
            int pos = 64; // 0b0100_0000
            Console.WriteLine("=== int (양수)에서는 >>, >>> 가 동일하게 보임 ===");
            PrintInt("pos        ", pos);
            PrintInt("pos >> 1   ", pos >> 1);
            PrintInt("pos >>> 1  ", pos >>> 1);
            Console.WriteLine();
            // pos = 00000000 00000000 00000000 01000000 (64)
            // pos >> 1 = 00000000 00000000 00000000 00100000 (32)
            // pos >>> 1 = 00000000 00000000 00000000 00100000 (32)


            // uint 에서는 원래부터 >> 가 논리 시프트이긴 하지만,
            // >>> 를 쓰면 "논리 시프트 의도"를 더 명확히 보여줄 수 있음
            uint u = 0b_1000_0000_0000_0000_0000_0000_0000_0000u;
            Console.WriteLine("=== uint 에서 >>> 사용 예시 ===");
            PrintUInt("u          ", u);
            PrintUInt("u >> 1     ", u >> 1);
            PrintUInt("u >>> 1    ", u >>> 1); // 의미는 동일하지만 의도 표현용
            Console.WriteLine();
            // u        = 10000000 00000000 00000000 00000000 (2147483648)
            // u >> 1   = 01000000 00000000 00000000 00000000 (1073741824)
            // u >>> 1  = 01000000 00000000 00000000 00000000 (1073741824)

            // >>>= 복합 대입 연산자 예제
            Console.WriteLine("=== >>>= 복합 대입 예시 ===");
            int mask = -1; // 0xFFFF_FFFF
            PrintInt("mask       ", mask);
            mask >>>= 4;   // 오른쪽으로 4비트 논리 시프트 (왼쪽에 0 채움)
            PrintInt("mask >>>=4 ", mask);
            // mask 초기값  = 11111111 11111111 11111111 11111111 (-1)
            // mask >>>= 4  = 00001111 11111111 11111111 11111111 (268435455)

            Console.ReadLine();
        }
    }

    public static void Test()
    {
        //use_UnsignedRightShiftOperator();
    }
}

