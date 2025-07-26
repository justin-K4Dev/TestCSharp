using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Structure
    {
        static void value_type_and_reference_type()
        {
            /*
                C# 은 Value Type과 Reference Type을 지원한다.
                C# 에서는 struct를 사용하면 Value Type을 만들고, class 를 사용하면 Reference Type을 만든다.

                C# .NET의 기본 데이타형들은 struct로 정의되어 있다.
                즉, int, double, float, bool 등의 기본 데이타 타입은 모두 struct로 정의된 Value Type이다.
                Value Type은 상속될 수 없으며, 주로 상대적으로 간략한 데이타 값을 저장하는데 사용된다.

                Reference Type은 class를 정의하여 만들며 상속이 가능하고,
                좀 더 복잡한 데이타와 행위들을 정의하는 곳에 많이 사용된다.
                Value Type의 파라미터 전달은 데이타를 복사(copy)하여 전달하는 반면,
                Reference Type은 Heap 상의 객체에 대한 레퍼런스(reference)를 전달하여 이루어진다.
                구현에 있어 어떤 Type을 선택하는가는 해당 Type의 특성을 고려해서 결정해야 하는 문제이다. 


                    // System.Int32 (Value Type)
                    public struct Int32 { ... }

                    // System.String (Reference Type)
                    public sealed class String { ... }
            */
            {
                Console.ReadLine();
            }
        }


        // 구조체 정의
        struct MyPoint
        {
            public int X;
            public int Y;

            public MyPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public override string ToString()
            {
                return string.Format("({0}, {1})", X, Y);
            }
        }

        static void struct_type()
        {
            /*
                C# struct는 구조체를 생성하고 Value Type을 정의하기 위해 사용된다.
                많은 경우 C#에서 클래스를 사용하지만,
                경우에 따라 클래스보다 상대적으로 가벼운 오버헤드를 지닌 구조체가 필요할 수 있다.
                C# 의 구조체는 클래스와 같이 메서드, 프로퍼티 등 거의 비슷한 구조를 가지고 있지만, 상속은 할 수 없다.
                하지만 C# 구조체가 상속(inheritance)은 할 수는 없어도,
                클래스와 마찬가지로 인터페이스(interface)를 구현할 수는 있다. (참조: struct 사용시 주의)
            */
            {
                // 구조체 사용
                MyPoint pt = new MyPoint(10, 12);

                Console.WriteLine(pt.ToString());

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //struct_type();

            //value_type_and_reference_type();
        }
    }
}
