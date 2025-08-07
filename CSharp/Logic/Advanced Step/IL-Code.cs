using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace AdvancedStep
{
    public class ILCode
    {
        static void IL_code_what()
        {
            /*
                1. IL 코드란?            
                  - IL은 .NET 언어(C#, F#, VB 등)로 작성된 소스코드를 컴파일하면 생성되는 “중간 코드(바이트코드)”입니다.
                  - 최종적으로 .NET 런타임(JIT)이 실행 시점에 이 IL을 “기계어”로 변환해 실행합니다.
                  - IL 코드는 어셈블리(예: MyApp.dll) 내부에 저장되어 있습니다.
                  - 도구: ILDasm, ILSpy, ildasm, ilasm 명령어 등.

                2. IL 코드의 기본 구조
                  - IL은 어셈블리어와 유사한 저수준 언어이지만, 플랫폼에 독립적입니다.

                  2.1. 주요 구성요소
                    - .assembly : 어셈블리 메타정보
                    - .class : 클래스 정의
                    - .method : 메서드 정의
                    - 명령어(Instruction) : 실제 동작 수행 (ldc.i4, call, ret 등)

                3. IL 코드의 문법
                  3.1. 클래스/메서드 선언

                    .assembly MyApp {}
                    .class public auto ansi beforefieldinit MyApp.Program
                    extends [mscorlib]System.Object
                    {
                        .method public hidebysig static void  Main() cil managed
                        {
                            .entrypoint
                            // 코드 시작
                            ...
                            ret
                        }
                    }

                    - .assembly: 어셈블리 이름
                    - .class: 클래스 정의 (상속, 접근자 포함)
                    - .method: 메서드 정의 (Main이 entrypoint)

                  3.2. IL 명령어(Instruction)

                    (1) 데이터 적재/저장
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | ldc.i4 10  | 4바이트 정수 10을 스택에 push
                      | ldarg.0    | 0번째 인자(보통 this)를 스택에 push
                      | stloc.0    | 스택 최상단 값을 지역변수 0에 저장
                      | ldloc.0    | 지역변수 0 값을 스택에 push

                    (2) 메서드 호출/반환                
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | call       | 정적 메서드 호출
                      | callvirt   | 가상(인스턴스) 메서드
                      | ret        | 반환

                    (3) 제어문
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | br.s label | label로 무조건 분기
                      | brtrue     | 스택 최상단이 true면 분기
                      | beq        | 값이 같으면 분기                

                    (4) 비교/연산
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | add        | 두 값 더하기
                      | sub        | 두 값 빼기
                      | ceq        | 두 값이 같으면 1, 아니면 0

                    (5) 지역 변수, 필드, 배열
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | ldloc.n    | 지역 변수 n 값을 스택에 push
                      | stloc.n    | 스택 값을 지역 변수 n에 저장
                      | ldfld      | 인스턴스 필드 값을 스택에 push
                      | stfld      | 스택 값을 인스턴스 필드에 저장
                      | ldsfld     | static 필드 값을 스택에 push
                      | stsfld     | 스택 값을 static 필드에 저장
                      | ldelem.i4  | 배열 int32 요소 값을 스택에 push
                      | stelem.i4  | 배열 int32 요소에 스택 값을 저장
                      | newarr     | 새 배열 생성

                    (6) 박싱/언박싱, 타입 변환
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | box        | 값 타입을 참조 타입(오브젝트)로 변환 (박싱)
                      | unbox.any  | 참조 타입을 값 타입으로 변환 (언박싱)
                      | castclass  | 객체를 지정 타입으로 캐스팅
                      | isinst     | 객체가 특정 타입인지 확인
                      | conv.i4    | 스택 상단 값을 int32로 변환

                    (7) 객체 생성 및 관리
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | newobj     | 새 객체 생성(생성자 호출)
                      | initobj    | 값 타입 인스턴스 초기화
                      | ldtoken    | 타입/메서드/필드 토큰 로드

                    (8) 스택 조작
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | dup        | 스택 최상단 값 복사 후 push
                      | pop        | 스택 최상단 값 버림

                    (9) 상수 및 null, 문자열
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | ldc.i4/8   | int32/int64 상수 push
                      | ldc.r4/8   | float32/float64 상수 push
                      | ldstr      | 문자열 상수 push
                      | ldnull     | null 값 push

                    (10) Delegate/간접 호출
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | calli      | 함수 포인터 통한 호출(간접 호출)

                    (11) 예외 처리/흐름제어
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | .try, catch| 예외 처리 블록
                      | throw      | 예외 던지기
                      | leave      | 블록(try/finally) 탈출
                      | endfinally | finally 블록 종료

                    (12) 분기/조건(비교 연산 포함)
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | beq        | 같으면 분기
                      | bne.un     | 다르면 분기
                      | bge        | 크거나 같으면 분기
                      | bgt        | 크면 분기
                      | ble        | 작거나 같으면 분기
                      | blt        | 작으면 분기
                      | brfalse    | 0/false면 분기
                      | brtrue     | 1/true면 분기

                    (13) 논리/비트 연산
                      | 명령어     | 설명
                      |------------|----------------------------------------
                      | and        | 비트 AND
                      | or         | 비트 OR
                      | xor        | 비트 XOR
                      | shl        | 비트 왼쪽 시프트
                      | shr        | 비트 오른쪽 시프트

                    (14) 기타 특수 명령어/지시문
                      | 명령어        | 설명
                      |---------------|----------------------------------------
                      | nop           | 아무것도 하지 않음 (No Operation)
                      | .maxstack     | 메서드 최대 스택 깊이 선언
                      | .locals init  | 지역변수 선언
                      | .entrypoint   | 프로그램 진입점 선언

                ※ IL 명령어는 스택 기반(모든 연산이 스택에 값을 push/pop)으로 동작하며, 복잡한 문법도 내부적으로 상태머신, 클래스, 헬퍼 메서드 등으로 변환됨.
            */
        }

        static void IL_code_CSharp_to_IL()
        {
            /*
                //=================================================================================
                // 1. 데이터 적재/저장
                //=================================================================================
                int x = 10;         // IL:
                                    // .locals init ([0] int32 x)
                                    // IL_0000: ldc.i4.s 10
                                    // IL_0002: stloc.0

                int y = x;          // IL:
                                    // IL_0003: ldloc.0
                                    // IL_0004: stloc.1

                int Add(int a, int b) => a + b;

                // IL:
                .method public hidebysig static int32 Add(int32 a, int32 b) cil managed
                {
                    .maxstack 2         
                    IL_0000: ldarg.0    // a
                    IL_0001: ldarg.1    // b
                    IL_0002: add        // a + b
                    IL_0003: ret        // return
                }


                //=================================================================================
                // 2. 메서드 호출/반환
                //=================================================================================
                int Sum(int a, int b) { return Add(a, b); }

                // IL:
                .method public hidebysig static int32 Sum(int32 a, int32 b) cil managed
                {
                    .maxstack 8                             
                    IL_0000: ldarg.0                        // a를 스택에 push
                    IL_0001: ldarg.1                        // b를 스택에 push
                    IL_0002: call int32 Add(int32, int32)   // Add 메서드 호출
                    IL_0007: ret                            // 결과 반환
                }


                //=================================================================================
                // 3. 제어문
                //=================================================================================
                bool IsPositive(int n)
                {
                    if (n > 0) return true;
                    return false;
                }

                // IL:
                .method public hidebysig static bool IsPositive(int32 n) cil managed
                {
                    .maxstack 1
                    .locals init ([0] bool V_0)
                    IL_0000: ldarg.0           // n을 스택에 push
                    IL_0001: ldc.i4.0          // 상수 0을 스택에 push
                    IL_0002: ble.s IL_0006     // n <= 0이면 IL_0006으로 분기
                    IL_0004: ldc.i4.1          // n > 0이면 true(1) push
                    IL_0005: ret               // 결과 반환
                    IL_0006: ldc.i4.0          // n <= 0이면 false(0) push
                    IL_0007: ret               // 결과 반환
                }


                //=================================================================================
                // 4. 비교/연산
                //=================================================================================
                int Max(int a, int b)
                {
                    if (a >= b) return a;
                    else return b;
                }

                // IL:
                .method public hidebysig static int32 Max(int32 a, int32 b) cil managed
                {
                    .maxstack 2
                    IL_0000: ldarg.0          // a를 스택에 push
                    IL_0001: ldarg.1          // b를 스택에 push
                    IL_0002: blt.s IL_0006    // a < b이면 IL_0006으로 분기
                    IL_0004: ldarg.0          // (a >= b) → a를 반환
                    IL_0005: ret              // 결과 반환
                    IL_0006: ldarg.1          // (a < b) → b를 반환
                    IL_0007: ret              // 결과 반환
                }


                //=================================================================================
                // 5. 지역 변수, 필드, 배열
                //=================================================================================
                class MyClass
                {
                    public int Field;
                    public static int StaticField;
                    public void SetField(int val)
                    {
                        Field = val;
                        StaticField = val;
                    }
                }

                // IL (SetField):
                .method public hidebysig instance void SetField(int32 val) cil managed
                {
                    .maxstack 2
                    IL_0000: ldarg.0                            // this(객체 참조)을 스택에 push (인스턴스 필드 접근을 위함)
                    IL_0001: ldarg.1                            // 메서드 인자 val을 스택에 push
                    IL_0002: stfld int32 MyClass::Field         // this.Field = val; (스택에서 값과 참조를 꺼내 대입)
                    IL_0007: ldarg.1                            // val을 스택에 push (static 필드 대입을 위해)
                    IL_0008: stsfld int32 MyClass::StaticField  // MyClass.StaticField = val; (스택에서 값 꺼내 대입)
                    IL_000D: ret                                // 결과 반환
                }

                int[] arr = new int[3];

                // IL:
                .locals init ([0] int32[] arr)
                IL_0000: ldc.i4.3                       // 3을 push
                IL_0001: newarr [mscorlib]System.Int32  // int[3] 생성
                IL_0006: stloc.0                        // arr에 저장

                arr[0] = 7;

                // IL:
                IL_0007: ldloc.0                     // arr을 push
                IL_0008: ldc.i4.0                    // 인덱스 0 push
                IL_0009: ldc.i4.7                    // 값 7 push
                IL_000A: stelem.i4                   // arr[0] = 7


                //=================================================================================
                // 6. 박싱/언박싱, 타입 변환
                //=================================================================================
                object o = 5;                               // 값타입을 object로 변환 (박싱)
                
                // IL:
                IL_0000: ldc.i4.5                           // 상수 5를 스택에 push
                IL_0001: box [mscorlib]System.Int32         // int32를 object(참조타입)로 박싱
                IL_0006: stloc.0                            // o 변수에 저장
                
                int i2 = (int)o;                            // object에서 int로 변환 (언박싱)

                // IL:
                IL_0007: ldloc.0                            // o 변수 값(object)을 스택에 push
                IL_0008: unbox.any [mscorlib]System.Int32   // object를 int32로 언박싱하여 스택에 push
                IL_000D: stloc.1                            // i2 변수에 저장
            
                object o2 = "test";                         // 문자열 리터럴을 object에 할당
                
                // IL:
                IL_000E: ldstr "test"                       // "test" 문자열을 스택에 push
                IL_0013: stloc.2                            // o2 변수에 저장

                string s2 = o2 as string;                   // object를 string으로 안전하게 캐스팅 (실패시 null)

                // IL:
                IL_0014: ldloc.2                            // o2(object)를 스택에 push
                IL_0015: isinst [mscorlib]System.String     // o2가 string이면 해당 참조, 아니면 null
                IL_001A: stloc.3                            // s2에 저장


                //=================================================================================
                // 7. 객체 생성 및 관리
                //=================================================================================
                var obj = new MyClass();
                
                // IL:
                IL_0000: newobj instance void MyClass::.ctor()  // MyClass의 생성자를 호출하여 새 객체 생성
                IL_0005: stloc.0                                // obj 지역 변수에 저장

                var type = typeof(MyClass);
                
                // IL:
                IL_0006: ldtoken MyClass                                                                                                            // MyClass 타입의 메타데이터 토큰을 스택에 push
                IL_000B: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)    // RuntimeTypeHandle을 받아 System.Type 객체로 변환
                IL_0010: stloc.1                                                                                                                    // type 변수에 저장


                //=================================================================================
                // 8. 스택 조작 (C#에서 직접은 불가, IL에서만 dup/pop 등 사용 가능)
                //=================================================================================
                // C#에서는 스택 조작 명령(dup, pop 등)을 직접 작성할 방법이 없습니다.
                // 하지만 다음과 같이 컴파일러가 중간값을 두 번 사용하는 경우 등에서 IL에서는 dup가 자동 사용될 수 있습니다:
            
                int a = 1;
                int b = a + a; // 'a'를 두 번 더함. 이때 IL에선 dup 명령이 쓰임.

                // IL: (dup, pop 예시)
                .locals init ([0] int32 a, [1] int32 b)
                IL_0000: ldc.i4.1       // 1을 push
                IL_0001: stloc.0        // a = 1
                IL_0002: ldloc.0        // a를 스택에 push
                IL_0003: dup            // 스택 최상단 값(a)를 복사해 다시 push (a + a 만들기)
                IL_0004: add            // 두 a를 더함
                IL_0005: stloc.1        // b = a + a
                
                // pop의 예시도 직접적으로 C#에는 없으나, 
                // 예를 들어, 어떤 계산 결과를 변수에 저장하지 않고 버릴 때 IL에서는 pop이 사용될 수 있습니다:

                int x = 42;
                x + 1; // 결과를 변수에 저장하지 않음

                // IL: (pop 예시)
                .locals init ([0] int32 x)
                IL_0000: ldc.i4.s 42      // 상수 42를 스택에 push
                IL_0002: stloc.0          // x = 42
                IL_0003: ldloc.0          // x 값을 스택에 push
                IL_0004: ldc.i4.1         // 상수 1을 스택에 push
                IL_0005: add              // x + 1 계산, 결과가 스택에 push
                IL_0006: pop              // 결과를 사용하지 않고 스택에서 버림


                //=================================================================================
                // 9. 상수 및 null, 문자열
                //=================================================================================
                int five = 5;             // 정수 상수 할당                
                // IL:
                IL_0000: ldc.i4.5         // 상수 5를 스택에 push
                IL_0001: stloc.0          // five 변수에 저장

                string hello = "Hello";   // 문자열 상수 할당
                // IL:
                IL_0002: ldstr "Hello"    // "Hello" 문자열 리터럴을 스택에 push
                IL_0007: stloc.1          // hello 변수에 저장
                
                object nothing = null;    // null 참조 할당
                // IL:
                IL_0008: ldnull           // null 값을 스택에 push
                IL_0009: stloc.2          // nothing 변수에 저장


                //=================================================================================
                // 10. Delegate/간접 호출
                //=================================================================================
                Func<int, int> square = x => x * x;

                // IL:

                // 1. 람다식 본문을 컴파일러가 "익명 메서드"로 변환한 클래스(클로저)
                .class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass0_0'
                       extends [mscorlib]System.Object
                {
                    // 람다식 본문을 익명 메서드로 변환
                    .method public hidebysig instance int32 '<square>b__0'(int32 x) cil managed
                    {
                        .maxstack 8
                        IL_0000: ldarg.1          // 인자 x를 스택에 push
                        IL_0001: ldarg.1          // 인자 x를 다시 push (x * x 준비)
                        IL_0002: mul              // 두 값을 곱함 (x * x)
                        IL_0003: ret              // 결과 반환
                    }
                }

                // 2. 델리게이트 인스턴스 생성 (람다를 Func<int,int>에 할당하는 부분)
                // 아래는 Main 등 람다 정의부에서 실제로 생성되는 흐름
                IL_0000: ldnull                                                                                         // this가 없는 static 람다라 ldnull, (인스턴스 캡처시엔 this 참조 push)
                IL_0001: ldftn instance int32 '<>c__DisplayClass0_0'::'<square>b__0'(int32)                             // 위 익명 메서드의 함수 포인터를 push (Delegate의 Target)
                IL_0007: newobj instance void class [mscorlib]System.Func`2<int32, int32>::.ctor(object, native int)    // Func<int,int> 델리게이트 객체를 생성 (object+함수포인터로)
                IL_000C: stloc.0                                                                                        // square에 저장

                // 3. 델리게이트 호출 (예: square(5) 실행)
                // IL_000D: ldloc.0                                                                                 // square 인스턴스 push
                // IL_000E: ldc.i4.5                                                                                // 인자 5를 스택에 push
                // IL_000F: callvirt instance int32 class [mscorlib]System.Func`2<int32, int32>::Invoke(int32)      // square.Invoke(5) 호출 → 내부적으로 익명 메서드 실행


                //=================================================================================
                // 11. 예외 처리/흐름제어
                //=================================================================================
                try
                {
                    throw new Exception("Err");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // IL (핵심 흐름)
                .try
                {
                    IL_0000: ldstr "Err"                                                    // "Err" 문자열 push
                    IL_0005: newobj instance void [mscorlib]System.Exception::.ctor(string) // Exception 생성
                    IL_000A: throw                                                          // 예외 발생
                    IL_000B: leave.s IL_0018                                                // (정상 흐름 시 탈출)
                }
                catch [mscorlib]System.Exception
                {
                    IL_000D: stloc.0                                                            // ex를 지역 변수에 저장
                    IL_000E: ldloc.0                                                            // ex push
                    IL_000F: callvirt instance string [mscorlib]System.Exception::get_Message() // ex.Message 호출
                    IL_0014: call void [mscorlib]System.Console::WriteLine(string)              // WriteLine 호출
                    IL_0019: leave.s IL_0018                                                    // catch 블록 탈출
                }


                //=================================================================================
                // 12. 분기/조건(비교 연산 포함)
                //=================================================================================
                bool IsEven(int n)
                {
                    return n % 2 == 0;
                }

                // IL:
                .method public hidebysig static bool IsEven(int32 n) cil managed
                {
                    .maxstack 2
                    IL_0000: ldarg.0       // 인자 n을 스택에 push
                    IL_0001: ldc.i4.2      // 상수 2를 스택에 push
                    IL_0002: rem           // n % 2 (n을 2로 나눈 나머지) 계산
                    IL_0003: ldc.i4.0      // 상수 0을 스택에 push
                    IL_0004: ceq           // (n % 2 == 0) → 같으면 1, 다르면 0 push
                    IL_0006: ret           // 결과 반환 (bool)
                }


                //=================================================================================
                // 13. 논리/비트 연산
                //=================================================================================
                int bitOr = 5 | 2;      // 비트 OR 연산

                // IL:
                IL_0000: ldc.i4.5       // 5를 스택에 push
                IL_0001: ldc.i4.2       // 2를 스택에 push
                IL_0002: or             // 5 | 2 (비트 OR)
                IL_0003: stloc.0        // bitOr에 저장
                
                int bitAnd = 5 & 3;     // 비트 AND 연산

                // IL:
                IL_0004: ldc.i4.5       // 5를 스택에 push
                IL_0005: ldc.i4.3       // 3을 스택에 push
                IL_0006: and            // 5 & 3 (비트 AND)
                IL_0007: stloc.1        // bitAnd에 저장
                
                int bitXor = 5 ^ 1;     // 비트 XOR 연산
                
                // IL:
                IL_0008: ldc.i4.5       // 5를 스택에 push
                IL_0009: ldc.i4.1       // 1을 스택에 push
                IL_000A: xor            // 5 ^ 1 (비트 XOR)
                IL_000B: stloc.2        // bitXor에 저장
                
                int shl = 2 << 1;       // 비트 왼쪽 시프트
                
                // IL:
                IL_000C: ldc.i4.2       // 2를 스택에 push
                IL_000D: ldc.i4.1       // 1을 스택에 push
                IL_000E: shl            // 2 << 1 (왼쪽 시프트)
                IL_000F: stloc.3        // shl에 저장
                
                int shr = 8 >> 2;       // 비트 오른쪽 시프트
                
                // IL:
                IL_0010: ldc.i4.8       // 8을 스택에 push
                IL_0011: ldc.i4.2       // 2를 스택에 push
                IL_0012: shr            // 8 >> 2 (오른쪽 시프트)
                IL_0013: stloc.4        // shr에 저장


                //=================================================================================
                // 14. 기타 특수 명령어/지시문
                //=================================================================================
                // nop, .maxstack, .locals init, .entrypoint 등은 C# 소스에는 없고, IL 코드 헤더나 컴파일러가 자동 추가

                class Program
                {
                    static void Main(string[] args)
                    {
                        // 프로그램 진입점 (entrypoint)
                        int x = 10;
                        Console.WriteLine(x);
                    }
                }

                // IL 코드 (헤더 및 메타데이터, 컴파일러가 자동 추가)
                .assembly ConsoleApp1 {}             // 어셈블리 정의

                .class private auto ansi beforefieldinit Program
                       extends [mscorlib]System.Object
                {
                    .method private hidebysig static void Main(string[] args) cil managed
                    {
                        .entrypoint                   // 프로그램 진입점 (Main)
                        .maxstack 2                   // 평가 스택 최대 깊이 (컴파일러가 결정)
                        .locals init ([0] int32 x)    // 지역 변수 선언 및 초기화

                        // (C#에는 명시적으로 보이지 않지만, 최적화/디버그 목적 등으로 nop이 삽입될 수 있음)
                        IL_0000: nop                  // 아무것도 하지 않음 (No Operation)
                        IL_0001: ldc.i4.s 10
                        IL_0003: stloc.0
                        IL_0004: ldloc.0
                        IL_0005: call void [mscorlib]System.Console::WriteLine(int32)
                        IL_000A: nop                  // (종료 직전에도 들어갈 수 있음)
                        IL_000B: ret
                    }
                }
            */
        }


        public static void Test()
        {
            IL_code_CSharp_to_IL();

            IL_code_what();
        }
    }
}

