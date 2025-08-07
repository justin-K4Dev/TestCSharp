using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace UsefulInterfaces
{
    public class LINQ
    {
        public class User
        {
            public string Name;
            public string Country;
            public override string ToString() => $"{Name}({Country})";
        }

        static void override_IGroupingTT()
        {
            /*
                IGrouping<TKey, TElement>

                ✅ 목적
                  - LINQ의 GroupBy 연산 결과에서 각 그룹(키와 요소 집합)을 표현하는 표준 인터페이스
                  - 각 그룹은 Key: 그룹의 기준 값(예: "국가", "성별" 등), IEnumerable<TElement>: 해당 그룹에 속하는 모든 요소 컬렉션
                  - 주로 LINQ 쿼리에서 자동 생성된 Grouping을 사용
            */

            var people = new List<User>
            {
                new User { Name = "Alice", Country = "Korea" },
                new User { Name = "Bob", Country = "USA" },
                new User { Name = "Charlie", Country = "Korea" },
                new User { Name = "David", Country = "USA" },
            };

            // GroupBy 사용
            IEnumerable<IGrouping<string, User>> groups = people.GroupBy(p => p.Country);

            foreach (var group in groups)
            {
                Console.WriteLine($"Country: {group.Key}");
                foreach (var person in group)
                    Console.WriteLine("  " + person.Name);
            }

            /*
                Country: Korea
                  Alice
                  Charlie
                Country: USA
                  Bob
                  David
            */
        }

        public static void Test()
        {
            override_IGroupingTT();
        }
    }
}
