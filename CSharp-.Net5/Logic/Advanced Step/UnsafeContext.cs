using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;


public class UnsafeContext
{
    static void UnsafeContext_what()
    {
        /*
            unsafe는 C#에서 포인터(pointer), 주소 연산자(&), 역참조(*) 등을 허용하는
            "비관리 메모리 접근"을 가능하게 하는 컨텍스트입니다.

            즉, .NET의 메모리 안전성, 가비지 컬렉션, 타입 검사를 우회하여 C/C++ 수준의 저수준 메모리 작업이 가능합니다.

            ✅ 주요 특징
            항목	                    설명
            키워드	                    unsafe
            주요 연산자	                *, &, ->, sizeof, stackalloc, fixed
            필요 설정	                /unsafe 컴파일 옵션 또는 <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
            메모리 안전성	            ❌ 보장되지 않음 (C와 동일한 포인터 위험 존재)
            가비지 컬렉션(GC)	        우회하거나 충돌 가능 (고정 필요)
            CLR 검증	                일부 코드가 type-safe 하지 않음

            ✅ 사용 목적
            목적	                    설명
            성능 최적화	                배열, 버퍼, 구조체에 직접 접근하여 속도 향상
            Interop	                    C/C++ 라이브러리와 포인터 기반 구조체 공유
            메모리 고정	                GC가 개입하지 않도록 fixed로 주소 유지
            스택 버퍼	                stackalloc으로 스택 기반 버퍼 생성
        */
    }

    static unsafe void use_unsafe()
    {
        //x의 주소를 포인터 p로 얻고, 역참조하여 값을 직접 변경함

        int x = 10;
        int* p = &x;
        *p = 20;

        Console.WriteLine(*p); // 20
    }

    static unsafe void use_unsafe_with_Array()
    {
        // 배열을 fixed로 고정하여 포인터를 통해 직접 수정

        int[] arr = new int[] { 1, 2, 3 };
        fixed (int* p = arr)
        {
            *(p + 1) = 42;
        }
        Console.WriteLine(arr[1]); // 출력: 42
    }

    static void use_stackalloc_Span()
    {
        // 스택 메모리(stackalloc) 를 Span<T>으로 안전하게 다루는 패턴 <= unsafe 같은 효과
        // GC 힙을 사용하지 않고 고속 메모리 버퍼 처리 가능

        Span<int> buffer = stackalloc int[5]; // 스택에 int 배열 5개 할당

        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = i * 10;

        foreach (var val in buffer)
            Console.WriteLine(val); // 출력: 0 10 20 30 40
    }

    /*
        🔧 C 함수 예 (C 언어로 작성되어 있다고 가정)

        // C 코드 (native.dll 내 함수)
        typedef struct {
            int x;
            int y;
        } Point;

        __declspec(dllexport) void SetPoint(Point* p)
        {
            p->x += 1;
            p->y += 2;
        }     
   */
    // 🔧 C# 인터페이스 정의 및 호출
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
    }

    public static class NativeMethods
    {
        [DllImport("native.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void SetPoint(Point* p);
    }

    static unsafe void use_unsafe_with_PIS()
    {
        Point pt = new Point { x = 10, y = 20 };

        Point* p = &pt; // ✅ 스택 변수는 fixed 없이 주소 사용 가능
        NativeMethods.SetPoint(p);

        Console.WriteLine($"x = {pt.x}, y = {pt.y}"); // x = 11, y = 22
    }

    public static void Test()
    {
        //use_unsafe_with_PIS();

        //use_stackalloc_Span();

        //use_unsafe_with_Array();

        //use_unsafe();

        //UnsafeContext_what();
    }
}
