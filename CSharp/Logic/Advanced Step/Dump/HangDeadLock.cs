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
    public class HangDeadLock
    {
        private static readonly object LockA = new object();
        private static readonly object LockB = new object();


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

        private static void StartDeadlock()
        {
            var threadA = new System.Threading.Thread(() =>
            {
                lock (LockA)
                {
                    System.Threading.Thread.Sleep(500);

                    lock (LockB)
                    {
                    }
                }
            });

            var threadB = new System.Threading.Thread(() =>
            {
                lock (LockB)
                {
                    System.Threading.Thread.Sleep(500);

                    lock (LockA)
                    {
                    }
                }
            });

            threadA.Start();
            threadB.Start();
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
                bool result = MiniDumpWriteDump(
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

                if (!result)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new InvalidOperationException(
                        "MiniDumpWriteDump failed. Win32Error=" + errorCode
                    );
                }
            }
        }

        static void writeDumpWhenHangDeadLock()
        {
            /*
                📚 .NET Framework 4.8 - Deadlock 상태 Dump 생성 테스트

                  1. 개요
                    - 의도적으로 Deadlock을 만든 뒤 Dump를 생성한다.
                    - 멀티 스레드 Lock 문제 분석 테스트에 사용한다.

                  2. 기본 개념
                    - 두 개의 lock 객체를 만든다.
                    - Thread A는 lock1 → lock2 순서로 진입한다.
                    - Thread B는 lock2 → lock1 순서로 진입한다.
                    - 서로 상대 lock을 기다리면서 Deadlock이 발생한다.

                  3. 핵심 특징
                    - Dump 분석 도구에서 Thread Wait 상태를 확인할 수 있다.
                    - WinDbg의 !syncblk, !threads, ~* kb 명령으로 분석 가능하다.

                  4. 실행 흐름
                    - Thread A 시작
                    - Thread B 시작
                    - Deadlock 발생
                    - Main Thread에서 Dump 생성

                  5. 대표 메서드 또는 주요 코드
                    - StartDeadlock()
                      Deadlock 상태를 만든다.

                    - CreateDump()
                      Deadlock 상태의 Dump를 생성한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - Thread A와 Thread B가 서로 다른 lock을 점유한 채 대기한다.
                    - Dump에는 두 스레드의 대기 상태와 lock 소유 정보가 포함된다.

                  7. 주의점
                    - 테스트용 코드이므로 운영 코드에 사용하면 안 된다.
                    - Deadlock 발생 후 프로세스는 정상 종료되지 않는다.

                  8. 예상 결과
                    - deadlock_{pid}.dmp 파일이 생성된다.
                    - WinDbg에서 어떤 스레드가 어떤 lock을 기다리는지 확인 가능하다.
            */
            {
                StartDeadlock();

                System.Threading.Thread.Sleep(3000);

                string outputDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "HandDeadLockTest"
                );

                Directory.CreateDirectory(outputDir);

                int pid = Process.GetCurrentProcess().Id;
                string dumpPath = Path.Combine(outputDir, "HangDeadlock_" + pid + ".dmp");

                createDump(dumpPath);

                Console.WriteLine("Deadlock dump created:");
                Console.WriteLine(dumpPath);

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //writeDumpWhenHangDeadLock();
        }
    }
}
