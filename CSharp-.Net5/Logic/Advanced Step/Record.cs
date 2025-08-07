using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;


public class Record
{
    public record Person(string Name, int Age);

    static void Record_what()
    {
        /*
            record는 C# 9.0에서 도입된 참조 타입(클래스 기반)의 불변 데이터 모델(immutable data model)을 만들기 위한 새로운 타입입니다.
            값 기반 동등성(value equality), 자동 Deconstruct, 간결한 초기화, with 복사 등을 지원합니다.


            ✅ 핵심 목적
              - 불변성(immutability) 중심의 데이터 모델 표현
              - 값 기반 비교 (==, .Equals) 지원
              - 데이터 중심 구조 (DTO, 메시지, 상태 모델 등)에 최적화


            ✅ 주요 특징
              | 특징                        | 설명
              |-----------------------------|---------------------------------------------------------------------------
              | 값 기반 비교                | record는 속성 값이 같으면 동일로 간주 (Equals, == 자동 오버라이드)
              | 간결한 선언                 | 생성자 + 속성을 한 줄로 정의 가능
              | with 표현식 지원            | 기존 값을 복사해 일부만 수정
              | Deconstruct 자동 제공       | 분해 할당 가능 (var (x, y) = person)
              | ToString() 자동 오버라이드  | Person { Name = "Alice", Age = 30 }

            ✅ 종류
              | 선언 방식                         | 의미
              |-----------------------------------|---------------------------------------------------------------
              | record                            | 클래스 기반 참조 타입 (기본)
              | record class                      | 명시적 클래스 기반 (record와 동일)
              | record struct (C# 10+)            | 값 타입 구조체 버전
              | readonly record struct (C# 10+)   | 변경 불가능한 값 타입 구조체

            ✅ record vs class vs struct
              | 비교 항목         | record             | class                | struct
              |-------------------|--------------------|----------------------|--------------------------
              | 값 비교 (==)      | 값 기반            | ❌ 참조 비교        | ✅ 값 비교
              | 불변 구조         | 권장               | ❌ (별도 처리 필요) | ✅ 가능
              | with 복사         | 지원               | ❌                  | ✅ (C# 10+만 struct record)
              | 속성 초기화       | 생성자 or init     | ✅ 자유롭게         | ✅ 생성자 필수
              | 용도              | DTO, 메시지 객체   | 일반 객체            | 성능 중심 구조체

       */
        {
            var p1 = new Person("Alice", 30);
            var p2 = new Person("Alice", 30);

            Console.WriteLine(p1 == p2); // true ✅ 값 비교
            Console.WriteLine(p1);       // Person { Name = Alice, Age = 30 }
        }

        Console.ReadLine();
    }

    static void use_with_copy()
    {
        // ✅ with 복사 (Immutable Copy)

        var p1 = new Person("Alice", 30);

        var p3 = p1 with { Age = 31 };
        Console.WriteLine(p3); // Person { Name = Alice, Age = 31 }
    }

    public static void Test()
    {
        use_with_copy();

        Record_what();
    }
}



