using Force.DeepCloner;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NuGetDotNet5.DeepClone;


public class BasicTest
{
    public static void Test()
    {
        testRecursiveReference();
        testContainers();
        testBasic();
    }

    //---------------------------------------------------------------------------------------------
    /*
         📚 DeepCloner 순환 참조 Deep Copy 테스트

           1. 개요
             - Parent / Children 구조처럼 순환 참조가 있는 객체 graph를 Deep Copy 하는 예제이다.
             - 일반 JSON 직렬화 방식으로는 순환 참조 복사가 어렵지만 DeepCloner는 객체 graph를 유지하며 복사할 수 있다.

           2. 기본 개념
             - root.Children[0].Parent가 다시 root를 참조하는 구조이다.
             - DeepClone은 이 순환 참조 관계를 복사본 내부에서 다시 연결한다.

           3. 핵심 특징
             - copy.Children[0].Parent == copy 결과가 true이다.
             - 복사본 child의 Parent가 원본 root가 아니라 복사본 root를 가리킨다.

           4. 실행 흐름
             - root Node 생성
             - child Node 생성
             - child.Parent = root 설정
             - root.Children.Add(child)
             - root.DeepClone()
             - 복사본의 순환 참조 유지 여부 확인

           5. 대표 메서드 또는 주요 코드
             - root.DeepClone()
               순환 참조가 있는 Node graph 전체를 복사한다.

           6. 멀티 스레드 환경에서 작동 특징
             - 복사 중 Children 컬렉션이나 Parent 참조가 변경되면 일관성이 깨질 수 있다.
             - 순환 참조 객체 graph는 변경 중 복사하지 않는 것이 안전하다.

           7. 주의점
             - 순환 참조 graph가 크면 복사 비용이 커질 수 있다.
             - 이벤트, unmanaged resource, thread handle 같은 외부 리소스를 가진 객체 graph는 복사 대상에서 제외하는 것이 좋다.
             - 순환 참조가 많은 도메인 모델은 clone 시점과 범위를 명확히 해야 한다.

           8. 예상 결과
             - 복사본 root 이름은 Root이다.
             - 복사본 child 이름은 Child이다.
             - 복사본 child의 Parent는 복사본 root를 가리킨다.
     */
    static void testRecursiveReference()
    {
        Console.WriteLine("[Recursive Reference]");

        var root = new Node { Name = "Root" };
        var child = new Node { Name = "Child", Parent = root };

        root.Children.Add(child);

        var copy = root.DeepClone();

        Console.WriteLine(copy.Name);                         // Root
        Console.WriteLine(copy.Children[0].Name);             // Child
        Console.WriteLine(copy.Children[0].Parent == copy);   // True
        Console.WriteLine();
    }

    public class Node
    {
        public string Name { get; set; } = "";
        public Node? Parent { get; set; }
        public List<Node> Children { get; set; } = new();
    }

    //---------------------------------------------------------------------------------------------

    /*
        📚 DeepCloner Container Deep Copy 테스트

          1. 개요
            - .NET에서 자주 사용하는 Collection 및 Concurrent Collection을
              Deep Clone 하는 예제이다.
            - Container 내부에 포함된 참조 타입 객체까지
              독립적으로 복사되는지 확인한다.
            - DeepCloner가 다양한 Collection 타입을 정상적으로
              처리할 수 있는지 검증한다.

          2. 기본 개념
            - Container 자체만 복사되는 것이 아니라
              내부에 저장된 객체까지 Deep Copy 된다.
            - 복사본 내부 객체를 수정해도
              원본 Container의 객체는 변경되지 않아야 한다.
            - 얕은 복사(Shallow Copy)와 달리
              참조 객체를 공유하지 않는다.

          3. 핵심 특징
            - List<T>
            - Array
            - HashSet<T>
            - Dictionary<TKey, TValue>
            - SortedDictionary<TKey, TValue>
            - SortedList<TKey, TValue>
            - SortedSet<T>
            - Queue<T>
            - Stack<T>
            - LinkedList<T>
            - ConcurrentDictionary<TKey, TValue>
            - ConcurrentQueue<T>
            - ConcurrentStack<T>
            - ConcurrentBag<T>

            와 같은 Container를 Deep Copy 할 수 있다.

            내부 User, Address 객체도
            독립적인 객체로 생성된다.

          4. 실행 흐름
            - Container 생성
            - User 객체 저장
            - DeepClone 수행
            - 복사본 내부 User 수정
            - 원본 User가 변경되지 않았는지 확인

          5. 대표 메서드 또는 주요 코드

            source.DeepClone()

              - Collection 전체를 Deep Copy 한다.
              - 내부 참조 객체까지 모두 복사한다.

            copy["user1"].Address.City = "Busan"

              - 복사본 객체 수정

            source["user1"].Address.City

              - 원본 객체가 변경되지 않았는지 확인

          6. 멀티 스레드 환경에서 작동 특징

            - DeepClone은 현재 Thread에서 동기적으로 수행된다.
            - Clone 수행 중 Collection이 변경되면
              일관성 없는 복사 결과가 발생할 수 있다.
            - Concurrent Collection도
              Clone 시점의 Snapshot을 보장하지 않는다.
            - 정확한 복사가 필요한 경우
              별도의 동기화가 필요하다.

          7. 주의점

            - 값 타입 Collection은 DeepClone보다
              생성자 복사가 더 빠르다.

              예)

                new List<int>(source)
                new HashSet<uint>(source)
                new Dictionary<string, int>(source)

            - 대량 Collection에서는
              Deep Clone 비용이 커질 수 있다.

            - Collection 내부에 다음 객체가 포함된 경우
              주의가 필요하다.

                Stream
                Socket
                Thread
                CancellationTokenSource
                IDisposable Resource

            - 외부 리소스를 보유한 객체는
              Clone 대상에서 제외하는 것이 좋다.

          8. 예상 결과

            - 복사본 Container 수정 시
              원본 Container는 변경되지 않는다.

            - 복사본 내부 User 수정 시
              원본 User는 변경되지 않는다.

            - 모든 Collection에서
              Deep Copy가 정상 수행된다.

            예)

                Original : Seoul
                Copy     : Busan

            결과가 출력되면
            원본과 복사본이 서로 독립적인 객체임을 의미한다.
    */

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public Address Address { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    public class Address
    {
        public string City { get; set; } = "";
    }

    //---------------------------------------------------------------------------------------------
    static void testContainers()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("HashSet");
        Console.WriteLine("========================================");

        {
            HashSet<uint> source = new() { 1, 2, 3 };

            var copy = source.DeepClone();

            copy.Add(4);

            Console.WriteLine(source.Contains(4)); // False
            Console.WriteLine(copy.Contains(4));   // True
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("Dictionary");
        Console.WriteLine("========================================");

        {
            var dict = new Dictionary<string, User>
            {
                ["user1"] = new User
                {
                    Id = 1,
                    Name = "Kim",
                    Address = new Address { City = "Seoul" }
                }
            };

            var copy = dict.DeepClone();

            copy["user1"].Address.City = "Busan";

            Console.WriteLine(dict["user1"].Address.City); // Seoul
            Console.WriteLine(copy["user1"].Address.City); // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("List");
        Console.WriteLine("========================================");

        {
            var source = new List<User>
        {
            new()
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            }
        };

            var copy = source.DeepClone();

            copy[0].Address.City = "Busan";

            Console.WriteLine(source[0].Address.City); // Seoul
            Console.WriteLine(copy[0].Address.City);   // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("Array");
        Console.WriteLine("========================================");

        {
            var source = new[]
            {
            new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            }
        };

            var copy = source.DeepClone();

            copy[0].Address.City = "Busan";

            Console.WriteLine(source[0].Address.City); // Seoul
            Console.WriteLine(copy[0].Address.City);   // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("Queue");
        Console.WriteLine("========================================");

        {
            var source = new Queue<User>();

            source.Enqueue(new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            });

            var copy = source.DeepClone();

            copy.Peek().Address.City = "Busan";

            Console.WriteLine(source.Peek().Address.City); // Seoul
            Console.WriteLine(copy.Peek().Address.City);   // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("Stack");
        Console.WriteLine("========================================");

        {
            var source = new Stack<User>();

            source.Push(new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            });

            var copy = source.DeepClone();

            copy.Peek().Address.City = "Busan";

            Console.WriteLine(source.Peek().Address.City); // Seoul
            Console.WriteLine(copy.Peek().Address.City);   // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("LinkedList");
        Console.WriteLine("========================================");

        {
            var source = new LinkedList<User>();

            source.AddLast(new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            });

            var copy = source.DeepClone();

            copy.First!.Value.Address.City = "Busan";

            Console.WriteLine(source.First!.Value.Address.City); // Seoul
            Console.WriteLine(copy.First!.Value.Address.City);   // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("ConcurrentDictionary");
        Console.WriteLine("========================================");

        {
            var source = new ConcurrentDictionary<string, User>();

            source["user1"] = new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            };

            var copy = source.DeepClone();

            copy["user1"].Address.City = "Busan";

            Console.WriteLine(source["user1"].Address.City); // Seoul
            Console.WriteLine(copy["user1"].Address.City);   // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("ConcurrentQueue");
        Console.WriteLine("========================================");

        {
            var source = new ConcurrentQueue<User>();

            source.Enqueue(new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            });

            var copy = source.DeepClone();

            copy.TryPeek(out var copiedUser);
            copiedUser!.Address.City = "Busan";

            source.TryPeek(out var originalUser);

            Console.WriteLine(originalUser!.Address.City); // Seoul
            Console.WriteLine(copiedUser.Address.City);    // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("ConcurrentStack");
        Console.WriteLine("========================================");

        {
            var source = new ConcurrentStack<User>();

            source.Push(new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            });

            var copy = source.DeepClone();

            copy.TryPeek(out var copiedUser);
            copiedUser!.Address.City = "Busan";

            source.TryPeek(out var originalUser);

            Console.WriteLine(originalUser!.Address.City); // Seoul
            Console.WriteLine(copiedUser.Address.City);    // Busan
        }

        Console.WriteLine();

        Console.WriteLine("========================================");
        Console.WriteLine("ConcurrentBag");
        Console.WriteLine("========================================");

        {
            var source = new ConcurrentBag<User>();

            source.Add(new User
            {
                Id = 1,
                Name = "Kim",
                Address = new Address { City = "Seoul" }
            });

            var copy = source.DeepClone();

            var original = source.First();
            var copied = copy.First();

            copied.Address.City = "Busan";

            Console.WriteLine(original.Address.City); // Seoul
            Console.WriteLine(copied.Address.City);   // Busan
        }

        Console.WriteLine();

        Console.ReadLine();
    }

    //---------------------------------------------------------------------------------------------

    /*
        📚 DeepCloner 기본 Deep Copy 테스트

        1. 개요
            - Force.DeepCloner를 사용하여 객체를 Deep Copy 하는 기본 예제이다.
            - .NET Framework, .NET Core, .NET 5+ 환경에서 사용할 수 있다.
            - DTO, 중첩 객체, List 같은 일반 객체 복사에 사용할 수 있다.

        2. 기본 개념
            - source.DeepClone() 확장 메서드를 호출한다.
            - 원본 객체와 내부 참조 객체까지 새 객체로 복사한다.

        3. 핵심 특징
            - User 객체뿐 아니라 Address, Tags도 독립 객체로 복사된다.
            - 복사본을 수정해도 원본에 영향이 없다.

        4. 실행 흐름
            - User 객체 생성
            - DeepClone 호출
            - 복사본 수정
            - 원본 값 유지 여부 확인

        5. 대표 메서드 또는 주요 코드
            - user.DeepClone()
            User 객체 전체를 Deep Copy 한다.

        6. 멀티 스레드 환경에서 작동 특징
            - DeepClone은 호출 thread에서 동기적으로 수행된다.
            - 복사 중 원본 객체가 다른 thread에서 변경되면 일관성 없는 복사본이 만들어질 수 있다.
            - 멀티 스레드 환경에서는 복사 대상 객체를 lock 하거나 immutable 구조로 관리하는 것이 좋다.

        7. 주의점
            - Deep Copy는 객체 graph가 클수록 비용이 커진다.
            - 단순 값 타입 컬렉션은 생성자 복사가 더 빠르다.
            - IDisposable, Stream, Socket 같은 리소스 객체 복사는 주의해야 한다.

        8. 예상 결과
            - 복사본의 Name, Address.City, Tags를 변경해도 원본은 변경되지 않는다.
    */
    static void testBasic()
    {
        var user = new User
        {
            Id = 1,
            Name = "Kim",
            Address = new Address { City = "Seoul" },
            Tags = new List<string> { "Admin", "User" }
        };

        var copy = user.DeepClone();

        copy.Name = "Lee";
        copy.Address.City = "Busan";
        copy.Tags.Add("Manager");

        Console.WriteLine("[Basic]");
        Console.WriteLine(user.Name);          // Kim
        Console.WriteLine(user.Address.City);  // Seoul
        Console.WriteLine(user.Tags.Count);    // 2
        Console.WriteLine();

        Console.ReadLine();
    }
}
