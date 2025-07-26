using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Enumerator
    {
        enum City
        {
            Seoul,   // 0
            Daejun,  // 1
            Busan = 5,  // 5
            Jeju = 10   // 10
        }

        enum Border
        {
            None = 0,
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8
        }

        static void enum_what()
        {
            //enum
            {
                /*
                    C#의 키워드 enum은 열거형 상수(constant)를 표현하기 위한 것으로
                    이를 이용하면 상수 숫자들을 보다 의미있는 단어들로 표현할 수 있어서 프로그램을 읽기 쉽게 해준다.

                    enum의 각 요소는 별도의 지정없이는 첫번째 요소가 0, 두번째가 1 등으로 1씩 증가된 값들을 할당받는다.
                    물론, 개발자가 임의로 의미있는 번호를 지정해 줄 수도 있다.
                    enum문은 클래스 안이나 네임스페이스내에서만 선언될 수 있다.
                    즉, 메서드 안이나 프로퍼티 안에서는 선언되지 않는다.

                    아래 예제는 Category 라는 enum 타입을 정의한 예인데, Cake는 숫자 0을 갖고, IceCream은 1, Bread는 2라는 값을 갖는다.
                    프로그램상에서 Category 값을 0, 1 처럼 직접 쓰는 대신 Category.Cake, Category.IceCream 와 같이 사용하면
                    그 의미를 파악하기가 훨씬 쉬워진다.

                        public enum Category
                        {
                            Cake,
                            IceCream,
                            Bread
                        }
                */
                {
                    Console.ReadLine();
                }
            }

            //enum 의 사용
            {
                /*
                    enum 타입은 숫자형 타입과 호환가능하다.
                    만약 enum 타입의 변수를 int로 캐스팅(주: Casting - 한 타입을 다른 타입으로 변경하는 것.
                    타입 변환이 실패할 수도 있다)하면 해당 enum값의 숫자 값을 얻게 된다.
                    또한, enum 타입의 변수는 enum 리터럴값과 서로 비교할 수 있다.
                    아래 코드는 enum 변수 myCity가 리터럴 City.Seoul 과 같은지 체크하는 예제이다. 
                */
                {
                    City myCity;

                    // enum 타입에 값을 대입하는 방법
                    myCity = City.Seoul;

                    // enum을 int로 변환(Casting)하는 방법. 
                    // (int)를 앞에 지정.
                    int cityValue = (int)myCity;

                    if (myCity == City.Seoul) // enum 값을 비교하는 방법
                    {
                        Console.WriteLine("Welcome to Seoul");
                    }

                    Console.ReadLine();
                }
            }
        }


        public enum NoFlagsEnum
        {
            Begin = 0
            , Left = 1, Right = 2, Top = 4, Bottom = 8
            , Max = Bottom
        }

        [Flags]
        public enum FlagsEnum
        {
            Begin = 0
            , Left = 1, Right = 2, Top = 4, Bottom = 8
            , Max = Bottom
        }

        static void flags_enum()
        {
            /*
                enum의 각 멤버들은 각 비트별로 구분되는 값들(예: 1,2,4,8,...)을 갖을 수 있는데,
                이렇게 enum 타입이 비트 필드를 갖는다는 것을 표시하기 위해 enum 선언문 바로 위에 [Flags] 라는 Attribute
                (주: Type 혹은 그 멤버를 선언할 때 그 위에 붙이는 특별한 특성값으로
                해당 타입 혹은 멤버가 어떤 특성을 갖고 있는지 나타내게 된다) 를 지정할 수 있다.

                [Flags] 특성을 갖는 플래그 enum은 OR 연산자를 이용해서 한 enum 변수에 다중값(예: 1+4)을 가질 수 있으며,
                AND 연산자를 이용하여 enum 변수가 특정 멤버를 포함하고 있는지 체크할 수 있다.

                아래 예제는 Border라는 플래그 enum으로 OR 연산을 통해 다중값을 표현하고,
                AND 연산을 통해 특정멤버를 체크해보는 코드를 보여주고 있다.

                또한, 플래그 enum에 대해 .ToString() 메서드를 사용하면 해당 플래그 멤버명들을 문자열로 보여준다는 것이다.
                [Flags]가 없으면 1+4 즉 5를 출력한다.
            */
            {
                // OR 연산자로 다중 플래그 할당
                Border b = Border.Top | Border.Bottom;

                // & 연산자로 플래그 체크
                if ((b & Border.Top) != 0)
                {
                    //HasFlag()이용 플래그 체크
                    if (b.HasFlag(Border.Bottom))
                    {
                        Console.WriteLine(b.ToString());
                        /*
                        output:
                            Top, Bottom
                        */
                    }
                }

                Border v = Border.Bottom;
                if (true == Enum.IsDefined(typeof(Border), v))
                {
                    Console.WriteLine(v.ToString());
                    /*
                    output:
                        Bottom
                    */

                    Console.ReadLine();
                }

                // flags enum 과 no flags enum 비교
                {
                    for (int i = 0; i <= (int)NoFlagsEnum.Max; ++i)
                    {
                        Console.WriteLine("NoFlagsEnum : type({0}), value({1})"
                                         , ((NoFlagsEnum)i).ToString(), i);
                        /*
                        output:
                            NoFlagsEnum : type(Begin), value(0)
                            NoFlagsEnum : type(Left), value(1)
                            NoFlagsEnum : type(Right), value(2)
                            NoFlagsEnum : type(3), value(3)
                            NoFlagsEnum : type(Top), value(4)
                            NoFlagsEnum : type(5), value(5)
                            NoFlagsEnum : type(6), value(6)
                            NoFlagsEnum : type(7), value(7)
                            NoFlagsEnum : type(Bottom), value(8)
                        */
                    }

                    for (int i = 0; i <= (int)FlagsEnum.Max; ++i)
                    {
                        Console.WriteLine("FlagsEnum : type({0}), value({1})"
                                         , ((FlagsEnum)i).ToString(), i);
                        /*
                        output:
                            FlagsEnum : type(Begin), value(0)
                            FlagsEnum : type(Left), value(1)
                            FlagsEnum : type(Right), value(2)
                            FlagsEnum : type(Left, Right), value(3)
                            FlagsEnum : type(Top), value(4)
                            FlagsEnum : type(Left, Top), value(5)
                            FlagsEnum : type(Right, Top), value(6)
                            FlagsEnum : type(Left, Right, Top), value(7)
                            FlagsEnum : type(Max), value(8)
                        */
                    }

                    NoFlagsEnum v1 = NoFlagsEnum.Bottom;
                    if (true == Enum.IsDefined(typeof(NoFlagsEnum), v1))
                    {
                        Console.WriteLine(v1.ToString());
                        /*
                        output:
                            Bottom
                        */

                        Console.ReadLine();
                    }

                    FlagsEnum v2 = FlagsEnum.Bottom;
                    if (true == Enum.IsDefined(typeof(FlagsEnum), v2))
                    {
                        Console.WriteLine(v2.ToString());
                        /*
                        output:
                            Max
                        */

                        Console.ReadLine();
                    }

                    Console.ReadLine();
                }
            }
        }


        [FlagsAttribute]
        public enum PermissionLevelType
        {
            has_not_permission = 0

            , has_attackable = 1
            , has_defencable = 2

            , has_both = has_attackable | has_defencable
        }

        static void flags_attribute()
        {
            PermissionLevelType perLevelType = PermissionLevelType.has_attackable;

            if ((perLevelType & PermissionLevelType.has_both) == PermissionLevelType.has_attackable)
            {
                Console.WriteLine(perLevelType.ToString());
                /*
                output:
                    has_attackable
                */
            }

            Console.ReadLine();
        }





        public static void Test()
        {
            //flags_attribute();

            //flags_enum();

            //enum_what();
        }
    }
}
