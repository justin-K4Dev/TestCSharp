using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStep
{
    public class Array
    {
        static void array_what()
        {
            /*
                배열은 일련의 동일한 데이타 타입 요소들로 구성된 데이타 집합으로서,
                인덱스를 통하여 개개의 배열요소(Element)를 엑세스할 수 있다.
                C#에서 배열은 Zero-based Array 즉 첫번째 요소가 인덱스 0를 갖는다.
                두번째 요소는 인덱스 1, 그 다음 요소들은 인덱스 2, 3, ... 등을 갖는다.

                배열의 요소는 Sqaure Bracket([ ])안에 인덱스를 넣어 엑세스한다.
                즉 배열 A의 첫번째 요소는 A[0] 으로 엑세스한다.

                C# 배열은 1차배열, 2차배열, 3차배열,..., 32차 배열 등 최고 32차 배열을 가질 수 있다.
                2차 이상의 다차원 배열은
                    (1) 각 차원별 요소 크기가 고정된 Rectangular 배열과
                    (2) 각 차원별 크기가 서로 다른 길이의 배열로 나뉠 수 있다. (계단형 배열)

                다음은 1차 배열과 2차,3차 배열(C-Style의 Rectangular 배열)을 선언하는 예이다.
                예제에서 배열 players는 10개의 string 요소를 갖도록 선언되었고,
                Regions/Depts는 각각 1차,2차 배열이며 초기화 값들을 할당하고 있다.
                마지막의 Cubes 는 3차배열을 선언한 예이다.
            */
            {
                // 1차 배열
                string[] players = new string[10];
                string[] Regions = { "서울", "경기", "부산" };

                // 2차 배열 선언 및 초기화
                string[,] Depts = { { "김과장", "경리부" }, { "이과장", "총무부" } };

                // 3차 배열 선언
                string[,,] Cubes;

                Console.ReadLine();
            }
        }


        static void jagged_array()
        {
            /*
                다차원 배열에서 각 차원별 배열 요소 크기가 동일한 Rectangular 배열은 C#에서 [,] 와
                같이 괄호안에 콤마로 분리하여 (C 언어 스타일) 다차원을 표현한다.
                하지만 각 차원별 배열 요소 크기가 가변적인 계단형 배열(Jagged Array)의 경우 [][]
                와 같이 각 차원마다 괄호를 별도로 사용한다 (Java 언어 스타일).

                계단형 배열은 배열의 배열(array of arrays)이라 불리우는데, 첫번째 차원의 크기는 컴파일 타임에 확정되어야 하고,
                그 이상 차원은 런타임시 동적으로 서로 다른 크기의 배열로 지정할 수 있다.
                이러한 계단형 배열은 각 차원별 배열 요소가 불규칙하여 Rectangular 배열처럼
                고정된 크기를 사용하면 메모리의 낭비가 심한 경우에 사용하면 유용하다.
            */
            {
                //Jagged Array (계단형 배열)
                //1차 배열 크기(3)는 명시해야
                int[][] A = new int[3][];

                //각 1차 배열 요소당 서로 다른 크기의 배열 할당 가능
                A[0] = new int[2];
                A[1] = new int[3] { 1, 2, 3 };
                A[2] = new int[4] { 1, 2, 3, 4 };

                A[0][0] = 1;
                A[0][1] = 2;

                Console.ReadLine();
            }

            {
                int[][] jagged = new int[2][];      // 2행
                jagged[0] = new int[3];             // 첫 번째 행: 3열
                jagged[1] = new int[2];             // 두 번째 행: 2열 (다를 수 있음)

                jagged[0][0] = 1;
                jagged[1][1] = 5;

                // 각 행이 독립된 배열 → 열 수를 자유롭게 조절 가능.
                // 메모리는 불연속 (각 행이 별도 힙에 저장됨).
            }

            {
                // 계단형 배열 (int[][]) 순회
                int[][] jagged = new int[3][];
                jagged[0] = new int[] { 1, 2 };
                jagged[1] = new int[] { 3, 4, 5 };
                jagged[2] = new int[] { 6 };

                for (int i = 0; i < jagged.Length; i++)
                {
                    for (int j = 0; j < jagged[i].Length; j++)
                    {
                        Console.WriteLine($"jagged[{i}][{j}] = {jagged[i][j]}");
                    }
                }
            }
        }

        static void multidimensional_arrays()
        {
            {
                int[,] matrix = new int[2, 3];  // 2행 3열
                matrix[0, 0] = 1;
                matrix[1, 2] = 5;

                // 메모리 상에 행과 열이 일정한 블록으로 연속 저장됨.
                // 모든 행은 같은 열 수를 가져야 함.

                // 행의 수 (1차원)
                int rows = matrix.GetLength(0);  // 결과: 3

                // 열의 수 (2차원):
                int cols = matrix.GetLength(1);  // 결과: 5

                // 전체의 요소 개수
                int total = matrix.Length;       // 결과 3 x 5 = 15

                Console.WriteLine($"전체개수 {total}: (행의 개수={rows}, 열의 개수={cols})");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Console.Write($"{matrix[i, j]} ");
                    }
                    Console.WriteLine(); // 줄바꿈
                }

                // 값만 순회하고 i, j 같은 인덱스를 모를 때 사용.
                foreach (int value in matrix)
                {
                    Console.WriteLine(value);
                }
            }

            {
                // 공간 좌표계의 3차원 구성 (x, y, z)
                int[,,] cube = new int[2, 3, 4]; // [면, 행, 열]

                for (int i = 0; i < cube.GetLength(0); i++)     // 1차원 (면)
                {
                    for (int j = 0; j < cube.GetLength(1); j++) // 2차원 (행)
                    {
                        for (int k = 0; k < cube.GetLength(2); k++) // 3차원 (열)
                        {
                            cube[i, j, k] = i + j + k;
                            Console.WriteLine($"cube[{i},{j},{k}] = {cube[i, j, k]}");
                        }
                    }
                }
            }

            {
                // 4개의 2차원 좌표 (x, y)
                int[,] dot2Ds = new int[,] {
                    {1, 2},   // 점 0
                    {3, 4},   // 점 1
                    {5, 6},   // 점 2
                    {7, 8}    // 점 3
                };

                // 전체 좌표 출력
                for (int i = 0; i < 4; i++)
                {
                    int x = dot2Ds[i, 0];  // i번째 점의 x좌표
                    int y = dot2Ds[i, 1];  // i번째 점의 y좌표
                    Console.WriteLine($"점 {i}: (x={x}, y={y})");
                }
            }

            {
                // 4개의 3차원 좌표: (x, y, z)
                int[,,] dots3Ds = new int[4, 1, 3]
                {
                    { { 1, 2, 3 } },   // 점 0: x=1, y=2, z=3
                    { { 4, 5, 6 } },   // 점 1
                    { { 7, 8, 9 } },   // 점 2
                    { { 10, 11, 12 } } // 점 3
                };

                // 좌표 출력
                for (int i = 0; i < 4; i++)
                {
                    int x = dots3Ds[i, 0, 0]; // x좌표
                    int y = dots3Ds[i, 0, 1]; // y좌표
                    int z = dots3Ds[i, 0, 2]; // z좌표
                    Console.WriteLine($"점 {i}: (x={x}, y={y}, z={z})");
                }
            }

            {
                // 3개의 4차원 좌표: (x, y, z, w)
                int[,] arr = new int[3, 4]
                {
                    { 1, 2, 3, 4 },
                    { 5, 6, 7, 8 },
                    { 9, 10, 11, 12 }
                };

                int rows = arr.GetLength(0); // 행
                int cols = arr.GetLength(1); // 열

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Console.Write($"{arr[i, j]} ");
                    }
                    Console.WriteLine(); // 줄바꿈
                }
            }
        }

        static void array_use()
        {
            /*
                C# 배열은 각 요소를 인덱스를 사용하여 엑세스한다.
                예를 들어 Regions 배열의 첫번째 요소는 Regions[0]으로, 두번째 요소는 Regions[1]로 표시된다.

                모든 C# 배열은 내부적으로 .NET Framework의 System.Array에서 파생된 것이다.
                따라서, System.Array의 메소드, 프로퍼티를 사용할 수 있다.

                배열은 고정된 크기를 가지며, 배열의 사이즈와 상관없이 한 요소를 엑세스하는 시간은 인덱스를 사용할 경우 O(1)이 된다.
                하지만 인덱스를 알지 못하고 소트되지 않은 배열에서 값으로 데이타를 찾기 위해서는 O(n)의 시간이 소요된다.
                소트된 배열에서 값을 찾는 경우는 Binary Search를 이용할 수 있으므로 O(log N)의 시간이 소요된다. 

                다음 예제는 점수 배열을 하나 하나 엑세스하면서 총합을 구하는 예이다.
                아래 예제에서 배열 scores의 사이즈를 구하기 위하여 scores.Length 처럼 .Length 속성을 사용하였는데,
                이는 System.Array 클래스에 정의된 속성이다.  
            */
            {
                int sum = 0;
                int[] scores = { 80, 78, 60, 90, 100 };
                for (int i = 0; i < scores.Length; i++)
                {
                    sum += scores[i];
                }

                Console.WriteLine(sum);

                Console.ReadLine();
            }

            /*
                100이하의 10개의 랜덤 숫자를 배열에 넣고 그 배열 요소들의 합을 구하는 예제.
            */
            {
                int sum = 0;
                int[] nums = new int[10];

                Random rand = new Random();
                for (int i = 0; i < nums.Length; i++)
                {
                    nums[i] = rand.Next() % 100;
                }

                for (int i = 0; i < nums.Length; i++)
                {
                    sum += nums[i];
                }

                Console.WriteLine(sum);

                Console.ReadLine();
            }
        }


        static int CalculateSum(int[] scoresArray) // 배열 받는 쪽
        {
            int sum = 0;
            for (int i = 0; i < scoresArray.Length; i++)
            {
                sum += scoresArray[i];
            }
            return sum;
        }

        static void array_forward()
        {
            /*
                C#에서 배열 전체를 전달하기 위해서는 보내는 쪽에서는 배열명을 사용하고,
                받는 쪽에서 동일한 배열타입의 배열을 받아들이면 된다.

                배열은 레퍼런스(Reference) 타입이기 때문에, 배열을 다른 객체나 메서드에 전달할 때,
                직접 모든 배열 데이타를 복사하지 않고, 배열 전체를 가리키는 참조 값(Reference pointer)만을 전달한다.
                즉, 전달하는 쪽에서는 단순히 레퍼런스인 배열명을 사용하며,
                받는 쪽에서는 아래 예제와 같이 배열 데이타 타입 및 배열 파라미터명을 사용한다.  
            */
            {
                int[] scores = { 80, 78, 60, 90, 100 };
                int sum = Array.CalculateSum(scores); // 배열 전달: 배열명 사용
                Console.WriteLine(sum);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //array_forward();

            //array_use();

            //jagged_array();

            //multidimensional_arrays();

            //array_what();
        }
    }
}
