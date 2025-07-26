using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class DictionaryInitializer
    {
        static void dictionary_initializer_use()
        {
            /*
                기존 C#에서 Dictionary를 초기화 하는 스타일({} 사용)과 초기화 후 사용하는 스타일 ([] 괄호 사용, 인덱서 스타일) 간에 약간의 차이가 있었다.
                즉, 아래 예제에서 보이듯이 처음 초기화 시에는 { "kim", 90 } 처럼 사용하고, 사용시에는 dic["kim"] 처럼 사용한다.
            */
            {
                // 이전의 C# 표현
                {
                    var scores = new Dictionary<string, int>()
                    {
                        { "kim", 100 },
                        { "lee",  90 }
                    };
                    int sc = scores["lee"];
                }

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //dictionary_initializer_use();
        }
    }
}
