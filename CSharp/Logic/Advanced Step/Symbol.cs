using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class Symbol
    {
        static void symbol_non_escape()
        {
            /*
                @ 심벌을 문자열 앞에 사용하면, 해당 문자열 안의 Escape 문자를 무시하고 문자 그대로 인식하도록 한다.
                예를 들어, 파일 패스를 지정할 때, Backslash를 한번 지정하면 이는 Escape문자로 인식되기 때문에
                2개의 Backslash를를 사용하게 되는데, @ 심벌을 문자열 시작 부호전에 사용하면,
                Backslash를 그대로 Backslash를로 인식하게 한다.
            */
            {
                string filename1 = "C:\\Temp\\1.txt";
                Console.WriteLine(filename1); // 출력: C:\Temp\1.txt


                // 특수문자 출력하기
                var special_character = "!@#$%^&*(\\'\"<>?:;";
                Console.WriteLine(special_character); // 출력: !@#$%^&*(\'"<>?:;


                // @심벌을 사용하여 보다 자연스럽게 패스 지정
                string filename2 = @"C:\Temp\1.txt";
                Console.WriteLine(filename2);

                
                Console.ReadLine();
            }
        }


        static void symbol_multi_line_string()
        {
            /*
                한 문자열 변수에 여러 줄의 문자열을 지정하는 경우에 @ 심벌을 사용하면 편리하다.
                물론 여러 문자열들을 + 로 연결하여 사용할 수도 있지만,
                아래와 같이 @ 심벌을 문자열 앞에 두면 복수 행의 문자열들을 갖는 문자 데이타를 지정할 수 있다. 
            */
            {
                string code = @"
                    public string ReadFile(string filename)
                    {
                        if (!string.IsNullOrEmpty(filename))
                        {
                            return File.ReadAllText(filename);
                        }
                        return string.Empty;
                    }
                    ";

                Console.WriteLine(code);

                Console.ReadLine();
            }
        }


        static void symbol_variable_name()
        {
            /*
                마지막으로 @ 심벌은 C# 키워드 앞에 붙여 일반 변수명으로 사용할 때 유용하게 활용된다.
                아래 예제 3-1에서 보이듯이, object는 C# 키워드 이므로 string object = "객체" 와 같이 쓸 수 없다.
                즉, 이렇게 하면 컴파일 에러가 발생할 것이다.
                하지만 object C# 키워드 앞에 @ 사인을 붙이면 변수명으로 사용할 수 있다.
                하지만, 이렇게 사용할 수 있다고 해서 굳이 변수명을 object로 사용하도록 권장하는 사람은 아무도 없다.
                오히려 반대로 대부분 C# 키워드는 변수명으로 사용하지 말도록 권장한다.
                그렇다면 이 기능은 왜 필요할까? 아래 예제 3-2를 살펴보자.
                이는 ASP.NET MVC의 Html Helper의 예를 보여주는데, ActionLink 메서드의 4번째 파라미터를 보면,
                익명타입 (Anonymous Type)의 객체를 생성한 후 Html Attribute와 관련된 속성들을 지정하고 있고 있다.
                그런데 여기서 class는 C#의 키워드 이므로 변수명으로 사용할 수 없는데,
                실제 Html 속성 class는 이미 표준에 의해 지정된 것이므로 @class와 같이 사용해야만 한다.
            */
            {
                // object라는 변수명 지정
                string @object = "객체";

                @object = @object + "1";
                Console.WriteLine(@object);

                Console.ReadLine();
            }
            /*
                // ASP.NET MVC Html Helper
                @Html.ActionLink("Goto Menu", "Menu", null,
                    new { @class="linkStyle", target="_blank"});


                <A href="/Home/Menu" class="linkStyle" target="_blank">
                    Goto Menu
                </A>
            */
        }


        public static void Test()
        { 
            //symbol_variable_name();

            //symbol_multi_line_string();

            //symbol_non_escape();
        }
    }
}
