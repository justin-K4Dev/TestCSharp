using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedStep
{
    public class Lambda
    {
        delegate int MyDelegate1(int a, int b);

        delegate void MyDelegate2();

        delegate void MyDelegate3(int a, int b);

        static void lambda_expression()
        {
            /*
                C# 3.0부터 지원하는 => 연산자는 C#에서 람다 식(Lambda Expression)을 표현할 때 사용한다.
                람다식은 무명 메서드와 비슷하게 무명 함수(anonymous function)를 표현하는데 사용된다.
                람다식은 아래와 같이 입력 파라미터(0개 ~ N개)를 => 연산자 왼쪽에, 실행 문장들을 오른쪽에 둔다.

                    람다 Synyax : (입력 파라미터) => { 문장블럭 };

                예를 들어 하나의 문자열을 받아 들여 메시지 박스를 띄운다면 다음과 같이 간단히 쓸 수 있다.

                    str => { MessageBox.Show(str); }

                입력 파라미터는 하나도 없는 경우부터 여러 개 있는 경우가 있다.
                다음 예는 파라미터가 없는 케이스 부터 두개 있는 케이스까지 보여준다.
                마지막 예는 입력 파라미터의 타입이 애매한 경우 이를 써줄 수 있음을 보여준다.
                일반적으로 입력타입은 컴파일러가 알아서 찾아낸다.

                    () => Write("No");
                    (p) => Write(p);
                    (s, e) => Write(e);
                    (string s, int i) => Write(s, i);

                Lambda Expression을 이용하면 이전 페이지에 소개한 delegate 와 무명 메서드를 더 간략히 표현할 수 있다.
                예를 들어 다음과 같은 Click 이벤트는 이벤트 핸들러 메서드인 button1_Click를 가리키고 있다.
                그래서 메서드 button1_Click은 해당 클래스내 어딘가에 정의되어 있어야 한다.

                    this.button1.Click += new System.EventHandler(button1_Click);

                    private void button1_Click(object sender, EventArgs e)
                    {
                        ((Button)sender).BackColor = Color.Red;
                    }

                위의 new System.EventHandler(button1_Click)은 간단히 button1_Click 메서드명만 사용하여 아래와 같이 줄일 수 있다.

                    this.button1.Click += button1_Click;

                이를 좀더 간단하려면 아래와 같이 무명 메서드(Anonymous Method)를 써서 표현할 수 있다.

                    this.button1.Click += delegate(object sender, EventArgs e)
                    {
                        ((Button)sender).BackColor = Color.Red;
                    };

                그리고 람다 식을 사용하면 이를 더 간단히 다음과 같이 바꿀 수 있다.
                람다 식의 오른쪽 실행 블럭이 한 문장일 때는 { } 괄호를 생략할 수 있다.
                아래 식은 무명 메서드를 쉽게 람다 식으로 변경한 간단한 예이다.

                    this.button1.Click += (sender, e) => ((Button)sender).BackColor = Color.Red;

                람다 식(Lambda Expression)은 .NET 여러 곳에서 사용되지만 특히 LINQ (Language Integrated Query) 에서 많이 사용된다.
                LINQ는 별도의 주제이지만 아래는 람다식이 LINQ의 Where 쿼리에서 사용된 간단한 예이다.

                    var proj = db.Projects.Where(p => p.Name == strName);
            */
            {
                MyDelegate1 add = (a, b) => a + b;
                MyDelegate2 lambda = () => Console.WriteLine("Lambda Expression");

                Console.WriteLine("sum: {0}", add(100, 1));

                lambda();

                Console.ReadLine();
            }
            {
                MyDelegate3 Compare = (a, b) =>
                {
                    if (a > b)
                    {
                        Console.WriteLine("{0} 보다 {1} 가 크다", b, a);
                    }
                    else if (a < b)
                    {
                        Console.WriteLine("{0} 보다 {1} 가 크다", a, b);
                    }
                    else
                    {
                        Console.WriteLine("{0}, {1} 는 같다", a, b);
                    }
                };

                Compare(100, 10);

                Console.ReadLine();
            }
        }


        class TreeNode
        {
            public string Value { get; set; }
            public List<TreeNode> Nodes { get; set; }

            public TreeNode()
            {
                Nodes = new List<TreeNode>();
            }
        }

        static void lambda_recursive()
        {
            Action<TreeNode> traverse = null;

            traverse = (n) => { Console.WriteLine(n.Value); n.Nodes.ForEach(traverse); };

            var root = new TreeNode { Value = "Root" };
            root.Nodes.Add(new TreeNode { Value = "ChildA" });
            root.Nodes[0].Nodes.Add(new TreeNode { Value = "ChildA1" });
            root.Nodes[0].Nodes.Add(new TreeNode { Value = "ChildA2" });
            root.Nodes.Add(new TreeNode { Value = "ChildB" });
            root.Nodes[1].Nodes.Add(new TreeNode { Value = "ChildB1" });
            root.Nodes[1].Nodes.Add(new TreeNode { Value = "ChildB2" });

            traverse(root);

            Console.ReadLine();
        }


        public static void Test()
        {
            //lambda_recursive();

            //lambda_expression();
        }
    }
}
