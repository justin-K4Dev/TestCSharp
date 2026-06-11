using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;

public class String
{
    static void string_Split()
    {
        // 빈 항목 제거: StringSplitOptions.RemoveEmptyEntries
        {
            string text = "a,bb,,ccc,,,dddd,,,,";
            string[] parts = text.Split(
                ',',
                options: StringSplitOptions.RemoveEmptyEntries
            );

            foreach (var p in parts)
            {
                Console.WriteLine($"[{p}]");
            }
            // [a]
            // [bb]
            // [ccc]
            // [dddd]

            Console.ReadLine();
        }
    }


    public static void Test()
    {
        //string_Split();
    }
}
