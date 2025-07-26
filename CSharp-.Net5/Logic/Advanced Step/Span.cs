using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;



namespace AdvancedStep;


public class Span
{
    public static void Span_what()
    {
        /*
            Span<T>는 C#에서 연속된 메모리 블록을 안전하고 효율적으로 다루기 위한 구조체입니다.
            힙이 아닌 스택 메모리, 배열, 슬라이스, 포인터 등의 메모리 영역을 복사 없이 참조할 수 있습니다.

            - System.Span<T>는 struct (값 타입)이며, 힙 할당 없이 GC 부담 없음
            - 메모리 슬라이스(부분 참조), 인덱싱, 범위 연산 등을 제공

            ✅ 안전하게 사용 가능 (범위 검사 포함)


            ✅ 주요 특징
            항목	            설명
            타입	            ref struct (스택 전용 구조체)
            GC 부담	            없음 (힙에 할당되지 않음)
            대상	            배열, stackalloc, 포인터, 문자열 등
            기능	            슬라이싱, 범위 접근, 복사, 변환
            불변성	            ReadOnlySpan<T> 제공 (읽기 전용)


            ✅ 언제 쓰나?
            용도	                  설명
            고성능 문자열 처리	      Span<char>을 사용해 복사 없이 부분 문자열 처리
            배열 슬라이스	          Span<byte>로 배열 일부만 참조
            스택 버퍼 처리	          stackalloc과 함께 GC 없이 메모리 처리
            unsafe 대체	              안전한 범위 검사 + 성능을 함께 제공
       */
        {
            // ✅ 복사 없이 배열의 일부를 슬라이스하여 직접 수정

            int[] numbers = { 1, 2, 3, 4, 5 };
            Span<int> span = numbers.AsSpan(1, 3); // { 2, 3, 4 }

            span[0] = 99;
            Console.WriteLine(numbers[1]); // 출력: 99
        }
    }

    static void use_Span_with_stackalloc()
    {
        // ✅ GC 힙 할당 없이 스택에 버퍼 생성 → 매우 빠름

        Span<int> span = stackalloc int[5];
        for (int i = 0; i < span.Length; i++)
            span[i] = i * 10;

        foreach (var item in span)
            Console.WriteLine(item);

        Console.WriteLine();
    }

    static void use_Span_with_ReadOnlySpan()
    {
        // ✅ GC 힙 할당 없이 스택에 버퍼 생성 → 매우 빠름

        ReadOnlySpan<char> slice = "Hello World".AsSpan(0, 5);
        Console.WriteLine(slice.ToString()); // 출력: Hello

        // 읽기 전용
        // 문자열 조작, 읽기 최적화 루틴에서 사용
    }

    static void use_Span_with_string()
    {
        // 한 번에 문자열을 직접 생성하며, 할당과 복사를 한 번에 처리
        // 내부적으로 Span<char> 를 이용해 효율적인 초기화
        {
            int value = 123;
            string result = string.Create(4, value, (span, val) =>
            {
                span[0] = '0';
                span[1] = 'x';
                span[2] = (char)('0' + ((val / 10) % 10));
                span[3] = (char)('0' + (val % 10));
            });
        }
    }


    public static void Test()
    {
        //use_Span_with_string();

        //use_Span_with_ReadOnlySpan();

        //use_Span_with_stackalloc();

        //Span_what();
    }
}
