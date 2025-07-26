using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;


public class AutoPropertyInitializer
{
    class MyClass
    {
		// 자동 속성 초기화
		public string Name { get; set; } = "(No name)";

		// 초기화 없을 경우 디폴드값 사용됨
		public string Nickname { get; }
		public int Age { get; }

		// Auto-Property Initializer 할당
		public bool Enabled { get; } = true;

		// 생성자에서 초기값 할당
		public int Level { get; }
	}

    static void auto_property_initializer_what()
    {
        /*
            자동 속성(Auto-Property)은 개발자가 필드를 지정할 필요가 없이 C# 컴파일러가 자동으로 해당 필드를 만들어 주는 속성(Property)이다.
            예를 들어 개발자가 public string Name { get; set; } 과 같이 써 주면,
            Name이라는 속성이 실제로 읽고 쓰는 필드(backing field)를 컴파일러가 자동으로 만들어 주게 된다.
            이러한 자동 속성 기능은 기존에 지원하던 기능인데, C# 6.0에서는 이 자동 속성에 초기값을 지정하는 기능을 추가하였다.
            이를 Auto-Property Initializer라 부르는데, 문법적으로는 Auto-Property 정의 뒤에 = 을 붙이고 초기값을 설정해 준다.                
        */
        {
            var new_may_class = new MyClass();

				Console.WriteLine(new_may_class.Name);

            Console.ReadLine();
        }
    }


    static void auto_property_initializer_read_only()
    {
        /*
            C# 6.0 이전의 자동 속성은 항상 get 과 set을 함께 사용하였다.
            외부에 읽기만 허용할 경우 { get; private set; } 과 같이 set 엑세스를 제한하기도 하였지만, 
            어쨌든 set을 생략해서는 안되었다.
            하지만, 이제 C# 6.0 에서는 읽기전용의 자동 속성을 쓸 수 있게 되었다.
            즉 get 만을 사용할 수 있게 된 것이다.
            이러한 읽기전용 자동속성이 도입되면서 이전에 복잡하게 구현되었던
            Immutable Property (변경이 불가능한 속성)을 간단히 구현할 수 있게 되었다.

            그런데, 만약 읽기전용 자동 속성에 값을 할당하지 않으면 어떻게 될까? 이런 경우 컴파일러는 자동으로 default 값을 할당한다.
            따라서, 아래 예제에서 Nickname과 Age의 값은 디폴트 값을 출력하게 된다.

            물론 읽기전용 자동속성에 초기값을 할당할 수 있다. 이는 위에서 말한 Auto-Property Initializer를 사용하거나
            클래스 생성자에서 해당 속성에 값을 할당하면 된다.
            아래 예제에서 Enabled와 Level 속성은
            각각 Auto-Property Initializer와 클래스 생성자에서 그 값을 초기화하고 있다.
        */
        {
            var my_class = new MyClass();

				Console.WriteLine(my_class.Name);
				Console.WriteLine(my_class.Nickname); // null 출력
				Console.WriteLine(my_class.Age); //  0 출력

				Console.ReadLine();
        }
    }


    public static void Test()
    {
        //auto_property_initializer_read_only();

        //auto_property_initializer_what();
    }
}
