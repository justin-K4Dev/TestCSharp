using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdvancedStep
{
    public class BlockingCollection
    {
        static void BlockingCollection_what()
        {
            /*
                📚 BlockingCollection<T>

                  1. 개요
                    - Producer / Consumer 구조를 쉽게 만들기 위한 동기식 Blocking Queue 도구.
                    - 내부적으로 ConcurrentQueue<T>, ConcurrentStack<T>, ConcurrentBag<T> 같은
                      IProducerConsumerCollection<T>를 감싸서 사용한다.
                    - 데이터를 넣는 쪽(Producer)과 꺼내는 쪽(Consumer)이 서로 다른 스레드에서 동작할 때 유용하다.
                    - .NET Framework 4.0 부터 추가되었다.
                    - .NET Core, .NET 5 이상 사용 가능하다.

                    * 대표 용도:
                      - 작업 큐
                      - 로그 큐
                      - DB 저장 요청 큐
                      - 게임 서버 LogicTask / Mailbox / JobQueue
                      - 전용 Thread 기반 Executor
                      - Producer / Consumer 패턴 구현

                  2. 기본 개념

                    BlockingCollection<T> collection = new BlockingCollection<T>();

                    Producer:
                      - Add(item)
                      - TryAdd(item)

                    Consumer:
                      - Take()
                      - TryTake(out item)
                      - GetConsumingEnumerable()

                  3. 핵심 특징

                    - Thread-Safe 하다.
                    - Add() / Take()가 Blocking 동작을 지원한다.
                    - 데이터가 없을 때 Take()를 호출하면 데이터가 들어올 때까지 기다린다.
                    - Bounded Capacity를 지정하면 큐가 가득 찼을 때 Add()가 대기한다.
                    - CompleteAdding()을 호출하면 더 이상 Add할 수 없음을 알릴 수 있다.
                    - Consumer는 CompleteAdding() 이후 남은 데이터를 모두 처리하고 종료할 수 있다.

                  4. Channel<T>와의 차이

                    BlockingCollection<T>
                      - 동기 Blocking 기반
                      - Thread 전용 Worker 구조에 적합
                      - Take(), Add()가 스레드를 실제로 막는다
                      - async/await 친화적이지 않다

                    Channel<T>
                      - async/await 기반
                      - 비동기 Producer / Consumer 구조에 적합
                      - WriteAsync(), ReadAsync()로 비동기 대기 가능
                      - ThreadPool 기반 서버 코드에 적합

                  5. 대표 메서드

                    Add(item)
                      - 데이터를 추가한다.
                      - Bounded Capacity가 가득 차 있으면 공간이 생길 때까지 대기한다.

                    TryAdd(item)
                      - 즉시 추가를 시도한다.
                      - 성공하면 true, 실패하면 false.

                    TryAdd(item, timeout)
                      - timeout 동안 추가를 시도한다.

                    Take()
                      - 데이터를 하나 꺼낸다.
                      - 데이터가 없으면 들어올 때까지 대기한다.

                    TryTake(out item)
                      - 즉시 꺼내기를 시도한다.
                      - 성공하면 true, 실패하면 false.

                    TryTake(out item, timeout)
                      - timeout 동안 꺼내기를 시도한다.

                    GetConsumingEnumerable()
                      - CompleteAdding()이 호출되고 내부 데이터가 모두 소비될 때까지
                        foreach로 데이터를 계속 꺼낸다.

                    CompleteAdding()
                      - 더 이상 데이터를 추가하지 않겠다고 알린다.

                    IsAddingCompleted
                      - Add가 완료되었는지 확인한다.

                    IsCompleted
                      - Add도 완료되었고, 내부 데이터도 모두 소비되었는지 확인한다.

                  6. Bounded Capacity

                    BlockingCollection<T> queue = new BlockingCollection<T>(boundedCapacity: 100);

                    - 최대 100개까지만 저장할 수 있다.
                    - 큐가 가득 찬 상태에서 Add()를 호출하면 공간이 생길 때까지 대기한다.
                    - Producer가 Consumer보다 너무 빠를 때 메모리 폭주를 막을 수 있다.
                    - 이것도 일종의 Backpressure 역할을 한다.

                  7. 사용 시 주의점

                    - BlockingCollection은 동기 Blocking 구조다.
                    - async/await 코드 안에서 Add(), Take()를 직접 호출하면 ThreadPool 스레드를 막을 수 있다.
                    - 전용 Thread Worker 구조에는 적합하다.
                    - ASP.NET Core 요청 처리 흐름처럼 비동기 서버 코드에서는 Channel<T>가 더 적합하다.
                    - CompleteAdding()을 호출하지 않으면 GetConsumingEnumerable() 루프가 끝나지 않을 수 있다.
                    - Add 완료 후 다시 Add하면 InvalidOperationException이 발생할 수 있다.
            */
            {
                BlockingCollection<int> queue = new BlockingCollection<int>();

                Console.WriteLine(
                    "[BlockingCollection] Enter | ThreadId={0}",
                    Environment.CurrentManagedThreadId);

                Task producer = Task.Run(() =>
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        queue.Add(i);

                        Console.WriteLine(
                            $"[Producer] Add: {i}, ThreadId = {Environment.CurrentManagedThreadId}");

                        System.Threading.Thread.Sleep(100);
                    }

                    queue.CompleteAdding();

                    Console.WriteLine(
                        $"[Producer] CompleteAdding, ThreadId = {Environment.CurrentManagedThreadId}");
                });

                Task consumer = Task.Run(() =>
                {
                    foreach (int value in queue.GetConsumingEnumerable())
                    {
                        Console.WriteLine(
                            $"[Consumer] Take: {value}, ThreadId = {Environment.CurrentManagedThreadId}");
                    }

                    Console.WriteLine(
                        $"[Consumer] Completed, ThreadId = {Environment.CurrentManagedThreadId}");
                });

                System.Threading.Tasks.Task.WaitAll(producer, consumer);

                Console.WriteLine(
                    "[BlockingCollection] End | ThreadId={0}",
                    Environment.CurrentManagedThreadId);

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static void BlockingCollection_with_Take()
        {
            /*
                📚 BlockingCollection<T> + Take()

                  1. 개요
                    - BlockingCollection<T>에서 Take()가 어떻게 동작하는지 확인하는 예제다.
                    - Consumer가 먼저 Take()를 호출하고,
                      Producer가 나중에 Add()를 호출하는 구조다.
                    - 큐에 데이터가 없는 상태에서 Take()를 호출하면
                      Consumer 스레드는 데이터가 들어올 때까지 대기한다.

                  2. 기본 개념

                    BlockingCollection<int> queue = new BlockingCollection<int>();

                    Consumer:
                      - queue.Take()
                      - 큐에 데이터가 있으면 즉시 꺼낸다.
                      - 큐에 데이터가 없으면 데이터가 들어올 때까지 현재 스레드를 Blocking한다.

                    Producer:
                      - queue.Add(100)
                      - 큐에 데이터를 추가한다.
                      - Take()에서 대기 중인 Consumer가 있으면 Consumer가 깨어난다.

                  3. 전체 흐름

                    Consumer Task
                      -> "Take 대기 시작" 출력
                      -> queue.Take() 호출
                      -> 현재 큐가 비어 있으므로 대기

                    Producer Task
                      -> 1초 대기
                      -> "Add 시작" 출력
                      -> queue.Add(100)
                      -> 큐에 100 추가
                      -> Take() 중이던 Consumer가 깨어남

                    Consumer Task
                      -> queue.Take()가 100을 반환
                      -> "Take 완료: 100" 출력

                  4. Take()의 역할

                    int value = queue.Take();

                    - 큐에서 값을 하나 꺼낸다.
                    - 큐에 값이 있으면 즉시 반환한다.
                    - 큐가 비어 있으면 값이 들어올 때까지 기다린다.
                    - 이 기다림은 async/await 대기가 아니라 동기 Blocking이다.
                    - 즉, Take()를 호출한 ThreadPool 스레드가 실제로 멈춘다.

                  5. Add()의 역할

                    queue.Add(100);

                    - 큐에 100을 추가한다.
                    - 대기 중인 Take()가 있으면 Take()가 깨어나 값을 가져간다.
                    - 이 예제에서는 Consumer가 이미 Take()에서 대기 중이므로,
                      Add(100) 이후 Consumer가 바로 진행할 수 있다.

                  6. Task.Delay(1000).Wait()

                    - Producer 쪽에서 1초 늦게 Add()하기 위한 코드다.
                    - Consumer가 먼저 Take()에 들어가서 대기하는 상황을 만들기 위해 사용했다.
                    - Wait()는 현재 Producer ThreadPool 스레드를 실제로 Blocking한다.
                    - async 예제라면 await Task.Delay(1000)을 쓰는 것이 더 자연스럽다.

                  7. 실행 스레드 관점

                    - Consumer는 Task.Run()으로 실행되므로 ThreadPool 스레드에서 실행된다.
                    - Producer도 Task.Run()으로 실행되므로 ThreadPool 스레드에서 실행된다.
                    - Consumer가 queue.Take()에 들어가면 데이터가 들어올 때까지
                      Consumer ThreadPool 스레드가 Blocking된다.
                    - Producer가 queue.Add(100)을 호출하면 Consumer의 Take()가 완료된다.

                  8. 예상 출력

                    [Consumer] Take 대기 시작, ThreadId = 5

                    약 1초 후:

                    [Producer] Add 시작, ThreadId = 6
                    [Producer] Add 완료, ThreadId = 6
                    [Consumer] Take 완료: 100, ThreadId = 5

                    ThreadId 값은 실행 환경에 따라 달라질 수 있다.

                  9. 주의점

                    - Take()는 데이터가 들어올 때까지 무한정 기다릴 수 있다.
                    - Producer가 Add()를 하지 않으면 Consumer는 계속 대기한다.
                    - 종료 가능한 구조가 필요하면 CancellationToken을 받는 Take(token) 또는
                      TryTake(out value, timeout)을 사용하는 것이 좋다.
                    - CompleteAdding()이 호출되고 더 이상 가져올 데이터가 없으면
                      Take()는 InvalidOperationException을 발생시킬 수 있다.
                    - async/await 기반 코드에서는 Channel<T>의 ReadAsync()가 더 적합할 수 있다.

                  10. 핵심 요약

                    - Take()는 큐에서 데이터를 하나 꺼낸다.
                    - 큐가 비어 있으면 현재 스레드를 Blocking한다.
                    - Add()가 호출되어 데이터가 들어오면 Take()가 깨어난다.
                    - 이 예제는 Consumer가 먼저 대기하고 Producer가 나중에 데이터를 넣는 구조를 보여준다.
            */

            BlockingCollection<int> queue = new BlockingCollection<int>();

            Task consumer = Task.Run(() =>
            {
                Console.WriteLine(
                    $"[Consumer] Take 대기 시작, ThreadId = {Environment.CurrentManagedThreadId}");

                int value = queue.Take();

                Console.WriteLine(
                    $"[Consumer] Take 완료: {value}, ThreadId = {Environment.CurrentManagedThreadId}");
            });

            Task producer = Task.Run(() =>
            {
                Task.Delay(1000).Wait();

                Console.WriteLine(
                    $"[Producer] Add 시작, ThreadId = {Environment.CurrentManagedThreadId}");

                queue.Add(100);

                Console.WriteLine(
                    $"[Producer] Add 완료, ThreadId = {Environment.CurrentManagedThreadId}");
            });

            Task.WaitAll(producer, consumer);

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        static void BlockingCollection_with_Bounded()
        {
            /*
                📚 BlockingCollection<T> + Bounded Capacity

                  1. 개요
                    - BlockingCollection<T>에 저장 가능한 최대 개수를 제한하는 예제다.
                    - Producer는 데이터를 빠르게 추가하고, Consumer는 데이터를 느리게 소비한다.
                    - 큐가 가득 차면 Producer의 Add()가 대기한다.
                    - Producer / Consumer 간 처리 속도 차이를 제어하는 Backpressure 예제다.

                  2. 기본 개념

                    BlockingCollection<int> queue =
                        new BlockingCollection<int>(boundedCapacity: 3);

                    - boundedCapacity가 3이므로 큐에는 최대 3개까지만 저장할 수 있다.
                    - 큐가 비어 있으면 Consumer는 데이터가 들어올 때까지 기다릴 수 있다.
                    - 큐가 가득 차 있으면 Producer는 공간이 생길 때까지 기다린다.

                  3. 전체 구조

                    Producer Task
                      -> 1부터 10까지 queue.Add(i) 시도
                      -> 큐에 공간이 있으면 즉시 Add 완료
                      -> 큐가 가득 차 있으면 Add()에서 대기
                      -> 모든 데이터를 넣은 뒤 CompleteAdding() 호출

                    Consumer Task
                      -> queue.GetConsumingEnumerable()로 데이터 소비
                      -> 값을 하나 꺼낼 때마다 500ms 대기
                      -> CompleteAdding()이 호출되고 큐가 비면 루프 종료

                  4. Producer 흐름

                    for (int i = 1; i <= 10; i++)
                    {
                        Console.WriteLine($"Add waiting: {i}");

                        queue.Add(i);

                        Console.WriteLine($"Add done: {i}");
                    }

                    - "Add waiting"은 Add() 호출 직전에 출력된다.
                    - "Add done"은 실제로 큐에 데이터가 추가된 뒤 출력된다.
                    - 만약 "Add waiting: 5"는 출력됐는데 "Add done: 5"가 늦게 출력된다면,
                      queue.Add(5)에서 대기하고 있었다는 의미다.

                  5. Consumer 흐름

                    foreach (int value in queue.GetConsumingEnumerable())
                    {
                        Thread.Sleep(500);
                    }

                    - GetConsumingEnumerable()은 큐에서 값을 하나씩 꺼낸다.
                    - 큐가 비어 있고 CompleteAdding()이 아직 호출되지 않았다면 데이터가 들어올 때까지 대기한다.
                    - CompleteAdding()이 호출된 후에도 큐에 남은 데이터는 계속 꺼낸다.
                    - 큐에 남은 데이터까지 모두 소비하면 foreach 루프가 종료된다.

                  6. Bounded Capacity 동작

                    예를 들어 Consumer가 아직 아무것도 꺼내지 않았다면:

                      queue = [1, 2, 3]
                      남은 공간 = 0

                    이 상태에서 Producer가 다음 코드를 실행하면:

                      queue.Add(4);

                    - 큐가 가득 차 있으므로 Add(4)는 즉시 완료되지 않는다.
                    - Consumer가 queue에서 하나를 꺼내 공간이 생길 때까지 대기한다.
                    - Consumer가 1을 꺼내면 큐 상태는 다음처럼 된다.

                      queue = [2, 3]
                      남은 공간 = 1

                    - 그때 Add(4)가 완료된다.

                  7. CompleteAdding()

                    queue.CompleteAdding();

                    - Producer가 더 이상 데이터를 추가하지 않겠다고 알린다.
                    - CompleteAdding() 이후 Add()를 호출하면 InvalidOperationException이 발생할 수 있다.
                    - Consumer는 CompleteAdding() 이후에도 큐에 남아 있는 데이터는 계속 처리한다.
                    - 큐가 완전히 비면 GetConsumingEnumerable() 루프가 종료된다.

                  8. 실행 스레드 관점

                    - Producer는 Task.Run()으로 실행되므로 ThreadPool 스레드에서 실행된다.
                    - Consumer도 Task.Run()으로 실행되므로 ThreadPool 스레드에서 실행된다.
                    - queue.Add(i)가 대기하면 Producer의 ThreadPool 스레드가 실제로 Blocking된다.
                    - Thread.Sleep(500)은 Consumer의 ThreadPool 스레드를 실제로 500ms 동안 Blocking한다.
                    - 따라서 async/await 기반 서버 코드에서는 Channel<T>가 더 적합할 수 있다.
                    - 반대로 전용 Thread 기반 Worker 구조에서는 BlockingCollection<T>가 단순하고 명확하다.

                  9. 예상 흐름

                    [Producer] Add waiting: 1
                    [Producer] Add done   : 1
                    [Producer] Add waiting: 2
                    [Producer] Add done   : 2
                    [Producer] Add waiting: 3
                    [Producer] Add done   : 3
                    [Producer] Add waiting: 4

                    여기서 큐가 가득 차 있으면 Add(4)가 대기할 수 있다.

                    [Consumer] Take       : 1

                    Consumer가 값을 하나 꺼내 공간이 생기면:

                    [Producer] Add done   : 4

                    이후 같은 흐름이 반복된다.

                  10. 주의점

                    - 실제 출력 순서는 스레드 스케줄링에 따라 달라질 수 있다.
                    - capacity가 3이어도 Add 4가 바로 성공할 수 있다.
                      Consumer가 그 사이에 이미 하나를 꺼냈을 수 있기 때문이다.
                    - CompleteAdding()을 호출하지 않으면 Consumer의 GetConsumingEnumerable() 루프가 끝나지 않을 수 있다.
                    - BlockingCollection<T>는 동기 Blocking 구조이므로 ThreadPool 위에서 과도하게 사용하면 스레드 고갈 문제가 생길 수 있다.

                  11. 핵심 요약

                    - boundedCapacity는 큐에 저장 가능한 최대 개수를 제한한다.
                    - 큐가 가득 차면 Add()는 공간이 생길 때까지 Blocking된다.
                    - Consumer가 Take하면 공간이 생기고 Producer가 다시 진행된다.
                    - CompleteAdding()은 더 이상 추가할 데이터가 없음을 알리는 종료 신호다.
                    - 이 예제는 Producer가 빠르고 Consumer가 느린 상황에서 Backpressure가 발생하는 구조를 보여준다.
            */

            BlockingCollection<int> queue = new BlockingCollection<int>(boundedCapacity: 3);

            Task producer = Task.Run(() =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine(
                        $"[Producer] Add waiting: {i}, ThreadId = {Environment.CurrentManagedThreadId}");

                    queue.Add(i);

                    Console.WriteLine(
                        $"[Producer] Add done   : {i}, ThreadId = {Environment.CurrentManagedThreadId}");
                }

                queue.CompleteAdding();
            });

            Task consumer = Task.Run(() =>
            {
                foreach (int value in queue.GetConsumingEnumerable())
                {
                    Console.WriteLine(
                        $"[Consumer] Take       : {value}, ThreadId = {Environment.CurrentManagedThreadId}");

                    // 일부러 느리게 소비
                    System.Threading.Thread.Sleep(500);
                }
            });

            Task.WaitAll(producer, consumer);

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        public sealed class BlockingLogicTaskWithResult : IDisposable
        {
            private readonly BlockingCollection<Action> _queue;
            private readonly System.Threading.Thread _thread;

            public BlockingLogicTaskWithResult(int capacity = 4096)
            {
                _queue = new BlockingCollection<Action>(boundedCapacity: capacity);

                _thread = new System.Threading.Thread(Run)
                {
                    IsBackground = true,
                    Name = "BlockingLogicTaskWithResult"
                };
            }

            public void Start()
            {
                _thread.Start();
            }

            public void Post(Action action)
            {
                _queue.Add(action);
            }

            public Task<T> InvokeAsync<T>(Func<T> func)
            {
                var tcs = new TaskCompletionSource<T>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

                _queue.Add(() =>
                {
                    try
                    {
                        T result = func();
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });

                return tcs.Task;
            }

            public void Complete()
            {
                _queue.CompleteAdding();
            }

            public void Join()
            {
                _thread.Join();
            }

            private void Run()
            {
                Console.WriteLine(
                    $"[BlockingLogicTaskWithResult] Start ThreadId = {Environment.CurrentManagedThreadId}");

                foreach (Action action in _queue.GetConsumingEnumerable())
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BlockingLogicTaskWithResult] Error: {ex}");
                    }
                }

                Console.WriteLine(
                    $"[BlockingLogicTaskWithResult] End ThreadId = {Environment.CurrentManagedThreadId}");
            }

            public void Dispose()
            {
                Complete();
                Join();
                _queue.Dispose();
            }
        }

        static void BlockingCollection_with_ReturnValue()
        {
            /*
                📚 BlockingCollection<T> + return 값 처리

                  1. 개요
                    - BlockingCollection<Action> 기반의 전용 Logic Thread에서 작업을 실행하고,
                      그 작업의 return 값을 호출자 쪽에서 받는 예제다.
                    - 일반적인 BlockingCollection<Action>은 Action만 저장하므로 직접적인 return 값이 없다.
                    - return 값을 호출자에게 전달하기 위해 TaskCompletionSource<T>를 사용한다.

                    * 대표 용도:
                      - Logic Thread에서 계산 후 결과 반환
                      - Actor / Zone / Room 전용 스레드에서 상태 조회 후 결과 반환
                      - 전용 Worker Thread에서 작업 실행 후 호출자에게 결과 전달
                      - 동기 작업을 전용 스레드에 위임하고 Task<T> 형태로 결과 받기

                  2. 기본 개념

                    BlockingCollection<Action> _queue;
                    TaskCompletionSource<T> tcs;

                    호출자:
                      - InvokeAsync<T>(Func<T> func)를 호출한다.
                      - InvokeAsync<T>()는 Task<T>를 즉시 반환한다.
                      - 호출자는 Task<T>.Result 또는 await Task<T>로 결과를 기다린다.

                    전용 Logic Thread:
                      - BlockingCollection에서 Action을 꺼낸다.
                      - Action 내부에서 Func<T>를 실행한다.
                      - T result = func();
                      - tcs.SetResult(result)를 호출한다.

                    결과 전달:
                      - tcs.SetResult(result)가 호출되면 InvokeAsync<T>()가 반환했던 Task<T>가 완료된다.
                      - 호출자는 완료된 Task<T>를 통해 result 값을 받는다.

                  3. 전체 흐름

                    호출자 스레드
                      -> logicTask.InvokeAsync(() => 10 + 20)
                      -> Task<int> resultTask 반환
                      -> resultTask.Result 또는 await resultTask 로 대기

                    BlockingLogicTaskWithResult 전용 스레드
                      -> Run() 실행
                      -> _queue.GetConsumingEnumerable()로 Action 대기
                      -> Action 꺼냄
                      -> Func<T> 실행
                      -> result = 30
                      -> tcs.SetResult(30)

                    호출자 스레드
                      -> resultTask 완료됨
                      -> result = 30 수신

                  4. InvokeAsync<T>()의 역할

                    public Task<T> InvokeAsync<T>(Func<T> func)
                    {
                        var tcs = new TaskCompletionSource<T>(
                            TaskCreationOptions.RunContinuationsAsynchronously);

                        _queue.Add(() =>
                        {
                            try
                            {
                                T result = func();
                                tcs.SetResult(result);
                            }
                            catch (Exception ex)
                            {
                                tcs.SetException(ex);
                            }
                        });

                        return tcs.Task;
                    }

                    - TaskCompletionSource<T>를 생성한다.
                    - Func<T>를 직접 즉시 실행하지 않는다.
                    - Func<T>를 감싼 Action을 BlockingCollection에 넣는다.
                    - 호출자에게는 tcs.Task를 반환한다.
                    - 실제 func() 실행은 전용 Logic Thread가 Action을 꺼낼 때 일어난다.

                  5. TaskCompletionSource<T>의 역할

                    tcs.SetResult(result)

                      - Task<T>를 정상 완료 상태로 만든다.
                      - Task<T>의 결과값을 result로 설정한다.
                      - 이 Task<T>를 기다리던 호출자를 깨운다.
                      - await task의 반환값이 result가 된다.

                    tcs.SetException(ex)

                      - Task<T>를 예외 완료 상태로 만든다.
                      - 호출자가 await 하면 해당 예외가 다시 발생한다.
                      - resultTask.Result를 사용하면 AggregateException 형태로 감싸질 수 있다.

                    TaskCreationOptions.RunContinuationsAsynchronously

                      - SetResult를 호출한 전용 Logic Thread에서
                        호출자의 continuation이 바로 실행되는 것을 방지한다.
                      - 호출자 후속 코드는 별도로 스케줄링된다.
                      - Logic Thread가 호출자 코드를 대신 실행해버리는 상황을 줄일 수 있다.
                      - Executor / Mailbox / Actor 구조에서는 사용하는 것이 안전하다.

                  6. resultTask.Result

                    int result = resultTask.Result;

                    - 현재 스레드를 Blocking하면서 Task<int>가 완료될 때까지 기다린다.
                    - 전용 Logic Thread에서 func() 실행 후 tcs.SetResult(30)을 호출하면 대기가 풀린다.
                    - result에는 30이 들어간다.

                    주의:
                      - 콘솔 테스트에서는 동작 확인용으로 사용할 수 있다.
                      - UI / ASP.NET / SynchronizationContext가 있는 환경에서는
                        .Result나 .Wait()가 데드락 또는 스레드 점유 문제를 만들 수 있다.
                      - 일반적으로는 await resultTask 사용을 권장한다.

                  7. Complete() / Join()

                    logicTask.Complete();

                      - 더 이상 작업을 추가하지 않겠다고 알린다.
                      - 내부적으로 BlockingCollection.CompleteAdding()을 호출한다.
                      - 이미 큐에 들어간 작업은 계속 처리된다.
                      - CompleteAdding() 이후 Add()를 호출하면 예외가 발생할 수 있다.

                    logicTask.Join();

                      - 전용 Logic Thread가 종료될 때까지 현재 스레드가 기다린다.
                      - Complete 후 큐에 남은 작업이 모두 처리되면
                        GetConsumingEnumerable() 루프가 끝나고 Run()이 종료된다.

                  8. 실행 스레드 관점

                    계산 실행 ThreadId
                      - BlockingLogicTaskWithResult 전용 ThreadId

                    tcs.SetResult(result) 호출 ThreadId
                      - BlockingLogicTaskWithResult 전용 ThreadId

                    결과 수신 ThreadId
                      - BlockingCollection_with_ReturnValue()를 실행한 호출자 ThreadId
                      - 또는 await를 사용한 경우 continuation이 실행되는 ThreadPool ThreadId일 수 있다.

                    즉:
                      - Func<T> 실행은 전용 Logic Thread에서 일어난다.
                      - return 값 생성도 전용 Logic Thread에서 일어난다.
                      - 호출자는 Task<T>를 통해 결과만 전달받는다.

                  9. 이 구조의 장점

                    - 작업 실행 위치를 전용 Thread로 고정할 수 있다.
                    - Actor / Zone / Room 내부 상태를 한 스레드에서만 안전하게 조회하거나 변경할 수 있다.
                    - 호출자는 Task<T> 형태로 결과를 받을 수 있다.
                    - 예외도 Task<T>를 통해 호출자에게 전달할 수 있다.
                    - BlockingCollection<Action> 구조를 크게 바꾸지 않고 return 값을 지원할 수 있다.

                  10. 주의점

                    - BlockingCollection<Action> 자체가 return 값을 지원하는 것은 아니다.
                    - return 값 처리는 Action 내부의 TaskCompletionSource<T>가 담당한다.
                    - Complete() 이후 InvokeAsync<T>()를 호출하면 Add()에서 예외가 발생할 수 있다.
                    - Dispose / Complete / Join 중복 호출에 대한 방어 코드가 있으면 더 안전하다.
                    - resultTask.Result는 현재 스레드를 막으므로 실제 코드에서는 await 사용을 우선 고려한다.
                    - func() 안에서 오래 걸리는 작업을 실행하면 전용 Logic Thread가 그동안 다른 작업을 처리하지 못한다.

                  11. 핵심 요약

                    - BlockingCollection<Action>은 기본적으로 반환값이 없다.
                    - 반환값이 필요하면 TaskCompletionSource<T>를 함께 사용한다.
                    - InvokeAsync<T>()는 작업을 큐에 넣고 Task<T>를 반환한다.
                    - 전용 Logic Thread가 func()를 실행한다.
                    - func()의 결과를 tcs.SetResult(result)로 Task<T>에 전달한다.
                    - 호출자는 resultTask.Result 또는 await resultTask로 결과를 받는다.
            */

            var logicTask = new BlockingLogicTaskWithResult(capacity: 1024);

            logicTask.Start();

            Task<int> resultTask = logicTask.InvokeAsync(() =>
            {
                Console.WriteLine(
                    $"계산 실행 ThreadId = {Environment.CurrentManagedThreadId}");

                return 10 + 20;
            });

            int result = resultTask.Result;

            Console.WriteLine(
                $"결과 수신 Result = {result}, ThreadId = {Environment.CurrentManagedThreadId}");

            logicTask.Complete();
            logicTask.Join();

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        static void BlockingCollection_with_TryAddTryTake()
        {
            /*
                📚 BlockingCollection<T> + TryAdd / TryTake

                  1. 개요
                    - BlockingCollection<T>에서 데이터를 "기다리지 않고" 추가하거나 꺼내는 예제다.
                    - Add() / Take()는 조건이 맞지 않으면 현재 스레드를 Blocking한다.
                    - TryAdd() / TryTake()는 즉시 시도하고, 성공 여부를 bool 값으로 반환한다.
                    - 큐가 가득 차 있거나 비어 있는 상황에서 무한 대기하지 않고 바로 판단하고 싶을 때 사용한다.

                  2. 기본 개념

                    BlockingCollection<int> queue =
                        new BlockingCollection<int>(boundedCapacity: 3);

                    - boundedCapacity가 3이므로 큐에는 최대 3개까지만 저장할 수 있다.
                    - TryAdd(value)는 큐에 공간이 있으면 값을 추가하고 true를 반환한다.
                    - TryAdd(value)는 큐에 공간이 없으면 기다리지 않고 false를 반환한다.
                    - TryTake(out value)는 큐에 데이터가 있으면 하나 꺼내고 true를 반환한다.
                    - TryTake(out value)는 큐가 비어 있으면 기다리지 않고 false를 반환한다.

                  3. 전체 구조

                    TryAdd 단계
                      -> 10 추가 시도
                      -> 20 추가 시도
                      -> 30 추가 시도
                      -> 40 추가 시도

                    TryTake 단계
                      -> 현재 큐에 들어 있는 값을 꺼낼 수 있는 만큼 모두 꺼냄
                      -> 더 이상 값이 없으면 TryTake()가 false를 반환하고 while 종료

                  4. TryAdd 흐름

                    bool add1 = queue.TryAdd(10);
                    bool add2 = queue.TryAdd(20);
                    bool add3 = queue.TryAdd(30);
                    bool add4 = queue.TryAdd(40);

                    - capacity가 3이므로 10, 20, 30은 추가에 성공한다.
                    - 10, 20, 30이 들어가면 큐는 가득 찬다.
                    - 40을 추가하려는 시점에는 남은 공간이 없다.
                    - TryAdd(40)은 기다리지 않고 즉시 false를 반환한다.
                    - 따라서 40은 큐에 들어가지 않는다.

                    큐 상태:

                      queue = [10, 20, 30]

                  5. TryTake 흐름

                    while (queue.TryTake(out int value))
                    {
                        Console.WriteLine($"Take: {value}");
                    }

                    - TryTake()는 즉시 꺼내기를 시도한다.
                    - 큐에 데이터가 있으면 true를 반환하고 value에 꺼낸 값을 넣는다.
                    - 큐가 비어 있으면 false를 반환한다.
                    - false가 반환되면 while 루프가 종료된다.

                    꺼내는 값:

                      10
                      20
                      30

                    이후 큐가 비어 있으므로 TryTake()는 false를 반환한다.

                  6. Add / TryAdd 차이

                    Add(item)
                      - 큐에 공간이 있으면 추가한다.
                      - 큐가 가득 차 있으면 공간이 생길 때까지 현재 스레드를 Blocking한다.

                    TryAdd(item)
                      - 큐에 공간이 있으면 추가하고 true를 반환한다.
                      - 큐가 가득 차 있으면 기다리지 않고 false를 반환한다.
                      - 실패한 값은 큐에 저장되지 않는다.

                  7. Take / TryTake 차이

                    Take()
                      - 큐에 데이터가 있으면 하나 꺼낸다.
                      - 큐가 비어 있으면 데이터가 들어올 때까지 현재 스레드를 Blocking한다.

                    TryTake(out item)
                      - 큐에 데이터가 있으면 하나 꺼내고 true를 반환한다.
                      - 큐가 비어 있으면 기다리지 않고 false를 반환한다.

                  8. 실행 스레드 관점

                    - 이 예제는 별도의 Producer / Consumer Task를 만들지 않는다.
                    - TryAdd와 TryTake가 모두 현재 메서드를 실행 중인 같은 스레드에서 수행된다.
                    - TryAdd / TryTake는 Blocking하지 않으므로 대기 시간이 거의 없다.
                    - 큐가 가득 차거나 비어 있으면 즉시 false를 반환한다.

                  9. 예상 출력

                    Add 10: True
                    Add 20: True
                    Add 30: True
                    Add 40: False
                    Take: 10
                    Take: 20
                    Take: 30

                  10. 주의점

                    - TryAdd()가 false를 반환한 값은 자동으로 재시도되지 않는다.
                    - 실패한 값을 다시 넣고 싶으면 호출자가 직접 재시도하거나 별도 처리해야 한다.
                    - TryTake()는 Producer가 나중에 데이터를 넣을 예정이어도 현재 비어 있으면 즉시 false를 반환한다.
                    - "기다려서라도 반드시 처리해야 하는 데이터"라면 Add() / Take() 또는 timeout 있는 TryAdd / TryTake를 고려해야 한다.
                    - "현재 가능한 만큼만 처리"하는 Tick 기반 처리에는 TryTake()가 유용하다.

                  11. 핵심 요약

                    - TryAdd()는 기다리지 않는 추가 시도다.
                    - TryTake()는 기다리지 않는 꺼내기 시도다.
                    - 성공하면 true, 실패하면 false를 반환한다.
                    - capacity가 3이므로 10, 20, 30은 저장되고 40은 실패한다.
                    - 이후 TryTake()로 현재 큐에 있는 10, 20, 30만 꺼낸다.
            */

            var queue = new BlockingCollection<int>(boundedCapacity: 3);

            // capacity가 3이므로 10, 20, 30까지는 성공한다.
            bool add1 = queue.TryAdd(10);
            bool add2 = queue.TryAdd(20);
            bool add3 = queue.TryAdd(30);
            // 큐가 이미 가득 찼으므로 40은 추가되지 않고 false를 반환한다.
            bool add4 = queue.TryAdd(40);

            Console.WriteLine($"Add 10: {add1}");
            Console.WriteLine($"Add 20: {add2}");
            Console.WriteLine($"Add 30: {add3}");
            Console.WriteLine($"Add 40: {add4}");

            // 현재 큐에 들어 있는 값을 즉시 꺼낼 수 있는 만큼 모두 꺼낸다.
            // 더 이상 꺼낼 값이 없으면 TryTake()가 false를 반환하고 루프가 종료된다.
            while (queue.TryTake(out int value))
            {
                Console.WriteLine($"Take: {value}");
            }

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        static void BlockingCollection_with_Timeout()
        {
            /*
                📚 BlockingCollection<T> + Timeout

                  1. 개요
                    - BlockingCollection<T>에서 일정 시간 동안만 Add / Take를 시도하는 예제다.
                    - Add() / Take()는 조건이 맞지 않으면 무한정 Blocking될 수 있다.
                    - TryAdd(item, timeout) / TryTake(out item, timeout)을 사용하면
                      지정한 시간까지만 기다리고, 실패하면 false를 반환한다.
                    - Producer / Consumer가 너무 오래 멈추면 안 되는 상황에서 유용하다.

                  2. 기본 개념

                    BlockingCollection<int> queue =
                        new BlockingCollection<int>(boundedCapacity: 1);

                    - boundedCapacity가 1이므로 큐에는 최대 1개까지만 저장할 수 있다.
                    - 큐가 가득 찬 상태에서 TryAdd(value, timeout)을 호출하면
                      timeout 시간 동안 공간이 생기기를 기다린다.
                    - 큐가 비어 있는 상태에서 TryTake(out value, timeout)을 호출하면
                      timeout 시간 동안 데이터가 들어오기를 기다린다.

                  3. 전체 구조

                    초기 상태
                      -> queue.Add(10)
                      -> capacity가 1이므로 큐가 가득 참

                    TryAdd 단계
                      -> TryAdd(20, 1000) 호출
                      -> 큐가 가득 차 있으므로 바로 추가 불가
                      -> 최대 1000ms 동안 공간 대기
                      -> 아무도 Take하지 않으므로 실패
                      -> false 반환

                    TryTake 단계
                      -> TryTake(out value, 1000) 호출
                      -> 큐에 10이 있으므로 즉시 성공
                      -> value = 10
                      -> true 반환

                  4. queue.Add(10)

                    queue.Add(10);

                    - 큐에 10을 추가한다.
                    - capacity가 1이므로 이 시점에서 큐는 가득 찬 상태가 된다.

                    큐 상태:

                      queue = [10]
                      남은 공간 = 0

                  5. TryAdd(value, timeout)

                    bool added = queue.TryAdd(20, millisecondsTimeout: 1000);

                    - 20을 큐에 추가하려고 시도한다.
                    - 하지만 현재 큐는 이미 10으로 가득 차 있다.
                    - TryAdd는 최대 1000ms 동안 공간이 생기기를 기다린다.
                    - 이 예제에서는 Consumer가 없으므로 아무도 데이터를 꺼내지 않는다.
                    - 따라서 1초 후 false를 반환한다.
                    - 20은 큐에 들어가지 않는다.

                    결과:

                      added == false

                    큐 상태:

                      queue = [10]

                  6. TryTake(out value, timeout)

                    bool taken = queue.TryTake(out int value, millisecondsTimeout: 1000);

                    - 큐에서 값을 꺼내려고 시도한다.
                    - 현재 큐에는 10이 들어 있다.
                    - 따라서 timeout까지 기다릴 필요 없이 즉시 성공한다.
                    - value에는 10이 들어간다.
                    - 반환값은 true다.

                    결과:

                      taken == true
                      value == 10

                    큐 상태:

                      queue = []

                  7. Add / TryAdd / TryAdd(timeout) 차이

                    Add(item)
                      - 큐에 공간이 있으면 추가한다.
                      - 큐가 가득 차 있으면 공간이 생길 때까지 현재 스레드를 계속 Blocking한다.

                    TryAdd(item)
                      - 큐에 공간이 있으면 추가하고 true를 반환한다.
                      - 큐가 가득 차 있으면 기다리지 않고 즉시 false를 반환한다.

                    TryAdd(item, timeout)
                      - 큐에 공간이 있으면 추가하고 true를 반환한다.
                      - 큐가 가득 차 있으면 timeout 동안 공간이 생기기를 기다린다.
                      - timeout 안에 공간이 생기면 true.
                      - timeout 안에 공간이 생기지 않으면 false.

                  8. Take / TryTake / TryTake(timeout) 차이

                    Take()
                      - 큐에 데이터가 있으면 하나 꺼낸다.
                      - 큐가 비어 있으면 데이터가 들어올 때까지 현재 스레드를 계속 Blocking한다.

                    TryTake(out item)
                      - 큐에 데이터가 있으면 하나 꺼내고 true를 반환한다.
                      - 큐가 비어 있으면 기다리지 않고 즉시 false를 반환한다.

                    TryTake(out item, timeout)
                      - 큐에 데이터가 있으면 하나 꺼내고 true를 반환한다.
                      - 큐가 비어 있으면 timeout 동안 데이터가 들어오기를 기다린다.
                      - timeout 안에 데이터가 들어오면 true.
                      - timeout 안에 데이터가 들어오지 않으면 false.

                  9. 실행 스레드 관점

                    - 이 예제는 별도의 Producer / Consumer Task 없이 현재 스레드에서 실행된다.
                    - TryAdd(20, 1000)은 최대 1초 동안 현재 스레드를 Blocking할 수 있다.
                    - TryTake(out value, 1000)은 데이터가 이미 있으므로 즉시 반환된다.
                    - Timeout이 있는 TryAdd / TryTake도 async 대기가 아니라 동기 Blocking 대기다.

                  10. 예상 출력

                    TryAdd 20 시작
                    TryAdd 20 결과: False
                    TryTake 시작
                    TryTake 결과: True, Value = 10

                  11. 사용하기 좋은 경우

                    - 무한 대기는 피하고 싶지만 잠깐은 기다려도 되는 경우.
                    - 큐가 계속 가득 차는 상황을 감지하고 싶은 경우.
                    - Producer 과부하를 로그로 남기거나 실패 처리하고 싶은 경우.
                    - 게임 서버 Tick / Logic Loop에서 일정 시간 이상 대기하면 안 되는 경우.
                    - DB 저장 큐, 로그 큐, 작업 큐에서 제한 시간 기반 제어가 필요한 경우.

                  12. 주의점

                    - TryAdd(item, timeout)이 false를 반환하면 item은 큐에 저장되지 않는다.
                    - 실패한 데이터를 재시도할지, 버릴지, 에러 처리할지는 호출자가 결정해야 한다.
                    - TryTake(out item, timeout)이 false를 반환하면 out value는 기본값일 수 있다.
                    - timeout 동안 현재 스레드가 Blocking되므로 ThreadPool에서 과도하게 사용하면 주의해야 한다.
                    - async/await 기반 코드에서는 Channel<T>의 WaitToWriteAsync / WriteAsync / ReadAsync를 고려할 수 있다.

                  13. 핵심 요약

                    - TryAdd(value, timeout)은 제한 시간 동안만 추가를 시도한다.
                    - TryTake(out value, timeout)은 제한 시간 동안만 꺼내기를 시도한다.
                    - 성공하면 true, 실패하면 false를 반환한다.
                    - 이 예제에서는 capacity가 1이고 이미 10이 들어 있으므로 TryAdd(20)는 실패한다.
                    - 이후 TryTake는 큐에 남아 있는 10을 즉시 꺼내 성공한다.
            */

            BlockingCollection<int> queue = new BlockingCollection<int>(boundedCapacity: 1);

            // capacity가 1이므로 10을 넣으면 큐가 가득 찬다.
            queue.Add(10);

            Console.WriteLine("TryAdd 20 시작");

            // 큐가 가득 찬 상태이므로 20을 바로 넣을 수 없다.
            // 최대 1000ms 동안 공간이 생기기를 기다린다.
            // 이 예제에서는 아무도 Take하지 않으므로 1초 후 false를 반환한다.
            bool added = queue.TryAdd(20, millisecondsTimeout: 1000);

            Console.WriteLine($"TryAdd 20 결과: {added}");

            Console.WriteLine("TryTake 시작");

            // 현재 큐에는 아직 10이 들어 있다.
            // 따라서 기다릴 필요 없이 즉시 꺼내기에 성공한다.
            bool taken = queue.TryTake(out int value, millisecondsTimeout: 1000);

            Console.WriteLine($"TryTake 결과: {taken}, Value = {value}");

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        static void BlockingCollection_with_Concurrent()
        {
            /*
                📚 BlockingCollection<T> + Concurrent Collection

                  1. 개요
                    - BlockingCollection<T>는 내부 저장소를 직접 구현한 컬렉션이라기보다는,
                      IProducerConsumerCollection<T>를 감싸서 Blocking 기능을 추가하는 래퍼에 가깝다.
                    - 생성자에 어떤 Concurrent Collection을 넣느냐에 따라 데이터가 꺼내지는 순서가 달라진다.
                    - 기본 생성자를 사용하면 일반적으로 ConcurrentQueue<T> 기반 FIFO Queue처럼 동작한다.
                    - 명시적으로 ConcurrentQueue<T>, ConcurrentStack<T>, ConcurrentBag<T> 등을 넣을 수 있다.

                  2. 기본 개념

                    BlockingCollection<int> fifo =
                        new BlockingCollection<int>(new ConcurrentQueue<int>());

                    BlockingCollection<int> lifo =
                        new BlockingCollection<int>(new ConcurrentStack<int>());

                    BlockingCollection<int> bag =
                        new BlockingCollection<int>(new ConcurrentBag<int>());

                    - BlockingCollection<T>는 Add / Take / CompleteAdding / GetConsumingEnumerable 같은
                      Producer / Consumer 기능을 제공한다.
                    - 실제 저장 순서와 꺼내는 순서는 내부에 전달한 Concurrent Collection이 결정한다.

                  3. 내부 컬렉션 종류

                    ConcurrentQueue<T>
                      - FIFO 구조다.
                      - 먼저 들어간 데이터가 먼저 나온다.
                      - 메시지 큐, 패킷 큐, 작업 큐, Actor Mailbox 등에 적합하다.

                    ConcurrentStack<T>
                      - LIFO 구조다.
                      - 나중에 들어간 데이터가 먼저 나온다.
                      - 최근 작업을 우선 처리하고 싶은 구조에 사용할 수 있다.

                    ConcurrentBag<T>
                      - 순서 보장이 약하다.
                      - 여러 스레드에서 임시 객체나 순서가 중요하지 않은 작업을 넣고 꺼낼 때 사용할 수 있다.
                      - FIFO / LIFO 같은 명확한 처리 순서가 필요한 구조에는 적합하지 않다.

                  4. BlockingCollection<T>가 추가하는 기능

                    Add(item)
                      - 내부 컬렉션에 item을 추가한다.
                      - boundedCapacity가 가득 차 있으면 공간이 생길 때까지 대기한다.

                    Take()
                      - 내부 컬렉션에서 item을 하나 꺼낸다.
                      - 데이터가 없으면 데이터가 들어올 때까지 대기한다.

                    TryAdd(item)
                      - 즉시 추가를 시도한다.
                      - 성공하면 true, 실패하면 false.

                    TryTake(out item)
                      - 즉시 꺼내기를 시도한다.
                      - 성공하면 true, 실패하면 false.

                    CompleteAdding()
                      - 더 이상 데이터를 추가하지 않겠다고 알린다.

                    GetConsumingEnumerable()
                      - CompleteAdding()이 호출되고 큐가 완전히 빌 때까지 데이터를 계속 꺼낸다.

                  5. 처리 순서 비교

                    | 내부 컬렉션              | 처리 순서 | 특징
                    |--------------------------|-----------|-----------------------------------------
                    | ConcurrentQueue<T>       | FIFO      | 먼저 들어간 값이 먼저 나온다
                    | ConcurrentStack<T>       | LIFO      | 나중에 들어간 값이 먼저 나온다
                    | ConcurrentBag<T>         | 불명확    | 순서 보장이 약하다

                  6. FIFO: ConcurrentQueue<T>

                    입력:
                      1, 2, 3, 4, 5

                    출력:
                      1, 2, 3, 4, 5

                    - 먼저 들어간 값이 먼저 나온다.
                    - 일반적인 작업 큐, 메시지 큐에 가장 자연스럽다.

                  7. LIFO: ConcurrentStack<T>

                    입력:
                      1, 2, 3, 4, 5

                    출력:
                      5, 4, 3, 2, 1

                    - 나중에 들어간 값이 먼저 나온다.
                    - 최근 작업을 우선 처리해야 하는 특수한 경우에 사용할 수 있다.

                  8. Bag: ConcurrentBag<T>

                    입력:
                      1, 2, 3, 4, 5

                    출력:
                      5, 4, 3, 2, 1 처럼 보일 수도 있지만 보장되지 않는다.

                    - ConcurrentBag<T>는 순서를 목적으로 만든 컬렉션이 아니다.
                    - 실행 환경, 스레드 수, 추가/소비 스레드에 따라 꺼내지는 순서가 달라질 수 있다.
                    - Mailbox / Packet Queue 처럼 순서가 중요한 구조에서는 사용하지 않는 것이 좋다.

                  9. Concurrent Collection + boundedCapacity

                    BlockingCollection<int> queue =
                        new BlockingCollection<int>(
                            new ConcurrentQueue<int>(),
                            boundedCapacity: 3);

                    - 내부 저장 순서는 ConcurrentQueue<T>가 담당한다.
                    - 최대 저장 개수 제한은 BlockingCollection<T>가 담당한다.
                    - capacity가 3이므로 큐가 가득 차면 Add()가 대기한다.
                    - Consumer가 값을 꺼내 공간을 만들면 Producer가 다시 진행된다.

                  10. 실행 스레드 관점

                    - FIFO / LIFO / Bag 예제는 현재 스레드에서 순차적으로 실행된다.
                    - boundedCapacity 예제는 Producer와 Consumer를 Task.Run()으로 실행한다.
                    - Producer가 queue.Add(i)에서 대기하면 ThreadPool 스레드가 실제로 Blocking된다.
                    - Consumer의 Thread.Sleep(500)도 ThreadPool 스레드를 실제로 Blocking한다.

                  11. 주요 로직

                    Mailbox / Packet Queue
                      - ConcurrentQueue<T> 기반 FIFO 추천
                      - 메시지 순서가 중요하기 때문이다.

                    최근 작업 우선 처리
                      - ConcurrentStack<T> 고려 가능
                      - 단, 일반적인 메시지 처리에는 부적합할 수 있다.

                    순서가 중요하지 않은 임시 작업 모음
                      - ConcurrentBag<T> 사용 가능
                      - 예: 임시 객체 수집, 순서 없는 병렬 결과 수집 등

                  12. 주의점

                    - BlockingCollection<T>는 순서를 직접 보장하는 것이 아니라,
                      내부 IProducerConsumerCollection<T>의 동작을 따른다.
                    - ConcurrentBag<T>는 순서가 보장되지 않는다.
                    - CompleteAdding()을 호출하지 않으면 GetConsumingEnumerable() 루프가 끝나지 않을 수 있다.
                    - boundedCapacity를 함께 사용하면 Add()가 Blocking될 수 있다.
                    - async/await 기반 대기가 필요하다면 Channel<T>가 더 적합할 수 있다.

                  13. 핵심 요약

                    - BlockingCollection<T>는 Concurrent Collection에 Blocking 기능을 추가한다.
                    - ConcurrentQueue<T>를 넣으면 FIFO처럼 동작한다.
                    - ConcurrentStack<T>를 넣으면 LIFO처럼 동작한다.
                    - ConcurrentBag<T>를 넣으면 순서가 명확하지 않다.
                    - boundedCapacity를 함께 사용하면 저장 개수 제한과 Backpressure를 만들 수 있다.
            */

            //-------------------------------------------------------------------------------------
            // FIFO: ConcurrentQueue<T>
            //-------------------------------------------------------------------------------------
            {
                var fifo = new BlockingCollection<int>(new ConcurrentQueue<int>());

                fifo.Add(1);
                fifo.Add(2);
                fifo.Add(3);
                fifo.Add(4);
                fifo.Add(5);

                fifo.CompleteAdding();

                Console.WriteLine("FIFO - ConcurrentQueue<T>");

                foreach (int value in fifo.GetConsumingEnumerable())
                {
                    Console.WriteLine(value);
                }

                /*
                    FIFO - ConcurrentQueue<T>
                    1
                    2
                    3
                    4
                    5

                    * 먼저 들어간 값 → 먼저 나옴
                      1 → 2 → 3 → 4 → 5
                */

                Console.ReadLine();
            }

            //-------------------------------------------------------------------------------------
            // LIFO: ConcurrentStack<T>
            //-------------------------------------------------------------------------------------
            {
                var lifo = new BlockingCollection<int>(new ConcurrentStack<int>());

                lifo.Add(1);
                lifo.Add(2);
                lifo.Add(3);
                lifo.Add(4);
                lifo.Add(5);

                lifo.CompleteAdding();

                Console.WriteLine("LIFO - ConcurrentStack<T>");

                foreach (int value in lifo.GetConsumingEnumerable())
                {
                    Console.WriteLine(value);
                }

                /*
                    LIFO - ConcurrentStack<T>
                    5
                    4
                    3
                    2
                    1

                    * 나중에 들어간 값 → 먼저 나옴
                      5 → 4 → 3 → 2 → 1
                */

                Console.ReadLine();
            }

            //-------------------------------------------------------------------------------------
            // Bag: ConcurrentBag<T>
            //-------------------------------------------------------------------------------------
            {
                var bag = new BlockingCollection<int>(new ConcurrentBag<int>());

                bag.Add(1);
                bag.Add(2);
                bag.Add(3);
                bag.Add(4);
                bag.Add(5);

                bag.CompleteAdding();

                Console.WriteLine("Bag - ConcurrentBag<T>");

                foreach (int value in bag.GetConsumingEnumerable())
                {
                    Console.WriteLine(value);
                }

                /*
                    Bag - ConcurrentBag<T>
                    5
                    4
                    3
                    2
                    1

                    * 실행 환경과 스레드 상황에 따라 다른 순서로 나올 수 있다 !!!
                      따라서 FIFO/LIFO 같은 명확한 순서 보장이 필요하지 않는 자료구조에서만 사용해야 한다. !!!
                */

                Console.ReadLine();
            }

            //-------------------------------------------------------------------------------------
            // Concurrent + boundedCapacity
            //-------------------------------------------------------------------------------------
            {
                BlockingCollection<int> queue =
                    new BlockingCollection<int>(
                        new ConcurrentQueue<int>(),
                        boundedCapacity: 3);

                Task producer = Task.Run(() =>
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        Console.WriteLine($"[Producer] Add waiting: {i}");

                        queue.Add(i);

                        Console.WriteLine($"[Producer] Add done   : {i}");
                    }

                    queue.CompleteAdding();
                });

                Task consumer = Task.Run(() =>
                {
                    foreach (int value in queue.GetConsumingEnumerable())
                    {
                        Console.WriteLine($"[Consumer] Take       : {value}");

                        System.Threading.Thread.Sleep(500);
                    }
                });

                Task.WaitAll(producer, consumer);

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        public static void Test()
        {
            //BlockingCollection_with_Concurrent();

            //BlockingCollection_with_Timeout();

            //BlockingCollection_with_TryAddTryTake();

            //BlockingCollection_with_ReturnValue();

            //BlockingCollection_with_Bounded();

            //BlockingCollection_with_Take();

            //BlockingCollection_what();
        }
    }
}
