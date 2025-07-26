using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Concurrent;
using System.Threading;


namespace Collections
{
    public class HashTable
    {
        static void hash_table_what()
        {
            /*
                자료구조 : 해시테이블 (Hash Table) 

                해시(Hash)는 키 값을 해시 함수(Hash function)으로 해싱하여
                해시테이블의 특정 위치로 직접 엑세스하도록 만든 방식이다.
                키 값을 통해 직접 엑세스하기 위해서 모든 가능한 키 값을 갖는 배열을 만들면,
                배열크기가 엄청나게 커지게 된다.
                예를 들어, 주민등록번호를 키 값으로 하는 경우,
                000000-0000000 부터 999999-9999999까지 10의 13승의 배열 공간이 필요한데,
                만약 회원수가 1000명인 경우, 1000명을 저장하기 위해 10^13의 엄청난 배열 공간이 필요하게 된다.
                이렇게 낭비되는 공간을 줄이기 위해 해시 함수를 사용하게 되는데,
                이 함수는 적은 공간 안에서 모든 키를 직접 찾아갈 수 있도록 해준다.
                하지만 경우에 따라 서로 다른 키가 동일한 해시테이블 버켓 위치를 가리킬 수 있는데,
                이를 해결하기 위해 여러 Collision Resolution 방식이 사용된다.
                Collision Resolution의 방식으로 Linear Probing, Quadratic Probing, Rehashing (Double Hashing), Chaining 등 여러가지가 있다.
                해시테이블 자료구조는 추가, 삭제, 검색에서 O(1)의 시간이 소요된다. 
            */
            {
                Console.ReadLine();
            }
        }


        static void Hashtable_use()
        {
            /*
                .NET에 해시테이블을 구현한 Non-generic 클래스로 Hashtable 클래스가 있다.
                Hashtable은 Key값과 Value값 모두 object 타입을 받아들이며, 박싱/언박싱을 하게 된다.
                Hashtable은 Rehashing (Double Hashing)방식을 사용하여 Collision Resolution을 하게 된다.
                즉, 해시함수를 H1(Key) 부터 Hk(Key) 까지 k개를 가지고 있으며,
                키 충돌(Collision)이 발생하면, 차기 해시함수를 계속 사용하여 빈 버켓을 찾게된다.
                이 자료구조는 추가, 삭제, 검색에서 O(1)의 시간이 소요된다.
            */
            {
                Hashtable ht = new Hashtable();
                ht.Add("irina", "Irina SP");
                ht.Add("tom", "Tom Cr");

                if (ht.Contains("tom"))
                {
                    Console.WriteLine(ht["tom"]);
                }

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //Hashtable_use();

            //hash_table_what();
        }
    }
}
