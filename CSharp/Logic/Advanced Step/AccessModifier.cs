using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class AccessModifier
    {
        static void access_modifier_what()
        {
            /*
                📄 Access Modifier 
            
                  - 접근 제한자는 외부로부터 클래스 혹은 클래스 멤버들(메서드, 속성, 이벤트, 필드)로의 접근을 제한할 때
                    사용하는 것으로 다음과 같은 종류가 있다. 

                    | Access Modifier   |  Desc
                    |-------------------|------------------------------------------------------------------------------
                    | public	        | 모든 외부 혹은 파생 클래스에서 이 클래스를 엑세스할 수 있다.
                    |                   | (개별 멤버의 엑세스 권한은 해당 멤버의 접근 제한자에 따라 별도로 제한된다)
                    | internal	        | 동일한 Assembly 내에 있는 다른 클래스들이 엑세스 할 수 있다.
                    |                   | 다른 어셈블리에서는 접근 불가.
                    | protected	        | 파생클래스에서 이 클래스 멤버를 엑세스할 수 있다.
                    | private	        | 동일 클래스 내의 멤버만 접근 가능하다.

                접근 제한자는 public class A {} 와 같이 클래스 앞에 사용하거나 메서드,
                속성 등의 클래스멤버 앞에 사용하여 (예: protected int GetValue(); ) 접근을 제한하게 된다.
            */
            {
                Console.ReadLine();
            }
        }

        static void access_modifier_use()
        {
            /*
                클래스의 필드는 기본적으로 private으로 설정하여 외부로터의 접근을 완전히 제한하는 것이 일반적이다
                (객체 지향 프로그래밍의 원칙에 따라).
                메서드는 외부에서 호출해야 하는 것은 public으로 하고 내부에서만 사용되는 것은 private으로 설정한다.
                메서드는 이외에도 어셈블리 내부에서만 사용 가능한 internal, 파생클래스에서 엑세스할 수 있는 protected 를 사용할 수 있다.
                속성은 메서드와 동일한 방식으로 설정한다.

                다음은 접근 제한자를 사용하는 예제이다.

                    internal class MyClass
                    {
                        private int _id = 0; 

                        public string Name { get; set; }

                        public void Run(int id) { }

                        protected void Execute() { }
                    }

                    * MyClass는 internal 클래스로서 동일 어셈블리 내에서만 접근 가능하다.
                    * _id 는 private 필드로서 클래스 내부에서만 사용 가능하다 .
                    * Name은 public 프로퍼티로서 클래스를 엑세스할 수 있는 모든 외부 객체에서 접근 가능하다.
                    * Run()은 public 메서드로서 클래스를 엑세스할 수 있는 모든 객체에서 접근 가능하다.
                    * Execute()는 protected 메서드로서 해당 MyClass와 이의 파생 클래스에서만 접근 가능하다.
            */
            {
                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //access_modifier_use();

            //access_modifier_what();
        }
    }
}
