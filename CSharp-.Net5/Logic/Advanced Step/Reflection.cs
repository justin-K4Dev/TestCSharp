using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;



public class Reflection
{
    [Serializable]
    public class MyClass { }

    static void use_GetCustomAttribute()
    {
        /*
            클래스/멤버에 붙은 어트리뷰트를 간결하게 읽는다.

            ✅ 기존보다 훨씬 간결한 어트리뷰트 조회 방식
       */

        var attr = typeof(MyClass).GetCustomAttribute<SerializableAttribute>();
        Console.WriteLine(attr != null ? "Serializable 적용됨" : "없음");
    }


    static void use_HasMetadataToken()
    {
        /*
            메서드가 메타데이터 토큰을 안전하게 가지고 있는지 확인

            ✅ 동적 코드 또는 Reflection.Emit 환경에서 토큰 접근 안전성을 확보

            ✅ MetadataToken 이란?
              - .NET 메타데이터 테이블에서 각 멤버(타입, 메서드, 속성 등)를 고유하게 식별하는 정수값
              - IL, 디버깅, Reflection.Emit, Roslyn 등에서 내부적으로 사용됨
              - 예: Console.WriteLine(string) → 0x060005C9
       */

        var method = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });

        if (method?.HasMetadataToken() == true)
        {
            Console.WriteLine($"MetadataToken = {method.MetadataToken}");
        }
    }


    class Animal { }
    class Dog : Animal { }

    static void use_IsAssignableTo()
    {
        /*
            타입 간 호환성 검사 (더 읽기 쉬운 방향으로)

            ✅ IsAssignableFrom()의 역방향을 직관적으로 표현
       */

        Console.WriteLine(typeof(Dog).IsAssignableTo(typeof(Animal))); // true
        Console.WriteLine(typeof(Animal).IsAssignableTo(typeof(Dog))); // false
    }


    interface IAnimal { }
    interface IWalker { }
    class DogEx : IAnimal, IWalker { }

    static void use_GetInterfaces()
    {
        /*
            인터페이스 목록 순서가 플랫폼/런타임 간 일관됨

            ✅ 테스트 시 비교용 인터페이스 배열의 순서 안정성 향상
       */

        var ifaces = typeof(DogEx).GetInterfaces();
        foreach (var iface in ifaces)
            Console.WriteLine(iface.Name); // 순서 일관 보장
    }


    public class Person
    {
        public string Name { get; }
        public Person(string name) => Name = name;
    }

    static void use_ConstructorInfo()
    {
        /*
            ConstructorInfo.Invoke(...) 확장 오버로드

            ✅ 고급 시나리오에서 바인딩 옵션, 로컬화 설정 등 제어 가능
       */

        // 함수의 시그니처와 정확히 일치하는 생성자를 반환 !!!
        var ctor = typeof(Person).GetConstructor(new[] { typeof(string) }); 
        // Invoke(...)는 해당 생성자를 실제로 실행하여 객체를 생성
        var obj = ctor?.Invoke(BindingFlags.Public, null, new object[] { "Alice" }, CultureInfo.InvariantCulture);

        // CultureInfo.InvariantCulture 란?
        // 문화권(locale)에 영향을 받지 않는 고정된 중립 문화 정보를 나타냅니다.
        // 날짜, 숫자, 통화 등의 문자열 포맷 및 파싱시 항상 동일한 기준을 제공합니다.

        Console.WriteLine(((Person)obj!).Name); // 출력: Alice
    }


    static void use_IsDynamicCodeCompiled()
    {
        /*
            동적 코드(IL.Emit, Expression.Compile 등) 실행 가능 여부 확인

            ✅ Blazor WebAssembly, NativeAOT 등에서 Reflection 제한 여부 판단
       */

        if (System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled)
        {
            Console.WriteLine("동적 코드 실행 가능");
        }
        else
        {
            Console.WriteLine("AOT 환경 - 동적 코드 불가");
        }
    }

    static void use_Assembly()
    {
        /*
            어셈블리 이름 출력 형식 일관화

            ✅ 런타임 간 차이 없는 어셈블리 비교용 문자열 확보
       */

        var asm = typeof(List<>).Assembly;

        Console.WriteLine(asm.GetName().FullName);
        // 예: System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=...
    }

    static void use_GetRequiredCustomModifiers()
    {
        /*
            IL에서 사용되는 modreq 정보 조회 (고급)

            ✅ IL/Interop/COM 환경에서 쓰이는 저수준 메타데이터 분석에 사용
       */

        var prop = typeof(ValueTuple<int>).GetProperty("Item1");
        var modifiers = prop?.GetRequiredCustomModifiers();

        foreach (var mod in modifiers ?? [])
        {
            Console.WriteLine("필수 수정자: " + mod.FullName);
        }
    }

    public static void Test()
    {
        //use_GetRequiredCustomModifiers();

        //use_Assembly();

        //use_IsDynamicCodeCompiled();

        //use_ConstructorInfo();

        //use_GetInterfaces();

        //use_IsAssignableTo();

        //use_HasMetadataToken();

        //use_GetCustomAttribute();
    }
}
