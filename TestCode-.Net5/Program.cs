using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;


namespace Simple;

public static class GuardHelper
{
    //=============================================================================================
    // Null Argument
    //=============================================================================================

    public static void throwIfNullArgument(
        [NotNull] object? argument,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
    {
        if (argument != null)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Argument '{paramName}' cannot be null.";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentNullException(paramName, message);
    }


    //=============================================================================================
    // Null State
    //=============================================================================================

    public static void throwIfNullState(
        [NotNull] object? value,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? valueName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
    {
        if (value != null)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Required object '{valueName}' is null.";

        if (exceptionFactory != null)
            throw exceptionFactory(message, valueName);

        throw new InvalidOperationException(message);
    }


    //=============================================================================================
    // Condition
    //=============================================================================================

    public static void throwIfFalse(
        bool condition,
        string? message = null,
        [CallerArgumentExpression(nameof(condition))] string? conditionText = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (condition)
            return;

        var finalMessage = message
            ?? $"Failed to check condition: '{conditionText}'.";

        if (exceptionFactory != null)
            throw exceptionFactory(finalMessage);

        throw new InvalidOperationException(finalMessage);
    }

    public static void throwIfTrue(
        bool condition,
        string? message = null,
        [CallerArgumentExpression(nameof(condition))] string? conditionText = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (!condition)
            return;

        var finalMessage = message
            ?? $"Unexpected condition is true: '{conditionText}'.";

        if (exceptionFactory != null)
            throw exceptionFactory(finalMessage);

        throw new InvalidOperationException(finalMessage);
    }

    /// <summary>
    /// Expression 기반 조건 체크.
    /// 조건식을 문자열로 얻어야 할 때만 사용한다.
    /// Compile 비용이 있으므로 고빈도 경로에서는 throwIfFalse(bool)를 사용한다.
    /// </summary>
    public static void throwIfFalseWithCondition(
        Expression<Func<bool>> conditionExpression,
        Func<string>? messageFactory = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        ArgumentNullException.ThrowIfNull(conditionExpression);

        var compiledExpression = conditionExpression.Compile();

        if (compiledExpression())
            return;

        var conditionText = conditionExpression.Body.ToString();

        var message = messageFactory?.Invoke()
            ?? $"Failed to check condition: '{conditionText}'.";

        if (exceptionFactory != null)
            throw exceptionFactory(message);

        throw new InvalidOperationException(message);
    }


    //=============================================================================================
    // Range - int
    //=============================================================================================

    public static void throwIfOutOfRange(
        int value,
        int min,
        int max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
    {
        validateRangeOrder(min, max);

        if (value >= min && value <= max)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' is out of range. value={value}, range=[{min}, {max}].";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    public static void throwIfLessThan(
        int value,
        int min,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
    {
        if (value >= min)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' must be greater than or equal to {min}. value={value}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    public static void throwIfGreaterThan(
        int value,
        int max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
    {
        if (value <= max)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' must be less than or equal to {max}. value={value}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentOutOfRangeException(paramName, value, message);
    }


    //=============================================================================================
    // Range - generic
    //=============================================================================================

    public static void throwIfOutOfRange<T>(
        T value,
        T min,
        T max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
        where T : IComparable<T>
    {
        validateRangeOrder(min, max);

        if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' is out of range. value={value}, range=[{min}, {max}].";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    public static void throwIfLessThan<T>(
        T value,
        T min,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) >= 0)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' must be greater than or equal to {min}. value={value}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    public static void throwIfGreaterThan<T>(
        T value,
        T max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, string?, Exception>? exceptionFactory = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(max) <= 0)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' must be less than or equal to {max}. value={value}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message, paramName);

        throw new ArgumentOutOfRangeException(paramName, value, message);
    }


    //=============================================================================================
    // Private
    //=============================================================================================

    private static void validateRangeOrder<T>(T min, T max)
        where T : IComparable<T>
    {
        if (min.CompareTo(max) <= 0)
            return;

        throw new ArgumentException($"Invalid range. min={min}, max={max}.");
    }

    //=============================================================================================
    // Overflow
    //=============================================================================================
    public static void throwIfOverflow(
        bool isOverflow,
        string? message = null,
        [CallerArgumentExpression(nameof(isOverflow))] string? conditionText = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (!isOverflow)
            return;

        var finalMessage = message
            ?? $"Overflow condition detected: '{conditionText}'.";

        if (exceptionFactory != null)
            throw exceptionFactory(finalMessage);

        throw new OverflowException(finalMessage);
    }

    public static void throwIfReachedMax(
        int value,
        int max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (value < max)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' has reached max value. value={value}, max={max}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message);

        throw new OverflowException(message);
    }

    public static void throwIfReachedMax(
        uint value,
        uint max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (value < max)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' has reached max value. value={value}, max={max}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message);

        throw new OverflowException(message);
    }

    public static void throwIfReachedMax(
        long value,
        long max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (value < max)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' has reached max value. value={value}, max={max}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message);

        throw new OverflowException(message);
    }

    public static void throwIfReachedMax(
        ulong value,
        ulong max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, Exception>? exceptionFactory = null)
    {
        if (value < max)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' has reached max value. value={value}, max={max}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message);

        throw new OverflowException(message);
    }

    public static void throwIfReachedMax<T>(
        T value,
        T max,
        Func<string>? messageFactory = null,
        [CallerArgumentExpression(nameof(value))] string? paramName = null,
        Func<string, Exception>? exceptionFactory = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(max) < 0)
            return;

        var message = messageFactory?.Invoke()
            ?? $"Value '{paramName}' has reached max value. value={value}, max={max}.";

        if (exceptionFactory != null)
            throw exceptionFactory(message);

        throw new OverflowException(message);
    }
}

public enum MultiLockOrderPolicy
{
    /// <summary>
    /// 호출자가 넘긴 순서를 그대로 사용한다.
    /// 데드락 방지 측면에서는 권장하지 않는다.
    /// </summary>
    InputOrder,

    /// <summary>
    /// key 오름차순으로 락을 획득한다.
    /// 다중 락 데드락 방지를 위한 기본 추천 정책.
    /// </summary>
    KeyAscending,

    /// <summary>
    /// key 내림차순으로 락을 획득한다.
    /// 모든 호출 지점이 같은 정책을 쓸 때만 안전하다.
    /// </summary>
    KeyDescending,
}

public interface IReadWriteLockProvider
{
    ReaderWriterLockSlim getRWLock();
}

public sealed class Synchronizer<TObject>
    where TObject : class, IReadWriteLockProvider
{
    private readonly ReaderWriterLockSlim m_rw_lock;
    private readonly TObject m_value;

    public Synchronizer(TObject value)
    {
        GuardHelper.throwIfNullArgument(value, () => "value is null !!!");

        m_value = value;
        m_rw_lock = value.getRWLock();

        GuardHelper.throwIfNullState(
            m_rw_lock,
            () => "getRWLock() returned null !!!");
    }

    public TObject Value => m_value;

    public TResult callReadFunc<TResult>(Func<TObject, TResult> readFunc)
    {
        GuardHelper.throwIfNullArgument(readFunc, () => "readFunc is null !!!");

        using (tryReadLock())
        {
            return readFunc(m_value);
        }
    }

    public TResult callWriteFunc<TResult>(Func<TObject, TResult> writeFunc)
    {
        GuardHelper.throwIfNullArgument(writeFunc, () => "writeFunc is null !!!");

        using (tryWriteLock())
        {
            return writeFunc(m_value);
        }
    }

    public void callReadAction(Action<TObject> readAction)
    {
        GuardHelper.throwIfNullArgument(readAction, () => "readAction is null !!!");

        using (tryReadLock())
        {
            readAction(m_value);
        }
    }

    public void callWriteAction(Action<TObject> writeAction)
    {
        GuardHelper.throwIfNullArgument(writeAction, () => "writeAction is null !!!");

        using (tryWriteLock())
        {
            writeAction(m_value);
        }
    }

    public IDisposable tryReadLock()
    {
        m_rw_lock.EnterReadLock();
        return new Unlocker(m_rw_lock, isReadLock: true);
    }

    public IDisposable tryWriteLock()
    {
        m_rw_lock.EnterWriteLock();
        return new Unlocker(m_rw_lock, isReadLock: false);
    }
}

public static class SynchronizerHelper
{
    public static async Task<IDisposable> tryReadLockAsync(this ReaderWriterLockSlim rwLock)
    {
        GuardHelper.throwIfNullArgument(rwLock, () => $"rwLock is null !!!");

        return await Task.Run(() =>
        {
            rwLock.EnterReadLock();
            return (IDisposable)new Unlocker(rwLock, false);
        });
    }

    public static async Task<IDisposable> tryWriteLockAsync(this ReaderWriterLockSlim rwLock)
    {
        GuardHelper.throwIfNullArgument(rwLock, () => $"rwLock is null !!!");

        return await Task.Run(() =>
        {
            rwLock.EnterWriteLock();
            return (IDisposable)new Unlocker(rwLock, true);
        });
    }
}


//=============================================================================================
// Synchronizer를 복수 개로 처리할 때 사용하는 클래스.
//
// 정책:
// - 생성자에서 기본 락 획득 순서 정책을 지정할 수 있다.
// - 각 함수 호출 시 별도 정책을 지정하면 생성자 정책을 override한다.
// - Stack<IDisposable>을 사용해 획득 역순으로 락을 해제한다.
// - 중간 실패 시 이미 획득한 락을 모두 해제한다.
//
// author : kangms
//=============================================================================================

public sealed class MultipleSynchronizer<TObject>
    where TObject : class, IReadWriteLockProvider
{
    private readonly ConcurrentDictionary<string, Synchronizer<TObject>> m_synchronizers = new();

    private readonly MultiLockOrderPolicy m_default_order_policy;

    public MultipleSynchronizer(
        Dictionary<string, TObject> toSyncObjects,
        MultiLockOrderPolicy orderPolicy = MultiLockOrderPolicy.InputOrder)
    {
        GuardHelper.throwIfNullArgument(toSyncObjects);

        m_default_order_policy = orderPolicy;

        foreach (var each in toSyncObjects)
        {
            GuardHelper.throwIfNullArgument(each.Key);
            GuardHelper.throwIfNullArgument(each.Value);

            var synchronizer = new Synchronizer<TObject>(each.Value);

            GuardHelper.throwIfFalse(
                m_synchronizers.TryAdd(each.Key, synchronizer),
                $"Duplicated synchronizer key. key={each.Key}",
                exceptionFactory: message => new ArgumentException(message, nameof(toSyncObjects)));
        }
    }

    public int Count => m_synchronizers.Count;

    public MultiLockOrderPolicy defaultOrderPolicy => m_default_order_policy;

    public Synchronizer<TObject> this[string key]
    {
        get
        {
            GuardHelper.throwIfNullArgument(key);

            if (m_synchronizers.TryGetValue(key, out var synchronizer))
                return synchronizer;

            throw new KeyNotFoundException($"Synchronizer not found. key={key}");
        }
    }

    //=============================================================================================
    // 등록된 객체 중 locks 파라미터에 의해 선택적으로 Read / Write Lock을 설정한다.
    //
    // orderPolicy가 null이면 생성자에서 설정한 기본 정책을 사용한다.
    //=============================================================================================

    public MultiLock tryLockAll(
        IEnumerable<(string Key, bool IsReadLock)> locks,
        MultiLockOrderPolicy? orderPolicy = null)
    {
        var policy = orderPolicy ?? m_default_order_policy;

        return new MultiLock(
            m_synchronizers,
            locks,
            policy);
    }

    //=============================================================================================
    // 등록된 모든 객체에 Read Lock을 설정한다.
    //
    // orderPolicy가 null이면 생성자에서 설정한 기본 정책을 사용한다.
    //=============================================================================================

    public MultiReadLock tryReadLockAll(MultiLockOrderPolicy? orderPolicy = null)
    {
        var policy = orderPolicy ?? m_default_order_policy;

        return new MultiReadLock(
            m_synchronizers,
            policy);
    }

    //=============================================================================================
    // 등록된 모든 객체에 Write Lock을 설정한다.
    //
    // orderPolicy가 null이면 생성자에서 설정한 기본 정책을 사용한다.
    //=============================================================================================

    public MultiWriteLock tryWriteLockAll(MultiLockOrderPolicy? orderPolicy = null)
    {
        var policy = orderPolicy ?? m_default_order_policy;

        return new MultiWriteLock(
            m_synchronizers,
            policy);
    }

    //=============================================================================================
    // Lock Request 정규화
    //=============================================================================================

    private static IReadOnlyList<(string Key, bool IsReadLock)> normalizeLockRequests(
        IEnumerable<(string Key, bool IsReadLock)> locks,
        MultiLockOrderPolicy orderPolicy)
    {
        GuardHelper.throwIfNullArgument(locks);

        var lockArray = locks.ToArray();

        foreach (var item in lockArray)
        {
            GuardHelper.throwIfNullArgument(item.Key);
        }

        var duplicatedKey = lockArray
            .GroupBy(x => x.Key, StringComparer.Ordinal)
            .FirstOrDefault(x => x.Count() > 1);

        if (duplicatedKey != null)
        {
            throw new InvalidOperationException(
                $"Duplicated lock key. key={duplicatedKey.Key}");
        }

        return orderPolicy switch
        {
            MultiLockOrderPolicy.InputOrder =>
                lockArray,

            MultiLockOrderPolicy.KeyAscending =>
                lockArray
                    .OrderBy(x => x.Key, StringComparer.Ordinal)
                    .ToArray(),

            MultiLockOrderPolicy.KeyDescending =>
                lockArray
                    .OrderByDescending(x => x.Key, StringComparer.Ordinal)
                    .ToArray(),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(orderPolicy),
                    orderPolicy,
                    "Unknown multi lock order policy.")
        };
    }

    private static IReadOnlyList<KeyValuePair<string, Synchronizer<TObject>>> normalizeSynchronizers(
        IEnumerable<KeyValuePair<string, Synchronizer<TObject>>> syncObjects,
        MultiLockOrderPolicy orderPolicy)
    {
        GuardHelper.throwIfNullArgument(syncObjects);

        var array = syncObjects.ToArray();

        return orderPolicy switch
        {
            MultiLockOrderPolicy.InputOrder =>
                array,

            MultiLockOrderPolicy.KeyAscending =>
                array
                    .OrderBy(x => x.Key, StringComparer.Ordinal)
                    .ToArray(),

            MultiLockOrderPolicy.KeyDescending =>
                array
                    .OrderByDescending(x => x.Key, StringComparer.Ordinal)
                    .ToArray(),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(orderPolicy),
                    orderPolicy,
                    "Unknown multi lock order policy.")
        };
    }

    private static Synchronizer<TObject> getSynchronizerOrThrow(
        ConcurrentDictionary<string, Synchronizer<TObject>> syncObjects,
        string key)
    {
        if (syncObjects.TryGetValue(key, out var synchronizer))
            return synchronizer;

        throw new KeyNotFoundException($"Synchronizer not found. key={key}");
    }

    //=============================================================================================
    // MultiLock
    //=============================================================================================

    public sealed class MultiLock : IDisposable
    {
        private readonly Stack<IDisposable> m_lock_objects = new();
        private bool m_disposed;

        public MultiLock(
            ConcurrentDictionary<string, Synchronizer<TObject>> syncObjects,
            IEnumerable<(string Key, bool IsReadLock)> locks,
            MultiLockOrderPolicy orderPolicy)
        {
            GuardHelper.throwIfNullArgument(syncObjects);
            GuardHelper.throwIfNullArgument(locks);

            try
            {
                var normalizedLocks = normalizeLockRequests(
                    locks,
                    orderPolicy);

                foreach (var (key, isReadLock) in normalizedLocks)
                {
                    var found_object = getSynchronizerOrThrow(
                        syncObjects,
                        key);

                    var lock_object = isReadLock
                        ? found_object.tryReadLock()
                        : found_object.tryWriteLock();

                    m_lock_objects.Push(lock_object);
                }
            }
            catch
            {
                disposeLockObjects();
                throw;
            }
        }

        public void Dispose()
        {
            if (m_disposed)
                return;

            m_disposed = true;
            disposeLockObjects();
        }

        private void disposeLockObjects()
        {
            while (m_lock_objects.Count > 0)
            {
                m_lock_objects.Pop().Dispose();
            }
        }
    }

    //=============================================================================================
    // MultiReadLock
    //=============================================================================================

    public sealed class MultiReadLock : IDisposable
    {
        private readonly Stack<IDisposable> m_lock_objects = new();
        private bool m_disposed;

        public MultiReadLock(
            IEnumerable<KeyValuePair<string, Synchronizer<TObject>>> syncObjects,
            MultiLockOrderPolicy orderPolicy)
        {
            GuardHelper.throwIfNullArgument(syncObjects);

            try
            {
                var normalizedSyncObjects = normalizeSynchronizers(
                    syncObjects,
                    orderPolicy);

                foreach (var each in normalizedSyncObjects)
                {
                    var lock_object = each.Value.tryReadLock();
                    m_lock_objects.Push(lock_object);
                }
            }
            catch
            {
                disposeLockObjects();
                throw;
            }
        }

        public void Dispose()
        {
            if (m_disposed)
                return;

            m_disposed = true;
            disposeLockObjects();
        }

        private void disposeLockObjects()
        {
            while (m_lock_objects.Count > 0)
            {
                m_lock_objects.Pop().Dispose();
            }
        }
    }

    //=============================================================================================
    // MultiWriteLock
    //=============================================================================================

    public sealed class MultiWriteLock : IDisposable
    {
        private readonly Stack<IDisposable> m_lock_objects = new();
        private bool m_disposed;

        public MultiWriteLock(
            IEnumerable<KeyValuePair<string, Synchronizer<TObject>>> syncObjects,
            MultiLockOrderPolicy orderPolicy)
        {
            GuardHelper.throwIfNullArgument(syncObjects);

            try
            {
                var normalizedSyncObjects = normalizeSynchronizers(
                    syncObjects,
                    orderPolicy);

                foreach (var each in normalizedSyncObjects)
                {
                    var lock_object = each.Value.tryWriteLock();
                    m_lock_objects.Push(lock_object);
                }
            }
            catch
            {
                disposeLockObjects();
                throw;
            }
        }

        public void Dispose()
        {
            if (m_disposed)
                return;

            m_disposed = true;
            disposeLockObjects();
        }

        private void disposeLockObjects()
        {
            while (m_lock_objects.Count > 0)
            {
                m_lock_objects.Pop().Dispose();
            }
        }
    }
}

public sealed class Unlocker : IDisposable
{
    private readonly ReaderWriterLockSlim m_ref_rw_lock;
    private readonly bool m_is_read_lock;
    private bool m_disposed;

    public Unlocker(ReaderWriterLockSlim readWriteLockObject, bool isReadLock)
    {
        GuardHelper.throwIfNullArgument(readWriteLockObject);

        m_ref_rw_lock = readWriteLockObject;
        m_is_read_lock = isReadLock;
    }

    public void Dispose()
    {
        if (m_disposed)
            return;

        m_disposed = true;

        if (m_is_read_lock)
        {
            m_ref_rw_lock.ExitReadLock();
        }
        else
        {
            m_ref_rw_lock.ExitWriteLock();
        }
    }
}

class Program
{
    private sealed class TestSyncObject : IReadWriteLockProvider
    {
        private readonly ReaderWriterLockSlim _rwLock = new();

        public int Value { get; set; }

        public ReaderWriterLockSlim getRWLock()
        {
            return _rwLock;
        }

        public override string ToString()
        {
            return $"Value={Value}";
        }
    }

    static void Main(string[] args)
    {
        {
            void printObjects(string title, Dictionary<string, TestSyncObject> objects)
            {
                Console.WriteLine($"[{title}]");

                foreach (var pair in objects.OrderBy(x => x.Key, StringComparer.Ordinal))
                {
                    Console.WriteLine($"  {pair.Key} = {pair.Value.Value}");
                }
            }

            Console.WriteLine("========================================");
            Console.WriteLine("MultipleSynchronizer Policy Test");
            Console.WriteLine("========================================");

            var objects = new Dictionary<string, TestSyncObject>
            {
                ["A"] = new TestSyncObject { Value = 10 },
                ["B"] = new TestSyncObject { Value = 20 },
                ["C"] = new TestSyncObject { Value = 30 },
            };

            var multiSync = new MultipleSynchronizer<TestSyncObject>(
                objects,
                MultiLockOrderPolicy.InputOrder);

            Console.WriteLine("\n== constructor default policy ==");
            Console.WriteLine($"defaultOrderPolicy = {multiSync.defaultOrderPolicy}");
            Console.WriteLine($"Count = {multiSync.Count}");

            printObjects("initial", objects);


            Console.WriteLine("\n== single synchronizer read ==");

            var aValue = multiSync["A"].callReadFunc(x => x.Value);
            Console.WriteLine($"A read value = {aValue}");


            Console.WriteLine("\n== single synchronizer write ==");

            multiSync["A"].callWriteFunc(x =>
            {
                x.Value += 5;
                return x.Value;
            });

            printObjects("after A += 5", objects);


            Console.WriteLine("\n== tryLockAll : default policy KeyAscending ==");

            using (
                multiSync.tryLockAll(
                    new[] {
                        (Key: "C", IsReadLock: false),
                        (Key: "A", IsReadLock: false),
                        (Key: "B", IsReadLock: false),
                    },
                    MultiLockOrderPolicy.KeyAscending
                )
            )
            {
                // 요청 순서는 C, A, B 이지만,
                // 실제 획득 순서는 생성자 기본 정책에 의해 A, B, C 가 된다.
                objects["A"].Value += 100;
                objects["B"].Value += 200;
                objects["C"].Value += 300;
            }

            printObjects("after default policy multi write", objects);


            Console.WriteLine("\n== tryLockAll : function override InputOrder ==");

            using (
                multiSync.tryLockAll(
                    new[] {
                        (Key: "C", IsReadLock: false),
                        (Key: "A", IsReadLock: false),
                        (Key: "B", IsReadLock: false),
                    },
                    MultiLockOrderPolicy.InputOrder
                )
            )
            {
                // 이번에는 함수 호출 정책 override로 인해
                // 요청 순서 C, A, B 그대로 획득한다.
                objects["C"].Value += 1;
                objects["A"].Value += 1;
                objects["B"].Value += 1;
            }

            printObjects("after InputOrder override multi write", objects);


            Console.WriteLine("\n== tryLockAll : function override KeyDescending ==");

            using (
                multiSync.tryLockAll (
                    new[] {
                        (Key: "A", IsReadLock: false),
                        (Key: "B", IsReadLock: false),
                        (Key: "C", IsReadLock: false),
                    },
                    MultiLockOrderPolicy.KeyDescending
                )
            )
            {
                // 실제 획득 순서는 C, B, A
                objects["A"].Value += 10;
                objects["B"].Value += 10;
                objects["C"].Value += 10;
            }

            printObjects("after KeyDescending override multi write", objects);


            Console.WriteLine("\n== mixed read/write lock ==");

            using (
                multiSync.tryLockAll(
                    new[] {
                        (Key: "A", IsReadLock: true),
                        (Key: "C", IsReadLock: false),
                    }
                )
            )
            {
                var readA = objects["A"].Value;
                objects["C"].Value += readA;

                Console.WriteLine($"read A = {readA}");
                Console.WriteLine("C += A");
            }

            printObjects("after mixed read/write", objects);


            Console.WriteLine("\n== tryReadLockAll : default policy ==");

            using (multiSync.tryReadLockAll())
            {
                var sum = objects.Values.Sum(x => x.Value);
                Console.WriteLine($"sum = {sum}");
            }


            Console.WriteLine("\n== tryReadLockAll : override KeyDescending ==");

            using (multiSync.tryReadLockAll(MultiLockOrderPolicy.KeyDescending))
            {
                var sum = objects.Values.Sum(x => x.Value);
                Console.WriteLine($"sum = {sum}");
            }


            Console.WriteLine("\n== tryWriteLockAll : default policy ==");

            using (multiSync.tryWriteLockAll())
            {
                foreach (var obj in objects.Values)
                {
                    obj.Value += 1;
                }
            }

            printObjects("after tryWriteLockAll +1", objects);


            Console.WriteLine("\n== tryWriteLockAll : override InputOrder ==");

            using (multiSync.tryWriteLockAll(MultiLockOrderPolicy.InputOrder))
            {
                foreach (var obj in objects.Values)
                {
                    obj.Value += 1;
                }
            }

            printObjects("after tryWriteLockAll InputOrder +1", objects);


            Console.WriteLine("\n== missing key test ==");

            try
            {
                using (
                    multiSync.tryLockAll(
                        new[] {
                            (Key: "A", IsReadLock: true),
                            (Key: "Z", IsReadLock: true),
                        }
                    )
                )
                {
                    Console.WriteLine("unexpected success");
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("KeyNotFoundException success");
                Console.WriteLine($"Message = {ex.Message}");
            }

            Console.WriteLine("\n== duplicated key test ==");

            try
            {
                using (
                    multiSync.tryLockAll(
                        new[] {
                            (Key: "A", IsReadLock: true),
                            (Key: "A", IsReadLock: false),
                        }
                    )
                )
                {
                    Console.WriteLine("unexpected success");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("InvalidOperationException success");
                Console.WriteLine($"Message = {ex.Message}");
            }


            Console.WriteLine("\n== dispose twice test ==");

            var lockObject = multiSync.tryLockAll(
                new[] {
                    (Key: "A", IsReadLock: true),
                    (Key: "B", IsReadLock: true),
                }
            );

            lockObject.Dispose();
            lockObject.Dispose();

            Console.WriteLine("Dispose twice success");


            Console.WriteLine("\n== parallel write test ==");

            var parallelObjects = new Dictionary<string, TestSyncObject>
            {
                ["A"] = new TestSyncObject { Value = 0 },
                ["B"] = new TestSyncObject { Value = 0 },
                ["C"] = new TestSyncObject { Value = 0 },
            };

            var parallelSync = new MultipleSynchronizer<TestSyncObject>(
                parallelObjects,
                MultiLockOrderPolicy.KeyAscending);

            var workerCount = Environment.ProcessorCount;
            var loopCount = 10_000;

            var tasks = Enumerable.Range(0, workerCount)
                .Select(worker => Task.Run(() =>
                {
                    for (var i = 0; i < loopCount; i++)
                    {
                        using (
                            parallelSync.tryLockAll(
                                new[] {
                                    (Key: "C", IsReadLock: false),
                                    (Key: "A", IsReadLock: false),
                                    (Key: "B", IsReadLock: false),
                                }
                            )
                        )
                        {
                            parallelObjects["A"].Value++;
                            parallelObjects["B"].Value++;
                            parallelObjects["C"].Value++;
                        }
                    }
                }))
                .ToArray();

            Task.WaitAll(tasks);

            var expected = workerCount * loopCount;

            Console.WriteLine($"expected = {expected}");
            Console.WriteLine($"A = {parallelObjects["A"].Value}");
            Console.WriteLine($"B = {parallelObjects["B"].Value}");
            Console.WriteLine($"C = {parallelObjects["C"].Value}");
            Console.WriteLine($"A check = {parallelObjects["A"].Value == expected}");
            Console.WriteLine($"B check = {parallelObjects["B"].Value == expected}");
            Console.WriteLine($"C check = {parallelObjects["C"].Value == expected}");

            Console.WriteLine("\n== constructor duplicate key test ==");

            try
            {
                var duplicatedObjects = new Dictionary<string, TestSyncObject>
                {
                    ["A"] = new TestSyncObject { Value = 1 },
                };

                // Dictionary 자체가 같은 key 중복을 허용하지 않기 때문에
                // 이 테스트는 구조상 실제 중복 삽입은 어렵다.
                // MultipleSynchronizer 내부 TryAdd 방어 로직은 유지하는 것이 좋다.

                var duplicatedSync = new MultipleSynchronizer<TestSyncObject>(
                    duplicatedObjects,
                    MultiLockOrderPolicy.KeyAscending);

                Console.WriteLine("constructor duplicate key structural test success");
                Console.WriteLine($"duplicatedSync.Count = {duplicatedSync.Count}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ArgumentException success");
                Console.WriteLine($"Message = {ex.Message}");
            }


            Console.WriteLine("========================================");
            Console.WriteLine("MultipleSynchronizer Policy Test End");
            Console.WriteLine("========================================");
        }
    }
}