using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class ClassInheritance
    {
        static void inheritance_class()
        {
            /*
                📄 class inheritance

                  C#에서 일종의 부모 클래스인 베이스 클래스(base class)로부터 상속하여 새로운 파생 클래스(derived class)를 만들 수 있다.
                  상속(inheritance)을 사용하게 되면 베이스 클래스의 데이타 및 메서드들을 (public 혹은 protected 멤버의 경우) 파생클래스에서 사용할 수 있게 된다.
                  파생 클래스는 베이스 클래스로부터 물려 받는 멤버들 외에 대개 새로운 메서드 및 데이타를 추가해서 사용하게 된다.

                  C#에서는 파생 클래스명 뒤에 Colon (:)을 찍고 베이스 클래스명을 써 주면 상속이 이루어 진다.
                  제약점은 C#에서는 파생클래스가 단 하나의 베이스 클래스로부터 상속되어져야 한다는 것이다.
                  즉, 하나의 파생클래스는 2개 이상의 베이스 클래스를 가질 수 없다.
                  아래 예는 파생클래스가 베이스 클래스의 속성(Property)을 사용하는 예이다.


                    // 베이스 클래스
                    public class Animal
                    {
                        public string Name { get; set; }
                        public int Age { get; set; }
                    }

                    // 파생클래스
                    public class Dog : Animal
                    {       
                        public void HowOld() 
                        {
                            // 베이스 클래스의 Age 속성 사용
                            Console.WriteLine("나이: {0}", this.Age);
                        }
                    }

                    public class Bird : Animal
                    {       
                        public void Fly()
                        {
                            Console.WriteLine("{0}가 날다", this.Name);
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }        


        static void abstract_class()
        {
            /*
                📄 abstract class 
            
                  C#의 클래스명 앞에 abstract라는 C# 키워드 붙이는 경우가 있다.
                  이를 추상 클래스라고 하는데, 이러한 종류의 클래스로부터는 객체를 직접 생성할 수 없다.
                  즉, new를 이용하여 클래스 객체를 생성할 수 없다.

                  또한 추상 클래스안에 어떤 멤버 앞에 abstract 키워드를 붙이는 경우가 있는데, 이는 해당 멤버가 구현되지 않았으며,
                  추상 클래스로부터 파생되는 파생클래스에서 반드시 그 멤버를 구현해 주어야 한다는 것을 의미한다.
                  파생 클래스에서는 abstract 메서드를 구현하기 위해서는 override 라는 C# 키워드를 사용하여 그 메서드를 새로 정의한다.


                    public abstract class PureBase
                    {
                        // abstract C#키워드 
                        public abstract int GetFirst();
                        public abstract int GetNext();   
                    }

                    public class DerivedA : PureBase
                    {
                        private int no = 1;

                        // override C#키워드 
                        public override int GetFirst()
                        {
                            return no;
                        }

                        public override int GetNext()
                        {
                            return ++no;
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void public_protected_member()
        {
            /*
                📄 public and protected member    

                  C#의 클래스 멤버 중 public으로 선언된 멤버들은 (파생클래스 포함한) 모든 외부 클래스에서 엑세스 할 수 있다.
                  만약 모든 클래스에서 사용되지 않게 하고 단지 파생클래스에서만 사용하게 하고 싶다면,
                  protected 라는 접근제한자(Access Modifier)를 사용한다.


                    public class MyBase
                    {
                        public string Name { get; set; }
                        protected int Age { get; set; }
                    }

                    public class MyDerived : MyBase
                    {       
                        public void Run()
                        {      
                            // 파생클래스이므로 Age 사용 가능
                            Console.WriteLine("나이: {0}", this.Age);
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void as_n_is_operator()
        {
            /*
                📄 as and is operator

                  C#의 as 연산자는 객체를 지정된 클래스 타입으로 변환하는데 사용된다.
                  만약 변환이 성공하면 해당 클래스 타입으로 캐스팅하고, 변환이 실패하면 null 을 리턴한다. 

                  이와는 대조적으로 암묵적 캐스팅(Implicit Casting)은 만약 변환이 실패하면 Exception을 발생시키게 되는데,
                  이를 catch하지 않은 경우 프로그램을 중지시키게 된다.

                  C#의 is 연산자는 is 앞에 있는 객체가 특정 클래스 타입이나 인터페이스를 갖고 있는지 확인하는데 사용된다.


                    class MyBase { }
                    class MyClass : MyBase { }

                    class Program
                    {
                        static void Main(string[] args)
                        {
                            MyClass c = new MyClass();
                            new Program().Test(c);
                        }

                        public void Test(object obj)
                        {
                            // as 연산자
                            MyBase a = obj as MyBase; 

                            // is 연산자
                            bool ok = obj is MyBase; //true

                            // 암묵적 캐스팅(Implicit Casting)
                            MyBase b = (MyBase) obj; 
                        }
                    }
            */
            {
                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //as_n_is_operator();

            //public_protected_member();

            //abstract_class();

            //inheritance_class();
        }
    }
}
