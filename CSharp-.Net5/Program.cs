using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSharp_Net5
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine($"Program.Main() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            //AdvancedStep.ExpressionTree.Test();
            //AdvancedStep.UnsafeContext.Test();
            //AdvancedStep.PlS.Test();
            //AdvancedStep.Span.Test();
            //AdvancedStep.InitOnlySetter.Test();
            //AdvancedStep.RequiredMember.Test();
            //AdvancedStep.DefaultInterfaceMethods.Test();
            //AdvancedStep.Reflection.Test();
            //AdvancedStep.Record.Test();
            //AdvancedStep.CallerInformationAttributes.Test();
            //AdvancedStep.InitMember.Test();
            //AdvancedStep.StaticLocalFunction.Test();
            //AdvancedStep.RawStringLiterals.Test();
            //AdvancedStep.AutoPropertyInitializer.Test();
            //AdvancedStep.DictionaryInitializer.Test();
            //AdvancedStep.ExpressionBodied.Test();
            //AdvancedStep.ExpressionTree.Test();
            //AdvancedStep.TargetTypeNew.Test();
            //AdvancedStep.NameOf.Test();
            //AdvancedStep.NullConditionalOperator.Test().Waitt();
            //AdvancedStep.Tuple.Test();
            //AdvancedStep.StringInterpolation.Test();
            //AdvancedStep.UsingDeclaration.Test();
            //AdvancedStep.UsingStatic.Test();
            //AdvancedStep.CheckedUnchecked.Test();
            //AdvancedStep.Switch.Test();
            //AdvancedStep.ArrayPool.Test();
            //AdvancedStep.MemoryPool.Test();
            //AdvancedStep.CLRMemory.Test();
            //AdvancedStep.GCFree.Test();


            //MultiThread.ValueTask.Test();
            //MultiThread.Tasking.Test();
            //MultiThread.PeriodicTimer.Test();
            //MultiThread.Parallel.Test();
            //MultiThread.AwaitInCatchBlock.Test();
            //MultiThread.AsyncAwait.Test().Wait();


            //Performance.BoxingUnboxingBenchmark.Test();
            //Performance.ConcurrencyAPIComparison.Test();


            Console.WriteLine($"Program.Main() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            Console.ReadLine();
        }
    }
}
