using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MultiThread
{
    public class SemaphoreSlim
    {
        private static void UpdateMax(ref int target, int value)
        {
            while (true)
            {
                int snapshot = Volatile.Read(ref target);

                if (value <= snapshot)
                    return;

                int original = Interlocked.CompareExchange(
                    ref target,
                    value,
                    snapshot);

                if (original == snapshot)
                    return;
            }
        }

        static async Task SemaphoreSlim_what()
        {
            /*
                📚 SemaphoreSlim

                  1. 개요
                    - SemaphoreSlim은 동시에 접근 가능한 작업 수를 제한하는 경량 세마포어이다.
                    - System.Threading 네임스페이스에 포함되어 있다.
                    - .NET Framework 4.0부터 SemaphoreSlim 타입이 제공되었다.
                    - WaitAsync()는 .NET Framework 4.5부터 제공되었다.
                    - async / await 환경에서는 lock 대신 SemaphoreSlim.WaitAsync()를 주로 사용한다.
                    - 동시에 하나만 실행해야 하는 비동기 작업, 외부 API 호출 제한, DB 작업 제한,
                      파일 IO 제한, 네트워크 요청 제한 등에 사용할 수 있다.

                  2. 기본 개념
                    - 생성 방식은 new SemaphoreSlim(initialCount, maxCount) 형태이다.
                    - initialCount는 현재 진입 가능한 슬롯 수를 의미한다.
                    - maxCount는 Release()를 통해 증가할 수 있는 최대 슬롯 수를 의미한다.
                    - new SemaphoreSlim(1, 1)은 동시에 하나의 작업만 진입할 수 있게 한다.
                    - new SemaphoreSlim(3, 3)은 동시에 최대 3개의 작업만 진입할 수 있게 한다.

                  3. 핵심 특징
                    - Wait()는 동기 대기 메서드이다.
                    - WaitAsync()는 비동기 대기 메서드이다.
                    - Release()는 점유한 슬롯을 반환한다.
                    - WaitAsync()로 진입한 뒤에는 반드시 Release()를 호출해야 한다.
                    - Release() 누락 시 다른 작업들이 영원히 대기할 수 있다.
                    - lock과 달리 await가 포함된 비동기 코드 보호에 사용할 수 있다.

                  4. 실행 흐름
                    - 작업이 WaitAsync()를 호출한다.
                    - 세마포어의 남은 슬롯 수가 1 이상이면 즉시 진입한다.
                    - 남은 슬롯 수가 0이면 다른 작업이 Release()할 때까지 대기한다.
                    - 보호 구간의 작업을 실행한다.
                    - finally 블록에서 Release()를 호출한다.
                    - 대기 중인 다음 작업이 진입한다.

                  5. 대표 메서드 또는 주요 코드
                    - Wait()
                      동기 방식으로 세마포어 진입을 대기한다.

                    - WaitAsync()
                      비동기 방식으로 세마포어 진입을 대기한다.

                    - WaitAsync(CancellationToken)
                      취소 가능한 비동기 대기 작업을 수행한다.

                    - Release()
                      점유한 세마포어 슬롯을 반환한다.

                    - CurrentCount
                      현재 사용 가능한 슬롯 수를 반환한다.
                      단, 멀티 스레드 환경에서는 순간 값이므로 제어 로직의 기준으로 남용하면 안 된다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - SemaphoreSlim 자체는 멀티 스레드 환경에서 안전하게 사용할 수 있다.
                    - 여러 스레드 또는 여러 Task가 동시에 WaitAsync()를 호출할 수 있다.
                    - 내부적으로 허용 개수만큼만 진입시키고 나머지는 대기시킨다.
                    - ThreadPool 작업, async Task, Parallel.ForEachAsync 등과 함께 사용할 수 있다.
                    - SemaphoreSlim은 특정 스레드 소유 개념이 없다.
                      즉, WaitAsync()한 스레드와 Release()하는 스레드가 반드시 같을 필요는 없다.
                    - 이 점은 lock과 다르다.
                      lock은 같은 스레드에서 진입과 해제가 이루어져야 한다.

                  7. 주의점
                    - WaitAsync() 이후에는 반드시 try/finally로 Release()를 보장해야 한다.
                    - Release()를 너무 많이 호출하면 maxCount를 초과할 수 있다.
                    - maxCount를 지정하지 않고 new SemaphoreSlim(1)로 만들면 maxCount가 int.MaxValue가 된다.
                      이 경우 Release()를 잘못 여러 번 호출해도 예외가 바로 발생하지 않을 수 있다.
                    - 따라서 보통 new SemaphoreSlim(1, 1)처럼 maxCount를 명확히 지정하는 것이 좋다.
                    - WaitAsync(CancellationToken)이 취소되면 세마포어를 획득하지 못한 것이므로 Release()를 호출하면 안 된다.
                    - SemaphoreSlim은 재진입 락이 아니다.
                      같은 async 흐름에서 다시 WaitAsync()를 호출하면 데드락이 발생할 수 있다.
                    - 보호 구간 안에서 오래 걸리는 작업을 너무 많이 넣으면 전체 처리량이 떨어질 수 있다.

                  8. 예상 결과
                    - new SemaphoreSlim(1, 1)을 사용하면 동시에 하나의 작업만 보호 구간에 진입한다.
                    - new SemaphoreSlim(N, N)을 사용하면 동시에 최대 N개의 작업만 보호 구간에 진입한다.
                    - Release()가 정상적으로 호출되면 대기 중인 다음 작업이 순차적으로 실행된다.
                    - Release()가 누락되면 대기 작업이 끝나지 않을 수 있다.
            */
            {
                var semaphore = new System.Threading.SemaphoreSlim(1, 1);

                int runningCount = 0;
                int maxRunningCount = 0;

                async Task WorkAsync(int jobId)
                {
                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - WaitAsync 대기 시작 / CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                    await semaphore.WaitAsync();

                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - WaitAsync 성공 / CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                    try
                    {
                        int current = Interlocked.Increment(ref runningCount);

                        UpdateMax(ref maxRunningCount, current);

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - 보호 구간 진입 / runningCount={current}, maxRunningCount={maxRunningCount}, CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                        await Task.Delay(50);

                        int after = Interlocked.Decrement(ref runningCount);

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - 보호 구간 종료 / runningCount={after}, CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    }
                    finally
                    {
                        semaphore.Release();

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - Release 호출 완료 / CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    }
                }

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim 단일 진입 제한 테스트 시작");
                Console.WriteLine("생성: new SemaphoreSlim(1, 1)");
                Console.WriteLine("동시에 20개 Task 실행");
                Console.WriteLine("하지만 보호 구간에는 최대 1개만 진입해야 함");
                Console.WriteLine($"초기 CurrentCount = {semaphore.CurrentCount}");
                Console.WriteLine("========================================");

                Task[] tasks = Enumerable.Range(0, 20)
                    .Select(i => WorkAsync(i + 1))
                    .ToArray();

                await Task.WhenAll(tasks);

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim 단일 진입 제한 테스트 종료");
                Console.WriteLine($"maxRunningCount = {maxRunningCount}");
                Console.WriteLine("기대값: 1");
                Console.WriteLine($"최종 CurrentCount = {semaphore.CurrentCount}");
                Console.WriteLine("========================================");

                Assert.AreEqual(
                    1,
                    maxRunningCount,
                    "보호 구간에는 동시에 하나의 작업만 진입해야 합니다.");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static async Task SemaphoreSlim_with_ConcurrencyLimit()
        {
            /*
                📚 SemaphoreSlim - 동시 실행 개수 제한

                  1. 개요
                    - SemaphoreSlim은 동시에 실행 가능한 작업 개수를 N개로 제한할 수 있다.
                    - new SemaphoreSlim(3, 3)은 동시에 최대 3개의 작업만 보호 구간에 진입하게 한다.
                    - 이 테스트는 30개의 작업을 동시에 실행해도 최대 동시 실행 수가 3을 넘지 않는지 확인한다.

                  2. 기본 개념
                    - initialCount = 3이면 처음에 3개의 작업이 즉시 진입할 수 있다.
                    - maxCount = 3이면 Release()로 반환 가능한 최대 슬롯 수도 3이다.
                    - runningCount는 현재 보호 구간 안에 있는 작업 수이다.
                    - maxRunningCount는 테스트 중 가장 많이 동시에 실행된 수이다.

                  3. 핵심 특징
                    - SemaphoreSlim은 단순한 lock보다 넓은 용도로 사용할 수 있다.
                    - lock은 동시에 1개만 허용하지만, SemaphoreSlim은 N개까지 허용할 수 있다.
                    - 외부 API 호출 제한, DB Connection 제한, 파일 처리 동시성 제한 등에 유용하다.

                  4. 실행 흐름
                    - 30개의 Task를 생성한다.
                    - 각 Task는 WaitAsync()로 세마포어 진입을 요청한다.
                    - 처음에는 최대 3개만 진입한다.
                    - 작업이 끝난 Task가 Release()를 호출한다.
                    - 대기 중인 다음 Task가 진입한다.
                    - 모든 Task가 완료될 때까지 반복된다.

                  5. 대표 메서드 또는 주요 코드
                    - new SemaphoreSlim(3, 3)
                      동시 진입 가능한 작업 수를 3개로 제한한다.

                    - WaitAsync()
                      슬롯이 없으면 비동기 대기한다.

                    - Release()
                      사용한 슬롯을 반환한다.

                    - Task.WhenAll()
                      모든 작업이 완료될 때까지 대기한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - 여러 Task는 ThreadPool에서 병렬 실행될 수 있다.
                    - 하지만 SemaphoreSlim이 보호 구간 진입 개수를 제한한다.
                    - runningCount는 순간적으로 증가/감소하므로 Interlocked를 사용한다.
                    - maxRunningCount는 최대 동시 진입 수를 검증하기 위한 값이다.

                  7. 주의점
                    - Release()를 너무 많이 호출하면 SemaphoreFullException이 발생할 수 있다.
                    - WaitAsync() 성공 후에만 Release()를 호출해야 한다.
                    - 보호 구간 안의 작업 시간이 길수록 대기 작업도 오래 기다린다.

                  8. 예상 결과
                    - 모든 Task가 완료된다.
                    - maxRunningCount는 3 이하이다.
                    - 충분한 작업 수가 있으므로 maxRunningCount는 보통 정확히 3이 된다.
            */
            {
                var semaphore = new System.Threading.SemaphoreSlim(3, 3);

                int runningCount = 0;
                int maxRunningCount = 0;

                async Task WorkAsync(int jobId)
                {
                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - WaitAsync 대기 시작 / CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                    await semaphore.WaitAsync();

                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - WaitAsync 성공 / CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                    try
                    {
                        int current = Interlocked.Increment(ref runningCount);

                        UpdateMax(ref maxRunningCount, current);

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - 보호 구간 진입 / runningCount={current}, maxRunningCount={maxRunningCount}, CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                        await Task.Delay(50);

                        int after = Interlocked.Decrement(ref runningCount);

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - 보호 구간 종료 / runningCount={after}, CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    }
                    finally
                    {
                        semaphore.Release();

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - Release 호출 완료 / CurrentCount={semaphore.CurrentCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    }
                }

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim 동시 실행 개수 제한 테스트 시작");
                Console.WriteLine("생성: new SemaphoreSlim(3, 3)");
                Console.WriteLine("동시에 30개 Task 실행");
                Console.WriteLine("하지만 보호 구간에는 최대 3개만 진입해야 함");
                Console.WriteLine($"초기 CurrentCount = {semaphore.CurrentCount}");
                Console.WriteLine("========================================");

                Task[] tasks = Enumerable.Range(0, 30)
                    .Select(i => WorkAsync(i + 1))
                    .ToArray();

                await Task.WhenAll(tasks);

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim 동시 실행 개수 제한 테스트 종료");
                Console.WriteLine($"maxRunningCount = {maxRunningCount}");
                Console.WriteLine("기대값: 3");
                Console.WriteLine($"최종 CurrentCount = {semaphore.CurrentCount}");
                Console.WriteLine("========================================");

                Assert.IsTrue(
                    maxRunningCount <= 3,
                    "보호 구간에 동시에 3개를 초과한 작업이 진입하면 안 됩니다.");

                Assert.AreEqual(
                    3,
                    maxRunningCount,
                    "충분한 작업 수가 있으므로 최대 동시 실행 수는 3이어야 합니다.");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static async Task SemaphoreSlim_with_Cancellation()
        {
            /*
                📚 SemaphoreSlim - CancellationToken

                  1. 개요
                    - WaitAsync(CancellationToken)을 사용하면 대기 중인 작업을 취소할 수 있다.
                    - 이미 다른 작업이 세마포어를 점유하고 있을 때, 대기 중인 작업은 취소될 수 있다.
                    - 이 테스트는 세마포어를 획득하지 못한 작업이 취소되는지 확인한다.

                  2. 기본 개념
                    - SemaphoreSlim(1, 1)은 하나의 작업만 진입할 수 있다.
                    - 먼저 WaitAsync()로 세마포어를 점유한다.
                    - 두 번째 WaitAsync(cancellationToken)는 대기 상태가 된다.
                    - CancellationTokenSource.Cancel()을 호출하면 대기 작업이 취소된다.

                  3. 핵심 특징
                    - WaitAsync(cancellationToken)이 취소되면 OperationCanceledException이 발생한다.
                    - 취소된 작업은 세마포어를 획득하지 못한 것이다.
                    - 따라서 취소된 WaitAsync에 대해서는 Release()를 호출하면 안 된다.

                  4. 실행 흐름
                    - 첫 번째 WaitAsync()로 세마포어를 점유한다.
                    - 두 번째 WaitAsync(cancellationToken)를 호출한다.
                    - CancellationTokenSource를 취소한다.
                    - 두 번째 WaitAsync가 OperationCanceledException을 발생시키는지 확인한다.
                    - 마지막에 첫 번째 점유에 대한 Release()만 호출한다.

                  5. 대표 메서드 또는 주요 코드
                    - WaitAsync()
                      세마포어를 점유한다.

                    - WaitAsync(CancellationToken)
                      취소 가능한 대기 작업을 수행한다.

                    - CancellationTokenSource.Cancel()
                      대기 작업에 취소 신호를 전달한다.

                    - Assert.ThrowsAsync<OperationCanceledException>()
                      취소 예외 발생 여부를 검증한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - 첫 번째 작업이 세마포어를 점유하고 있으면 두 번째 작업은 대기한다.
                    - 대기 중인 작업은 CancellationToken에 의해 취소될 수 있다.
                    - 취소는 세마포어의 슬롯 수를 증가시키지 않는다.

                  7. 주의점
                    - WaitAsync가 취소된 경우 Release()를 호출하면 안 된다.
                    - Release()는 실제로 세마포어를 획득한 작업만 호출해야 한다.
                    - finally에서는 내가 획득한 경우에만 Release()하도록 구조화해야 한다.

                  8. 예상 결과
                    - 두 번째 WaitAsync는 OperationCanceledException을 발생시킨다.
                    - 세마포어 카운트는 잘못 증가하지 않는다.
                    - 마지막 Release()는 첫 번째 WaitAsync에 대해서만 수행된다.
            */
            {
                var semaphore = new System.Threading.SemaphoreSlim(1, 1);

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim CancellationToken 테스트 시작");
                Console.WriteLine("생성: new SemaphoreSlim(1, 1)");
                Console.WriteLine("initialCount = 1, maxCount = 1");
                Console.WriteLine($"현재 Count = {semaphore.CurrentCount}");
                Console.WriteLine("========================================");

                Console.WriteLine("[1] 첫 번째 WaitAsync() 호출");
                await semaphore.WaitAsync();
                Console.WriteLine($"    첫 번째 WaitAsync() 성공");
                Console.WriteLine($"    현재 Count = {semaphore.CurrentCount}");
                Console.WriteLine("    세마포어가 이미 점유되었으므로 다음 WaitAsync는 대기해야 합니다.");

                try
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        Console.WriteLine();
                        Console.WriteLine("[2] 두 번째 WaitAsync(cts.Token) 호출");
                        Task waitingTask = semaphore.WaitAsync(cts.Token);

                        Console.WriteLine($"    두 번째 WaitAsync 상태 = {waitingTask.Status}");
                        Console.WriteLine($"    현재 Count = {semaphore.CurrentCount}");
                        Console.WriteLine("    Count가 0이므로 두 번째 WaitAsync는 완료되지 않고 대기 상태여야 합니다.");

                        Console.WriteLine();
                        Console.WriteLine("[3] CancellationTokenSource.Cancel() 호출");
                        cts.Cancel();

                        Console.WriteLine($"    Cancel() 호출 후 두 번째 WaitAsync 상태 = {waitingTask.Status}");
                        Console.WriteLine("    이제 waitingTask를 await하면 OperationCanceledException이 발생해야 합니다.");

                        bool exceptionThrown = false;

                        try
                        {
                            Console.WriteLine();
                            Console.WriteLine("[4] waitingTask await 시작");

                            await waitingTask;

                            Console.WriteLine("    오류: waitingTask가 예외 없이 완료되었습니다.");
                            Assert.Fail("OperationCanceledException 예외가 발생해야 합니다.");
                        }
                        catch (OperationCanceledException ex)
                        {
                            exceptionThrown = true;

                            Console.WriteLine("    OperationCanceledException 발생");
                            Console.WriteLine($"    Exception Type = {ex.GetType().Name}");
                            Console.WriteLine($"    Message = {ex.Message}");
                        }

                        Console.WriteLine();
                        Console.WriteLine("[5] 취소 예외 발생 여부 확인");
                        Console.WriteLine($"    exceptionThrown = {exceptionThrown}");
                        Console.WriteLine("    기대값: true");

                        Assert.IsTrue(
                            exceptionThrown,
                            "OperationCanceledException 예외가 발생해야 합니다.");

                        Console.WriteLine($"    현재 Count = {semaphore.CurrentCount}");
                        Console.WriteLine("    취소된 WaitAsync는 세마포어를 획득하지 않았으므로 Count는 여전히 0이어야 합니다.");
                    }
                }
                finally
                {
                    Console.WriteLine();
                    Console.WriteLine("[6] finally에서 첫 번째 WaitAsync에 대한 Release() 호출");
                    semaphore.Release();
                    Console.WriteLine($"    Release() 이후 Count = {semaphore.CurrentCount}");
                }

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim CancellationToken 테스트 종료");
                Console.WriteLine($"최종 Count = {semaphore.CurrentCount}");
                Console.WriteLine("기대값: 1");
                Console.WriteLine("========================================");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static async Task SemaphoreSlim_with_ReleaseOverlapCall()
        {
            /*
                📚 SemaphoreSlim - Release 중복 호출

                  1. 개요
                    - SemaphoreSlim은 Release()를 통해 세마포어 슬롯을 반환한다.
                    - .NET Framework 4.8.1에서 사용 가능하다.
                    - maxCount를 명확히 지정하면 Release()를 너무 많이 호출했을 때 예외를 확인할 수 있다.

                  2. 기본 개념
                    - initialCount = 1이면 처음 슬롯이 하나 있다.
                    - Wait()를 호출하면 슬롯이 0이 된다.
                    - Release()를 한 번 호출하면 슬롯이 다시 1이 된다.
                    - maxCount = 1이므로 여기서 다시 Release()하면 최대값을 초과한다.

                  3. 핵심 특징
                    - maxCount를 지정하지 않으면 Release() 실수를 잡기 어려울 수 있다.
                    - new SemaphoreSlim(1, 1)은 잘못된 Release()를 빠르게 발견하는 데 유리하다.
                    - Release() 초과 호출 시 SemaphoreFullException이 발생한다.

                  4. 실행 흐름
                    - SemaphoreSlim(1, 1)을 생성한다.
                    - Wait()로 슬롯을 획득한다.
                    - Release()로 슬롯을 반환한다.
                    - 다시 Release()를 호출한다.
                    - SemaphoreFullException 발생을 검증한다.

                  5. 대표 메서드 또는 주요 코드
                    - Wait()
                      동기 방식으로 세마포어 슬롯을 획득한다.

                    - Release()
                      슬롯을 반환한다.

                    - Assert.ThrowsException<SemaphoreFullException>()
                      maxCount 초과 Release 예외를 검증한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - Release() 초과 호출은 멀티 스레드 환경에서 치명적인 버그가 될 수 있다.
                    - 세마포어 카운트가 잘못 증가하면 의도보다 많은 작업이 보호 구간에 진입할 수 있다.
                    - maxCount 지정은 이러한 실수를 조기에 발견하는 데 도움이 된다.

                  7. 주의점
                    - Release()는 Wait() 또는 WaitAsync() 성공 횟수와 짝이 맞아야 한다.
                    - 예외 발생 시에도 finally에서 정확히 한 번만 Release()해야 한다.
                    - 취소된 WaitAsync에 대해서는 Release()하면 안 된다.

                  8. 예상 결과
                    - 첫 번째 Release()는 성공한다.
                    - 두 번째 Release()는 SemaphoreFullException을 발생시킨다.
            */
            {
                var semaphore = new System.Threading.SemaphoreSlim(1, 1);

                Console.WriteLine("========================================");
                Console.WriteLine("SemaphoreSlim Release 중복 호출 테스트 시작");
                Console.WriteLine("생성: new SemaphoreSlim(1, 1)");
                Console.WriteLine("initialCount = 1, maxCount = 1");
                Console.WriteLine($"현재 Count = {semaphore.CurrentCount}");
                Console.WriteLine("========================================");

                Console.WriteLine("[1] Wait() 호출");
                semaphore.Wait();
                Console.WriteLine($"    Wait() 이후 Count = {semaphore.CurrentCount}");

                Console.WriteLine("[2] 첫 번째 Release() 호출");
                semaphore.Release();
                Console.WriteLine($"    첫 번째 Release() 이후 Count = {semaphore.CurrentCount}");
                Console.WriteLine("    첫 번째 Release()는 정상입니다.");

                bool exceptionThrown = false;

                Console.WriteLine("[3] 두 번째 Release() 호출");
                Console.WriteLine("    이미 Count가 maxCount인 1이므로 여기서는 예외가 발생해야 합니다.");

                try
                {
                    semaphore.Release();

                    Console.WriteLine("    오류: 두 번째 Release()가 성공했습니다. 이 경우는 기대한 결과가 아닙니다.");
                }
                catch (SemaphoreFullException ex)
                {
                    exceptionThrown = true;

                    Console.WriteLine("    SemaphoreFullException 발생");
                    Console.WriteLine($"    Message = {ex.Message}");
                }

                Console.WriteLine("========================================");
                Console.WriteLine($"exceptionThrown = {exceptionThrown}");
                Console.WriteLine("기대값: true");
                Console.WriteLine("========================================");

                Assert.IsTrue(
                    exceptionThrown,
                    "SemaphoreFullException 예외가 발생해야 합니다.");

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        public sealed class AsyncLock : IDisposable
        {
            private readonly System.Threading.SemaphoreSlim _semaphore = new System.Threading.SemaphoreSlim(1, 1);

            private bool _disposed;

            public async Task<Releaser> LockAsync(
                CancellationToken cancellationToken = default)
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(AsyncLock));

                await _semaphore.WaitAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Releaser(_semaphore);
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _semaphore.Dispose();
            }

            public sealed class Releaser : IDisposable
            {
                private System.Threading.SemaphoreSlim _semaphore;

                internal Releaser(System.Threading.SemaphoreSlim semaphore)
                {
                    _semaphore = semaphore;
                }

                public void Dispose()
                {
                    var semaphore = Interlocked.Exchange(ref _semaphore, null);

                    if (semaphore != null)
                    {
                        semaphore.Release();
                    }
                }
            }
        }


        private static void doUpdateMax(ref int target, int value)
        {
            while (true)
            {
                int snapshot = Volatile.Read(ref target);

                if (value <= snapshot)
                    return;

                int original = Interlocked.CompareExchange(
                    ref target,
                    value,
                    snapshot);

                if (original == snapshot)
                    return;
            }
        }

        static async Task SemaphoreSlim_with_AsyncLock()
        {
            /*
                📚 SemaphoreSlim - AsyncLock

                  1. 개요
                    - AsyncLock은 SemaphoreSlim(1, 1)을 사용하여 async 환경에서 lock처럼 사용하기 위한 래퍼이다.
                    - .NET Framework 4.8.1에서 사용할 수 있다.
                    - using 패턴으로 Release() 누락을 줄일 수 있다.

                  2. 기본 개념
                    - AsyncLock 내부에는 SemaphoreSlim(1, 1)이 있다.
                    - LockAsync()는 WaitAsync()를 호출한다.
                    - LockAsync()는 IDisposable Releaser를 반환한다.
                    - Releaser.Dispose()에서 Release()를 호출한다.

                  3. 핵심 특징
                    - try/finally를 직접 반복 작성하지 않아도 된다.
                    - using 구문으로 보호 구간을 표현할 수 있다.
                    - 보호 구간이 끝나면 Dispose()에 의해 자동으로 Release()된다.

                  4. 실행 흐름
                    - 여러 Task가 동시에 LockAsync()를 호출한다.
                    - 하나의 Task만 Releaser를 획득하고 보호 구간에 진입한다.
                    - 보호 구간이 끝나면 Releaser.Dispose()가 호출된다.
                    - Dispose() 내부에서 SemaphoreSlim.Release()가 호출된다.
                    - 다음 대기 Task가 진입한다.

                  5. 대표 메서드 또는 주요 코드
                    - LockAsync()
                      세마포어를 비동기로 획득하고 Releaser를 반환한다.

                    - Releaser.Dispose()
                      세마포어를 반환한다.

                    - using
                      스코프 종료 시 Dispose()를 자동 호출한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - 여러 Task가 동시에 실행되어도 내부 SemaphoreSlim이 동시 진입을 1개로 제한한다.
                    - Release() 호출이 using 패턴으로 보장되므로 실수 가능성이 줄어든다.
                    - Releaser.Dispose()는 Interlocked.Exchange를 사용하여 중복 Dispose에 대한 안정성을 높였다.

                  7. 주의점
                    - AsyncLock은 재진입 락이 아니다.
                    - 같은 async 흐름 안에서 다시 LockAsync()를 호출하면 대기 상태가 될 수 있다.
                    - Dispose() 이후 AsyncLock을 다시 사용하면 ObjectDisposedException이 발생한다.

                  8. 예상 결과
                    - 모든 Task가 완료된다.
                    - 보호 구간에 동시에 들어간 작업 수는 항상 1개이다.
                    - maxRunningCount는 1이다.
            */
            {
                using (var asyncLock = new AsyncLock())
                {
                    int runningCount = 0;
                    int maxRunningCount = 0;

                    async Task WorkAsync(int jobId)
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - Lock 대기 시작 / Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                        using (await asyncLock.LockAsync())
                        {
                            int current = Interlocked.Increment(ref runningCount);

                            doUpdateMax(ref maxRunningCount, current);

                            Console.WriteLine(
                                $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - Lock 획득 / runningCount={current}, maxRunningCount={maxRunningCount}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");

                            await Task.Delay(50);

                            int after = Interlocked.Decrement(ref runningCount);

                            Console.WriteLine(
                                $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - 보호 구간 종료 / runningCount={after}, Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");
                        }

                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] Job {jobId:00} - Lock 해제 완료 / Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    }

                    Console.WriteLine("========================================");
                    Console.WriteLine("SemaphoreSlim_with_AsyncLock 테스트 시작");
                    Console.WriteLine("동시에 20개 Task 실행");
                    Console.WriteLine("하지만 AsyncLock 때문에 보호 구간에는 항상 1개만 진입해야 함");
                    Console.WriteLine("========================================");

                    Task[] tasks = Enumerable.Range(0, 20)
                        .Select(i => WorkAsync(i + 1))
                        .ToArray();

                    await Task.WhenAll(tasks);

                    Console.WriteLine("========================================");
                    Console.WriteLine("SemaphoreSlim_with_AsyncLock 테스트 종료");
                    Console.WriteLine($"maxRunningCount = {maxRunningCount}");
                    Console.WriteLine("기대값: 1");
                    Console.WriteLine("========================================");

                    Assert.AreEqual(1, maxRunningCount);
                }

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        public static void Test()
        {
            //SemaphoreSlim_with_AsyncLock().Wait();

            //SemaphoreSlim_with_ReleaseOverlapCall().Wait();

            //SemaphoreSlim_with_Cancellation().Wait();

            //SemaphoreSlim_with_ConcurrencyLimit().Wait();

            //SemaphoreSlim_what().Wait();
        }
    }
}
