using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace AdvancedStep
{
    public class Reflection
    {

        static void reflection_what()
        {
            /*
                .NET Reflection

                .NET Reflection은 .NET 객체의 클래스 타입, 메서드, 프로퍼티 등의 메타 정보를 런타임 중에 알아내는 기능을 제공한다.
                또한, 이러한 메타 정보를 얻은 후, 직접 메서드를 호출하거나 프로퍼티를 변경하는 등의 작업도 가능하다.
                물론 객체에서 메서드를 직접 호출하는 경우가 더 빠르겠지만,
                어떤 경우는 런타임중에 이런 메타 정보가 동적으로 알아낼 필요가 있다.
                예를 들어, 테스트 어셈블리에 있는 테스트 클래스들의 Public 메서드를 선별해서 이를 동적으로 호출하는 경우라든가,
                특정 클래스 안에 지정된 이름의 멤버가 있는지 판단하는 경우 등등에 .NET Reflection이 활용될 수 있다.
            */
            {
                Console.ReadLine();
            }
        }


        class MyClass1
        {
            public string Name { get; set; }
        }

        static void SetDefaultName(object myObject)
        {
            // Name이라는 속성이 있는지?
            PropertyInfo pi = myObject.GetType().GetProperty("Name");

            // 있으면 속성값 설정
            if (pi != null)
            {
                pi.SetValue(myObject, "Lee", null);
            }
        }

        static void reflection_with_property()
        {
            /*
                .NET Reflection을 이용해서 많이 사용되는 케이스의 하나로 프로퍼티를 접근하는 경우를 들어 보자.
                클래스의 프로퍼티 정보를 얻기 위해서 우선 .NET 클래스를 GetType() 등의 메서드를 써서 알아낸 후,
                클래의 특정 속성정보를 알기 위해 Type.GetProperty()라는 메서드를 호출한다.
                클래스가 가진 모든 프러퍼티를 가져오기 위해서는 GetProperties()라는 메서트를 호출하면 된다.
                아래의 예제는 해당 객체가 Name이라는 속성을 가지고 있는지 체크해서,
                만약 있으면, 특정 값을 그 속성에 설정하는 예이다. 
            */
            {
                MyClass1 m1 = new MyClass1();

                SetDefaultName(m1);

                Console.WriteLine(m1.Name);

                Console.ReadLine();
            }
        }


        class MyClass2
        {
            public void MyMethod()
            {
                Console.WriteLine("MyClass2.MyMethod");
            }
        }

        class MyClass3
        {
            public void MyMethod()
            {
                Console.WriteLine("MyClass3.MyMethod");
            }
        }

        class MyClass4
        {
            public void DifferentMethod()
            {
                Console.WriteLine("MyClass4.DifferentMethod");
            }
        }

        static void MyFunc(object myObject)
        {
            // 해당 객체가 MyMethod라는 메서드를 가지고 있는지
            MethodInfo mi = myObject.GetType().GetMethod("MyMethod");
            if (mi != null)
            {
                // 만약 메서드가 있으면, 호출
                mi.Invoke(myObject, null);
            }
            else
            {
                Console.WriteLine(myObject.GetType().Name +
                 ": MyMethod not found");
            }
        }

        static void reflection_with_method_invoke()
        {
            /*
                .NET Reflection을 이용해서 메서드 프로토타입 즉, 함수 인자, 리턴값 등의 메타 정보를 읽어 오는 것이 가능하며,
                이를 통해 직접 그 메서드를 호출하는 것이 가능하다.
                특정 메서드 정보를 얻기 위해서 Type.GetMethod()를 호출하고,
                모든 메서드 정보를 가져오기 위해서는 GetMethods()를 호출한다.
                아래의 예제는 해당 객체가 MyMethod이라는 메서드를 가지고 있는지 체크해서,
                만약 있으면 해당 메서드를 호출하는 예이다.
            */
            {
                MyClass2 m1 = new MyClass2();
                MyClass3 m2 = new MyClass3();
                MyClass4 m3 = new MyClass4();

                MyFunc(m1);
                MyFunc(m2);
                MyFunc(m3);

                Console.ReadLine();
            }
        }


        class MyClass5
        {
            public MyClass5()
            {
                this.Name = "No name";
            }

            public int Id { get; set; }
            public string Name { get; set; }
        }

        static void reflection_with_create_class()
        {
            /*
                클래스 타입을 알 때 .NET Reflection을 이용해서 해당 클래스의 객체를 생성할 수 있다.
                이는 컴파일시에 직접 new를 사용하는 방식이 아니라,
                클래스 타입명을 문자열로 받아들여 런타임시에 해당 클래스의 객체를 생성할 수 있는 것을 뜻한다.
                또한, 어떤 객체를 받아들여 해당 타입을 얻은 후에 (ex: obj.GetType()),
                이 타입의 또 다른 객체를 생성하는 것도 가능하다.
                아래의 예제는 해당 Customer라는 클래스명을 사용하여 런타임시에 해당 객체를 생성하는 예이다.
                Type.GetType()을 이용하여 해당 타입을 알아내고 (주: 클래스명 앞에 네임스페이스를 붙여야 한다),
                이어 Activator의 CreateInstance()를 사용하여 실제 클래스 객체를 생성한다. 
            */
            {
                // 네임스페이스와 클래스명 함께 
                Type customerType = Type.GetType("Reflection.MyClass5");

                // Type으로부터 클래스 객체 생성
                object obj = Activator.CreateInstance(customerType);

                // 생성된 객체 사용예
                string name = ((MyClass5)obj).Name;
                Console.WriteLine(name); // No name

                Console.ReadLine();
            }
        }


        class MyFilter<T> where T : struct
        {
            private List<T> _elements;

            public MyFilter()
            {
                _elements = new List<T>();
            }

            public MyFilter(List<T> elements)
            {
                _elements = elements;
            }
        }

        static void RunByFilter(object filter)
        {
            // Type명 비교
            if (filter.GetType().Name == typeof(MyFilter<>).Name)
            {
                // Generic의 T 파라미터 타입 가져오기 : int
                Type genArgType = filter.GetType().GetGenericArguments()[0];

                // MyFilter<>에 int를 적용하여 실제 타입 확정
                Type actualType = typeof(MyFilter<>).MakeGenericType(genArgType);

                // 실제 타입으로부터 객체 생성
                object obj = Activator.CreateInstance(actualType, true);

                Console.WriteLine(obj.GetType().Name); //MyFilter`1
            }
        }

        static void reflection_with_create_generic_type()
        {
            /*
                클래스 타입이 확정되지 않은 Generic Type인 경우에는 (ex: MyFilter<T>) 파라미터 T가 정해지지 않으면
                Activator로 객체를 생성할 수 없다.
                그것은 Generic Type 자체로는 클래스가 되지 못하기 때문인데,
                이 경우 먼저 Generic의 T 파라미터 타입을 GetGenericArguments()를 써서 알아낸 후,
                MakeGenericType(T)을 사용하여 구체적인 클래스를 만들어야 한다.
                아래 예제는 MyFilter라는 Generic 타입이 object로 Run()함수에 전달되었을 때,
                T 파라미터를 알아내고 Generic으로부터 객체를 생성하는 예를 보여주고 있다.
            */
            {
                MyFilter<int> filter = new MyFilter<int>();
                RunByFilter(filter);

                Console.ReadLine();
            }
        }

		static Type findAssemblyName(string assemblyQualifiedName)
		{
			// This will return null
			// Just here to test that the simple GetType overload can't return the actual type
			var t0 = Type.GetType(assemblyQualifiedName);

			// Throws exception is type was not found
			return Type.GetType( assemblyQualifiedName,
				                 (name) => {
					                // Returns the assembly of the type by enumerating loaded assemblies
					                // in the app domain            
					                return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
				                 },
				                 null,
				                 true );
		}

		public static List<Type> findTypeByNames(string className, string fullName, bool ignoreCase = true)
		{
			var found_types = new List<Type>();
			if (true == ignoreCase)
			{
				className = className.ToLower();
				fullName = fullName.ToLower();
			}

			foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] assemblyTypes = a.GetTypes();
				for (int i = 0; i < assemblyTypes.Length; i++)
				{
					if (ignoreCase)
					{
						if (    assemblyTypes[i].Name.Equals(className, StringComparison.OrdinalIgnoreCase)
							 && assemblyTypes[i].FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase) )
						{
							found_types.Add(assemblyTypes[i]);
						}
					}
					else
					{
						if (   true == assemblyTypes[i].Name.Equals(className)
							&& true == assemblyTypes[i].FullName.Equals(fullName) )
						{
							found_types.Add(assemblyTypes[i]);
						}
					}
				}
			}

			return found_types;
		}

        static void reflection_with_find_assembly_time()
        {
            findTypeByNames("", "");

		}


		public static void Test()
        {
            //reflection_with_find_assembly_time();

            //reflection_with_create_generic_type();

            //reflection_with_create_class();

            //reflection_with_method_invoke();

            //reflection_with_property();

            //reflection_what();
        }
    }
}
