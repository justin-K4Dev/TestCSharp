using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;


public class MemoryPool
{
    static void MemoryPool_what()
    {
        /*
            MemoryPool<T>는 T 타입에 대한 슬라이스 가능하고 고성능인 메모리 풀 인터페이스 클래스입니다.
            "Primitive 또는 구조체(value type)"에 특화된 메모리 풀 입니다.
            ❌ class 타입(T 참조형)에 사용하는 건 부적절


            ✅ 주요 특징
            분류	                    특징	                        설명
            ✅ 구조적	                추상 클래스	                    MemoryPool<T>는 직접 인스턴스화할 수 없으며, Shared 또는 상속 구현체를 통해 사용
            ✅ 사용성	                버퍼 대여/반납 패턴	            Rent(size)로 메모리 대여, Dispose()로 반납 (IMemoryOwner<T> 기반)
            ✅ 안전성	                명시적 수명 관리	            IDisposable 기반으로 명확한 리소스 관리 가능 (누수 방지)
            ✅ 성능	                    GC-Free 재사용 메모리	        힙 할당 최소화 → GC Pressure 감소, 고성능 유지
            ✅ 메모리 표현	            Memory<T> 기반	                Memory<T>와 Span<T>를 통해 안전하고 빠르게 메모리 조작 가능
            ✅ 비동기 안전	            Async/await 호환 가능	        Memory<T>는 힙에 존재하므로 비동기 환경에서도 안전하게 사용 가능 (Span<T>는 불가)
            ✅ 범용성	                타입 제네릭 지원	            byte, char, float, struct 등 어떤 타입이든 메모리 풀 사용 가능
            ✅ 표준화	                공용 풀 제공 (Shared)	        대부분의 상황에서 적절한 기본 구현 제공 (Thread-safe, 확장 가능)
            ✅ 커스터마이징	            사용자 정의 풀 구현 가능	    MemoryPool<T>를 상속하여 사용자 정의 풀 구성 가능 (고정 크기 풀 등)
            ✅ 최대 크기 제한	        MaxBufferSize 제공	            풀에서 대여할 수 있는 최대 버퍼 크기 확인 가능


            ✅ 왜 필요한가?
            ArrayPool<T>는 배열만 다루고 단순히 byte[]를 공유하지만,
            MemoryPool<T>는 Memory<T> 및 IMemoryOwner<T>를 활용해 보다 안전하고 구조화된 메모리 관리를 지원합니다.


            ✅ 특히 Span<T>, Memory<T> 기반 API에 사용되며, 비동기 환경에서도 안정적


            ✅ 핵심 개념
            개념	            설명
            MemoryPool<T>	    메모리 버퍼를 관리하는 추상 클래스
            IMemoryOwner<T>	    Memory<T>를 소유하는 disposable 객체 (빌리고 반납)
            Rent(int size)	    최소 크기 이상의 메모리 대여 (Memory<T> 반환)
            Dispose()	        사용이 끝난 메모리 버퍼를 명시적으로 해제, 미호출시 👉 Memory<T> 참조가 없다면 나중에 GC가 회수


            ✅ IMemoryOwner<T>가 없으면?
            ❌ 문제점
            Memory<T>만 있으면 수명 관리가 모호
            실수로 여러 곳에서 공유 → 메모리 중복 사용, GC pressure ↑
            GC에 전적으로 의존 → 실시간 시스템에서는 예측 불가 성능


            ✅ 기술적으로 중요한 설계 포인트
            항목	                설명
            IMemoryOwner<T>	        메모리 소유권을 관리하는 인터페이스. 버퍼의 수명과 반환 시점을 정확히 제어 가능
            버퍼 범위 보호	        Memory<T>는 Span<T>보다 안전한 슬라이스 구조로, 외부에서 범위를 침범하지 않음
            풀 구현 선택 가능	    Shared 외에도 고정 크기, zero-GC 풀 등 구현 가능
            GC Pressure 최소화	    ArrayPool보다도 더 구조화된 버퍼 재활용 가능


            ✅ MemoryPool<T> vs ArrayPool<T>
            항목	            MemoryPool<T>	                    ArrayPool<T>
            반환 타입	        IMemoryOwner<T> → Memory<T>	    T[] (배열)
            메모리 구조	        안전한 슬라이스 (Memory<T>)	        배열 전체 접근
            비동기 지원	        ✅ (Memory<T>는 async-safe)	        ❌ (Span<T>는 ref struct)
            소유권 관리	        명확 (Dispose())	                약함 (동시 접근 시 주의 필요)
            API 안정성	        더 정교하고 안전함	                더 단순하지만 잠재적 위험 존재


            ✅ 어떤 상황에 적합한가?
            시나리오	                        이유
            네트워크 통신 (Socket)	            버퍼 재사용 + async-safe
            Stream 처리 (PipeReader 등)	        슬라이스 기반 메모리 처리에 최적
            GC를 피해야 하는 고성능 환경	    반복적인 메모리 할당 없이 안정적 사용
            데이터 인코딩/디코딩	            Span<T>, Memory<T> 기반 빠른 처리 가능


            ┌─────────┐
            │  MemoryPool<T>   │
            └─────┬───┘
                   │ Rent()
                   ▼
            ┌─────────────┐
            │ IMemoryOwner<T> (512B)   │──→ Disposable, 버퍼 포함
            └─────────────┘
                   │
                   ▼
             Memory<T> → Span<T> → 처리
        */

        {
            using var owner = MemoryPool<byte>.Shared.Rent(1024); // 최소 1024 byte
            Memory<byte> memory = owner.Memory;

            // Span<byte>로 변환하여 사용
            var span = memory.Span;
            for (int i = 0; i < 10; i++)
                span[i] = (byte)(i + 1);

            Console.WriteLine(string.Join(", ", span[..10].ToArray()));

            owner.Dispose(); // 안전한 메모리 반환
        }
    }

    static void use_MemoryPool()
    {
        // ✅ using으로 자동 반환
        {
            for (int i = 0; i < 10000; i++)
            {
                using var owner = MemoryPool<byte>.Shared.Rent(1024);
                // 작업 수행 후 자동 반환
            }
        }

        // ✅ async 환경
        {
            IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(1024);
            try
            {
                // 사용 : await stream.ReadAsync(owner.Memory);
            }
            finally
            {
                owner.Dispose();  // ❗await 이후에도 반드시 반환 보장
            }
        }
    }


    public static void Test()
    {
        //use_MemoryPool();

        //MemoryPool_what();
    }
}
