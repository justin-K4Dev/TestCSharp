using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace UsefulInterfaces;

public class Database
{
    static void override_IDbConnection()
    {
        /*
            IDbConnection

            ✅ 목적
              - 데이터베이스와의 연결(커넥션)을 나타내는 인터페이스
                (Open, Close, ConnectionString 등 지원)

            IDbCommand

            ✅ 목적
              - SQL 쿼리(명령)를 실행하는 객체 (ExecuteReader, ExecuteNonQuery 등)

            IDataReader

            ✅ 목적
              - 쿼리 결과(행 집합)를 "앞으로만 읽을 수 있는 스트림" 형태로 반환하는 인터페이스
              - (Read, GetInt32, GetString 등)


            ✅ 공통
              - IDbConnection/IDbCommand/IDataReader는
                .NET에서 모든 관계형 데이터베이스를 동일 코드로 제어하는 ADO.NET 표준 인터페이스
        */

        // 1. DB 연결 객체 생성 (In-Memory SQLite)
        using (IDbConnection conn = new System.Data.SQLite.SQLiteConnection("Data Source=:memory:"))
        {
            conn.Open();

            // 2. 테이블 생성
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE People (Id INTEGER PRIMARY KEY, Name TEXT)";
                cmd.ExecuteNonQuery();
            }

            // 3. 데이터 삽입
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO People (Name) VALUES ('Alice'), ('Bob'), ('Charlie')";
                cmd.ExecuteNonQuery();
            }

            // 4. SELECT 쿼리 및 데이터 읽기
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Name FROM People";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        Console.WriteLine($"Id: {id}, Name: {name}");
                    }
                }
            }
        }
        /*
        출력:
            Id: 1, Name: Alice
            Id: 2, Name: Bob
            Id: 3, Name: Charlie
        */
    }

    public static void Test()
    {
        override_IDbConnection();
    }
}

