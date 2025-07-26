using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace UsefulInterfaces
{
    public class CollectionEnum
    {
        public class FixedSizeList<T> : System.Collections.Generic.IList<T>
        {
            private T[] _array;
            public FixedSizeList(int size) { _array = new T[size]; }
            public T this[int index] { get { return _array[index]; } set { _array[index] = value; } }
            public int Count => _array.Length;
            public bool IsReadOnly => false;
            public void Add(T item) { throw new NotSupportedException(); }
            public void Clear() => Array.Clear(_array, 0, _array.Length);
            public bool Contains(T item) => Array.IndexOf(_array, item) != -1;
            public void CopyTo(T[] array, int arrayIndex) => _array.CopyTo(array, arrayIndex);
            public System.Collections.Generic.IEnumerator<T> GetEnumerator() => ((System.Collections.Generic.IEnumerable<T>)_array).GetEnumerator();
            public int IndexOf(T item) => Array.IndexOf(_array, item);
            public void Insert(int index, T item) => _array[index] = item;
            public bool Remove(T item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { _array[index] = default(T); }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        static void override_ICollectionT()
        {
            /*
                ICollection<T>, IList<T>, IDictionary<TKey, TValue>

                ✅ 목적
                  - 커스텀 컬렉션 직접 구현(특수 데이터구조 등)
                  - 리스트, 딕셔너리 동작 커스터마이징
            */

            {
                var original = new List<string> { "apple", "banana", "cherry" };
                var roList = new ReadOnlyList<string>(original);

                Console.WriteLine("ReadOnlyList contents:");
                foreach (var s in roList)
                    Console.WriteLine(s);

                Console.WriteLine($"Contains 'banana'? {roList.Contains("banana")}");
                Console.WriteLine($"Index of 'cherry': {roList.IndexOf("cherry")}");

                // 읽기는 가능
                Console.WriteLine("Item at index 1: " + roList[1]);

                // 쓰기 시도: 예외 발생
                try
                {
                    roList[0] = "orange";
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine($"Set item throws: {ex.Message}");
                }

                try
                {
                    roList.Add("orange");
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine($"Add() throws: {ex.Message}");
                }
            }

            {
                // 크기 5짜리 정수형 FixedSizeList 생성
                var list = new FixedSizeList<int>(5);

                // 값 할당
                for (int i = 0; i < list.Count; i++)
                    list[i] = (i + 1) * 10;

                // 값 읽기 및 출력
                Console.WriteLine("List contents:");
                foreach (var n in list)
                    Console.WriteLine(n);

                // Contains, IndexOf 테스트
                Console.WriteLine("\nContains 30? " + list.Contains(30)); // true
                Console.WriteLine("IndexOf 40: " + list.IndexOf(40));     // 3

                // Insert (index 값 덮어쓰기)
                list.Insert(2, 99);
                Console.WriteLine("\nAfter Insert(2, 99):");
                foreach (var n in list)
                    Console.WriteLine(n);

                // RemoveAt 테스트 (해당 위치 0으로 초기화)
                list.RemoveAt(4);
                Console.WriteLine("\nAfter RemoveAt(4):");
                foreach (var n in list)
                    Console.WriteLine(n);

                // Add, Remove는 NotSupportedException 발생
                try { list.Add(123); }
                catch (NotSupportedException ex) { Console.WriteLine("\nAdd() throws: " + ex.Message); }

                try { list.Remove(99); }
                catch (NotSupportedException ex) { Console.WriteLine("Remove() throws: " + ex.Message); }

                // Clear() 테스트
                list.Clear();
                Console.WriteLine("\nAfter Clear():");
                foreach (var n in list)
                    Console.WriteLine(n);
            }
        }

        public class Range : System.Collections.Generic.IEnumerable<int>
        {
            int _start, _count;
            public Range(int start, int count) { _start = start; _count = count; }

            public System.Collections.Generic.IEnumerator<int> GetEnumerator()
            {
                for (int i = 0; i < _count; ++i)
                    yield return _start + i;
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }

        static void override_IEnumerableT()
        {
            /*
                IEnumerable<T> / IEnumerator<T>

                ✅ 목적
                  - foreach 지원, 컬렉션/시퀀스 객체 직접 구현
                  - 커스텀 컬렉션, 필터링된 시퀀스 등 만들 때
            */
            {
                foreach (var n in new Range(5, 3)) // 5,6,7 출력
                    Console.WriteLine(n);
            }
        }


        public class MyCollection : System.Collections.IEnumerable
        {
            private int[] _items = { 1, 2, 3, 4 };
            public System.Collections.IEnumerator GetEnumerator()
            {
                for (int i = 0; i < _items.Length; i++)
                    yield return _items[i];
            }
        }

        static void override_IEnumerable()
        {
            /*
                IEnumerable / IEnumerator (비제네릭)

                ✅ 목적
                  - 구버전 프레임워크, 혹은 일반적인 컬렉션 루프 지원
                  - 특별한 경우 아니면 IEnumerable<T>를 쓰는 것이 좋지만, 프레임워크 통합할 때 필요
            */

            var coll = new MyCollection();

            // IEnumerable 구현으로 foreach 사용 가능
            foreach (var item in coll)
            {
                Console.WriteLine(item);
            }
        }

        public class ReadOnlyList<T> : System.Collections.Generic.IList<T>
        {
            private System.Collections.Generic.List<T> inner = new System.Collections.Generic.List<T>();

            // 생성자: 기존 리스트로 초기화 가능
            public ReadOnlyList(System.Collections.Generic.IEnumerable<T> collection)
            {
                inner.AddRange(collection);
            }

            // 인덱서 - 읽기만 허용
            public T this[int i]
            {
                get { return inner[i]; }
                set { throw new NotSupportedException(); }
            }

            public int Count => inner.Count;
            public bool IsReadOnly => true;

            public void Add(T item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public bool Contains(T item) => inner.Contains(item);
            public void CopyTo(T[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);
            public System.Collections.Generic.IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
            public int IndexOf(T item) => inner.IndexOf(item);
            public void Insert(int index, T item) { throw new NotSupportedException(); }
            public bool Remove(T item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
        }

        // 읽기전용 리스트 클래스 (내부 데이터를 배열로 보관)
        public class MyReadOnlyList<T> : System.Collections.Generic.IReadOnlyList<T>
        {
            private readonly T[] _items;

            // 생성자: 컬렉션으로부터 초기화
            public MyReadOnlyList(System.Collections.Generic.IEnumerable<T> collection)
            {
                var arr = collection as T[];
                _items = (null != arr) ? arr : new System.Collections.Generic.List<T>(collection).ToArray();
            }

            // 인덱서: 읽기 전용
            public T this[int index] => _items[index];

            // Count: 요소 개수
            public int Count => _items.Length;

            // 열거 지원
            public System.Collections.Generic.IEnumerator<T> GetEnumerator() => ((System.Collections.Generic.IEnumerable<T>)_items).GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _items.GetEnumerator();
        }

        public class MyReadOnlyCollection<T> : System.Collections.Generic.IReadOnlyCollection<T>
        {
            private readonly T[] _items;

            public MyReadOnlyCollection(System.Collections.Generic.IEnumerable<T> collection)
            {
                var arr = collection as T[];
                _items = (null != arr) ? arr : new List<T>(collection).ToArray();
            }

            public int Count => _items.Length;

            public System.Collections.Generic.IEnumerator<T> GetEnumerator() => ((System.Collections.Generic.IEnumerable<T>)_items).GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _items.GetEnumerator();
        }

        public class MyReadOnlyDictionary<TKey, TValue> : System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>
        {
            private readonly Dictionary<TKey, TValue> _dict;

            public MyReadOnlyDictionary(IDictionary<TKey, TValue> source)
            {
                _dict = new Dictionary<TKey, TValue>(source); // 내부 복사, 불변
            }

            public TValue this[TKey key] => _dict[key];

            public IEnumerable<TKey> Keys => _dict.Keys;
            public IEnumerable<TValue> Values => _dict.Values;
            public int Count => _dict.Count;
            public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
            public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
            public System.Collections.Generic.IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _dict.GetEnumerator();
        }

        static void override_IReadOnlyCollection()
        {
            /*
                IReadOnlyCollection<T>, IReadOnlyList<T>, IReadOnlyDictionary<TKey, TValue>

                ✅ 목적
                  - 읽기 전용 컬렉션 계약, 불변 컬렉션 설계
            */
            {

                // IReadOnlyCollection<T> 예제 (Count만 있고 인덱서 없음)
                IReadOnlyCollection<int> numbers = new HashSet<int> { 10, 20, 30, 40 };
                Console.WriteLine("\nIReadOnlyCollection<T> Test:");
                Console.WriteLine($"Count: {numbers.Count}");
                foreach (var n in numbers)
                    Console.WriteLine(n);


                // IReadOnlyList<T> 예제
                IReadOnlyList<string> fruits = new List<string> { "Apple", "Banana", "Cherry" };
                Console.WriteLine("IReadOnlyList<T> Test:");
                for (int i = 0; i < fruits.Count; i++)
                    Console.WriteLine($"fruits[{i}]: {fruits[i]}");


                // IReadOnlyDictionary<TKey, TValue> 예제
                IReadOnlyDictionary<string, int> scores = new Dictionary<string, int>
                {
                    ["Alice"] = 90,
                    ["Bob"] = 80
                };
                Console.WriteLine("\nIReadOnlyDictionary<TKey, TValue> Test:");
                foreach (var key in scores.Keys)
                    Console.WriteLine($"{key}: {scores[key]}");
                Console.WriteLine($"ContainsKey(\"Bob\"): {scores.ContainsKey("Bob")}");
                int v;
                bool found = scores.TryGetValue("Carol", out v);
                Console.WriteLine($"TryGetValue(\"Carol\", out v): {found} (value: {v})");
            }

            {
                var numbers = new[] { 1, 2, 3, 4 };
                var readOnlyCol = new MyReadOnlyCollection<int>(numbers);

                Console.WriteLine("MyReadOnlyCollection contents:");
                foreach (var n in readOnlyCol)
                    Console.WriteLine(n);
                Console.WriteLine($"Count: {readOnlyCol.Count}");
            }

            {
                var source = new List<int> { 10, 20, 30 };
                var roList = new MyReadOnlyList<int>(source);

                Console.WriteLine("MyReadOnlyList contents:");
                for (int i = 0; i < roList.Count; i++)
                    Console.WriteLine($"[{i}]: {roList[i]}");

                Console.WriteLine("foreach:");
                foreach (var v in roList)
                    Console.WriteLine(v);

                // 읽기전용이므로 쓰기는 불가 (컴파일 오류)
                // roList[1] = 123; // Error!

                // 내부 컬렉션 변경과는 무관 (불변)
                source[1] = 99;
                Console.WriteLine($"roList[1] (after source[1]=99): {roList[1]}"); // 20, 원본과 독립
            }

            {
                var srcDict = new Dictionary<string, int>
                {
                    ["apple"] = 100,
                    ["banana"] = 200
                };

                var roDict = new MyReadOnlyDictionary<string, int>(srcDict);

                Console.WriteLine("MyReadOnlyDictionary contents:");
                foreach (var kv in roDict)
                    Console.WriteLine($"{kv.Key}: {kv.Value}");

                Console.WriteLine($"Count: {roDict.Count}");
                Console.WriteLine($"ContainsKey(\"apple\"): {roDict.ContainsKey("apple")}");
                int v;
                bool found = roDict.TryGetValue("banana", out v);
                Console.WriteLine($"TryGetValue(\"banana\", out v): {found} (value: {v})");

                // 읽기전용이라 set 불가, roDict["apple"] = 123; // 컴파일 에러
            }
        }

        public static void Test()
        {
            override_IReadOnlyCollection();

            override_IEnumerable();

            override_IEnumerableT();

            override_ICollectionT();
        }
    }
}
