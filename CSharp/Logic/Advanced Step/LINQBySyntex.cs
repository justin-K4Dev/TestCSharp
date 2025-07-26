using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace AdvancedStep
{
    public class LINQBySyntex
    {
        class Woman
        {
            public string name { get; set; }
            public int age { get; set; }
        }

        class Student
        {
            public string name { get; set; }
            public int [] score { get; set; }
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


        static void LINQ_what()
        {
            /*
                LINQ 는 Language Integrated Query 의 약어로 직역하면 '질의로 통합된 언어' 이다.
                C# 에서는 데이터에 대해 질문하는 LINQ 라는 기능을 제공한다.
                LINQ 의 역할은 데이터에 대해 질문하고, 그 답에 해당하는 데이터를 찾는 것이다.
                이러한 LINQ의 질의 기능은 프로그램에서 데이터 검색을 편리 하게 해준다.
            */
            {
                Console.ReadLine();
            }
        }


        static void LINQ_from()
        {
            /*
                from 절은 데이터를 검색할 범위를 지정해 준다.
                LINQ Expression 은 반드시 from 절로 시작해야 한다.

                # foreach 문과 from 절의 element 변수의 차이점
                    foreach의 element 변수에는 실제로 array의 데이터가 저장된다.
                    하지만 LINQ에서는 element 변수에 데이터가 저장되지 않고,
                    단순히 'array에 존재하는 요소' 라는 의미로만 사용된다.

                from 에서 사용가능한 타입은 IEnumerable<T> 인터페이스를 상속 하는 타입이다.
                C# 에서의 배열이나 콜렉션등은 모두 IEnumerable<T>을 상속하기 때문에
                배열이나 콜렉션 타입이라면 모두 사용 가능 하다.
            */
            {
                Woman[] womanList = {
                            new Woman() { name = "julia", age = 24 }
                        ,   new Woman() { name = "yumi", age = 19 }
                        ,   new Woman() { name = "hani", age = 30 }
                    };

                var woman = from    woman_info in womanList
                            where   woman_info.age > 20
                            orderby woman_info.age
                            select  woman_info;

                Console.ReadLine();
            }
        }


        static void LINQ_where()
        {
            /*
                where 절은 범위 내에서 데이터를 걸러내는 필터 역할을 한다.
                따라서 where 절 에서는 데이터를 걸러내기 위한 필터 조건을 작성해 주면 된다.
            */
            {
                Woman[] womanList = {
                    new Woman() { name = "julia", age = 24 },
                    new Woman() { name = "yumi", age = 19 },
                    new Woman() { name = "hani", age = 30 },
                    new Woman() { name = "nana", age = 27 },
                    new Woman() { name = "jane", age = 24 }
                };

                // 동적 필터 조건
                int? minAge = 20;
                int? maxAge = 30;
                string nameStartsWith = "j";

                var result =
                from woman in womanList
                where
                    (!minAge.HasValue || woman.age >= minAge) &&
                    (!maxAge.HasValue || woman.age <= maxAge) &&
                    (string.IsNullOrWhiteSpace(nameStartsWith) ||
                    woman.name.StartsWith(nameStartsWith, StringComparison.OrdinalIgnoreCase))
                orderby woman.age, woman.name
                select new
                {
                    woman.name,
                    woman.age,
                    isAdult = woman.age >= 20
                };

                foreach (var w in result)
                {
                    Console.WriteLine($"Name: {w.name}, Age: {w.age}, Adult: {w.isAdult}");
                }

                Console.ReadLine();
            }
        }


        static void LINQ_orderby()
        {
            /*
                orderby 절은 걸러진 데이터를 정렬해준다.
                기본값으로 오름차순 정렬을 사용하기 때문에
                정렬 키워드 없이 정렬 기준만 제시하면, 그 기준에 따라 오름차순으로 정렬해 준다.
            */
            {
                Woman[] womanList = { 
                        new Woman() { name = "julia", age = 24 }
                    ,   new Woman() { name = "yumi", age = 19 }
                    ,   new Woman() { name = "hani", age = 30 }
                };

                var woman = from    woman_info in womanList
                            where   woman_info.age > 20
                            orderby woman_info.age  //기본 오름차순 정렬 (default asceding order sort)
                            //orderby woman.age ascending <- 오름차순, 코드 가독성을 위해 명시 권장
                            //orderby woman.age descending <- 내림차순
                            select  woman_info;

                Console.ReadLine();
            }
        }


        static void LINQ_select()
        {
            /*
                최종적으로 검색된 데이터를 추출하는 역할을 한다.
                추출된 데이터 타입은 select 절에 지정한 변수의 타입으로 결정되며,
                Anonymous Type 으로 만들어서 추출 할 수도 있다.

                    select woman;       //Woman 형 배열 데이터 추출
                    select woman.name;  //String 형 name 배열 데이터 추출
                    select new { title = "adult", name = woman.name };
            */
            {
                Woman[] womanList =
                {
                        new Woman() { name = "julia", age = 24 }
                    ,   new Woman() { name = "yumi", age = 19 }
                    ,   new Woman() { name = "hani", age = 30 }
                    ,   new Woman() { name = "sujan", age = 32 }
                };

                var Woman = from woman in womanList
                            where woman.age > 20
                            orderby woman.age ascending
                            select new
                            {
                                title = "adult",
                                name = woman.name
                            };

                foreach (var v in Woman)
                {
                    Console.WriteLine("{0}: {1}", v.title, v.name);
                }

                Console.ReadLine();
            }
        }


        static void LINQ_multiple_data_range_search_set()
        {
            /*
                from 절을 통해 지정한 데이터의 범위내에서
                한번 더 데이터 범위를 지정할 경우
                중첩 from 절을 사용 한다.

                    Student[] studentList = {
                        new Student() { name = "justin", score = new int[] { 1, 2, 3, 4 } },
                        new Student() { name = "hani", score = new int[] { 11, 12, 13, 14 } },
                        new Student() { name = "jim", score = new int[] { 21, 22, 23, 24 } }
                    };

                    var Students =  from student in studentList
                                    from score in student.score
                                    where score > 89
                                    select new { Name = student.name, Score = score };

                첫번째 from 에서 studentList 로 범위를 지정하고,
                두번째 from 에서 studentList 내의 점수(score) 를 데이터 범위를 지정 하였다.

                ( from 에서 범위를 지정하는 대상은 배열이나 컬렉션이어야 한다!!! )
            */
            {
                Student[] studentList = {
                        new Student() { name = "justin", score = new int[] { 1, 2, 3, 4 } }
                    ,   new Student() { name = "hani", score = new int[] { 11, 12, 13, 14 } }
                    ,   new Student() { name = "jim", score = new int[] { 21, 22, 23, 24 } }
                };
                
                var Students = from     student in studentList
                               from     score in student.score
                               where    score > 89
                               select   new { Name = student.name, Score = score };

                foreach (var student in Students)
                {
                    Console.WriteLine("Good: {0} , {1} score", student.Name, student.Score);
                }

                Console.ReadLine();
            }
        }


        static void LINQ_group_by()
        {
            /*
                많은 데이터가 중구난방으로 섞여있는 데이터 배열이 있다.
                그리고 나는 이 배열을 특정 기준에 따라 두 그룹으로 나누고 싶다!!
                이럴 때에 사용하는 키워드가 group ~by ~into 이다.

                    group A by B into C : A 를 B 기준에 따라 분류 하여 C 로 저장한다.
                                          (C 안에는 기준에 따라 두 개의 그룹으로 나눠서 저장된다.)

                    Person [] peopleList = {
                        new Person() { sex = "F", name = "jully" },
                        new Person() { sex = "M", name = "justin" },
                        new Person() { sex = "F", name = "kami" },
                        new Person() { sex = "M", name = "kang" }
                    };

                    var Group = from person in peopleList
                                group person by person.sex == "M" into data
                                select new { sexCheck = data.Key, People = data };

                peopleList 에서 person 데이터를 추출해서 남자면 남자 그룹에,
                여자면 여자 그룹에 저장한다.

                그리고 group.Key 에는 남자 그룹인 경우 true 값이,
                여자 그룹인 경우 false 값이 저장된다.

                    peopleList
                    ( sex = F , name = jully | sex = M , name = justin |
                      sex = F , name = kami  | sex = M , name = kang )

                    group info
                    ( sexCheck = false , People ( F , jully ) |
                                         People ( F , kami ),
                      sexCheck = true  , People ( M , justin ) |
                                         People ( M , kang ) )
            */
            {
                Person[] peopleList = {
                        new Person() { sex = "F", name = "jully" }
                    ,   new Person() { sex = "M", name = "justin" }
                    ,   new Person() { sex = "F", name = "kami" }
                    ,   new Person() { sex = "M", name = "kang" }
                };

                var Group = from    person in peopleList
                            group   person by person.sex == "M" into data
                            select  new { sexCheck = data.Key, People = data };

                foreach (var group in Group)
                {
                    if (group.sexCheck)
                    {
                        Console.WriteLine("Man List");
                        foreach (var person in group.People)
                        {
                            Console.WriteLine("Name: {0}", person.name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Female List");
                        foreach (var person in group.People)
                        {
                            Console.WriteLine("Name: {0}", person.name);
                        }
                    }
                }

                Console.ReadLine();
            }
        }


        static void LINQ_join()
        {
            /*
                서로 다른 두 개의 데이터가 있는데, 두 개의 데이터가 서로 통합될 수 있는 유사성을 가진다면
                굳이 따로따로 처리하기 보다는 하나의 데이터로 통합하여 처리하는 것이 훨씬 효율적이겠다.

                LINQ 는 이를 위해 데이터를 통합하는 기능을 제공하는데,
                그 기능을 하는 키워드가 join 이다.

                join 은 LINQ 쿼리식에서 서로 다른 두 데이터를 합치는 기능을 수행 한다.
            */
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

                var query = from student in students
                            join score in scores
                            on student.StudentId equals score.StudentId
                            select new
                            {
                                student.Name,
                                score.Score
                            };

                foreach (var item in query)
                {
                    Console.WriteLine($"{item.Name} : {item.Score}");
                }

                Console.ReadLine();
            }
        }
        

        static void LINQ_inner_join()
        {
            /*
                내부 조인은 두 데이터를 비교해서 특정 조건이 일치 하는 경우에만 추출하여 통합한다.

                예를 들어 1번 데이터에는 A,B,C,D 학생의 인적사항이 담겨있고,
                2번 데이터에는 B,C,D,E 학생의 성적 정보가 담겨 있다면,

                학생이 일치하는 조건으로 둘을 내부 조인 했을때 B,C,D 학생의 데이터만 추출하여 통합한다.

                    from a in A
                    join b in B on a.XXX equals b.YYY
                    // a.XXX 와 b.YYY 가 일치하는 a, b 데이터만 추출

                join 으로 일치되는 a, b 데이터를 추출한 후에,
                Anonymous Type 을 만들어서 통합 시켜주면 된다.
            */
            {
                Profile[] profileList =
                {
                        new Profile() { name = "justin", age = 15 }
                    ,   new Profile() { name = "kami", age = 20 }
                    ,   new Profile() { name = "jully", age = 25 }
                };

                Score[] scoreList =
                {
                        new Score() { name = "justin", math = 88, english = 58 }
                    ,   new Score() { name = "kami", math = 77, english = 77 }
                    ,   new Score() { name = "kang", math = 66, english = 99 }
                };

                var Students = from profile in profileList
                               join score in scoreList on profile.name equals score.name
                               select new
                               {
                                   Name = profile.name,
                                   Age = profile.age,
                                   Math = score.math,
                                   English = score.english
                               };

                foreach (var student in Students)
                {
                    Console.WriteLine(student);
                }

                Console.ReadLine();
            }
        }


        static void LINQ_outer_join()
        {
            /*
                외부 조인은 기본적으로 내부 조인과 비슷하다.
                외부 조인은 조건이 일치 하더라도,
                기준 데이터를 하나도 누락 시키지 않고, 그대로 추출한 후에,
                빈 데이터를 채워서 통합한다.

                    from a in A
                    join b in B on a.XXX equals b.YYY into data
                    // a.XXX 와 b.YYY 가 일치 하는 a, b 데이터 추출하여 data 에 저장
                    // a 데이터는 하나도 누락되지 않는다.
                    from b in data.DefaultIfEmpty( new a() { empty = "blank" } )
                    // data 에서 비어있는 데이터를 채운 후, 다시 b 데이터를 가져온다.
            */
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

                var Students = from profile in profileList
                               join score in scoreList on profile.name equals score.name into temp
                               from score in temp.DefaultIfEmpty(new Score() { math = 100, english = 100 })
                               select new
                               {
                                   Name = profile.name,
                                   Age = profile.age,
                                   Math = score.math,
                                   English = score.english
                               };

                foreach (var student in Students)
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

            //LINQ_group_by();

            //LINQ_multiple_data_range_search_set();

            //LINQ_select();

            //LINQ_orderby();

            //LINQ_where();

            //LINQ_from();
        }
    }
}
