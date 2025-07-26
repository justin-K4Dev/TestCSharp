using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;



public class TargetTypeNew
{
    public class Person
    {
        public string Name { get; }
        public int Age { get; }

        public Person(string name, int age)
            => (Name, Age) = (name, age);

        public override string ToString()
            => $"{Name}, {Age} years old";

        public static void PrintPerson(Person person)
        {
            Console.WriteLine(person);
        }
    }

    void PrintPerson(Person person)
    {
        Console.WriteLine(person);
    }

    static void TargetTypeNew_what()
    {
        /*
            변수를 선언할 때 명시된 타입을 컴파일러가 참조하여,
            new() 키워드 뒤에 타입명을 생략할 수 있는 기능입니다.
       */
        {
            Person p = new("Charlie", 40); // ← 여기서 'Person'을 생략함!
            Console.WriteLine(p);
        }

        Console.ReadLine();
    }

    static void use_TargetTypeNew()
    {

        // ✅ 1.Target - Typed new() 사용 예(C# 9 이상)

        //🔸 메서드 인자에서 사용
        {
            Person.PrintPerson(new("Alice", 30)); // OK! Person 타입으로 유추됨
        }

        // 🔸 속성 초기화에서 사용
        /*
            public class User
            {
                public Person Info { get; set; } = new("Bob", 40); // OK
            }
        */

        // 🔸 튜플과 함께 사용
        {
            var list = new List<(string Name, int Age)>
            {
                new("Alice", 30),
                new("Bob", 25)
            };
        }

        Console.ReadLine();
    }

    public static void Test()
    {
        //use_TargetTypeNew();

        //TargetTypeNew_what();
    }
}
