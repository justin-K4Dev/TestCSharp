using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;




namespace AdvancedStep
{
    public class Dynamic
    {
        static void static_and_dynamic_language_concept()
        {
            /*
                프로그래밍은 언어는, 그를 구분하는 한 방식으로,
                컴파일시 Type Checking을 진행하는 Static Language (Statically Typed Language)와
                런타임시 Type을 판별하는 Dynamic Language로 구분할 수 있다.
                예를 들어, C#은 Static Language에 속하며, Python, Ruby, JavaScript 등은 Dynamic Language에 속한다.

                C#은 기본적으로 Static Language이지만, C# 4.0에서 Dynamic Language의 요소를 추가하였다.
                즉, C# 4.0 언어에서는 dynamic 이라는 키워드를 추가하였고,
                .NET Framework 4.0에는 DLR (Dynamic Language Runtime)를 추가함으로서
                Dynamic Language가 갖는 기능을 언어와 프레임워크에 수용한 것이다.
                따라서 .NET 4.0의 DLR을 사용하면서 다른 Dynamic Language를 함께 사용하는 것이 가능해 졌다.
            */
            {
                Console.ReadLine();
            }
        }
        
        
        static void dynamic_keyword()
        {
            /*
                C#의 dyanmic은 C# 4.0에서 새로 도입된 키워드로서
                컴파일러에게 변수의 Type을 체크하지 않도록 하고 런타임시까지는 해당 타입을 알 수 없음을 표시한다.
                내부적으로 dynamic 타입은 object 타입을 사용하므로 dynamic 타입의 변수는 중간에 다른 타입의 값을 가질 수 있다.
                예를 들어, dynamic 변수에 숫자를 할당했다가 나중에 다시 문자열을 할당하는 것이 가능하다.
                object 타입과 dynamic 타입의 차이점은 object 타입은 구체적 타입의 속성과 메서드를 사용하기 전에
                반드시 캐스팅을 통해 구체적 타입으로 변경 후에 사용해야 하는 반면,
                dynamic 타입은 캐스팅이 없이도 직접 실제 타입(underlying type)의 메서드와 속성을 사용할 수 있다는 점이다.
            */
            {
                // 1. dynamic은 중간에 형을 변환할 수 있다.

                dynamic v = 1;
                // System.Int32 출력
                Console.WriteLine(v.GetType());

                v = "abc";
                // System.String 출력
                Console.WriteLine(v.GetType());


                // 2. dynamic은 cast가 필요없다

                object o = 10;
                // 틀린표현
                // (에러: Operator '+' cannot be applied to operands of type 'object' and 'int')
                // o = o + 20;
                // 맞는 표현: object 타입은 casting이 필요하다
                o = (int)o + 20;

                // dynamic 타입은 casting이 필요없다.
                dynamic d = 10;
                d = d + 20;

                Console.ReadLine();
            }
        }


        static void anonymous_type_with_dynamic()
        {
            /*
                C# dynamic의 간단한 예제를 보이기 위해 아래 예제는 익명타입 객체를 dynamic 에 할당하고
                이를 다른 클래스 메서드에 파라미터로 전달하는 예를 들었다.
                dynamic은 컴파일러에게 하나의 정적 Type으로 인식되기 때문에
                메서드 원형에서도 - int, string과 같이 - 파라미터 타입에 dynamic 이라고 지정한다.
                전달된 dynamic 파라미터는 (아래 Class2.Run() 메서드에서 보듯이) 그 dynamic 객체로부터 직접 속성을 호출할 수 있다.

                하지만, 이 예제코드는 2가지 큰 제약점을 가지고 있는데,
                첫째는 익명타입은 한번 생성된 후 다시 새로운 속성을 추가할 수 없고
                또한 익명타입 자체가 메서드 이벤트 등을 갖지 못하기 때문에,
                이러한 멤버를 동적으로 할당하여 dynamic 타입에서 추가할 수 없다.
                두번째는 만약 Class2가 동일한 어셈블리가 아닌 다른 어셈블리에 놓인다면,
                이 코드는 에러(object does not contain a definition for Name)를 발생시킨다.

                이는 코드에서 dynamic 타입이 익명타입(underlying type)인데,
                다른 어셈블리에서는 이 익명타입을 볼 수 없기 때문이다.
                이러한 제약점은 모두 아래의 ExpandoObject 클래스를 사용하여 해결할 수 있다.


                    // 동일 어셈블리에서 익명타입에 dynamic 사용한 경우
                    class Class1
                    {
                        public void Run()
                        {
                            dynamic t = new { Name = "Kim", Age = 25 };

                            var c = new Class2();        
                            c.Run(t);
                        }
                    }

                    class Class2
                    {
                        public void Run(dynamic o)
                        {
                            // dynamic 타입의 속성 직접 사용
                            Console.WriteLine(o.Name);
                            Console.WriteLine(o.Age);
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void ExpandoObject_use()
        {
            /*
                DRL (Dynamic Runtime Language) 네임스페이스인 System.Dynamic 에는 2개의 중요한 클래스가 있는데,
                개발자가 dynamic 타입을 쉽게 생성하도록 도와주는 ExpandoObject 클래스와 보다
                유연한 Customization을 위한 고급 dynamic 기능을 지원하는 DynamicObject 클래스가 그것이다.
                여기서 일반적으로 많이 사용되는 ExpandoObject 클래스는 dynamic 타입에 속성, 메서드, 이벤트를
                동적으로 쉽게 할당할 수 있게 도와주는 클래스이다.

                사용법은 먼저 ExpandoObject 클래스로부터 객체를 생성한 후 이를 C# dynamic 변수에 할당한다.
                이후 이 dynamic 타입의 변수에서 새로운 속성, 메서드 혹은 이벤트를 할당하면 된다.
                메서드 할당은 델리게이트를 할당하는데,
                예를 들어 아래 예제와 같이 람다식을 Delegate로 캐스팅하는 방식 등을 사용할 수 있다.

                이벤트는 먼저 이벤트 필드를 NULL로 초기화 한 후 사용한다.
                이벤트는 += 혹은 -= 연산자를 써서 이벤트 핸들러를 추가 혹은 제거할 수 있다.
                다른 일반 타입과 마찬가지로 dynamic 타입은 파라미터로 타 메서드에 전달할 수 있으며,
                ExpandoObject 클래스로부터 생성된 dynamic 타입은 다른 어셈블리로 전달되는 경우에도 문제 없이 사용할 수 있다.

                    public void M1()
                    {
                        // ExpandoObject에서 dynamic 타입 생성
                        dynamic person = new ExpandoObject();

                        // 속성 지정
                        person.Name = "Kim";      
                        person.Age = 10;

                        // 메서드 지정
                        person.Display = (Action)(() =>
                        {
                            Console.WriteLine("{0} {1}", person.Name, person.Age);
                        });

                        person.ChangeAge = (Action<int>)((age) => { 
                            person.Age = age;
                            if (person.AgeChanged != null)
                            {
                                person.AgeChanged(this, EventArgs.Empty);
                            }
                        });

                        // 이벤트 초기화
                        person.AgeChanged = null; //dynamic 이벤트는 먼저 null 초기화함

                        // 이벤트핸들러 지정
                        person.AgeChanged += new EventHandler(OnAgeChanged);

                        // 타 메서드에 파라미터로 전달
                        M2(person);
                    }

                    private void OnAgeChanged(object sender, EventArgs e)
                    {
                        Console.WriteLine("Age changed");
                    }

                    // dynamic 파라미터 전달받음
                    public void M2(dynamic d)
                    {
                        // dynamic 타입 메서드 호출 
                        d.Display();
                        d.ChangeAge(20);
                        d.Display();
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void ExpandoObject_with_dynmamic()
        {
            /*
                ExpandoObject 클래스는 동적으로 추가되는 멤버들을 내부 해시테이블에 저장하고 있는데,
                필요한 경우 이 정보를 IDictionary<String, Object> 인터페이스를 통해 쉽게 엑세스할 수 있다.
                즉, ExpandoObject 클래스 자체가 IDictionary<String, Object> 인터페이스 구현하고 있어서
                이 클래스 객체를 IDictionary<String, Object> 인터페이스로 캐스팅하여 내부 멤버 데이타를 엑세스할 수 있다.
                아래 예제는 동적으로 속성, 메서드, 이벤트를 추가하고 이들을 IDictionary 인터페이스를 통해 출력해 보는 코드이다.
            */
            {
                dynamic person = new ExpandoObject();
                person.Name = "Kim";
                person.Age = 10;
                person.Display = (Action)(() => { });
                person.ChangeAge = (Action<int>)((age) => { person.Age = age; });
                person.AgeChanged = null;
                person.AgeChanged += new EventHandler((s, e) => { });

                // ExpandoObject를 IDictionary로 변환
                var dict = (IDictionary<string, object>)person;

                // person 의 속성,메서드,이벤트는
                // IDictionary 해시테이블에 저정되어 있는데
                // 아래는 이를 출력함
                foreach (var d in dict)
                {
                    Console.WriteLine("{0}: {1}", d.Key, d.Value);
                }

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //ExpandoObject_with_dynmamic();

            //ExpandoObject_use();

            //anonymous_type_with_dynamic();

            //dynamic_keyword();

            //static_and_dynamic_language_concept();
        }
    }
}
