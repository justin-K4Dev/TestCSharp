using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace AdvancedStep;



public class Switch
{

    static void Switch_what()
    {
        /*
            switch는 하나의 값 또는 조건을 기준으로 여러 분기 경로 중 하나를 선택할 수 있게 해주는
            조건 분기 제어문(Control Statement) 입니다.


            ✅ 종류
              | 종류                   | 설명                              | 도입 버전
              |------------------------|-----------------------------------|--------------
              | 전통적인 switch 문     | 값 기반 분기 (int, enum 등)       | ✅ C# 1.0
              | switch 표현식          | 함수형 스타일, 값을 반환          | ✅ C# 8.0
              | 패턴 기반 switch       | 타입, 속성, 관계 조건 분기        | ✅ C# 8~11
              | 논리 패턴, 관계 연산   | >=, and, or, not                  | ✅ C# 9~11
        */

        {
            var num = 1;

            string result = num switch
            {
                1 => "One",
                2 => "Two",
                _ => "Other"
            };

            Console.WriteLine(result); // 출력: One
        }
    }

    static void use_switch_Expression()
    {
        int day = 2;

        string name = day switch
        {
            1 => "Monday",
            2 => "Tuesday",
            _ => "Unknown"
        };

        Console.WriteLine(name); // 출력: Tuesday
    }

    class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    static void use_switch_with_PatternMatching()
    {
        // Property Pattern 매칭

        object obj = new Person("Alice", 20);

        string result = obj switch
        {
            Person { Age: < 18 } => "Minor",
            Person { Age: >= 18 } => "Adult",
            null => "Null",
            _ => "Unknown"
        };

        Console.WriteLine(result); // 출력: Adult
    }

    public record Monster(string Name, int Age);

    public record Point(int X, int Y);

    static void use_switch_with_advanced_PatternMatching()
    {
        // 🔹 관계 패턴 (Relational)
        {
            int score = 85;

            string result = score switch
            {
                >= 90 => "A",
                >= 80 => "B",
                < 60 => "F",
                _ => "Invalid"
            };

            Console.WriteLine(result); // 출력: B
        }

        // 🔹 논리 패턴 (Logical : and, or, not)
        {
            int number = 85;

            string result = number switch
            {
                < 0 => "Negative",
                0 => "Zero",
                > 0 and < 10 => "Small positive",
                >= 10 and <= 100 => "Medium",
                > 100 => "Large"
            };
            Console.WriteLine(result); // 출력: Medium

            DayOfWeek param = DayOfWeek.Monday;
            var day = param is DayOfWeek.Saturday or DayOfWeek.Sunday;
            Console.WriteLine(day); // 출력: false

            object my = null;
            var isNull = my is not null;
            Console.WriteLine(isNull); // 출력: true
        }

        // 🔹 Property Pattern + Relational Pattern + Logical Pattern 조건 필터
        {
            var monster = new Monster("master", 30);

            string result = monster switch
            {
                { Age: < 13 } => "Child",
                { Age: >= 13 and < 20 } => "Teenager",
                { Age: >= 20 and < 65 } => "Adult",
                { Age: >= 65 } => "Senior"
            };
            Console.WriteLine(result); // 출력: Adult

        }

        // 🔹 Deconstruction Pattern 조건 필터
        {
            var p = new Point(10, 30);

            // Tuple Deconstruction 연동 !!!
            string result = p switch
            {
                ( > 0, > 0) => "1st Quadrant",
                ( < 0, > 0) => "2nd Quadrant",
                ( < 0, < 0) => "3rd Quadrant",
                ( > 0, < 0) => "4th Quadrant",
                _ => "Origin or Axis"
            };
            Console.WriteLine(result); // 출력: 1st Quadrant
        }

        // 🔹 Tuple Pattern + Logical Pattern 조건 필터
        {
            var t = ("Admin", 30);

            // Tuple Deconstruction 연동 !!!
            var result = t switch
            {
                ("Admin", >= 5) => ("Admin", 1),
                ("Admin", < 5) => ("Admin", 2),
                ("User", >= 3) => ("User", 1),
                ("User", < 3) => ("User", 2),
                _ => ("Guest", 0)
            };
            Console.WriteLine(result); // 출력: (Admin, 1)
        }

        // 🔹 when 조건 필터
        {
            {
                int n = 100;
                string result = n switch
                {
                    int x when x % 2 == 0 => "Even",
                    int x when x % 2 != 0 => "Odd",
                    _ => "Unknown"
                };
                Console.WriteLine(result); // 출력: Even
            }

            {
                object obj = new Person("Alice", 22);

                string result = obj switch
                {
                    Person { Age: > 18 } p when p.Name.StartsWith("A") => $"성인 A씨: {p.Name}",
                    Person { Age: > 18 } p => $"성인: {p.Name}",
                    _ => "미성년자 또는 알 수 없음"
                };

                Console.WriteLine(result); // 출력: 성인 A씨: Alice    
            }
        }
    }

    public static void Test()
    {
        //use_switch_with_advanced_PatternMatching();

        //use_switch_with_PatternMatching();

        //use_switch_Expression();

        //Switch_what();
    }
}
