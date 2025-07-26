using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;



namespace AdvancedStep
{
    public class GCFree
    {
        // Marshal.AllocHGlobal 사용 (unmanaged heap)
        // Unmanaged 메모리 직접 할당 (Marshal, Unsafe, NativeMemory)
        // .NET 5 이상에서는 System.Runtime.InteropServices.NativeMemory 사용 가능
        // 그 외에는 Marshal.AllocHGlobal 또는 Unsafe 라이브러리 사용
        // ⚠️ GC가 이 메모리를 추적하지 않기 때문에 직접 해제하지 않으면 메모리 누수 발생
        public static void useUnmanagedMemory()
        {
            Console.WriteLine("Unmanaged memory with Marshal:");
            int size = 5;
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(int) * size);

            try
            {
                for (int i = 0; i < size; i++)
                    Marshal.WriteInt32(ptr, i * sizeof(int), i * 100);

                for (int i = 0; i < size; i++)
                    Console.WriteLine(Marshal.ReadInt32(ptr, i * sizeof(int)));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            Console.WriteLine();
        }


        // fixed 포인터 사용
        // fixed 키워드를 이용한 고정 메모리 접근 (pinning)
        // fixed는 GC가 해당 배열을 이동하지 못하도록 핀을 고정합니다.
        // 여전히 GC heap에 존재하지만 이동이 불가능하므로 native interop용도로 사용 가능
        public unsafe static void useFixedPointer()
        {
            Console.WriteLine("fixed pointer example:");
            int[] arr = new int[] { 1, 2, 3, 4, 5 };

            fixed (int* p = arr)
            {
                for (int i = 0; i < arr.Length; i++)
                    Console.WriteLine(p[i]);
            }
            Console.WriteLine();
        }

        // IDisposable 패턴을 이용한 명시적 리소스 해제
        // GC가 메모리는 자동 수거하지만, 핸들, 파일, 소켓 등 unmanaged 리소스는 직접 해제해야 합니다.
        class CustomBuffer : IDisposable
        {
            private IntPtr buffer;
            private int size;

            public CustomBuffer(int size)
            {
                this.size = size;
                buffer = Marshal.AllocHGlobal(size);
            }

            public void Dispose()
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = IntPtr.Zero;
                }
                GC.SuppressFinalize(this);
            }

            ~CustomBuffer()
            {
                Dispose();
            }
        }

        public static void Test()
        {
            useFixedPointer();
            useUnmanagedMemory();
        }
    }
}
