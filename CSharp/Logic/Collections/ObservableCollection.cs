using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;



namespace Collections
{
    public class ObservableCollection
    {
        static void ObservableCollection_what()
        {
            /*
                ObservableCollection

                ✅ 개요
                  - .NET에서 제공하는, 변경 알림이 가능한 컬렉션(List) 클래스
                  - Add, Remove, Clear, 인덱서 등 일반 리스트처럼 사용하면서
                    요소의 추가/삭제/수정/초기화 시점에 이벤트(CollectionChanged)를 발생
                  - WPF, MAUI, WinUI 등 데이터 바인딩 UI 프레임워크에서
                    UI와 컬렉션을 자동으로 동기화하는 데 표준적으로 사용
            */

            // ObservableCollection 생성
            var fruits = new ObservableCollection<string>();

            // 컬렉션 변경 이벤트 핸들러 등록
            fruits.CollectionChanged += (sender, e) =>
            {
                Console.WriteLine($"Action: {e.Action}");
                if (e.NewItems != null)
                    foreach (var item in e.NewItems)
                        Console.WriteLine($"+ {item}");
                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                        Console.WriteLine($"- {item}");
            };

            // 값 추가
            fruits.Add("Apple");
            fruits.Add("Banana");

            // 값 삭제
            fruits.Remove("Apple");

            // 값 교체
            fruits[0] = "Cherry";

            // 전체 삭제
            fruits.Clear();

            /*
                출력:
                Action: Add
                + Apple
                Action: Add
                + Banana
                Action: Remove
                - Apple
                Action: Replace
                + Cherry
                - Banana
                Action: Reset            
            */

            Console.ReadLine();
        }


        public static void Test()
        {
            ObservableCollection_what();
        }
    }
}
