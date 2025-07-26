using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
	public class LinkedHashSet<T> : ISet<T>
	{
		private readonly Dictionary<T, LinkedListNode<T>> table;
		private readonly LinkedList<T> list;

		public LinkedHashSet()
		{
			table = new Dictionary<T, LinkedListNode<T>>();
			list = new LinkedList<T>();
		}

		public LinkedHashSet(IEnumerable<T> collection)
		{
			var countable = collection as ICollection<T>;
			table = (countable != null)
				? new Dictionary<T, LinkedListNode<T>>(countable.Count)
				: new Dictionary<T, LinkedListNode<T>>();
			list = new LinkedList<T>();
			UnionWith(collection);
		}

		public LinkedHashSet(IEqualityComparer<T> comparer)
		{
			table = new Dictionary<T, LinkedListNode<T>>(comparer);
			list = new LinkedList<T>();
		}

		public LinkedHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			var countable = collection as ICollection<T>;
			table = (countable != null)
				? new Dictionary<T, LinkedListNode<T>>(countable.Count, comparer)
				: new Dictionary<T, LinkedListNode<T>>(comparer);
			list = new LinkedList<T>();
			UnionWith(collection);
		}

		public LinkedHashSet(int capacity, IEqualityComparer<T> comparer)
		{
			table = new Dictionary<T, LinkedListNode<T>>(capacity, comparer);
			list = new LinkedList<T>();
		}

		public IEqualityComparer<T> Comparer
		{
			get { return table.Comparer; }
		}

		public bool Add(T item)
		{
			if (table.ContainsKey(item))
			{
				return false;
			}
			table.Add(item, list.AddLast(item));
			return true;
		}

		private void EnsureNotNull(IEnumerable<T> other)
		{
			if (other == null) throw new ArgumentNullException("other");
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			foreach (var item in other) Remove(item);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			var intersect = Intersect(other);
			for (var n = list.First; n != null;)
			{
				if (intersect.Contains(n.Value))
				{
					n = n.Next;
				}
				else
				{
					n = RemoveAndNext(n);
				}
			}
		}
		private HashSet<T> Intersect(IEnumerable<T> other)
		{
			var intersect = new HashSet<T>();
			foreach (var item in other)
				if (table.ContainsKey(item))
					intersect.Add(item);
			return intersect;
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			return new HashSet<T>(list).IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			return new HashSet<T>(list).IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			return new HashSet<T>(list).IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			return new HashSet<T>(list).IsSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			return new HashSet<T>(list).Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			return new HashSet<T>(list).SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			var intersect = AddAndIntersect(other);
			for (var n = list.First; n != null;)
			{
				if (intersect.Contains(n.Value))
				{
					n = RemoveAndNext(n);
				}
				else
				{
					n = n.Next;
				}
			}
		}

		private HashSet<T> AddAndIntersect(IEnumerable<T> other)
		{
			var intersect = new HashSet<T>();
			foreach (var item in other)
				if (!Add(item))
					intersect.Add(item);
			return intersect;
		}

		private LinkedListNode<T> RemoveAndNext(LinkedListNode<T> node)
		{
			var n = node.Next;
			table.Remove(node.Value);
			list.Remove(node);
			return n;
		}

		public void UnionWith(IEnumerable<T> other)
		{
			EnsureNotNull(other);
			foreach (var item in other) Add(item);
		}

		void ICollection<T>.Add(T item)
		{
			this.Add(item);
		}

		public void Clear()
		{
			table.Clear();
			list.Clear();
		}

		public bool Contains(T item)
		{
			return table.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return table.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			LinkedListNode<T> node = null;
			if (table.TryGetValue(item, out node))
			{
				table.Remove(item);
				list.Remove(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}


		public static void Test()
		{
			//LinkedList_use();

			//linked_list_what();
		}
	}

}
