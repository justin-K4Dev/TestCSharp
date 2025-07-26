using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
	public interface ITree<T>
	{
		void addChild(ITree<T> child);
		ITree<T> getChild(int i);
		T getValue();
		bool isEmpty();
	}

	public class HeirerchyNode<T> : ITree<T>
	{
		public T m_value;
		private HashSet<ITree<T>> m_children;

		public HeirerchyNode(T value)
		{
			this.m_value = value;
		}

		public void addChild(ITree<T> child)
		{
			if (m_children == null)
			{
				m_children = new HashSet<ITree<T>>();
			}

			m_children.Add(child);
		}

		public ITree<T> getChild(int i)
		{
			return m_children.ToList()?[i];
		}

		public bool isEmpty()
		{
			return false;
		}

		public T getValue()
		{
			return m_value;
		}

		public bool isEqual(T value)
		{
			return m_value.Equals(value);
		}


		public static void Test()
		{

		}
	}	
}
