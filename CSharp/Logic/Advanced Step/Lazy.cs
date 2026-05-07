using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AdvancedStep.Lazy;


namespace AdvancedStep
{
    public class Lazy
    {
        public sealed class HeavyService
        {
            public HeavyService()
            {
                Console.WriteLine("HeavyService 생성됨");
            }

            public void DoWork()
            {
                Console.WriteLine("HeavyService 작업 실행");
            }
        }

        static void Lazy_what()
        {
            /*
                📚 Lazy<T>

                  1. 개요
                    - 객체 생성을 "실제로 필요할 때까지" 미루기 위한 클래스.
                    - 이를 지연 초기화(Lazy Initialization)라고 한다.
                    - 무거운 객체, 설정 로딩, 캐시 생성, 싱글톤 객체 생성 등에 자주 사용된다.
                    - Lazy<T>는 .NET Framework 4.0 부터 추가되었다.
                    - 이후 .NET Core, .NET 5 이상 사용 가능하다.

                    * 예를 들어 다음과 같은 객체에 적합하다.
                      - DB 연결 설정
                      - 대용량 캐시
                      - 설정 파일 파싱 결과
                      - 메타 데이터 테이블
                      - 싱글톤 서비스
                      - 각종 시스템의 정책 데이터

                  2. 기본 개념

                    Lazy<T> lazy = new Lazy<T>(() => new T());

                    - Lazy<T> 객체를 생성했다고 해서 T 객체가 바로 생성되는 것은 아니다.
                    - lazy.Value 에 처음 접근하는 순간 실제 T 객체가 생성된다.
                    - 이후 lazy.Value 에 다시 접근하면 처음 생성된 객체를 계속 재사용한다.

                  3. 대표 속성

                    - Value
                      : 실제 객체를 반환한다.
                      : 최초 접근 시 객체를 생성한다.

                    - IsValueCreated
                      : 실제 객체가 이미 생성되었는지 확인한다.

                  4. 기본 사용 예

                    Lazy<MyService> service = new Lazy<MyService>(() => new MyService());

                    // 아직 MyService 객체는 생성되지 않음

                    MyService instance = service.Value;

                    // 이 시점에 MyService 객체가 생성됨

                  5. Lazy<T>의 장점

                    - 필요 없는 객체를 미리 만들지 않아도 된다.
                    - 초기 구동 시간을 줄일 수 있다.
                    - 무거운 객체 생성을 필요한 시점까지 미룰 수 있다.
                    - 싱글톤 초기화를 간단하게 만들 수 있다.
                    - 기본적으로 스레드 안전한 초기화를 지원한다.

                  6. 스레드 안전성

                    Lazy<T>는 기본적으로 Thread-Safe 하다.

                    즉, 여러 스레드가 동시에 Value 에 접근하더라도
                    객체는 한 번만 생성된다.

                    기본 모드는 다음과 같다.

                      LazyThreadSafetyMode.ExecutionAndPublication

                    이 모드에서는 여러 스레드가 동시에 접근해도
                    실제 객체 생성은 단 한 번만 성공적으로 수행된다.

                    7. LazyThreadSafetyMode 종류

                    | 모드                    | 의미
                    |-------------------------|---------------------------------------------------------
                    | ExecutionAndPublication | 기본값. 하나의 스레드만 생성 실행. 가장 안전하고 일반적.
                    | PublicationOnly         | 여러 스레드가 동시에 생성할 수 있지만, 그중 하나만 최종 사용됨.
                    | None                    | 스레드 안전성 없음. 단일 스레드 환경에서만 사용 권장.

                  8. 예외 처리 특징

                    - 기본 모드인 ExecutionAndPublication 에서는
                      Value 생성 중 예외가 발생하면 그 예외가 캐싱된다.

                    - 즉, 첫 번째 Value 접근에서 예외가 발생하면
                      이후 다시 Value 에 접근해도 같은 예외가 다시 발생한다.

                    - 재시도를 원한다면 Lazy<T>를 새로 만들어야 한다.

                  9. 사용시 주의점

                    - Lazy<T> 자체는 지연 초기화 도구이지, 객체의 생명주기 관리 도구는 아니다.
                    - IDisposable 객체를 Lazy<T>로 만들었다면 Dispose 처리는 직접 해야 한다.
                    - Value 접근 시점에 생성 비용이 몰릴 수 있다.
                    - 생성 함수 안에서 다시 자기 자신의 Value 에 접근하면 순환 참조 문제가 생길 수 있다.
                    - 실패 후 재시도가 필요한 로직에는 기본 Lazy<T>가 적합하지 않을 수 있다.
            */
            {
                var lazyService = new Lazy<HeavyService>(() =>
                {
                    return new HeavyService();
                });

                Console.WriteLine("Lazy 객체 생성 완료");
                Console.WriteLine($"IsValueCreated: {lazyService.IsValueCreated}");

                HeavyService service = lazyService.Value; // 실제 최초 접근할때 생성 !!!

                Console.WriteLine($"IsValueCreated: {lazyService.IsValueCreated}");

                service.DoWork();

                HeavyService sameService = lazyService.Value;

                Console.WriteLine($"Same instance: {ReferenceEquals(service, sameService)}");

                /*
                    Lazy 객체 생성 완료
                    IsValueCreated: False
                    HeavyService 생성됨
                    IsValueCreated: True
                    HeavyService 작업 실행
                    Same instance: True                 
                */

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        public sealed class ConfigManager
        {
            private static readonly Lazy<ConfigManager> _instance =
                new Lazy<ConfigManager>(() => new ConfigManager());

            public static ConfigManager Instance => _instance.Value;

            private ConfigManager()
            {
                Console.WriteLine("ConfigManager 생성");
            }

            public string GetValue(string key)
            {
                return $"Config value for {key}";
            }
        }

        static void Lazy_with_singleton()
        {
            /*
                📌 Lazy + singleton

                  - Instance 에 처음 접근할 때까지 객체가 생성되지 않는다.
                  - 기본적으로 스레드 안전하다.
                  - lock 코드를 직접 작성하지 않아도 된다.
                  - static 생성자보다 생성 시점을 더 명확하게 제어할 수 있다.
            */

            Console.WriteLine("에플리케이션 Start !!!");

            string value = ConfigManager.Instance.GetValue("Port");

            Console.WriteLine(value);
        }

        //-----------------------------------------------------------------------------------------

        public sealed class MetaDataStore
        {
            private readonly Dictionary<int, string> _datas;

            public MetaDataStore()
            {
                Console.WriteLine("메타데이터 로딩 시작");

                _datas = new Dictionary<int, string>
                {
                    [1001] = "Data 1",
                    [1002] = "Data 2",
                    [1003] = "Data 3"
                };

                Console.WriteLine("메타데이터 로딩 완료");
            }

            public string GetData(int metaId)
            {
                return _datas.TryGetValue(metaId, out string name)
                    ? name
                    : "Unknown";
            }
        }

        public static class Metadata
        {
            private static readonly Lazy<MetaDataStore> _dataStore =
                new Lazy<MetaDataStore>(() => new MetaDataStore());

            public static MetaDataStore DataStore => _dataStore.Value;
        }

        static void metadataStore_with_Lazy()
        {
            /*
                📚 Lazy<T> + Metadata Store

                  1. 개요
                    - Lazy<T>를 사용해서 메타데이터 저장소 생성을 실제 사용 시점까지 미루는 예제다.
                    - Lazy<T>는 .NET Framework 4.0부터 사용할 수 있다.
                    - .NET Core, .NET 5 이상에서도 사용할 수 있다.
                    - 메타데이터 로딩 비용이 크거나, 특정 기능을 사용할 때만 필요한 데이터 저장소에 적합하다.
                    - 게임 서버에서는 스킬 데이터, 아이템 데이터, 몬스터 데이터, 설정 캐시 같은 읽기 전용 데이터 저장소에 사용할 수 있다.

                  2. 기본 개념
                    - MetaDataStore는 실제 메타데이터를 보관하는 저장소 클래스다.
                    - Metadata는 MetaDataStore에 접근하기 위한 static 진입점 역할을 한다.
                    - Lazy<MetaDataStore>는 MetaDataStore 인스턴스를 즉시 생성하지 않고 보관한다.
                    - _dataStore.Value에 처음 접근하는 순간 new MetaDataStore()가 실행된다.
                    - 이후 _dataStore.Value에 다시 접근하면 처음 생성된 같은 인스턴스를 반환한다.

                    핵심 구조:

                      private static readonly Lazy<MetaDataStore> _dataStore =
                          new Lazy<MetaDataStore>(() => new MetaDataStore());

                      public static MetaDataStore DataStore => _dataStore.Value;

                  3. 핵심 특징
                    - Lazy<T> 객체를 생성해도 내부 T 객체가 즉시 생성되지는 않는다.
                    - Value 프로퍼티에 처음 접근할 때 내부 객체가 생성된다.
                    - 한 번 생성된 객체는 계속 재사용된다.
                    - 기본 Lazy<T>는 Thread-Safe하다.
                    - 여러 스레드가 동시에 Value에 접근해도 기본 모드에서는 하나의 인스턴스만 생성된다.
                    - 생성자에서 예외가 발생하면 기본 모드에서는 예외가 캐싱될 수 있다.

                  4. 실행 흐름
                    - metadataStore_with_Lazy() 호출
                    - "메타데이터 시작" 출력
                    - 이 시점에는 아직 Metadata.DataStore에 접근하지 않았으므로 MetaDataStore는 생성되지 않는다.
                    - Metadata.DataStore.GetData(1001) 호출
                    - Metadata.DataStore 프로퍼티가 _dataStore.Value에 접근
                    - Lazy<T>가 처음으로 new MetaDataStore() 실행
                    - MetaDataStore 생성자에서 Dictionary<int, string> 초기화
                    - GetData(1001) 실행
                    - 1001에 해당하는 "Data 1" 반환
                    - "Data 1" 출력

                  5. 대표 메서드 또는 주요 코드
                    - new Lazy<MetaDataStore>(() => new MetaDataStore())
                      : MetaDataStore 생성 로직을 Lazy<T>에 등록한다.

                    - _dataStore.Value
                      : 실제 MetaDataStore 인스턴스를 가져온다.
                      : 최초 접근 시점이면 MetaDataStore를 생성한다.

                    - Metadata.DataStore
                      : 외부에서 MetaDataStore에 접근하기 위한 static 프로퍼티다.

                    - MetaDataStore.GetData(int metaId)
                      : metaId에 해당하는 문자열 데이터를 조회한다.
                      : 데이터가 없으면 "Unknown"을 반환한다.

                    - _datas.TryGetValue(metaId, out string name)
                      : Dictionary에서 metaId에 해당하는 값을 안전하게 조회한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - Lazy<T>의 기본 모드는 LazyThreadSafetyMode.ExecutionAndPublication이다.
                    - 여러 스레드가 동시에 Metadata.DataStore에 접근해도 MetaDataStore 생성은 한 번만 성공한다.
                    - 최초 생성 중 다른 스레드가 Value에 접근하면 생성이 끝날 때까지 대기할 수 있다.
                    - MetaDataStore 생성 이후에는 모든 스레드가 같은 인스턴스를 공유한다.
                    - Lazy<T>는 객체 생성 과정의 Thread-Safe를 보장하지만, 생성된 객체 내부의 모든 동작까지 자동으로 Thread-Safe하게 만들어주지는 않는다.
                    - 이 예제의 Dictionary는 생성 이후 읽기만 하므로 일반적으로 안전하게 사용할 수 있다.
                    - 만약 생성 이후 Dictionary를 추가/삭제/수정한다면 lock, ReaderWriterLockSlim, ConcurrentDictionary 같은 별도 동기화가 필요하다.

                  7. 주의점
                    - Lazy<T>는 내부 Value 객체를 자동으로 Dispose하지 않는다.
                    - MetaDataStore가 IDisposable을 구현한다면 별도의 Dispose 처리가 필요하다.
                    - 최초 접근 시점에 로딩 비용이 몰릴 수 있다.
                    - 서버 시작 시 모든 메타데이터를 미리 검증해야 하는 구조라면 Lazy<T> 사용이 적합하지 않을 수 있다.
                    - 생성자에서 예외가 발생하면 기본 모드에서는 예외가 캐싱되어 이후 접근 시에도 같은 예외가 다시 발생할 수 있다.
                    - 실패 후 재시도가 필요한 로딩 구조라면 Lazy<T>를 새로 만들거나 Reset 가능한 래퍼가 필요하다.
                    - static Lazy<T>는 프로세스 생명주기 동안 유지되므로, 테스트 코드에서는 상태 공유에 주의해야 한다.

                  8. 예상 결과
                    - "메타데이터 시작" 출력 시점에는 MetaDataStore가 아직 생성되지 않는다.
                    - Metadata.DataStore.GetData(1001)을 호출하는 순간 MetaDataStore가 생성된다.
                    - 메타데이터 로딩 로그가 출력된다.
                    - 1001에 해당하는 "Data 1"이 출력된다.
                    - 이후 다시 Metadata.DataStore.GetData(...)를 호출해도 MetaDataStore는 다시 생성되지 않는다.

                    예상 출력:

                      메타데이터 시작
                      메타데이터 로딩 시작
                      메타데이터 로딩 완료
                      Data 1
            */


            Console.WriteLine("메타데이터 시작");

            // 아직 MetaDataStore 생성 안 됨.
            // Lazy<MetaDataStore> 객체만 존재한다.

            string dataName = Metadata.DataStore.GetData(1001);

            // 이 시점에 Metadata.DataStore가 _dataStore.Value에 접근한다.
            // 따라서 처음 한 번 new MetaDataStore()가 실행된다.

            Console.WriteLine(dataName);

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        static void testMode(LazyThreadSafetyMode mode)
        {
            int factoryCallCount = 0;

            Lazy<int> lazyValue = new Lazy<int>(
                valueFactory: () =>
                {
                    int callNo = Interlocked.Increment(ref factoryCallCount);

                    Console.WriteLine(
                        $"[{mode}] Factory 실행 CallNo = {callNo}, ThreadId = {Environment.CurrentManagedThreadId}");

                    // 여러 스레드가 동시에 접근하는 상황을 더 잘 보이게 하기 위한 지연
                    Thread.Sleep(300);

                    return callNo;
                },
                mode: mode);

            Task[] tasks = Enumerable.Range(1, 10)
                .Select(index => Task.Run(() =>
                {
                    try
                    {
                        int value = lazyValue.Value;

                        Console.WriteLine(
                            $"[{mode}] Task {index}, Value = {value}, ThreadId = {Environment.CurrentManagedThreadId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"[{mode}] Task {index}, Exception = {ex.GetType().Name}, Message = {ex.Message}");
                    }
                }))
                .ToArray();

            Task.WaitAll(tasks);

            Console.WriteLine($"[{mode}] IsValueCreated = {lazyValue.IsValueCreated}");
            Console.WriteLine($"[{mode}] Factory Call Count = {factoryCallCount}");
        }

        static void use_LazyThreaSafeMode()
        {
            /*
                📚 LazyThreadSafetyMode 모드 비교

                  1. 개요
                    - LazyThreadSafetyMode의 3가지 모드를 비교하는 예제다.
                    - ExecutionAndPublication, PublicationOnly, None의 차이를 확인한다.
                    - 여러 Task가 동시에 Lazy<T>.Value에 접근했을 때 valueFactory가 몇 번 실행되는지 관찰한다.

                  2. 기본 개념
                    - Lazy<T>는 Value에 처음 접근할 때 valueFactory를 실행한다.
                    - 멀티 스레드에서 동시에 접근하면 mode에 따라 valueFactory 실행 횟수와 결과 저장 방식이 달라진다.

                  3. 핵심 특징
                    - ExecutionAndPublication:
                      valueFactory가 한 번만 실행된다.

                    - PublicationOnly:
                      valueFactory가 여러 번 실행될 수 있다.
                      하지만 하나의 결과만 최종 Value로 사용된다.

                    - None:
                      Thread-Safe하지 않다.
                      동시에 접근하면 안전하지 않다.

                  4. 실행 흐름
                    - 각 모드별 Lazy<int>를 생성한다.
                    - 여러 Task가 동시에 lazy.Value에 접근한다.
                    - valueFactory 실행 횟수를 카운트한다.
                    - 최종 IsValueCreated와 Factory 실행 횟수를 출력한다.

                  5. 대표 메서드 또는 주요 코드
                    - testMode(mode)
                      : 특정 LazyThreadSafetyMode를 테스트한다.

                    - Interlocked.Increment(ref factoryCallCount)
                      : valueFactory가 몇 번 실행되었는지 Thread-Safe하게 증가시킨다.

                    - Task.WaitAll(tasks)
                      : 모든 Task가 Value 접근을 끝낼 때까지 기다린다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - ExecutionAndPublication은 하나의 스레드만 생성 작업을 수행한다.
                    - PublicationOnly는 여러 스레드가 동시에 생성 작업을 수행할 수 있다.
                    - None은 동기화가 없으므로 테스트 결과가 안정적이지 않을 수 있다.

                  7. 주의점
                    - None 모드는 멀티 스레드 테스트에서 예기치 않은 결과가 나올 수 있다.
                    - PublicationOnly는 valueFactory가 여러 번 실행되어도 문제가 없는 경우에만 사용해야 한다.
                    - ExecutionAndPublication은 안전하지만 생성 중 대기하는 스레드가 생길 수 있다.

                  8. 모드 선택 기준

                    | 모드                    | valueFactory 실행 횟수        | Thread-Safe | 예외 캐싱 | 추천 상황
                    |-------------------------|-------------------------------|-------------|-----------|-------------------------------
                    | ExecutionAndPublication | 한 번만 실행                  | O           | O         | Singleton, Config, Metadata
                    | PublicationOnly         | 여러 번 실행될 수 있음        | O           | X         | 실패 후 재시도, 중복 생성 허용
                    | None                    | 여러 번 실행될 수 있음        | X           | 보장 약함 | 단일 스레드 전용

                    1) ExecutionAndPublication
                      - 가장 일반적인 선택이다.
                      - 객체가 반드시 한 번만 생성되어야 할 때 사용한다.
                      - Singleton, 전역 설정, 메타데이터 저장소처럼 중복 생성되면 안 되는 객체에 적합하다.
                      - valueFactory 예외가 캐싱되므로 초기화 실패 후 자동 재시도는 되지 않는다.

                    2) PublicationOnly
                      - 여러 스레드가 동시에 valueFactory를 실행할 수 있다.
                      - 그중 하나의 결과만 최종 Value로 게시된다.
                      - 중복 생성이 발생해도 문제가 없고, 실패 후 재시도가 필요할 때 고려한다.
                      - valueFactory 예외가 캐싱되지 않으므로 다음 접근에서 다시 초기화를 시도할 수 있다.

                    3) None
                      - Lazy<T> 내부 동기화를 사용하지 않는다.
                      - 멀티 스레드 환경에서는 안전하지 않다.
                      - 단일 스레드에서만 접근한다고 확신할 수 있을 때 사용한다.
                      - 예: 게임 서버의 특정 Logic Thread 내부에서만 접근하는 Lazy 객체.

                  9. 예상 결과
                    - ExecutionAndPublication:
                      Factory 호출 횟수는 보통 1이다.

                    - PublicationOnly:
                      Factory 호출 횟수는 1보다 클 수 있다.

                    - None:
                      Factory 호출 횟수와 결과는 동시성 상황에 따라 불안정할 수 있다.
            */
            {
                testMode(LazyThreadSafetyMode.ExecutionAndPublication);
                Console.WriteLine();

                testMode(LazyThreadSafetyMode.PublicationOnly);
                Console.WriteLine();

                testMode(LazyThreadSafetyMode.None);

                Console.ReadLine();
            }
        }

        //-----------------------------------------------------------------------------------------

        static void testExceptionMode(LazyThreadSafetyMode mode)
        {
            int attempt = 0;

            Lazy<int> lazyValue = new Lazy<int>(
                valueFactory: () =>
                {
                    attempt++;

                    Console.WriteLine($"[{mode}] Factory Attempt = {attempt}");

                    if (attempt == 1)
                        throw new InvalidOperationException("첫 번째 초기화 실패");

                    return 100;
                },
                mode: mode);

            for (int i = 1; i <= 2; i++)
            {
                try
                {
                    int value = lazyValue.Value;

                    Console.WriteLine($"[{mode}] Value = {value}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{mode}] Exception = {ex.Message}");
                }
            }
        }
        static void use_LazyThreadSafetyMode_with_Exception()
        {
            /*
                📚 LazyThreadSafetyMode + 예외 캐싱 차이

                  1. 개요
                    - Lazy<T> 초기화 중 예외가 발생했을 때,
                      LazyThreadSafetyMode에 따라 예외가 캐싱되는지 비교하는 예제다.
                    - ExecutionAndPublication은 예외를 캐싱한다.
                    - PublicationOnly는 예외를 캐싱하지 않는다.

                  2. 기본 개념
                    - Lazy<T>.Value에 처음 접근하면 valueFactory가 실행된다.
                    - valueFactory에서 예외가 발생하면 Value 접근도 예외로 실패한다.
                    - 모드에 따라 실패한 예외를 Lazy<T>가 기억할 수도 있고,
                      다음 접근에서 다시 초기화를 시도할 수도 있다.

                  3. 핵심 특징
                    - ExecutionAndPublication:
                      첫 번째 초기화 실패 예외가 캐싱된다.
                      이후 Value에 다시 접근해도 valueFactory를 다시 실행하지 않는다.
                      같은 예외가 다시 발생한다.

                    - PublicationOnly:
                      초기화 실패 예외가 캐싱되지 않는다.
                      다음 Value 접근 시 valueFactory를 다시 실행할 수 있다.
                      따라서 재시도가 가능하다.

                  4. 실행 흐름
                    - ExecutionAndPublication 모드 테스트
                      -> 첫 번째 Value 접근
                      -> valueFactory 실행
                      -> 예외 발생
                      -> 예외 캐싱
                      -> 두 번째 Value 접근
                      -> valueFactory 재실행 안 함
                      -> 같은 예외 발생

                    - PublicationOnly 모드 테스트
                      -> 첫 번째 Value 접근
                      -> valueFactory 실행
                      -> 예외 발생
                      -> 예외 캐싱 안 함
                      -> 두 번째 Value 접근
                      -> valueFactory 재실행
                      -> 성공 가능

                  5. 대표 메서드 또는 주요 코드
                    - lazyValue.Value
                      : Lazy<T> 내부 값을 가져온다.
                      : 최초 접근 시 valueFactory를 실행한다.

                    - LazyThreadSafetyMode.ExecutionAndPublication
                      : valueFactory 예외를 캐싱한다.

                    - LazyThreadSafetyMode.PublicationOnly
                      : valueFactory 예외를 캐싱하지 않는다.

                    - attempt
                      : valueFactory가 몇 번 실행되었는지 확인하기 위한 변수다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - ExecutionAndPublication은 하나의 스레드만 valueFactory를 실행하고,
                      실패 예외도 캐싱한다.
                    - PublicationOnly는 여러 스레드가 valueFactory를 실행할 수 있으며,
                      실패 예외는 게시하지 않는다.
                    - PublicationOnly에서는 실패한 초기화가 Lazy<T>.Value로 확정되지 않으므로
                      이후 다시 초기화를 시도할 수 있다.

                  7. 주의점
                    - ExecutionAndPublication에서 초기화 실패 후 재시도하려면
                      Lazy<T> 인스턴스를 새로 만들어야 한다.
                    - PublicationOnly는 재시도에 유리하지만,
                      valueFactory가 여러 번 실행될 수 있으므로 중복 실행되어도 안전해야 한다.
                    - valueFactory 안에서 파일 생성, DB Insert, 외부 API 호출처럼 부작용이 있는 작업을 하면
                      PublicationOnly에서 중복 실행 문제가 생길 수 있다.
                    - None 모드는 Thread-Safe하지 않으므로 예외 캐싱 비교 예제에서는 보통 제외한다.

                  8. 예상 결과
                    - ExecutionAndPublication:
                      Factory Attempt = 1만 출력된다.
                      두 번 모두 "첫 번째 초기화 실패" 예외가 발생한다.

                    - PublicationOnly:
                      Factory Attempt = 1에서 실패한다.
                      Factory Attempt = 2에서 다시 실행되고 성공한다.

                  9. 핵심 요약
                    - 기본 Lazy<T> 모드인 ExecutionAndPublication은 예외를 캐싱한다.
                    - PublicationOnly는 예외를 캐싱하지 않아 재시도 가능하다.
                    - 재시도 가능성이 필요하면 PublicationOnly를 고려할 수 있다.
                    - 하지만 중복 생성과 부작용에 주의해야 한다.
            */

            testExceptionMode(LazyThreadSafetyMode.ExecutionAndPublication);
            Console.WriteLine();

            testExceptionMode(LazyThreadSafetyMode.PublicationOnly);

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        public sealed class ResettableLazy<T>
        {
            private readonly Func<T> _factory;
            private Lazy<T> _lazy;

            public ResettableLazy(Func<T> factory)
            {
                _factory = factory;
                _lazy = CreateLazy();
            }

            public T Value => _lazy.Value;

            public bool IsValueCreated => _lazy.IsValueCreated;

            public void Reset()
            {
                _lazy = CreateLazy();
            }

            private Lazy<T> CreateLazy()
            {
                return new Lazy<T>(_factory);
            }
        }

        static void resettable_Lazy()
        {
            /*
                📚 ResettableLazy<T>

                  1. 개요
                    - Lazy<T>에 Reset 기능을 추가한 간단한 래퍼 예제다.
                    - 기본 Lazy<T>는 한 번 Value가 생성되면 같은 값을 계속 재사용한다.
                    - 기본 Lazy<T>에는 생성된 값을 버리고 다시 초기화하는 Reset 기능이 없다.
                    - ResettableLazy<T>는 내부 Lazy<T> 인스턴스를 새로 만들어서 다시 지연 초기화할 수 있게 한다.
                    - Lazy<T>는 .NET Framework 4.0부터 사용할 수 있다.
                    - .NET Core, .NET 5 이상에서도 사용할 수 있다.

                    * 대표 용도:
                      - 설정 캐시 다시 로딩
                      - 메타데이터 재로딩
                      - 임시 캐시 초기화
                      - 테스트 코드에서 Lazy 상태 초기화
                      - 초기화 실패 후 Lazy<T> 재생성
                      - 특정 시점에 값 재계산

                  2. 기본 개념
                    - ResettableLazy<T>는 내부에 Lazy<T>를 보관한다.
                    - Value 프로퍼티는 내부 Lazy<T>.Value를 그대로 반환한다.
                    - Reset()을 호출하면 내부 Lazy<T>를 새 인스턴스로 교체한다.
                    - Reset() 이후 다시 Value에 접근하면 factory가 다시 실행된다.

                    핵심 구조:

                      private readonly Func<T> _factory;
                      private Lazy<T> _lazy;

                      public T Value => _lazy.Value;

                      public void Reset()
                      {
                          _lazy = CreateLazy();
                      }

                      private Lazy<T> CreateLazy()
                      {
                          return new Lazy<T>(_factory);
                      }

                    - _factory는 값을 생성하는 함수다.
                    - _lazy는 현재 사용 중인 Lazy<T> 인스턴스다.
                    - Reset()은 기존 _lazy를 버리고 새 Lazy<T>를 만든다.

                  3. 핵심 특징
                    - 최초 Value 접근 전까지 값은 생성되지 않는다.
                    - Value에 처음 접근하면 _factory가 실행된다.
                    - 한 번 생성된 값은 Reset 전까지 계속 재사용된다.
                    - Reset()을 호출하면 IsValueCreated는 다시 false 상태가 된다.
                    - Reset() 이후 Value에 접근하면 _factory가 다시 실행된다.
                    - 기본 Lazy<T>의 부족한 "재초기화" 기능을 외부 래퍼로 구현한 형태다.

                  4. 실행 흐름
                    - ResettableLazy<string> lazy 생성
                    - 내부에서 _factory 저장
                    - 내부 _lazy = CreateLazy() 실행
                    - 아직 Value에 접근하지 않았으므로 문자열 값은 생성되지 않음
                    - Console.WriteLine(lazy.Value) 호출
                    - 내부 Lazy<string>.Value 접근
                    - _factory 실행
                    - "값 생성" 출력
                    - 현재 시각 문자열 생성
                    - 첫 번째 값 출력
                    - lazy.Reset() 호출
                    - 내부 Lazy<string>을 새 Lazy<string>으로 교체
                    - Console.WriteLine(lazy.Value) 다시 호출
                    - 새 Lazy<string>.Value 접근
                    - _factory 다시 실행
                    - "값 생성" 다시 출력
                    - 새 시각 문자열 생성
                    - 두 번째 값 출력

                  5. 대표 메서드 또는 주요 코드
                    - ResettableLazy(Func<T> factory)
                      : 값을 생성할 factory를 저장하고 내부 Lazy<T>를 생성한다.

                    - Value
                      : 현재 내부 Lazy<T>.Value를 반환한다.
                      : 최초 접근 시 factory가 실행된다.

                    - IsValueCreated
                      : 현재 내부 Lazy<T>가 값을 이미 생성했는지 확인한다.

                    - Reset()
                      : 내부 Lazy<T>를 새 Lazy<T>로 교체한다.
                      : 기존에 생성된 값은 더 이상 사용하지 않는다.

                    - CreateLazy()
                      : _factory를 이용해 새 Lazy<T> 인스턴스를 생성한다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - 내부 Lazy<T> 자체는 기본적으로 Thread-Safe하다.
                    - 기본 Lazy<T> 모드는 LazyThreadSafetyMode.ExecutionAndPublication이다.
                    - 따라서 하나의 _lazy 인스턴스에 대해 여러 스레드가 동시에 Value에 접근하면
                      factory는 한 번만 실행된다.
                    - 하지만 이 ResettableLazy<T> 클래스의 Reset() 메서드는 현재 Thread-Safe하지 않다.
                    - 어떤 스레드가 Value를 읽는 중에 다른 스레드가 Reset()을 호출하면
                      서로 다른 Lazy<T> 인스턴스를 보게 될 수 있다.
                    - 멀티 스레드에서 Reset()과 Value 접근이 동시에 일어날 수 있다면 lock 또는 Interlocked 기반 동기화가 필요하다.

                    예:

                      Thread A -> lazy.Value 접근
                      Thread B -> lazy.Reset() 호출

                    이 경우 현재 구현에서는 동작 순서에 따라 오래된 Lazy<T> 값과 새 Lazy<T> 값이 섞일 수 있다.

                  7. 주의점
                    - Reset()은 기존 값이 IDisposable이어도 자동 Dispose하지 않는다.
                    - T가 IDisposable이면 Reset 전에 기존 값을 Dispose할지 정책을 정해야 한다.
                    - Reset() 이후 기존 값에 대한 참조를 다른 코드가 들고 있다면 그 객체는 계속 살아 있을 수 있다.
                    - 현재 구현은 멀티 스레드 Reset에 안전하지 않다.
                    - factory에서 예외가 발생하면 내부 Lazy<T> 기본 모드에서는 예외가 캐싱될 수 있다.
                    - 예외 후 재시도하려면 Reset()을 호출해서 새 Lazy<T>를 만들어야 한다.
                    - Reset이 자주 일어나는 구조라면 Lazy<T>보다 명시적인 캐시 관리 객체가 더 적합할 수 있다.
                    - DateTime.Now.ToString("HH:mm:ss")는 초 단위이므로 Reset 직후 같은 초에 다시 생성되면 같은 문자열이 출력될 수 있다.

                  8. 예상 결과
                    - 첫 번째 lazy.Value 접근 시 "값 생성"이 출력된다.
                    - 첫 번째 시각 문자열이 출력된다.
                    - Reset() 호출 후 두 번째 lazy.Value 접근 시 "값 생성"이 다시 출력된다.
                    - 두 번째 시각 문자열이 출력된다.
                    - Reset 전후 시간이 같은 초라면 두 값이 같게 보일 수 있다.
                    - 1초 이상 차이가 나면 서로 다른 시간이 출력된다.

                    예상 출력 예:

                      값 생성
                      14:30:10
                      값 생성
                      14:30:10

                    또는 시간이 바뀌면:

                      값 생성
                      14:30:10
                      값 생성
                      14:30:11

                  9. 핵심 요약
                    - 기본 Lazy<T>는 Reset 기능이 없다.
                    - ResettableLazy<T>는 내부 Lazy<T>를 새로 만들어 Reset을 구현한다.
                    - Reset() 이후 Value에 다시 접근하면 factory가 다시 실행된다.
                    - 단일 스레드나 테스트용으로는 간단하고 유용하다.
                    - 멀티 스레드에서 사용하려면 Reset과 Value 접근에 대한 동기화가 필요하다.
                    - IDisposable 값 처리 정책은 별도로 설계해야 한다.
            */

            ResettableLazy<string> lazy = new ResettableLazy<string>(() =>
            {
                Console.WriteLine("값 생성");
                return DateTime.Now.ToString("HH:mm:ss");
            });

            // 첫 번째 Value 접근.
            // 이 시점에 내부 Lazy<string>의 factory가 실행된다.
            Console.WriteLine(lazy.Value);

            // 내부 Lazy<string>을 새 Lazy<string>으로 교체한다.
            // 기존에 생성된 값은 더 이상 ResettableLazy<T>에서 사용하지 않는다.
            lazy.Reset();

            // Reset 이후 첫 번째 Value 접근.
            // 새 Lazy<string>이므로 factory가 다시 실행된다.
            Console.WriteLine(lazy.Value);

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        public sealed class ResourceHolder : IDisposable
        {
            public ResourceHolder()
            {
                Console.WriteLine("ResourceHolder 생성");
            }

            public void Use()
            {
                Console.WriteLine("ResourceHolder 사용");
            }

            public void Dispose()
            {
                Console.WriteLine("ResourceHolder 해제");
            }
        }

        public sealed class LazyResourceOwner : IDisposable
        {
            private readonly Lazy<ResourceHolder> _resource =
                new Lazy<ResourceHolder>(() => new ResourceHolder());

            public ResourceHolder Resource => _resource.Value;

            public void Dispose()
            {
                /*
                    중요:
                    Lazy<T> 자체는 IDisposable 이 아니다.
                    Lazy<T> 안에 들어 있는 Value 객체가 IDisposable 이라면
                    소유자가 직접 Dispose 해야 한다.

                    단, Dispose 시점에 _resource.Value 를 바로 호출하면 안 된다.

                    이유:
                    - 아직 ResourceHolder 가 생성되지 않았을 수도 있다.
                    - 그런데 Dispose 에서 _resource.Value 에 접근하면
                      Dispose 하려고 새 객체를 생성하는 이상한 상황이 생긴다.

                    따라서 반드시 IsValueCreated 를 확인한 뒤,
                    실제 생성된 경우에만 Dispose 한다.
                */

                if (_resource.IsValueCreated)
                {
                    _resource.Value.Dispose();
                }
            }
        }

        static void Lazy_with_IDisposable()
        {
            /*
                📚 Lazy<T> + IDisposable

                  1. 개요
                    - Lazy<T> 안에 IDisposable 객체를 넣었을 때 Dispose를 어떻게 처리해야 하는지 보여주는 예제다.
                    - Lazy<T>는 .NET Framework 4.0부터 사용할 수 있다.
                    - .NET Core, .NET 5 이상에서도 사용할 수 있다.
                    - IDisposable은 파일, 소켓, DB 연결, 네이티브 핸들, 메모리 버퍼 같은 명시적 해제가 필요한 리소스를 정리할 때 사용한다.
                    - Lazy<T>는 내부 Value 객체를 지연 생성하지만, 내부 객체의 Dispose를 자동으로 호출해주지는 않는다.
                    - 따라서 Lazy<T>로 IDisposable 객체를 감싼 경우, 소유자 클래스가 직접 Dispose 정책을 가져야 한다.

                  2. 기본 개념
                    - ResourceHolder는 IDisposable을 구현한 리소스 객체다.
                    - LazyResourceOwner는 Lazy<ResourceHolder>를 소유하는 클래스다.
                    - ResourceHolder는 LazyResourceOwner.Resource에 처음 접근하는 순간 생성된다.
                    - LazyResourceOwner.Dispose()가 호출될 때 ResourceHolder가 이미 생성되어 있다면 직접 Dispose한다.
                    - ResourceHolder가 아직 생성되지 않았다면 Dispose하지 않는다.

                    핵심 구조:

                      private readonly Lazy<ResourceHolder> _resource =
                          new Lazy<ResourceHolder>(() => new ResourceHolder());

                      public ResourceHolder Resource => _resource.Value;

                      public void Dispose()
                      {
                          if (_resource.IsValueCreated)
                          {
                              _resource.Value.Dispose();
                          }
                      }

                  3. 핵심 특징
                    - Lazy<T> 자체는 IDisposable을 구현하지 않는다.
                    - Lazy<T> 안에 들어 있는 T가 IDisposable이어도 Lazy<T>가 자동으로 Dispose하지 않는다.
                    - Dispose 시점에 _resource.Value를 무조건 호출하면 안 된다.
                    - _resource.Value는 최초 접근 시 객체를 생성하기 때문이다.
                    - 사용하지도 않은 ResourceHolder를 Dispose하려고 생성하는 상황을 피해야 한다.
                    - 따라서 Dispose에서는 반드시 IsValueCreated를 확인한 뒤, 이미 생성된 경우에만 Value.Dispose()를 호출한다.

                  4. 실행 흐름
                    - case 1. Lazy Value를 사용하지 않는 경우
                      -> LazyResourceOwner 생성
                      -> 내부 Lazy<ResourceHolder>만 생성됨
                      -> owner.Resource에 접근하지 않음
                      -> ResourceHolder는 생성되지 않음
                      -> using 블록 종료
                      -> LazyResourceOwner.Dispose() 호출
                      -> _resource.IsValueCreated가 false
                      -> ResourceHolder.Dispose() 호출 안 함

                    - case 2. Lazy Value를 사용하는 경우
                      -> LazyResourceOwner 생성
                      -> owner.Resource.Use() 호출
                      -> owner.Resource가 _resource.Value에 접근
                      -> ResourceHolder 생성
                      -> ResourceHolder.Use() 실행
                      -> using 블록 종료
                      -> LazyResourceOwner.Dispose() 호출
                      -> _resource.IsValueCreated가 true
                      -> _resource.Value.Dispose() 호출
                      -> ResourceHolder 해제

                  5. 대표 메서드 또는 주요 코드
                    - new Lazy<ResourceHolder>(() => new ResourceHolder())
                      : ResourceHolder 생성 로직을 Lazy<T>에 등록한다.
                      : 이 시점에는 ResourceHolder가 생성되지 않는다.

                    - Resource => _resource.Value
                      : ResourceHolder에 접근하는 프로퍼티다.
                      : 최초 접근 시 ResourceHolder가 생성된다.

                    - _resource.IsValueCreated
                      : Lazy<T> 내부 Value가 이미 생성되었는지 확인한다.
                      : Value가 생성되지 않았다면 false를 반환한다.

                    - _resource.Value.Dispose()
                      : 이미 생성된 ResourceHolder를 명시적으로 해제한다.

                    - using (var owner = new LazyResourceOwner())
                      : using 블록이 끝날 때 owner.Dispose()가 자동 호출된다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - Lazy<T>의 기본 모드는 LazyThreadSafetyMode.ExecutionAndPublication이다.
                    - 여러 스레드가 동시에 Resource에 접근해도 ResourceHolder 생성은 한 번만 성공한다.
                    - 한 스레드가 ResourceHolder를 생성하는 동안 다른 스레드는 생성 완료를 기다릴 수 있다.
                    - 하지만 Dispose와 Resource 접근이 동시에 발생하는 상황은 별도로 동기화하지 않으면 안전하지 않을 수 있다.

                    예:

                      Thread A -> owner.Resource.Use()
                      Thread B -> owner.Dispose()

                    - Thread A가 Resource를 사용하는 중 Thread B가 Dispose하면 ObjectDisposed 상태의 리소스를 사용할 위험이 있다.
                    - 실제 서버 코드에서는 Dispose 이후 Resource 접근 방지, 사용 중 Dispose 방지, lock, reference count, lifecycle state 같은 정책이 필요할 수 있다.
                    - Lazy<T>는 생성의 Thread-Safe를 도와줄 뿐, 리소스 사용/해제 전체 생명주기를 자동으로 안전하게 만들어주지는 않는다.

                  7. 주의점
                    - Lazy<T>는 내부 Value를 자동 Dispose하지 않는다.
                    - IDisposable 객체를 Lazy<T>로 감쌌다면 소유자가 Dispose를 책임져야 한다.
                    - Dispose에서 IsValueCreated 확인 없이 _resource.Value.Dispose()를 호출하면 안 된다.
                    - Value가 생성되지 않은 상태에서 _resource.Value를 호출하면 그 순간 새 ResourceHolder가 생성된다.
                    - 즉, 사용하지도 않은 리소스를 Dispose하려고 생성하는 비효율적인 코드가 될 수 있다.
                    - ResourceHolder.Dispose()가 여러 번 호출될 가능성이 있다면 ResourceHolder 자체도 중복 Dispose에 안전하게 구현하는 것이 좋다.
                    - LazyResourceOwner.Dispose()도 실전 코드에서는 중복 호출 방어가 있는 것이 좋다.
                    - ResourceHolder가 파일, 소켓, DB 연결 같은 실제 리소스를 가진다면 Dispose 누락은 리소스 누수로 이어질 수 있다.
                    - static Lazy<T>에 IDisposable 객체를 넣는 경우, 애플리케이션 종료 시점의 해제 정책을 별도로 정해야 한다.

                  8. 예상 결과
                    - case 1에서는 owner.Resource에 접근하지 않는다.
                    - 따라서 ResourceHolder 생성 로그도, 해제 로그도 출력되지 않는다.

                    출력 예:

                      case 1. Lazy Value를 사용하지 않는 경우

                      case 2. Lazy Value를 사용하는 경우
                      ResourceHolder 생성
                      ResourceHolder 사용
                      ResourceHolder 해제

                    중요한 점:
                      - case 1에서 ResourceHolder 해제가 출력되지 않는 것은 누수가 아니다.
                      - ResourceHolder가 애초에 생성되지 않았기 때문이다.
                      - case 2에서는 ResourceHolder가 생성되었으므로 using 종료 시점에 해제된다.

                  9. 핵심 요약
                    - Lazy<T>는 객체 생성을 지연시킬 뿐, Dispose를 대신해주지 않는다.
                    - Lazy<T> 안의 Value가 IDisposable이면 소유자가 직접 Dispose해야 한다.
                    - Dispose에서는 IsValueCreated를 먼저 확인해야 한다.
                    - IsValueCreated가 true일 때만 _resource.Value.Dispose()를 호출한다.
                    - Lazy<T> + IDisposable 조합에서는 생성 시점과 해제 시점을 모두 명확히 관리해야 한다.
            */

            Console.WriteLine("case 1. Lazy Value를 사용하지 않는 경우");

            using (var owner = new LazyResourceOwner())
            {
                // owner.Resource 에 접근하지 않음
                // 따라서 ResourceHolder 는 생성되지 않는다.
            }

            /*
                출력:
                case 1. Lazy Value를 사용하지 않는 경우

                ResourceHolder 생성/해제 출력 없음.
                이유:
                ResourceHolder 가 아예 생성되지 않았기 때문.
            */

            Console.WriteLine();

            Console.WriteLine("case 2. Lazy Value를 사용하는 경우");

            using (var owner = new LazyResourceOwner())
            {
                owner.Resource.Use();
            }

            /*
                출력:
                case 2. Lazy Value를 사용하는 경우
                ResourceHolder 생성
                ResourceHolder 사용
                ResourceHolder 해제

                이유:
                owner.Resource 에 접근하는 순간 ResourceHolder 가 생성되고,
                using 블록이 끝날 때 LazyResourceOwner.Dispose()가 호출되면서
                내부 ResourceHolder 도 함께 Dispose 된다.
            */

            Console.ReadLine();
        }

        //-----------------------------------------------------------------------------------------

        public static void Test()
        {
            //Lazy_with_IDisposable();

            //resettable_Lazy();

            //use_LazyThreadSafetyMode_with_Exception();

            //use_LazyThreaSafeMode();

            //metadataStore_with_Lazy();

            //Lazy_with_singleton();

            //Lazy_what();
        }
    }
}