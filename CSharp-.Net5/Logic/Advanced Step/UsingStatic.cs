using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// using static 사용
using static System.Console;




namespace AdvancedStep;

public class UsingStatic
{
    static void using_static_what()
    {
        /*
            정적 멤버 직접 사용 (클래스명 생략)

            지금까지의 C#에서 static 메서드(혹은 속성)를 사용하기 위해서는 클래스명.메서드명 (혹은 클래스명.속성명) 과 같이
            클래스명을 반드시 앞에 지정해 주었다.

            C# 6.0에서는 using static 클래스명을 써 준다면,
            해당 C# 파일 내에서는 해당 클래스명 없이 메서드를 직접 사용할 수 있게 하였다.
            예를 들어 아래 예제에서 처럼 처음 System.Console 클래스를 using static 과 함께 지정한 후에는
            본문에서 Console.WriteLine() 대신 WriteLine()을 직접 사용할 수 있다.
        */
        {
			// Console. 생략 가능 
			WriteLine("csharpstudy.com");

			Console.ReadLine();
        }
    }


    public static void Test()
    {
        //using_static_what();
    }
}
