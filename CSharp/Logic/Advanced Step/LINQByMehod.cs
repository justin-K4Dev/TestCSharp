using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace AdvancedStep
{
    public class LINQByMethod
    {

        class Woman
        {
            public string name { get; set; }
            public int age { get; set; }
        }

        class Student
        {
            public string name { get; set; }
            public int[] score { get; set; }
        }

        class Person
        {
            public string sex { get; set; }
            public string name { get; set; }

            public int age { get; set; }
        }

        class Profile
        {
            public string name { get; set; }
            public int age { get; set; }
        }

        class Score
        {
            public string name { get; set; }
            public int math { get; set; }
            public int english { get; set; }
        }

        static void LINQ_from()
        {
            Woman[] womanList = {
                new Woman() { name = "julia", age = 24 },
                new Woman() { name = "yumi", age = 19 },
                new Woman() { name = "hani", age = 30 }
            };

            var woman = womanList
                .Where(woman_info => woman_info.age > 20)
                .OrderBy(woman_info => woman_info.age);

            Console.ReadLine();
        }


        // 조건 필터 메서드
        static bool passesFilter(Woman w, int? minAge, int? maxAge, string nameStartsWith)
        {
            // 기본 조건 AND 방식
            bool ageOk = (!minAge.HasValue || w.age >= minAge) &&
                         (!maxAge.HasValue || w.age <= maxAge);

            // 이름 조건 - 소문자 비교
            bool nameOk = string.IsNullOrEmpty(nameStartsWith) ||
                          w.name.StartsWith(nameStartsWith, StringComparison.OrdinalIgnoreCase);

            return ageOk && nameOk;
        }

        static void LINQ_where()
        {
            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "sue", age = 22 },
                    new Woman() { name = "nana", age = 26 }
                };

                var top3 = womanList.OrderByDescending(s => s.age).Take(3);

                var skip2 = womanList.Skip(2);

                var firstStudent = womanList.First();

                var lastStudent = womanList.Last();

                var third = womanList.ElementAt(2);

                Console.ReadLine();
            }

            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "sue", age = 22 },
                    new Woman() { name = "nana", age = 26 }
                };

                bool hasPerfect = womanList.Any(s => s.age == 100);   // false
                bool allPassed = womanList.All(s => s.age >= 30);     // true

                Console.ReadLine();
            }
            {
                // ToDictionary – 조건 검색 후 딕셔너리로 변환

                string[] fruits = { "apple", "banana", "cherry", "blueberry" };

                // 길이 6 이상인 과일을 이름으로 딕셔너리 만들기
                var fruitDict = fruits
                    .Where(f => f.Length >= 6)
                    .ToDictionary(f => f, f => f.Length);

                // 접근
                Console.WriteLine(fruitDict["banana"]);  // 6

                Console.ReadLine();
            }

            {
                // 중첩 컬렉션 평탄화 후 검색

                var students = new[]
                {
                    new { Name = "Alice", Scores = new[] { 80, 85, 90 } },
                    new { Name = "Bob", Scores = new[] { 60, 70 } }
                };

                // 모든 점수 중 80 이상만
                var highScores = students
                    .SelectMany(s => s.Scores)
                    .Where(score => score >= 80);

                Console.WriteLine(string.Join(", ", highScores)); // 80, 85, 90

                Console.ReadLine();
            }

            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "sue", age = 22 },
                    new Woman() { name = "nana", age = 26 }
                };

                // 나이가 19 이하 이름이 s로 시작하는 학생
                var filtered = womanList.Where(s => s.age >= 19 && s.name.StartsWith("s"));

                // 인덱스를 사용한 필터 (Where overload)
                var evenIndexItems = womanList.Where((value, index) => index % 2 == 0);

                // Any와 함께 사용하여 조건 만족 여부만 판단
                bool hasPerfectScore = womanList.Any(s => s.age == 30);

                var firstAge = womanList.FirstOrDefault(s => s.age > 22);
                if (firstAge != null)
                {
                    Console.WriteLine(firstAge.name);
                }

                Console.ReadLine();
            }

            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "sue", age = 22 },
                    new Woman() { name = "nana", age = 26 }
                };

                // 동적 조건 예시
                int? minAge = 20;
                int? maxAge = 28;
                string nameStartsWith = "j";

                var filtered = womanList
                    .Where(w => passesFilter(w, minAge, maxAge, nameStartsWith))
                    .OrderBy(w => w.age);

                foreach (var woman in filtered)
                {
                    Console.WriteLine($"{woman.name} - {woman.age}");
                }

                Console.ReadLine();
            }

            {
                var requiredChars = new[] { 'p', 'o', 's' };
                var words = new[] { "sod", "eocd", "qixm", "adio", "soo", "pose", "sop", "ops" };

                var result = words
                    .Where(word => requiredChars.All(c => word.Contains(c)))
                    .ToList();

                Console.WriteLine("Matched words:");
                foreach (var word in result)
                {
                    Console.WriteLine(word);
                }

                Console.ReadLine();
            }

            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "sue", age = 22 },
                    new Woman() { name = "nana", age = 26 }
                };

                int maxAge = womanList.Max(s => s.age);
                var highest = womanList.Where(s => s.age == maxAge);

                Console.ReadLine();
            }

            {
                // Distinct와 결합해서 중복 제거 후 검색

                int[] nums = { 1, 2, 2, 3, 4, 4, 5 };

                // 짝수 중 중복 없이
                var evenDistinct = nums.Where(n => n % 2 == 0).Distinct();

                Console.ReadLine();
            }

            {
                // 문자열 배열에서 필터링

                string[] fruits = { "apple", "banana", "blueberry", "cherry" };

                // "b"로 시작하는 문자열
                var startsWithB = fruits.Where(f => f.StartsWith("b"));

                // 길이가 6 이상인 문자열
                var longFruits = fruits.Where(f => f.Length >= 6);

                Console.ReadLine();
            }
        }

        static void LINQ_orderby()
        {
            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 }
                };

                var woman = womanList
                    .Where(w => w.age > 20)
                    .OrderBy(w => w.age); // 기본 오름차순 정렬

                Console.ReadLine();
            }

            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 }
                };

                var result = womanList
                    .Where(w => w.age > 20)
                    .OrderBy(w => w.age)                         // 오름차순 정렬
                    .ThenBy(w => w.name)                         // 이름 기준 추가 정렬
                    .Select(w => new                             // Select로 원하는 필드만 추출
                    {
                        Name = w.name,
                        Age = w.age,
                        IsAdult = w.age >= 20                    // 추가 계산 필드
                    });

                foreach (var woman in result)
                {
                    Console.WriteLine($"Name: {woman.Name}, Age: {woman.Age}, Adult: {woman.IsAdult}");
                }

                Console.ReadLine();
            }

            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 }
                };

                // OrderBy, Take 등과 함께 사용
                // 상위 3명 고득점자
                var top3 = womanList
                    .OrderByDescending(s => s.age)
                    .Take(3);
            }
        }


        static void LINQ_select()
        {
            {
                Woman[] womanList =
                {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "sujan", age = 32 }
                };

                var result = womanList
                    .Where(w => w.age > 20)
                    .OrderBy(w => w.age)
                    .Select(w => new
                    {
                        title = "adult",
                        name = w.name
                    });

                foreach (var v in result)
                {
                    Console.WriteLine("{0}: {1}", v.title, v.name);
                }

                Console.ReadLine();
            }
        }

        static void LINQ_multiple_data_range_search_set()
        {
            {
                Student[] studentList = {
                    new Student() { name = "justin", score = new int[] { 1, 2, 3, 4 } },
                    new Student() { name = "hani", score = new int[] { 11, 12, 13, 14 } },
                    new Student() { name = "jim", score = new int[] { 21, 22, 23, 24 } }
                };

                var Students = studentList
                    .SelectMany(student => student.score,
                                (student, score) => new { student.name, score })  // 두 컬렉션 병합
                    .Where(x => x.score > 89)
                    .Select(x => new { Name = x.name, Score = x.score });

                foreach (var student in Students)
                {
                    Console.WriteLine("Good: {0} , {1} score", student.Name, student.Score);
                }

                Console.ReadLine();
            }
        }

        static void LINQ_group_by()
        {
            {
                Person[] peopleList = {
                    new Person() { sex = "F", name = "jully" },
                    new Person() { sex = "M", name = "justin" },
                    new Person() { sex = "F", name = "kami" },
                    new Person() { sex = "M", name = "kang" }
                };

                var group = peopleList
                    .GroupBy(person => person.sex == "M")
                    .Select(g => new { sexCheck = g.Key, People = g });

                foreach (var g in group)
                {
                    if (g.sexCheck)
                    {
                        Console.WriteLine("Man List");
                    }
                    else
                    {
                        Console.WriteLine("Female List");
                    }

                    foreach (var person in g.People)
                    {
                        Console.WriteLine("Name: {0}", person.name);
                    }
                }

                Console.ReadLine();
            }

            {
                Person[] peopleList = {
                    new Person() { sex = "F", name = "jully", age = 24 },
                    new Person() { sex = "M", name = "justin", age = 30 },
                    new Person() { sex = "F", name = "kami", age = 28 },
                    new Person() { sex = "M", name = "kang", age = 22 }
                };

                var grouped = peopleList
                    .GroupBy(p => p.sex)
                    .Select(g => new
                    {
                        Sex = g.Key,
                        Count = g.Count(),
                        AverageAge = g.Average(p => p.age),
                        People = g.ToList()
                    })
                    .ToDictionary(x => x.Sex, x => new
                    {
                        x.Count,
                        x.AverageAge,
                        x.People
                    });

                foreach (var kvp in grouped)
                {
                    Console.WriteLine($"Sex: {kvp.Key}, Count: {kvp.Value.Count}, Average Age: {kvp.Value.AverageAge:F1}");
                    foreach (var person in kvp.Value.People)
                    {
                        Console.WriteLine($" - {person.name}, Age: {person.age}");
                    }
                    Console.WriteLine();
                }

                Console.ReadLine();
            }
        }

        static void LINQ_SelectMany()
        {
            {
                var students = new[]
                {
                    new { Name = "Alice", Scores = new[] { 80, 85, 90 } },
                    new { Name = "Bob", Scores = new[] { 60, 70 } }
                };

                // 모든 점수 중 80 이상만
                var highScores = students
                    .SelectMany(s => s.Scores)
                    .Where(score => score >= 80);

                Console.WriteLine(string.Join(", ", highScores)); // 80, 85, 90

                Console.ReadLine();
            }

            {
                var classes = new[]
                {
                    new { Name = "Class1", Students = new[] { "A", "B" } },
                    new { Name = "Class2", Students = new[] { "C", "D" } },
                };

                var allStudents = classes.SelectMany(c => c.Students);  // ["A", "B", "C", "D"]

                Console.ReadLine();
            }
        }

        static void LINQ_Aggregate()
        {
            {
                string[] words = { "C#", "is", "awesome" };

                // 문자열 연결 (첫 글자만 대문자)
                string sentence = words.Aggregate((a, b) => $"{a} {b}");
                Console.WriteLine(sentence);  // C# is awesome

                // 누적 합 계산 예
                int[] nums = { 1, 2, 3, 4 };
                int sum = nums.Aggregate((acc, n) => acc + n);
                Console.WriteLine(sum); // 10

                Console.ReadLine();
            }

            {
                string[] words = { "C#", "LINQ", "Rocks" };

                string result = words.Aggregate((a, b) => $"{a} {b}");
                Console.WriteLine(result);  // C# LINQ Rocks

                Console.ReadLine();
            }
        }

        static void LINQ_join()
        {
            {
                // 예시 클래스
                var students = new[]
                {
                    new { StudentId = 1, Name = "Alice" },
                    new { StudentId = 2, Name = "Bob" },
                    new { StudentId = 3, Name = "Charlie" }
                };

                var scores = new[]
                {
                    new { StudentId = 1, Score = 85 },
                    new { StudentId = 2, Score = 90 },
                    new { StudentId = 4, Score = 70 } // StudentId 4는 학생 리스트에 없음
                };

                var query = students.Join( scores                         // inner collection
                                         , student => student.StudentId   // outer key selector
                                         , score => score.StudentId       // inner key selector
                                         , (student, score) => new        // result selector
                                           {
                                                student.Name,
                                                score.Score
                                           }
                );

                foreach (var item in query)
                {
                    Console.WriteLine($"{item.Name} : {item.Score}");
                }

                Console.ReadLine();
            }
        }


        static void LINQ_inner_join()
        {
            {
                Profile[] profileList =
                {
                    new Profile() { name = "justin", age = 15 },
                    new Profile() { name = "kami", age = 20 },
                    new Profile() { name = "jully", age = 25 }
                };

                Score[] scoreList =
                {
                    new Score() { name = "justin", math = 88, english = 58 },
                    new Score() { name = "kami", math = 77, english = 77 },
                    new Score() { name = "kang", math = 66, english = 99 }
                };

                var students = profileList.Join( scoreList                        // inner collection
                                               , profile => profile.name          // outer key selector
                                               , score => score.name              // inner key selector
                                               , (profile, score) => new             // result selector
                                                 {
                                                    Name = profile.name,
                                                    Age = profile.age,
                                                    Math = score.math,
                                                    English = score.english
                                                 }
                );

                foreach (var student in students)
                {
                    Console.WriteLine(student);
                }

                Console.ReadLine();
            }
        }

        static void LINQ_outer_join()
        {
            {
                Profile[] profileList =
                {
                new Profile() { name = "justin", age = 15 },
                new Profile() { name = "kami", age = 20 },
                new Profile() { name = "jully", age = 25 }
            };

                Score[] scoreList =
                {
                new Score() { name = "justin", math = 88, english = 58 },
                new Score() { name = "kami", math = 77, english = 77 },
                new Score() { name = "kang", math = 66, english = 99 }
            };

                var students = profileList
                    .GroupJoin(
                        scoreList,                                 // inner collection
                        profile => profile.name,                   // outer key selector
                        score => score.name,                       // inner key selector
                        (profile, scores) => new { profile, scores }
                    )
                    .SelectMany(
                        ps => ps.scores.DefaultIfEmpty(new Score { math = 100, english = 100 }),
                        (ps, score) => new
                        {
                            Name = ps.profile.name,
                            Age = ps.profile.age,
                            Math = score.math,
                            English = score.english
                        }
                    );

                foreach (var student in students)
                {
                    Console.WriteLine(student);
                }

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //LINQ_outer_join();

            //LINQ_inner_join();

            //LINQ_join();

            //LINQ_Aggregate();

            //LINQ_SelectMany();

            //LINQ_group_by();

            //LINQ_multiple_data_range_search_set();

            //LINQ_select();

            //LINQ_orderby();

            //LINQ_where();

            //LINQ_from();
        }
    }
}
