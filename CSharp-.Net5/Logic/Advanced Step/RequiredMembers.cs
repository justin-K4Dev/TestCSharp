using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;



public class RequiredMember
{
    public class ServerConfig
    {
        public required int Port { get; set; }
        public string? Host { get; set; }
    }

    static void RequiredMembers_what()
    {
        /*
            required 키워드를 클래스의 속성 또는 필드 앞에 붙이면,
            객체 초기화 시 반드시 값을 설정해야 하는 필수 멤버로 지정할 수 있습니다.
            init과 함께 사용시 생성 시에만 값 설정 허용, 이후 불변 !!!

            🔹 도입 버전
              -  C# 11.0 이상
              - .NET 7.0 이상에서 사용 가능


            ❗ 문제점: required만 사용할 경우의 한계
              | 문제                                    | 설명
              |-----------------------------------------|---------------------------------------------------------------------------
              | 값은 필수지만, 변경은 언제든 가능       | set 접근자는 초기화 이후에도 값을 바꿀 수 있습니다. 즉, 불변성이 보장되지 않습니다.
              | 불변 객체와 궁합이 나쁨                 | 초기화 이후 값을 고정하려면 init이 필요하지만, set만 있으면 외부 수정 가능
              | 초기화 후 실수로 값 덮어쓰기 가능       | 생성 이후 의도치 않게 속성이 변경될 수 있는 여지가 있음
       */
        {
            //var config = new ServerConfig
            //{
            //    Host = "localhost"
            //};
            // ❌ 오류: required 멤버 'Port'가 설정되지 않았습니다.
        }
        {
            var config = new ServerConfig
            {
                Port = 8080,
                Host = "localhost"
            };
        }

        Console.ReadLine();
    }

    public static void Test()
    {
        //RequiredMembers_what();
    }
}
