using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class AnonymousType
    {
        static void anonymouse_type_what()
        {
            /*
                C#에서 어떤 클래스를 사용하기 위해서는 일반적으로 먼저 클래스를 정의한 후 사용한다.
                하지만 C# 3.0부터 클래스를 미리 정의하지 않고 사용할 수 있게하는 익명타입(Anonymous Type)을 지원하게 되었다.
                Anonymous Type은 new { ... } 와 같은 형식을 사용하며, new 블럭안에 속성=값 할당을 하고, 각 속성/값은 콤마로 분리한다.

                Anonymous Type은 읽기 전용이므로 속성값을 갱신할 수는 없다.
                C# 키워드 var는 컴파일러가 타입을 추론해서 찾아내도록 할 때 사용되는데,
                익명 타입 객체를 변수에 할당할 때는 특별히 타입명이 없으므로 var를 사용한다.

                    익명 타입 : new { 속성1=값, 속성2=값; }
                    ex) var t = new { Name="홍길동", Age=20 };
            */
            {
                Console.ReadLine();
            }
        }

        static void anonymouse_type_use()
        {
            /*
                Anonymous Type은 공식적으로 클래스를 정의할 필요 없이 Type을 간단히 임시로 만들어 사용할 때 유용하다.
                그리고 반드시 선언과 함께 new 키워드로 인스턴스를 생성 해줘야 하며,
                생성된 인스턴스는 읽기전용이기 때문에 값 변경이 불가능 하다.
                특히 Anonymous Type은 LINQ를 사용할 때 많이 사용된다.
                아래 예제는 익명타입의 객체 3개를 배열에 저장하고,
                LINQ문을 이용해 특정 조건의 데이타를 찾은 후, 만약 발견되었을 경우 데이타를 보여주는 예이다.
            */
            {
                // Anonymous Type 객체 3개를 Array에 담음
                var v = new[] {
                        new { Name="Lee", Age=33 },
                        new { Name="Kim", Age=25 },
                        new { Name="Park", Age=37 },
                    };

                // LINQ를 이용해 30세 이상 첫 객체를 찾음
                var under30 = v.FirstOrDefault(p => p.Age > 30);
                if (under30 != null)
                {
                    // Lee를 출력
                    Console.WriteLine(under30.Name);
                }

                Console.ReadLine();
            }
            {
                var temp = new { Age = 11, Name = "justin" };

                Console.WriteLine("Age:{0}, Name:{1}", temp.Age, temp.Name);

                var tempArr = new
                {
                    Int = new int[] { 11, 22, 33, 44, 55 },
                    Float = new float[] { 0.1f, 0.2f, 0.3f }
                };

                foreach (var element in tempArr.Int)
                {
                    Console.Write("{0} ", element);
                }
                Console.WriteLine();

                foreach (var element in tempArr.Float)
                {
                    Console.Write("{0} ", element);
                }
                Console.WriteLine();

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //anonymouse_type_use();

            //anonymouse_type_what();
        }
    }
}
