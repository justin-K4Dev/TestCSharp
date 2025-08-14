using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;




namespace AdvancedStep;

public class PlS
{
    /*
        ✅ LayoutKind 열거형의 종류

        ✅ 요약 비교표
          | LayoutKind   | 설명                           | 필드 순서 제어   | 마샬링/Interop 사용
          |--------------|--------------------------------|------------------|----------------------
          | Sequential   | 필드 선언 순서대로 메모리 배치 | ✅ 예           | ✅ 가능
          | Explicit     | 필드마다 수동 오프셋 지정      | ✅ 명시적       | ✅ 가능
          | Auto         | CLR이 최적화하여 자동 배치     | ❌ 불가         | ❌ 사용 금지

        ✅ 추천 사용
          | 사용 상황                     | 권장 LayoutKind
          |-------------------------------|-------------------------
          | 일반 구조체 (내부용)          | Auto 또는 생략
          | P/Invoke / C API 연동         | Sequential or Explicit
          | 공용체 표현 (union)           | Explicit
          | 커스텀 바이너리 직렬화        | Explicit
          | 패딩 없는 메모리 압축 구조체  | Explicit + [FieldOffset] 조합
   */

    /*
        LayoutKind.Sequential   
          - 필드를 선언된 순서대로 메모리에 배치합니다.
          - 구조체가 C에서 선언된 구조체와 유사한 방식으로 정렬됩니다.
          - CLR이 컴파일 시 필드 순서를 선언된 순서대로 유지
          - 하지만 필드 간 패딩(padding)은 런타임이 최적화를 위해 자동 삽입할 수 있음
          - ✅ 구조체를 P/Invoke, 바이너리 직렬화 등과 연결할 때 주로 사용
   */
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    /*
        LayoutKind.Explicit
          - 개발자가 각 필드의 메모리 오프셋을 명시적으로 지정합니다.
          - C의 공용체(union) 같은 구조를 표현하거나, 바이트 단위 컨트롤이 필요할 때 사용합니다.
          - CLR이 컴파일 시 필드 순서를 선언된 순서대로 유지
          - 하지만 필드 간 패딩(padding)은 런타임이 최적화를 위해 자동 삽입할 수 있음
          - ✅ 구조체를 P/Invoke, 바이너리 직렬화 등과 연결할 때 주로 사용
   */
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct MyUnion
    {
        [FieldOffset(0)]
        public int IntValue;

        [FieldOffset(0)]
        public float FloatValue;

        [FieldOffset(4)]
        public byte Flag;
    }

    /*
        LayoutKind.Auto(기본값)
          - CLR이 레이아웃을 자동으로 결정합니다.
          - C# 관리 코드 내에서만 사용되며, P/Invoke나 마샬링에 사용 불가능        
          - .NET 런타임이 필드 순서와 정렬을 자체적으로 최적화함
          - ⚠️ 이 레이아웃은 외부에 노출(Interop)하면 예기치 못한 동작 발생 가능
          - ❌ [StructLayout(LayoutKind.Auto)]는 Interop/PInvoke에서는 오류 발생
   */
    // 생략 시 LayoutKind.Auto가 기본
    public struct Person
    {
        public string Name;
        public int Age;
    }


    static void PIS_what()
    {
        /*
            P/Invoke = Platform Invocation Services

            P/Invoke는 .NET (C#, VB.NET 등) 코드에서
            Windows API 또는 다른 네이티브 DLL 함수(C/C++)를 호출할 수 있도록 하는 메커니즘입니다.


            즉, C# 같은 관리 언어에서
            👉 C/C++로 작성된 네이티브 라이브러리 (.dll)의 함수를 호출할 수 있도록 해주는 .NET 런타임 기능입니다.


            ✅ 핵심 구성요소
              | 구성               | 예시                          | 설명
              |--------------------|-------------------------------|------------------------------------------------------
              | [DllImport]        | [DllImport("user32.dll")]     | 외부 DLL을 지정
              | extern 메서드 선언 | public static extern int      | 외부 함수 시그니처 매핑
              |                    | MessageBox(...)               |
              | 메모리 마샬링      | 구조체/문자열/포인터 변환     | C ↔ C# 사이의 데이터 호환


            ✅ 사용의 예
              - [DllImport("user32.dll", CharSet = CharSet.Unicode)]
              - public static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);


            ✅ 언제 쓰나?
              | 상황                    | 설명
              |-------------------------|-----------------------------------------------
              | Windows API 호출        | GDI, Win32, COM 등
              | C/C++ 라이브러리 연동   | OpenCV, TensorRT, SQLite 등
              | 하드웨어/드라이버 통신  | 장치 드라이버, 센서, 시리얼 통신 등
        */

        {
            MyUnion data = new MyUnion { IntValue = 123, Flag = 1 };
            Console.WriteLine(data.IntValue); // 출력: 123
            Console.WriteLine(data.FloatValue); // IntValue와 같은 메모리 참조 → reinterpret cast, 출력: 1.72E-43
            Console.WriteLine(data.Flag); // 출력: 1
        }
    }

    public static void Test()
    {
        //PIS_what();
    }
}
