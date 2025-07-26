using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Namespace
    {
        static void namespace_what()
        {
            /*
                .NET Framework은 무수하게 많은 클래스들을 가지고 있는데, 예를 들면 .NET 4.0은 약 11,000개의 클래스를 가지고 있다.
                이렇게 많은 클래스들을 충돌없이 보다 편리하게 관리/사용하기 위해 .NET에서 네임스페이스를 사용한다.
                C#에서도 이러한 개념을 적용하여 클래스들이 대개 네임스페이스 안에서 정의된다.
                비록 클래스가 네임스페이스 없이도 정의 될 수는 있지만, 거의 모든 경우 네임스페이스를 정의하는 것이 일반적이다.

                    namespace MyNamespace
                    {
                        class A
                        {
                        }
                        class B
                        {
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void namespace_reference()
        {
            /*
                네임스페이스를 사용하기 위해서는 두가지 방식이 있다.
                첫째는 클래스명 앞에 네임스페이스 전부를 적는 경우와
                둘째는 프로그램 맨 윗단에 해당 using을 사용하여 C# (.cs) 파일에서 사용하고자 하는 네임스페이스를
                한번 설정해 주고, 이후 해당 파일 내에서 네임스페이스 없이 직접 클래스를 사용하는 경우이다.
                실무에서는 주로 두번째 방식을 택한다. 
            */
            {
                System.Console.WriteLine();

                //두번째 방식
                //using System; //System 네임스페이스 지정

                Console.WriteLine();

                Console.ReadLine();
            }
        }


        static void example()
        {
            int a = -100, b = -900;
            int abs_Sum = System.Math.Abs(a) + Math.Abs(b);

            /*
                클래스 Class1은 MySystem.MySubSystem 이라는 네임스페이스 안에서 정의되어 있다.
                네임스페이스는 계층적으로 구성될 수 있다.
                System.Math.Abs(a)은 사용하고자 하는 클래스명앞에 해당 네임스페이스를 적은 경우이고,
                Math.Abs(b)은 네임스페이스를 맨 윗단에 using System; 으로 정의해 준 경우이다.
                다른 클래스에서 위의 Class1을 사용하려면, 해당 DLL을 참조 추가하고 usng MySystem.MySubSystem;
                을 파일 상단에 적어 주면 된다.
            */
            {
                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //example();

            //namespace_reference();
            
            //namespace_what();
        }
    }
}
