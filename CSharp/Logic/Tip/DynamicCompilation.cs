using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Diagnostics;


namespace Tip
{
    public class DynamicCompilation
    {
        static void dynamic_build_what()
        {
            /*
                .NET에서 제공하는 컴파일 서비스를 이용하면 C#이나 VB.NET 코드 등을 컴파일해서 .EXE 나 .DLL 등을 동적으로 빌드할 수 있다.
                .NET의 CodeDomProvider 클래스는 이러한 동적 컴파일 (Dynamic Compilation) 기능을 제공하는데,
                CodeDomProvider.CreateProvider() 를 사용하여 해당 언어에 대한 컴파일러 객체를 생성하고,
                필요한 컴파일 옵션(예를 들어 EXE를 만드는지 DLL을 만드는지 등)을 지정하게 된다.
                실제 컴파일 빌드를 하는 메서드는 직접 문자열로부터 컴파일하는 CompileAssemblyFromSource(),
                파일로부터 컴파일하는 CompileAssemblyFromFile(), 그리고 DOM 트리로부터 컴파일하는 CompileAssemblyFromDom() 등이 있다.
                아래 예제는 C# 코드가 들어 있는 문자열로부터 컴파일 빌드하여 TEST.EXE라는 실행파일을 만드는 간단한 예이다. 
            */
            {
                //컴파일할 코드
                string code = @"
                    using System; 
                    namespace TEST 
                    {
                        class Program
                        {
                            static void Main(string[] args)
                            {
                                int sum = 0;
                                for (int i = 0; i < 100; i++)
                                {
                                    sum += i;
                                }
                                Console.WriteLine(sum);
                                Console.ReadLine();
                            }
                        }
                    }
                    ";

                //C# 컴파일러 객체 생성
                CodeDomProvider codeDom = CodeDomProvider.CreateProvider("CSharp");

                //컴파일러 파라미터 옵션 지정
                CompilerParameters cparams = new CompilerParameters();
                cparams.GenerateExecutable = true;
                cparams.OutputAssembly = "TEST.EXE";

                //소스코드를 컴파일해서 EXE 생성
                CompilerResults results = codeDom.CompileAssemblyFromSource(cparams, code);

                //컴파일 에러 있는 경우 표시
                if (results.Errors.Count > 0)
                {
                    foreach (var err in results.Errors)
                    {
                        Console.WriteLine(err.ToString());
                    }
                    return;
                }

                //(Optional) 테스트 실행
                Process.Start("TEST.EXE");

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //dynamic_build_what();
        }
    }
}
