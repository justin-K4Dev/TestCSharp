using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;



public class ArrayPool
{

    static void ArrayPool_what()
    {
        /*
            📄 ArrayPool<T>  
              - .NET에서 배열 재사용을 위한 고성능 메모리 풀(Pool) 기능을 제공하는 클래스입니다.
              - 불필요한 힙 할당과 GC 압력을 줄이기 위해, 자주 사용되는 배열을 미리 만들어 두고 재사용할 수 있게 해줍니다.


            ✅ 주요 특징
              | 특징	            | 설명
              |---------------------|-----------------------------------------------------
              | Shared 인스턴스	    | ArrayPool<T>.Shared는 .NET이 관리하는 전역 풀
              | Thread-safe	        | 멀티스레드 환경에서도 안전
              | 동적 할당 가능	    | 풀에 없으면 새 배열 할당 (단, 사용 후 반환 권장)
              | 초기화 선택 가능	| Return(clearArray: true) 설정 시 보안상 유용


            ✅ 왜 필요한가?
              - C#에서 배열을 자주 생성(new T[])하면 다음 문제가 발생합니다:
                + 매번 힙에 새로운 메모리 할당 → GC 대상 증가
                + 특히 짧은 시간에 많은 배열을 만들면 GC Pressure 심화
                  예: 네트워크 패킷 버퍼, 임시 정렬/필터 연산 등

            🔧 ArrayPool<T>를 사용하면 기존 배열을 재활용해서 힙 할당과 GC를 줄일 수 있습니다.


            ✅ 언제 사용하나?
              | 상황	                     | 이유
              |------------------------------|-----------------------------------------------------
              | 네트워크/소켓 서버	         | 패킷 버퍼 반복 생성 방지
              | 이미지 처리/압축/포맷 변환	 | 대용량 배열 반복 할당 제거
              | 문자열 조립/Stream 처리	     | 임시 버퍼 재사용
              | 고성능 게임 서버	         | stackalloc 대신 힙 버퍼 재사용 필요할 때
        */

        {
            // 1. 전역 풀에서 배열을 대여 (최소 크기 1024)
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024); // 최소 길이 이상의 배열을 빌림

            try
            {
                // 2. 버퍼 사용
                for (int i = 0; i < 10; i++)
                    buffer[i] = (byte)(i * 2);

                Console.WriteLine(string.Join(", ", buffer[..10]));
            }
            finally
            {
                // 3. 사용 후 반환 (가능하면 true로 Clear 요청)
                ArrayPool<byte>.Shared.Return(buffer, clearArray: true); // 배열을 풀로 반환, clearArray는 초기화 여부
            }
        }
    }


    static void use_ArrayPool()
    {
        // maxArrayLength: 풀에서 관리할 최대 배열 길이
        // maxArraysPerBucket: 같은 크기 그룹(bucket)에 유지할 배열 수
        {
            var customPool = ArrayPool<byte>.Create(maxArrayLength: 1024, maxArraysPerBucket: 50);
        }

        /*
            ✅ 사용 시 주의사항
              | 주의                                     | 설명
              |------------------------------------------|----------------------------------------------------------
              | 배열 내용이 재사용될 수 있음             | 반환 전에 민감 정보 삭제하거나 clearArray: true 설정
              | 반환된 배열은 다시 사용하지 말 것        | Double-use는 버그 원인
              | 너무 작은 배열 요청은 재사용 대상 아님   | 최소 한 버킷 이상 크기 요청 권장


            ✅ 요약: 풀 크기 초과 시 동작
              👉 ArrayPool<T>는 풀에 여유가 없으면 자동으로 새로운 배열을 new로 생성합니다.       
              - 예외(Exception)는 발생하지 않습니다.
              - 필요한 크기의 배열을 무조건 반환합니다.
              - 단지 풀 외부에서 새로 생성된 배열이기 때문에,
              - 해당 배열은 풀 내부 캐시로 관리되지 않을 수 있고,
              - 반환하지 않으면 GC 대상이 될 수 있습니다.


            ✅ 내부 동작 요약
              | 동작 조건	                             | 처리 방식
              |------------------------------------------|----------------------------------------------------------
              | 풀에 여유 배열 있음	                     | 기존 배열 반환 (빠르고 GC-free)
              | 풀에 없지만 요청 크기 정해짐	         | new T[size]로 새 배열 생성 (GC 대상이 됨)
              | 반환 시 버킷이 가득 참	                 | 배열을 버리고 GC 대상 처리 (다음 Rent에 재사용 안 함)

        */
        {
            var pool = ArrayPool<byte>.Create(1024, 2); // 최대 1024 byte 배열 2개까지만 유지

            byte[] a = pool.Rent(1024); // ✅ 버킷에서 할당
            byte[] b = pool.Rent(1024); // ✅ 버킷에서 할당
            byte[] c = pool.Rent(1024); // ✅ 풀에 없어서 new byte[1024] 할당됨 (GC 대상)

            Console.WriteLine($"a.Length: {a.Length}, b.Length: {b.Length}, c.Length: {c.Length}");

            pool.Return(a);
            pool.Return(b);
            pool.Return(c); // 자동으로 맞는 버킷에 다시 보관되거나 버려짐
        }
    }

    public static void Test()
    {
        //use_ArrayPool();

        //ArrayPool_what();
    }
}
