using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace UsefulInterfaces
{
    public class Resource
    {
        public class FileWriter : IDisposable
        {
            private StreamWriter _writer;

            public FileWriter(string path)
            {
                _writer = new StreamWriter(path);
            }

            public void Write(string msg) => _writer?.WriteLine(msg);

            public void Dispose()
            {
                _writer?.Dispose();
                _writer = null;
            }
        }

        static void override_IDisposableT()
        {
            /*
                IDisposable

                ✅ 목적
                  - using문을 통한 "명시적 자원해제"
                  - 파일, DB연결, 네트워크 등 관리되지 않는 자원 해제              
            */

            using (var fw = new FileWriter("out.txt"))
                fw.Write("Hello!");
        }

        public static void Test()
        {
            override_IDisposableT();
        }
    }
}
