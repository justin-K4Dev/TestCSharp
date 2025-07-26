using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Using
    {
		static void using_for_directive()
		{
			/*
				지시문 용도로 사용 한다.
				
				- namespace 의 이름을 생략할 수 있다.

				using System.Text; //코드 상단에 네임스페이스 정의
				using MyProject = Kangms.MyNetwrok.DreamProject; // 별칭(alias)
            */
			{
				Console.ReadLine();
			}
		}

		static void using_for_statement()
		{
			/*
				using 선언은 using 키워드 뒤에 오는 변수 선언으로서,
				using 뒤에 있는 변수가 using을 둘러싼 범위를 벗어날 경우 Dispose 하도록 컴파일러에게 지시하게 된다.
				기존의 using 문을 사용할 경우 괄호 {...} 를 표시해야 했는데, using 블럭 전체를 들여쓰기 해야 하는 불편함이 있었다.
				using 선언은 (별도의 괄호를 메서드 내부에 사용하지 않는 한) 통상 메서드가 끝날 때 Dispose() 를 자동 호출하게 해 준다.
				물론 경우에 따라 긴 메서드 안에 특정 부분에서만 using 을 사용하고 빨리 Dispose() 해 주어야 한다면,
				기존의 using 문을 사용할 수 있다.
            */
			{
				using (var reader = new StreamReader("src.txt"))
				{
					string data = reader.ReadToEnd();
					Console.WriteLine(data);

				}  // 여기서 Dispose() 호출됨

				Console.ReadLine();
			}
		}

		public static void Test()
        {
			//using_for_statement();

			//using_for_directive();
		}
	}
}
