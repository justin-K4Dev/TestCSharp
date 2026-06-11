using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Win32.SafeHandles;


namespace AdvancedStep.Dump
{
    public class UnhandleException
    {
        [DllImport("Dbghelp.dll", SetLastError = true)]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            int processId,
            SafeFileHandle hFile,
            MiniDumpType dumpType,
            IntPtr exceptionParam,
            IntPtr userStreamParam,
            IntPtr callbackParam
        );

        [Flags]
        private enum MiniDumpType
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpWithThreadInfo = 0x00001000
        }

        private static void createDump(string dumpPath)
        {
            using (Process process = Process.GetCurrentProcess())
            using (FileStream fileStream = new FileStream(
                dumpPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            {
                MiniDumpWriteDump(
                    process.Handle,
                    process.Id,
                    fileStream.SafeFileHandle,
                    MiniDumpType.MiniDumpWithFullMemory |
                    MiniDumpType.MiniDumpWithHandleData |
                    MiniDumpType.MiniDumpWithThreadInfo,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero
                );
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string outputDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "UnhandleExceptionDumpTest"
                );

                Directory.CreateDirectory(outputDir);

                int pid = Process.GetCurrentProcess().Id;
                string dumpPath = Path.Combine(outputDir, "UnhandleException_" + pid + ".dmp");

                createDump(dumpPath);

                Console.WriteLine("Crash dump created:");
                Console.WriteLine(dumpPath);
            }
            catch
            {
                // Crash 상황에서는 추가 예외를 외부로 던지지 않는 것이 안전하다.
            }
        }

        static void writeDumpByUnhandledException()
        {
            /*
                📚 .NET Framework 4.8 - UnhandledException 발생 시 Dump 생성 테스트

                  1. 개요
                    - 처리되지 않은 예외가 발생했을 때 자동으로 Dump 파일을 생성한다.
                    - Crash 원인 분석용으로 사용한다.

                  2. 기본 개념
                    - AppDomain.CurrentDomain.UnhandledException 이벤트를 등록한다.
                    - 예외가 처리되지 않고 프로세스가 종료되기 전에 Dump를 생성한다.

                  3. 핵심 특징
                    - 실제 장애 발생 시점의 상태를 Dump로 남길 수 있다.
                    - 운영 환경의 Crash 분석에 유용하다.

                  4. 실행 흐름
                    - UnhandledException 이벤트 등록
                    - 의도적으로 예외 발생
                    - 이벤트 핸들러에서 Dump 생성
                    - 프로세스 종료

                  5. 대표 메서드 또는 주요 코드
                    - OnUnhandledException()
                      처리되지 않은 예외 발생 시 Dump를 생성한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - 어떤 스레드에서 처리되지 않은 예외가 발생하더라도 AppDomain 이벤트로 전달된다.
                    - Dump에는 예외 발생 시점의 다른 스레드 상태도 함께 포함된다.

                  7. 주의점
                    - 프로세스가 이미 불안정한 상태일 수 있으므로 복잡한 로직은 피하는 것이 좋다.
                    - Dump 저장 실패를 대비해 최소한의 코드만 실행하는 것이 좋다.

                  8. 예상 결과
                    - 예외 발생 직후 crash_{pid}.dmp 파일이 생성된다.
            */
            {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                Console.WriteLine("Press Enter to throw test exception.");
                Console.ReadLine();

                throw new InvalidOperationException("Test crash for dump generation.");

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            writeDumpByUnhandledException();
        }
    }

}
