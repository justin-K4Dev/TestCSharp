using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;



namespace AdvancedStep.Dump
{
    public class MiniDump
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
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000
        }

        static void createDump(string dumpPath)
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

            if (!System.IO.File.Exists(dumpPath))
            {
                throw new FileNotFoundException("Dump file was not created.", dumpPath);
            }
        }

        static void writeDumpByMiniDumpWriteDump()
        {
            /*
                📚 .NET Framework 4.8 - MiniDumpWriteDump를 이용한 Dump 파일 생성 테스트

                  1. 개요
                    - .NET Framework 4.8 애플리케이션에서 Windows Native API인 MiniDumpWriteDump를 호출하여 Dump 파일을 생성한다.
                    - .NET Framework 4.8은 Windows 전용이므로 Linux, macOS에서는 사용할 수 없다.
                    - 애플리케이션 Crash, Hang, Deadlock, Memory Leak 분석용 Dump를 생성할 때 사용한다.

                  2. 기본 개념
                    - DbgHelp.dll의 MiniDumpWriteDump API를 P/Invoke로 호출한다.
                    - 현재 프로세스 핸들과 PID를 전달한다.
                    - FileStream으로 생성한 파일 핸들에 Dump 데이터를 기록한다.

                  3. 핵심 특징
                    - .NET Framework 4.8에서 사용 가능한 대표적인 Dump 생성 방식이다.
                    - 외부 도구 없이 코드 내부에서 Dump를 만들 수 있다.
                    - Windows Native API 기반이므로 WinDbg, Visual Studio에서 분석 가능하다.

                  4. 실행 흐름
                    - Dump 저장 폴더 생성
                    - 현재 프로세스 정보 가져오기
                    - Dump 파일 생성
                    - MiniDumpWriteDump 호출
                    - 성공 여부 확인

                  5. 대표 메서드 또는 주요 코드
                    - createDump()
                      현재 프로세스의 Dump 파일을 생성한다.

                    - MiniDumpWriteDump()
                      Windows DbgHelp.dll에서 제공하는 Native Dump 생성 API이다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - 호출 시점의 모든 스레드 상태가 Dump에 포함된다.
                    - Deadlock, Thread Wait, Lock 점유 상태 분석에 사용할 수 있다.
                    - Full Memory Dump를 생성하면 각 스레드의 Stack과 Heap 메모리 분석이 가능하다.

                  7. 주의점
                    - Windows 전용이다.
                    - Dump 파일 크기가 클 수 있다.
                    - Dump 생성 중 프로세스가 잠시 멈춘 것처럼 보일 수 있다.
                    - 운영 환경에서는 Dump 파일에 개인정보, 토큰, 비밀번호 등이 포함될 수 있으므로 보안 관리가 필요하다.
                    - 32bit 프로세스는 32bit 분석 도구, 64bit 프로세스는 64bit 분석 도구 사용을 권장한다.

                  8. 예상 결과
                    - 바탕화면의 NetFx48DumpTest 폴더에 .dmp 파일이 생성된다.
            */
            {
                string outputDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "MiniDumpTest"
                );

                Directory.CreateDirectory(outputDir);

                int pid = Process.GetCurrentProcess().Id;
                string dumpPath = Path.Combine(outputDir, "MiniDump_" + pid + ".dmp");

                createDump(dumpPath);

                Console.WriteLine("Dump created:");
                Console.WriteLine(dumpPath);

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            writeDumpByMiniDumpWriteDump();
        }
    }
}
