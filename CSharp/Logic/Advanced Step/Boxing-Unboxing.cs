using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep
{
    public class BoxingUnboxing
    {
        interface IPrintable
        {
            void Print();
        }

        struct MyStruct : IPrintable
        {
            public void Print() => Console.WriteLine("Hello");
        }

        static void boxing_unboxing_what()
        {
            /*
                C#에서 Boxing과 Unboxing은 값 타입(value type)과 참조 타입(reference type) 간의 변환을 의미합니다. 
                이 개념은 성능, 메모리 할당, 타입 안정성에 밀접하게 연결되어 있기 때문에 정확히 이해하는 것이 중요합니다.

                Boxing/Unboxing은 값 타입을 참조 타입처럼 사용할 때 발생하며, 불필요하면 피해야 합니다.
                특히 루프, 제네릭, 인터페이스 내에서의 암묵적 Boxing은 성능 저하의 원인이 됩니다.


                ✅ 기본 개념 요약
                  - Boxing : 값 타입을 참조 타입으로 변환 (힙에 복사됨)
                  - Unboxing : 박스된 참조 타입을 다시 값 타입으로 변환 (힙 → 스택 복사)


                🧠 중요한 포인트
                  1. Boxing은 힙에 메모리를 할당한다
                    - 성능 오버헤드 있음 (GC 대상이 됨)
                    - 짧은 시간 안에 많은 박싱이 발생하면 GC 부담 ↑

                  2. Unboxing은 캐스팅이 필요하다
                    - 잘못된 타입으로 언박싱하면 InvalidCastException 발생
            */

            {
                int num = 42;               // 값 타입 (스택)
                object boxed = num;         // ⚠️ Boxing: 값 → 참조 (힙에 복사)
                int unboxed = (int)boxed;   // ⚠️ Unboxing: 참조 → 값 (다시 스택에 복사)
            }

            {
                object boxed = 42;            // ⚠️ Boxing
                double wrong = (double)boxed; // ❌ 런타임 예외
            }

            /*
                ✅ 언제 발생하나?

                상황	                                        예시	                           결과
                값 타입을 object나 인터페이스로 저장할 때	     object o = 123;	                 Boxing
                인터페이스 파라미터로 넘길 때	               IComparable c = 5;	             Boxing
                ToString() 등 호출시 암묵적 boxing	        list.Add(3) (List<object>)	      Boxing
                박스된 값을 다시 캐스팅할 때	               int x = (int)obj;	             Unboxing                          
            */


            /*
                ✅ 구조체와 인터페이스에서의 주의점

                  IPrintable p = new MyStruct();  // ⚠️ Boxing 발생 위치

                  - 이유: MyStruct는 값 타입이고 IPrintable은 참조 인터페이스 → 참조로 넘기기 위해 Boxing

                  object o = 123;        // ⚠️ Boxing
                  int y = (int)o;        // ⚠️ Unboxing
            */

            {
                IPrintable printable = new MyStruct(); // 여기서 Boxing 발생
                printable.Print();

                /*
                    ✅ IL 코드로 Boxing 확인하기

                      1. Release 모드 빌드 (Ctrl + Shift + B)
                      2. Developer Command Prompt에서 다음 명령 실행:
                        - ildasm 빌드된 실행파일명

                      3. 해당 함수명 더블클릭 → IL 코드 확인
                        → 아래와 같은 box, unbox.any 명령어 확인 가능

                        IL_0000:  ldloca.s   0
                        IL_0002:  initobj    MyStruct
                        IL_0008:  ldloc.0
                        IL_0009:  box        MyStruct    // ✅ Boxing 발생!
                        IL_000e:  stloc.1
                        IL_0010:  ldc.i4.s   123
                        IL_0012:  box        [mscorlib]System.Int32 // ✅ Boxing
                        IL_0017:  unbox.any  [mscorlib]System.Int32 // ✅ Unboxing
                */

                /*                    
                    ✅ ILSpy에서 Boxing 확인 절차
                 
                      - ILSpy 설치 및 설정 : https://github.com/icsharpcode/ILSpy

                      1. Release 모드 빌드 (Ctrl + Shift + B)
                      2. ILSpy로 어셈블리 열기
                      3. File → Open... 또는 드래그하여 빌드된 파일(exe 또는 .dll) 열기
                */
            }
        }

        public class Printer<T>
        {
            public void Print(T value)
            {
                Console.WriteLine(value);
            }
        }

        public struct ActionStruct<T>
        {
            public Action<T> Action;

            public void Invoke(T value) => Action(value);
        }

        static void boxing_unboxing_avoid()
        {
            /*
                ✅ Boxing 방지 방법
                방법	                                        설명
                제네릭 사용	List<int>는 박싱 없음,            List<object>는 박싱 발생
                struct의 인터페이스 사용 최소화	            인터페이스로 넘기면 박싱 발생
                Span<T>, ref struct 활용	                   절대 힙 할당 불가 → boxing 자체 불가능
                값 타입을 참조로 변환하지 않기	                object, IComparable, IEnumerable 등 주의  
            */


            // 제네릭을 통한 Boxing 제거
            // ✔️ 제네릭은 타입이 컴파일 타임에 확정되므로 Boxing 없이 동작
            {
                var intPrinter = new Printer<int>();
                intPrinter.Print(123); // No boxing

                var strPrinter = new Printer<string>();
                strPrinter.Print("Hello"); // No boxing
            }

            // interface 회피 → delegate or Action<T> 활용
            {
                var actionStruct = new ActionStruct<int> { Action = x => Console.WriteLine(x) };
                actionStruct.Invoke(100); // No boxing
            }
        }

        public class BoxingBenchmark
        {
            public void WithBoxing()
            {
                object obj;
                for (int i = 0; i < 10000; i++)
                {
                    obj = i;            // Boxing
                    int val = (int)obj; // Unboxing
                }
            }

            public void WithoutBoxing()
            {
                for (int i = 0; i < 10000; i++)
                {
                    int val = i;   // 값 타입 그대로 사용 (no boxing)
                }
            }
        }

        static void boxing_unboxing_benchmarking()
        {
            for (int i = 0; i < 10000; i++)
            {
                int val = i;   // 값 타입 그대로 사용 (no boxing)
            }
        }

        public static void Test()
        {
            boxing_unboxing_avoid();

            boxing_unboxing_what();
        }
    }
}
