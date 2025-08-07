using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdvancedStep.ExpressionBodied;

namespace AdvancedStep
{
    public class NullConditionalOperator
    {
        class Customer
        {
            public Int32 Age { get; set; }
        }

		static void null_condition_operator_what()
        {
            /*
                C# 프로그래밍에서 NULL 체크만큼 많은 시간을 할애하는 곳도 아마 드물 것이다.
                즉, 객체의 메서드나 속성을 사용하기 전에 객체가 NULL인지 항상 체크해 줘야 하는 경우가 많다.
                C# 6.0 에서는 이러한 불편을 덜어 주기 위해 ? (혹은 ?.) 이라는 널 조건 연산자 (Null-conditional operator)를 새로 추가하였다.
                널 조건 연산자는 ? 앞에 있는 객체가 NULL인지 체크해서 NULL이면 그냥 NULL을 리턴하고, 그렇지 않으면 ? 다음의 속성이나 메서드를 실행한다.
                이렇게 하면 일일이 if 문을 써서 null을 체크할 필요 없이 축약해서 개발자의 의도를 표현할 수 있다.
                문법적인 측면에서 ? 다음에 일반적으로 속성이나 메서드가 오기 때문에 ?. 와 같이 표현되지만,
                만약 인덱서 혹은 배열 요소등을 접근할 경우는 ?[] 과 같이 표현될 수도 있다.
            */
            {
                var rows = new List<Int32>();

				// rows가 NULL이면 cnt 도 NULL
				// rows가 NULL이 아니면 cnt는 실제 rows 갯수
				int? cnt = rows?.Count;


				var customers = new List<Customer>();
				// customers 컬렉션이 NULL이면 c는 NULL
				// 아니면, c는 첫번째 배열요소
				Customer c = customers?[0];

                // customers가 널인지 체크하고
                // 다시 customers[0]가 널인지 체크
				int? age = customers?[0]?.Age;


				Console.ReadLine();
            }
        }


        static void null_coalescing_operator_what()
        {
            /*
                널 조건 연산자 (Null-conditional operator) 만을 사용하게 되면,
                리턴 변수는 항상 null을 가질 수 있는 Nullabel Type이어야 한다.
                ?. 연산자 앞의 값이 널인 경우 null을 리턴해야 하기 때문이다.
                만약 리턴 변수가 null을 가질 수 없는 경우라면 (혹은 null이 아니어야 한다면) 어떻게 null 리턴을 막을 수 있을까?
                이러한 경우에 ?? 연산자 (null-coalescing operator)를 함께 사용하게 된다.
                즉 null 인 경우 ?? 뒤의 디폴트 값을 리턴하는 것인데, 이렇게 하면 리턴 변수형이 꼭 Nullable Type일 필요가 없게 된다.
            */
            {
				var rows = new List<Int32>();

				// rows가 NULL이면 cnt = 0
				// 아니면 cnt는 실제 rows 갯수
				int cnt = rows?.Count ?? 0;


				Console.ReadLine();
            }
        }


		public class MyButton
		{
			public event EventHandler Clicked;

			// 이전 방식
			public void Click1()
			{
				//...

				// 스텝1. 임시변수에 이벤트 복사 (Thread safety 때문)
				var tempClicked = Clicked;
				// 스텝2. 널 체크
				if (tempClicked != null)
				{
					// 스텝3. 이벤트 Invoke
					tempClicked(this, null);
				}
			}

			// C# 6.0 방식
			public void Click2()
			{
				// ...

				// 위의 3 스텝을 널 조건 연산자을 사용하여
				// 한 문장으로 표현
				Clicked?.Invoke(this, null);
			}
		}

		static void null_condition_operator_with_event_call()
        {
            /*
                널 조건 연산자 (Null-conditional operator)을 활용하는 대표적인 예 중이 하나로 이벤트를 호출(Fire)하는 예를 들 수 있다.
                기존의 C#에서 이벤트를 Fire하는 루틴은 크게 3 가지 스텝을 밟았다.
                    (1) 먼저 이벤트 필드를 메서드 내에서 로컬 변수에 할당한다.
                    (2) 다음 로컬 이벤트 변수가 널인지 체크한다.
                    (3) 마지막으로 이벤트를 Invoke 한다.

                여기서 첫번째 이벤트를 로컬 변수에 할당하는 이유는 Thread Safety 때문이다
                (즉, 만약 이러한 할당을 하지 않는다면, 널 체크가 끝나고
                 세번재 Invoke 단계에 들어 가기 직전에 다른 쓰레드가 이벤트 핸들러를 Unsubscribe할 경우 이벤트 필드가 NULL이 되어
                 Null Exception이 발생할 수 있기 때문이다.
                 이벤트 필드는 모든 쓰레드가 엑세스할 수 있지만, 메서드 로컬 변수는 각 쓰레드에서 배타적으로 사용한다).

                Null-conditional operator을 사용하는 아래 두번째 메서드(Click2)를 보면,
                위의 3가지 스텝을 한 문장으로 간략히 표현하고 있다.
                여기서 이벤트를 Fire하는 부분을 Clicked?(this, null) 처럼 사용하지 않는 이유는 문법적인 혼동이 피하기 위해서이다.
                또한, Clicked?.Invoke() 표현에서 Clicked를 별도의 임시 변수에 두지 않았는데,
                이는 실행시 ? 이전 문장이 한번 실행되어 임시 변수에 자동 할당되기 때문이다.
                따라서, 이 표현은 Thread Safety를 보장하는 표현이라 볼 수 있다.
            */
            {
                Console.ReadLine();
            }
        }

        static void null_forgiving_operator_what()
        {
            /*                 
                C# 8.0부터는 널 취소 연산자(Null-forgiving operator) 라는 새로운 기능이 도입되었습니다.
                이 연산자는 일반적으로 Nullable 값 형식의 인스턴스 또는 참조 형식에 대한 null 값 검사를 생략할 때 사용됩니다.

                기본적으로 C#에서는 null을 가질 수 있는 형식에 접근할 때 null 조건부 연산자(?.)를 사용하여 null 검사를 수행합니다.
                그러나 때로는 해당 변수가 null일 가능성이 없을 때에도 컴파일러가 여전히 null 검사를 적용하게 됩니다.
                이 때문에 불필요한 코드가 추가될 수 있습니다.

                널 취소 연산자(!)는 컴파일러에게 특정 변수가 null일 가능성이 없음을 알려줍니다.
                따라서 해당 변수에 대한 null 검사를 생략할 수 있습니다. 널 취소 연산자는 변수 또는 값 뒤에 ! 기호를 붙여서 사용됩니다.
            */
            {
                string? nullableString = "Hello";

                // 컴파일러에게 nullableString이 null이 아님을 알려줍니다.
                string nonNullableString = nullableString!;

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //null_forgiving_operator_what();

            //null_condition_operator_with_event_call();

            //null_coalescing_operator_what();

            //null_condition_operator_what();
        }
    }
}
