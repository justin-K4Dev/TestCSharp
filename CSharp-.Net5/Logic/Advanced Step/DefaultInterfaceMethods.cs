using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;






namespace AdvancedStep;

public class DefaultInterfaceMethods
{
    public interface ILogger
    {
        void Log(string msg) => Console.WriteLine($"Log: {msg}");
    }

    public class ConsoleLogger : ILogger
    {
        // Log() 구현 안 해도 됨. 인터페이스의 기본 구현 사용됨
    }

    static void DefaultInterfaceMethods_what()
    {
        /*
            인터페이스 내에서 메서드 본문을 직접 정의할 수 있게 하는 기능입니다.
            즉, 기본 구현이 포함된 인터페이스를 만들 수 있습니다.
        
            🔹 도입 버전
              -  C# 8.0 이상
              - .NET Core 3.0 이상에서 사용 가능
       */
        {
            ILogger logger = new ConsoleLogger();
            logger.Log("Hello"); // 출력: Log: Hello
        }

        Console.ReadLine();
    }

    public static void Test()
    {
        //DefaultInterfaceMethods_what();
    }
}
