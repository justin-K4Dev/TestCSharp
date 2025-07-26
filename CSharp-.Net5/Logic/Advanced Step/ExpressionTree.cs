using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;



namespace AdvancedStep;

public class ExpressionTree
{

    static void ExpressionTree_what()
    {
        /*
            식 트리(Expression Tree)는 C# 코드(예: 람다식)를 데이터 구조로 표현한 것이다.
            각 노드는 메서드 호출, 연산자, 상수, 매개변수 등 표현식(Expression)을 나타낸다.


            ✅ 언제 사용하나?
            사용                                  사례	설명
            LINQ to SQL / Entity Framework	      쿼리 분석 및 SQL로 변환
            DSL (도메인 특화 언어)	              식을 조작하거나 자동 생성
            동적 코드 생성	                      런타임에 식을 구성하고 실행 (Expression.Compile())
            ORM 최적화	                          조건문, 필터, 정렬 등을 식으로 받아 분석

       */
        {
            Expression<Func<int, bool>> expr = x => x > 5;
            Console.WriteLine(expr); // 출력: x => (x > 5)

            /*
                ✅ 내부 구조 보기
                Lambda
                 └── BinaryExpression (>)
                     ├── ParameterExpression (x)
                     └── ConstantExpression (5) 
            */
        }
    }

    static void exec_ExpressionTree()
    {
        // 식 트리를 실제 델리게이트 함수로 실행하려면 Compile()을 사용합니다
        Expression<Func<int, bool>> expr = x => x > 5;

        var func = expr.Compile();
        Console.WriteLine(func(10)); // true
        Console.WriteLine(func(3));  // false
    }


    public static void Test()
    {
        //exec_ExpressionTree();

        //ExpressionTree_what();
    }
}
