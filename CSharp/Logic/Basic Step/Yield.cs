using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BasicStep
{
    public class Yield
    {
        static IEnumerable<int> GetNumber()
        {
            yield return 10;  // 첫번째 루프에서 리턴되는 값
            yield return 20;  // 두번째 루프에서 리턴되는 값
            yield return 30;  // 세번째 루프에서 리턴되는 값
        }

        static void yield_what()
        {
            /*
                C#의 yield 키워드는 호출자(Caller)에게 컬렉션 데이타를 하나씩 리턴할 때 사용한다.
                흔히 Enumerator(Iterator)라고 불리우는 이러한 기능은
                집합적인 데이타셋으로부터 데이타를 하나씩 호출자에게 보내주는 역할을 한다.

                yield는 yield return 또는 yield break의 2가지 방식으로 사용되는데,
                (1) yield return은 컬렉션 데이타를 하나씩 리턴하는데 사용되고,
                (2) yield break는 리턴을 중지하고 Iteration 루프를 빠져 나올 때 사용한다.

                yield 의 간단한 예제로 아래 코드를 살펴보자.
                GetNumber() 라는 메서드는 3개의 yield return 문을 가지고 있다.
                만약 외부에서 이 GetNumber()를 호출하게 되면,
                첫번째 호출시에는 첫번째 yield return 10 을 실행하여 10을 리턴하게 되고,
                두번째로 호출되면 yield return 20 이 실행되어 20을 리턴하게 된다.
                이러한 방식으로 GetNumber()는 한꺼번에 10,20,30을 모두 리턴하는 것이 아니라,
                한번 호출시마다 다음 yield return 문의 값을 리턴하는 것이다.
            */
            {
                foreach (int num in GetNumber())
                {
                    Console.WriteLine(num);
                }

                Console.ReadLine();
            }

            /*
                이러한 특별한 리턴 방식은 다음과 같은 경우에 유용하게 사용된다.

                (1) 만약 데이타의 양이 커서 모든 데이타를 한꺼번에 리턴하는 것하는 것 보다 조금씩 리턴하는 것이 더 효율적일 경우.
                    예를 들어, 어떤 검색에서 1만 개의 자료가 존재하는데, UI에서 10개씩만 On Demand로 표시해 주는게 좋을 수 있다.
                    즉, 사용자가 20개를 원할지, 1000개를 원할지 모르기 때문에, 일종의 Lazy Operation을 수행하는 것이 나을 수 있다.
                (2) 어떤 메서드가 무제한의 데이타를 리턴할 경우.
                    예를 들어, 랜덤 숫자를 무제한 계속 리턴하는 함수는 결국 전체 리스트를 리턴할 수 없기 때문에
                    yield 를 사용해서 구현하게 된다. 
                (3) 모든 데이타를 미리 계산하면 속도가 느려서 그때 그때 On Demand로 처리하는 것이 좋은 경우.
                    예를 들어 소수(Prime Number)를 계속 리턴하는 함수의 경우,
                    소수 전체를 구하면 (물론 무제한의 데이타를 리턴하는 경우이기도 하지만) 시간상 많은 계산 시간이 소요되므로
                    다음 소수만 리턴하는 함수를 만들어 소요 시간을 분산하는 Lazy Calcuation을 구현할 수 있다. 
            */
        }


        class MyList
        {
            private int[] data = { 1, 2, 3, 4, 5 };

            public IEnumerator GetEnumerator()
            {
                int i = 0;
                while (i < data.Length)
                {
                    yield return data[i];
                    i++;
                }
            }

            //...
        }

        static void yield_with_enumerator()
        {
            /*
                C#에서 yield 가 자주 사용되는 곳은 집합적 데이타를 가지고 있는 컬렉션 클래스이다.
                일반적으로 컬렉션 클래스는 데이타 요소를 하나 하나 사용하기 위해 흔히 Enumerator(Iterator) 를 구현하는 경우가 많은데,
                Enumerator를 구현하는 한 방법으로 yield 를 사용할 수 있다.

                (참고: 인터페이스에 대한 설명이 차후 아티클에 나오지만, 여기서는 관련된 사항을 참고로 적어둔다.
                Enumerator는 데이타 요소를 하나씩 리턴하는 기능을 하는 것으로 C#/.NET에서 IEnumerator 라는 인터페이스를 구현해야 한다.
                IEnumerator 인터페이스는 Current (속성), MoveNext() (메서드), Reset() (메서드) 등 3개의 멤버로 이루어져 있는데,
                Enumerator가 되기 위해서는 Current와 MoveNext() 를 반드시 구현해야 한다.

                일반적으로 컬렉션 클래스와 별도로 Enumerator를 구현할 수 있고, 이를 컬렉션과 동일한 클래스에서 구현할 수도 있다.
                컬렉션 클래스와 같이 Enumeration이 가능한 클래스를 Enumerable 클래스라 부르는데,
                C#/.NET에서 Enumerable 클래스는 IEnumerable 인터페이스를 구현해야 한다.
                IEnumerable 인터페이스는 GetEnumerator() 라는 하나의 메서드를 가지고 있는데
                GetEnumerator()는 IEnumerator 구현한 객체를 리턴한다.)

                컬렉션 타입에 혹은 Enumerable 클래스에서 GetEnumerator() 메서드를 구현하는 한 방법으로 yield 를 사용할 수 있다.
                즉, GetEnumerator() 메서드에서 yield return를 사용하여 컬렉션 데이타를 순차적으로 하나씩 넘겨주는 코드를 구현하고,
                리턴타입으로 IEnumerator 인터페이스를 리턴할 수 있다.

                C#에서 Iterator 방식으로 yield 를 사용하면, 명시적으로 별도의 Enumerator 클래스를 작성할 필요가 없다. 

                아래의 예제는 MyList라는 컬렉션 타입에 있는 데이타를 하나씩 리턴하는 GetEnumerator() 메서드의 샘플코드이다.
                예제의 GetEnumerator() 메서드는 데이타를 하나씩 리턴하기 위해 yield return문을 while 루프 안에서 사용하고 있다.
                클래스 안의 샘플 data는 1부터 5까지 숫자인데, 외부 호출자가 순차적으로 호출하면 yield return에서 하나씩 리턴한다.
                처음엔 1, 다음에는 2 등등.

                호출자(Caller)가 이 메서드를 사용하는 방법은
                (1) foreach 문을 사용하여 C#에서 자동으로 Iterator 루프 처리를 하게 하는 방법과
                (2) GetEnumerator()로부터 IEnumerable 인터페이스를 얻어 MoveNext() 메서드와
                Current 속성을 사용하여 개발자가 직접 수동으로 요소 하나씩 사용하는 방법이 있다.
                일반적으로 그 편리성 때문에 foreach 문을 사용하는 방식을 사용한다.

                아래 예제의 하단은 foreach문을 사용하여 Enumeration을 하는 방법과
                IEnumerator의 멤버를 써서 수동으로 Enumeration을 하는 방법을 예시하고 있다. 
            */
            {
                // (1) foreach 사용하여 Iteration
                var list = new MyList();

                foreach (var item in list)
                {
                    Console.WriteLine(item);
                }

                // (2) 수동 Iteration
                IEnumerator it = list.GetEnumerator();
                it.MoveNext();
                Console.WriteLine(it.Current);  // 1
                it.MoveNext();
                Console.WriteLine(it.Current);  // 2

                Console.ReadLine();
            }
        }


        static void yield_process_sequence()
        {
            /*
                C#에서 호출자가 yield를 가진 Iteration 메서드를 호출하면 다음과 같은 방식으로 실행된다.
                즉, 호출자(A)가 IEnumerable을 리턴하는 메서드(B)를 호출하면, yield return 문에서 하나의 값을 리턴하고,
                해당 메서드(B)의 위치를 기억해 둔다.
                호출자(A)가 다시 루프를 돌아 다음 값을 메서드(B)에 요청하면,
                메서드의 기억된 위치 다음 문장부터 실행하여 다음 yield 문을 만나 값을 리턴한다.  
            */
            {
                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //yield_process_sequence();

            //yield_with_enumerator();

            //yield_what();
        }
    }
}
