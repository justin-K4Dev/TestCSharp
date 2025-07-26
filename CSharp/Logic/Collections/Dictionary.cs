using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Concurrent;
using System.Threading;



namespace Collections
{
    public class Dictionary
    {
		static void dictionary_what()
		{
			/*
                .NET에 Generic방식으로 해시테이블을 구현한 클래스로 Dictionary<Tkey,TValue> 클래스가 있다.
                Dictionary는 Key값과 Value값 모두 Strong type을 받아들이며, 박싱/언박싱을 일으키지 않는다.
                Dictionary는 Chaining 방식을 사용하여 Collision Resolution을 하게 된다.
                이 자료구조는 추가, 삭제, 검색에서 O(1)의 시간이 소요된다.
            */

			Dictionary<string, int> dic = new Dictionary<string, int>()
			{
					{"dog", 2}
				,   {"iguana", -1}
				,   {"cat", 1}
				,   {"cow", 0}
			};

			foreach (KeyValuePair<string, int> pair in dic)
			{
				Console.WriteLine("{0}, {1}"
								 , pair.Key
								 , pair.Value);
			}

			foreach (var pair in dic)
			{
				Console.WriteLine("{0}, {1}"
								 , pair.Key
								 , pair.Value);
			}


			Console.ReadLine();
		}

		static void Dictionary_use()
		{
			Dictionary<int, string> emp = new Dictionary<int, string>();
			emp.Add(1001, "Jane");
			emp.Add(1002, "Tom");
			emp.Add(1003, "Cindy");

			string name = emp[1002];
			Console.WriteLine(name);

			Console.ReadLine();
		}

		static void dictionary_keys_to_list()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>()
            {
                {"dog", 2},
                {"iguana", -1},
                {"cat", 1},
                {"cow", 0}
            };

            List<string> list = new List<string>(dic.Keys);

            foreach (string k in list)
            {
                Console.WriteLine("{0}, {1}"
                                 , k
                                 , dic[k]);
            }

            Console.ReadLine();
        }

        static void dictionary_keys_value_check()
        {
            //key 체크
            {
                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(100, "Jhon");
                dict.Add(200, "Steven");

                if (dict.ContainsKey(200))
                {
                    Console.WriteLine(true);
                }
            }

            //value 체크
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                dict.Add("dog", 1);
                dict.Add("cow", 2);

                if (dict.ContainsValue(1))
                {
                    Console.WriteLine(true);
                }
            }

            Console.ReadLine();
        }

		static void dictionary_indexer()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();

			dictionary[1] = 2;
			dictionary[2] = 1;
			dictionary[3] = 3;

			Console.WriteLine(dictionary[1]);
			Console.WriteLine(dictionary[2]);

			Console.ReadLine();
		}

		public class CustomKey
		{
			public int CustomID { get; set; }
			public string CustomName { get; set; }
		}

		class CustomComparer : IEqualityComparer<KeyValuePair<int, CustomKey>>
		{
			public bool Equals(KeyValuePair<int, CustomKey> x, KeyValuePair<int, CustomKey> y)
			{
				if (   x.Key == y.Key
					&& (x.Value.CustomID == y.Value.CustomID)
					&& (x.Value.CustomName == y.Value.CustomName))
				{
					return true;
				}

				return false;
			}

			public int GetHashCode(KeyValuePair<int, CustomKey> obj)
			{
				return string.Format($"{obj.Value.CustomID}_{obj.Value.CustomName}").GetHashCode();
			}
		}

		static void dictionary_custom_compare()
		{
			IDictionary<int, CustomKey> customDic = new Dictionary<int, CustomKey>()
			{
				{ 1, new CustomKey(){ CustomID =1, CustomName = "Bill"}}
			,   { 2, new CustomKey(){ CustomID =2, CustomName = "Steve"}}
			,   { 3, new CustomKey(){ CustomID =3, CustomName = "Ram"}}
			};

			CustomKey std = new CustomKey() { CustomID = 1, CustomName = "Bill" };

			KeyValuePair<int, CustomKey> elementToFind = new KeyValuePair<int, CustomKey>(1, std);

			customDic.Contains(elementToFind, new CustomComparer()); // returns true

			Console.ReadLine();
		}

        public class EntityKey
		{
			public int Id { get; set; }
			public string Name { get; set; }

			public EntityKey(int id, string name)
			{
				this.Id = id;
				this.Name = name;
			}

			public override bool Equals(object obj)
			{
				var entity = obj as EntityKey;
				if (entity == null)
				{
					return false;
				}

				return entity.Id == this.Id
					   && entity.Name == this.Name;
			}

			public override int GetHashCode()
			{
				return string.Format($"{this.Id}_{this.Name}").GetHashCode();
			}
		}

		static void dictionary_custom_key()
        {
            var key = new EntityKey(3, "test");
            var entityDic = new Dictionary<EntityKey, string>(5) { { key, "Hello!" } };

            var foundObj = entityDic[key]; // found key

            //key.Id = 4;
            //var obj1 = entityDic[key]; // KeyNotFoundException

            var obj3 = entityDic[new EntityKey(3, "test")]; // found key

            var obj4 = entityDic[new EntityKey(0, "test")]; // KeyNotFoundException

            //var obj5 = entityDic[new EntityKey(3, "justin")]; // KeyNotFoundException

            Console.ReadLine();
        }

		//Struct.
		struct SomeStruct
		{
			public int a, b;
		}

		class StructComparer : IEqualityComparer<SomeStruct>
		{
			bool IEqualityComparer<SomeStruct>.Equals(SomeStruct x, SomeStruct y)
			{
				return x.a == y.a && x.b == y.b;
			}

			int IEqualityComparer<SomeStruct>.GetHashCode(SomeStruct obj)
			{
				return obj.a ^ obj.b;
			}
		}

		//Enum.
		enum SomeEnum
		{
			StateA = 1
		,	StateB = 2
		}

		class EnumComparer : IEqualityComparer<SomeEnum>
		{
			bool IEqualityComparer<SomeEnum>.Equals(SomeEnum x, SomeEnum y)
			{
				return (int)x == (int)y;
			}

			int IEqualityComparer<SomeEnum>.GetHashCode(SomeEnum obj)
			{
				return ((int)obj).GetHashCode();
			}
		}


		static void Dictionary_with_no_Garbage()
		{
			//dictionary 검색시 키에 접근할때 IEqualityComparer<T> 를 이용하여
			//Equals(), GetHashCode() 재정의 할 경우
			//struct & enum 연산시 boxing, unboxing 이 발생하지 않는다.

			//Usage.
			Dictionary<SomeStruct, int> dic_struct = new Dictionary<SomeStruct, int>(new StructComparer());

			//Usage.
			Dictionary<SomeEnum, int> dic_enum = new Dictionary<SomeEnum, int>(new EnumComparer());
		}

		class Entity
		{
			public Int32 ID { get; set; }
			public string Name { get; set; }
			public bool IsRemove { get; set; }

			public Entity(int id, string name)
            {
                ID = id;
                Name = name;
				IsRemove = false;
            }

			public string toString()
			{
				return Name + "_" + (IsRemove == true ? "Destroy" : "New");
			}
        }

		static void ConcurrentDictionary_use()
		{
			/*
                .NET 4.0 부터 멀티쓰레딩 환경에서 Dictionary를 보다 간편하게 사용할 수 있는 새로운 클래스인 ConcurrentDictionary<T> 가 제공되었다.
                ConcurrentDictionary 클래스에서는 기본적으로 데이타를 추가하기 위해 TryAdd() 메서드를 사용하고,
                키값을 읽기 위해서는 TryGetValue() 메서드를 사용한다.
                또한 기존 키값을 갱신하기 위해서 TryUpdate() 메서드를, 기존 키를 지우기 위해서는 TryRemove() 메서드를 사용한다.

                아래 예제는 하나의 쓰레드가 ConcurrentDictionary 에 Key 1부터 10까지 계속 집어 넣을 때,
                동시에 다른 쓰레드에서는 계속 그 해시테이블에서 Key 1부터 10까지의 데이타를 빼내 (순차적으로) 읽어 오는 작업을 하는 샘플 코드이다. 
            */
			{
				var dict = new ConcurrentDictionary<int, Entity>();

				Task t1 = Task.Factory.StartNew(() =>
				{
                    int key = 1;
                    while (key <= 10)
                    {
						var new_value = new Entity(key, "D" + key);

                        Console.WriteLine("Begin AddOrUpdate() - {0},{1}", key, new_value.toString());
                        dict.AddOrUpdate(key, new_value, (_key, _oldValue) => new_value);
                        Console.WriteLine("End AddOrUpdate() - {0},{1}", key, new_value.toString());

                        key++;
                    }
				});

				Task t2 = Task.Factory.StartNew(() =>
				{
					while (true)
					{
                        int key = 1;
                        Entity value;

                        while (key <= 10)
						{
							if (dict.TryGetValue(key, out value))
							{
								Console.WriteLine("Try GetValue() - {0},{1}", key, value.toString());
							}

							key++;
						}
					}
				});

                Task t3 = Task.Factory.StartNew(() =>
                {
                    Random rand = new Random();

                    while (true)
					{
                        var key = rand.Next() % 10 + 1;
                        Entity value;

                        if (dict.TryRemove(key, out value))
                        {
							value.IsRemove = true;
                            Console.WriteLine("Try Remove() - {0},{1}", key, value.toString());
                        }
                    }
                });

                Task t4 = Task.Factory.StartNew(() =>
                {
					while(true)
					{
                        var list = dict.GetEnumerator();
                        while (list.MoveNext())
                        {
                            Console.WriteLine("GetEnumerator() - {0},{1}", list.Current.Key, list.Current.Value.toString()); // Destory 상태인 경우도 있다 !!!
                        }

                        foreach (var each in dict)
                        {
                            Console.WriteLine("Foreach - {0},{1}", each.Key, each.Value.toString()); // Destory 상태인 경우도 있다 !!!
                        }
                    }
                });

                Task.WaitAll(t1, t2, t3, t4);

				Console.ReadLine();
			}
		}

		public static void Test()
        {
			//ConcurrentDictionary_use();

			//Dictionary_with_no_Garbage();

            //dictionary_custom_key();

            //dictionary_custom_compare();

            //dictionary_indexer();

            //dictionary_keys_value_check();

            //dictionary_keys_to_list();

            //Dictionary_use();

            //dictionary_what();
        }
    }
}
