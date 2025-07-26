using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class DataType
    {
        static void data_type_what()
        {
            /*
                C# DataType	    .NET DataType       Description
                bool	        System.Boolean	    True or False
                byte	        System.Byte	        8 bit unsigned integer
                sbyte	        System.SByte	    8 bit signed integer
                short	        System.Int16	    16 bit signed integer
                int	            System.Int32	    32 bit signed integer
                long	        System.Int64	    64 bit signed integer
                ushort	        System.UInt16	    16 bit unsigned integer
                uint	        System.UInt32	    32 bit unsigned integer
                ulong	        System.UInt64	    64 bit unsigned integer
                float	        System.Single	    32 bit single precision, Floating point number
                double	        System.Double	    64 bit double precision, Floating point number
                decimal	        System.Decimal	    128 bit Decimal
                char	        System.Char	        16 bit Unicode Character
                string	        System.String	    Unicode String
                                System.DateTime	    Date and Time, No separate C# keywords
                object	        System.Object	    Any type of base class that can contain any type

                The C# compiler changes the data type converted to C# keywords to .NET.
            */
            {
                Console.ReadLine();
            }
        }


        static void data_type_use()
        {
            /*
                // Bool
                bool b = true;

                // Numeric
                short sh = -32768;
                int i = 2147483647;
                long l = 1234L;      // L suffix
                float f = 123.45F;   // F suffix
                double d1 = 123.45;
                double d2 = 123.45D; // D suffix
                decimal d = 123.45M; // M suffix

                // Char/String
                char c = 'A';
                string s = "Hello";

                // DateTime  2011-10-30 12:35
                DateTime dt = new DateTime(2011, 10, 30, 12, 35, 0);

                * float 데이타 타입은 숫자 뒤에 123.45F와 같이 F를 붙여 double이 아닌 float 타입임을 나타낸다.
                * double 데이타 타입은 숫자 뒤에 123.45D과 같이 D를 붙이거나
                  혹은 아무것도 붙이지 않음으로 해서 double 타입임을 나타낸다.
                * decimal 데이타 타입은 숫자 뒤에 123.45M과 같이 M를 붙여 decimal 타입임을 나타낸다.
                * char 데이타 타입은 작은따옴표 ' (single quotation)을 사용하여 한 문자를 할당한다.
                * string 데이타 타입은 큰따옴표 " (double quotation)을 사용하여 문자열을 할당한다.
            */
            {
                Console.ReadLine();
            }
        }


        static void literal_use()
        {
            /*
                123    // int 리터럴
                12.3   // double 리터럴
                "A"    // string 리터럴
                'a'    // char 리터럴
                true   // bool 리터럴

                C# Literal DataType     Suffix (Can be case sensitive)  Example
                long	                L                               1024L
                uint	                U	                            1024U
                ulong	                UL	                            1024UL
                float	                F	                            10.24F
                double	                D	                            10.24D or 10.24
                decimal	                M	                            10.24M
            */
            {
                Console.ReadLine();
            }
        }


        static void max_min_use()
        {
            //max, min
            {
                int i = int.MaxValue;
                Console.Write("{0} ", i);

                float f = float.MinValue;
                Console.Write("{0} ", f);

                Console.ReadLine();
            }
        }


        static void null_use()
        {
            /*
                어떤 변수가 메모리 상에 어떤 데이타도 가지고 있지 않다는 의미로서 NULL을 사용하는데,
                NULL을 표현하기 위하여 C# 에서는 소문자 null 이라는 키워드를 사용한다.

                모든 데이타 타입이 NULL을 가질 수 있는 것은 아니며,
                사실 데이타 타입은 NULL을 가질 수 있는 타입 (Reference 타입)
                과 가질 수 없는 타입 (Value 타입)으로 구분될 수 있다.

                아래는 NULL을 가질 수 있는 문자열(string) 타입의 변수 s 에 null 을 할당하는 예이다.  
            */
            {
                string s;
                s = null;

                Console.ReadLine();
            }
        }


        static void nullable_use()
        {
            /*
                정수(int)나 날짜(DateTime)와 같은 Value Type은 일반적으로 NULL을 가질 수 없다.
                C# 2.0에서부터 이러한 타입들에 NULL을 가질 수 있게 하였는데, 이를 Nullable Type 이라 부른다.

                C#에서 물음표(?)를 int나 DateTime 타입명 뒤에 붙이면 즉, int? 혹은 DateTime? 같이 하면 Nullable Type이 된다.
                이는 컴파일하면 .NET의 Nullable<T> 타입으로 변환된다.
                Nullable Type (예: int?) 을 일반 Value Type (예: int)으로 변경하기 위해서는 Nullable의 .Value 속성을 사용한다.
            */
            {
                // Nullable 타입
                int? i = null;
                i = 101;

                bool? b = null;

                //int? 를 int로 할당
                Nullable<int> j = null;
                j = 10;
                int k = j.Value;

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //nullable_use();

            //null_use();

            //max_min_use();

            //literal_use();

            //data_type_use();

            //data_type_what();
        }

    }
}
