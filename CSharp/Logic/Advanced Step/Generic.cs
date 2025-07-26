using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdvancedStep
{
    public class Generic
    {
        //어떤 요소 타입도 받아들 일 수 있는
        //스택 클래스를 C# Generics를 이용해 정의함
        class Stack<T>
        {
            T [] elements;
            int pos = 0;

            public Stack()
            {
                elements = new T[100];
            }

            public void Push(T element)
            {
                elements[++pos] = element;
            }

            public T Pop()
            {
                return elements[pos--];
            }
        }

        //type parameter T in angle brackets
        class GenericList<T>
        {
            //the nested class is also generic on T.
            private class Node
            {
                //T used in non-generic constructor.
                public Node(T t)
                {
                    next = null;
                    data = t;
                }

                private Node next;
                public Node Next
                {
                    get { return next; }
                    set { next = value; }
                }

                //T as private member data type.
                private T data;

                //T as return type of property.
                public T Data
                {
                    get { return data; }
                    set { data = value; }
                }
            }

            private Node head;

            //constructor
            public GenericList()
            {
                head = null;
            }

            //T as method parameter type:
            public void AddHead(T t)
            {
                Node n = new Node(t);
                n.Next = head;
                head = n;
            }

            //foreach 구문에서 반환시 필요
            public System.Collections.Generic.IEnumerator<T> GetEnumerator()
            {
                Node current = head;

                while (current != null)
                {
                    yield return current.Data;
                    current = current.Next;
                }
            }
        }

        static void generic_what()
        {
            /*
                일반적으로 클래스를 정의할 때, 클래스 내의 모든 데이타 타입을 지정해 주게 된다.
                하지만 어떤 경우는 클래스의 거의 모든 부분이 동일한데 한 두개의 데이타 타입만이 다른 경우가 있을 수 있다.
                예를 들어, 사칙연산을 하는 클래스C 가 있다고 가정하자.
                이 클래스C에는 int 타입의 필드들이 있고, int 타입을 파라미터로 받아 계산하는 메서드들도 있다.
                그러면 이 클래스C를 double 타입의 데이타를 가지고 사용할 수 있을까? 그렇 수 없다.
                왜냐하면 이미 모든 필드 및 파라미터가 int로 설정되어 있기 때문이다.

                이런 경우 C#의 Generics를 사용할 수 있는데,
                Generics에서는 int, float, double 같은 데이타 요소 타입을 확정하지 않고
                이 데이타 타입 자체를 타입파라미터(Type Parameter)로 받아들이도록 클래스를 정의한다.
                이렇게 정의된 클래스 즉 C# Generics를 나중에 사용할 때는 클래스명과 함께 구체적인 사용 타입을 함께 지정해 준다.

                이렇게 하면 한 두개의 상이한 데이타 타입 때문에 여러 개의 클래스들을 따로 만들 필요가 없어지게 된다.
                C# Generics는 이러한 클래스 이외에도 인터페이스나 메서드에도 적용될 수 있다.

                요약하면, C# Generics는 C++의 템플릿과 비슷한 (주: 내부 아키텍쳐는 상당한 차이점이 있다) 개념으로서 클래스,
                인터페이스, 메서드 등에 <T> 같은 타입 파라미터를 붙여 구현한다.
                사용시에는 이 타입 파라미터에 특정 타입을 지정하게 되는데,
                실행(Runtime)시에 Generics 로부터 지정된 타입의 객체(object)를 구체적으로 생성해서 사용하게 된다.
           */
            {
                //두 개의 서로 다른 타입을 요소로 갖는
                //스택클래스 객체를 생성
                Stack<int> numberStack = new Stack<int>();
                Stack<string> nameStack = new Stack<string>();

                Console.ReadLine();
            }
            {
                //int is the type argument
                GenericList<int> list = new GenericList<int>();

                for (int x = 0; x < 10; x++)
                {
                    list.AddHead(x);
                }

                foreach (int i in list)//public IEnumerator<T> GetEnumerator() 구현 필요
                {
                    Console.Write(i + " ");
                }
                /*
                output:
                    9 8 7 6 5 4 3 2 1 0
                */

                Console.ReadLine();
            }
            /*
                ArrayList에 추가되는 모든 참조나 값 형식은 Object에 암시적으로 업캐스팅(upcast) 됩니다.
                항목(items)이 값 형식이면 이를 목록에 추가할 때 boxing 해야 하고 이를 검색할 때 unboxing 해야 합니다.
                캐스팅이나 boxing 및 unboxing 작업은 모두 성능을 저하 시킵니다.
            */
            {
                //The .NET Framework 1.1 way to create a list:
                System.Collections.ArrayList list1 = new System.Collections.ArrayList();
                list1.Add(3);
                list1.Add(105);

                System.Collections.ArrayList list2 = new System.Collections.ArrayList();
                list2.Add("It is raining in Redmond.");
                list2.Add("It is snowing in the mountains.");
            }
            /*
                유형이 다른 컬렉션(a heterogeneous collection)을 만드는 경우
                규칙을 정확히 따르는(perfectly acceptable) 의도적인(intentional) 선택일지라도
                문자열과 ints를 단일 ArrayList에 결합하면 프로그래밍 오류가 발생할 확률이 더 커지고
                이러한 오류는 런타임 이전에 발견할 수 없습니다.
            */
            {
                System.Collections.ArrayList list = new System.Collections.ArrayList();

                //add an integer to the list.
                list.Add(3);
                //add a string to the list.
                //this will compile, but may cause an error later.
                list.Add("It is raining in Redmond.");

                int t = 0;
                //this causes an InvalidCastException to be returned.
                foreach (int x in list)
                {
                    t += x;
                }
            }

            {
                //the .NET Framework 2.0 way to create a list
                List<int> list1 = new List<int>();

                //no boxing, no casting:
                list1.Add(3);

                //compile-time error:
                //list1.Add("It is raining in Redmond.");
            }
        }


        //T는 Value 타입
        struct CustomStruct { }

        //T는 Reference 타입
        class CustomClass { }

        class BaseNode { }
        class BaseNodeGeneric<T> { }

        //concrete type
        class NodeConcrete<T> : BaseNode { }

        //closed constructed type
        class NodeClosed<T> : BaseNodeGeneric<int> { }

        //open constructed type 
        class NodeOpen<T> : BaseNodeGeneric<T> { }

        static void generic_type_parameter()
        {
            /*
                1. 제네릭 형식 또는 메서드 정의에서 형식 매개 변수(a type parameters) 는 
                   클라이언트가 제네릭 형식의 변수를 인스턴스화할 때 지정하는 특정 형식에 대한 자리 표시자(placeholder) 입니다.

                2. 제네릭 소개에 나열된(listed) GenericList<T> 등의 제네릭 클래스는 실제로 형식이 아니고
                   형식에 대한 청사진(blueprint)과 같으므로 있는 그대로(as-is) 사용할 수는 없습니다.

                3. 클라이언트 코드에서 GenericList<T>를 사용하려면
                   꺾쇠괄호(the angle brackets) 내에 형식 매개 변수를 지정하는 방법으로
                   생성된 형식을 선언하고 인스턴스화 해야 합니다.

                4. 이 특정 클래스(this particular class) 에 대한 형식 매개 변수의 형식은
                   컴파일러 에서 인식하는 모든 형식이 될 수 있습니다.

                5. 만들 수 있는 생성된 형식 인스턴스의 수에는 제한이 없고(Any number of),
                   각 인스턴스에서는 다음과 같이 서로 다른 형식 매개 변수를 사용할 수 있습니다.
            */
            {
                GenericList<int> list1 = new GenericList<int>();
                GenericList<CustomStruct> list3 = new GenericList<CustomStruct>();
                GenericList<CustomClass> list2 = new GenericList<CustomClass>();
            }
            /*
                6. 형식 매개 변수 명명 지침
                   필수적. 단일 문자 이름으로도 자체 설명이 가능하여(completely self explanatory) 설명적인 이름(descriptive names)을 굳이 사용할 필요가 없는 경우가 아니면
                   제네릭 형식 매개 변수 이름을 설명적인 이름으로 지정 하십시오.

                    public interface ISessionChannel<TSession> { ... }
                    public delegate TOutput Converter<TInput, TOutput>(TInput from);
                    public class List<T> { ... }

                7. 선택적. 단일 문자 형식 매개변수를 사용하는 형식에는
                   형식 매개 변수 이름(the type parameter name) 으로 T 를 사용 하십시오.

                    public int IComparer<T>() { return 0; }
                    public delegate bool Predicate<T>(T item);
                    public struct Nullable<T> where T : struct { ... }

                8. 필수적. 설명적인(descriptive) 형식 매개 변수 이름 앞에 "T"를 붙이십시오.

                    public interface ISessionChannel<TSession>
                    {
                        TSession Session { get; }
                    }
                
                    public interface ISessionChannel<TSession>
                    {
                        TSession Session { get; }
                    }

                9. 선택적. 매개 변수 이름 안에는 형식 매개 변수에 적용되는 제약 조건(constraints) 을 나타내십시오.

                10. 예를 들어 ISession 으로 제한되는 매개 변수의 이름은 TSession 이 될 수 있습니다.
            */
        }

        static void dotnet_generic_class()
        {
            /*
                .NET Framework에는 상당히 많은 Generic 클래스들이 포함되어 있는데,
                특히 System.Collections.Generic 네임스페이스에 있는 모든 자료구조 관련 클래스들은 Generic 타입이다.
                흔히 사용하는 List<T>, Dictionary<T>, LinkedList<T> 등의 클래스들은 이 네임스페이스 안에 들어 있다.
                아래는 이들을 사용한 한 예이다.
            */
            {
                List<string> nameList = new List<string>();
                nameList.Add("홍길동");
                nameList.Add("이태백");

                Dictionary<string, int> dic = new Dictionary<string, int>();
                dic["길동"] = 100;
                dic["태백"] = 90;

                Console.ReadLine();
            }
        }


        class ObjectFactory
        {
            public static void Initialize<T>(T[] array) where T : new()
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new T();
                }
            }
        }

        //T는 Value 타입
        class MyClass1<T> 
            where T : struct
        {
        }

        //T는 Reference 타입
        class MyClass2<T> 
            where T : class
        {
        }

        //T는 디폴트 생성자를 가져야 함
        class MyClass3<T> 
            where T : new()
        {
            T data = default(T);

            public MyClass3()
            {
                data = new T();
            }
        }

        //base class
        class MyBase { }

        //T는 MyBase의 파생클래스이어야 함
        class MyClass4<T>
            where T : MyBase
        { }

        //T는 IComparable 인터페이스를 가져야 함
        class MyClass5<T>
            where T : System.IComparable
        { }

        //generic + interface
        interface IRun
        {
            void run();
        }

        interface IFly
        {
            void fly();
        }

        class Character : IFly, IRun, System.IComparable<Character>
        {
            int number = 0;

            public int Number
            {
                get { return number; }
                set { number = value; }
            }

            public virtual void on_update()
            {
                Console.WriteLine("call Character.on_update()");
            }

            void IRun.run()
            {
                Console.WriteLine("call IRun.run()");
            }

            void IFly.fly()
            {
                Console.WriteLine("call IFly.fly()");
            }

            //IComparable<T>.CompareTo(T param)
            public int CompareTo(Character other)
            {
                return this.number - other.number;
            }
        }

        class PC : Character
        {
            public override void on_update()
            {
                base.on_update();

                ((IRun)this).run();
                ((IFly)this).fly();
            }
        }

        //좀 더 복잡한 제약들
        class CharacterList<T>            
            where T : Character, new()
        {
            private class Node
            {
                private Node next;
                private T data;

                public Node(T t)
                {
                    next = null;
                    data = t;
                }

                public Node Next
                {
                    get { return next; }
                    set { next = value; }
                }

                public T Data
                {
                    get { return data; }
                    set { data = value; }
                }
            }

            private void AddHead(T data)
            {
                Node node = new Node(data);
                node.Next = head;
                head = node;
            }

            readonly int MAX = 0;

            private Node head;

            public CharacterList(int count)
            {
                MAX = count;

                for(int i = 0; i < count; i++)
                {
                    this.AddHead(new T());
                }                           
            }

            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= MAX)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        Node current = head;

                        for (int i = 1; i <= index; i++)
                        {
                            current = current.Next;
                        }

                        return current.Data;
                    }
                }
                set
                {
                    if (index < 0 || index >= MAX)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        Node current = head;

                        for (int i = 1; i <= index; i++)
                        {
                            current = current.Next;
                        }

                        current.Data = value;
                    }
                }
            }

            public System.Collections.Generic.IEnumerator<T> GetEnumerator()
            {
                Node current = head;

                while (current != null)
                {
                    yield return current.Data;
                    current = current.Next;
                }
            }

            public T FindFirstOccurrence(int number)
            {
                Node current = head;
                T t = null;

                while (current != null)
                {
                    //the constraint enables access to the Name property.
                    if (current.Data.Number == number)
                    {
                        t = current.Data;
                        break;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
                return t;
            }
        }

        //제네릭 형식은 다음과 같이 여러 형식 매개 변수 및 제약 조건을 사용할 수 있습니다.
        class SuperKeyType<K, V, U>
            where U : System.IComparable<U>
            where V : new()
        { }

        static void generic_type_constraint()
        {
            /*
                C# Generics를 선언할 때, 타입 파라미터가 Value Type인지 Reference 타입인지,
                또는 어떤 특정 베이스 클래스로부터 파생된 타입인지,
                어떤 인터페이스를 구현한 타입인지 등등을 지정할 수 있는데,
                이는 where T : 제약조건 과 같은 식으로 where 뒤에 제약 조건을 붙이면 가능하다.
                아래는 다양한 제약을 가한 예제들이다.


                    제약 조건                       설명

                    where T : struct                형식 인수가 값 형식이어야 합니다.
                                                    Nullable 를 제외한 임의의 값 형식을 지정할 수 있습니다.
                                                    자세한 내용은 Nullable 형식 사용(C# 프로그래밍 가이드) 를 참조하십시오.

                    where T : class                 형식 인수가 참조 형식이어야 합니다.
                                                    이는 모든 클래스, 인터페이스, 대리자 또는 배열 형식에도 적용 됩니다.

                    where T : new()                 형식 인수가 매개 변수 없는 공용 생성자를 가지고 있어야 합니다.
                                                    다른 제약 조건과 함께 사용하는 경우 new() 제약 조건은 마지막에 지정 해야 합니다.

                    where T : <기본 클래스 이름>    형식 인수가 지정된 기본 클래스이거나 지정된 기본 클래스에서 파생되어야 합니다.

                    where T : <인터페이스 이름>     형식 인수가 지정된 인터페이스이거나 지정된 인터페이스를 구현해야 합니다.
                                                    여러 인터페이스 제약 조건을 지정할 수 있습니다.
                                                    제한하는 인터페이스는 제네릭이 될 수도 있습니다.

                    where T : U                     T 에 대해 지정한 형식 인수가 U에 대해 지정한 인수 이거나
                                                    이 인수에서 파생되어야 합니다.

            */
            {
                var pcList = new PC[3];
                ObjectFactory.Initialize<PC>(pcList);
                foreach(var value in pcList)
                {
                    value.on_update();
                }
                /*
                output:
                    call Character.on_update()
                    call IRun.run()
                    call IFly.fly()
                    call Character.on_update()
                    call IRun.run()
                    call IFly.fly()
                    call Character.on_update()
                    call IRun.run()
                    call IFly.fly()
                */

                Console.ReadLine();

                CharacterList<PC> charList = new CharacterList<PC>(10);

                charList[0] = new PC();

                charList[0].on_update();
                /*
                output:
                    call Character.on_update()
                    call IRun.run()
                    call IFly.fly()
                */

                Console.ReadLine();
            }
        }


        class CharacterComparer : System.Collections.Generic.IComparer<Character>
        {
            //IComparer<T>.Compare(T a, T b)
            public int Compare(Character a, Character b)
            {
                if (a.Number > b.Number)
                {
                    return -1;
                }

                if (a.Number < b.Number)
                {
                    return 1;
                }

                return 0;
            }
        }

        //Type parameter T in angle brackets.
        class GenericObjectList<T> : System.Collections.Generic.IEnumerable<T>
        {
            protected Node head;
            protected Node current = null;

            //Nested class is also generic on T
            protected class Node
            {
                public Node next;
                private T data;  //T as private member datatype

                public Node(T t)  //T used in non-generic constructor
                {
                    next = null;
                    data = t;
                }

                public Node Next
                {
                    get { return next; }
                    set { next = value; }
                }

                public T Data  //T as return type of property
                {
                    get { return data; }
                    set { data = value; }
                }
            }

            public GenericObjectList()  //constructor
            {
                head = null;
            }

            public void AddHead(T t)  //T as method parameter type
            {
                Node n = new Node(t);
                n.Next = head;
                head = n;
            }

            //Implementation of the iterator
            public System.Collections.Generic.IEnumerator<T> GetEnumerator()
            {
                Node current = head;
                while (current != null)
                {
                    yield return current.Data;
                    current = current.Next;
                }
            }

            //IEnumerable<T> inherits from IEnumerable, therefore this class 
            //must implement both the generic and non-generic versions of 
            //GetEnumerator. In most cases, the non-generic method can 
            //simply call the generic method.
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class SortedObjectList<T> : GenericObjectList<T> where T : System.IComparable<T>
        {
            //A simple, unoptimized sort algorithm that 
            //orders list elements from lowest to highest:
            public void BubbleSort()
            {
                if (null == head || null == head.Next)
                {
                    return;
                }
                bool swapped;

                do
                {
                    Node previous = null;
                    Node current = head;
                    swapped = false;

                    while (current.next != null)
                    {
                        //Because we need to call this method, the SortedObjectList
                        //class is constrained on IEnumerable<T>
                        if (current.Data.CompareTo(current.next.Data) > 0)
                        {
                            Node tmp = current.next;
                            current.next = current.next.next;
                            tmp.next = current;

                            if (previous == null)
                            {
                                head = tmp;
                            }
                            else
                            {
                                previous.next = tmp;
                            }
                            previous = tmp;
                            swapped = true;
                        }
                        else
                        {
                            previous = current;
                            current = current.next;
                        }
                    }
                } while (swapped);
            }
        }

        //A simple class that implements IComparable<T> using itself as the 
        //type argument. This is a common design pattern in objects that 
        //are stored in generic lists.
        class Person : System.IComparable<Person>
        {
            string name;
            int age;

            public Person(string s, int i)
            {
                name = s;
                age = i;
            }

            //This will cause list elements to be sorted on age values.
            public int CompareTo(Person p)
            {
                return age - p.age;
            }

            public override string ToString()
            {
                return name + ":" + age;
            }

            //Must implement Equals.
            public bool Equals(Person p)
            {
                return (this.age == p.age);
            }
        }

        //단일 형식에 다음과 같이 여러 인터페이스를 제약 조건으로 지정할 수 있습니다.
        class Queue<T> where T : System.IComparable<T>, IEnumerable<T> { }

        //인터페이스에서는 다음과 같이 두 개 이상의 형식 매개 변수를 정의할 수 있습니다.
        interface IDictionary<K, V> { }

        //클래스에 적용되는 상속 규칙이 인터페이스에도 적용됩니다.
        interface IMonth<T> { }

        interface IJanuary : IMonth<int> { }        //No error
        interface IFebruary<T> : IMonth<int> { }    //No error
        interface IMarch<T> : IMonth<T> { }         //No error
        //interface IApril<T>  : IMonth<T, U> {}    //Error

        interface IBaseInterface<T> { }

        class SampleClass : IBaseInterface<string> { }

        interface IBaseInterface1<T> { }
        interface IBaseInterface2<T, U> { }

        class SampleClass1<T> : IBaseInterface1<T> { }          //No error
        class SampleClass2<T> : IBaseInterface2<T, string> { }  //No error


        static void generics_with_interface()
        {
            {
                Character char1 = new Character();
                char1.Number = 20;
                Character char2 = new Character();
                char2.Number = 30;
                Character char3 = new Character();
                char3.Number = 10;

                List<Character> charList = new List<Character>();

                charList.Add(char1);
                charList.Add(char2);
                charList.Add(char3);

                charList.Sort(new CharacterComparer());

                foreach (Character charObj in charList)
                {
                    Console.WriteLine("number {0}", charObj.Number);
                }
                /*
                output:
                    number 30
                    number 20
                    number 10
                */

                Console.ReadLine();
            }

            {
                //Declare and instantiate a new generic SortedList class.
                //Person is the type argument.
                SortedObjectList<Person> list = new SortedObjectList<Person>();

                //Create name and age values to initialize Person objects.
                string[] names = new string[]
                {
                    "Franscoise",
                    "Bill",
                    "Li",
                    "Sandra",
                    "Gunnar",
                    "Alok",
                    "Hiroyuki",
                    "Maria",
                    "Alessandro",
                    "Raul"
                };

                int[] ages = new int[] { 45, 19, 28, 23, 18, 9, 108, 72, 30, 35 };

                //Populate the list.
                for (int x = 0; x < 10; x++)
                {
                    list.AddHead(new Person(names[x], ages[x]));
                }

                //Print out unsorted list.
                foreach (Person p in list)
                {
                    System.Console.WriteLine(p.ToString());
                }
                System.Console.WriteLine("Done with unsorted list");

                //Sort the list.
                list.BubbleSort();

                //Print out sorted list.
                foreach (Person p in list)
                {
                    System.Console.WriteLine(p.ToString());
                }
                System.Console.WriteLine("Done with sorted list");

                Console.ReadLine();
            }
        }


        static void DisplayDefaultOf<T>()
        {
            var val = default(T);

            Console.WriteLine($"Default value of {typeof(T)} is {(val == null ? "null" : val.ToString())}.");
        }

        struct DataInfo
        {
            public string stringValue
            {
                set { stringValue = value; }
                get { return "A"; }
            }

            public DataObject dataObject { get; set; }
        }

        class DataObject
        {
            string stringValue;
            int intValue;
            float floatValue;
            double doubleValue;

            public DataObject() { }

            public int accessInt { get; set; }
            public DataInfo accessDataInfo { get; set; }
        }

        static void fill_up<T>(T[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = default(T);
            }
        }
        /*
        static T[] InitializeArray<T>(int length, T initialValue = default) //C# 7.1 에서 제공
        {
            if (length < 0)
            {
                return default;
            }

            var array = new T[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = initialValue;
            }

            return array;
        }
        */
        static void Display<T>(T[] values) => Console.WriteLine($"[ {string.Join(", ", values)} ]");

        // T is the type of data stored in a particular instance of GenericList.
        public class ObjectNodeList<T>
        {
            private class Node
            {
                // Each node has a reference to the next node in the list.
                public Node Next;
                // Each node holds a value of type T.
                public T Data;
            }

            // The list is initially empty.
            private Node head = null;

            // Add a node at the beginning of the list with t as its data value.
            public void AddNode(T t)
            {
                Node newNode = new Node();
                newNode.Next = head;
                newNode.Data = t;
                head = newNode;
            }

            // The following method returns the data value stored in the last node in
            // the list. If the list is empty, the default value for type T is
            // returned.
            public T GetLast()
            {
                // The value of temp is returned as the value of the method. 
                // The following declaration initializes temp to the appropriate 
                // default value for type T. The default value is returned if the 
                // list is empty.
                T temp = default(T);

                Node current = head;
                while (current != null)
                {
                    temp = current.Data;
                    current = current.Next;
                }
                return temp;
            }
        }

        static void generics_with_default()
        {
            /*
                default 는 C# 의 기본 키워드로 제약 조건이 필요 하지도 않고,
                다른 많은 곳에서도 사용할 수 있는 기능 입니다.
                default 를 각 타입에 사용할 경우 아래 표와 같은 값이 생성 됩니다.             

                    type	    default value

                    class	    null
                    struct	    Struct { fields = default(type) }
                    enum	    0 (가장 처음 나오는 0인 item)             
            */
            {
                Console.WriteLine(default(int));  //output: 0
                //Console.WriteLine(default(object) is null);  //output: True <- C# 7.0 에서 제공

                DisplayDefaultOf<int?>();
                DisplayDefaultOf<DataInfo>();
                DisplayDefaultOf<Character>();
                DisplayDefaultOf<System.Collections.Generic.List<int>>();

                /*
                output:
                    Default value of System.Nullable`1[System.Int32] is null.
                    Default value of AdvancedStep.Generic+DataInfo is AdvancedStep.Generic+DataInfo.
                    Default value of AdvancedStep.Generic+Character is (0, 0).
                    Default value of System.Collections.Generic.List`1[System.Int32] is null.
                */

                Console.ReadLine();
            }
            {
                int n = default(int);
                DataObject d = default(DataObject);
                DataInfo i = default(DataInfo);

                Console.WriteLine(n);

                if (d == null)
                {
                    Console.WriteLine("d is null");
                }

                Console.WriteLine("i.stringValue() = {0}", i.stringValue);

                if (i.dataObject == null)
                {
                    Console.WriteLine("i.dataObject() is null");
                }

                DataObject[] dataList = new DataObject[3];
                dataList[0] = new DataObject();

                fill_up<DataObject>(dataList);

                foreach (var data in dataList)
                {
                    if (null == data)
                    {
                        Console.WriteLine("data is null");
                    }
                }

                /*
                output:
                    0
                    d is null
                    i.stringValue() = A
                    i.dataObject() is null
                    data is null
                    data is null
                    data is null
                */

                Console.ReadLine();
            }
            {
                //Display(InitializeArray<int>(3));
                //Display(InitializeArray<bool>(4, default)); //C# 7.1 에서 제공

                //Character character = default; //C# 7.1 에서 제공
                //Display(InitializeArray(3, character));

                /*
                output:
                    [ 0, 0, 0 ]
                    [ False, False, False, False ]
                    [ , ,  ]
                */

                Console.ReadLine();
            }
            {
                // Test with a non-empty list of integers.
                ObjectNodeList<int> gll = new ObjectNodeList<int>();
                gll.AddNode(5);
                gll.AddNode(4);
                gll.AddNode(3);
                int intVal = gll.GetLast();
                // The following line displays 5.
                System.Console.WriteLine(intVal);

                // Test with an empty list of integers.
                ObjectNodeList<int> gll2 = new ObjectNodeList<int>();
                intVal = gll2.GetLast();
                // The following line displays 0.
                System.Console.WriteLine(intVal);

                // Test with a non-empty list of strings.
                ObjectNodeList<string> gll3 = new ObjectNodeList<string>();
                gll3.AddNode("five");
                gll3.AddNode("four");
                string sVal = gll3.GetLast();
                // The following line displays five.
                System.Console.WriteLine(sVal);

                // Test with an empty list of strings.
                ObjectNodeList<string> gll4 = new ObjectNodeList<string>();
                sVal = gll4.GetLast();
                // The following line displays a blank line.
                System.Console.WriteLine(sVal);

                /*
                output:
                */

                Console.ReadLine();
            }
        }


        //제네릭 메서드는 다음과 같이 형식 매개 변수를 사용하여 선언한 메서드입니다.
        static T Max<T>(T a, T b) 
            where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        static void Initialize<T>(T[] array) 
            where T : new()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }
        }

        class ObjectInfo
        {
            public int no = 0;
        }

        //포함하는 클래스(containing class)와 동일한 형식 매개 변수를 사용하는 제네릭 메서드를 정의 하면
        //컴파일러에서 CS0693 경고가 발생 합니다.
        //이는 메서드 범위 내에서 내부 T에 대해 제공한 인수가 외부 T에 대에 제공한 인수를 숨기기 때문 입니다.
        class GenericObject<T>
        {
            //CS0693 compile error
            //void SampleMethod<T>() { }
        }

        class GenericList2<T>
        {
            //No warning
            void SampleMethod<U>() { }
        }

        //SwapIfGreater<T>라는 이 버전의 Swap<T>은 IComparable<T>을 구현하는 형식 인수만 함께 사용할 수 있습니다.
        void SwapIfGreater<T>(ref T lhs, ref T rhs) where T : System.IComparable<T>
        {
            T temp;
            if (lhs.CompareTo(rhs) > 0)
            {
                temp = lhs;
                lhs = rhs;
                rhs = temp;
            }
        }

        //제네릭 메서드는 여러 형식 매개 변수에 대해 오버로드할 수 있습니다.
        void DoWork() { }
        void DoWork<T>() { }
        void DoWork<T, U>() { }

        static void generic_method()
        {
            int z = Max(5, 10);
            string last = Max("ant", "zoo");

            ObjectInfo[] list = new ObjectInfo[10];
            Initialize<ObjectInfo>(list);

            Console.ReadLine();
        }


        //제네릭 대리자를 참조하는 코드에서는 제네릭 클래스를 인스턴스화하거나
        //제네릭 메서드를 호출할 때와 마찬가지로 다음 예제와 같이 형식 매개 변수를 지정하여 폐쇄형 구성 형식을 만들 수 있습니다.
        public delegate void Del<T>(T item);
        public static void Notify(int i) { }

        //제네릭 클래스 내에 정의된 대리자에서는 클래스 메서드와 마찬가지로 제네릭 클래스 형식 매개 변수를 사용할 수 있습니다.
        class Deque<T>
        {
            T[] items;
            int index;

            //대리자를 참조하는 코드에서는 다음과 같이 포함하는 클래스의 형식 매개 변수를 지정해야 합니다.
            public delegate void DequeDelegate(T[] items);
        }
        private static void DoWork(float[] items) { }


        //sender 인수에 강력한 형식을 사용할 수 있고 Object 사이에서 캐스팅할 필요가 없으므로
        //제네릭 대리자는 특히 일반적인 디자인 패턴을 기반으로 이벤트를 정의할 때 유용합니다.
        delegate void EventHandler<T, U>(T sender, U eventArgs);

        class Event<T>
        {
            public class EventArgs : System.EventArgs { }
            public event EventHandler<Event<T>, EventArgs> eventHandler;

            protected virtual void OnEventChanged(EventArgs a)
            {
                eventHandler(this, a);
            }
        }

        class EventReceiver
        {
            public void handleEventChange<T>(Event<T> eventObj, Event<T>.EventArgs args) { }
        }


        static void generic_delegate()
        {
            {
                Del<int> m1 = new Del<int>(Notify);
                Del<int> m2 = Notify;
            }
            {
                Deque<float> s = new Deque<float>();
                Deque<float>.DequeDelegate d = DoWork;
            }
            {
                Event<double> s = new Event<double>();
                EventReceiver o = new EventReceiver();
                s.eventHandler += o.handleEventChange;
            }
        }


        public static void Test()
        {
            //generic_delegate();

            //generic_method();

            //generics_with_default();

            //generics_with_interface();

            //generic_type_constraint();

            //dotnet_generic_class();

            //generic_what();
        }
    }
}
