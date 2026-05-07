using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace AdvancedStep;

public class Channel
{
    static void Channel_what()
    {
        /*
            📚 Channel<T>

              1. 개요
                
                * 생산자(Producer)와 소비자(Consumer) 사이에서 데이터를 안전하게 주고받기 위한 비동기 큐.
                * System.Threading.Channels 네임스페이스에 포함되어 있다.
                * 여러 스레드 / 여러 Task 사이에서 데이터를 전달할 때 lock 없이 안전하게 사용할 수 있다.
                * 에플리케이션에서 다음과 같은 용도로 사용할 수 있다.
                  - Actor 모델의 MailBox
                  - 네트워크 패킷 처리 큐
                  - DB 저장 요청 큐
                  - 로그 처리 큐
                  - 비동기 작업 파이프라인
                  - 생산자/소비자 구조
                * .NET Core 3.0 부터 기본 공유 프레임워크에 포함 !!!
                  - .NET Core 3.0 이상 / .NET 5 이상에서는 별도 NuGet 없이 바로 사용 가능
                  - .NET Standard 1.3 이상
                  - .NET Framework 4.6.1 이상

              2. 기본 구조

                Channel<T>
                  ├─ ChannelWriter<T> : 데이터를 쓰는 쪽
                  └─ ChannelReader<T> : 데이터를 읽는 쪽

                즉, Channel<T> 자체를 직접 많이 다루기보다는
                Writer 와 Reader 를 나누어 사용한다.

                Producer
                  -> channel.Writer.WriteAsync(...)
                  -> channel.Writer.TryWrite(...)

                Consumer
                  -> channel.Reader.ReadAsync(...)
                  -> channel.Reader.TryRead(...)
                  -> channel.Reader.ReadAllAsync(...)

              3. Channel 종류

                1) Unbounded Channel

                  - 크기 제한이 없는 Channel.
                  - Write 쪽에서 거의 대기하지 않는다.
                  - 소비가 느리면 메모리에 계속 쌓일 수 있다.
                  - 처리량은 좋지만, 폭주 상황에서는 위험할 수 있다.

                  생성 예:

                    Channel.CreateUnbounded<T>();

                2) Bounded Channel

                  - 크기 제한이 있는 Channel.
                  - 큐가 가득 차면 Writer 가 대기하거나, 기존/신규 데이터를 버릴 수 있다.
                  - 서버에서는 대부분 Bounded Channel이 더 안전하다.
                  - 백프레셔(Backpressure)를 걸 수 있다.

                  생성 예:

                    Channel.CreateBounded<T>(capacity);

              4. 대표 메서드

                Writer 쪽

                  - WriteAsync(value)
                    : 공간이 생길 때까지 기다렸다가 데이터를 쓴다.

                  - TryWrite(value)
                    : 즉시 쓸 수 있으면 true, 아니면 false.

                  - Complete()
                    : 더 이상 데이터를 쓰지 않겠다고 알린다.

                  - TryComplete()
                    : Complete()의 예외 없는 버전.

                Reader 쪽

                  - ReadAsync()
                    : 데이터가 들어올 때까지 기다렸다가 하나 읽는다.

                  - TryRead(out value)
                    : 즉시 읽을 수 있으면 true, 아니면 false.

                  - WaitToReadAsync()
                    : 읽을 데이터가 생기거나 Channel 이 닫힐 때까지 기다린다.

                  - ReadAllAsync()
                    : Channel 이 Complete 될 때까지 모든 데이터를 async stream 으로 읽는다.

              5. BoundedChannelFullMode

                Bounded Channel 이 가득 찼을 때 어떻게 처리할지 정하는 옵션이다.

                | 모드             | 의미
                |------------------|---------------------------------------------------------
                | Wait             | 공간이 생길 때까지 Writer 가 기다린다. 가장 일반적.
                | DropNewest       | 가장 최근에 들어온 데이터를 버린다.
                | DropOldest       | 가장 오래된 데이터를 버린다.
                | DropWrite        | 지금 쓰려는 데이터를 버린다.

                서버에서 안정적인 메시지 처리가 중요하면 보통 Wait 를 사용한다.
                로그, 위치 갱신, 상태 스냅샷처럼 일부 유실이 허용되면 DropOldest 등을 고려할 수 있다.

              6. lock / ConcurrentQueue 와의 차이

                ConcurrentQueue<T>
                  - 단순한 스레드 안전 큐.
                  - 데이터가 없을 때 기다리는 기능은 직접 구현해야 한다.
                  - SemaphoreSlim, AutoResetEvent 등을 함께 써야 하는 경우가 많다.

                Channel<T>
                  - 큐 + 비동기 대기 + 완료 신호 + 백프레셔를 함께 제공한다.
                  - async/await 기반 서버 코드와 잘 맞는다.
                  - Producer / Consumer 구조를 깔끔하게 만들 수 있다.

              7. 사용 시 주의점

                - Complete()를 호출하지 않으면 ReadAllAsync() 루프가 끝나지 않는다.
                - Unbounded Channel 은 생산 속도가 소비 속도보다 빠르면 메모리가 계속 증가할 수 있다.
                - SingleReader / SingleWriter 옵션을 정확히 지정하면 내부 최적화에 도움이 된다.
                - Channel 자체는 메시지 처리 순서를 보장하지만,
                  여러 Consumer 가 동시에 읽으면 처리 완료 순서는 달라질 수 있다.
                - Actor / Entity Mailbox 처럼 "순차 처리"가 중요하면 Consumer 를 1개만 둬야 한다.

              8. Channel 진입/처리 구조

                Producer 1개 이상
                  ┌─────────────┐
                  │        WriteAsync        │
                  └──────┬──────┘
                                ↓
                  ┌─────────────┐
                  │        Channel<T>        │
                  │          Queue           │
                  └──────┬──────┘
                                ↓
                  ┌─────────────┐
                  │         ReadAsync        │
                  └─────────────┘
                Consumer 1개 이상

              9. 에플리케이션 설계 관점에서의 핵심

                - Actor 역할을 하는 클래스 마다 Channel을 맴버로 두고 MailBox와 같은 역할을 할 수 있다.
                - 외부 스레드는 메시지를 Channel 에 넣기만 한다.
                - 해당 Actor의 처리 루프 하나가 메시지를 순서대로 읽고 처리한다.
                - 그러면 Actor 내부 상태는 lock 없이도 안전하게 다룰 수 있다.
                - 단, Actor 하나당 Consumer 는 반드시 1개여야 순서와 단일 실행이 보장된다.
        */
        {
            Channel<int> channel = System.Threading.Channels.Channel.CreateUnbounded<int>();

            Console.WriteLine($"[Main] Start ThreadId = {Environment.CurrentManagedThreadId}");

            // Producer
            Task producer = Task.Run(async () =>
            {
                Console.WriteLine($"[Producer] Task 시작 ThreadId = {Environment.CurrentManagedThreadId}");

                for (int i = 1; i <= 5; i++)
                {
                    Console.WriteLine($"[Producer] WriteAsync 전  Value = {i}, ThreadId = {Environment.CurrentManagedThreadId}");
                    await channel.Writer.WriteAsync(i); // 함수 진입 / 종료시 스레드가 변경될 수 있다. (기본 TaskScheduler 사용)
                    Console.WriteLine($"[Producer] WriteAsync 후  Value = {i}, ThreadId = {Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Write: {i}");

                    Console.WriteLine($"[Producer] Delay 전       Value = {i}, ThreadId = {Environment.CurrentManagedThreadId}");
                    await Task.Delay(100); // 함수 진입 / 종료시 스레드가 변경될 수 있다.
                    Console.WriteLine($"[Producer] Delay 후       Value = {i}, ThreadId = {Environment.CurrentManagedThreadId}");
                }

                Console.WriteLine($"[Producer] Complete 전 ThreadId = {Environment.CurrentManagedThreadId}");
                channel.Writer.Complete();
                Console.WriteLine($"[Producer] Complete 후 ThreadId = {Environment.CurrentManagedThreadId}");
            });

            // Consumer
            Task consumer = Task.Run(async () =>
            {
                Console.WriteLine($"[Consumer] Task 시작 ThreadId = {Environment.CurrentManagedThreadId}");

                await foreach (int value in channel.Reader.ReadAllAsync()) // 값을 꺼낼때 마다 스레드가 변경될 수 있다. (기본 TaskScheduler 사용)
                {
                    Console.WriteLine($"[Consumer] Read value = {value}, ThreadId = {Environment.CurrentManagedThreadId}");
                    Console.WriteLine($"Read : {value}");
                }

                Console.WriteLine($"[Consumer] ReadAllAsync 종료 ThreadId = {Environment.CurrentManagedThreadId}");
                Console.WriteLine("Channel completed.");
            });

            Console.WriteLine($"[Main] WhenAll 전 ThreadId = {Environment.CurrentManagedThreadId}");
            System.Threading.Tasks.Task.WhenAll(producer, consumer).Wait();
            Console.WriteLine($"[Main] WhenAll 후 ThreadId = {Environment.CurrentManagedThreadId}");

            Console.ReadLine();
        }
    }

    //---------------------------------------------------------------------------------------------

    static void use_BoundedChannel()
    {
        /*
            📚 Bounded Channel 예제

              1. 개요
                - 크기 제한이 있는 Channel을 사용하는 예제.
                - Producer는 데이터를 빠르게 넣으려고 하고,
                  Consumer는 데이터를 느리게 꺼내도록 만들어서
                  Channel이 가득 찼을 때 Writer가 어떻게 대기하는지 확인한다.

              2. Channel 설정

                capacity: 3

                - Channel 내부에 최대 3개까지만 데이터를 저장할 수 있다.
                - Consumer가 데이터를 꺼내지 않으면 4번째 Write부터는 대기할 수 있다.

                FullMode = BoundedChannelFullMode.Wait

                - Channel이 가득 찼을 때 데이터를 버리지 않는다.
                - 공간이 생길 때까지 Writer가 기다린다.
                - 가장 안전한 방식이다.
                - 게임 서버의 중요한 메시지 큐, DB 저장 큐 등에 적합하다.

                SingleWriter = false

                - Writer가 여러 개일 수 있다는 의미.
                - 여러 Task / 여러 Thread에서 동시에 Write할 수 있다.

                SingleReader = true

                - Reader가 하나라는 의미.
                - 하나의 Consumer가 순차적으로 데이터를 처리하는 구조에 적합하다.
                - Actor / Mailbox / Zone Loop 구조에 잘 맞는다.

              3. Producer 흐름

                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine($"Write waiting: {i}");

                    await channel.Writer.WriteAsync(i);

                    Console.WriteLine($"Write done   : {i}");
                }

                - WriteAsync(i)는 데이터를 Channel에 넣는다.
                - Channel에 공간이 있으면 즉시 완료된다.
                - Channel이 가득 차 있으면 공간이 생길 때까지 await에서 대기한다.
                - 대기가 끝나고 실제로 쓰기에 성공하면 "Write done"이 출력된다.

              4. Consumer 흐름

                await foreach (int value in channel.Reader.ReadAllAsync())
                {
                    Console.WriteLine($"Read         : {value}");
                    await Task.Delay(500);
                }

                - Channel에서 값을 하나씩 읽는다.
                - 값을 읽은 뒤 500ms 동안 대기한다.
                - 일부러 느리게 소비하므로 Producer가 중간에 막히는 상황을 만들 수 있다.

              5. Backpressure

                - Producer가 Consumer보다 빠르면 Channel에 데이터가 쌓인다.
                - capacity 3개가 모두 차면 Producer의 WriteAsync가 대기한다.
                - Consumer가 하나를 읽어서 공간을 만들면 Producer가 다시 진행된다.

                즉, Consumer의 처리 속도가 Producer의 쓰기 속도를 제어하게 된다.

              6. Complete()

                channel.Writer.Complete();

                - 더 이상 데이터를 쓰지 않겠다고 Reader에게 알린다.
                - Complete()가 호출되고, Channel 안의 데이터가 모두 소비되면
                  ReadAllAsync() 루프가 정상 종료된다.

                Complete()를 호출하지 않으면 ReadAllAsync()는
                새로운 데이터가 들어올 가능성이 있다고 보고 계속 기다릴 수 있다.

              7. 이 예제에서 관찰할 점

                - 처음 몇 개는 빠르게 Write done이 찍힌다.
                - Consumer가 느리기 때문에 Channel이 곧 가득 찬다.
                - 이후 Producer는 WriteAsync에서 대기한다.
                - Consumer가 Read를 한 번 할 때마다 Producer가 하나씩 더 진행된다.
        */

        var options = new BoundedChannelOptions(capacity: 3)
        {
            FullMode = BoundedChannelFullMode.Wait,

            // Writer 가 여러 개면 false
            SingleWriter = false,

            // Reader 가 하나면 true
            SingleReader = true
        };

        var channel = System.Threading.Channels.Channel.CreateBounded<int>(options);

        Task producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"Write waiting: {i}");

                await channel.Writer.WriteAsync(i);

                Console.WriteLine($"Write done   : {i}");
            }

            channel.Writer.Complete();
        });

        var consumer = Task.Run(async () =>
        {
            await foreach (int value in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Read         : {value}");

                // 일부러 느리게 소비
                await Task.Delay(500);
            }
        });

        System.Threading.Tasks.Task.WhenAll(producer, consumer).Wait();

        Console.ReadLine();
    }

    //---------------------------------------------------------------------------------------------

    static void use_TryChannel()
    {
        /*
            📚 TryWrite / TryRead

              1. 개요
                - Channel에 데이터를 "기다리지 않고" 넣거나 꺼내는 예제.
                - TryWrite()는 즉시 쓸 수 있으면 true, 못 쓰면 false를 반환한다.
                - TryRead()는 즉시 읽을 수 있으면 true, 읽을 데이터가 없으면 false를 반환한다.

              2. 핵심 특징
                - await를 사용하지 않는다.
                - 대기하지 않는다.
                - 현재 시점에 가능하면 성공하고, 불가능하면 바로 실패한다.

              3. 쓰기 흐름
                3.1. Channel 설정
                  - Bounded Channel
                  - capacity = 3
                  - 최대 3개까지만 내부 큐에 저장 가능

                3.2. 데이터 설정
                  - TryWrite(10) → 성공
                  - TryWrite(20) → 성공
                  - TryWrite(30) → 성공
                  - TryWrite(40) → 실패

                3.3. 이유:
                  - capacity가 3이므로 10, 20, 30까지 들어가면 Channel이 가득 찬다.
                  - 40을 넣을 때는 공간이 없고, TryWrite()는 기다리지 않으므로 false를 반환한다.

              4. 읽기 흐름
                - TryRead(out value)를 반복하면서 현재 Channel에 들어 있는 값을 모두 꺼낸다.
                - 10, 20, 30이 순서대로 읽힌다.
                - 더 이상 읽을 값이 없으면 TryRead는 false를 반환하고 while문이 종료된다.

              5. 주의점
                - TryWrite()는 실패할 수 있다.
                - 실패한 값은 Channel에 들어가지 않는다.
                - 이 예제에서 40은 저장되지 않는다.
                - TryRead()는 데이터가 들어올 때까지 기다리지 않는다.
                - 읽을 값이 없으면 즉시 false를 반환한다.

              6. 언제 사용하나?
                - 대기하면 안 되는 상황
                - 빠르게 성공/실패 여부만 확인하고 싶은 상황
                - 로그, 위치 갱신, 상태 갱신처럼 일부 데이터 유실이 허용되는 상황
                - 게임 서버에서 프레임/tick 안에 즉시 처리 가능한 만큼만 처리하고 싶을 때
        */


        var channel = System.Threading.Channels.Channel.CreateBounded<int>(3);

        bool write1 = channel.Writer.TryWrite(10);
        bool write2 = channel.Writer.TryWrite(20);
        bool write3 = channel.Writer.TryWrite(30);
        bool write4 = channel.Writer.TryWrite(40);

        Console.WriteLine($"Write 10: {write1}");
        Console.WriteLine($"Write 20: {write2}");
        Console.WriteLine($"Write 30: {write3}");
        Console.WriteLine($"Write 40: {write4}");

        while (channel.Reader.TryRead(out int value))
        {
            Console.WriteLine($"Read: {value}");
        }

        /*
            Write 10: True
            Write 20: True
            Write 30: True
            Write 40: False
            Read: 10
            Read: 20
            Read: 30
        */

        Console.ReadLine();
    }

    //---------------------------------------------------------------------------------------------

    public interface IActorMessage
    {
    }

    public sealed class MoveMessage : IActorMessage
    {
        public int X { get; }
        public int Y { get; }

        public MoveMessage(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public sealed class AttackMessage : IActorMessage
    {
        public long TargetId { get; }

        public AttackMessage(long targetId)
        {
            TargetId = targetId;
        }
    }

    private static void HandleMessage(IActorMessage message)
    {
        switch (message)
        {
            case MoveMessage move:
                Console.WriteLine(
                    $"[Handle] Move X = {move.X}, Y = {move.Y}, " +
                    $"ThreadId = {Environment.CurrentManagedThreadId}");
                break;

            case AttackMessage attack:
                Console.WriteLine(
                    $"[Handle] Attack TargetId = {attack.TargetId}, " +
                    $"ThreadId = {Environment.CurrentManagedThreadId}");
                break;

            default:
                Console.WriteLine(
                    $"[Handle] Unknown Message = {message.GetType().Name}, " +
                    $"ThreadId = {Environment.CurrentManagedThreadId}");
                break;
        }
    }

    private static async Task runNormalLoopAsync(Channel<IActorMessage> channel)
    {
        /*
            📌 일반 Loop

              - WaitToReadAsync()로 읽을 수 있는 데이터가 생길 때까지 대기한다.
              - 데이터가 있으면 TryRead()로 현재 쌓여 있는 메시지를 모두 처리한다.
              - Consumer는 1개이므로 Actor 내부 상태는 순차적으로 처리된다.
              - await 이후 동일 스레드에서 재개된다는 보장은 없다.
        */

        while (await channel.Reader.WaitToReadAsync())
        {
            Console.WriteLine($"[runNormalLoopAsync] Wake Up ThreadId = {Environment.CurrentManagedThreadId}");

            while (channel.Reader.TryRead(out IActorMessage? message))
            {
                HandleMessage(message);
            }
        }

        Console.WriteLine($"[runNormalLoopAsync] Completed ThreadId = {Environment.CurrentManagedThreadId}");
    }

    private static async Task normalLoop_with_Channel()
    {
        Channel<IActorMessage> channel = System.Threading.Channels.Channel.CreateBounded<IActorMessage>(
            new BoundedChannelOptions(capacity: 1024)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

        Task consumer = Task.Run(() => runNormalLoopAsync(channel));

        Console.WriteLine($"[Main] ThreadId = {Environment.CurrentManagedThreadId}");

        await channel.Writer.WriteAsync(new MoveMessage(10, 20));
        await channel.Writer.WriteAsync(new AttackMessage(10001));
        await channel.Writer.WriteAsync(new MoveMessage(30, 40));
        await channel.Writer.WriteAsync(new AttackMessage(10002));
        await channel.Writer.WriteAsync(new MoveMessage(50, 60));

        channel.Writer.Complete();

        await consumer;
    }

    private static async Task runBatchLoopAsync(
        Channel<IActorMessage> channel,
        int maxBatchSize,
        CancellationToken cancellationToken = default)
    {
        /*
            📌 Batch Loop

              - WaitToReadAsync()로 읽을 수 있는 데이터가 생길 때까지 대기한다.
              - 한 번 깨어나면 최대 maxBatchSize 개수까지만 처리한다.
              - Batch 내부에서는 await 없이 TryRead()만 반복하므로
                보통 같은 ThreadId에서 연속 처리된다.
              - Batch가 끝나면 다시 WaitToReadAsync()로 돌아가므로
                다음 Batch는 다른 ThreadPool 스레드에서 실행될 수 있다.
              - 한 Actor가 너무 많은 메시지를 독점 처리하지 않게 할 수 있다.
        */

        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            int processed = 0;

            Console.WriteLine();
            Console.WriteLine($"[runBatchLoopAsync] Batch Start ThreadId = {Environment.CurrentManagedThreadId}");

            while (processed < maxBatchSize &&
                   channel.Reader.TryRead(out IActorMessage? message))
            {
                HandleMessage(message);

                processed++;
            }

            Console.WriteLine($"[runBatchLoopAsync] Batch End Count = {processed}, ThreadId = {Environment.CurrentManagedThreadId}");
        }

        Console.WriteLine($"[runBatchLoopAsync] Completed ThreadId = {Environment.CurrentManagedThreadId}");
    }

    private static async Task batchLoop_with_Channel()
    {
        Channel<IActorMessage> channel = System.Threading.Channels.Channel.CreateBounded<IActorMessage>(
            new BoundedChannelOptions(capacity: 1024)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

        using CancellationTokenSource cts = new CancellationTokenSource();

        Task consumer = Task.Run(() =>
            runBatchLoopAsync(
                channel,
                maxBatchSize: 3,
                cancellationToken: cts.Token));

        Console.WriteLine($"[Main] ThreadId = {Environment.CurrentManagedThreadId}");

        for (int i = 1; i <= 10; i++)
        {
            if (i % 2 == 0)
            {
                await channel.Writer.WriteAsync(new AttackMessage(10000 + i));
            }
            else
            {
                await channel.Writer.WriteAsync(new MoveMessage(i * 10, i * 10 + 1));
            }

            Console.WriteLine($"[Producer] Write message {i}, ThreadId = {Environment.CurrentManagedThreadId}");
        }

        channel.Writer.Complete();

        await consumer;
    }

    public static void loop_with_Channel()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Test 1. Normal Loop");
        Console.WriteLine("========================================");

        normalLoop_with_Channel().Wait();

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("Test 2. Batch Loop");
        Console.WriteLine("========================================");

        batchLoop_with_Channel().Wait();

        Console.ReadLine();
    }

    //---------------------------------------------------------------------------------------------

    public sealed class ActorInbox
    {
        private readonly Channel<IActorMessage> _channel;

        public ActorInbox(int capacity = 1024)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,

                // 여러 네트워크 스레드 / 시스템에서 메시지를 넣을 수 있음
                SingleWriter = false,

                // Actor 하나가 순차 처리
                SingleReader = true
            };

            _channel = System.Threading.Channels.Channel.CreateBounded<IActorMessage>(options);
        }

        public ValueTask EnqueueAsync(IActorMessage message, CancellationToken cancellationToken = default)
        {
            return _channel.Writer.WriteAsync(message, cancellationToken);
        }

        public bool TryEnqueue(IActorMessage message)
        {
            return _channel.Writer.TryWrite(message);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await foreach (IActorMessage message in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    HandleMessage(message);
                }
            }
            catch (OperationCanceledException)
            {
                // 서버 종료 또는 Actor 종료
            }
        }

        private void HandleMessage(IActorMessage message)
        {
            switch (message)
            {
                case MoveMessage move:
                    Console.WriteLine($"Move: {move.X}, {move.Y}");
                    break;

                case AttackMessage attack:
                    Console.WriteLine($"Attack Target: {attack.TargetId}");
                    break;

                default:
                    Console.WriteLine($"Unknown message: {message.GetType().Name}");
                    break;
            }
        }

        public void Complete()
        {
            _channel.Writer.TryComplete();
        }
    }

    public static void like_SimpleMailBox()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();

        var inbox = new ActorInbox();

        Task actorLoop = inbox.RunAsync(cts.Token);

        inbox.EnqueueAsync(new MoveMessage(10, 20));
        inbox.EnqueueAsync(new AttackMessage(10001));
        inbox.EnqueueAsync(new MoveMessage(15, 25));

        inbox.Complete();

        actorLoop.Wait();

        Console.ReadLine();
    }

    //---------------------------------------------------------------------------------------------

    public sealed class LogicTask
    {
        private readonly Channel<Func<ValueTask>> _channel;
        private readonly int _maxBatchSize;

        public LogicTask(
            int capacity = 4096,
            int maxBatchSize = 64)
        {
            _maxBatchSize = maxBatchSize;

            _channel = System.Threading.Channels.Channel.CreateBounded<Func<ValueTask>>(
                new BoundedChannelOptions(capacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
        }

        public ValueTask PostAsync(
            Func<ValueTask> action,
            CancellationToken cancellationToken = default)
        {
            return _channel.Writer.WriteAsync(action, cancellationToken);
        }

        public bool TryPost(Func<ValueTask> action)
        {
            return _channel.Writer.TryWrite(action);
        }

        public async Task runLoopAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                /*
                    📌 Batch Read 처리

                      - WaitToReadAsync()로 읽을 데이터가 생길 때까지 대기한다.
                      - 한 번 깨어나면 TryRead()로 최대 _maxBatchSize 개까지 연속 처리한다.
                      - Batch 내부에서는 Channel Read 쪽 await가 없으므로,
                        보통 같은 ThreadPool 스레드에서 연속 처리된다.
                      - 단, action() 내부에서 await가 발생하면
                        이후 continuation은 다른 스레드에서 재개될 수 있다.
                      - Batch가 끝나면 다시 WaitToReadAsync()로 돌아간다.
                      - 너무 많은 작업을 한 번에 독점 처리하지 않도록 maxBatchSize로 제한한다.
                */

                while (await _channel.Reader.WaitToReadAsync(cancellationToken))
                {
                    int processed = 0;

                    Console.WriteLine();
                    Console.WriteLine(
                        $"[LogicTask] Batch Start ThreadId = {Environment.CurrentManagedThreadId}");

                    while (processed < _maxBatchSize &&
                           _channel.Reader.TryRead(out Func<ValueTask>? action))
                    {
                        try
                        {
                            Console.WriteLine(
                                $"[LogicTask] Execute Job BatchIndex = {processed}, ThreadId = {Environment.CurrentManagedThreadId}");

                            await action();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"LogicTask error: {ex}");
                        }

                        processed++;
                    }

                    Console.WriteLine(
                        $"[LogicTask] Batch End Count = {processed}, ThreadId = {Environment.CurrentManagedThreadId}");
                }

                Console.WriteLine(
                    $"[LogicTask] Completed ThreadId = {Environment.CurrentManagedThreadId}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(
                    $"[LogicTask] Canceled ThreadId = {Environment.CurrentManagedThreadId}");
            }
        }

        public void Complete()
        {
            _channel.Writer.TryComplete();
        }
    }

    public static void like_MailBox()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();

        LogicTask logicTask = new LogicTask(
            capacity: 4096,
            maxBatchSize: 3);

        Task loop = logicTask.runLoopAsync(cts.Token);

        _ = logicTask.PostAsync(() =>
        {
            Console.WriteLine($"Job 1, ThreadId = {Environment.CurrentManagedThreadId}");
            return ValueTask.CompletedTask;
        });

        _ = logicTask.PostAsync(() =>
        {
            Console.WriteLine($"Job 2, ThreadId = {Environment.CurrentManagedThreadId}");
            return ValueTask.CompletedTask;
        });

        _ = logicTask.PostAsync(() =>
        {
            Console.WriteLine($"Job 3, ThreadId = {Environment.CurrentManagedThreadId}");
            return ValueTask.CompletedTask;
        });

        _ = logicTask.PostAsync(() =>
        {
            Console.WriteLine($"Job 4, ThreadId = {Environment.CurrentManagedThreadId}");
            return ValueTask.CompletedTask;
        });

        _ = logicTask.PostAsync(() =>
        {
            Console.WriteLine($"Job 5, ThreadId = {Environment.CurrentManagedThreadId}");
            return ValueTask.CompletedTask;
        });

        logicTask.Complete();

        loop.Wait();

        Console.ReadLine();
    }

    //---------------------------------------------------------------------------------------------

    /*
        SingleThreadTaskScheduler 재정의

          - async/await를 유지하면서도 continuation을 특정 스레드로 복귀시킬 수 있다.
          - action 내부에서 await가 발생해도 await 이후 코드는 다시 전용 스케줄러 스레드에서 실행된다.
          - 단, ConfigureAwait(false), Task.Run 내부 실행, blocking wait 사용에는 주의해야 한다.
          - MailBox를 처리하는 ActorExecutor 구조에 사용할 수 있다.
    */
    public sealed class SingleThreadTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> _tasks = new();
        private readonly Thread _thread;

        public int ThreadId { get; private set; }

        public SingleThreadTaskScheduler(string name = "SingleThreadTaskScheduler")
        {
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = name
            };

            _thread.Start();
        }

        private void Run()
        {
            ThreadId = Environment.CurrentManagedThreadId;

            Console.WriteLine($"[Scheduler] Start ThreadId = {ThreadId}");

            foreach (Task task in _tasks.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }

            Console.WriteLine($"[Scheduler] End ThreadId = {Environment.CurrentManagedThreadId}");
        }

        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            return _tasks.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            /*
                같은 전용 스레드에서 요청된 경우에만 inline 실행 허용.
                다른 스레드에서 inline 실행되면 단일 스레드 보장이 깨질 수 있다.
            */
            if (Environment.CurrentManagedThreadId == ThreadId)
            {
                return TryExecuteTask(task);
            }

            return false;
        }

        public void Dispose()
        {
            _tasks.CompleteAdding();
        }
    }

    public sealed class LogicTaskWithChannel
    {
        private readonly System.Threading.Channels.Channel<Func<ValueTask<int>>> _channel;
        private readonly int _maxBatchSize;

        public LogicTaskWithChannel(
            int capacity = 4096,
            int maxBatchSize = 64)
        {
            _maxBatchSize = maxBatchSize;

            _channel = System.Threading.Channels.Channel.CreateBounded<Func<ValueTask<int>>>(
                new BoundedChannelOptions(capacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
        }

        public ValueTask PostAsync(
            Func<ValueTask<int>> action,
            CancellationToken cancellationToken = default)
        {
            return _channel.Writer.WriteAsync(action, cancellationToken);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine($"[LogicTaskWithChannel] Run Start ThreadId = {Environment.CurrentManagedThreadId}");

                while (await _channel.Reader.WaitToReadAsync(cancellationToken))
                {
                    int processed = 0;

                    Console.WriteLine();
                    Console.WriteLine($"[LogicTaskWithChannel] Batch Start ThreadId = {Environment.CurrentManagedThreadId}");

                    while (processed < _maxBatchSize &&
                           _channel.Reader.TryRead(out Func<ValueTask<int>>? action))
                    {
                        try
                        {
                            Console.WriteLine($"[LogicTaskWithChannel] Action Before ThreadId = {Environment.CurrentManagedThreadId}");

                            int result = await action();

                            Console.WriteLine($"[LogicTaskWithChannel] Action Result = {result}, ThreadId = {Environment.CurrentManagedThreadId}");
                            Console.WriteLine($"[LogicTaskWithChannel] Action After  ThreadId = {Environment.CurrentManagedThreadId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"LogicTaskWithChannel error: {ex}");
                        }

                        processed++;
                    }

                    Console.WriteLine($"[LogicTaskWithChannel] Batch End Count = {processed}, ThreadId = {Environment.CurrentManagedThreadId}");
                }

                Console.WriteLine($"[LogicTaskWithChannel] Run End ThreadId = {Environment.CurrentManagedThreadId}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[LogicTaskWithChannel] Canceled ThreadId = {Environment.CurrentManagedThreadId}");
            }
        }

        public void Complete()
        {
            _channel.Writer.TryComplete();
        }
    }

    static void like_MailBox_with_CustomTaskScheduler()
    {
        using var scheduler = new SingleThreadTaskScheduler("SingleThreadTaskScheduler");

        Console.WriteLine($"SingleThreadTaskScheduler Start ThreadId = {Environment.CurrentManagedThreadId}");

        var logicTask = new LogicTaskWithChannel(
            capacity: 4096,
            maxBatchSize: 3);

        var logicLoop = System.Threading.Tasks.Task.Factory.StartNew(
            async () =>
            {
                await logicTask.RunAsync();
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            scheduler).Unwrap();

        logicTask.PostAsync(async () =>
        {
            Console.WriteLine($"Job 1 Start ThreadId = {Environment.CurrentManagedThreadId}");

            await Task.Delay(300);

            Console.WriteLine($"Job 1 After Delay ThreadId = {Environment.CurrentManagedThreadId}");

            return 100;
        });

        logicTask.PostAsync(async () =>
        {
            Console.WriteLine($"Job 2 Start ThreadId = {Environment.CurrentManagedThreadId}");

            await Task.Delay(300);

            Console.WriteLine($"Job 2 After Delay ThreadId = {Environment.CurrentManagedThreadId}");

            return 200;
        });

        logicTask.Complete();

        logicLoop.Wait();

        Console.WriteLine($"SingleThreadTaskScheduler End ThreadId = {Environment.CurrentManagedThreadId}");

        Console.ReadLine();
    }

    public static void Test()
    {
        //like_MailBox_with_CustomTaskScheduler();

        //like_MailBox();

        //like_SimpleMailBox();

        //loop_with_Channel();

        //use_TryChannel();

        //use_BoundedChannel();

        //Channel_what();
    }
}

