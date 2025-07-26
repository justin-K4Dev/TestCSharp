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
    public class DynamicArray
    {
        static void dynamic_array_what()
        {
            /*
                자료구조 : 동적 배열 (Dynamic Array)      
                
                배열은 고정된 크기의 연속된 배열요소들의 집합이므로 배열을 초기화 할 때 총 배열 요소의 수를 미리 지정해야 한다.
                하지만 경우에 따라 배열요소가 몇 개나 필요한 지 미리 알 수 없는 경우가 있으며,
                중간에 필요에 따라 배열을 확장해야 하는 경우도 있다.
                .NET에는 이러한 동적 배열을 지원하는 클래스로 ArrayList와 List<T>이 있다.
                이들 동적 배열 클래스들은 배열 확장이 필요한 경우,
                내부적으로 배열 크기가 2배인 새로운 배열을 생성하고
                모든 기존 배열 요소들을 새로운 배열에 복사한 후 기존 배열을 해제한다.
                동적 배열의 Time Complexity는 배열과 같이 인덱스를 통할 경우 O(1), 값으로 검색할 경우 O(n)을 갖는다.
            */
            {
                Console.ReadLine();
            }
        }


        static void ArrayList_use()
        {
            /*
                ArrayList는 모든 배열 요소가 object 타입인 Non-generic 동적 배열 클래스이다.
                .NET의 Non-generic 클래스들은 System.Collections 네임스페이스 안에 있으며,
                단점으로 박싱 / 언박싱이 일어나게 된다.
                ArrayList는 배열 요소를 읽어 사용할 때 object를 리턴하므로
                일반적으로 원하는 타입으로 먼저 캐스팅(Casting)한 후 사용하게 된다.
            */
            {
                ArrayList myList = new ArrayList();
                myList.Add(90);
                myList.Add(88);
                myList.Add(75);

                // int로 casting
                int val = (int)myList[1];

                Console.ReadLine();
            }
        }


        static void List_use()
        {
            /*
                List<T>는 배열요소가 T 타입인 Generics로서 동적 배열을 지원하는 클래스이다.
                .NET의 Generic 클래스들은 System.Collections.Generic 네임스페이스 안에 있다.
                List클래스는 내부적으로 배열을 가지고 있으며, 동일한(Homogeneous) 타입의 데이타를 저장한다.
                만약 미리 할당된 배열 크기(Capacity라 부른다)가 부족하면
                내부적으로 배열을 2배로 늘려 동적으로 배열을 확장한다.
                ArrayList와 다르게 캐스팅을 할 필요가 없으며, 박싱 / 언박싱의 문제를 발생시키지 않는다. 
            */
            {
                List<int> myList = new List<int>();
                myList.Add(90);
                myList.Add(88);
                myList.Add(75);
                int val = myList[1];

                Console.ReadLine();
            }
        }


        static void SortedList_use()
        {
            /*
                SortedList클래스는 Key값으로 Value를 찾는 Map ADT 타입 (ADT: Abstract Data Type)을
                내부적으로 배열을 이용해 구현한 클래스이다.
                .NET에서 MAP ADT를 구현한 클래스로는 해시테이블을 이용한 Hashtable/Dictionary클래스,
                이진검색트리를 이용한 SortedDictionary, 그리고 배열을 이용한 SortedList 등이 있다.
                SortedList클래스는 내부적으로 키값으로 소트된 배열을 가지고 있으며,
                따라서 이진검색(Binary Search)가 가능하기 때문에 O(log n)의 검색 시간이 소요된다.
                만약 미리 할당된 배열 크기(Capacity라 부른다)가 부족하면
                내부적으로 배열을 2배로 늘려 동적으로 배열을 확장한다. 
            */
            {
                SortedList<int, string> list = new SortedList<int, string>();
                list.Add(1001, "Tim");
                list.Add(1020, "Ted");
                list.Add(1010, "Kim");

                string name = list[1001];

                foreach (KeyValuePair<int, string> kv in list)
                {
                    Console.WriteLine("{0}:{1}", kv.Key, kv.Value);
                }
                // 출력
                //1001:Tim
                //1010:Kim
                //1020:Ted

                Console.ReadLine();
            }
        }


        static void ConcurrentBag_use()
        {
            /*
                .NET 4.0 부터 멀티쓰레딩 환경에서 리스트를 보다 간편하게 사용할 수 있는
                새로운 클래스인 ConcurrentBag<T> 가 제공되었다.
                ConcurrentBag<T> 클래스는 리스트와 비슷하게 객체들의 컬렉션을 저장하는데,
                List<T> 와는 달리 입력 순서를 보장하지는 않는다.
                ConcurrentBag 에 데이타를 추가하기 위해 Add() 메서드를 사용하고,
                데이타를 읽기 위해서는 foreach문 혹은 TryPeek(), TryTake() 메서드를 사용한다.
                TryPeek()은 ConcurrentBag에서 데이타를 읽기만 하는 것이고,
                TryTake()는 데이타를 읽을 후 해당 요소를 ConcurrentBag에서 삭제하게 된다.

                ConcurrentBag는 멀티쓰레드가 동시에 엑세스할 수 있는데,
                예를 들어 ThreadA와 ThreadB가 ConcurrentBag에 데이타를 쓸 때,
                ThreadA가 1,2,3 을 넣고, ThreadB가 4,5,6 을 넣으면, ThreadA는 ConcurrentBag을 다시 읽을 때,
                자신이 쓴 1,2,3을 우선순위로 먼저 읽은 다음, 나머지 다른 쓰레드에 의해 입력된 요소들 (4,5,6)을 읽게 된다.

                아래 예제에서 첫번째 쓰레드는 100개의 숫자를 ConcurrentBag에 넣게 되고,
                동시에 두번째 쓰레드는 1초마다 10회에 걸쳐 해당 ConcurrentBag의 내용을 출력하는 것이다.
            */
            {
                var bag = new ConcurrentBag<int>();

                // 데이타를 Bag에 넣는 쓰레드
                Task t1 = Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        bag.Add(i);
                        Thread.Sleep(100);
                    }
                });

                // Bag에서 데이타를 읽는 쓰레드
                Task t2 = Task.Factory.StartNew(() =>
                {
                    int n = 1;
                    // Bag 데이타 내용을 10번 출력함
                    while (n <= 10)
                    {
                        Console.WriteLine("{0} iteration", n);
                        int count = 0;

                        // Bag에서 데이타 읽기
                        foreach (int i in bag)
                        {
                            Console.WriteLine(i);
                            count++;
                        }
                        Console.WriteLine("Count={0}", count);

                        Thread.Sleep(1000);
                        n++;
                    }
                });

                // 두 쓰레드가 끝날 때까지 대기
                Task.WaitAll(t1, t2);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //List_use();

            //ArrayList_use();

            //dynamic_array_what();

            //SortedList_use();

            //ConcurrentBag_use();
        }
    }
}
