using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep.Dump;

public class CreateDump
{
    static void writeDumpBy_createdump()
    {
        /*
            📚 createdump를 이용한 Dump 파일 생성 테스트

              1. 개요
                - .NET 런타임에서 제공하는 createdump 도구를 외부 프로세스로 실행하여 Dump 파일을 생성한다.
                - Windows, Linux, macOS에서 사용 가능하다.
                - 단, createdump 실행 파일이 PATH에 있거나 정확한 경로를 알고 있어야 한다.
                - 운영 중인 .NET 프로세스의 장애 분석, 메모리 분석, Hang 분석 등에 사용한다.

              2. 기본 개념
                - ProcessStartInfo를 사용해 createdump 명령을 실행한다.
                - 대상 PID를 전달하면 해당 프로세스의 Dump를 파일로 출력한다.
                - Dump 종류는 --full, --normal, --triage 등을 사용할 수 있다.

              3. 핵심 특징
                - 외부 실행 파일 기반이다.
                - 구현이 단순하다.
                - C# 코드에서 직접 Dump를 만드는 것이 아니라 createdump 도구에 위임한다.
                - PATH 문제, 권한 문제, OS별 실행 파일 위치 문제를 고려해야 한다.

              4. 실행 흐름
                - 현재 프로세스 PID 확인
                - Dump 출력 경로 생성
                - createdump 실행
                - 종료 코드 확인
                - Dump 파일 생성 여부 확인

              5. 대표 메서드 또는 주요 코드
                - CreateDumpByCreatedump()
                  createdump 명령을 실행하여 Dump 파일을 생성한다.

              6. 멀티 스레드 환경에서 작동 특징
                - createdump는 대상 프로세스의 전체 스레드 상태를 수집한다.
                - Dump 생성 시점의 managed/native thread call stack이 포함된다.
                - Full Dump의 경우 메모리까지 포함하므로 생성 중 프로세스가 일시적으로 지연될 수 있다.

              7. 주의점
                - createdump가 PATH에 없으면 실행 실패한다.
                - 대상 프로세스 접근 권한이 없으면 실패한다.
                - Full Dump는 파일 크기가 매우 클 수 있다.
                - 운영 환경에서는 Dump 저장 경로의 디스크 용량을 확인해야 한다.

              8. 예상 결과
                - Desktop/ApiDumpTest/createdump_{pid}.dmp 파일이 생성된다.
        */
        {
            int pid = Environment.ProcessId;

            string outputDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "CreateDumpTest"
            );

            Directory.CreateDirectory(outputDir);

            string dumpPath = Path.Combine(outputDir, $"CreateDump_{pid}.dmp");

            createDumpByCreatedump(pid, dumpPath);

            Console.WriteLine($"Dump created: {dumpPath}");
        }
    }

    static void createDumpByCreatedump(int pid, string dumpPath)
    {
        string fileName = OperatingSystem.IsWindows()
            ? "createdump.exe"
            : "createdump";

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = $"--full --name \"{dumpPath}\" {pid}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using Process? process = Process.Start(startInfo);

        if (process == null)
            throw new InvalidOperationException("createdump process start failed.");

        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception(
                $"createdump failed. ExitCode={process.ExitCode}\nSTDOUT:\n{stdout}\nSTDERR:\n{stderr}"
            );
        }

        if (!File.Exists(dumpPath))
            throw new FileNotFoundException("Dump file was not created.", dumpPath);
    }

    public static void Test()
    {
        writeDumpBy_createdump();
    }
}
