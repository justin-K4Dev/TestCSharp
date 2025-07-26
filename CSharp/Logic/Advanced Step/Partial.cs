using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class Partial
    {
        static void Partial_type_what()
        {
            /*
                C# 2.0에서 partial 키워드가 도입되어 Partial Class, Partial Struct, Partial Interface 를 사용할 수 있게 되었다.
                기본적으로 이들 Partial 타입들이 만든어진 이유는 Code Generator가 만든 코드와 사용자가 만드는 코드를 분리하기 위함이다.
                예를 들어, 윈폼에서는 Form UI 디자인과 관련된 Form1.designer.cs 파일과 사용자가 쓰는 Form1.cs 파일에 동일한 클래스명을 두고
                이를 partial로 선언하는데, 컴파일러는 나중에 이를 합쳐 하나의 클래스로 만든다.

                또한, ASP.NET 웹폼에서는 하나의 웹페이지를 만들 때, page1.aspx, page1.aspx.designer.cs, page1.aspx.cs와 같이 3개의 파일을 만드는데,
                XML인 page1.aspx 이외의 .cs 파일 안에는 윈폼과 마찬가지고 partial 클래스를 사용하고 있다.
                웹 개발자는 Code Behind라 불리우는 page1.aspx.cs 파일에서 주로 작업한다.
                이러한 Partial 기능은 개발자에게 포커스 해야하는 코드를 분리해 준다는 점에 크게 도움이 된다.

                아래 예제는 C# partial 키워드를 사용한 코드들인데, partial 키워드는 class, struct, interface 키워드 바로 앞에 위치해야 하며,
                2개 이상의 파일에 걸쳐 Type을 나눌 수 있게 한다.
                예제에서 처음의 partial class는 하나의 클래스를 3개로 분리한 예이며, 두번째의 partial struct는 struct를 2개로 분리한 예이다
                (비록 아래와 같이 필드를 분리하는 것도 가능하지만, 필드끼리 한군데 모아 두는 것이 권장사항이다).
                세번째는 partial interface를 2개로 분리한 경우인데 이 interface의 두 멤버를 구현한 클래스의 예제를 함께 보여주고 있다.


                    // 1. Partial Class - 3개로 분리한 경우
                    partial class Class1
                    {
                        public void Run() { }
                    }

                    partial class Class1
                    {
                        public void Get() { }
                    }

                    partial class Class1
                    {
                        public void Put() { }
                    }

                    // 2. Partial Struct
                    partial struct Struct1
                    {
                        public int ID;
                    }

                    partial struct Struct1
                    {
                        public string Name;

                        public Struct1(int id, string name)
                        {
                            this.ID = id;
                            this.Name = name;
                        }
                    }

                    // 3. Partial Interface
                    partial interface IDoable
                    {
                        string Name { get; set; }
                    }

                    partial interface IDoable
                    {
                        void Do();
                    }

                    // IDoable 인터페이스를 구현
                    public class DoClass : IDoable
                    {
                        public string Name { get; set; }

                        public void Do()
                        {
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void Partial_method_what()
        {
            /*
                C# 3.0에서는 C# 2.0에서 소개된 3개의 partial type들 이외에 새로운 partial 기능을 하나 추가하였다.
                즉, partial 기능을 Type이 아닌 메서드에 적용한 Partial Method 기능이다.
                Partial Method는 그 전제 조건으로 메서드가 반드시 Private 메서드이어야 하고, 리턴 값이 없어야(void) 한다.

                사용하는 방법은 아래 예제에서 보이듯이, 첫번째 파일(or 클래스)에 void DoThis(); 와 같이 메서드 Body가 없이 메서드 선언부만 적는다.
                그리고 구현 파일에서 DoThis() { ... } 와 같이 실제 메서드를 구현한다.
                여기서 한가지 주목한 점은 만약 두번째 실제 메서드 구현이 생략되고 메서드 선언부만 있게 된다면,
                C# 컴파일러는 컴파일시 DoThis() 전체를 없애 버린다는 것이다.
                즉, 특정 메서드가 다른 파일에서 구현되었는가의 여부에 따라 메서드 전체를 생략할 수 있는 기능을 제공하는 것이다.


                    // Partial Method (C# 3.0)
                    public partial class Class2
                    {
                        public void Run()
                        {
                            DoThis();
                        }

                        // 조건1: private only
                        // 조건2: void return only
                        partial void DoThis();
                    }

                    public partial class Class2
                    {
                        partial void DoThis()
                        {
                            Log(DateTime.Now);
                        }
                    }                    
            */
            {
                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //Partial_method_what();

            //Partial_type_what();
        }
    }
}
