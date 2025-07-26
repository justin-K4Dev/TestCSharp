using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class AnonymousFunction
    {
        delegate int MyDelegate(int a, int b);

        static void anonymous_method_what()
        {
            /*
                 앞의 C# delegate 예제를 보면 예제의 delegate들은 모두 이미 정의된 메서드를 가리키고 있었다.
                 이렇게 미리 정의된 메서드들과는 달리, C# 2.0에서부터 메서드를
                 미리 정의하지 않아도 되는 무명 메서드 즉 Anonymous Method를 지원하게 되었다.
                 만약 어떤 메서드가 일회용으로 단순한 문장들로 구성되어 있다면, 굳이 별도의 메서드를 정의하지 않아도 되는 것이다.

                 Anonymous Method를 만들기 위해서는 delegate 키워드와 함께 아래와 같이 파라미터와 실행문장블럭을 적으면 된다.
                 delegate 뒤의 파라미터는 해당 메서드 Prototype에 맞는 동일한 파라미터 타입이어야 하며,
                 실행문장블럭에는 여러 문장들을 쓸 수 있다.

                     Anonymous Method : delegate(파라미터들) { 실행문장들 };

                     ex) delegate(int param1) { Console.Write(param1); };
            */
            {
                MyDelegate add;

                add = delegate (int a, int b)
                {
                    return a + b;
                };

                Console.WriteLine(add(11, 22));

                Console.ReadLine();
            }
        }


        static void anonymous_method_use()
        {
            /*
                Anonymous Method 를 사용하기 위해서는 메서드가 필요한 곳에 직접 delegate로 시작하는 무명메서드를 써주면 된다.

                아래 예제에서 보면 button1.Click 에는 미리 정의된 메서드명(button1_Click) 을 가리키는 이벤트 핸들러를 지정하였고,
                button2.Click 에서는 무명메서드를 사용하여 직접 간단한 문장을 delegate(){ ... }안에 포함시킨 것이다.

                    public partial class Form1 : Form
                    {
                        public Form1()
                        {
                            InitializeComponent();

                            //메서드명을 지정
                            this.button1.Click += new System.EventHandler(this.button1_Click);

                            //무명메서드를 지정
                            this.button2.Click += delegate(object s, EventArgs e)
                            {
                                MessageBox.Show("버튼2 클릭");
                            };
                        }

                        private void button1_Click(object sender, EventArgs e)
                        {
                            MessageBox.Show("버튼1 클릭");
                        }
                    }                   
            */
            {
                Console.ReadLine();
            }
        }


        static void delegate_vs_anonymous_method()
        {
            // Delegate Type vs Anonymous Method (1)
            {
                /*
                    C#의 delegate는 Delegate 타입을 정의할 때도 사용되고, 무명메서드를 정의할 때도 사용된다.
                    Delegate 타입을 정의하는 아래 첫번째 예의 경우,
                    SumDelegate는 Delegate 클래스 Type명을 가리키게 되고,
                    클래스 객체를 생성하는 것처럼 new를 사용하여 Delegate 객체를 생성하고 이 객체에 특정 메서드를 연관시켜 할당하게 된다.
                    무명메서드는 이름이 없는 메서드 자체 만을 가리키는 것으로 그 자체로 Delegate 타입이 되는 것은 아니다.
                    따라서 아래 (무명메서드1) 예제처럼 EventHandler Delegate 객체를 new로 생성하고 무명메서드를 파라미터로 전달하게 된다.
                    하지만, Event 필드와 같이 이미 어떤 Delegate 타입이 사용될 지 아는 경우에는
                    (즉, Click 이벤트는 항상 new EventHandler(object, EventArgs)를 받아들인다는 것으로 안다),
                    아래 (무명메서드2/3) 예제처럼 new EventHandler()나 (EventHandler) 캐스팅을 생략할 수 있다.
                    또한 마지막으로 만약 파라미터를 무명메서드 안에서 사용하지 않는다면,
                    (무명메소드4) 예제 처럼 파라미터들을 완전히 생략할 수 있다.

                        // Delegate 타입 : 
                        public delegate int SumDelegate(int a, int b);
                      
                        // Delegate 사용 : 
                        SumDelegate sumDel = new SumDelegate(mySum);
                      
                        // 무명메서드1 
                        button1.Click += new EventHandler(delegate(object s, EventArgs a) { MessageBox.Show("OK"); });

                        // 무명메서드2
                        button1.Click += (EventHandler) delegate(object s, EventArgs a) { MessageBox.Show("OK"); };

                        // 무명메서드3 
                        button1.Click += delegate(object s, EventArgs a) { MessageBox.Show("OK"); };

                        // 무명메서드4 
                        button1.Click += delegate { MessageBox.Show("OK"); };                    
                */
                {
                    Console.ReadLine();
                }
            }

            // Delegate Type vs Anonymous Method (2)
            {
                /*
                    Delegate 타입을 사용해야 하는 곳에 무명메서드만 직접 사용하는 경우
                    컴파일 에러(Cannot convert anonymous method to type 'System.Delegate' because it is not a delegate type)가 발생할 수 있다.
                    아래 예처럼, Control.Invoke() 혹은 Control.BeginInvoke() 메서드는 델리게이트 Type을 파라미터로 받아들인다.
                    즉, Invoke() 메서드는 파라미터로 받아들이는 Delegate의 파리미터가 몇 개인지, 리턴 값은 무엇인지 미리 알지 못한다는 뜻이다.
                    따라서 무조건 무명메서드를 전달할 수 없고, 어떤 Delegate 타입인지를 지정해야 하고,
                    이런 조건이 충족되었을 때, Delegate 타입과 부합되는 무명메서드를 사용할 수 있다.

                        // 틀림: 컴파일 에러 발생
                        this.Invoke(delegate {button1.Text = s;} );

                        // 맞는 표현 
                        MethodInvoker mi = new MethodInvoker(delegate() { button1.Text = s; });
                        this.Invoke(mi);

                        // 축약된 표현
                        this.Invoke((MethodInvoker) delegate { button1.Text = s; });
                */
                {
                    Console.ReadLine();
                }
            }
        }


        public static void Test()
        {
            //delegate_vs_anonymous_method();

            //anonymous_method_use();

            //anonymous_method_what();
        }
    }
}
