using System;
using System.Collections.Generic;
using FastDeepCloner;

namespace NuGetDotNet5.FastDeepCloner;

public class BasicTest
{
    public static void Test()
    {
        testRequiredCase();
        testBasic();
    }

    //---------------------------------------------------------------------------------------------

    /*
        📚 FastDeepCloner를 꼭 사용해야 하는 상황 테스트

          1. 개요
            - 컴파일 타임에 실제 타입을 알 수 없고 object로 전달되는 객체를 Deep Copy 하는 예제이다.
            - private field와 object 타입 Dictionary 내부 값까지 복사해야 하는 상황이다.
            - 플러그인 설정, 동적 옵션 객체, 테스트 도구, 관리 도구에서 사용할 수 있다.

          2. 기본 개념
            - PluginOption은 내부에 private Dictionary<string, object>를 가진다.
            - 외부에서는 _internalSettings에 직접 접근할 수 없다.
            - Dictionary 내부에는 byte[], List<string> 같은 참조 타입 값이 들어간다.
            - 수동 복사를 하려면 private field 접근과 타입별 분기 처리가 필요하다.

          3. 핵심 특징
            - FastDeepCloner는 Reflection 기반으로 private field까지 복사할 수 있다.
            - object 타입으로 받은 실제 런타임 객체를 복사할 수 있다.
            - Dictionary 내부의 byte[] 같은 중첩 참조 객체도 독립 복사된다.

          4. 실행 흐름
            - PluginOption 객체 생성
            - 내부 private Dictionary에 설정값 추가
            - object 타입으로 CloneUnknownObject에 전달
            - DeepCloner.Clone(source) 호출
            - 복사본 내부 byte[] 수정
            - 원본 byte[] 값이 변경되지 않았는지 확인

          5. 대표 메서드 또는 주요 코드
            - CloneUnknownObject(object source)
              컴파일 타임 타입을 모르는 object를 Deep Copy 한다.

            - DeepCloner.Clone(source)
              실제 런타임 타입과 내부 private field까지 복사한다.

          6. 멀티 스레드 환경에서 작동 특징
            - Clone은 현재 thread에서 동기적으로 수행된다.
            - 복사 중 PluginOption 내부 Dictionary가 다른 thread에서 수정되면 문제가 될 수 있다.
            - 멀티 스레드 환경에서는 lock 또는 불변 설정 객체를 사용하는 것이 안전하다.

          7. 주의점
            - 성능이 중요한 요청 처리 경로에서는 사용하지 않는 것이 좋다.
            - object graph가 크면 복사 비용이 커진다.
            - 순환 참조, 이벤트 핸들러, unmanaged resource 포함 객체는 주의해야 한다.
            - IDisposable 리소스를 가진 객체를 복사하는 것은 권장하지 않는다.

          8. 예상 결과
            - 복사본의 Name을 변경해도 원본 Name은 유지된다.
            - 복사본의 byte[] 값을 변경해도 원본 byte[] 값은 유지된다.
            - private Dictionary 내부 값까지 독립 복사된 것을 확인할 수 있다.
    */
    static void testRequiredCase()
    {
        var option = new PluginOption
        {
            Name = "ImagePlugin"
        };

        option.Add("RetryCount", 3);
        option.Add("Buffer", new byte[] { 1, 2, 3 });
        option.Add("Tags", new List<string> { "A", "B" });

        object unknownObject = option;

        var copied = (PluginOption)CloneUnknownObject(unknownObject);

        copied.Name = "CopiedPlugin";

        var copiedBuffer = (byte[])copied.Get("Buffer")!;
        copiedBuffer[0] = 99;

        var copiedTags = (List<string>)copied.Get("Tags")!;
        copiedTags.Add("C");

        var originalBuffer = (byte[])option.Get("Buffer")!;
        var originalTags = (List<string>)option.Get("Tags")!;

        Console.WriteLine("[Required Case]");
        Console.WriteLine(option.Name);          // ImagePlugin
        Console.WriteLine(copied.Name);          // CopiedPlugin

        Console.WriteLine(originalBuffer[0]);    // 1
        Console.WriteLine(copiedBuffer[0]);      // 99

        Console.WriteLine(originalTags.Count);   // 2
        Console.WriteLine(copiedTags.Count);     // 3
        Console.WriteLine();

        Console.ReadLine();
    }

    static object CloneUnknownObject(object source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return DeepCloner.Clone(source);
    }

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

    public class PluginOption
    {
        private Dictionary<string, object> _internalSettings = new();

        public string Name { get; set; } = "";

        public void Add(string key, object value)
        {
            _internalSettings[key] = value;
        }

        public object? Get(string key)
        {
            return _internalSettings.TryGetValue(key, out var value)
                ? value
                : null;
        }
    }

    //---------------------------------------------------------------------------------------------

    /*
        📚 FastDeepCloner 기본 Deep Copy 테스트

          1. 개요
            - FastDeepCloner를 사용하여 객체를 Deep Copy 하는 기본 예제이다.
            - .NET 5 이상에서 사용 가능하다.
            - 단순 DTO, 중첩 객체, List 같은 참조 타입 컬렉션을 복사할 때 사용할 수 있다.

          2. 기본 개념
            - DeepCloner.Clone(source)를 호출하면 source 객체와 내부 참조 객체까지 복사한다.
            - 복사본을 수정해도 원본 객체에는 영향을 주지 않는다.

          3. 핵심 특징
            - User 객체 안의 Address 객체와 Tags List도 새 객체로 복사된다.
            - 단순 참조 복사가 아니라 내부 객체까지 복사된다.

          4. 실행 흐름
            - User 객체 생성
            - DeepCloner.Clone(user) 호출
            - 복사본 수정
            - 원본 값이 변경되지 않았는지 확인

          5. 대표 메서드 또는 주요 코드
            - DeepCloner.Clone(user)
              User 객체 전체를 Deep Copy 한다.

          6. 멀티 스레드 환경에서 작동 특징
            - Clone 자체는 호출 thread에서 동기적으로 실행된다.
            - 복사 중 원본 객체가 다른 thread에서 변경되면 일관성 없는 복사본이 만들어질 수 있다.
            - 멀티 스레드 환경에서는 복사 대상 객체를 lock 하거나 불변 객체로 관리하는 것이 좋다.

          7. 주의점
            - Reflection 기반 복사이므로 성능 비용이 있다.
            - API hot path나 대량 반복 호출에는 적합하지 않다.
            - 단순 값 타입 컬렉션은 생성자 복사가 더 빠르다.

          8. 예상 결과
            - 복사본을 수정해도 원본 User 값은 유지된다.
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

        var copy = DeepCloner.Clone(user);

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