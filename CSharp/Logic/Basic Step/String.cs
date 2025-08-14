using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;


namespace BasicStep
{
    public class String
    {
        static void string_what()
        {
            /*
                문자열(string)은 프로그램에서 가장 많이 쓰이는 데이타 타입 중의 하나이다.
                C#에서 문자열(string)은 이중부호를 사용하여 표현되며, 단일 문자(char)는 단일부호를 사용하여 표현된다.

                C#의 키워드 string은 .NET의 System.String 클래스와 동일하며,
                따라서 System.String 클래스의 모든 메서드와 속성(Property)을 사용할 수 있다.
                예를 들어 일정 문자열 부분만 뽑아내는 Substring() 메서드, 문자열 길이를 구하는 Length 속성 등을 모두 사용할 수 있다.

                C# 문자열은 Immutable 즉 한번 문자열이 설정되면, 다시 변경할 수 없다.
                (주: 한번 그 값이 설정되면 다시 변경할 수 없는 타입을 Immutable Type이라 부르고,
                반대로 값을 계속 변경할 수 있는 것을 Mutable Type이라 부른다)
                예를 들어, 문자열 변수 s 가 있을 때, s = "C#"; 이라고 한 후 다시 s = "F#"; 이라고 실행하면,
                .NET 시스템은 새로운 string 객체를 생성하여 "F#"이라는 데이타로 초기화 한 후 이를 변수명 s 에 할당한다.
                즉, 변수 s 는 내부적으로는 전혀 다른 메모리를 갖는 객체를 가리키는 것이다.
            */
            {
                // 문자열(string) 변수
                string s1 = "C#";
                string s2 = "Programming";

                // 문자(char) 변수 
                char c1 = 'A';
                char c2 = 'B';

                // 문자열 결합
                string s3 = s1 + " " + s2;
                Console.WriteLine("String: {0}", s3);

                // 부분문자열 발췌
                string s3substring = s3.Substring(1, 5);
                Console.WriteLine("Substring: {0}", s3substring);

                Console.ReadLine();
            }
        }

        static void string_InterpolatedString()
        {
            /*
                문자열 내에 변수나 표현식을 직접 삽입할 수 있게 해주는 문법으로,
                $"..." 형식으로 작성하며 C# 6.0에서 도입되었습니다.

                🔧 사용 문법
                    - $"문자열 {표현식}"
                    - $ 접두사를 붙이면 문자열 안에 중괄호 {}로 감싼 C# 표현식을 평가해서 삽입할 수 있음
            */
            {
                var name = "Alice";
                Console.WriteLine($"Hello, {name}!"); // 출력: Hello, Alice!

                Console.ReadLine();
            }
            {
                // 🔸 숫자 포맷팅
                double price = 12.345;
                Console.WriteLine($"Price: {price:F2}"); // → Price: 12.35

                //🔸 날짜 포맷팅
                var date = DateTime.Now;
                Console.WriteLine($"Today is {date:yyyy-MM-dd}"); // → Today is 2025-06-12

                //🔸 계산식 삽입
                int a = 5, b = 10;
                Console.WriteLine($"{a} + {b} = {a + b}"); // → 5 + 10 = 15
            }
        }

        static void string_character_array()
        {
            /*
                문자열(string)은 문자(character)의 집합체이다.
                문자열 안에 있는 각 문자를 엑세스하고 싶으면, [인덱스] (square bracket)을 사용하여 문자 요소를 엑세스한다.

                예를 들어, 문자열 변수 s가 "Hello" 값을 가지고 있을 때,
                s[0]이라고 하면 첫번째 문자 H를, s[1]이라 하면 두번째 문자 e 를 리턴한다.
                이는 문자열을 문자배열처럼 취급하는 것으로 일반 C# 배열과 마찬가지로 첫번째 요소는 [0]으로 엑세스한다.

                문자배열(char array)을 문자열(string)으로 변환하기 위해서는 아래와 같이 new string(문자배열)을 사용한다.

                하나의 문자는 상응하는 ASCII 코드 값을 가지는데,
                예를 들어 대문자 A는 65, B는 66, Z는 90을 갖는다.
                소문자는 a가 97, b가 98, ... 등을 갖는다.
                하나의 문자는 이처럼 숫자값으로 표현되므로 문자에 숫자를 더하거나 빼면 다른 문자로 표현될 수 있다.
                예를 들어 문자A 를 갖는 변수에 1을 더하면 66이 되어 문자 B가 된다.    
            */
            {
                string s1 = "C# Studies";

                // 문자열을 배열인덱스로 한문자 엑세스 
                for (int i = 0; i < s1.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, s1[i]);
                }

                // 문자열을 문자배열로 변환
                string str = "Hello";
                char[] charArray1 = str.ToCharArray();

                for (int i = 0; i < charArray1.Length; i++)
                {
                    Console.WriteLine(charArray1[i]);
                }

                // 문자배열을 문자열로 변환
                char[] charArray2 = { 'A', 'B', 'C', 'D' };
                string s2 = new string(charArray2);

                Console.WriteLine(s2);

                // 문자 연산
                char c1 = 'A';
                char c2 = (char)(c1 + 3);
                Console.WriteLine(c2);  // D 출력

                Console.ReadLine();
            }
        }


        static void StringBuilder_what()
        {
            /*
                문자열을 다루는데 중요한 클래스 중의 하나는 System.Text.StringBuilder 클래스이다.
                String 클래스는 위에서 설명한 대로 Immutable이기 때문에, 문자열 갱신을 많이 하는 프로그램에는 적당하지 않다.
                반면 Mutable 타입인 StringBuilder 클래스는 문자열 갱신이 많은 곳에서 자주 사용되는데
                이는 이 클래스가 별도 메모리를 생성,소멸하지 않고 일정한 버퍼를 갖고 문자열 갱신을 효율적으로 처리하기 때문이다.

                특히 루프 안에서 계속 문자열을 추가 변경하는 코드에서는 string 대신 StringBuilder를 사용해야 한다.
            */
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= 26; i++)
                {
                    sb.Append(i.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                string s = sb.ToString();

                Console.WriteLine(s);

                Console.ReadLine();
            }
            {
                var sb = new StringBuilder();
                sb.Append("Hello");
                sb.Append(", ");
                sb.Append("World!");
                string result = sb.ToString();

                Console.WriteLine(result); // 출력: Hello, World!
            }
        }


        static void string_2_byte_array()
        {
            /*
                string 문자열을 C#의 Char[] 배열로 변경하는 것은 String 클래스의 ToCharArray()라는 메서드를 사용하면 간단하다.
                그렇다면, string은 byte[] 배열로 변경하는 것은 가능한가?
                만약 가능했다면, string 클래스 안에 ToByteArray() 같은 메서드가 존재할 듯 한데, 이런 메서드는 존재하지 않는다.
                왜냐하면, String은 직접 byte[] 변경할 수 없기 때문이다.

                먼저 반대의 경우를 생각해 보자.
                byte[]를 직접 string으로 변경할 수 있는가?
                이를 위해 우선 byte[] 가 어떤 Charset을 가지고 인코딩(Encoding) 되었는지 알아야 할 것이다.
                이는 ASCII, Unicode, UTF8, GB18030 등 다양한 인코딩 방식에 따라 바이트들이 의미하는 문자가 완전히 다르기 때문이다.

                따라서 byte배열을 .NET의 유니코드 string으로 변경하기 위해서는
                해당 바이트가 어떤 인코딩인지 알고 이를 유니코드 String으로 변경하게 된다.
                동일한 로직으로 문자열을 Byte배열로 변경할 때도 인코딩 방식에 따라 다른 바이트값들을 갖게 된다.

                문자열을 Byte[] 배열로 변경하기 위해서는 System.Text.Encoding의 인코딩 방식을 지정한 후 GetBytes() 메소드를 호출하면 된다.
                예를 들어, 유니코드 인코딩을 사용하여 Byte[]로 변환하는 경우, System.Text.Unicode.GetBytes() 메서드를 호출하고,
                UTF8 인코딩을 사용하는 경우, System.Text.UTF8.GetBytes() 메서드를 호출하면 된다.

                 Byte[] 배열을 String으로 변환하기 위해서는 바이트로 인코딩했던 동일한 인코더를 사용하여야 한다.
                 즉, 유니코드 인코더를 사용하여 String은 Byte[]로 변환했었다면
                 Encoding.Unicode.GetString()을 사용하여 Byte 배열을 문자열로 변경한다. 
            */
            {
                // covert String -> memory !!!

                System.String a = "우리12abc헐";

                // String to char[] - char is Unicode
                char[] b = a.ToCharArray();

                // String to byte[]
                byte[] c = System.Text.Encoding.Default.GetBytes(a); // String to default byte
                byte[] d = System.Text.Encoding.Unicode.GetBytes(a); // String to unicode byte
                byte[] e = System.Text.Encoding.UTF8.GetBytes(a);  // String to UTF8 byte
                byte[] f = System.Text.Encoding.ASCII.GetBytes(a); // 하위 7bit 만 변환됨.

                Console.WriteLine("Origin String == {0}", a);

                Console.WriteLine("Length");
                Console.WriteLine("String Length == {0}", a.Length); // 한글, 영어 모두 1 글자 길이 = 1
                Console.WriteLine("char[] - Length after conversion to Unicode == {0}", b.Length); // 한글, 영어 모두 1 글자 길이 = 1

                Console.WriteLine("byte[] - Length after conversion to default == {0}", c.Length); // 한글 1자 길이 = 2 bytes, 영어 1자 길이 = 1byte
                Console.WriteLine("byte[] - Length after conversion to Unicode == {0}", d.Length); // 한글, 영어 모두 1 글자 길이 = 2 
                Console.WriteLine("byte[] - Length after conversion to UTF8    == {0}", e.Length); // 한글, 영어 모두 1 글자 길이 = 가변?

                Console.WriteLine("byte[] - Length after conversion to ASCII   == {0}", f.Length); // 한글, 영어 모두 1 글자 길이 = 1

                Console.WriteLine("Incoding Name");
                Console.WriteLine("Encoding.Default.EncodingName == {0}", System.Text.Encoding.Default.EncodingName);
                Console.WriteLine("Encoding.Unicode.EncodingName == {0}", System.Text.Encoding.Unicode.EncodingName);
                Console.WriteLine("Encoding.UTF8.EncodingName    == {0}", System.Text.Encoding.UTF8.EncodingName);
                Console.WriteLine("Encoding.ASCII.EncodingName   == {0}", System.Text.Encoding.ASCII.EncodingName);


                // covert memory -> String !!!

                Console.WriteLine(b);  // output char[].

                System.String aa = new System.String(b);  // char[] to String
                Console.WriteLine("char[] -> String == {0}", aa);

                Console.WriteLine(c); // Not printing properly. WriteLine outputs "string".

                Console.WriteLine("Output after convert byte[] to String");
                // byte[] to String
                Console.WriteLine("Default byte[] -> String == {0}", System.Text.Encoding.Default.GetString(c));
                Console.WriteLine("Unicode byte[] -> String == {0}", System.Text.Encoding.Unicode.GetString(d));
                Console.WriteLine("UTF8 byte[] -> String    == {0}", System.Text.Encoding.UTF8.GetString(e));

                // Not printing properly.
                Console.WriteLine("ASCII byte[] -> String   == {0}", System.Text.Encoding.ASCII.GetString(f));

                Console.WriteLine("Output after encoding conversion");

                // Convert UTF8 byte[] to Default byte[]
                byte[] new_default = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, e);

                Console.WriteLine(System.Text.Encoding.Default.GetString(new_default));

                Console.ReadLine();
            }
        }


        static void string_2_binary()
        {
            /*
                string 타입의 문자열 을 바이너리 식 문자열으로 변경하려면,
                예를 들어 C# 문자열 'AB'를 유니코드 바이트 안에 표현되어 있는
                Binary Format인 '01000001000000000100001000000000' 으로 string에 저장하려면,
                (1) 먼저 C# string 문자열을 Unicode Encoding으로 된 byte[] 배열로 변경한다.
                이는 UnicodeEncoding.Unicode.GetBytes()을 사용하여 얻어 올 수 있다.
                (2) 하나의 byte에 저장된 Binary 포맷 데이타를 문자열로 변경하려면, Convert.ToString(byte1, base) 메서드를 사용한다.
                여기서 byte1은 입력 byte이며, base에는 2진수를 나타내는 2를 사용한다.
                (3) Step2에서 하나의 문제점은 리턴값이 8자리 보다 작을 경우 문자열 앞에 0을 채우지 않는다는 것이다.
                예를 들어, 00011001을 리턴하길 기대했을 때, 11001만 리턴하는 것이다.
                이를 보정하기 위해 string.PadLeft() 메서드를 사용하여 앞에 0을 채워준다.
                아래 예제는 문자열을 2진수 문자열로 변환하여 텍스트 파일에 저장하는 예이다. 
            */
            {
                string input = "iPad 가격";

                byte[] bytes = UnicodeEncoding.Unicode.GetBytes(input);
                string binaryString = string.Empty;

                foreach (byte b in bytes)
                {
                    // byte를 2진수 문자열로 변경
                    string s = Convert.ToString(b, 2);
                    binaryString += s.PadLeft(8, '0');
                }
                Console.WriteLine(binaryString);

                Console.ReadLine();
            }

            /*
                위와 반대로, 바이트 표현식인 2진수로 된 문자열을 실제 유니코드 문자열로 변환하기 위해서는,
                (1) 먼저 2진수 문자열을 8개씩 쪼개서 한 바이트씩 처리해야 한다.
                (2) 2진수 문자 8개를 숫자로 변경하기 위해서는 Convert.ToInt32(s, 2) 메서드를 사용한다.
                    여기서 첫번째 파라미터 s는 8자리 2진수 문자열을 나타내고, 두번째 파라미터 2는 2진수를 의미한다.
                    즉 문자열을 2진수에 기초하여 숫자로 변환하는 것이다.
                (3) 모든 2진수 문자열을 byte[] 배열에 넣었으면,
                    Unicode 인코딩의 GetString() 메서드를 사용하여 실제 유니코드 문자열로 변경한다.
            */
            {
                string binData = "0110100100000000010100000000000001100001000000000110010000000000";

                int nbytes = binData.Length / 8;
                byte[] outBytes = new byte[nbytes];

                for (int i = 0; i < nbytes; i++)
                {
                    // 8자리 숫자 즉 1바이트 문자열 얻기
                    string binStr = binData.Substring(i * 8, 8);
                    // 2진수 문자열을 숫자로 변경
                    outBytes[i] = (byte)Convert.ToInt32(binStr, 2);
                }

                // Unicode 인코딩으로 바이트를 문자열로
                string result = UnicodeEncoding.Unicode.GetString(outBytes);
                Console.WriteLine(result);

                Console.ReadLine();
            }
        }


        static void hex_string_2_byte_array()
        {
            /*
                byte 혹은 byte 배열을 16진 문자열로 변경하거나 반대로 16진수 문자열을 byte나 byte 배열로 변경해야 할 경우가 있다.
                아래는 각각의 경우에 대한 변환 방법이다.

                하나의 byte를 16진수(Hex) 문자열로 변경하는 것은
                byte의 ToString("X2") 메서드를 사용하여 간단히 변환할 수 있다 (아래 예제 1번).

                복수의 byte들 즉 바이트 배열을 16진수 문자열로 변경하기 위해서는
                바이트 하나 하나를 16진수 문자열로 변환한 후 이를 모두 결합시키면 되는데,
                아래 예제 2번과 같이 Array.ConvertAll() 메서드과 String.Concat() 를 쓰면 간단하게 전체 변환을 수행할 수 있다.
                또 다른 방식으로는 BitConverter.ToString() 메서드를 사용할 수 있는데,
                이 메서드는 바이트배열을 A1-B2-C3-11-2F 와 같이 "-" 로 연결된 16진수 문자열을 변환해 준다.
                여기서 "-" 이 필요 없는 경우에는 아래 예제 3번과 같이 Replace()를 써서 이를 제거한다.

                하나의 16진수 문자열을 한 byte로 변환하는 방법은 Convert.ToByte()를 쓰거나 byte.Parse() 메서드를 사용할 수 있다.
                (아래 예제 3번).

                여러 16진수 문자열을 byte[] 배열로 변환하기 위해서는
                아래 예제 4번과 같이 16진수 문자열을 2 문자씩 떼어내어 이를 바이트로 변환하고 바이트배열에 순차적으로 할당한다.
                아래 예제는 for 루프를 사용하여 Hex 문자를 바이트로 변환하여 해당 배열요소에 할당하는 예이다.
            */
            {
                // (1) 한 byte 를 Hex 문자로 변환하는 방법
                byte b1 = 0xFE;
                string hex1 = b1.ToString("X2");

                // (2) byte[] 을 Hex 문자열로 변환하는 방법
                byte[] bytes = new byte[] { 0xA1, 0xB2, 0xC3, 0x11, 0x2F };
                string h = string.Concat(System.Array.ConvertAll(bytes, byt => byt.ToString("X2")));
                // 혹은
                // h = BitConverter.ToString(bytes).Replace("-", "");


                // (3) 한 Hex 문자를 byte 로 변환하는 방법
                byte b2 = Convert.ToByte("3F", 16);
                // 혹은
                // b2 = byte.Parse("3F", NumberStyles.HexNumber);

                // (4) 여러 Hex 문자열을 byte[] 로 변환하는 방법
                string hexString = "A1B2C3";
                byte[] xbytes = new byte[hexString.Length / 2];
                for (int i = 0; i < xbytes.Length; i++)
                {
                    xbytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                Console.ReadLine();
            }
        }


        static void binary_string_2_byte_array()
        {
            /*
                바이트를 0과 1로된 비트 문자열로 변환하거나 반대로 비트 문자열을 바이트로 변환하는 방법을 소개한다.

                하나의 byte를 2진수 비트 문자열로 변경하는 것은 Convert.ToString() 메서드를 사용하여 변환할 수 있다.
                단, 아래 예제 1번에서 처럼 0x11 인 경우 Convert.ToString() 메서드는 유효숫자 "10001" 만을 리턴하기 때문에,
                8비트 모두를 넣기 위해 PadLeft() 메서드를 써서 전체 자리가 8자리이고
                빈 자리에 문자 "0"을 넣으라고 지정해 주어야 한다.

                만약 byte가 아니고 4 바이트 크기의 int 인 경우 동일한게 Convert.ToString() 메서드를 사용하여 변환하고,
                아래 예제 2번과 같이 PadLeft()의 전체 자릿수를 32로 지정한다.

                위와 반대로 2진수 비트 문자열을 byte 로 변환하기 위해서는 Convert.ToByte() 를 사용한다 (아래 예제 3번).
                또한, 32비트 비트 문자열을 int로 변환하기 위해서는 Convert.ToInt32()을 사용한다 (아래 예제 4번).
            */
            {
                // (1) byte를 비트문자열로 변환
                byte a = 0x11;
                string s = Convert.ToString(a, 2).PadLeft(8, '0'); // 00010001

                // (2) int를 비트문자열로 변환
                int j = 0x01020304;
                s = Convert.ToString(j, 2).PadLeft(32, '0');

                // (3) 비트 문자열을 1 바이트로 변환
                string bitStr = "01110000";
                byte b = Convert.ToByte(bitStr, 2); // 112 = 0x70

                // (4) 비트 문자열을 int 로 변환
                string bstr = "00000001000000100000001100000100";
                int i = Convert.ToInt32(bstr, 2);  // 16909060 = 0x01020304   

                Console.ReadLine();
            }
        }


        static void string_Format_use()
        {
            /*
                서식 지정자 (Format Specifier)

                C# .NET에는 크게 2개의 Format Specifier가 있다.
                일반적으로 많이 사용되는 형식을 표현하는 표준 Format Specifier와
                사용자가 임의로 다양하게 형식을 지정할 수 있는 Custom Format Specifier가 그것이다.
                일반적인 표준 출력 형식의 문법은 다음과 같다.

                    {n,w:tp}

                여기서 n은 Argument 위치이며 0부터 시작한다.
                w는 출력 Width를 가리키며, t는 출력 데이타 타입을
                그리고 마지막으로 p는 정확도(Precision)을 나타낸다.

                예를 들어 아래 예제를 살펴보면,
                string.Format의 첫번째 파라미터는 Standard Format Specifier를 표현하는 것으로,
                첫부분의 0은 Format Specifier다음의 첫번째 파라미터 즉 val을 입력으로 받는다는 뜻이고,
                콤마 뒤의 10은 전체 넓이가 10임을 의미하며
                따라서 출력변수 s는 앞의 공백을 포함하여 총 10자리를 갖게 된다.
                10 다음의 N은 출력이 Numeric 형식으로 출력된다는 것이고,
                이 형식은 만약 1000 자리마다 콤마를(Locale에 따라 다름) 표시한다.
                마지막 N다음의 2는 소숫점 2자리까지 출력하고 싶다는 것을 나타낸다. 
            */
            {
                decimal val = 1234.5678M;
                string s = string.Format("{0,10:N2}", val);
                // 출력: "  1,234.57"
                Console.WriteLine(s);

                Console.ReadLine();
            }

            /*
                숫자: 표준 (Standard Format Specifier)

                자주 사용되는 숫자 형식은 표준 Format Specifier를 사용하여 출력 포맷을 만들 수 있다.
                표준 Format 타입으로 N, D, C, X, F, E 등을 사용할 수 있는데,
                아래 예제는 이들의 사용 예를 보여주고 있다.
                모든 숫자 형식은 컴퓨터에 설정된 기본 Locale 에 따라 다르게 출력될 수 있으므로 주의할 필요가 있다.
                아래 예제 중 Currency 타입의 경우 만약 Locale이 English/US로 되어 있는 경우,
                기본적으로 소숫점 뒤 2자리 까지 출력한다.
                만약 Cents 단위를 없애고 싶으면 0:C0 과 같이 Precision을 0으로 하면 된다. 
            */
            {
                // N: Number 타입
                string.Format("{0:N2}", 1234.567); // 1,234.57

                // D: Decimal 타입
                string.Format("{0:D9}", 12345); // 000012345

                // C: Currency 타입
                string.Format("{0:C}", 12345); // $12,345.00
                string.Format("{0:C0}", 12345); // $12,345

                // X: 16진수
                string.Format("{0:X}", 1000); // 3E8

                // F: Fixed Point
                string.Format("{0:F3}", 12345.6); // 12345.600

                // E: Scientific
                string.Format("{0:E}", 12345.6); // 1.23456E+004

                Console.ReadLine();
            }

            /*
                숫자: 사용자 서식 지정자 (Custom Format Specifier)

                표준 형식이외에 개발자는 몇개의 심벌을 사용하여 다양하게 숫자 출력 형식을 지정할 수 있다.
                Custom Format Specifier에서 숫자를 표현하기 위해 사용하는 심벌은 다음과 같다.

                    # : Digit placeholder (0가 앞에 붙지 않음)
                    0 : Zero placeholder (0가 앞에 붙음)
                    . : 소숫점 (Decimal point)
                    , : 천 자리 (Thousands operator)
                    ; : Section separator

                이중 마지막의 Section separator는 다음과 같이 사용한다.
                Custom Format Specifier에서 Optional로 양수, 음수, 0 에 대해 각각 다른 표현식을 사용할 수 있다.
                예를 들어 아래와 같은 코드는 살펴보면, Format Specifier가 2개의 세미콜론으로 구분되어져
                3개의 섹션을 만들고 있음을 알 수 있다.

                    string.Format("{0:#,##0;(#,##0);Zero}", val)

                처음 섹션은 양수 포맷을 가리키고, 두번째는 음수 포맷,
                그리고 마지막은 값이 0 인 경우의 숫자 포맷을 가리킨다.
                즉, 숫자 값이 양수인 경우 천자리가 콤마로 표시되는 형식으로,
                숫자가 음수인 경우 마이너스(-) 대신 괄호로 둘러싸인 숫자로 (주로 회계에서 사용함),
                그리고 0인 경우 문자열 Zero를 리턴한다. 
            */
            {
                int val = -12345;
                string s = string.Format("{0:#,##0}", val);
                // 출력: -12,345

                s = string.Format("{0:#,##0;(#,##0)}", val);
                // 출력 : (12,345)

                val = 0;
                s = string.Format("{0:#,##0;(#,##0);Zero}", val);
                // 출력 : Zero

                Console.ReadLine();
            }

            /*
                날짜 : 표준 (Format Specifier)

                숫자와 마찬가지로 날짜 Format Specifier는 표준 Format Specifier와 Custom Format Specifier로 나눌 수 있다.
                자주 사용되는 날짜 형식은 표준 Format Specifier을 이용하여 표현할 수 있는데,
                여기에는 d, D, t, T, g, G, f, F, s, o, u 등등의 매우 다양한 지시어가 있다.
                날짜 및 시간 형식은 숫자와 마찬 가지로 Locale 설정에 따라 달라질 수 있다.
                즉, 각 국가마다 다른 날짜, 시간을 사용하고 사용자 컴퓨터의 Locale (제어판의 국가 및 언어 설정) 에 따라 다르게 출력될 수 있다.
                다음 예제들은 표준 Format Specifier를 사용했을때 어떻게 날짜 및 시간이 다르게 표현되는지 예를 보여준 것이다. 
            */
            {
                // d : 축약된 날짜 형식
                DateTime today = DateTime.Now;
                string.Format("{0:d}", today); // 4/14/2014

                // D : 긴 날짜 형식
                string.Format("{0:D}", today); // Monday, April 14, 2014

                // t : 축약된 시간
                string.Format("{0:t}", today); // 2:17 PM

                // T : 긴 시간 형식
                string.Format("{0:T}", today); // 2:18:01 PM

                // g : 일반 날 및 시간 (초 생략)
                string.Format("{0:g}", today); // 4/14/2014 2:18 PM

                // G : 일반 날짜 및 시간
                string.Format("{0:G}", today); // 4/14/2014 2:19:11 PM

                // f : Full 날짜 및 시간 (초 생략)
                string.Format("{0:f}", today);
                // Monday, April 14, 2014 2:52 PM

                // F : Full 날짜 및 시간
                string.Format("{0:F}", today);
                // Monday, April 14, 2014 2:52:11 PM

                // s : ISO 8601 표준
                string.Format("{0:s}", today);
                // 2014-04-14T14:53:09

                // o : Round-trip 패턴
                string.Format("{0:o}", today);
                // 2014-04-14T22:54:19.0279340Z

                // u : Universal Sortable 패턴
                string.Format("{0:u}", today);
                // 2014-04-14 15:15:51Z

                Console.ReadLine();
            }

            /*
                날짜: 사용자 지정 날짜 (Custom Format Specifier)

                표준 날짜 형식 이외의 날짜 포맷에 대해서는 Custom Format Specifier를 사용할 수 있는데,
                이의 지시어로서 아래와 같은 심벌들을 사용할 수 있다.
                그리고 이 Custom 날짜 심벌들을 아래 예제와 같이 연월일 시분초를 적절히 조합하여 다양하게 사용할 수 있다.

                  - 날짜 심볼
                    M: 월. 10 이하는 한자리
                    MM: 2자리 월
                    MMM: 축약형 월 이름(예: APR)
                    d: 일. 10 이하는 한자리
                    dd: 2자리 일자
                    ddd: 축약형 요일 이름(예: Mon)
                    yy: 2자리 연도
                    yyyy: 4자리 연도
                    h: 시간(12시간, 10 이하 한자리)
                    hh: 2자리 시간 (12시간)
                    H: 시간(24시간, 10 이하 한자리)
                    HH: 2자리 시간 (24시간)
                    m: 분(10 이하 한자리)
                    mm: 2자리 분
                    s: 초(10 이하 한자리)
                    ss: 2자리 초
                    tt: AM / PM
            */
            {
                DateTime today = DateTime.Now;
                string.Format("{0:M/d/yyyy}", today);
                // 출력 4/15/2014

                string.Format("{0:yyyy/MM/dd}", today);
                // 출력 2014/04/14

                string.Format("{0:d/M/yyyy HH:mm:ss}", today);
                // 출력 14/4/2014 14:59:24

                Console.ReadLine();
            }

            /*
                ToString() : Format Specifier

                숫자 혹은 날짜 타입의 ToString() 메서드는 Format Specifier를 받아들일 수 있다.
                즉, DateTime.ToString("s") 와 같이 표준 Format Specifier를 지정할 수도 있고,
                DateTime.ToString("yyyy/MM/dd") 와 같이 Custom Format Specifier를 지정할 수도 있다.
                DateTime.ToString()와 같이 파라미터가 없을 경우는 DateTime.ToString("G")와 동일한 포맷이다.
                또한 아래 예제에서 처럼 ToXxString()과 같은 보조 메서드들은
                각각 아래와 같은 D,T,d,t 등의 형식과 날짜 포맷이 동일하다.
            */
            {
                string s;
                DateTime today = DateTime.Now;
                s = today.ToString("s");
                s = today.ToString("yyyy/MM/dd");

                s = today.ToLongDateString();  // D 형식
                s = today.ToLongTimeString();  // T 형식
                s = today.ToShortDateString(); // d 형식
                s = today.ToShortTimeString(); // t 형식

                Console.ReadLine();
            }

            /*
                날짜 데이타를 문자열로 Transfer해야 할 때

                날짜 데이타를 문자열로 다른 국가 혹은 다른 시스템으로 Transfer 해야 할 때 주의해야 할 점이 있다.
                한 지역 혹은 한 시스템의 문자열 날짜 포맷이 다른 시스템으로 전송되었을 때,
                타 시스템은 날짜를 파싱할 수 없을 수도 있으며, 파싱하더라도 시간 차이로 인해 다른 시간으로 인식될 수 있다는 점이다.
                이러한 오류를 막기 위해 일반적으로 날짜를 UTC 표준 날짜로 변경하고,
                이 날짜 데이타를 ISO 8601 포맷으로 변경하여 타 시스템에 전송하는 것이 안전하다.
                C# .NET에서 이를 구현하는 방법은 여러가지가 있을 수 있는데, 아래 예제는 서로 다른 몇 가지 방식을 예로 들고 있다.
                현재 UTC 시간은 DateTime.UtcNow 속성에서 구할 수 있고,
                특정 DateTime 변수의 로컬 타임은 DateTime의 ToUniversalTime() 메서드를 사용하여 상응하는 UTC 시간을 구할 수 있다.
                아래 코드에서
                방법1은 표준 Format Specifier o 를 사용하여 ISO 8601 형식으로 밀리초 단위도 함께 포함하는 결과를 출력한다.
                방법2는 표준 Format Specifier s 를 사용하여 ISO 8601 형식으로 초 단위까지 결과를 출력한다.
                방법3은 Custom Format Specifier 를 사용하여 마찬가지로 ISO 8601 형식으로 초 단위까지 결과를 출력한다.
                모든 출력 문자열의 마지막에는 Z가 표시되어 있는데, 이는 해당 시간이 UTC 시간임을 표시한다.
                만약 이것이 생략되면 로컬타임으로 인식하게 된다. 
            */
            {
                DateTime today = DateTime.Now;
                DateTime utcTime = today.ToUniversalTime();

                // 방법 1
                string s = DateTime.UtcNow.ToString("o");
                // 출력: 2014-04-14T23:05:03.5243772Z

                // 방법 2
                s = DateTime.UtcNow.ToString("s",
                    System.Globalization.CultureInfo.InvariantCulture) + "Z";
                // 출력: 2014-04-14T23:05:03Z

                // 방법 3
                s = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                // 출력: 2014-04-14T23:05:03Z

                Console.ReadLine();
            }
        }


        static void BASE64_2_byte_array_or_string()
        {
            /*
                BASE64 Incoding / Decoding

                Base64는 바이너리 데이타를 아스키 문자열로 표현하는 인코딩 방식의 하나이다.
                Base64는 영문 대문자(A ~ Z) 26개, 영문 소문자 (a ~ z) 26개, 그리고 숫자 (0 ~ 9) 10개 등 62개의 값들을 기본적으로 가지며,
                마지막에 + 와 / 두개를 합쳐 총 64개의 인코딩 문자를 갖는다.

                Base64는 64개의 데이타를 가지므로 6비트의 공간만 필요하다.
                바이너리 데이타 즉 바이트들을 Base64 인코딩 데이타로 변환하기 위해서는
                연속적인 바이트들을 앞에서부터 6비트씩 끊어서 이를 Base64 인코딩값으로 변환하면 된다.

                바이트들의 모든 비트들을 남김없이 모두 Base64 인코딩값으로 변환하려면
                바이트들의 숫자가 3의 배수 (3, 6, 9, ... )이어야 한다.
                즉, 바이트 8비트와 Base64 6비트의 최소공배수인 24비트 (3 bytes)가 되면
                3 바이트의 데이타를 짤림이 없이 4개의 Base64 인코딩값으로 표현할 수 있다.
                예를 들어, ABC 라는 3개의 바이트가 있다면,
                아래 그림과 같이 이를 6비트씩 쪼개 4개의 Base64 인코딩 문자로 변경할 수 있다.
                만약 바이트의 숫자가 3의 배수가 아니라면, 아래에서 설명하는 Padding 과정을 거치게 된다.

                C#에서 바이트들을 Base64로 변환하기 위해서는
                간단하게 Convert.ToBase64String() 정적 메서드를 사용한다.

                Byte[] 배열을 웹상에서 전송하기 위해 많이 사용되는 방식으로 BASE64 인코딩을 들 수 있다.
                송신 쪽에서는 Convert.ToBase64String(byte[])를 사용하여
                바이트들을 BASE64 인코딩된 문자열로 변경하고 String을 전송하게 되고,
                수신 쪽에서는 Convert.FromBase64String(string)을 사용하여
                BASE64 인코딩된 문자열을 다시 바이트 배열로 변경하여 사용하게 된다. 
            */
            {
                // convert String -> Char[] 
                string str = "Hello 한국";
                char[] uchars = str.ToCharArray();

                // String은 바이트로 직접 변환할 수 없으며,
                // Encoding을 통해 변환 가능. 16바이트 생성
                byte[] unicodeBytes = System.Text.Encoding.Unicode.GetBytes(str);
                // 보다 컴팩트한 UTF8 인코딩. 12바이트 생성
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);

                // Byte Array를 BASE64 Encoding
                string s64 = Convert.ToBase64String(unicodeBytes);
                // BASE64 인코딩한 String을 다시 Byte Array로
                byte[] bytes64 = Convert.FromBase64String(s64);

                Console.ReadLine();
            }

            {
                // ABC 문자 바이트들
                byte[] abc = new byte[] { 0x41, 0x42, 0x43 };

                // 바이너리 데이타를 Base64로 변경
                string base64 = Convert.ToBase64String(abc);

                // 출력: QUJD
                Console.WriteLine(base64);

                Console.ReadLine();
            }

            /*
                BASE64 Padding

                바이트들을 Base64로 변환할 때, 만약 바이트의 숫자가 3의 배수가 아니라면, 빈 비트들을 패딩하게 된다.
                예를 들어, 2 바이트를 Base64로 변환하는 경우, 나머지 1 바이트를 추가해 총 3바이트를 처리하는데,
                가상으로 추가된 1 바이트는 모두 0 이라고 가정하고,
                2 바이트를 6비트씩 모두 처리한 후 마지막 6비트는 Base64 패딩 문자인 = 을 추가하게 된다.
                즉, 아래 그림을 보면, (1) AB 바이트에 대해 QU를 얻어내고 (2) B의 마지막 4비트와 패딩으로 들어간 2비트를 합쳐 I 를 얻어낸다.
                그리고 마지막 6비트는 패딩으로 문자 = 을 추가해서 결국 QUI= 라는 Base64 인코딩 결과를 얻게 된다.

                Base64 패딩은 만약 바이트수가 3의 배수이면 0개, (3의 배수 - 1) 이면, 1개,
                그리고 (3의 배수 -2) 이면 2개가 된다.    
            */
            {
                // AB 문자 바이트들
                byte[] abc = new byte[] { 0x41, 0x42 };

                // 바이너리 데이타를 Base64로 변경
                string base64 = Convert.ToBase64String(abc);

                // 출력: QUI=
                Console.WriteLine(base64);

                Console.ReadLine();
            }

            /*
                BASE64 <-> string Incoding / Decoding

                문자열을 Base64로 인코딩하기 위해서는 우선 문자열을 바이트배열로 전환해야 한다.
                문자열을 바이트배열로 변환하기 위해서는
                문자열 인코딩 아티클 에서 설명한 바와 같이 Encoding을 통해 변환하게 된다.
                일단 바이트 배열로 변환한 후에는 Convert.ToBase64String() 를 사용하여 다시 Base64로 인코딩하면 된다.
                아래 예제A는 문자열을 유니코드 바이트배열로 변경하여 다시 Base64 인코딩으로 변환한 예이다.

                위의 인코딩의 반대 과정인 디코딩과정은
                Base64로 인코드된 문자열을 Convert.FromBase64String() 메서드를 통해 다시 바이트배열로 변환하면 된다.
                그리고 이어 이 바이트배열을 다시 유니코드 인코딩을 통해 (Encoding.Unicode.GetString() 메서드 사용) 원래의 문자열로 변환하게 된다. 
            */
            {
                // Incoding
                // 문자열을 유니코드 인코딩으로 바이트배열로 변환
                string s = "John 굿모닝";
                byte[] bytes = Encoding.Unicode.GetBytes(s);

                // 바이트들을 Base64로 변환
                string base64 = Convert.ToBase64String(bytes);

                // 출력: SgBvAGgAbgAgAH+tqLrdsg==
                Console.WriteLine(base64);


                // Decoding
                // Base64 인코드된 문자열을 바이트배열로 변환
                byte[] orgBytes = Convert.FromBase64String("SgBvAGgAbgAgAH+tqLrdsg==");

                // 바이트들을 다시 유니코드 문자열로
                string orgStr = Encoding.Unicode.GetString(orgBytes);

                // 출력: John 굿모닝
                Console.WriteLine(orgStr);

                Console.ReadLine();
            }
        }


        static void Unicode_and_BOM()
        {
            /*
                일련의 텍스트 데이타를 16비트 유니코드로 저장할 때, Endian 에 따라 바이트 위치가 변경될 수 있다.
                즉, Big-Endian의 경우, MSB (most significant byte) 바이트를 먼저 쓰는 반면,
                Little-Endian의 경우, LSB (least significant byte) 바이트를 먼저 쓰게 된다.
                예를 들어, 한글 "가"의 유니코드 포인트는 U+AC00 로서 Big-Endian의 경우 "AC 00" (낮은 메모리에서 높은 메모리 방향) 와 같이 저장되지만,
                Little-Endian의 경우 "00 AC" 와 같이 저장된다.

                텍스트 파일에 이러한 유니코드 데이타를 저장할 때,
                어떤 Endian이 사용되었는지를 나타내기 위해 BOM (byte order mark) 이라는 표시를 사용하게 되었다.
                즉, Big-Endian의 경우 파일의 첫부분에 "FE FF" 를 사용하였고,
                Little-Endian의 경우 "FF FE" 라는 BOM 마크를 표시한 것이다.
                (BOM은 텍스트 파일 이외에 텍스트 스트림 등에 나타날 수 있으며, Optional로서 항상 있는 것은 아니다)

            */
            {
                Console.ReadLine();
            }
        }


        static void UTF_incoding_BOM()
        {
            /*
                예전 16비트 유니코드는 UCS-2 인코딩을 사용하다 이를 좀 더 확장한 UTF-16 인코딩을 사용하고 있다.
                (C#에서 Unicode Encoding이라 함은 UTF-16 인코딩을 의미한다).
                UTF-16 인코딩은 모든 문자에 대해 16비트를 사용하므로 알파벳과 같은 8비트로 전송할 수 있는 문자들에는 비효율적인 측면이 있다.
                UTF-8 인코딩은 문자의 종류에 따라 1바이트부터 4바이트까지 다양하게 인코딩할 수 있는데,
                ASCII 문자는 1바이트로 표현하기 때문에 ASCII 데이타가 주를 이루지만 다른 문자들도 함께 있어야 하는 상황에 유용하다.
                UTF 인코딩에는 UTF-8, UTF-16 이외에 UTF-32, UTF-7 등이 있다.
                텍스트 파일 혹은 텍스트 스트림에서는 이러한 UTF 인코딩의 종류에 따라 서로 다른 BOM (byte order mark)을 사용하고 있다.
                아래는 각 인코딩에서 사용하는 BOM을 표시한 것이다.

                    Incoding Type	            BOM
                    Unicode (Big-Endian)	    FE FF
                    Unicode (Little-Endian)	    FF FE
                    UTF8	                    EF BB BF
                    UTF-32 (Big-Endian)	        00 00 FE FF
                    UTF-32 (Little-Endian)	    FF FE 00 00
            */
            {
                Console.ReadLine();
            }
        }


        static void text_file_incoding()
        {
            /*
                C# 코드에서 텍스트 파일을 생성할 때, 특정한 인코딩을 지정할 수 있다.
                특히, 텍스트가 ASCII 이외의 국제 문자의 경우 UTF 인코딩을 지정해 주는 것이 좋다.
                예를 들어, 한글 문자열이 있는 텍스트 파일을 .csv 파일로 저장하고 이를 영문 OS에서 Excel 로 읽으면,
                한글이 깨져 보이게 된다. (동일한 .csv 파일을 NotePad로 읽으면 한글을 읽을 수 있다).
                따라서, 이러한 경우는 인코딩을 명시하는 것이 필요하다.

                아래 예제에서 StreamWriter 객체를 생성할 때 생성자에서 Encoding.UTF8 과 같이 인코딩 방식을 지정하였다.
                이렇게 인코딩을 지정하면, 텍스트 파일의 처음 BOM 부분이 자동으로 추가되게 된다.
                즉, UTF8의 경우 파일 처음 부분에 0xEF 0xBB 0xBF 등 3개의 바이트가 BOM으로 추가된다.

                아래 예제는 다양한 인코딩 방식으로 동일한 데이타를 저장한 예이다.
                이 코드를 실행하여 생성된 텍스트 파일을 Hex Editor로 살펴보면 아래 그림과 같이 BOM이 자동 추가됨을 알 수 있다.
            */
            {
                string data = "A";
                //string data = "가";

                StreamWriter wr = new StreamWriter("default.csv");
                wr.Write(data);
                wr.Close();

                wr = new StreamWriter("utf8.csv", false, Encoding.UTF8);
                wr.Write(data);
                wr.Close();

                wr = new StreamWriter("unicode.csv", false, Encoding.Unicode);
                wr.Write(data);
                wr.Close();

                wr = new StreamWriter("utf32.csv", false, Encoding.UTF32);
                wr.Write(data);
                wr.Close();

                /*
                    wr = new StreamWriter("utf7.csv", false, Encoding.UTF7);
                    wr.Write(data);
                    wr.Close();

                    wr = new StreamWriter("cp949.csv", false, Encoding.GetEncoding(949));
                    wr.Write(data);
                    wr.Close();
                */

                Console.ReadLine();
            }
        }

        static void string_search()
        {
            {
                string data_1 = "Hello";
                data_1.StartsWith("Hello"); // "Hello"로 시작하는지

                string data_2 = "World";
                data_2.EndsWith("World"); // "World!"로 끝나는지

                Console.ReadLine();
            }

            {
                "Hello".Contains("hello");  // false

                Console.ReadLine();
            }

            {
                string text = "Hello, world!";
                bool contains = text.IndexOf("WORLD", StringComparison.OrdinalIgnoreCase) >= 0; //대소문자를 무시함

                Console.WriteLine(contains);  // true

                Console.ReadLine();
            }
        }


        static void string_filter()
        {
            {
                // 해당 문자가 알파벳 문자(Unicode에서 Letter로 분류되는 문자) 인지 여부를 반환합니다.

                Console.WriteLine(char.IsLetter('A')); // True
                Console.WriteLine(char.IsLetter('z')); // True
                Console.WriteLine(char.IsLetter('1')); // False
                Console.WriteLine(char.IsLetter('!')); // False

                Console.ReadLine();
            }

            {
                // LINQ를 사용해서 문자열 전체 검사

                string s = "Hello";
                bool allLetters = s.All(char.IsLetter);  // True

                string t = "Hi123";
                bool allLetters2 = t.All(char.IsLetter); // False
            }

            {
                string original = "HeLLo WoRLd!";
                string lower = original.ToLower();

                Console.WriteLine(lower);  // 출력: "hello world!"

                string upper = original.ToUpper();

                Console.WriteLine(upper);  // 출력: "HELLO WORLD!"

                Console.ReadLine();
            }

            {
                string test = "A1 3_#";

                foreach (char c in test)
                {
                    Console.WriteLine($"문자: '{c}'");

                    // 숫자인지 확인
                    Console.WriteLine($"IsDigit: {char.IsDigit(c)}");

                    // 문자 또는 숫자인지 확인
                    Console.WriteLine($"IsLetterOrDigit: {char.IsLetterOrDigit(c)}");

                    // 공백문자인지 확인
                    Console.WriteLine($"IsWhiteSpace: {char.IsWhiteSpace(c)}");

                    Console.WriteLine();
                }

                Console.ReadLine();
            }
        }

        static void string_language()
        {
            {
                string turkish = "I"; // 터키어에서는 소문자 'ı'로 변환됨
                string lowerInvariant = turkish.ToLowerInvariant();  // "i", 고정된 문화권(영어 기반)으로 변환
                string lowerTurkish = turkish.ToLower(new CultureInfo("tr-TR"));  // "ı", 특정 문화권 기준으로 변환

                Console.WriteLine(lowerInvariant);  // "i"
                Console.WriteLine(lowerTurkish);    // "ı"
            }
        }


        public static void Test()
        {
            //string_language();

            //string_filter();

            //string_search();

            //text_file_incoding();

            //UTF_incoding_BOM();

            //Unicode_and_BOM();

            //BASE64_2_byte_array_or_string();

            //string_Format_use();

            //binary_string_2_byte_array();

            //hex_string_2_byte_array();

            //string_2_binary();

            //string_2_byte_array();

            //StringBuilder_what();

            //string_character_array();

            //string_InterpolatedString();

            //string_what();
        }
    }
}
