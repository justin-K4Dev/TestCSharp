using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace UsefulInterfaces;


public class BindingValid
{
    public class UserInput : System.ComponentModel.DataAnnotations.IValidatableObject
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Email))
                yield return new ValidationResult("Email required", new[] { nameof(Email) });
            if (Password.Length < 6)
                yield return new ValidationResult("Password too short", new[] { nameof(Password) });
        }
    }

    static void override_IValidatableObject()
    {
        /*
            IDataErrorInfo

            ✅ 목적
              - 모델 단위 커스텀 유효성 검사
              - Attribute 기반보다 복잡한 조건 검사 가능
        */


        var user = new UserInput { Email = "", Password = "abc" };

        var results = new List<ValidationResult>();
        var context = new ValidationContext(user);
        bool isValid = Validator.TryValidateObject(user, context, results, true);

        Console.WriteLine($"Is Valid: {isValid}");
        foreach (var result in results)
            Console.WriteLine(result.ErrorMessage);
    }

    static void override_ICollectionView()
    {
        /*
            ICollectionView

            ✅ 목적
              - WPF/MVVM의 컬렉션 바인딩에 특화된 "뷰(view) 컬렉션" 인터페이스
              - 필터(Filter), 정렬(Sort), 그룹(Group), 커서(Cursor) 등을 컬렉션에 동적으로 적용할 수 있음
              - 원본 데이터 컬렉션(예: ObservableCollection<T>)은 그대로 두고,
                뷰(View) 컬렉션만 사용자 UI/상황에 맞게 가공해서 보여줌
              - UI에서 DataGrid/ListView/ListBox 등과 바인딩할 때
                데이터의 동적 필터링/정렬/그룹핑 등을 아주 쉽게 구현할 수 있음
              - ICollectionView는 WPF에서 컬렉션을 UI에 보여줄 때,
                동적으로 필터·정렬·그룹핑·커서 등 다양한 뷰를 제공하는 인터페이스
        */

        {
            /*
                //=================================================================================
                // 1. ViewModel & XAML
                //=================================================================================

                public class PeopleViewModel
                {
                    public ObservableCollection<Person> People { get; } = new ObservableCollection<Person>
                    {
                        new Person { Name = "Alice", Age = 25 },
                        new Person { Name = "Bob", Age = 18 },
                        new Person { Name = "Carol", Age = 30 }
                    };

                    public ICollectionView PeopleView { get; }

                    public PeopleViewModel()
                    {
                        // 컬렉션 뷰 생성: 필터 및 정렬 지정
                        PeopleView = CollectionViewSource.GetDefaultView(People);
                        PeopleView.Filter = o => ((Person)o).Age >= 20; // 20세 이상만 표시
                        PeopleView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    }
                }

                //=================================================================================
                // 2. XAML 바인딩 예시
                //=================================================================================

                <!-- MainWindow.xaml -->
                <Window ...>
                    <Window.DataContext>
                        <local:PeopleViewModel />
                    </Window.DataContext>
                    <Grid>
                        <ListView ItemsSource="{Binding PeopleView}">
                            <ListView.View>
                                <GridView>
                                    <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}" />
                                    <GridViewColumn Header="Age" DisplayMemberBinding="{Binding Age}" />
                                </GridView>
                            </ListView.View>
                        </ListView>
                    </Grid>
                </Window>
            */
        }

        {
            /*
                //=================================================================================
                //  3. 콘솔 단독 테스트(비-WPF, 동작 원리 확인)
                //=================================================================================

                var people = new ObservableCollection<Person>
                {
                    new Person { Name = "Alice", Age = 25 },
                    new Person { Name = "Bob", Age = 18 },
                    new Person { Name = "Carol", Age = 30 }
                };

                var view = CollectionViewSource.GetDefaultView(people);

                // 20세 이상만 필터링
                view.Filter = o => ((Person)o).Age >= 20;

                // 이름 오름차순 정렬
                view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

                foreach (Person p in view)
                    Console.WriteLine($"{p.Name} ({p.Age})");

                // 출력:
                // Alice (25)
                // Carol (30)             
            */
        }
    }


    public static void Test()
    {
        override_ICollectionView();

        override_IValidatableObject();
    }
}