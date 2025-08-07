using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;



public class Tuple
{
    static (int id, string name) GetUser() => (1, "Alice");

    static void Tuple_what()
    {
        /*
            Tuple은 여러 개의 서로 다른 타입의 값을 하나의 구조로 묶어 표현하는 타입입니다.
            튜플은 값 타입처럼 동작하며, 간단한 반환값 그룹화, 비공식적인 DTO로 주로 사용됩니다.


            ✅ 종류별 구분
              | 종류                           | 설명                           | 도입 버전
              |--------------------------------|--------------------------------|----------
              | Tuple<T1, T2, ...> (클래스형)  | 참조 타입 튜플 (System.Tuple)  | C# 4.0
              | (T1, T2, ...) (값형)           | 값 타입 튜플 (ValueTuple)      | C# 7.0


            이름 붙은 튜플은 필드 이름을 통해 접근 가능
            Item1, Item2도 여전히 사용 가능
            
            🔧 ValueTuple 이란?
              - System.ValueTuple<T1, T2, ...>는 여러 값을 하나로 묶을 수 있는
              - 값 타입 튜플(struct)로, 성능이 뛰어나고 구조 분해(deconstruction)도 지원합니다.
       */

        //✅ 튜플 생성
        {
            var t = (42, "Hello", true);  // 타입: (int, string, bool), ValueTuple 생성
        }

        //✅ 튜플 반환
        {
            var (id, name) = GetUser(); // 분해 가능
        }

        //✅ 명명된 튜플
        {
            var point = (x: 10, y: 20);
            Console.WriteLine(point.x); // 출력: 10
        }

        //✅ 튜플 분해 (Deconstruction)
        {
            (int a, int b) = (100, 200);
            Console.WriteLine(a); // 100
            Console.WriteLine(b); // 200
        }
    }

    static void use_Tuple()
    {
        var person = (id: 1001, name: "Alice", age: 30);

        Console.WriteLine(person.name); // Alice

        var (id, name, age) = person;
    }


    public static void Test()
    {
        //use_Tuple();

        //Tuple_what();
    }
}
