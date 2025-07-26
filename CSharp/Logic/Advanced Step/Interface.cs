using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdvancedStep
{
    public class Interface
    {
        static void interface_what()
        {
            /*
                interface

                클래스와 비슷하게 인터페이스는 메서드, 속성, 이벤트 등을 갖지만,
                인터페이스는 이를 직접 구현하지 않고 단지 정의(prototype definition)만을 갖는다.
                즉, 인터페이스는 추상 멤버(abstract member)로만 구성된 추상 베이스클래스(abstract base class)와 개념적으로 비슷하다.
                클래스가 인터페이스를 가지는 경우 해당 인터페이스의 모든 멤버에 대한 구현(implementation)을 제공해야 한다.

                클래스는 하나의 베이스 클래스만을 가질 수 있지만 인터페이스는 여러 개를 가질 수 있다.
                아래의 예를 보면, MyConnection이라는 클래스는 Component라는 하나의 베이스클래스와
                IDbConnection, IDisposable이라는 2개의 인터페이스를 상속하고 있음을 알 수 있다.

                    public class MyConnection : Component, IDbConnection, IDisposable
                    { }
            */
            {
                Console.ReadLine();
            }

            /*
                interface 정의 
            
                인터페이스는 C# 키워드 interface를 사용하여 정의한다.
                인터페이스 정의 시에는 (메서드와 같은) 내부 멤버들에 대해 public과 같은 접근 제한자를 사용하지 않는다.
                예를 들어, 아래 CompareTo() 메서드 앞에 public 을 쓸 수 없다.

                    public interface IComparable
                    {
                        int CompareTo(object obj);
                    }
            */
            {
                Console.ReadLine();
            }

            /*
                interface 구현 
            
                C# 클래스가 인터페이스를 갖는 경우 인터페이스의 모든 멤버에 대한 구현을 제공해야 한다.
                C# 에서는 인터페이스로부터 직접 new를 사용하여 객체를 생성할 수 없다.
                아래의 클래스는 IComparable이라는 인터페이스를 갖는 경우로서 CompareTo() 메서드를 구현한 예이다.

                    public class MyClass : IComparable
                    {
                        private int key;
                        private int value;

                        // IComparable 의 CompareTo 메서드 구현
                        public int CompareTo(object obj)
                        {
                            MyClass target = (MyClass)obj;
                            return this.key.CompareTo(target.key);
                        }
                    }

            */
            {
                Console.ReadLine();
            }
        }


        static void interface_use()
        {
            /*
                C# 실무에서 클래스와 인터페이스를 잘 정의하고 사용하는 것은 매우 중요하다.
                비지니스를 객체지향 프로그래밍으로 디자인하고 구현하는데 가장 중요한 핵심이기 때문이다.
                자연스럽게 .NET Framework도 상당히 많은 인터페이스를 구현했으며, 자주 사용되고 있다.

                다음의 예는 IDnConnection이라는 인터페이스를 사용하는 예제이다.
                이 코드에서 GetDbConnection() 메서드는 시스템의 구성파일로부터 DB타입과
                Connection String을 받아와 데이타베이스 Connection을 리턴한다.
                GetDbConnection()가 DB connection을 리턴할 때 IDbConnection을 리턴하고 있는데,
                이 때문에 이 메서드를 사용하는 클라이언트에서는 어떤 DB를 사용하든지 상관없이
                모든 DB 클래스에 공통적으로 구현된 IDbConnection 메서드들을 사용할 수 있게 된다.

                    public void Run()
                    {
                        // 인터페이스 사용하기 때문에
                        // 특정 DB Connection 을 신경 쓸 필요가 없다
                        IDbConnection dbCon = GetDbConnection();
                        dbCon.Open();
                        if (dbCon.State == ConnectionState.Open)
                        {
                            dbCon.Close();
                        }
                    }

                    // IDbConnection 인터페이스를 리턴
                    public IDbConnection GetDbConnection()
                    {
                        IDbConnection dbConn = null;
                        string cn = ConfigurationManager.AppSettings["Connection"];
                        switch (ConfigurationManager.AppSettings["DbType"])
                        {
                            case "SQLServer":
                                dbConn = new SqlConnection(cn);
                                break;
                            case "Oracle":
                                dbConn = new OracleConnection(cn);
                                break;
                            case "OleDB":
                                dbConn = new OleDbConnection(cn);
                                break;         
                        }

                        return dbConn;
                    }
            */
            {
                Console.ReadLine();
            }
        }

		interface IMyFunctor
		{
			void onTest1();
		}

		class BaseClass : IMyFunctor
		{
			public void onTest1()
			{
				Console.WriteLine("BaseClass.onTest1()");
				//base.onTest1(); //컴파일 에러 !!!
				//this.onTest1(); //재귀호출이 계속되어 Stack Overflow 예외 발생 !!
			}
		}

		class DrivenClass : BaseClass, IMyFunctor
		{
			public new void onTest1()
			{
				Console.WriteLine("DrivenClass.onTest1()");
				base.onTest1();
			}
		}

		class MyClass : DrivenClass, IMyFunctor
		{
			public new void onTest1()
			{
				Console.WriteLine("MyClass.onTest1()");
				base.onTest1();
			}
		}

		static void interface_override()
		{
			IMyFunctor myFunctor = new MyClass();
			BaseClass baseClass = new MyClass();
			MyClass myClass = new MyClass();

			Console.WriteLine("Call IMyFunctor.onTest1()");
			myFunctor.onTest1();
			Console.ReadLine();
			/*
				Call IMyFunctor.onTest1()
				DrivenClass.onTest1()
				MyClass.onTest1()			
				BaseClass.onTest1()			
			*/

			Console.WriteLine("Call BaseClass.onTest1()");
			baseClass.onTest1();
			Console.ReadLine();
			/*
				Call BaseClass.onTest1()
				BaseClass.onTest1()
			*/

			Console.WriteLine("Call MyClass.onTest1()");
			myClass.onTest1();
			Console.ReadLine();
			/*
				Call MyClass.onTest1()
				MyClass.onTest1()
				DrivenClass.onTest1()
				BaseClass.onTest1()
			*/
		}

		public static void Test()
        {
			//interface_override();

			//interface_use();

			//interface_what();
		}
    }
}
