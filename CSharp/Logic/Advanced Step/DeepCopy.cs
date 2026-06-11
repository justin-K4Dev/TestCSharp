using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static AdvancedStep.DeepCopy;
using static UsefulInterfaces.LINQ;

namespace AdvancedStep
{
    public class DeepCopy
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; } = new Address();
            public List<string> Tags { get; set; } = new List<string>();
        }

        public class Address
        {
            public string City { get; set; }
        }

        //---------------------------------------------------------------------------------------------

        public static User Clone(User source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new User
            {
                Id = source.Id,
                Name = source.Name,
                Address = Clone(source.Address),
                Tags = source.Tags is null
                    ? new List<string>()
                    : new List<string>(source.Tags)
            };
        }

        public static Address Clone(Address source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Address
            {
                City = source.City
            };
        }

        public static List<T> CloneList<T>(
            List<T> source,
            Func<T, T> itemClone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(itemClone).ToList();
        }

        public static HashSet<T> CloneHashSet<T>(
            HashSet<T> source,
            Func<T, T> itemClone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new HashSet<T>(
                source.Select(itemClone),
                source.Comparer
            );
        }

        public static Dictionary<TKey, TValue> CloneDictionary<TKey, TValue>(
            Dictionary<TKey, TValue> source,
            Func<TValue, TValue> valueClone)
            where TKey : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.ToDictionary(
                pair => pair.Key,
                pair => valueClone(pair.Value),
                source.Comparer
            );
        }

        public static Queue<T> CloneQueue<T>(
            Queue<T> source,
            Func<T, T> itemClone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Queue<T>(source.Select(itemClone));
        }

        public static Stack<T> CloneStack<T>(
            Stack<T> source,
            Func<T, T> itemClone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Stack<T>(
                source.Reverse().Select(itemClone)
            );
        }

        public static ConcurrentDictionary<TKey, TValue> CloneConcurrentDictionary<TKey, TValue>(
            ConcurrentDictionary<TKey, TValue> source,
            Func<TValue, TValue> valueClone)
            where TKey : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ConcurrentDictionary<TKey, TValue>(
                source.Select(pair =>
                    new KeyValuePair<TKey, TValue>(
                        pair.Key,
                        valueClone(pair.Value)))
            );
        }

        public static ConcurrentQueue<T> CloneConcurrentQueue<T>(
            ConcurrentQueue<T> source,
            Func<T, T> itemClone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ConcurrentQueue<T>(
                source.Select(itemClone)
            );
        }

        public static ConcurrentBag<T> CloneConcurrentBag<T>(
            ConcurrentBag<T> source,
            Func<T, T> itemClone)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ConcurrentBag<T>(
                source.Select(itemClone)
            );
        }

        //=========================================================================================

        static User CreateUser(int id, string name, string city)
        {
            return new User
            {
                Id = id,
                Name = name,
                Address = new Address { City = city },
                Tags = new List<string> { "Admin", "User" }
            };
        }

        static void Print(string title, string original, string copy)
        {
            Console.WriteLine($"[{title}]");
            Console.WriteLine($"Original: {original}");
            Console.WriteLine($"Copy    : {copy}");
            Console.WriteLine();
        }

        static void testUser()
        {
            var source = CreateUser(1, "Kim", "Seoul");

            var copy = DeepCopy.Clone(source);

            copy.Name = "Lee";
            copy.Address.City = "Busan";
            copy.Tags.Add("Manager");

            Print("User", source.Address.City, copy.Address.City);
            Console.WriteLine(source.Tags.Count); // 2
            Console.WriteLine(copy.Tags.Count);   // 3
            Console.WriteLine();
        }

        static void testList()
        {
            var source = new List<User>
        {
            CreateUser(1, "Kim", "Seoul")
        };

            var copy = DeepCopy.CloneList(
                source,
                DeepCopy.Clone
            );

            copy[0].Address.City = "Busan";

            Print("List<User>", source[0].Address.City, copy[0].Address.City);
        }

        static void testHashSet()
        {
            var source = new HashSet<User>
        {
            CreateUser(1, "Kim", "Seoul")
        };

            var copy = DeepCopy.CloneHashSet(
                source,
                DeepCopy.Clone
            );

            foreach (var user in copy)
            {
                user.Address.City = "Busan";
                break;
            }

            var originalUser = source.First();
            var copiedUser = copy.First();

            Print("HashSet<User>", originalUser.Address.City, copiedUser.Address.City);
        }

        static void testDictionary()
        {
            var source = new Dictionary<string, User>
            {
                ["user1"] = CreateUser(1, "Kim", "Seoul")
            };

            var copy = DeepCopy.CloneDictionary(
                source,
                DeepCopy.Clone
            );

            copy["user1"].Address.City = "Busan";

            Print("Dictionary<string, User>", source["user1"].Address.City, copy["user1"].Address.City);
        }

        static void testQueue()
        {
            var source = new Queue<User>();
            source.Enqueue(CreateUser(1, "Kim", "Seoul"));

            var copy = DeepCopy.CloneQueue(
                source,
                DeepCopy.Clone
            );

            copy.Peek().Address.City = "Busan";

            Print("Queue<User>", source.Peek().Address.City, copy.Peek().Address.City);
        }

        static void testStack()
        {
            var source = new Stack<User>();
            source.Push(CreateUser(1, "Kim", "Seoul"));

            var copy = DeepCopy.CloneStack(
                source,
                DeepCopy.Clone
            );

            copy.Peek().Address.City = "Busan";

            Print("Stack<User>", source.Peek().Address.City, copy.Peek().Address.City);
        }

        static void testConcurrentDictionary()
        {
            var source = new ConcurrentDictionary<string, User>();
            source["user1"] = CreateUser(1, "Kim", "Seoul");

            var copy = DeepCopy.CloneConcurrentDictionary(
                source,
                DeepCopy.Clone
            );

            copy["user1"].Address.City = "Busan";

            Print("ConcurrentDictionary<string, User>", source["user1"].Address.City, copy["user1"].Address.City);
        }

        static void testConcurrentQueue()
        {
            var source = new ConcurrentQueue<User>();
            source.Enqueue(CreateUser(1, "Kim", "Seoul"));

            var copy = DeepCopy.CloneConcurrentQueue(
                source,
                DeepCopy.Clone
            );

            copy.TryPeek(out var copiedUser);
            source.TryPeek(out var originalUser);

            copiedUser.Address.City = "Busan";

            Print("ConcurrentQueue<User>", originalUser.Address.City, copiedUser.Address.City);
        }

        static void testConcurrentBag()
        {
            var source = new ConcurrentBag<User>();
            source.Add(CreateUser(1, "Kim", "Seoul"));

            var copy = DeepCopy.CloneConcurrentBag(
                source,
                DeepCopy.Clone
            );

            var originalUser = source.First();
            var copiedUser = copy.First();

            copiedUser.Address.City = "Busan";

            Print("ConcurrentBag<User>", originalUser.Address.City, copiedUser.Address.City);
        }

        //-----------------------------------------------------------------------------------------

        public static void Test()
        {
            //testUser();
            //testList();
            //testHashSet();
            //testDictionary();
            //testQueue();
            //testStack();
            //testConcurrentDictionary();
            //testConcurrentQueue();
            //testConcurrentBag();
        }
    }
}
