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
                C# 6.0에서는 이러한 스타일들을 통일시켜 초기화과정에서도 Indexer 스타일의 괄호([])를 사용할 수 있도록 하였다.
                이는 해시테이블을 보다 직관적으로 초기화하고 사용하는데 도움이 된다.
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

                // C# 6.0 표현
                {
                    var scores = new Dictionary<string, int>()
                    {
                        ["kim"] = 100,
                        ["lee"] = 90
                    };
                    int sc = scores["lee"];
                }

                Console.ReadLine();
            }
        }


        static void dictionary_initializer_with_indexer()
        {
            /*
                C# 6.0의 Dictionary 초기화 기능은 해시테이블, Dictionary 뿐만 아니라, 인덱서를 지원하는 모든 객체에서 사용될 수 있다.
            */
            {
                var A = new[] { 1, 2, 3 };

                // List는 인덱서를 지원하므로
                // Dictionary Initializer 사용 가능

                var L = new List<int>(A) { [2] = 9 };

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //dictionary_initializer_with_indexer();

            //dictionary_initializer_use();
        }
    }
}
