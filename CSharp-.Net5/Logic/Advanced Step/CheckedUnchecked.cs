using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace CSharp
{
    public class CheckedUnchecked
    {
        static void CheckedUnchecked_what()
        {
            /*
                checked
                  - 정수형 연산(+, -, *, 등)에서 오버플로우(overflow) 발생시 예외(OverflowException)를 강제로 발생시키는 키워드
                  - OverflowException은 일반적인 예외처럼 try-catch 블록에서 포착 가능 !!!

                unchecked
                  - 오버플로우가 발생해도 예외 없이 wrap-around 값만 반환하도록 하는 키워드
                  - try-catch로 감싸도 예외가 발생하지 않는다 !!!.

                📌 wrap-around 란?
                  - 정수형 데이터에서 최댓값/최솟값을 넘었을 때 값이 다시 반대편으로 "돌아가는" 현상을 뜻
                  - 시계 바늘이 12시에서 1시간 더하면 다시 1시로 돌아가는 것처럼
                  - 예)

                    int.MaxValue(= 2,147,483,647)에 1을 더하면
                    → **int.MinValue(-2,147,483,648)**로 돌아감

                    int.MinValue에 1을 빼면
                    → int.MaxValue로 돌아감
            */

            {
                int max = int.MaxValue;
                int min = int.MinValue;

                // unchecked (wrap-around, 예외 없음)
                try
                {
                    unchecked
                    {
                        int u = max + 1;
                        Console.WriteLine($"unchecked: int.MaxValue + 1 = {u}"); // -2147483648
                    }
                }
                catch (OverflowException)
                {
                    Console.WriteLine("unchecked: 예외 발생");
                }

                // checked (예외 발생)
                try
                {
                    checked
                    {
                        int c = max + 1;
                        Console.WriteLine($"checked: int.MaxValue + 1 = {c}");
                    }
                }
                catch (OverflowException)
                {
                    Console.WriteLine("checked: OverflowException 발생!");
                }

                // checked 블록 없이 checked 식 사용도 가능
                try
                {
                    int c2 = checked(min - 1);
                    Console.WriteLine($"checked: int.MinValue - 1 = {c2}");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("checked (식): OverflowException 발생!");
                }

                /*
                    unchecked: int.MaxValue + 1 = -2147483648
                    checked: OverflowException 발생!
                    checked (식): OverflowException 발생!            
                */
            }

            {
                // wrap-around의 예시

                int x = int.MaxValue;    // 2147483647
                int y = x + 1;           // wrap-around 발생! 결과: -2147483648 (int.MinValue)
                Console.WriteLine(y);    // -2147483648
            }
        }

        public static void Test()
        {
            CheckedUnchecked_what();
        }
    }
}
