using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;



public class InitOnlySetter
{
    public class Config
    {
        public string Url { get; init; } // 초기화 시 1회만 가능 → 이후 불변
    }

    static void InitOnlySetter_what()
    {
        /*
            init 키워드는 초기화는 가능하지만, 객체 생성 이후에는 변경 불가
            즉, set처럼 외부에서 값을 할당할 수는 있지만 생성 시에만 1회 허용
            불변 객체(Immutable Object)를 간편하게 만들기 위해 추가 !!!
            required 와 함께 사용해야 실용적 !!!

            🔹 도입 버전
            ✅ C# 9.0 이상
            🔧 .NET 5.0 이상에서 사용 가능
       */
        {
            var config = new Config { Url = "https://api" }; // OK

            //config.Url = "https://hack"; // ❌ 컴파일 오류
        }
    }

    public class AppSettings
    {
        public required string ApiKey { get; init; } // ✅ 반드시 설정 + 생성 후 불변
        public string? OptionalComment { get; init; } // ❓ 있어도 되고 없어도 되는 경우
    }

    static void init_with_required()
    {
        {
            //var config = new AppSettings
            //{
            //    OptionalComment = "localhost"
            //};
            // ❌ 오류: required 멤버 'ApiKey'가 설정되지 않았습니다.
        }
        {
            var config = new AppSettings
            {
                ApiKey = "myKey",
                OptionalComment = "localhost"
            };
        }
    }

    public static void Test()
    {
        //init_with_required();

        //InitOnlySetter_what();
    }
}
