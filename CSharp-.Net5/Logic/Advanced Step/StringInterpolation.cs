using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;

public class StringInterpolation
{
    static void string_interpolation_concept()
    {
        /*
            C#에서 문자열을 포맷팅하기 위해서는 string.Format()안의 서식 문자열에 {0},{1},{2} 등과 같은 인수를 넣어 해당 위치에 파라미터들이 들어가게 하였다.
            C# 6.0 에서는 새로운 문자열 내삽(안에 직접 집어 넣는) 기능을 추가하였다.
            이 기능은 {0},{1} 등과 같이 위치를 지정하고, 다시 파라미터 리스트를 순서에 맞게 넣어야 하는 번거로움을 해결하기 위한 것으로,
            {} 안에 직접 파라미터를 넣게 된다.
            String Interpolation 기능을 사용하기 위해서는 $ 사인을 전체 서식문자열 앞에 추가하여야 한다.
            아래 예제는 직사각형 객체 r의 속성을 내삽 기능을 이용해 출력한 예이다.
            r.Height와 r.Width라는 객체의 속성을 { } 안에 직접 쓸 수 있으며,
            r.Height * r.Width 과 같은 연산식도 포함할 수 있다 (이 경우 ( ) 괄호로 묶어 줌).
        */
        {
				Rectangle r = new Rectangle();
				r.Height = 10;
				r.Width = 32;

				// Format string 앞에 $ 를 붙인다
				// {} 안에 속성 혹은 계산식 등을 넣을 수 있다.
				string s = $"{r.Height} x {r.Width} = {(r.Height * r.Width)}";
				Console.WriteLine(s);



				Console.ReadLine();
        }
    }


    public static void Test()
    {
        //string_interpolation_concept();
    }
}
