using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public class BinarySearchTree
    {
        static void binary_search_tree_what()
        {
            /*
                자료구조 : 이진검색트리 (Binary Search Tree) 

                이진트리(Tree)의 특수한 형태로 자주 사용되는 트리로서 이진검색트리 (Binary Search Tree)가 있다.
                이진검색트리는 이진트리의 모든 속성을 가짐과 동시에 중요한 또 하나의 속성을 가지고 있는데,
                그것은 특정 노드에서 자신의 노드보다 작은 값들은 모두 왼쪽에 있고,
                큰 값들은 모두 오른쪽에 위치한다는 점이다. 또한 중복된 값을 허락하지 않는다.
                따라서 전체 트리가 소트되어 있는 것과 같은 효과를 같게 되어 검색에 있어
                배열이나 이진트리처럼 순차적으로 모든 노드를 검색하는 것(O(n))이 아니라,
                매 검색마다 검색영역을 절반으로 줄여 O(log n)으로 검색할 수 있게 된다.
                하지만 노드들이 한쪽으로 일렬로 기울어진 Skewed Tree인 경우,
                검색영역을 n-1로만 줄이기 때문에 O(n)만큼의 시간이 소요된다.
                즉, 예를 들어 소트된 데이타를 이진검색트리에 추가하게 되면,
                이렇게 한쪽으로 치우쳐 진 트리가 생겨 검색시간이 O(n)으로 떨어지게 되는데,
                이러한 현상을 막기 위하여 노드 추가/갱신시 트리 스스로 다시 밸런싱(Self balancing)하여
                검색 최적화를 유지할 수 있다.
                이러한 트리를 Self-Balancing Binary Search Tree 또는 Balanced Search Tree라 하는데,
                가장 보편적인 방식으로 AVL Tree, Red-Black Tree 등을 들 수 있다.
                
                NOTE: 참고로 Search Tree에는 최대 2개의 자식노드를 갖는 Binary Search Tree 이외에,
                여러 개의 자식노드들을 갖는 N-Way 검색 트리 (n-way Search Tree)가 있는데,
                대표적으로 B-Tree (혹은 이의 변형인 B* Tree, B+ Tree)가 있으며
                흔히 SQL Server와 같은 관계형 DB 인덱스로 주로 사용된다. 
            */
            {
                Console.ReadLine();
            }
        }


        // 이진 트리 노드 클래스
        class BinaryTreeNode<T>
        {
            public T Data { get; set; }
            public BinaryTreeNode<T> Left { get; set; }
            public BinaryTreeNode<T> Right { get; set; }

            public BinaryTreeNode(T data)
            {
                this.Data = data;
            }
        }

        class BST<T>
        {
            private BinaryTreeNode<T> root = null;
            private Comparer<T> comparer = Comparer<T>.Default;

            public void Insert(T val)
            {
                BinaryTreeNode<T> node = root;
                if (node == null)
                {
                    root = new BinaryTreeNode<T>(val);
                    return;
                }

                while (node != null)
                {
                    int result = comparer.Compare(node.Data, val);
                    if (result == 0)
                    {
                        //throw new InvalidDataException("Duplicate value");
                        return;
                    }
                    else if (result > 0)
                    {
                        if (node.Left == null)
                        {
                            node.Left = new BinaryTreeNode<T>(val);
                            return;
                        }
                        node = node.Left;
                    }
                    else
                    {
                        if (node.Right == null)
                        {
                            node.Right = new BinaryTreeNode<T>(val);
                            return;
                        }
                        node = node.Right;
                    }
                }
            }

            public void PreOrderTraversal()
            {
                PreOrderRecursive(root);
            }

            private void PreOrderRecursive(BinaryTreeNode<T> node)
            {
                if (node == null) return;
                Console.WriteLine(node.Data);
                PreOrderRecursive(node.Left);
                PreOrderRecursive(node.Right);
            }
        }

        static void binary_search_tree_use()
        {
            /*
                .NET Framework은 이진검색트리와 관련된 클래스를 제공하지 않는다.
                아래 소스는 간단한 이진검색트리를 C#으로 구현해 본 것으로서,
                이진검색트리의 기본적인 개념을 예시하기 위한 코드이다.
            */
            {
                BST<int> bst = new BST<int>();
                bst.Insert(4);
                bst.Insert(2);
                bst.Insert(6);
                bst.Insert(1);
                bst.Insert(7);

                bst.PreOrderTraversal();

                Console.ReadLine();
            }
        }


        static void SortedDictionary_use()
        {
            /*
                .NET의 SortedDictionary클래스는 내부적으로 이진검색트리(BST, Binary Search Tree)를 사용하여
                Key를 갖고 Value를 찾는 Map ADT 타입 (ADT: Abstract Data Type)을 구현한 클래스이다.
                (.NET에서 MAP ADT를 구현한 클래스로는
                해시테이블을 이용한 Hashtable/Dictionary클래스, 이진검색트리를 이용한 SortedDictionary,
                그리고 배열을 이용한 SortedList 등이 있다)
                비록 .NET이 이진검색트리를 클래스를 public으로 제공하지 않지만,
                내부적으로 BST를 사용해 SortedDictionary를 구현하고 있다.
                SortedDictionary는 기본적으로 BST 자료구조이므로 중복된 키를 허용하지 않으며,
                추가, 삭제, 검색에서 O(log n)의 시간이 소요된다.
            */
            {
                SortedDictionary<int, string> tmap = new SortedDictionary<int, string>();
                tmap.Add(1001, "Tom");
                tmap.Add(1003, "John");
                tmap.Add(1010, "Irina");

                // Key 1010의 데이타 읽기
                string name1010 = tmap[1010];

                // Iterator 사용
                foreach (KeyValuePair<int, string> kv in tmap)
                {
                    Console.WriteLine("{0}:{1}", kv.Key, kv.Value);
                }

                Console.ReadLine();
            }
        }

        public static void Test()
        {
            //SortedDictionary_use();

            //binary_search_tree_use();

            //binary_search_tree_what();
        }
    }
}
