using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace BasicStep
{
    public class MyException : System.Exception
    {
        public MyException() : base() { }
        public MyException(string message) : base(message) { }
        public MyException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class Exception
    {
        static void exception_what()
        {
            /*
                C#을 포함한 모든 .NET 프로그래밍 언어는 .NET Framework의 Exception 메카니즘에 따라 Exception을 처리한다.
                .NET의 System.Exception은 모든 Exception의 Base 클래스이며,
                예외 처리는 이 Exception 객체를 기본으로 처리하게 된다.

                만약 Exception이 발생하였는데 이를 프로그램 내에서 처리하지 않으면 (이를 Unhandled Exception이라 부른다) 프로그램은 Crash하여 종료하게 된다.

                C#에서는 try, catch, finally라는 키워드를 사용하여 Exception을 핸들링하게 되며,
                또한 throw라는 C# 키워드를 통해 Exception을 만들어 던지거나 혹은 기존 Exception을 다시 던질 수 있다.

                    try
                    {
                        DoSomething();
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                        throw;
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void try_catch_finally_what()
        {
            /*
                try 블럭은 실제 실행하고 싶은 프로그램 명령문들을 갖는 블럭이다.
                만약 여기서 어떤 에러가 발생하면, 이는 catch 문에서 잡히게 된다.
                catch문은 모든 Exception을 일괄적으로 잡거나 혹은 특정 Exception을 선별하여 잡을 수 있다.

                모든 Exception을 잡고 싶을 때는 catch { ... } 와 같이 하거나 catch (Exception ex) { ... }처럼
                모든 Exception의 베이스 클래스인 System.Exception를 잡으면 된다.

                특정 Exception을 잡기 위해서는 해당 Exception Type을 catch하면 된다.
                즉, Argument와 관련된 Exception을 잡고 싶으면, catch (ArgumentException ex) { ... } 와 같이 잡게된다.

                catch 블럭은 하나 혹은 여러 개 일 수 있다.
                여러 catch를 사용하는 이유는 각 Exception 유형에 따라 서로 다른 에러 핸들링을 하기 위함이다.

                finally는 Exception이 발생했던 발생하지 않았던 상관없이 마지막에 반드시 실행되는 블럭이다.
                예를 들어, try 블럭에서 SQL Connection객체를 만든 경우,
                finally 블럭에서 Connection 객체의 Close() 메서드를 호출하면,
                에러 발생 여부와 상관없이 항상 Connection객체가 닫히게 된다.

                    try
                    {
                       ...
                    }
                    catch (ArgumentException ex)
                    {
                       ...
                    }
                    catch (AccessViolationException ex)
                    {
                       ...
                    }
                    finally
                    {
                       ...
                    }
            */
            {
                Console.ReadLine();
            }
        }


        static void throw_what()
        {
            /*
                try 블럭에서 Exception 이 발생하였는데 이를 catch 문에서 잡었다면, Exception은 이미 처리된 것으로 간주된다.
                때때로 catch문에서 기존의 Exception을 다시 상위 호출자로 보내고 싶을 때가 있는데, 이때 throw 를 사용한다.

                throw 문은 크게 2가지로 구별될 수 있다.
                즉, (1) throw 문 다음에 Exception 객체가 있는 경우와 (2) throw 문 다음에 아무것도 없는 경우이다.

                throw 문 다음에 Exception 객체가 있는 경우
                throw문 다음에 Exception 객체를 둘 수 있다.
                새로운 Exception객체를 만들어 던지기 위해서는 throw new MyException();
                와 같이 C#의 new를 사용하여 새로운 Exception 객체를 만든 후, 이 객체를 던지면 된다.
                만약 기존에 이미 Exception 객체가 있는 경우는 throw ex; 와 같이 기존 객체를 사용할 수 있다.

                throw 문 다음에 아무것도 없는 경우
                throw; 와 같이 뒤에 어떠한 Exception 객체 없이 그냥 throw를 사용할 수 있는데,
                이는 catch문에서 잡힌 Exception을 그대로 상위 호출 함수에게 전달하는 일을 한다.
                즉, 에러를 발생시킨 하위 Call Stack 모두를 상위 호출 함수에 전달하려면 이렇게 throw; 만을 호출한다.

                이와는 대조적으로 throw 뒤에 Exception 객체를 둔 경우는 기존의 하위 콜스택 정보를 잃어 버리고
                현재 메서드로부터 시작되는 콜스택을 만들어 보내는 효과가 있어서 디버깅에 필요한 정보를 잃을 수 있다.

                아래 예제는 IndexOutOfRangeException가 발생한 경우 MyException이라는 사용자 정의 Exception객체를 만들어 던지게 하고,
                그 외 모든 Exception의 경우는 발생한 Exception을 그대로 상위 호출 함수에게 전달하는 예이다.
            */
            {
                try
                {
                    //...
                }
                catch (IndexOutOfRangeException ex)
                {
                    // 새로운 Exception 생성하여 throw
                    throw new MyException("Invalid array index", ex);
                }
                catch (FileNotFoundException ex)
                {
                    bool success = false;
                    if (!success)
                    {
                        // 기존 Exception을 throw
                        throw ex;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    // 발생된 Exception을 그대로 호출자에 전달
                    throw;
                }

                Console.ReadLine();
            }
        }

		static void try_catch_finally_dispose()
		{
			/*
				try catch finally 구문을 활용하여 Exception 발생시 Dispose 를 호출 할 수 있다.
            */
			{
				StreamReader streamReader = null;
				try
				{
					streamReader = new StreamReader("file1.txt");
				}
				catch (FileNotFoundException)
				{
					Console.WriteLine("The file cannot be found.");
				}
				catch (IOException)
				{
					Console.WriteLine("An I/O error has occurred.");
				}
				catch (OutOfMemoryException)
				{
					Console.WriteLine("There is insufficient memory to read the file.");
				}
				finally
				{
					streamReader?.Dispose();
				}
			}
		}

		static void example()
        {
            /*
                아래 예제는 SQL Server에 연결하여 데이타베이스 내의 테이블,뷰 등의 개체 수를 가져온 후
                이를 화면에 뿌리는 코드이다.
                만약 SQLException 타입의 에러가 발생하면 catch 블럭에서 잡아서 에러 메시지만
                콘솔에 표시하고 Exception을 삼키게 된다.
                finally 블럭은 SqlConnection의 Close() 메서드를 실행하여 Connection을 닫는다.
                물론 에러가 발생하지 않더라도 finally 블럭은 실행되며,
                따라서 SQL Connection은 항상 닫히게 된다.
            */
            {
                string connStr = "Data Source=(local);Integrated Security=true;";
                string sql = "SELECT COUNT(1) FROM sys.objects";
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(connStr);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    object count = cmd.ExecuteScalar();
                    Console.WriteLine(count);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (conn != null &&
                        conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

                Console.ReadLine();
            }
        }


        public static void Test()
        {
			//example();

			//try_catch_finally_dispose();

			//throw_what();

			//try_catch_finally_what();

			//exception_what();
		}
	}
}
