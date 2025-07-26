using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;



public class StaticLocalFunction
{
    static void StaticLocalFunction_what()
    {
        /*
            Static Local Function은 static으로 선언된 지역 함수이며,
            외부 지역 변수, 인스턴스 필드, this 등의 캡처를 금지합니다.

            🔹 도입 버전
            ✅ C# 8.0
            🔧 .NET Core 3.0 / .NET Standard 2.1 이상에서 사용 가능aaaaaaaaaaaaaaaaaaaaaaaa


            🔧 장점 요약
            ✅ 캡처 금지 → 클로저 할당/생성 없음
            ✅ 더 명확하고 안전한 함수 작성 가능
            ✅ 성능 민감한 코드에서 유리
       */
        {
            //int value = 10;

            //static int Multiply(int x) => x * value; // ❌ 컴파일 에러: value 캡처 불가
        }
        {
            static int Multiply(int x, int y) => x * y;

            Console.WriteLine(Multiply(3, 4)); // ✅ 출력: 12
        }
    }

    public static void Test()
    {
        //StaticLocalFunction_what();
    }
}
