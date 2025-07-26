using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class NameOf
    {
        class MyPerson
        {
            public float Height { get; set; }
        }

        static void nameof_what()
        {
            /*
                C# 6.0의 nameof 연산자는 Type이나 메서드, 속성 등의 이름을 리턴하는 것으로
                이러한 명칭들을 하드코딩하지 않게 하는 잇점이 있다.
                즉, 이는 하드코딩에 의한 타이핑 오류 방지나 혹은 차후 리팩토링에서 유연한 구조를 만들어 준다는 잇점이 있다.
                예를 들어, 아래 예제와 같이 ArgumentException을 발생시킬 때,
                파라미터명을 직접 하드코딩하지 않고 nameof()를 사용하면,
                만약 리팩토링을 통해 id가 identity로 변경하더라도 아무런 문제가 없게 된다.
            */
            {
				// 1. 파마미터명 id (Hard coding 하지 않음)
				try
				{
					Int64 id = 1000;
					throw new ArgumentException("Invalid argument", nameof(id));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}

				var my_person = new MyPerson();

				// 2. 속성명을 nameof 로 추출
				Console.WriteLine("{0} : {1}", nameof(my_person.Height), my_person.Height);

				// 3. 메서드명 로깅에 추가
				Console.WriteLine($"{nameof(nameof_what)} : Started");


				Console.ReadLine();
            }
        }


        public static void Test()
        {
            //nameof_what();
        }
    }
}
