using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MultiThread;


public class ValueTask
{

    static ValueTask<int> ValueTask_what()
    {
        /*
            📄 ValueTask
              - .NET에서 비동기 작업을 나타내는 타입으로,
                Task보다 가볍고 효율적인 대안을 제공하는 값 타입(struct) 기반의 비동기 반환 타입입니다.


            🧠 도입 목적
              | 문제                                                         | 해결책 (ValueTask)
              |--------------------------------------------------------------|-----------------------------------------------------
              | Task는 항상 힙 할당 → GC 부담                               | ValueTask는 대부분 스택 할당됨
              | 자주 즉시 완료되는 비동기 메서드에 불필요한 Task 생성 발생   | ValueTask는 값 또는 실제 Task를 래핑할 수 있음

            ✅ 주요 특징
              | 특징                                | 설명
              |-------------------------------------|--------------------------------------------------------------
              | struct 기반 비동기 반환 타입        | GC 비용을 줄이기 위해 값 타입으로 설계됨
              | 1회 await만 가능                    | ValueTask는 재사용 불가 — await 2회 이상 시 예외 발생
              | Task와 유사한 API                   | ConfigureAwait, GetAwaiter() 등 사용 가능
              | 캐시 또는 즉시 반환 시 유리         | 반환값이 자주 캐시되어 있다면 힙 할당 없이 성능 최적화 가능

            🆚 Task vs ValueTask
              | 항목             | Task                        | ValueTask
              |------------------|-----------------------------|----------------------------
              | 타입             | 참조 타입 (class)           | 값 타입 (struct)
              | 힙 할당          | 항상 발생                   | 필요 시만 발생
              | 즉시 완료 반환   | 힙 할당 불필요하지만 발생   | ✅ 스택에서 즉시 반환 가능
              | 재사용성         | 가능                        | ❌ 1회 await만 가능
              | await 지원       | ✅                         | ✅

       */

        return new ValueTask<int>(123); // 즉시 결과 반환
    }

    static async Task<int> getFromAsync(int value)
    {
        await Task.Delay(100); // 비동기 작업 시뮬레이션
        return value;
    }

    static ValueTask<int> use_ValueTask(bool useTask, int value)
    {
        if(useTask == false)
            return new ValueTask<int>(value); // ✅ 즉시 결과

        // ✅ Task<int>를 만들어서 ValueTask<int>로 감쌈
        return new ValueTask<int>(getFromAsync(value));
    }

    public static void Test()
    {
        //use_ValueTask(useTask:false, 10);

        //ValueTask_what();
    }
}
