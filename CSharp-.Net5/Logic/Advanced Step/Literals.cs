using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;

public class Literals
{

    static void RawStringLiterals_what()
    {
        /*
            Raw String Literal은 C# 11에서 도입된 문자열 리터럴 문법으로,
            이스케이프 없이 줄바꿈, 따옴표, 들여쓰기 등을 그대로 작성할 수 있는 간결한 다중 줄 문자열 표현입니다.


            🔹 도입 버전
              -  C# 11.0 이상
              -  .NET 7.0 이상에서 사용 가능


            ✅ 기본 문법

                string json = """
                {
                  "name": "ChatGPT",
                  "type": "AI"
                }
                """;

                - 삼중 따옴표 """로 시작하고 종료
                - 문자열 내용은 있는 그대로 표현됨
                - 이스케이프 없이 줄바꿈, 들여쓰기, 따옴표 사용 가능


            🔸 따옴표 중첩이 필요한 경우 ("""", """"" 등)

                string quote = """""
                He said, "Hello, world!"
                """""";

                - 문자열 내부에 " 따옴표가 포함될 경우, 외부 따옴표 수를 내부 최대 연속 따옴표 수 + 1 이상으로 설정


            🔸 보간된 문자열 ($"""...""")

                var name = "justin, Kang";
                string msg = $"""
                {
                  "name": "{name}"
                }
                """;

                - $""" 문법으로 문자열 보간 (interpolation) 가능
                - 변수, 표현식 등 삽입 가능


            💡 특징 정리
              | 기능               | 지원 여부   | 설명
              |--------------------|-------------|-----------------------------------------------------
              | 줄바꿈 포함        | ✅         | 자동 줄바꿈 포함
              | 이스케이프 없음    | ✅         | \n, \" 등 필요 없음
              | 들여쓰기 유지      | ✅         | 들여쓰기된 텍스트 보존
              | 따옴표 중첩 허용   | ✅         | 따옴표 개수 늘려 충돌 방지
              | 문자열 보간        | ✅         | $""" ... {var} ... """ 지원
       */
        {
            string json = """
            {
              "name": "justin, Kang",
              "active": true
            }
            """;

            var userName = "goodMan";
            string query = $"""
            SELECT * FROM users WHERE name = "{userName}"
            """;
        }
    }

    // int를 32비트 2진수로 보기 좋게 출력
    private static void PrintInt(string name, int value)
    {
        string bin = Convert.ToString(value, 2).PadLeft(32, value < 0 ? '1' : '0');
        bin = GroupBits(bin);
        Console.WriteLine($"{name} = {value,6}  (0b_{bin})");
    }

    // byte를 8비트 2진수로 출력
    private static void PrintByte(string name, byte value)
    {
        string bin = Convert.ToString(value, 2).PadLeft(8, '0');
        bin = GroupBits(bin);
        Console.WriteLine($"{name} = {value,6}  (0b_{bin})");
    }

    // "1111000011110000..." → "1111_0000_1111_0000_..." 로 4비트씩 구분
    private static string GroupBits(string bin)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < bin.Length; i++)
        {
            if (i > 0 && i % 4 == 0) sb.Append('_');
            sb.Append(bin[i]);
        }
        return sb.ToString();
    }

    static void BinaryLiterals_what()
    {

        /*
            2진 리터럴(0b_...)

            🔹 도입 버전
              - 2진 리터럴(0b, 숫자 중간 _) : C# 7.0+

            🔹 핵심 요약
              - 0b_XXXX 형태로 2진 리터럴 작성
        */

        Console.WriteLine("=== 1. 10진 / 16진 / 2진 리터럴 값 비교 ===");

        int dec = 60;                     // 10진수
        int hex = 0x3C;                   // 16진수
        int bin = 0b_0011_1100;           // 2진 리터럴

        PrintInt("dec (60)  ", dec);
        PrintInt("hex (0x3C)", hex);
        PrintInt("bin (0b..) ", bin);
        Console.WriteLine("=> 세 값은 모두 동일한 비트 패턴입니다.\n");

        Console.WriteLine("=== 2. 2진 리터럴로 플래그(Flags) 정의 ===");
        const int FLAG_MOVE = 0b_0001;   // 1
        const int FLAG_JUMP = 0b_0010;   // 2
        const int FLAG_ATTACK = 0b_0100;   // 4
        const int FLAG_HIDE = 0b_1000;   // 8

        int state = 0;
        // 이동 + 공격 상태 켜기
        state |= FLAG_MOVE;
        state |= FLAG_ATTACK;

        PrintInt("state", state);
        Console.WriteLine($"Has MOVE?   {((state & FLAG_MOVE) != 0 ? "YES" : "NO")}");
        Console.WriteLine($"Has JUMP?   {((state & FLAG_JUMP) != 0 ? "YES" : "NO")}");
        Console.WriteLine($"Has ATTACK? {((state & FLAG_ATTACK) != 0 ? "YES" : "NO")}");
        Console.WriteLine($"Has HIDE?   {((state & FLAG_HIDE) != 0 ? "YES" : "NO")}");
        Console.WriteLine();

        Console.WriteLine("=== 3. 2진 리터럴과 시프트 연산 ===");
        int a = 0b_0001_0000;  // 16
        PrintInt("a          ", a);
        PrintInt("a << 1     ", a << 1);  // 32
        PrintInt("a << 2     ", a << 2);  // 64
        PrintInt("a >> 1     ", a >> 1);  // 8
        Console.WriteLine();

        Console.WriteLine("=== 4. 2진 리터럴로 상위/하위 비트 마스킹 ===");
        byte value = 0b_1011_0101;                    // 0xB5 = 181
        byte high4 = (byte)(value & 0b_1111_0000);    // 상위 4비트만
        byte low4 = (byte)(value & 0b_0000_1111);    // 하위 4비트만

        PrintByte("value     ", value);
        PrintByte("high4     ", high4);
        PrintByte("low4      ", low4);
        Console.WriteLine("// high4 는 상위 4비트, low4 는 하위 4비트만 남습니다.\n");
    }


    public static void Test()
    {
        //BinaryLiterals_what();

        //RawStringLiterals_what();
    }
}
