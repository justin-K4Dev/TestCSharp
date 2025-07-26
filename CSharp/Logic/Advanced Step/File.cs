using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace AdvancedStep
{
    public class File
    {
        static void exclusive_open()
        {
            /*
                파일을 오픈하는 한 방법으로 System.IO.File.Open() 을 사용하곤 하는데,
                흔히 파일을 오픈할 때 다음 예제와 같이 배타적으로 오픈하곤 한다.
                이 코드는 파일을 읽기/쓰기 모드로 배타적으로 오픈한 것이다.
                이렇게 오픈된 파일에는 읽기와 쓰기를 할 수 있으며, 파일이 성공적으로 오픈된다면,
                다른 쓰레드나 프로세스는 이 파일을 어떤 모드로도 오픈할 수 없다.
                (일반적으로 다른 프로세스가 사용 중이라는 에러가 뜬다).
            */
            {
                String fileName = "test.txt";

                byte[] bytes;
                using (var fs = System.IO.File.Open(fileName, FileMode.Open))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                }

                Console.ReadLine();
            }
        }


        static void Run()
        {
            String fileName = "test.txt";

            byte[] bytes;
            using (var fs = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                Thread.Sleep(2000); //Test
            }
        }

        static void shared_open()
        {
            /*
                파일을 다른 쓰레드나 프로세스에서 함께 사용하기 위해서는 FileAccess 와 FileShare를 알맞게 사용해야 한다.
                FileAccess는 Read, Write, ReadWrite 세가지 중에 하나를 선택하는데,
                이는 File.Open()을 수행하는 쓰레드가 파일을 읽기전용, 쓰기전용(읽기 불가), 읽기+쓰기로 파일을 엑세스하길 원한다것을 표시한다.
                FileShare는 None, Read, Write, ReadWrite 등을 사용하는데, 현재 쓰레드가 파일 오픈에 성공한다면,
                다른 프로세스에게 읽기 혹은 쓰기를 공유하겠다는 것을 나타낸다.
                만약 FileShare.None 이면, 다른 프로세스는 그 파일을 엑세스할 수 없다.

                파일 오픈이 요청되면, 시스템은 그 파일의 FileAccess 요청이 이미 그 파일을 오픈한 다른 프로세스들의 FileShare와 서로 허용되는지를 체크하게 된다.
                예를 들어, 다른 3개의 프로세스가 이미 파일을 FileAccess.Read로 오픈했고 읽기/쓰기를 허용했다면,
                새 쓰레드는 읽기 모드로 FileShare.Read 혹은 FileShare.ReadWrite을 사용해서 파일을 오픈할 수 있다.
                하지만, 이미 오픈된 파일이 FileAccess.Read와 FileShare.Read로 설정된 경우,
                읽기만 허용했기 때문에 FileAccess.Write 모드로 파일을 오픈할 수는 없다.
                파일 오픈에 있어 누가 먼저 파일을 어떤 모드로 오픈하고 공유했는가가 중요한 역활을 한다. 

                아래 예제는 FileAccess.Read, FileShare.Read를 사용한 코드로서 파일을 읽기 모드로 열고,
                다른 프로세스도 읽을 수 있도록 허용한 코드이다.
                만약 복수 개의 쓰레드가 동시에 이 메서드를 사용한다면, 파일을 동시에 멀티쓰레드로 읽는 것이 가능하다.
            */
            {
                new Thread(Run).Start();
                new Thread(Run).Start();
                Thread.Sleep(7000);

                Console.ReadLine();
            }
        }


        static void Writing()
        {
            String fileName = "test.txt";

            var bytes = new byte[200];
            for (int i = 0; i < 200; i++)
            {
                bytes[i] = 55;
            }

            long bytesWritten = 0;

            using (var fs = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                // 100 바이트 먼저 쓰고
                Console.WriteLine("First write : 100 bytes");
                fs.Write(bytes, 0, 100);

                // 3초후 나머지 100 바이트 씀
                Thread.Sleep(3 * 1000);
                Console.WriteLine("Second write: 100 bytes");
                fs.Write(bytes, 100, 100);
                bytesWritten = fs.Length;
            }
            Console.WriteLine("Total writing: {0} bytes", bytesWritten);
        }

        static void Reading()
        {
            String fileName = "test.txt";

            // Write가 먼저 파일핸들 획득하도록 잠시 대기. 

            byte[] bytes;
            long bytesRead = 0;
            using (var fs = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // 쓰고 있는 중 데이타 읽기 
                Console.WriteLine("Reading data...");
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                bytesRead = fs.Length;
            }
            Console.WriteLine("Total reading: {0} bytes", bytesRead);
        }

        static void dirty_read()
        {
            /*
                하나의 쓰레드가 파일에 쓰고 있는 중에 다른 쓰레드가 그 데이타를 읽게 된다면,
                읽는 쪽에서는 완전한 데이타를 읽기 못할 수 있다.
                흔히 DB에서 완전히 Commit되지 않은 데이타를 읽으려 할 때 이를 Dirty Read라 부른다.
                파일시스템에서도 이러한 Dirty Read가 가능한데,
                다음 예제는 2개의 쓰레드가 서로 FileShare.ReadWrite 공유를 하면서 한 파일을 동시에 읽고 쓰는 코드이다.
                여기서 쓰기 쓰레드는 200 바이트를 쓰지만,
                읽기 쓰레드는 쓰기가 채 마치기 전에 읽기를 끝내기 때문에 100 바이트만 읽게된다.  
            */
            {
                // 쓰기 쓰레드 
                Thread t1 = new Thread(Writing);
                // 읽기 쓰레드
                Thread t2 = new Thread(Reading);

                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //dirty_read();

            //shared_open();

            //exclusive_open();
        }
    }
}
