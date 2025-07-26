using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MultiThread
{
    public class AwaitInCatchBlock
    {

#if WORKING
        class MyResult
        {

        }

        static async Task<MyResult> getResponseAsync()
        {

        }

		static async Task<MyResult> writeExceptionLog(Exception ex)
		{

		}

		static async Task<MyResult> close()
		{

		}

		static void await_in_catch_finally()
        {
            /*
                catch & finally 블럭에서 await 사용 
             
                C# 5.0에서 await 기능을 도입할 때, catch 나 finally 블럭에서 await를 사용하는 기능을 지원하지 않았다.
                이 때문에 개발자들은 여러 Workaround를 사용했어야 했었는데, 이제 C# 6.0 에서는 이를 기본적으로 지원하게 되었다.
                catch 블럭에서 일반적으로 에러를 로깅을 하는 경우가 많은데,
                이 때 로깅처리를 비동기적으로 하기 위해 await를 사용할 수 있다.
            */
            {
				try
				{
					//...
					var response = await getResponseAsync();
					//...
				}
				catch (Exception ex)
				{
					//에러를 비동기로 로깅
					await writeExceptionLog(ex);
				}
				finally
				{
					//Close를 비동기로 처리
					await close();
				}


				Console.ReadLine();
            }
        }

        
        static void exception_filter()
        {
            /*
                VB.NET 이나 F# 에서는 이미 지원되고 있었지만,
                C#에는 지금까지 지원되지 않았던 것으로 Exception Filter라는 것이 있다.
                Exception Filter란 catch 시 특정한 조건으로만 다시 필터링하는 하여 catch하는 것으로,
                C# 6.0 문법에서는 catch() 문 뒤에 추가적인 when 조건문을 사용하게 된다.
                예를 들어 아래 예제처럼, Win32Exception 에러가 발생했을 때,
                이 Exception 객체의 내부 속성인 NativeErrorCode을 추가적으로 조사해서
                그 값이 0x10 인 경우에만 catch 블럭이 실행하도록 하는 기능이다.
            */
            {
				// Exception Filter
				try
				{
					//...
				}
				catch (Win32Exception ex) when (ex.NativeErrorCode == 0x10)
				{
					Log(ex);
				}

				Console.ReadLine();
            }
        }

#endif

		public static void Test()
        {
            //exception_filter();

            //await_in_catch_finally();
        }
    }
}
