using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Comment
    {
        static void comment_what()
        {
            {
                //코멘트: 한 라인 코멘트는 두개의 슬래시 사용함
                int a = 1;
                Console.Write("{0} ", a);

                int b = 1;  //코멘트: 하나의 문장 뒤에 코멘트를 달 수 있음
                Console.WriteLine("{0} ", b);

                /*
                   복수 라인에 대한 코멘트
                   int c;
                   int d;
                */

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //comment_what();
        }
    }
}
