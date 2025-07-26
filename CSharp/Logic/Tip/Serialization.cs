using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tip
{
    public class Serialization
    {
        static void dotnet_serialzation()
        {
            /*
                .NET Serialization

                클래스 객체의 데이타를 파일이나 DB에 저장하거나 웹서비스 등으로 네트워크 상으로
                전송할 수 있는데 이를 Serialization이라 한다.
                이렇게 저장 혹은 전송된 결과는 다시 그대로 메모리상의 클래스 객체로 가져올 수 있는 이를 Deserialization이라 한다.
                .NET에서는 저장 포맷에 따라 크게 세가지 방식의 Serialization을 제공하는데,
                XML Serialization, SoapFormatter Serialization, Binary Serialization이 그것이다. 
            */
            {
                Console.ReadLine();
            }
        }


        // XML Serialzation
        public class XMLInfo  // 클래스는 public 이어야
        {
            // public 멤버만 저장
            public string Name;
            public int? Height;
            public int Age
            {
                get { return _age; }
                set { if (value >= 0) _age = value; }
            }

            // private 멤머이므로 저장 안함
            private int avgWeight;
            private int _age;

            public XMLInfo()
            {
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}", Name, Age);
            }
        }

        static void XML_serialization()
        {
            /*
                XML Serialziation은 클래스 객체 데이타를 XML 포맷으로 저장하는데, 특히 public 멤버만 저장한다.
                즉, public이 아닌 필드/속성은 저장되지 않는다.
                (이렇게 public 멤버들만 저장하는 것을 Shallow Serialization이라 부르며,
                모든 멤버를 다 저장하는 것은 Deep Serialization이라 부른다.
                XML Serialziation은 Shallow Serialization은,
                SoapFormatter/Binary Serialization은 Deep Serialization을 구현한다)
                또한 Serialziation할 클래스는 public으로 선언되어져야 하며
                Deserialization을 위해 파라미터가 없는 생성자를 가져야 한다. 
            */
            {
                string xmlFilename = @"C:\temp\test.xml";

                // serialization
                {
                    XMLInfo objXMLInfo = new XMLInfo();
                    objXMLInfo.Name = "홍길동";
                    objXMLInfo.Age = 26;

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLInfo));
                    using (StreamWriter wr = new StreamWriter(xmlFilename))
                    {
                        xmlSerializer.Serialize(wr, objXMLInfo);
                    }
                }

                // deserialization
                {
                    XMLInfo p;
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLInfo));
                    using (StreamReader rdr = new StreamReader(xmlFilename))
                    {
                        p = (XMLInfo)xmlSerializer.Deserialize(rdr);
                    }
                }

                /*
                    // saved XML file
                    //
                    <?xml version="1.0" encoding="utf-8"?>
                    <Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                          <Name>홍길동</Name>
                          <Height xsi:nil="true" />
                          <Age>26</Age>
                    </Person> 
                */

                Console.ReadLine();
            }
        }


        // SoapFormatter Serialzation
        [Serializable]
        class SoapInfo
        {
            public string Name;

            // Soap Serailization은 Generic Type을 
            // 지원하지 않는다. 
            // int? 은 Nullable<int>이므로 에러 발생함.
            //public int? Height;

            public int Age
            {
                get { return _age; }
                set { if (value >= 0) _age = value; }
            }

            //Serialize를 원하지 않는 필드는
            //아래 NonSerialized Attribute를 표시
            [NonSerialized]
            private int avgWeight;

            private int _age;

            public SoapInfo()
            {
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}", Name, Age);
            }
        }

        static void SoapFormatter_serialization()
        {
            /*
                SoapFormatter Serialization은 SOAP (Simple Object Access Protocol) 포맷(XML 형태)으로 클래스 객체를 Serialize해서 저장한다.
                SOAP 포맷은 웹 서비스를 통해 데이타를 주고 받을 때 많이 사용된다.
                SoapFormatter와 Binary Serialization을 사용하기 위해서는 클래스 선언문 위에 [Serializable]이라는 Attribute를 붙여야 한다.
                XML Serialziation와 달리, 클래스가 public일 필요가 없으며 public 멤버 뿐만 아니라 모든 멤버들을 Serialize한다.
                BinaryFormatter와 다르게 SoapFormatter은 Generic Type을 저장할 수 없다. 
            */
            {
                string xmlFilename = @"C:\temp\test.xml";

                // serialization
                {
                    SoapInfo objSoapInfo = new SoapInfo();
                    objSoapInfo.Name = "홍길동";
                    objSoapInfo.Age = 26;

                    SoapFormatter soapFmt = new SoapFormatter();
                    using (FileStream fs = new FileStream(xmlFilename, FileMode.Create))
                    {
                        soapFmt.Serialize(fs, objSoapInfo);
                    }
                }

                // deserialization
                {
                    SoapInfo p;
                    SoapFormatter soapFmt = new SoapFormatter(); ;
                    using (FileStream rdr = new FileStream(xmlFilename, FileMode.Open))
                    {
                        p = (SoapInfo)soapFmt.Deserialize(rdr);
                    }
                }

                /*
                    // saved Soap XML format file
                    //
                    <SOAP-ENV:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:SOAP-ENC="http://schemas.xmlsoap.org/soap/encoding/" xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/" xmlns:clr="http://schemas.microsoft.com/soap/encoding/clr/1.0" SOAP-ENV:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/">
                    <SOAP-ENV:Body>
                    <a1:SoapInfo id="ref-1" xmlns:a1="http://schemas.microsoft.com/clr/nsassem/Serial/Serial%2C%20Version%3D1.0.0.0%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull">
                    <Name id="ref-3">홍길동</Name>
                    <_age>26</_age>
                    </a1:SoapInfo>
                    </SOAP-ENV:Body>
                    </SOAP-ENV:Envelope>
                */

                Console.ReadLine();
            }
        }


        // BinaryFormatter Serialzation
        [Serializable]
        class BinaryInfo
        {
            public string Name;

            // Binary Serailization은 Generic Type을 
            // 지원한다.        
            public Nullable<int> Height;

            public int Age
            {
                get { return _age; }
                set { if (value >= 0) _age = value; }
            }

            //Serialize를 원하지 않는 필드는
            //아래 NonSerialized Attribute를 표시
            [NonSerialized]
            private int avgWeight;

            private int _age;

            public BinaryInfo()
            {
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}", Name, Age);
            }
        }

        static void BinaryFormatter_serialization()
        {
            /*
                BinaryFormatter Serialization은 .NET Framework 고유의 방식으로 클래스 객체를 Serialize해서 저장한다.
                즉, .NET에서만 사용되며, SOAP과 다르게 타 시스템과 호환성을 제공하지 않는다.
                클래스 선언문 위에 [Serializable]이라는 Attribute를 붙여야 하며, 모든 멤버들을 Serialize한다.
                BinaryFormatter은 SoapFormatter에서 저장할 수 없는 Generic Type도 저장할 수 있다.
                상대적으로 Output 크기가 작으며, 속도도 다른 Serialization 방식보다 빠른 편이다.
            */
            {
                string xmlFilename = @"C:\temp\test.dat";

                // serialization
                {
                    BinaryInfo objBinaryInfo = new BinaryInfo();
                    objBinaryInfo.Name = "홍길동";
                    objBinaryInfo.Age = 26;
                    objBinaryInfo.Height = 6;

                    BinaryFormatter binFmt = new BinaryFormatter();
                    using (FileStream fs = new FileStream(xmlFilename, FileMode.Create))
                    {
                        binFmt.Serialize(fs, objBinaryInfo);
                    }
                }

                // deserialization
                {
                    BinaryInfo p;
                    BinaryFormatter binFmt = new BinaryFormatter(); ;
                    using (FileStream rdr = new FileStream(xmlFilename, FileMode.Open))
                    {
                        p = (BinaryInfo)binFmt.Deserialize(rdr);
                    }
                }

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //BinaryFormatter_serialization();

            //SoapFormatter_serialization();

            //XML_serialization();
        }
    }
}
