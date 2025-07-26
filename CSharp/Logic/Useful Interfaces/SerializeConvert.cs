using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;




namespace UsefulInterfaces
{
    public class Comparison
    {
        public class Money : IFormattable
        {
            public decimal Amount;
            public string Currency;
            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (format == "SIMPLE")
                    return $"{Amount} {Currency}";
                else
                    return $"{Currency} {Amount:N2}";
            }
            public override string ToString() => ToString(null, null);
        }

        static void override_IFormattable()
        {
            /*
                IFormattable

                ✅ 목적
                  - ToString(string format, IFormatProvider formatProvider) 제공
                  - 커스텀 출력 포맷 지원
            */
            var m = new Money { Amount = 12345.67m, Currency = "KRW" };
            Console.WriteLine(m.ToString("SIMPLE", null)); // "12345.67 KRW"
        }


        public class MyNumber : IConvertible
        {
            public int Value { get; set; }

            public MyNumber(int v) { Value = v; }

            public TypeCode GetTypeCode() => TypeCode.Object;

            public bool ToBoolean(IFormatProvider provider) => Value != 0;
            public byte ToByte(IFormatProvider provider) => Convert.ToByte(Value);
            public char ToChar(IFormatProvider provider) => Convert.ToChar(Value);
            public DateTime ToDateTime(IFormatProvider provider) => new DateTime(Value);
            public decimal ToDecimal(IFormatProvider provider) => Value;
            public double ToDouble(IFormatProvider provider) => Value;
            public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Value);
            public int ToInt32(IFormatProvider provider) => Value;
            public long ToInt64(IFormatProvider provider) => Value;
            public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Value);
            public float ToSingle(IFormatProvider provider) => Value;
            public string ToString(IFormatProvider provider) => Value.ToString(provider);
            public object ToType(Type conversionType, IFormatProvider provider) =>
                Convert.ChangeType(Value, conversionType, provider);
            public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Value);
            public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Value);
            public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Value);
        }

        static void override_IConvertible()
        {
            /*
                IConvertible

                ✅ 목적
                  - 커스텀 타입을 .NET의 기본 데이터 타입(int, double, string 등)으로 변환시킬 때                
            */

            MyNumber num = new MyNumber(42);
            IConvertible convertible = num;

            // 다양한 타입으로 변환
            int asInt = convertible.ToInt32(null);
            string asStr = convertible.ToString(null);
            bool asBool = convertible.ToBoolean(null);
            double asDouble = convertible.ToDouble(null);

            Console.WriteLine($"ToInt32: {asInt}");     // 42
            Console.WriteLine($"ToString: {asStr}");    // "42"
            Console.WriteLine($"ToBoolean: {asBool}");  // True
            Console.WriteLine($"ToDouble: {asDouble}"); // 42

            // Convert.ChangeType을 통해 자동 변환도 가능
            object boxed = num;
            Console.WriteLine("ChangeType to int: " + Convert.ChangeType(boxed, typeof(int)));
            Console.WriteLine("ChangeType to string: " + Convert.ChangeType(boxed, typeof(string)));
        }

        //=========================================================================================

        [Serializable]
        public class CustomData : System.Runtime.Serialization.ISerializable
        {
            public int Age;
            public string Name;

            public CustomData() { }

            protected CustomData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                Age = info.GetInt32("Age");
                Name = info.GetString("Name");
            }
            public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                info.AddValue("Age", Age);
                info.AddValue("Name", Name);
            }
        }

        static void override_ISerializable()
        {
            /*
                ISerializable

                ✅ 목적
                  - 커스텀 직렬화 포맷이 필요한 경우
                  - 예: 네트워크/파일 저장 시 특정 데이터만 저장, 버전 호환 등
            */

            // 직렬화할 객체 생성
            var data = new CustomData { Age = 42, Name = "Alice" };

            // 메모리 스트림에 바이너리 직렬화
            var ms = new MemoryStream();
            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(ms, data);

            // 스트림 포지션을 맨 앞으로 되돌림
            ms.Position = 0;

            // 역직렬화
            var restored = (CustomData)formatter.Deserialize(ms);

            // 결과 확인
            Console.WriteLine($"Deserialized: Age={restored.Age}, Name={restored.Name}");
        }

        //=========================================================================================

        public class PersonXML : IXmlSerializable
        {
            public string Name { get; set; }
            public int Age { get; set; }

            // 예시: "Age"는 XML에 저장하지 않고, "Name"만 저장
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElementString("Name", Name);
                // Age는 저장하지 않음!
            }

            public void ReadXml(XmlReader reader)
            {
                reader.MoveToContent();
                reader.ReadStartElement();
                Name = reader.ReadElementContentAsString("Name", "");
                // Age는 XML에 없으므로 기본값 유지
                reader.ReadEndElement();
            }

            // 스키마 사용 안 함
            public XmlSchema GetSchema() => null;
        }

        static void override_IXmlSerializable()
        {
            /*
                IXmlSerializable

                ✅ 목적
                  - 내 객체를 원하는 XML 포맷으로 저장/복원하고 싶을 때              
            */
            var p = new PersonXML { Name = "Alice", Age = 30 };

            // 직렬화
            var xmlSerializer = new XmlSerializer(typeof(PersonXML));
            using (var sw = new StringWriter())
            {
                xmlSerializer.Serialize(sw, p);
                string xml = sw.ToString();
                Console.WriteLine("Serialized XML:\n" + xml);

                // 역직렬화
                using (var sr = new StringReader(xml))
                {
                    var restored = (PersonXML)xmlSerializer.Deserialize(sr);
                    Console.WriteLine($"\nDeserialized: Name={restored.Name}, Age={restored.Age}");
                    // Age는 0 (기본값)
                }
            }
            /*
                출력:
                Serialized XML:
                <?xml version="1.0" encoding="utf-16"?>
                <Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                  <Name>Alice</Name>
                </Person>

                Deserialized: Name=Alice, Age=0            
            */
        }


        public static void Test()
        {
            override_IXmlSerializable();

            override_ISerializable();

            override_IFormattable();

            override_IConvertible();
        }
    }
}
