using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class HelloWorld
    {
        static void hello_world()
        {
            System.Console.WriteLine("Hello World !!!");

            /*
                * 모든 C# 프로그램은 Main()이라는 시작 함수(메서드)를 가져야 한다.
                  Main() 메소드는 임의의 클래스 안에서 존재하며, 프로그램 상에 단 1개만 있어야 한다.

                * Main()는 static으로 선언되며, 메소드 인자는 string[] 문자열이다.

                * System.Console은 .NET Framework 클래스이며,
                  WriteLine은 화면에 데이타를 Console클래스의 출력하는 메서드이다.
            */

            Console.ReadLine();
        }

        public static void Test()
        {
            //hello_world();
        }
    }
}
