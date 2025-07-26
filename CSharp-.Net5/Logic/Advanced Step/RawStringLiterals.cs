using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;

public class RawStringLiterals
{

    static void RawStringLiterals_what()
    {
        /*
            Raw String Literal은 C# 11에서 도입된 문자열 리터럴 문법으로,
            이스케이프 없이 줄바꿈, 따옴표, 들여쓰기 등을 그대로 작성할 수 있는 간결한 다중 줄 문자열 표현입니다.


            🔹 도입 버전
            ✅ C# 11.0 이상
            🔧 .NET 7.0 이상에서 사용 가능


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
                기능	               지원 여부	   설명
                줄바꿈 포함	           ✅	           자동 줄바꿈 포함
                이스케이프 없음	       ✅	           \n, \" 등 필요 없음
                들여쓰기 유지	       ✅	           들여쓰기된 텍스트 보존
                따옴표 중첩 허용	   ✅	           따옴표 개수 늘려 충돌 방지
                문자열 보간	           ✅	           $""" ... {var} ... """ 지원
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

    public static void Test()
    {
        //RawStringLiterals_what();
    }
}
