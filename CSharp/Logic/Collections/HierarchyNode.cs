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

    public class HierarchyNode<T> : ITree<T>
    {
        public T m_value;
        private HashSet<ITree<T>> m_children;

        public HierarchyNode(T value)
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
            if (m_children == null || m_children.Count <= i)
                throw new IndexOutOfRangeException("No such child node");
            return m_children.ToList()[i];
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
            // 트리 생성
            var root = new HierarchyNode<string>("root");
            var child1 = new HierarchyNode<string>("child1");
            var child2 = new HierarchyNode<string>("child2");
            var grandChild = new HierarchyNode<string>("grandChild");

            // 트리 구조:
            // root
            // ├── child1
            // │    └── grandChild
            // └── child2

            root.addChild(child1);
            root.addChild(child2);
            child1.addChild(grandChild);

            // 테스트: 루트 노드 값
            Console.WriteLine($"Root: {root.getValue()}"); // Root: root

            // 자식 노드 값 출력
            for (int i = 0; i < 2; ++i)
            {
                Console.WriteLine($"Root child[{i}]: {root.getChild(i).getValue()}");
                // Root child[0]: child1
                // Root child[1]: child2
            }

            // 자식의 자식
            var gc = root.getChild(0).getChild(0);
            Console.WriteLine($"Child1's child: {gc.getValue()}"); // Child1's child: grandChild

            // isEqual 테스트
            Console.WriteLine($"root.isEqual(\"root\"): {root.isEqual("root")}"); // True
            Console.WriteLine($"root.isEqual(\"child1\"): {root.isEqual("child1")}"); // False

            // isEmpty 테스트
            Console.WriteLine($"root.isEmpty(): {root.isEmpty()}"); // False

            // 예외 테스트 (잘못된 인덱스 접근)
            try
            {
                root.getChild(10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}"); // No such child node
            }
        }
    }
}