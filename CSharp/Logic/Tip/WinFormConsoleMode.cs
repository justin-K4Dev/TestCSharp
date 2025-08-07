using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Tip
{
    public class WinFormConsoleMode
    {
        static void console_with_WinForm()
        {
            /*
                때때로 윈폼 프로그램에서 콘솔 실행 모드를 지원하고 싶은 경우가 있다.
                예를 들어, 윈폼 UI에서 여러 옵션이 설정된 후 어떤 Task를 실행한다고 했을때,
                이러한 옵션들을 Configuration 파일에 저장한 후 이를 가지고 콘솔 모드에서 실행하고 싶을 때가 있다.
                VS 프로젝트에서 콘솔과 윈폼을 동시에 선택할 수 없으므로, 둘을 동시에 구현하기 위해서는
                    (1)콘솔 프로젝트에서 윈폼을 띄우거나,
                    (2)윈폼 프로젝트에서 콘솔모드를 추가하는 방법 밖에 없다.

                콘솔 프로젝트에서 윈폼을 추가하게 되면 콘솔 창이 먼저 뜨고 윈폼 UI 프로그램이 띄워지게 된다.
                처음 띄워진 콘솔 창을 없앨수는 있지만, 사용자 입장에서 보면 콘솔창이 나타났다 사라지는 것을 목격하게 된다.
                두번째 방식인 윈폼에서 콘솔모드를 추가하는 방식은 콘솔 사용시만 Win32 Console API를 사용하여
                콘솔을 새로 생성하거나 기존의 콘솔에 Attach하여 사용하는 방식이다.
                일반적으로 기존 콘솔에 Attach하는 방식이 선호되는데, 아래 코드는 이를 구현한 예제이다.
                코드의 내용은 사용자가 콘솔에서 Argument를 하나 사용한 경우, AttachConsole() API를 사용하여
                해당 프로그램이 기존 콘솔을 사용하도록 한다.
                Attach 이후에 Console 에 출력이 가능하며, 콘솔이 더이상 필요 없을 시에 FreeConsole() API를 호출한다.
                한가지 문제점은 FreeConsole() 호출시에 바로 리턴하면 콘솔에 Prompt 를 볼 수 없다는 것이다.
                이를 해결하는 Workaround 로서 예제에서는 SendKeys.SendWait()를 사용하여 ENTER 키를 콘솔로 보냈다.
                이 두번째 방식의 큰 단점은 콘솔에서 출력 Redirect가 잘 동작하지 않는다는 것이다.
                윈폼과 콘솔 모드는 완벽하게 동시에 지원하기에는 어려움이 있다.

                    [STAThread]
                    static void Main(string[] args)
                    {
                        if (args.Length == 1)
                        {
                            // 현재 콘솔에 Attach
                            AttachConsole(ATTACH_PARENT_PROCESS);
        
                            Console.WriteLine("구성파일 " + args[0]);
                            RunWithConfig(args[0]);  // 어떤 작업 실행
        
                            // 콘솔 Attach 해제
                            FreeConsole();

                            // Enter 보내기
                            SendKeys.SendWait("{Enter}");        
                        }
                        else
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new Form1());
                        }
                    }

                    // ** P/Invoke로 Console API 사용 **
                    // 새 콘솔 생성
                    [DllImport("kernel32.dll")]
                    static extern Boolean AllocConsole();

                    // 기존 콘솔에 Attach
                    [DllImport( "kernel32.dll" )]
                    static extern bool AttachConsole( int dwProcessId );

                    // 콘솔 해제
                    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
                    static extern bool FreeConsole();

                    // 부모 프로세스에 Attach 하는 옵션
                    const int ATTACH_PARENT_PROCESS = -1;
            */
            {
                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //console_with_WinForm();
        }
    }
}
