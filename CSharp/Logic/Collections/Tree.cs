using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public class Tree
    {
        static void tree_what()
        {
            /*
                자료구조 : 트리 (Tree) 

                트리(Tree)는 계층적인 자료를 나타내는데 자주 사용되는 자료 구조로서 하나이하의 부모노드와 복수 개의 자식노드들을 가질 수 있다.
                트리는 하나의 루트(Root) 노드에서 출발하여 자식노드들을 갖게 되며, 각각의 자식 노드는 또한 자신의 자식노드들을 가질 수 있다.
                트리 구조는 한 노드에서 출발하여 다시 자기 자신의 노드로 돌아오는 순환(Cycle)구조를 가질 수 없다.
                트리구조는 계층적인 정부 혹은 기업 조직도, 대중소 지역 구조, 데이타 인덱스 파일 등에 적합한 자료구조이다.
            */
            {
                Console.ReadLine();
            }
        }


        static void binary_tree_what()
        {
            /*
                자료구조 : 이진 트리 (Binary Tree)

                트리(Tree)에서 많이 사용되는 특별한 트리로서 이진트리를 들 수 있는데, 이진 트리는 자식노드가 0개 ~ 2개인 트리를 말한다.
                따라서 이진트리 노드는 데이타필드와 왼쪽노드 및 오른쪽노드를 갖는 자료 구조로 되어 있다.
                이진 트리는 루트 노드로부터 출발하여 원하는 특정 노드에 도달할 수 있는데,
                이때의 검색 시간(Search Time)은 O(n)이 소요된다.
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

        // 이진 트리 클래스
        class BinaryTree<T>
        {
            public BinaryTreeNode<T> Root { get; set; }

            // 트리 데이타 출력 예
            public void PreOrderTraversal(BinaryTreeNode<T> node)
            {
                if (node == null) return;

                Console.WriteLine(node.Data);
                PreOrderTraversal(node.Left);
                PreOrderTraversal(node.Right);
            }
        }

        static void binary_tree_use()
        {
            /*
                .NET Framework은 트리 혹은 이진 트리와 관련된 클래스를 제공하지 않는다.
                이진트리를 구현하는 방식은 일반적으로 배열을 이용하는 방법과 연결리스트(Linked List)를 이용하는 방법이 있다.
                아래 소스는 연결리스트를 사용하여 간단한 이진 트리를 C#으로 구현해 본 것으로서,
                이진 트리의 기본적인 개념을 예시하기 위한 코드이다. 
            */
            {
                BinaryTree<int> btree = new BinaryTree<int>();
                btree.Root = new BinaryTreeNode<int>(1);
                btree.Root.Left = new BinaryTreeNode<int>(2);
                btree.Root.Right = new BinaryTreeNode<int>(3);
                btree.Root.Left.Left = new BinaryTreeNode<int>(4);

                btree.PreOrderTraversal(btree.Root);

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //binary_tree_use();

            //binary_tree_what();

            //tree_what();
        }
    }
}
