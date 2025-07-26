using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Event
    {
        //클래스 내의 이벤트 정의
        class MyButton
        {
            public string Text;
            //이벤트 정의
            public event EventHandler Click;

            public void MouseButtonDown()
            {
                if (this.Click != null)
                {
                    // 이벤트핸들러들을 호출
                    Click(this, EventArgs.Empty);
                }
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Button 클릭");
        }

        //이벤트 사용
        public void Run()
        {
            MyButton btn = new MyButton();
            //Click 이벤트에 대한 이벤트핸들러로
            //btn_Click 이라는 메서드를 지정함
            btn.Click += new EventHandler(btn_Click);
            btn.Text = "Run";
            //....
        }

        static void event_what()
        {
            /*
                이벤트는 클래스내에 특정한 일(event)이 있어났음을 외부의 이벤트 가입자(subscriber)들에게 알려주는 기능을 한다.
                C#에서 이벤트는 event라는 키워드를 사용하여 표시하며, 클래스 내에서 일종의 필드처럼 정의된다.

                이벤트에 가입하는 외부 가입자 측에서는 이벤트가 발생했을 때 어떤 명령들을 실행할 지를 지정해 주는데,
                이를 이벤트 핸들러라 한다.

                이벤트에 가입하기 위해서는 += 연산자를 사용하여 이벤트핸들러를 이벤트에 추가한다.
                반대로 이벤트핸들러를 삭제하기 위해서는 -= 연산자를 사용한다.
                하나의 이벤트에는 여러 개의 이벤트핸들러들을 추가할 수 있으며,
                이벤트가 발생되면 추가된 이벤트핸들러들을 모두 차례로 호출한다.

                다음 코드는 클래스(MyButton) 내에서 이벤트(Click)를 정의하고 이를 사용하는 예제이다.
                (주: 고급문법의 C# delegate에서 C# event와 delegate와의 관계가 자세히 설명)

                    - Run() 참조

                    * 이벤트 Click은 외부에서 엑세스할 수 있게 public 필드로 정의되어 있다.
                    * MouseButtonDown() 메소드에서 이벤트를 외부로 보내기 전에
                      이벤트에 가입한 가입자들이 있는지 체크하기 위해 if (Click != null)을 사용한다.
                    * 이벤트에 가입하거나 탈퇴하기 위해서는 += (subscribe) 또는 -= (unsubscribe)연산자를 사용한다.
                    * 여기서 void btn_Click(object sender, EventArgs e) 메서드는 이벤트핸들러로 사용되고 있다.
            */
            {
                Event e = new Event();
                e.Run();

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //event_what();
        }
    }
}
