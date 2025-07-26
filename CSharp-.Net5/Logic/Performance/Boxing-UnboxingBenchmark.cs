using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;



namespace Performance;

public class BoxingUnboxingBenchmark
{
    [Benchmark]
    public void WithBoxing()
    {
        object obj;
        for (int i = 0; i < 100000; i++)
        {
            obj = i;            // Boxing
            int val = (int)obj; // Unboxing
        }
    }

    [Benchmark]
    public void WithoutBoxing()
    {
        for (int i = 0; i < 100000; i++)
        {
            int val = i;   // 값 타입 그대로 사용 (no boxing)
        }
    }


    public static void Test()
    {
        BenchmarkRunner.Run<BoxingUnboxingBenchmark>();
    }
}
