using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;

public class CLRMemory
{
    struct MyStruct
    {
        public int X;
        public int Y;
    }

    class MyClass
    {
        public int A;
        public int B;
    }

    class UnmanagedResource : IDisposable
    {
        private IntPtr unmanagedPtr;
        private bool disposed = false;

        public UnmanagedResource()
        {
            // ✔️ [6] Unmanaged Heap (GC가 관리하지 않음)
            unmanagedPtr = Marshal.AllocHGlobal(1024);
            Console.WriteLine("Unmanaged memory allocated.");
        }

        ~UnmanagedResource()
        {
            // ✔️ [11] Finalizer Queue에서 호출됨 (GC Root에서 제거된 후)
            Console.WriteLine("Finalizer called.");
            FreeUnmanaged();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                FreeUnmanaged();

                // 특정 객체의 소멸자 실행 자체를 방지
                // 개별 객체 단위 호출
                // 주로 Dispose 패턴에서 사용
                GC.SuppressFinalize(this); // ✔️ Finalizer 호출 방지
                disposed = true;
            }
        }

        private void FreeUnmanaged()
        {
            if (unmanagedPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanagedPtr); // ✔️ Unmanaged Heap 수동 해제
                unmanagedPtr = IntPtr.Zero;
                Console.WriteLine("Unmanaged memory freed.");
            }
        }
    }

    static void CLRMemory_DotNet5Plus_what()
    {
        // 👉 .NET Framework (예: 4.8 이하) 와 .NET 5 이상 (.NET Core 기반) 에서는 GC(가비지 컬렉션) 및 Heap 관리 방식에 차이가 있다 !!!

        /*
            .NET 5 이상에서는 .NET Framework와 달리 CoreCLR 기반의 통합 런타임 (.NET Unified Runtime)을 사용하며,
            메모리 관리 방식이 더욱 세밀하고 성능 위주로 최적화되었습니다.

            ▶️ 주요 메모리 구조 구성도 :

            ✅ 프로세스 가상 메모리  ← 운영체제가 프로세스마다 생성              
            ├── [Kernel 영역]      ← OS 내부 커널 전용
            ├── [User 영역]        ← 사용자 프로세스 메모리 공간
            │
            │   ├── CLR 런타임 영역 (Common Language Runtime)  ← .NET 런타임
            │   │   ├── ✅ Managed 영역 (GC 관리 대상)
            │   │   │   ├── Managed Heap
            │   │   │   │   ├── SOH (Small Object Heap - Gen 0 ~ Gen 2)
            │   │   │   │   ├── LOH (Large Object Heap - Gen 2 포함)
            │   │   │   │   └── Pinned Object Heap (POH: .NET 5+에서 도입된 고정 객체 영역)
            │   │   │   ├── Finalization Queue (소멸자 대기 큐)
            │   │   │   └── Type Metadata / Reflection Info
            │   │   │
            │   │   ├── ⚙️ CLR 내부 Unmanaged 영역 (GC 비관리, Native 구조)
            │   │   │   ├── JIT Code Heap (IL → Native 코드 캐시)
            │   │   │   ├── Method Table / VTable (타입 구조)
            │   │   │   ├── Interop Layer (P/Invoke, COM 지원 등)
            │   │   │   └── Execution Engine (보안, 예외, JIT, GC 포함)
            │   │
            │   ├── Native Heap (OS 제공, Marshal.AllocHGlobal 등 직접 할당)
            │   │   └── COM, C++ 라이브러리, P/Invoke 대상 등이 여기 존재
            │   │
            │   ├── Stack (각 Thread 마다 존재)          ← OS가 관리, CLR이 추적 (GC Root)
            │   │   ├── 값 형식 변수 (struct 등)
            │   │   └── 참조형 변수 (GC Root로 사용 가능)
            │   │
            │   └── Code 영역
            │       ├── IL 코드 영역
            │       └── JIT 컴파일된 Native 코드 (일부는 CLR 내부 Heap에도 캐싱)


            ✅ Pinned Object Heap (POH)이란?
            - .NET 5 이상에서 새롭게 도입된 GC 힙의 일종으로,
            - 고정(pinned)된 객체들이 할당되는 별도의 힙입니다.
            - Blittable 타입만 할당 가능 !!!
            - 일반적으로 고정 객체는 이동이 불가능하므로 SOH/LOH에서 단편화를 유발할 수 있는데,
              이를 방지하고 GC 효율을 높이기 위해 POH가 도입됨.
            - 예: GCHandle.Alloc(obj, GCHandleType.Pinned) 또는 고정된 버퍼가 여기에 할당됩니다.
            - POH는 Gen 2 컬렉션 시 함께 수집됩니다.


            🎯 Blittable 타입들의 조건 정리
            항목	                                    가능 여부
            ✅ Primitive 타입	                        int, float, double, byte, short, long, bool(1바이트) 등	
            ✅ Struct (값형)	                        내부 필드가 모두 blittable일 때만 가능	
            ✅ fixed buffer (unsafe struct)	            fixed byte[256] 같은 배열 포함 가능 (unsafe 필요)	
            ❌ string, object, class 포함 struct	    참조형 필드 포함 → blittable 불가	
            ❌ Nullable<T>	                            내부에 boxing 로직 필요 → blittable 아님	
            ❌ Auto-layout struct	                    필드 순서 보장 안됨 → Interop 불안정	
            ✅ [StructLayout(LayoutKind.Sequential)]	필드 순서 보장 → Interop 적합 → blittable 가능


            ▶️ 메모리 흐름 요약
            - 값 타입(struct): 스택에 직접 저장
            - 참조 타입(class): 참조는 스택, 실제 객체는 Managed Heap에 저장
            - 참조는 GC Root로 간주되어 객체의 생존 여부 판단 기준이 됨
            - 객체가 살아남으면 GC 수집 시 상위 세대로 승격됨 (Gen0 → Gen1 → Gen2)
            - POH는 pinned 객체 전용으로 단편화를 방지
            - LOH와 POH는 Gen 2 수집 시 함께 수거됨

            ✅ 메모리 할당 및 메모리 해제 흐름
              - .NET Framework 4.x와 동일 + POH 관리 추가
        */
        {
            // [1] 값 타입은 Stack에 직접 저장, [ Stack ]
            MyStruct localStruct = new MyStruct { X = 1, Y = 2 };

            // [2] 참조 타입은 Stack에 참조(포인터)가 저장되고, 실제 객체는 Managed Heap에 생성됨, [ Stack + Managed Heap ]
            MyClass localClass = new MyClass { A = 10, B = 20 };
            // (if Finalizer 존재) Finalizer Queue 등록 

            // [3] localClass는 Stack의 GC Root로 간주됨 → GC가 이 객체를 수집하지 않음, [ Stack (GC Root) ]
            Console.WriteLine($"Struct: {localStruct.X}, {localStruct.Y}");
            Console.WriteLine($"Class: {localClass.A}, {localClass.B}");

            // [4] Unmanaged 메모리 보유 객체 생성 → Stack에 obj 참조 저장됨, [ Stack + Managed Heap ]
            UnmanagedResource obj = new UnmanagedResource(); // 객체 자체는 Managed Heap

            // [5] obj는 GC Root이므로 아직 수집되지 않음, [ Stack + Unmanaged Heap ]
            Console.WriteLine("Before Dispose");

            // [6] 명시적으로 메모리 해제 + Finalizer 억제, [ Finalizer Queue 해제 ]
            obj.Dispose();

            // [7] obj 참조 제거 → GC Root에서 제외됨, [ GC 대상됨 ]
            obj = null;
            // 명시적 수거를 유도하고 싶다면 obj = null; 은 유효합니다.
            // 하지만 GC는 어차피 Scope 벗어나면 참조가 끊긴 것으로 판단하므로, 보통은 불필요합니다.


            // [8] POH 예시: 고정된 객체를 GCHandle로 핀 하면 POH에 할당됨 (.NET 5+)
            byte[] pinnedBuffer = new byte[1024];
            GCHandle handle = GCHandle.Alloc(pinnedBuffer, GCHandleType.Pinned);
            Console.WriteLine($"Pinned buffer address: {handle.AddrOfPinnedObject()}");

            handle.Free();

            // [9] GC 명시 호출 (전체 세대 수집), [ Managed Heap 전체 수집 (Gen 0, 1, 2 + LOH + POH) ]
            GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            // 이 설정은 모든 GC 대상 객체를 수집하고, 세대 승격 및 힙 압축까지 포함합니다.
            // Finalizer 실행 대상 객체가 있을 경우, GC.WaitForPendingFinalizers()로 마무리해줘야 안정적입니다.

            Console.WriteLine("After GC");

            // [10] 지역 변수(localStruct, localClass)는 Stack에 저장된 상태, [ Stack ]
            // GC는 지역 값 타입에는 관여하지 않음

            /*
                객체 생성
                    ↓
                Stack에 참조 저장
                    ↓
                (Optional) Finalizer 존재 시 → 생성 시 Finalization Queue 등록
                    ↓
                Dispose 호출 → Unmanaged 자원 해제 + Finalizer 억제 (GC.SuppressFinalize), Finalization Queue 에서 제거
                    ↓
                Stack에서 참조 제거 → GC Root에서 제외됨
                    ↓
                GCHandle.Alloc(..., GCHandleType.Pinned) 호출
                  → 객체가 POH(Pinned Object Heap)에 고정됨 (.NET 5+)
                  → GC가 이동시키지 않음, 수집 대상도 아님
                    ↓
                GCHandle.Free() 호출
                  → POH에서 고정 해제됨
                  → 이후 발생하는 GC에서 수집 가능 상태로 전환
                    ↓
                GC.Collect(2) → 전체 세대 수집 시작 (Gen 0~2 + LOH + POH)
                    ↓
                        Mark Phase → GC Root로부터 참조 가능한 객체 마킹 (생존 객체로 Making), (병렬 처리 가능)
                            ↓
                        Sweep Phase → 참조되지 않은 객체 식별 및 정리, (병렬 처리 가능)
                            ↓
                        Compact Phase → 생존 객체 힙 상에서 재배치 (압축), Gen 0~2만 대상, LOH/POH는 non-compacting, (병렬 처리 가능)
                            ↓
                        Finalization Queue에 있던 참조 해제된 객체는 F-reachable Queue로 이동
                    ↓
                Finalizer Thread가 F-reachable 객체의 소멸자(~ClassName) 실행
                    ↓
                다음 GC 사이클에서 해당 객체 메모리 실제 해제 (Reclamation)
                    ↓
                메서드 종료 시 Stack 프레임 해제
                  → 남은 참조 객체는 GC 대상이 됨
            */
        }
    }

    static void CLRMemory_DotNet5Plus_ImprovedGC()
    {
        /*
            ✅ 1. GC 모드 자동 설정 최적화 (특히 ASP.NET Core 호스팅 시)
              - ASP.NET Core + Kestrel + .NET 5~8 환경에서는 기본적으로 Server GC가 자동 적용됨
              - UseServerGC 설정을 생략해도 적절한 환경이면 Server GC가 활성화됨
              🟢 .NET 5+는 환경 감지 기반으로 자동 설정이 더 똑똑해짐

            ✅ 2. Background GC 향상 (Server GC 포함)
              - .NET 5+에서 Server GC에서도 Background GC가 더 자주, 더 짧게, 더 병렬적으로 실행되도록 개선
              - 전체 정지 시간이 감소 → GC pause time 줄어듦
              - Gen2 Full GC의 영향을 줄이는 구조

            ✅ 3. SOH/LOH 힙 세분화 (Segmented heap) 구조 개선
              - Server GC 환경에서도 Large Object Heap (LOH) 및 Pinned Object Heap (POH) 분리 지원 (.NET 5+)
              - Workstation GC도 세대 분리 구조는 동일하게 유지되며, GC 알고리즘은 동일

            ✅ 4. Pinned Object Heap(POH) 도입 (.NET 5)
              - Server GC에서도 pinned 객체로 인한 힙 단편화 부담을 줄이기 위해 POH가 도입됨
              - POH는 Server/Workstation GC 모두에서 작동하며, 메모리 고정이 GC 전체 성능에 미치는 영향을 완화
           
            ✅ 5. GC Metrics 노출 강화 (.NET 6~8)
              - System.Diagnostics.Metrics, EventCounters, dotnet-counters, dotnet-trace 등을 통해
                Server GC와 Workstation GC 각각의 힙 사용량, 스레드 수, GC 시간을 세밀하게 측정 가능

            ✅ 6. ThreadPool 및 GC 간 상호작용 최적화
              - .NET 6+에서는 GC 활동과 ThreadPool 스케줄링 간의 충돌을 최소화하여
                GC 중에도 Task가 병렬로 실행되는 구조가 개선됨
              - Server GC 환경에서 짧은 GC 주기 + 빠른 Task 스케줄링 조합이 훨씬 잘 작동
        */
    }

    static void CLRMemory_DotNet5Plus_WorkstationGCMode()
    {
        /*
            Workstation GC 설정

            ✅ 빠른 응답성과 짧은 GC 중단 시간이 중요한 클라이언트 앱에 적합
              - 예: WPF, WinForms, 데스크탑 도구, 실시간 UI 앱
            
            📁 runtimeconfig.template.json

            {
              "runtimeOptions": {
                "configProperties": {
                  "System.GC.Server": false,                // ❗ Workstation GC 사용
                  "System.GC.Concurrent": true,             // Background GC 활성화 (UI 응답성 향상)
                  "System.GC.RetainVM": false,              // GC 후 메모리 해제 (메모리 절약)
                  "System.GC.AllowVeryLargeObjects": false  // 일반 앱에서 대형 배열은 제한
                }
              }
            }
        */
    }

    static void CLRMemory_DotNet5Plus_ServerGCMode()
    {
        /*
            Server GC 설정

            ✅ 서버 응답성보다는 Throughput(처리량) 중시 환경에 적합
              - 예: ASP.NET Core, 게임 서버, 고부하 API 서버
            
            📁 runtimeconfig.template.json

            {
              "runtimeOptions": {
                "configProperties": {
                  "System.GC.Server": true,                     // Server GC 사용
                  "System.GC.Concurrent": true,                 // Background GC 사용 (권장)
                  "System.GC.RetainVM": true,                   // GC 후에도 메모리 유지 (서버에서 메모리 재사용 기대 시 유용)
                  "System.GC.AllowVeryLargeObjects": true       // 2GB 초과 배열 허용 (.NET Core/5+ 전용)
                }
              }
            }
        */
    }

    public static void Test()
    {
        //CLRMemory_DotNet5Plus_ServerGCMode();

        //CLRMemory_DotNet5Plus_WorkstationGCMode();

        //CLRMemory_DotNet5Plus_ImprovedGC();

        //CLRMemory_DotNet5Plus_what();
    }
}
