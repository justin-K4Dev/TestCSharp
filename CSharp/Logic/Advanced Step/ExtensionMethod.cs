using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    // static class를 정의
    public static class ExClass
    {
        // static 확장메서드를 정의. 첫번째 파라미터는
        // 어떤 클래스가 사용할 지만 지정. 
        public static string ToChangeCase(this String str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ch in str)
            {
                if (ch >= 'A' && ch <= 'Z')
                    sb.Append((char)('a' + ch - 'A'));
                else if (ch >= 'a' && ch <= 'x')
                    sb.Append((char)('A' + ch - 'a'));
                else
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        // 이 확장메서드는 파라미터 ch가 필요함
        public static bool Found(this String str, char ch)
        {
            int position = str.IndexOf(ch);
            return position >= 0;
        }
    }

    public class ExtensionMethod
    {
        static void extension_method()
        {
            /*
                C# 3.0부터 지원하는 확장메서드(Extension Method)는 특수한 종류의 static 메서드로서
                마치 다른 클래스의 인스턴스 메서드인 것처럼 사용된다.
                일반적으로 instance 메서드를 추가하기 위해서는 해당 클래스안에 메서드를 추가한다.
                만약 여러 개의 클래스들에 instance 메서드를 추가하고 싶다면,
                각 클래스마다 메서드를 추가해 주어야 한다 (물론 Base Class가 있는 경우, 클래스 상속을 이용할 수도 있다). 

                확장 메서드는 메서드가 사용될 클래스명(혹은 Type)을 첫번째 파라미터로 지정하여
                마치 해당 클래스(혹은 Type)가 확장메서드를 인스턴스 메서드로 갖는 것과 같은 효과를 낸다.
                약간 특이한 문법이지만, 확장 메서드의 첫번째 파라미터는 앞에 항상 this를 써준다.

                아래 예제는 String 클래스를 첫번째 파라미터로 갖는 확장메서드
                즉 String 클래스에서만 동작하는 확장 메서드를 정의한 예이다.
            */
            {
                string s = "This is a Test";

                // s객체 즉 String객체가
                // 확장메서드의 첫 파리미터임
                // 실제 ToChangeCase() 메서드는
                // 파라미터를 갖지 않는다.
                string s2 = s.ToChangeCase();

                // String 객체가 사용하는 확장메서드이며
                // z 값을 파라미터로 사용
                bool found = s.Found('z');

                Console.ReadLine();
            }
        }


        static void extension_method_with_Enumerable()
        {
            /*
                System.Linq.Enumerable 클래스는 LINQ 쿼리에서 사용되는 많은 확장 메서드들을 포함하는 클래스이다.
                한 예를 들어, Enumerable 클래스는 다음과 같은 Where() 확장메서드를 포함하고 있다.
                첫번째 파라미터는 이 메서드가 IEnumerable<T> 인터페이스를 지원하는 모든 Type에 사용된다는 것을 의미한다.
                두번째 파라미터는 Func 라는 Delegate를 받아들인다는 것을 의미한다.
                보통 여기에는 LINQ 쿼리를 Lambda Expression으로 표현하여 넣게 된다. 

                    public static IEnumerable<TSource> Where<TSource>(
                    this IEnumerable<TSource> source, Func<TSource, bool> predicate)

                아래는 Where() 확장메서드를 사용한 예이다.
                Where() 확장메서드 첫번째 파라미터에 해당되는 this IEnumerable<T>은 IEnumerable<T>를 갖는 클래스에서 사용된다는 의미인데,
                변수 list 객체가 IEnumerable을 구현한 문자열 리스트이므로 list.Where()처럼 사용할 수 있다.
                두번째 파라미터는 람다식으로 표현된 것으로 Element중 A로 시작되는 문자들을 선별하고 있다. 

                    List<string> list = new List<string> { "Apple", "Grape", "Banana" };
                    IEnumerable<string> q = list.Where(p => p.StartsWith("A"));

                또 다른 예로서 아래 예제는 Where() 확장 메서드를 정수 리스트 객체에 적용하여 3으로 나누어 떨어지는 데이타만 출력해 본 예이다.
                Where()의 리턴 값은 IEnumerable<int>인데, 이를 배열로 변경할 경우는 ToArray()를,
                리스트로 변경할 경우는 ToList()를 사용하여 리턴 데이타를 특별한 자료구조에 저장할 수 있다.
            */
            {
                List<int> nums = new List<int> { 55, 44, 33, 66, 11 };

                // Where 확장 메서드 정수 리스트에 사용
                var v = nums.Where(p => p % 3 == 0);

                // IEnumerable<int> 결과를 정수리스트로 변환
                List<int> arr = v.ToList<int>();

                // 리스트 출력
                arr.ForEach(n => Console.WriteLine(n));

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //extension_method();

            //extension_method_with_Enumerable();
        }
    }
}
