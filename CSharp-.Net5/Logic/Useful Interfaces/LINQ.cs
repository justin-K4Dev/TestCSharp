using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace UsefulInterfaces;

public class LINQ
{
    static void override_IQueryable()
    {
        /*
            IQueryable<T>

            ✅ 목적
              - LINQ 쿼리의 "지연 평가"와 "쿼리 구문 트리 변환"을 지원하는 컬렉션 인터페이스
              - LINQ-to-SQL, Entity Framework, MongoDB, ElasticSearch 등
                LINQ 쿼리를 실제 DB 질의문, REST API 호출 등으로 변환해서 실행할 수 있게 해줍
              - LINQ 쿼리(Where, Select, GroupBy 등)를 "Expression Tree"로 받아서, 나중에 IQueryProvider가 해석/실행하는 방식

            IQueryProvider

            ✅ 목적
              - IQueryable의 쿼리 실행을 담당하는 프로바이더 인터페이스
              - 쿼리(Expression) 트리를 받아 실제 실행(데이터 조회, 쿼리 변환 등)을 담당
        */

        // List<T>에서 AsQueryable()로 IQueryable<T>를 얻는다
        IQueryable<int> queryable = new List<int> { 10, 15, 20, 25, 30 }.AsQueryable();

        // LINQ 쿼리 구문 그대로 사용
        var q = queryable
            .Where(x => x % 10 == 0)
            .Select(x => x * 2);

        // IQueryProvider 타입 확인
        Console.WriteLine("IQueryProvider 타입: " + queryable.Provider.GetType().FullName);

        // 실제 쿼리 실행 (이 때 연산이 일어남)
        foreach (var item in q)
        {
            Console.WriteLine(item);
        }
        /*
            IQueryProvider 타입: System.Linq.EnumerableQuery`1
            20
            40
            60
        */
    }

    public static void Test()
    {
        override_IQueryable();
    }
}
