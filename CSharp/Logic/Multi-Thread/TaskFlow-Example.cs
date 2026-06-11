using System;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread
{
    public class TaskFlowExample
    {
        private static void Log(string message)
        {
            Console.WriteLine($"[TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}] {message}");
        }

        private static void Test_NewTask_Start()
        {
            Log("[01] new Task(...) 호출");

            var task = new Task(() =>
            {
                Log("[06] Worker Thread가 Task 획득");
                Log("[07] Queue에서 Task 제거 / 실제 실행 시작 / Thread 할당 시점");
                Log("[08] 설정된 메서드 / Lambda 실행");
                Log("[09] 일반 동기 메서드");
                Log("[30] 동기 메서드 끝까지 실행");
            });

            Log($"[01] 생성 직후 상태: {task.Status}");

            Log("[03] task.Start() 호출");
            task.Start();

            Log("[04] TaskScheduler 등록");
            Log("[05] ThreadPool 작업 큐 등록");

            task.Wait();

            Log($"[32] 작업 완료 상태: {task.Status}");
            Log("[33] Thread 반환 / 재사용");
            Log("[34] Task 객체 유지 / 참조 해제 시 GC 대상");

            Log("==============================");
        }

        private static async Task Test_TaskRunAsync()
        {
            Log("[02] Task.Run(...) 호출");

            var task = Task.Run(() =>
            {
                Log("[06] Worker Thread가 Task 획득");
                Log("[07] Queue에서 Task 제거 / 실제 실행 시작 / Thread 할당 시점");
                Log("[08] 설정된 메서드 / Lambda 실행");
            });

            Log($"[02] Task 상태: {task.Status}");

            await task;

            Log($"[32] 작업 완료 상태: {task.Status}");

            Log("==============================");
        }

        private static async Task Test_SyncMethodAsync()
        {
            Log("[09] 일반 동기 메서드 테스트");

            await Task.Run(() =>
            {
                Log("[08] 설정된 메서드 / Lambda 실행");
                Log("[09] 일반 동기 메서드");
                System.Threading.Thread.Sleep(300);
                Log("[30] 동기 메서드 끝까지 실행 / 중간 중단 없음");
            });

            Log("[32] 작업 완료");
            Log("[33] Thread 반환 / 재사용");

            Log("==============================");
        }

        private static async Task Test_Async_WithAwait_CompletedTaskAsync()
        {
            Log("[10] async 메서드 테스트 - await 대상이 이미 완료된 경우");

            await Task.Run(async () =>
            {
                Log("[10] async 메서드");
                Log("[11] await 사용 호출");

                Task completedTask = Task.CompletedTask;

                Log("[12] await SomeTaskAsync() 도달");
                Log($"[13] await 대상 Task 상태 확인: {completedTask.Status}");
                Log("[14] 이미 완료됨 / 즉시 계속 실행");

                await completedTask;

                Log("[22] await 이후 코드 재개");
                Log("[29] async 메서드 계속 실행");
            });

            Log("[31] async 메서드 끝까지 실행");
            Log("[32] 작업 완료");

            Log("==============================");
        }

        private static async Task Test_Async_WithAwait_NotCompletedTaskAsync()
        {
            Log("[10] async 메서드 테스트 - await 대상이 아직 실행 중인 경우");

            await Task.Run(async () =>
            {
                Log("[10] async 메서드");
                Log("[11] await 사용 호출");

                Task delayTask = Task.Delay(1000);

                Log("[12] await SomeTaskAsync() 도달");
                Log($"[13] await 대상 Task 상태 확인: {delayTask.Status}");
                Log("[15] 아직 실행 중 / continuation 등록");
                Log("[16] 현재 async 메서드 일시 중단 / StateMachine 저장 / Thread 반환 가능");

                await delayTask;

                Log("[17] await 대상 Task 완료");
                Log("[18] await 이후 코드 실행 예약");
                Log("[19] SynchronizationContext 복귀 또는 [20] ThreadPool Thread 재개");
                Log("[21] async StateMachine 복원");
                Log("[22] await 이후 코드 재개");
                Log("[29] async 메서드 계속 실행");
            });

            Log("[31] async 메서드 끝까지 실행");
            Log("[32] 작업 완료");
            Log("[33] Thread 반환 / 재사용");

            Log("==============================");
        }

        private static async Task Test_Async_WithoutAwaitAsync()
        {
            Log("[10] async 메서드 테스트 - await 없이 async 함수 호출");

            await Task.Run(async () =>
            {
                Log("[10] async 메서드");

                Log("[24] await 없이 async 함수 호출");
                Task fireAndForgetTask = FireAndForgetAsync();

                Log("[25] Task 객체만 반환받음 / 현재 메서드는 대기 안 함");
                Log($"[25] 반환받은 Task 상태: {fireAndForgetTask.Status}");

                Log("[26] 다음 코드 즉시 실행 / fire-and-forget 상태");
                Log("[27] 호출한 Task가 계속 실행되는 동안 현재 async 메서드는 먼저 완료 가능");
                Log("[28] 예외가 현재 try/catch에서 잡히지 않을 수 있음");

                Log("[29] async 메서드 계속 실행");

                await Task.Delay(200);
            });

            Log("[31] async 메서드 끝까지 실행");

            await Task.Delay(1500);

            Log("==============================");
        }

        private static async Task FireAndForgetAsync()
        {
            Log("[24] FireAndForgetAsync 시작");
            await Task.Delay(1000);
            Log("[24] FireAndForgetAsync 완료");

            Log("==============================");
        }

        private static async Task Test_FaultedAsync()
        {
            Log("[32] Faulted 상태 테스트");

            try
            {
                await Task.Run(() =>
                {
                    Log("[08] 설정된 메서드 / Lambda 실행");
                    throw new InvalidOperationException("Task 내부 예외 발생");
                });
            }
            catch (Exception ex)
            {
                Log($"[32] 상태 변경: Faulted / 예외 처리: {ex.Message}");
            }

            Log("==============================");
        }

        private static async Task Test_CanceledAsync()
        {
            Log("[32] Canceled 상태 테스트");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            try
            {
                await Task.Run(() =>
                {
                    cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Log("[32] 상태 변경: Canceled");
            }

            Log("==============================");
        }

        public static async Task runAllAsync()
        {
            Test_NewTask_Start();
            await Test_TaskRunAsync();
            await Test_SyncMethodAsync();
            await Test_Async_WithAwait_CompletedTaskAsync();
            await Test_Async_WithAwait_NotCompletedTaskAsync();
            await Test_Async_WithoutAwaitAsync();
            await Test_FaultedAsync();
            await Test_CanceledAsync();

            Console.ReadLine();
        }

        public static void Test()
        {
            runAllAsync().Wait();
        }
    }
}
