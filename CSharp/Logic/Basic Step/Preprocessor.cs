#define TEST_ENV
//#define PROD_ENV

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BasicStep
{
    public class Preprocessor
    {
        static void preprocessor_directive()
        {
            /*
                C# 전처리기 지시어 (Preprocessor Directive)는 실제 컴파일이 시작되기 전에 컴파일러에게 특별한 명령을 미리 처리하도록 지시하는 것이다.
                모든 C# 전처리기 지시어는 # 으로 시작되며, 한 라인에 한 개의 전처리기 명령만을 사용한다.
                전처리기 지시어는 C# Statement가 아니기 때문에 끝에 세미콜론(;)을 붙이지 않는다.

                전처리기 지시어는 해당 파일 안에서만 효력을 발생한다.
                만약 하나의 클래스가 두개의 파일에 나뉘어 Partial Class로 저장되었을 때,
                두개의 파일에서 동일한 심벌(#define으로 정의)을 사용하고 싶다면, 두개의 파일에 각각 심벌을 정의해야 한다.
                C# 전처리기는 C/C++와 달리 별도의 Preprocessor를 갖지 않고 컴파일러가 Preprocessor Directive를 함께 처리한다.
            */
            {
                Console.ReadLine();
            }
        }


        static void conditional_compile()
        {
            /*
                C# 전처리기에서 자주 사용되는 것으로 #define과 #if ... #else ... #endif 가 있다.
                #define은 심벌을 정의할 때 사용하는데,
                예를 들어, #define DEBUG 혹은 #define RELEASE와 같이 하나의 심벌을 정의한다.
                이렇게 정의된 심벌은 다른 전처리기 지시어에서 사용되는데,
                예를 들어 #if (DEBUG)와 같이 if 지시어에서 사용될 수 있다.

                흔히 #define과 #if를 결합하여 조건별로 서로 다른 코드 블럭을 컴파일하도록 할 수 있다.
                아래 코드는 TEST_ENV 라는 심벌을 정의되었지를 보고 서로 다른 코드 블럭을 컴파일하는 예를 보여주고 있다.
                Visual Studio에서는 조건별로 컴파일 되지 못하는 영역은 회색으로 표시해 준다.
            */
            {
                bool verbose = false;
                // ...

#if (TEST_ENV)
                Console.WriteLine("Test Environment: Verbose option is set.");
                verbose = true;
#else
                    Console.WriteLine("Production");
#endif
                if (verbose)
                {
                    //....
                }

                Console.ReadLine();
            }
            /*
                TEST_ENV 심벌이 정의되어 있으므로 콘솔에 Test Environment를 출력하고 verbose 에 true를 할당한다
            */
        }


        static void region_preprocessor_directive()
        {
            /*
                #region은 코드 블럭을 논리적으로 묶을 때 유용하다.
                예를 들어, Public 메서드들만 묶어 [Public Methods]라고 명명할 수 있고,
                Private 메소드들을 묶어 [Privates] 라고 명명할 수 있다.

                #region은 #endregion과 쌍을 이루며 한 영역을 형성한다.
                #region 안에 다른 Nested Region을 둘 수도 있다.
                Visual Studio에서 #region 영역은 좌측에 +/- 로 표시되어 Expand/Collapse를 할 수 있는 기능을 제공한다.
                이 전처리기 지시어는 실제 가장 많이 사용되는데,
                특히 복잡하고 긴 클래스를 개념적으로 묶을 때 매우 유용하다.

                    class ClassA
                    {
                        #region Public Methods        
                        public void Run() { }
                        public void Create() { }        
                        #endregion

                        #region Public Properties
                        public int Id { get; set; }
                        #endregion

                        #region Privates
                        private void Execute() { }
                        #endregion
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void etc_preprocessor_directive()
        {
            /*
                #undef 는 #define과 반대로 지정된 심벌을 해제 할 때 사용한다.
                #elif 는 #if와 함께 사용하여 else if를 나타낸다.
                #line 은 거의 사용되진 않지만, 라인번호를 임의로 변경하거나 파일명을 임의로 다르게 설정할 수 있게 해준다.
                #error 는 전처리시 Preprocessing을 중단하고 에러 메시지를 출력하게 한다.
                #warning 은 경고 메서지를 출력하지만 Preprocessing은 계속 진행한다.
                warning과 error는 특정 컴포넌트가 어떤 환경에서 실행되지 않아야 할 때 이를 경고나 에러를 통해 알리고자 할 때 사용될 수 있다. 

                아래 코드는 (1) 첫번째 예제는 Enterprise Edition이 아닌 경우 경고 메시지를 내는 것을 보여주고,
                (2) 두번째 예제는 Edition 심벌이 복수로 지정된 경우에 에러를 내게 하는 케이스를 예시하고 있다.
                만약 여러 개의 파일들을 병합한 후, 컴파일을 해야하는 경우 이러한 에러 체크가 유용할 수 있다.


                    // #warning 예제 -----------------------------------
                    #if (!ENTERPRISE_EDITION)
                    #warning This class should be used in Enterprise Edition
                    #endif

                    namespace App1 {
                        class EnterpriseUtility {
                        }
                    }

                    // #error 예제 --------------------------------------
                    #define STANDARD_EDITION
                    #define ENTERPRISE_EDITION

                    #if (STANDARD_EDITION && ENTERPRISE_EDITION)
                    #error Use either STANDARD or ENTERPRISE edition. 
                    #endif

                    namespace App1 {
                        class Class1 {
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void pragma_preprocessor_directive()
        {
            /*
                #pragma 지시어는 위의 표준 C# 전처리기 지시어와 다르게,
                컴파일러 제작업체가 고유하게 자신들의 것을 만들어 사용할 수 있는 지시어이다.
                즉, 어떤 컴파일러를 쓰느냐에 따라 지원되는 #pragma가 서로 다르며, 개발자가 임의로 지정하여 사용할 수 없다.

                MS의 C# 컴파일러는 현재 #prama warning과 #pragma checksum 2개를 지원하고 있다.
                #prama warning는 경고메서지를 Disable/enable 하게 할 수 있으며,
                #pragma checksum는 주로 ASP.NET 페이지 디버깅을 위해 만들어진 것으로 ASPX 페이지의 파일 체크섬을 생성할 때 사용된다.

                아래 예제는 #pragma warning을 사용하여 컴파일시 CS3021 경고를 Disable하는 예를 보여준다.


                    // CS3021 Warning을 Disable
                    #pragma warning disable 3021

                    namespace App1
                    {
                        [System.CLSCompliant(false)] 
                        class Program
                        {
                            static void Main(string[] args)
                            {
                            }
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //pragma_preprocessor_directive();

            //etc_preprocessor_directive();

            //region_preprocessor_directive();

            //conditional_compile();

            //preprocessor_directive();
        }
    }
}
