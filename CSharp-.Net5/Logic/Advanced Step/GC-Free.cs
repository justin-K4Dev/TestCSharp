using System;
using System.Buffers;
using System.Runtime.InteropServices;



namespace AdvancedStep;


public class GCFree
{
    // stackalloc 사용 (스택 메모리)
    // stackalloc은 힙이 아닌 스택 메모리를 직접 할당하는 방식으로, GC가 관여하지 않습니다.
    // Span<T> 은 스택, 힙, unmanaged 영역 모두 포인터처럼 접근할 수 있도록 도와주는 안전한 래퍼입니다.
    // GC 힙을 사용하지 않기 때문에 매우 빠르고 predictability가 뛰어납니다.
    public static void use_stackalloc()
    {
        Console.WriteLine("[1] stackalloc example:");

        Span<int> span = stackalloc int[5];
        for (int i = 0; i < span.Length; i++)
            span[i] = i * 10;

        foreach (var item in span)
            Console.WriteLine(item);

        Console.WriteLine();
    }


    public sealed class NativeBuffer : IDisposable
    {
        public IntPtr Ptr { get; private set; }
        public int Size { get; }

        public NativeBuffer(int size)
        {
            Size = size;
            Ptr = Marshal.AllocHGlobal(size);
        }

        public unsafe void Clear()
        {
            Span<byte> span = new Span<byte>((void*)Ptr, Size);
            span.Clear();
        }

        public void Dispose()
        {
            if (Ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Ptr);
                Ptr = IntPtr.Zero;
            }
        }

        ~NativeBuffer() => Dispose();
    }

    public sealed class NativeBufferPool
    {
        private readonly int bufferSize;
        private readonly Stack<NativeBuffer> pool = new();

        public NativeBufferPool(int bufferSize, int initialCount = 5)
        {
            this.bufferSize = bufferSize;
            for (int i = 0; i < initialCount; i++)
                pool.Push(new NativeBuffer(bufferSize));
        }

        public NativeBuffer Rent()
        {
            return pool.Count > 0 ? pool.Pop() : new NativeBuffer(bufferSize);
        }

        public void Return(NativeBuffer buffer)
        {
            buffer.Clear();
            pool.Push(buffer);
        }

        public void DisposeAll()
        {
            while (pool.Count > 0)
                pool.Pop().Dispose();
        }
    }


    // ArrayPool<T> 사용
    // .NET에서는 GC pressure를 줄이기 위해 메모리 재사용 풀을 제공합니다.
    // ArrayPool 사용 (재사용 가능한 배열)
    public static void use_ArrayPool()
    {
        Console.WriteLine("[4] ArrayPool:");
        var pool = ArrayPool<byte>.Shared;
        byte[] buffer = pool.Rent(10);

        try
        {
            for (int i = 0; i < 10; i++)
                buffer[i] = (byte)(i + 1);

            foreach (var b in buffer[..10])
                Console.WriteLine(b);
        }
        finally
        {
            pool.Return(buffer);
        }

        Console.WriteLine();
    }

    // NativeBuffer & NativeBufferPool 커스텀 구현 예제
    // ※ ArrayPool<T>는 배열을 GC 대상에서 제거하진 않지만, 재사용하여 GC 발생을 억제하는 데 유용
    public static void use_NativeBufferPool()
    {
        Console.WriteLine("[5] NativeBufferPool:");
        var pool = new NativeBufferPool(16, 2);

        for (int i = 0; i < 3; i++)
        {
            var buf = pool.Rent(); // GC heap에 있지만 재사용 가능
            unsafe
            {
                byte* p = (byte*)buf.Ptr;
                for (int j = 0; j < buf.Size; j++)
                    p[j] = (byte)(i + j);
            }
            Console.WriteLine($"Buffer[{i}] Ptr: {buf.Ptr}");

            // 사용 후 반환
            pool.Return(buf);
        }

        pool.DisposeAll();

        Console.WriteLine();
    }

    static void use_MemoryPool()
    {
        // 1. MemoryPool<byte> 생성 (Shared 풀)
        MemoryPool<byte> pool = MemoryPool<byte>.Shared;

        for (int i = 0; i < 10; i++)
        {
            // 2. 메모리 임대 (최대 4096바이트까지 요청)
            using IMemoryOwner<byte> bufferOwner = pool.Rent(1024); // 1KB

            Memory<byte> buffer = bufferOwner.Memory;

            // 3. 버퍼에 데이터 채우기
            Span<byte> span = buffer.Span;
            for (int j = 0; j < 10; j++)
                span[j] = (byte)(j * i);

            // 4. 결과 확인
            Console.Write($"Iteration {i}: ");
            for (int j = 0; j < 10; j++)
                Console.Write($"{span[j]} ");
            Console.WriteLine();

            // 5. using 블록 종료 → 자동 반환
        }

        Console.WriteLine("버퍼 풀링을 이용한 메모리 재사용 완료!");
    }


    public static void Test()
    {
        //use_MemoryPool();

        //use_NativeBufferPool();

        //use_ArrayPool();

        //use_stackalloc();
    }
}
