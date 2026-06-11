using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Diagnostics.NETCore.Client;


namespace AdvancedStep.Dump;

public class DiagnosticsClient
{
    static void writeDumpBy_DiagnosticsClient()
    {
        /*
            📚 Microsoft.Diagnostics.NETCore.Client를 이용한 Dump 파일 생성 테스트

              1. 개요
                - Microsoft.Diagnostics.NETCore.Client의 DiagnosticsClient를 사용하여 .NET 프로세스의 Dump를 생성한다.
                - Windows, Linux, macOS에서 사용 가능하다.
                - 대상 프로세스는 .NET 프로세스여야 한다.
                - C# 코드 내부에서 Dump 생성을 제어하고 싶을 때 사용한다.

              2. 기본 개념
                - DiagnosticsClient 객체를 PID로 생성한다.
                - WriteDump() 메서드를 호출해 Dump 파일을 생성한다.
                - DumpType.Full, Heap, Mini, Triage 등을 선택할 수 있다.

              3. 핵심 특징
                - 외부 createdump 명령을 직접 실행하지 않는다.
                - C# API로 Dump 생성 요청을 보낼 수 있다.
                - 테스트 코드, 진단 도구, 서비스 내부 진단 기능에 적합하다.
                - 내부적으로 .NET diagnostics IPC 채널을 사용한다.

              4. 실행 흐름
                - 현재 프로세스 PID 확인
                - DiagnosticsClient 생성
                - WriteDump 호출
                - Dump 파일 생성 확인

              5. 대표 메서드 또는 주요 코드
                - CreateDumpByDiagnosticsClient()
                  DiagnosticsClient.WriteDump()를 호출하여 Dump 파일을 생성한다.

              6. 멀티 스레드 환경에서 작동 특징
                - Dump 생성 시점의 모든 managed thread 상태를 수집한다.
                - 각 스레드의 call stack, lock 상태, heap 상태 등을 분석할 수 있다.
                - Full Dump 또는 Heap Dump는 프로세스에 일시적인 부하를 줄 수 있다.

              7. 주의점
                - 대상 프로세스가 .NET 프로세스가 아니면 사용할 수 없다.
                - 권한이 부족하면 실패할 수 있다.
                - Self dump, 즉 자기 자신의 Dump 생성은 가능하지만 Dump 생성 중 애플리케이션이 잠깐 느려질 수 있다.
                - Full Dump는 파일 크기가 매우 클 수 있다.

              8. 예상 결과
                - Desktop/ApiDumpTest/diagnostics_client_{pid}.dmp 파일이 생성된다.
        */
        {
            int pid = Environment.ProcessId;

            string outputDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "DiagnosticsDumpTest"
            );

            Directory.CreateDirectory(outputDir);

            string dumpPath = Path.Combine(outputDir, $"DiagnosticsClient_{pid}.dmp");

            createDumpByDiagnosticsClient(pid, dumpPath);

            Console.WriteLine($"Dump created: {dumpPath}");
        }
    }

    static void createDumpByDiagnosticsClient(int pid, string dumpPath)
    {
        var client = new Microsoft.Diagnostics.NETCore.Client.DiagnosticsClient(pid);

        client.WriteDump(
            DumpType.Full,
            dumpPath,
            logDumpGeneration: true
        );

        if (!File.Exists(dumpPath))
            throw new FileNotFoundException("Dump file was not created.", dumpPath);
    }

    public static void Test()
    {
        writeDumpBy_DiagnosticsClient();
    }
}
