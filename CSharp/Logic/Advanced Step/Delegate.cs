using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class Delegate
    {
        delegate void MyEventDelegate(int a);

        public delegate int MyDelegate(int a, int b);

        public delegate T MyDelegate<T>(T a, T b);


        public static int Plus(int a, int b) { return a + b; }
        public static float Plus(float a, float b) { return a + b; }
        public static double Plus(double a, double b) { return a + b; }
        public static int Minus(int a, int b) { return a - b; }
        public static int Multiply(int a, int b) { return a * b; }

        public static void Calculator(int a, int b, MyDelegate dele)
        {
            Console.WriteLine(dele(a, b));
        }

        public static void Calculator<T>(T a, T b, MyDelegate<T> dele)
        {
            Console.WriteLine(dele(a, b));
        }

        static void delegate_what()
        {
            /*
                C# delegate는 약간 낯선 개념이고 경우에 따라 어렵게 느껴질 수 있다.
                여기서 그 기초 개념이 무엇인지 자세히 집어보도록 하자.

                다음과 같은 하나의 함수가 있다고 가정하자.
                (주: 여기서의 함수는 개념적으로 메서드와 동일한 의미로 가정)

                    void RunA(int i) { ... }

                이 함수는 정수 하나를 파라미터로 받아들인다.
                이 함수를 호출하기 위해서는 아래와 같이 정수를 메서드 파라미터로 넘기면 된다.

                    int j = 123;
                    RunA(j);

                좀 더 복잡한 경우로서 다음과 같이 클래스 객체를 넘기는 경우를 생각해 볼 수 있다.

                    void RunB(MyClass c) { ... }

                이 경우 MyClass는 위의 int와 같은 Built-in 타입이 아니므로
                개발자가 그 타입을 어딘가에서 정의해 주어여야 한다 (예를 들어 아래와 같이).

                    class MyClass
                    {
                        int id;   //필드
                        string name;       
                        public void Display() { }  //메서드
                    }

                이 함수(RunB)를 호출하기 위해서는 아래와 같이 클래스 인스턴스를 만들어
                이를 메서드 파라미터로 넘기면 된다.

                    MyClass c = new MyClass();
                    RunB(c);

                그런데 위의 2가지 케이스를 자세히 보면, 파라미터로서 정수 혹은 객체
                즉 어떤 "데이타"를 메서드에 보내고 있는 것을 알 수 있다.

                그러면, 이러한 통상적인 개념의 물리적 "데이타" 말고, 어떤 "메서드" 자체를
                함수(메서드)의 파라미터로 전달할 수 있을까?

                (주: 사실 "추상적 개념으로" 클래스 객체는 데이타(필드)와 행위(메서드)를 함께 포함하고 있는 것이고,
                이를 메서드 파라미터로 보낼 수 있다는 것은,
                클래스의 일부인 메서드만을 보낼 수 있음을 (보낼 수 있게 만들 수 있음을) 의미하기도 한다.)

                Delegate는 이렇게 메서드를 다른 메서드로 전달할 수 있도록 하기 위해 만들어 졌다.
                아래 그림에서 보이듯이, 숫자 혹은 객체를 메서드의 파라미터로써 전달할 수 있듯이,
                메서드 또한 파라미터로서 전달할 수 있다. (주: 복수 개의 메서드들도 전달 가능)
                Delegate는 메서드의 입력 파라미터로 피호출자에게 전달될 수 있을 뿐만 아니라,
                또한 메서드의 리턴값으로 호출자에게 전달 수도 있다.

                    void Run( int i )

                    void Run( MyClass c )

                    void Run( MyDelegate d )                    

                예를 들어, 다음과 같은 함수를 가정해 보자.
                여기서 MyDelegate가 델리게이트 타입이라고 가정하면,
                이 함수는 다른 어떤 메서드를 Run() 메서드에 전달하고 있는 것이다.

                    void Run(MyDelegate method) { ... }

                그러면 델리게이트 타입 MyDelegate은 어떻게 정의되는가?
                위의 클래스 예제(MyClass)의 경우 C# 키워드 class 를 사용하여 해당 클래스를 정의하였는데,
                델리케이트 타입을 정의하기 위해선 특별한 C# 키워드 delegate 를 사용한다.

                    delegate int MyDelegate(string s);

                델리케이트 정의에서 중요한 것은 입력 파리미터들과 리턴 타입이다.
                만약 어떤 메서드가 이러한 델리게이트 메서드 원형(Prototype)과 일치한다면,
                즉 입력 파리미터 타입 및 갯수, 리턴 타입이 동일하다면 그 메서드는 해당 델리게이트에서 사용될 수 있다.

                델리케이트 정의는 마치 하나의 함수/메서드를 정의하는 Prototype 선언식처럼 보이는데,
                사실 내부적으로 이 선언식은 컴파일러에 의해 특별한 클래스로 변환된다. 
                (주: C# 컴파일러는 위의 delegate 정의를 읽어,
                 System.MulticastDelegate 클래스로부터 파생된 MyDelegate 클래스를 생성하게 된다.
                 따라서 delegate는 메서드를 전달하기 위해 메서드 메타정보를 내부에 갖고 있는 특별한 종류의 Wrapper 클래스라 볼 수 있다.
                 그러면, 델리게이트 생성시 C# delegate 키워드 말고 직접 System.MulticastDelegate 클래스로부터 파생된 클래스를 만들 수 있을까?
                 이는 불가능하다.
                 System.MulticastDelegate 클래스는 특별한 .NET 클래스로 Base클래스로 사용될 수 없다.)

                이렇게 C# delegate 식을 클래스가 아닌 함수 선억식처럼 정의하게 한 것은
                내부의 복잡한 설계를 숨기고 '메서드를 전달하는 본연의 의도'를 더 직관적으로 표현하기 위한 것으로 볼 수 있다.

                델리게이트가 이렇게 정의된 후에는 클래스 객체를 생성한 것과 비슷한 방식으로 new를 써서 델리케이트 객체를 생성한다.
                (주: delegate는 결국 클래스이므로 클래스 객체 생성과 같은 방식을 사용한다)

                델리케이트를 다른 메서드에 전달하는 방식은 델리케이트 객체를 메서드 호출 파라미터에 넣으면 된다.
                이는 메서드를, 좀 더 정확히는 그 메서드 정보를 갖는 Wrapper 클래스의 객체를, 파라미터로 전달하는 것이 된다.

                    // int StringToInt(string s) { ... }

                    MyDelegate m = new MyDelegate(StringToInt);
                    Run(m);

                전달된 델리게이트로부터 실제 메서드를 호출하는 것은 어떻게 하는가?
                이는 델리게이트 객체의 .Invoke() 메서드나 .BeginInvoke() 메서드를 써서 호출한다.
                예를 들어, m 이라는 델리게이트 객체를 전달 받았을 경우, 아래와 같이 Invoke() 메서드를 호출한다.
                만약 메서드에 입력파라미터가 있을 경우, 이를 Invoke() 안에 추가한다.

                    i = m.Invoke("123");

                또 다른 메서드 호출방법으로 C# 프로그래머들이 더 애용하는 방식은,
                .Invoke 메서드명을 생략하고 다음과 같이 직접 함수처럼 사용하는 방법이다.
                이 방식은 마치 메서드를 직접 호출하는 느낌을 주므로 더 직관적이다.

                    i = m("123");

                참고로, 아래 예제는 위의 설명을 종합한 간단한 delegate 샘플이다.

                    class Program
                    {
                        static void Main(string[] args)
                        {
                            new Program().Test();
                        }

                        // 델리게이트 정의
                        delegate int MyDelegate(string s);

                        void Test()
                        {
                            //델리게이트 객체 생성
                            MyDelegate m = new MyDelegate(StringToInt);

                            //델리게이트 객체를 메서드로 전달
                            Run(m);
                        }

                        // 델리게이트 대상이 되는 어떤 메서드
                        int StringToInt(string s)
                        {
                            return int.Parse(s);
                        }

                        // 델리게이트를 전달 받는 메서드
                        void Run(MyDelegate m)
                        {
                            // 델리게이트로부터 메서드 실행
                            int i = m("123");

                            Console.WriteLine(i);
                        }
                    }
            */
            {
                // Delegate 변수 활용
                MyDelegate calculate;

                calculate = new MyDelegate(Plus);
                int sum = calculate(11, 22);
                Console.WriteLine("sum: {0}", sum);

                calculate = new MyDelegate(Minus);
                Console.WriteLine("sum: {0}", calculate(33, 44));

                Console.ReadLine();
            }
            {
                // Delegate Callback 활용
                MyDelegate plus = new MyDelegate(Plus);
                MyDelegate minus = new MyDelegate(Minus);
                MyDelegate multiply = new MyDelegate(Multiply);

                Calculator(11, 22, plus);
                Calculator(44, 33, minus);
                Calculator(10, 50, multiply);

                Console.ReadLine();
            }
        }


        static void delegate_concept()
        {
            /*
                C# delegate는 C/C++의 함수 포인터와 비슷한 개념으로 메서드 파라미터와 리턴 타입에 대한 정의를 한 후,
                동일한 파라미터와 리턴 타입을 가진 메서드를 서로 호환해서 불러 쓸 수 있는 기능이다.
                예를 들면, 아래 RunDelegate 델리게이트는 입력 파라미터가 int 하나이고 리턴 값이 없는 메서드를 가리킨다.
                RunThis() 메서드와 RunThat()메서드는 모두 int 파라미터 하나에 리턴 값이 없는 메서드이므로,
                RunDelegate의 함수 형식(prototype)과 맞으므로 이 delegate를 사용할 수 있다.

                    using System;
                    namespace MySystem
                    {
                        class MyClass
                        {
                            // 1. delegate 선언
                            private delegate void RunDelegate(int i);

                            private void RunThis(int val)
                            {
                                // 콘솔출력 : 1024
                                Console.WriteLine("{0}", val);
                            }

                            private void RunThat(int value)
                            {
                                // 콘솔출력 : 0x400
                                Console.WriteLine("0x{0:X}", value);
                            }

                            public void Perform()
                            {
                                // 2. delegate 인스턴스 생성
                                RunDelegate run = new RunDelegate(RunThis);
                                // 3. delegate 실행
                                run(1024);

                                //run = new RunDelegate(RunThat); 을 줄여서 
                                //아래와 같이 쓸 수 있다.
                                run = RunThat;

                                run(1024);
                            }
                        }

                        class Program
                        {
                            static void Main(string[] args)
                            {
                                MyClass mc = new MyClass();
                                mc.Perform();
                            }
                        }
                    }                        
            */
            {
                Console.ReadLine();
            }
        }

        
        static void delegate_forward()
        {
            /*
                Delegate는 동일한 함수 Prototype을 갖는 메서드를 가리키므로 함수의 포인터를 파라미터로 전달하듯,
                다른 함수의 파라미터로 사용될 수 있다.
                delegate 파라미터를 전달받은 쪽은 이를 자신의 내부 함수를 호출하듯 사용할 수 있다.
                (C# delegate는 내부적으로 .NET Delegate / MulticastDelegate 클래스를 사용한다.
                 따라서 이 클래스가 지원하는 속성 (예: .Method - 함수 Prototype을 보여줌)과 메서드 (예: GetInvokcationList())를 모두 사용할 수 있다)

                아래 예제는 올림차순으로 비교하는 함수(AscendingCompare) 와 내림차순으로 비교하는 함수(DescendingCompare)를 delegate로 전달하여,
                하나의 Sort메서드에서 비교함수에 따라 여러 개의 소트가 가능하다는 것을 보여주는 예이다.

                    class MySort
                    {
                        // 델리게이트 CompareDelegate 선언
                        public delegate int CompareDelegate(int i1, int i2);

                        public static void Sort(int[] arr, CompareDelegate comp)
                        {
                            if (arr.Length < 2) return;
                            Console.WriteLine("함수 Prototype: " + comp.Method);

                            int ret;
                            for (int i = 0; i < arr.Length - 1; i++)
                            {
                                for (int j = i+1; j < arr.Length; j++)
                                {
                                    ret = comp(arr[i], arr[j]);
                                    if (ret == -1)
                                    {
                                        // 교환
                                        int tmp = arr[j];
                                        arr[j] = arr[i];
                                        arr[i] = tmp;
                                    }
                                }
                            }
                            Display(arr);
                        }
                        static void Display(int[] arr)
                        {
                            foreach (var i in arr) Console.Write(i + " ");
                            Console.WriteLine();
                        }
                    }

                    class Program
                    {
                        static void Main(string[] args)
                        {
                            (new Program()).Run();
                        }

                        void Run()
                        {
                            int[] a = { 5, 53, 3, 7, 1 };

                            // 올림차순으로 소트
                            MySort.CompareDelegate compDelegate = AscendingCompare;
                            MySort.Sort(a, compDelegate);

                            // 내림차순으로 소트
                            compDelegate = DescendingCompare;
                            MySort.Sort(a, compDelegate);            
                        }

                        // CompareDelegate 델리게이트와 동일한 Prototype
                        int AscendingCompare(int i1, int i2)
                        {
                            if (i1 == i2) return 0;
                            return (i2 - i1) > 0 ? 1 : -1;
                        }

                        // CompareDelegate 델리게이트와 동일한 Prototype
                        int DescendingCompare(int i1, int i2)
                        {
                            if (i1 == i2) return 0;
                            return (i1 - i2) > 0 ? 1 : -1;
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void delegate_field_and_attribute()
        {
            /*
                Delegate 전달 에서 보듯이, delegate는 메서드의 파라미터로 전달될 수 있다.
                이와 맥락을 같이하여 delegate는 또한 클래스의 필드나 속성에 사용될 수 있다.
                일종의 함수 포인터를 필드나 속성에 저장 하는 것과 비슷한 맥락이다.
                메서드 파라미터로 전달하던, 필드로 저장하던 클래스 객체의 입장에서는 전달된 delegate를 필요에 따라 자신의 클래스 내에서 사용하면 된다.

                아래 예는 delegate 필드(MyClick)를 정의한 후, 클래스 내부 함수(MyAreaClicked)에서 delegate 필드가 NULL이 아니면,
                해당 delegate를 실행하는( MyClick(this); ) 코드를 보여준다.
                delegate 실행은 메서드를 호출하는 것과 같은데, 델리게이트 필드 자신이 마치 메서드명인 것처럼 사용하면 된다.  

                    using System.Windows.Forms;

                    namespace MySystem
                    {
                        class MyArea : Form
                        {
                            public MyArea()
                            {
                                // 이 부분은 당분간 무시 (무명메서드 참조)
                                // 예제를 테스트하기 위한 용도임.
                                this.MouseClick += delegate { MyAreaClicked(); };
                            }

                            public delegate void ClickDelegate(object sender);

                            // Delegate 필드
                            public ClickDelegate MyClick;

                            // 필드외 Delegate 프로퍼티도 가능
                            //public ClickDelegate Click { get; set; }

                            //...
                            //...

                            // 예제를 단순화 하기 위해
                            // MyArea가 클릭되면 아래 함수가 호출된다고 가정
                            void MyAreaClicked()
                            {
                                if (MyClick != null)
                                {
                                    MyClick(this);
                                }
                            }
                        }

                        class Program
                        {
                            static MyArea area;

                            static void Main(string[] args)
                            {
                                area = new MyArea();
                                area.MyClick = Area_Click;
                                area.ShowDialog();
                            }

                            static void Area_Click(object sender)
                            {
                                area.Text = "MyArea 클릭!";
                            }
                        }
                    }                    
            */
            {
                Console.ReadLine();
            }
        }


        static void delegate_multicast_and_chain()
        {
            /*
                Delegate는 여러 개의 메서드들을 할당하는 것이 가능하다.
                C# 연산자 += 을 사용하면 메서드를 계속 delegate 에 추가하게 되는데,
                내부적으로는 .NET MulticastDelegate 클래스에서
                이 메서드들의 리스트(이를 InvocationList 라고 한다)를 관리하게 된다.

                복수개의 메서드들이 한 delegate에 할당되면, 이 delegate가 실행될 때,
                InvocationList로부터 순서대로 메서드를 하나씩 가져와 실행한다.
                아래 예제는 복수 개의 메서드를 한 delegate에 계속 추가하는 예이다.

                    class Program
                    {
                        static MyArea area;

                        static void Main(string[] args)
                        {
                            area = new MyArea();

                            //복수개의 메서드를 delegate에 할당(Chain)
                            area.MyClick += Area_Click;
                            area.MyClick += AfterClick;

                            area.ShowDialog();
                        }

                        static void Area_Click(object sender)
                        {
                            area.Text += " MyArea 클릭! ";
                        }

                        static void AfterClick(object sender)
                        {
                            area.Text += " AfterClick 클릭! ";
                        }
                    }
            */
            {
                MyDelegate dele;
                dele = new MyDelegate(Plus);
                dele += Minus;
                dele += Multiply;

                dele(10, 100);

                dele -= Plus;
                dele -= Multiply;

                dele(55, 10);

                Console.ReadLine();
            }
        }


        class EventManager
        {
            public event MyEventDelegate eventCall;

            public void NumberCheck(int num)
            {
                if (num % 2 == 0)
                    eventCall(num);
            }
        }

        static void EvenNumber(int num)
        {
            Console.WriteLine("{0} is even number", num);
        }

        static void delegate_event()
        {
            /*
                모든 이벤트(event)는 특수한 형태의 delegate이다.
                C#의 delegate 기능은 경우에 따라 잘못 사용될 소지가 있다.
                예를 들어, 우리가 Button 컨트롤을 개발해 판매한다고 하자.
                이 컨트롤은 delegate 필드를 가지고 있고, 클릭시 InvocationList에 있는 모든 메서드들을 차례로 실행하게 하였다.
                그런데, Button컨트롤을 구입한 개발자는 한 컴포넌트에서 추가 연산(+=)을 하지 않고 할당 연산( = )을 하였다.
                이 할당연산은 기존에 가입된 모든 메서드 리스트를 지워버리고 마지막 할당된 메서드 1개만 InvocationList에 넣게 한다.
                즉, 누구든 할당 연산자를 한번 사용하면 기존에 가입받은 모든 메서드 호출요구를 삭제하는 문제가 발생한다.

                이러한 문제점과 더불어 또다른 중요한 문제점은 delegate는 해당 클래스 내부에서 뿐만 아니라,
                외부에서도 누구라도 메서드를 호출하듯 해당 delegate를 호출할 수 있다는 점이다.

                주의할 점은 event 변수는 메소드를 참조할때 += 로만 참조 가능하다.
                아래 예제는 할당연산자를 사용해서 기존 delegate를 덮어쓰는 예와 delegate를 외부에서 호출하는 예를 보여준다.

                    using System.Windows.Forms;
                    namespace MySystem
                    {
                        class MyArea : Form
                        {
                            public MyArea()
                            {
                                // 이 부분은 당분간 무시. (무명메서드 참조)
                                // 예제를 테스트하기 위한 용도임.
                                this.MouseClick += delegate { MyAreaClicked(); };
                            }

                            public delegate void ClickEvent(object sender);

                            // event 필드
                            public event ClickEvent MyClick;

                            // 예제를 단순화 하기 위해
                            // MyArea가 클릭되면 아래 함수가 호출된다고 가정
                            void MyAreaClicked()
                            {
                                if (MyClick != null)
                                {
                                    MyClick(this);
                                }
                            }
                        }

                        class Program
                        {
                            static MyArea area;

                            static void Main(string[] args)
                            {
                                area = new MyArea();

                                // 이벤트 가입
                                area.MyClick += Area_Click;
                                area.MyClick += AfterClick;

                                // 이벤트 탈퇴
                                area.MyClick -= Area_Click;

                                // Error: 이벤트 직접호출 불가
                                //area.MyClick(this);

                                area.ShowDialog();
                            }

                            static void Area_Click(object sender)
                            {
                                area.Text += " MyArea 클릭! ";      
                            }

                            static void AfterClick(object sender)
                            {
                                area.Text += " AfterClick 클릭! ";      
                            }
                        }
                    }

            */
            {
                EventManager em = new EventManager();
                em.eventCall += new MyEventDelegate(EvenNumber);

                for (int i = 1; i < 10; ++i)
                {
                    em.NumberCheck(i);
                }

                Console.ReadLine();
            }
        }


        static void delegate_generic()
        {
            /*
                Delegate 를 일반화 하면 어떤 타입의 메소드든지 참조 할 수 있다.
            */
            {
                MyDelegate<int> plus_i = new MyDelegate<int>(Plus);
                MyDelegate<float> plus_f = new MyDelegate<float>(Plus);
                MyDelegate<double> plus_d = new MyDelegate<double>(Plus);

                Calculator(10, 22, plus_i);
                Calculator(3.5f, 5.5f, plus_f);
                Calculator(5.5, 2.4, plus_d);

                Console.ReadLine();
            }
        }


        static void delegate_function_pointer()
        {
            /*
                Delegate는 메서드의 레퍼런스를 갖고 있다는 점에서 C의 함수 포인터(function pointer)와 닮았다.
                하지만 C# delegate는 몇 가지 측면에서 C의 함수 포인터와 다르다.

                첫번째로 클래스의 개념이 없는 C에서의 함수 포인터는 말 그대로 외부의 어떤 함수에 대한 주소값만을 갖는다.
                반면 C#의 delegate는 클래스 객체의 인스턴스 메서드에 대한 레퍼런스를 갖기 위해 그 C# 객체의 주소(객체 레퍼런스)와 메서드 주소를 함께 가지고 있다.
                (주: 물론 Static 메서드의 경우에는 객체의 레퍼런스값이 null 이 된다)
                C# delegate는 델리게이트 Type을 정의하는 것으로 이 Type으로부터 델리게이트 객체를 생성할 때, 이 객체가 메서드 정보와 객체 정보를 가진다.
                클래스를 사용하는 C++에는 Pointer to member function이 있는데,
                이는 한 클래스의 멤버 함수에 대한 포인터로서 '객체'에 대한 컨텍스트를 가지고 있다는 점에서 C#의 delegate와 비슷하다.
                단, C#의 delegate는 메서드 Prototype이 같다면 어느 클래스의 메서드도 쉽게 할당할 수 있는데 반해,
                C++의 Pointer to member는 함수 포인터 선언시 특정 클래스를 지정해주기 때문에 한 클래스에 대해서만 사용할 수 있다.

                두번째로 C의 함수 포인터는 하나의 함수 포인터를 갖는데 반해,
                C# delegate는 하나 이상의 메서드 레퍼런스들을 가질 수 있어서 Multicast가 가능하다.

                또한 C의 함수포인터는 Type Safety를 완전히 보장하지 않는 반면, C#의 delegate는 엄격하게 Type Safety를 보장한다.

                    // C 함수 포인터 예제
                    void myfunc(int x) 
                    {
                        printf( "%d\n", x );
                    }

                    void main()
                    {
                        // 함수포인터 f 정의
                        void (*f)(int);    

                        // 함수포인터에 함수 지정
                        f = &myfunc;

                        // 함수 실행
                        f(2);
                    }

                    // C++ Pointer to member 예제
                    #include <iostream>
                    #include <string>
                    class Cls
                    {
                    public:
                        // 클래스 메서드 멤버
                        void myfunc(std::string str)
                        {
                            std::cout << str << std::endl;    
                        }
                    };

                    void main()
                    {
                        // Pointer to member function 정의
                        void (Cls::*fp)(std::string);

                        // Pointer to member 지정
                        fp = &Cls::myfunc;

                        // Cls 객체 생성 및 객체 포인터 지정
                        Cls obj;
                        Cls* pObj = &obj;

                        // Cls 객체에서 함수포인터 사용
                        (pObj->*fp)("hello");
                    }                    
            */
            {
                Console.ReadLine();
            }
        }


        static void delegate_Func_and_Action_what()
        {
            /*
                프로그램을 작성하던 중에 무명 메소드가 필요해졌다고 생각해 보자.
                무명 메소드를 사용하기 위해서는 이를 참조할 수 있는 Delegate 변수가 있어야 하며,
                또한 Delegate 변수를 생성 하기에 앞서 Delegate Type 을 선언해야 한다.

                그러면 각기 다른 Type의 무명 메소드를 여러개 만들 때는 어떻게 해야할까?
                당연히 무명 메소드 마다 그 Type 에 맞는 Delegate Type 과 변수를 따로 따로 선언해야 할 것이다.
                이는 매우 비효율적인 작업이기 때문에 C# 에서는 Func 과 Action 이라는 Delegate 를 제공 한다.

                Func 와 Action 은 미리 선언된 Delegate 변수로써 별도의 선언 없이 사용 가능 하다.
                Func 는 반환값이 있는 메소드를 참조하는 Delegate 변수 이고,
                Action 은 반환값이 없는 메소드를 참조하는 Delegate 변수 이다.
            */
            {
                Console.ReadLine();
            }
        }


        static float Func3(int a, int b, int c)
        {
            return (a + b + c) * 0.1f;
        }

        static void delegate_Func()
        {
            /*
                .Net Framework 에는 총 17가지의 Func Delegate 가 준비되어 있다.
                즉, 매개변수가 없는 메소드 부터 매개변수가 16개인 메소드까지
                총 17개의 메소드를 참조 가능하다는 말이다. (무명 메소드 뿐만 아니라 일반 메소드도 참조 가능)
                이 정도면 특별한 경우가 아니고서야 별도의 Delegate 를 만들어 쓸 필요가 없겠다.
                Func Delegate 변수를 선언 하는 방법은 다음과 같다.

                    // 매개변수 X , 반환값 float
                    Func<float> func0 = () => 0.1f;

                    // 매개변수 int 1개, 반환값 float
                    Func<int, float> func1 = (a) => a * 0.1f;

                    // 매개변수 int 2개, 반환값 float
                    Func<int, int, float> func2 = (b, c) => (b + c) * 0.1f;

                위 코드를 보면 매개변수는 앞쪽에 지정하고, 반환값은 맨뒤에 지정 한다.
                Func Delegate 는 반환값을 갖는 메소드를 참조하는 Delegate 이기 때문에 반드시 반환형을 지정해주어야 한다.
                위와 같이 Func Delegate 로 메소드를 참조하면 전처럼 Delegate Type 을 선언하는 과정이
                불필요해지므로 아주 간결하게 코드를 작성할 수 있다.
            */
            {
                Func<float> func0 = () => 0.1f;
                Func<int, float> func1 = (a) => a * 0.1f;
                Func<int, int, float> func2 = (b, c) => (b + c) * 0.1f;

                Func<int, int, int, float> func3;

                func3 = new Func<int, int, int, float>(Func3);

                Console.WriteLine("func0 return: {0}", func0());
                Console.WriteLine("func1 return: {0}", func1(10));
                Console.WriteLine("func2 return: {0}", func2(10, 100));
                Console.WriteLine("func3 return: {0}", func3(10, 100, 1000));

                Console.ReadLine();
            }
        }


        static void Func1(string name)
        {
            Console.WriteLine("name: {0}", name);
        }

        static void delegate_Action()
        {
            /*
                Action Delegate 는 Func Delegate 와 유사하다.
                단지 참조 하는 메소드의 반환값이 없다.

                    // 매개변수 X
                    Action act0 = () => Console.Write("Action Delegate");

                    // 매개변수 string 1개
                    Action<string> act1 = (name) => Console.Write(name);

                    // 매개변수 string 2개
                    Action<string, string> act2 = (name, age) => Console.Write(name + age);

                    // 매개변수 int 3개
                    int sum = 0;
                    Action<int, int, int> act3 = (a, b, c) => sum = a + b + c;
            */
            {
                int sum = 0;

                Action act0 = () => Console.WriteLine("name: act0");
                Action<string> act1 = new Action<string>(Func1);
                Action<string, string> act2 = (name, age) =>
                {
                    Console.Write("name: {0}", name);
                    Console.WriteLine("age: {0}", age);
                };

                Action<int, int, int> act3 = (a, b, c) => sum = a + b + c;

                act0();
                act1("call act1");
                act2("call act2", "24");
                act3(100, 200, 300);

                Console.WriteLine("sum: {0}", sum);

                Console.ReadLine();
            }
        }
        

        static void delegate_Predicate_what()
        {
            /*
                .NET 의 Predicate<T> delegate 는 Action/Func delegate 와 비슷한데,
                리턴값이 반드시 bool이고 입력값이 T 타입인 delegate이다.
                Action이나 Func와 달리, 입력 파라미터는 1개이다.
                이 특수한 delegate는 .NET의 Array나 List 클래스의 메서드들에서 자주 사용된다.
                Predicate<T>은 Func<T, bool>와 같이 표현할 수 있는데, Func이 실제로 보다 많은 함수들을 표현할 수 있다.
                Predicate은 .NET 2.0에서 Array나 List등을 지원하기 위해 만들어 졌으며,
                보다 일반화된 Func는 .NET 3.5에서 도입되어 LINQ 등을 지원하도록 만들어 졌다. 
            */
            {
                Predicate<int> p = delegate (int n)
                {
                    return n >= 0;
                };
                bool res = p(-1);

                Predicate<string> p2 = s => s.StartsWith("A");
                res = p2("Apple");

                Console.ReadLine();
            }
        }


        static bool IsPositive(int i)
        {
            return i >= 0;
        }

        static void delegate_Predicate_and_Func()
        {
            /*
                Action, Predicate, Func 등의 Delegate는 .NET Framework에서 많이 사용되는데,
                많은 경우 이들 Delegate들은 .NET Framework의 기존 메서드들에서 요구되는 파라미터로 사용되는 경우가 많다.
                특히, Predicate는 Array나 List의 메서드들에서 많이 사용되고, Func는 LINQ 에서 많이 사용된다.
                아래 예제는 Array.Find() 메서드에서 Predicate을 파라미터로 받아들이는 경우와
                LINQ의 Where() 메서드에서 Func를 사용한 예이다.
            */
            {
                int[] arr = { -10, 20, -30, 4, -5 };

                // Predicate의 사용
                // Array.Find( int[], Predicate<int> )    
                int pos = Array.Find(arr, IsPositive);

                // LINQ에서 Func의 사용
                // Where( Func<int, bool> predicate )
                var v = arr.Where(n => n >= 0); // Where()메서드의 IntelliSense를 Capture한 것으로 Func의 프로토타입을 볼 수 있다.

                IEnumerable<int> resultList = v;
                foreach (int value in resultList)
                {
                    Console.Write(value);
                }

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //delegate_Predicate_and_Func();

            //delegate_Predicate_what();

            //delegate_Action();

            //delegate_Func();

            //delegate_Func_and_Action_what();

            //delegate_function_pointer();

            //delegate_generic();

            //delegate_event();

            //delegate_multicast_and_chain();

            //delegate_field_and_attribute();

            //delegate_forward();

            //delegate_concept();

            //delegate_what();
        }
    }
}
