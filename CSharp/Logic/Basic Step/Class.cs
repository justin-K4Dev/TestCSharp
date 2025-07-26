using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BasicStep
{
    public class MyCustomer
    {
        // 필드
        private string name;
        private int age;

        // 이벤트 
        public event EventHandler NameChanged;

        // 생성자 (Constructor)
        public MyCustomer()
        {
            name = string.Empty;
            age = -1;
        }

        // 속성
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    if (NameChanged != null)
                    {
                        NameChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        public int Age
        {
            get { return this.age; }
            set { this.age = value; }
        }

        // 메서드
        public string GetCustomerData()
        {
            string data = string.Format("Name: {0} (Age: {1})",
                        this.Name, this.Age);
            return data;
        }
    }
    /*
        * 위의 예는 public class 이므로 모든 객체로부터 접근 가능하다. 만약 internal class 이면 해당 어셈블리 내에서만 접근 가능.
        * 클래스 생성자(Constructor)는 클래스로부터 객체가 만들어 질때 호출되는 것으로 주로 필드를 초기화 하는데 사용한다.
        * 클래스에 어떤 필드, 메서드, 프로퍼티를 만들 것인가는 주로 업무 분석을 통해 해당 클래스의 역할에 따라 결정된다. 
    */

    public class Class
    {
        static void class_what()
        {
            /*
                C# class 키워드는 Reference Type을 정의하는데 사용된다.
                클래스는 메서드 (Method), 속성 (Property), 필드 (Field), 이벤트 (Event) 등을 멤버로 포함하는 소프트웨어 단위로서
                보통 이 클래스 정의로부터 객체 (Object)를 생성해서 사용하게 된다.
                클래스를 정의할 때 중요한 멤버는 공용(public) 메서드와 속성인데,
                이 public 멤버들은 외부 객체와의 상호작용을 위해 사용되어 진다.

                    class member 종류	        설명

                    메서드 (Method)	            클래스에서 실제 행동을 일으키는 코드 블럭.
                                                대개 동사 혹은 동사+명사 식으로 메서드명을 정함.
                                                예) Calculate(), DeleteData()
                    프로퍼티 (Property)	        클래스의 내부 데이타를 외부에서 사용할 수 있게 하거나,
                                                외부에서 클래스 내부의 데이타를 간단하게 설정할 때 사용한다.
                    필드 (Field)	                클래스의 내부 데이타는 필드에 저장하게 되며,
                                                필드들은 클래스 객체의 상태를 유지하는데 이용된다.
                                                필드는 접근제한자(Access Modifier)에 따라 외부 객체 혹은 상속 객체에서 보여질 수 있다.
                                                (public 필드를 만들어 문법적으로 필드를 외부에 노출할 수는 있지만,
                                                 이는 객체 지향 프로그래밍 방식에 어긋난다.
                                                 이 경우 주로 private 필드를 만들고 public 프로퍼티를 이용해 필드값을 외부에 전달하는 방식을 사용한다)
                    이벤트 (Event)	            이벤트는 객체 내부의 특정 상태 혹은 이벤트를 외부로 전달하는데 이용되는데,
                                                예를 들어 버튼 클래스의 경우 버튼이 클릭되었으면,
                                                버튼클릭 이벤트에 가입한 모든 외부 객체들에게 그 사실을 통보(casting)하는 것이다.
            */
            {
                Console.ReadLine();
            }

            /*
                클래스 정의는 class 라는 C# 키워드를 사용해서 정의한다. class 키워드 뒤에 클래스명을 써 주고,
                클래스 블럭안에 필드,메서드,속성,이벤트 등을 정의해 준다.
                클래스 각 멤버들은 public, protected, private 등의 접근제한자(Access Modifier)에 따라
                외부 객체로부터 접근이 허용될 수도 있고 제한될 수도 있다.
            */
            {
                MyCustomer customer = new MyCustomer(); 

                Console.ReadLine();
            }
        }


        static void partial_what()
        {
            /*
                C# 2.0에서부터 Partial 클래스라는 개념이 도입되었다.
                이는 하나의 클래스를 2개 이상의 파일에 나누어 정의할 수 있는 기능이다.

                Visual Studio에서 Windows Form을 만들면 자동으로 동일 클래스를 2개의 파일에 나누어 저장한다.
                예를 들어, Form1을 생성하면 Form1.cs와 Form1.designer.cs 파일이 생성된다.
                이러한 2개 파일에 동일한 클래스의 필드, 메서드, 속성 등을 나누어 저장하기 위해 partial class를 이용한다.

                Partial 클래스를 정의하기 위해서는 C# 키워드 partial을 class 앞에 써주면 된다.
                (좀 더 자세한 내용은 C#의 partial 키워드 설명을 참조)

                    public partial class Form1 { ... }
            */
            {
                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //partial_what();

            //class_what();
        }
    }
}
