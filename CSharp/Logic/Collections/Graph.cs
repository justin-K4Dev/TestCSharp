using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public class Graph
    {
        static void graph_what()
        {
            /*
                자료구조 : 그래프 (Graph) 

                그래프(Graph)는 노드(꼭지점, Vertex)과 변(Edge)로 구성되어 있는 자료 구조로서 트리와 다르게 사이클(Cycle)을 허용한다.
                Edge 에 방향을 허용하느냐 마느냐에 따라서 방향이 있는 그래프(Directed Graph),
                혹은 방향이 없는 그래프(Undirected Graph)로 나눌 수 있다.
                또한 각 변에 가중치(Weight)를 주어 노드와 노드 사이에 숫자로 거리, 비용등의 관계를 표현할 수 있다.
                일반적으로 Graph 자료구조는 인접리스트(Adjacency List)나 인접 행렬 (Adjacency Matrix) 등의 방법으로 표현한다.
                그래프는 도시간 최단 행로를 구하거나 웹 링크 연결도를 표현하는 등 여러 종류의 자료를 표현하는데 사용될 수 있다. 
            */
            {
                Console.ReadLine();
            }
        }


        // GraphNode class
        class GraphNode<T>
        {
            private List<GraphNode<T>> _neighbors;
            private List<int> _weights;

            public T Data { get; set; }

            public GraphNode()
            {
            }

            public GraphNode(T value)
            {
                this.Data = value;
            }

            public List<GraphNode<T>> Neighbors
            {
                get
                {
                    _neighbors = _neighbors ?? new List<GraphNode<T>>();
                    return _neighbors;
                }
            }

            public List<int> Weights
            {
                get
                {
                    _weights = _weights ?? new List<int>();
                    return _weights;
                }
            }
        }

        // Graph Template class
        class TGraph<T>
        {
            private List<GraphNode<T>> _nodeList;

            public TGraph()
            {
                _nodeList = new List<GraphNode<T>>();
            }

            public GraphNode<T> AddNode(T data)
            {
                GraphNode<T> n = new GraphNode<T>(data);
                _nodeList.Add(n);
                return n;
            }

            public GraphNode<T> AddNode(GraphNode<T> node)
            {
                _nodeList.Add(node);
                return node;
            }

            public void AddEdge(GraphNode<T> from, GraphNode<T> to, bool oneway = true, int weight = 0)
            {
                from.Neighbors.Add(to);
                from.Weights.Add(weight);

                if (!oneway)
                {
                    to.Neighbors.Add(from);
                    to.Weights.Add(weight);
                }
            }

            internal void DebugPrintLinks()
            {
                foreach (GraphNode<T> graphNode in _nodeList)
                {
                    foreach (var n in graphNode.Neighbors)
                    {
                        string s = graphNode.Data + " - " + n.Data;
                        Console.WriteLine(s);
                    }
                }
            }
        }


        static void graph_use()
        {
            /*
                .NET Framework은 그래프와 관련된 클래스를 제공하지 않는다.
                그래프를 구현하는 한 방식으로 아래 코드는 각 노드마다 인접한 노드들의 리스트를 가지고 있는 인접리스트(Adjacency List)를 사용하고 있다.
                Graph클래스는 GraphNode들을 갖는 리스트를 기본 필드로 갖고 있으며,
                각 GraphNode는 데이타 (혹은 키+데이타) 필드와 인접 노드 리스트를 기본적인 필드로 가지고 있다.
                GraphNode는 필요에 따라 Weight 배열을 가질 수 있는데,
                이는 각 변(Edge)의 가중치를 저장할 필요가 있을 때 사용된다.
            */
            {
                TGraph<int> g = new TGraph<int>();
                var n1 = g.AddNode(10);
                var n2 = g.AddNode(20);
                var n3 = g.AddNode(30);
                var n4 = g.AddNode(40);
                var n5 = g.AddNode(50);

                g.AddEdge(n1, n3);
                g.AddEdge(n2, n4);
                g.AddEdge(n3, n4);
                g.AddEdge(n3, n5);

                g.DebugPrintLinks();

                Console.ReadLine();
            }
        }


        public static void Test()
        {
            //graph_use();

            //graph_what();
        }
    }
}
