using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace Tip
{
    public class Trace
    {
        static void logging_with_Trace()
        {
            /*
                .NET에 내장된 Trace 클래스를 활용하여 간단한 로그를 콘솔이나 파일 혹은 이벤트 로그에 남길 수 있다.
                로그를 어디에 쓸 것인가를 정하기 위해 Trace Listener 를 지정하면 되는데,
                .NET에서 기본적으로 몇가지의 Trace Listener들을 제공하고 있다.
                (주: 이러한 Trace Listener들은 Trace 클래스와 Debug 클래스 모두에서 사용할 수 있다)

                DefaultTraceListener는 디버그창(OutputDebugString)에 메시지를 보내는 것으로
                VS의 Output 창이나 DebugMon, DbgView 같은 도구로 로그를 볼 수 있다.

                TextWriterTraceListener는 TextWriter나 Stream으로 메시지를 보내는 것으로 파일에 로그를 남길 때 사용한다.
                ConsoleTraceListener는 콘솔에 메시지를 보낼 때 사용하며,
                EventLogTraceListener는 윈도우즈의 Event Log에 로그 메시지를 보낼 때 사용한다.

                Trace 클래스에는 여러가지 Static 출력 메서드들이 있는데,
                Trace.Write(), Trace.WriteLine() 등과 같은 기본 메서드들를 비롯하여 에러를 출력하는 TraceError(),
                경고를 출력하는 TraceWarning(), 그리고 Info를 출력하는 TraceInformation() 등을 사용할 수 있다.

                아래 예제는 콘솔과 파일 그리고 이벤트로그에 간단한 메시지를 로깅하는 코드이다.
            */
            {
                // 1. DefaultTraceListener 사용. VS Output 창에 로깅
                System.Diagnostics.Trace.WriteLine("Default Logging");

                // 2. 콘솔에 로깅
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
                System.Diagnostics.Trace.WriteLine("Console Log");

                // 3. 파일에 로깅
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener("Logs.txt"));
                System.Diagnostics.Trace.AutoFlush = true;
                System.Diagnostics.Trace.WriteLine("File Log");
                //Trace.Flush();  // AutoFlush 하지 않으면 수동으로 Flush 할 것

                // 4. EventLog에 로깅
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.Add(new EventLogTraceListener("Application"));
                System.Diagnostics.Trace.WriteLine("My Event Log");

                // 5. 콘솔과 파일에 동시 로깅
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
                System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener("Logs.txt"));
                System.Diagnostics.Trace.AutoFlush = true;
                // Trace*() 메서드들 사용
                System.Diagnostics.Trace.TraceInformation("My Info");
                System.Diagnostics.Trace.TraceWarning("My Warning");
                System.Diagnostics.Trace.TraceError("My Error");

                Console.ReadLine();
            }
        }


        static void logging_with_Trace_on_App_Config()
        {
            /*
                위와 같이 C#에서 Trace Listener들을 직접 지정하는 대신
                App.config 같은 구성파일에 사용할 Trace Listener들을 설정할 수도 있다.
                이렇게 하면 프로그램을 재컴파일하지 않고 쉽게 사용할 Trace Listener들을 변경할 수 있는 장점이 있다.

                구성파일을 사용하기 위해서는 아래 예제와 같이 system.diagnostics 노드 밑에 trace와 listeners 노드를 만들고
                그 밑에 사용하고자 하는 Trace Listener들을 추가하면 된다.
                아래는 콘솔과 파일 출력에 Trace 메시지를 출력하도록 App.config를 설정한 예이다.

                    <?xml version="1.0" encoding="utf-8" ?>
                    <configuration>
                        <system.diagnostics>
                            <trace autoflush="true">
                                <listeners>
                                    <add name="consoleListener" type="System.Diagnostics.ConsoleTraceListener" />
                                    <add name="fileListener" type="System.Diagnostics.TextWriterTraceListener" initializeData="Logs.txt" />                
                                </listeners>
                            </trace>
                        </system.diagnostics>
                    </configuration>
            */
            {
                Console.ReadLine();
            }
        }


        // TraceSource 생성
        private static TraceSource traceSource = new TraceSource("MyTrace");

        static void logging_with_TraceSource()
        {
            /*
                .NET의 Trace Listener가 메시지를 받는 쪽이라면, 메시지를 보내는 쪽은 Trace Source에 해당된다.
                .NET의 TraceSource 클래스는 로그 메시지를 보낼 수 있는 소스를 생성하고 사용할 수 있는 클래스이다.
                TraceSource 를 사용하기 위해서는 먼저 TraceSource 객체를 생성해야 하는데,
                생성시 Trace 소스명을 파라미터로 지정한다.
                아래 예제에선 MyTrace라는 이름의 TraceSource 객체를 생성하였다.

                TraceSource 객체를 생성하였으면, 이 객체의 메서드인 TraceEvent(), TraceInformation(), TraceData() 등을 사용하여
                로그 메시지를 출력할 수 있다.
                특히, 이 메서드들 중 TraceEvent() 혹은 TraceData()는 다양한 TraceEventType을 지정할 수 있는데,
                이벤트 타입으로는 Critical, Error, Warning, Information, Verbose, Start, Stop, Suspend, Resume, Transfer 등이 있다.

                TraceSource 에서 메시지를 보낼 때, 이를 받는 쪽 즉 Trace Listener을 또한 지정해 주어야 하는데,
                이는 TraceSource의 Listeners 속성에 직접 추가하거나 혹은 App.config 파일에 아래 예제 에서와 같이 Trace Listener들을 지정할 수 있다.

                아래 예제는 간단한 로그 메시지를 TraceSource를 사용하여 Logs.txt 파일에 저장하는 예이다.
            */
            {
                // TraceSource 사용
                traceSource.TraceEvent(TraceEventType.Start, 0, "Main Start");

                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine(i);
                    traceSource.TraceInformation("msg#" + i.ToString());
                }

                traceSource.TraceEvent(TraceEventType.Stop, 0, "Main End");
                traceSource.Flush();

                Console.ReadLine();

                /*
                    // App.config file
                    <?xml version="1.0" encoding="utf-8" ?>
                    <configuration>
                        <system.diagnostics>
                            <sources>
                                <source name="MyTrace" switchValue="All">
                                    <listeners>
                                        <add name="fileListener" type="System.Diagnostics.TextWriterTraceListener" initializeData="Logs.txt" />
                                    </listeners>
                                </source>
                            </sources>
                        </system.diagnostics>
                    </configuration>
                */
            }
        }


        public static void Test()
        {
            //logging_with_TraceSource();

            //logging_with_Trace_on_App_Config();

            //logging_with_Trace();
        }
    }
}
