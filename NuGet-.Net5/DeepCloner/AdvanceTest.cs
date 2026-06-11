using System.Diagnostics;



using Force.DeepCloner;
using FastDeepCloner;


namespace NuGetDotNet5.DeepClone;


public class AdvanceTest
{
    public static void Test()
    {
        testPerformaneCompare();
    }

    //---------------------------------------------------------------------------------------------

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public Address Address { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    public class Address
    {
        public string City { get; set; } = "";
    }

    //---------------------------------------------------------------------------------------------

    static void testPerformaneCompare()
    {
        var users = Enumerable.Range(1, 10_000)
        .Select(i => new User
        {
            Id = i,
            Name = $"User-{i}",
            Address = new Address { City = "Seoul" },
            Tags = new List<string> { "A", "B", "C" }
        })
        .ToList();

        var sw = Stopwatch.StartNew();

        var deepClonerCopy = users.DeepClone();

        sw.Stop();
        Console.WriteLine($"DeepCloner: {sw.ElapsedMilliseconds} ms");

        sw.Restart();

        var fastDeepClonerCopy = DeepCloner.Clone(users); //FastDeepCloner

        sw.Stop();
        Console.WriteLine($"FastDeepCloner: {sw.ElapsedMilliseconds} ms");

        Console.ReadLine();
    }
}
