using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulInterfaces
{
    public class CompareSortCopy
    {
        public class Player : IComparable<Player>
        {
            public string Name { get; set; }
            public int Score { get; set; }

            public int CompareTo(Player other)
            {
                if (other == null) return 1;
                // Score 내림차순 정렬
                return other.Score.CompareTo(this.Score);
            }
        }

        static void override_IComparableT()
        {
            /*
                IComparable<T>

                ✅ 목적
                  - 객체의 "정렬" 기준 정의 (예: List<T>.Sort()에서 사용)
                  - 숫자, 문자열, 날짜 등 비교 가능한 타입에 적용
            */
            {
                var players = new List<Player>
                {
                    new Player { Name = "A", Score = 10 },
                    new Player { Name = "B", Score = 25 },
                };
                players.Sort(); // Score 기준 정렬
            }
        }

        public class Point : IEquatable<Point>
        {
            public int X, Y;
            public bool Equals(Point other)
            {
                if (other == null) return false;
                return X == other.X && Y == other.Y;
            }

            // 오버라이드도 같이 작성
            public override bool Equals(object obj) => Equals(obj as Point);

            // .NET Core 2.1+에서는 HashCode.Combine(X, Y) 사용 가능
            public override int GetHashCode()
            {
                unchecked // 오버플로우 무시(해시 충돌 방지용이 아님, 단지 성능 + 예외방지)
                {
                    int hash = 17;
                    hash = hash * 31 + X;
                    hash = hash * 31 + Y;
                    return hash;
                }
            }
        }

        static void override_IEquatableT()
        {
            /*
                IEquatable<T>

                ✅ 목적
                  - "동등성(Equality)" 비교 최적화
                  - 해시테이블/딕셔너리/Set 등에서 활용
            */
            {
                var p1 = new Point { X = 3, Y = 5 };
                var p2 = new Point { X = 3, Y = 5 };
                var p3 = new Point { X = 1, Y = 2 };
                Point pNull = null;

                Console.WriteLine("p1.Equals(p2): " + p1.Equals(p2));                   // True
                Console.WriteLine("p1.Equals((object)p2): " + p1.Equals((object)p2));   // True
                Console.WriteLine("p1.GetHashCode() == p2.GetHashCode(): " + (p1.GetHashCode() == p2.GetHashCode())); // True

                Console.WriteLine("p1.Equals(p3): " + p1.Equals(p3));                   // False
                Console.WriteLine("p1.Equals((object)p3): " + p1.Equals((object)p3));   // False
                Console.WriteLine("p1.GetHashCode() == p3.GetHashCode(): " + (p1.GetHashCode() == p3.GetHashCode())); // False (거의 항상)

                Console.WriteLine("p1.Equals(null): " + p1.Equals(pNull));              // False
                Console.WriteLine("p1.Equals((object)null): " + p1.Equals((object)null)); // False

                string notAPoint = "NotAPoint";
                Console.WriteLine("p1.Equals(notAPoint): " + p1.Equals(notAPoint));     // False
            }
        }

        public class Person : ICloneable
        {
            public string Name;
            public object Clone()
            {
                // 얕은 복사 (MemberwiseClone)
                return this.MemberwiseClone();
            }
        }

        static void override_ICloneable()
        {
            /*
                ICloneable

                ✅ 목적
                  - 객체의 "깊은 복사/얕은 복사" 구현
                  - 값 복사가 필요한 구조에서 사용     
            */

            var person1 = new Person { Name = "Alice" };
            var person2 = (Person)person1.Clone();

            Console.WriteLine("person1.Name: " + person1.Name);     // Alice
            Console.WriteLine("person2.Name: " + person2.Name);     // Alice

            // 두 객체가 서로 다른 인스턴스인지 확인
            Console.WriteLine("ReferenceEquals(person1, person2): " + ReferenceEquals(person1, person2)); // False

            // 값 변경시 독립적으로 동작하는지 확인
            person2.Name = "Bob";
            Console.WriteLine("person1.Name (after change): " + person1.Name);   // Alice
            Console.WriteLine("person2.Name (after change): " + person2.Name);   // Bob

            // 얕은 복사임을 확인 (만약 참조형 필드가 있었다면 같은 인스턴스를 가리킴)
            // string은 불변이므로 크게 의미 없지만, 구조 확인용으로 표시
            Console.WriteLine("person1.Name == person2.Name: " + (person1.Name == person2.Name)); // False after change
        }

        public class ScoreComparer : System.Collections.Generic.IComparer<Player>
        {
            public int Compare(Player a, Player b)
            {
                return (b?.Score ?? 0) - (a?.Score ?? 0);
            }
        }

        static void override_IComparerT()
        {
            /*
                IComparer<T>

                ✅ 목적
                  - "정렬기준"을 외부에서 동적으로 지정할 때
                  - 여러 방식의 정렬 필요할 때
            */

            var players = new List<Player>
            {
                new Player { Name = "Alice", Score = 50 },
                new Player { Name = "Bob", Score = 80 },
                new Player { Name = "Charlie", Score = 65 }
            };

            // 정렬 전 출력
            Console.WriteLine("Before Sort:");
            foreach (var p in players)
                Console.WriteLine(p);

            // ScoreComparer를 사용한 내림차순 정렬
            players.Sort(new ScoreComparer());

            // 정렬 후 출력
            Console.WriteLine("\nAfter Sort (by Score Descending):");
            foreach (var p in players)
                Console.WriteLine(p);
        }

        public class Monster : ICloneable
        {
            public string Name;
            public int Age;

            public object Clone()
            {
                // 얕은 복사 (MemberwiseClone)
                return this.MemberwiseClone();
            }
        }

        // 이름만 비교하는 EqualityComparer
        public class MonsterNameComparer : IEqualityComparer<Monster>
        {
            public bool Equals(Monster x, Monster y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(Monster obj)
            {
                return obj.Name?.ToLowerInvariant().GetHashCode() ?? 0;
            }
        }

        static void override_IEqualityComparerT()
        {
            /*
                IEqualityComparer<T>

                ✅ 목적
                  - 컬렉션(특히 Dictionary, HashSet 등)에서
                    동등성(Equals)과 해시코드(GetHashCode)를 커스텀해서
                    비교 기준을 원하는 대로 정의할 때 쓰는 인터페이스      
            */

            var people = new HashSet<Monster>(new MonsterNameComparer())
            {
                new Monster { Name = "Alice", Age = 20 },
                new Monster { Name = "Bob", Age = 25 },
                new Monster { Name = "alice", Age = 30 } // 이름은 같지만 나이는 다름
            };

            Console.WriteLine("HashSet<Person> contents:");
            foreach (var p in people)
                Console.WriteLine(p);
            // "Alice(20)", "Bob(25)"만 출력됨

            // Dictionary에 적용도 가능
            var dict = new Dictionary<Monster, string>(new MonsterNameComparer())
            {
                [new Monster { Name = "Charlie", Age = 40 }] = "first",
                [new Monster { Name = "charlie", Age = 41 }] = "second" // 키 중복됨!
            };

            Console.WriteLine("\nDictionary<Monster, string> contents:");
            foreach (var kv in dict)
                Console.WriteLine($"{kv.Key} : {kv.Value}");
            // "charlie"로 한 번만 저장됨, value는 "second"

            /*
                출력:
                HashSet<Person> contents:
                Alice(20)
                Bob(25)

                Dictionary<Person, string> contents:
                Charlie(40) : second            
            */
        }

        public static void Test()
        {
            override_IEqualityComparerT();

            override_IComparerT();

            override_ICloneable();

            override_IEquatableT();

            override_IComparableT();
        }
    }
}
