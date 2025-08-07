using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;




namespace UsefulInterfaces
{
    public class BindingValid
    {
        public class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;
            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute; _canExecute = canExecute;
            }
            public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
            public void Execute(object parameter) => _execute();
            public event EventHandler CanExecuteChanged;
            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        static void override_ICommand()
        {
            /*
                ICommand (WPF/MAUI 등)

                ✅ 목적
                  - MVVM 패턴에서 UI의 "명령" 구현(버튼 클릭 등)
                  - RelayCommand, DelegateCommand 등으로 자주 활용
            */

            bool canRun = true;
            var command = new RelayCommand(
                execute: () => Console.WriteLine("Command executed!"),
                canExecute: () => canRun
            );

            Console.WriteLine($"CanExecute: {command.CanExecute(null)}"); // true
            if (command.CanExecute(null))
                command.Execute(null); // 실행됨

            canRun = false;
            Console.WriteLine($"CanExecute: {command.CanExecute(null)}"); // false
            if (command.CanExecute(null))
                command.Execute(null); // 실행 안됨

            // CanExecuteChanged 이벤트 테스트 (WPF에서는 UI 자동 갱신)
            command.CanExecuteChanged += (s, e) => Console.WriteLine("CanExecuteChanged raised!");
            command.RaiseCanExecuteChanged(); // 수동으로 이벤트 발생
        }


        public class UserViewModel : System.ComponentModel.INotifyPropertyChanged
        {
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            private string _name;
            public string Name
            {
                get { return _name; }
                set { _name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name")); }
            }
        }

        static void override_INotifyPropertyChanged()
        {
            /*
                INotifyPropertyChanged

                ✅ 목적
                  - WPF, MAUI, MVVM 등 데이터 바인딩 환경에서 속성값 변경 감지
            */

            var user = new UserViewModel();

            // PropertyChanged 이벤트 구독
            user.PropertyChanged += (s, e) =>
            {
                var vm = s as UserViewModel;
                if (null != vm)
                    Console.WriteLine($"Property changed: {e.PropertyName}, New Value: {vm.Name}");
            };

            // Name 속성 변경
            user.Name = "Alice";
            user.Name = "Bob";
            user.Name = "Bob"; // 값이 같으므로 이벤트 발생 안 함

            // 출력:
            // Property changed: Name, New Value: Alice
            // Property changed: Name, New Value: Bob
        }


        //=========================================================================================


        static void override_INotifyCollectionChanged()
        {
            /*
                INotifyCollectionChanged

                ✅ 목적
                  - 컬렉션(리스트 등)의 변경(추가, 삭제, 수정, 리셋 등)을 알리는 이벤트 인터페이스
            */

            var collection = new ObservableCollection<string>();

            // 컬렉션 변경 이벤트 구독
            collection.CollectionChanged += (sender, e) =>
            {
                Console.WriteLine($"Action: {e.Action}");
                if (e.NewItems != null)
                    foreach (var item in e.NewItems)
                        Console.WriteLine($"+ {item}");
                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                        Console.WriteLine($"- {item}");
            };

            collection.Add("Apple");     // Add
            collection.Add("Banana");    // Add
            collection.RemoveAt(0);      // Remove
            collection[0] = "Cherry";    // Replace
            collection.Clear();          // Reset

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
        }


        //=========================================================================================

        public class PersonViewModel : INotifyDataErrorInfo
        {
            private string _name;
            private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    ValidateName();
                }
            }

            private void ValidateName()
            {
                ClearErrors(nameof(Name));
                if (string.IsNullOrWhiteSpace(Name))
                    AddError(nameof(Name), "Name is required.");
                else if (Name.Length < 2)
                    AddError(nameof(Name), "Name is too short.");
            }

            public bool HasErrors => _errors.Count > 0;

            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

            public IEnumerable GetErrors(string propertyName)
                => propertyName != null && _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;

            // 에러 추가/제거 유틸
            private void AddError(string propertyName, string error)
            {
                if (!_errors.ContainsKey(propertyName))
                    _errors[propertyName] = new List<string>();
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            private void ClearErrors(string propertyName)
            {
                if (_errors.Remove(propertyName))
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        static void override_INotifyDataErrorInfo()
        {
            /*
                INotifyDataErrorInfo

                ✅ 목적
                  - WPF, UWP, WinUI 등 .NET 데이터 바인딩 환경에서 사용되는 유효성 검사
                  - 주로 DataGrid, 폼 편집, 바인딩 객체의 Undo/Redo, 트랜잭션성 편집 등에 활용
                  - 속성 값이 바뀔 때마다(비동기 포함) 동적으로 검증/알림이 필요할 때 필수
            */

            var vm = new PersonViewModel();
            vm.ErrorsChanged += (s, e) =>
            {
                Console.WriteLine($"ErrorsChanged: {e.PropertyName}");
                var errs = vm.GetErrors(e.PropertyName) as IEnumerable<string>;
                if (errs != null)
                    foreach (var err in errs)
                        Console.WriteLine($"  Error: {err}");
            };

            Console.WriteLine("=== Name = '' (empty) ===");
            vm.Name = "";
            Console.WriteLine($"HasErrors: {vm.HasErrors}");

            Console.WriteLine("\n=== Name = 'A' (too short) ===");
            vm.Name = "A";
            Console.WriteLine($"HasErrors: {vm.HasErrors}");

            Console.WriteLine("\n=== Name = 'Alice' (valid) ===");
            vm.Name = "Alice";
            Console.WriteLine($"HasErrors: {vm.HasErrors}");

            /*
                === Name = '' (empty) ===
                ErrorsChanged: Name
                  Error: Name is required.
                HasErrors: True

                === Name = 'A' (too short) ===
                ErrorsChanged: Name
                  Error: Name is too short.
                HasErrors: True

                === Name = 'Alice' (valid) ===
                ErrorsChanged: Name
                HasErrors: False
            */
        }

        //=========================================================================================

        public class UserInput : System.ComponentModel.IDataErrorInfo
        {
            public string this[string columnName] => columnName == "Name" && string.IsNullOrEmpty(Name) ? "Name is required" : "";
            public string Error => "";
            public string Name { get; set; }
        }

        static void override_IDataErrorInfo()
        {
            /*
                IDataErrorInfo

                ✅ 목적
                  - WPF/MAUI 등에서 "입력 값의 에러" UI 자동 처리
            */

            var user = new UserInput();

            // Name이 비어 있을 때 오류 메시지 출력
            Console.WriteLine("Error for Name(empty): " + user["Name"]); // "Name is required"

            // Name에 값을 할당
            user.Name = "Alice";
            Console.WriteLine("Error for Name('Alice'): " + user["Name"]); // ""

            // 다른 컬럼명을 넣으면 항상 ""
            Console.WriteLine("Error for Age: " + user["Age"]); // ""
        }

        //=========================================================================================
        public class BoolToVisibilityConverter : System.Windows.Data.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
                => (bool)value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
                => (Visibility)value == System.Windows.Visibility.Visible;
        }

        static void override_IValueConverter()
        {
            /*
                IValueConverter (WPF/MAUI 등 XAML 계열)

                ✅ 목적
                  - UI 바인딩 시 데이터 변환 로직 분리
                  - 예) bool -> Visible, int -> 색상
            */

            var converter = new BoolToVisibilityConverter();

            // True -> Visible
            object vis = converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);
            Console.WriteLine($"Convert(true): {vis}"); // Visible

            // False -> Collapsed
            vis = converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);
            Console.WriteLine($"Convert(false): {vis}"); // Collapsed

            // Visible -> true
            object b = converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture);
            Console.WriteLine($"ConvertBack(Visible): {b}"); // True

            // Collapsed -> false
            b = converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.InvariantCulture);
            Console.WriteLine($"ConvertBack(Collapsed): {b}"); // False
        }

        //=========================================================================================
        public class PersonEdit : System.ComponentModel.IEditableObject
        {
            private string _nameBackup;
            private int _ageBackup;

            public string Name { get; set; }
            public int Age { get; set; }

            public void BeginEdit()
            {
                // 현재 값을 백업해 둔다
                _nameBackup = Name;
                _ageBackup = Age;
                Console.WriteLine("BeginEdit: Backup stored");
            }

            public void EndEdit()
            {
                // 백업 불필요. 현재 값 확정
                Console.WriteLine("EndEdit: Commit changes");
            }

            public void CancelEdit()
            {
                // 백업 값으로 롤백
                Name = _nameBackup;
                Age = _ageBackup;
                Console.WriteLine("CancelEdit: Changes rolled back");
            }
        }

        static void override_IEditableObject()
        {
            /*
                EditableObject

                ✅ 목적
                  - UI에서 여러 속성을 한꺼번에 편집(수정/롤백) 기능 구현
                  - 주로 DataGrid, 폼 편집, 바인딩 객체의 Undo/Redo, 트랜잭션성 편집 등에 활용
            */
            var p = new PersonEdit { Name = "Alice", Age = 25 };

            Console.WriteLine($"Before edit: Name={p.Name}, Age={p.Age}");

            // 편집 트랜잭션 시작
            p.BeginEdit();

            // 값 수정 (임시)
            p.Name = "Bob";
            p.Age = 30;
            Console.WriteLine($"During edit: Name={p.Name}, Age={p.Age}");

            // 취소해보기
            p.CancelEdit();
            Console.WriteLine($"After CancelEdit: Name={p.Name}, Age={p.Age}");

            // 다시 편집
            p.BeginEdit();
            p.Name = "Carol";
            p.Age = 28;

            // 확정(커밋)
            p.EndEdit();
            Console.WriteLine($"After EndEdit: Name={p.Name}, Age={p.Age}");

            /*
                출력:
                Before edit: Name=Alice, Age=25
                BeginEdit: Backup stored
                During edit: Name=Bob, Age=30
                CancelEdit: Changes rolled back
                After CancelEdit: Name=Alice, Age=25
                BeginEdit: Backup stored
                EndEdit: Commit changes
                After EndEdit: Name=Carol, Age=28 
            */
        }

        public class MyComponent : ISupportInitialize
        {
            private bool _initializing = false;
            private int _a, _b;

            public int A
            {
                get { return _a; }
                set
                {
                    _a = value;
                    MaybeProcess();
                }
            }

            public int B
            {
                get { return _b; }
                set
                {
                    _b = value;
                    MaybeProcess();
                }
            }

            public void BeginInit()
            {
                Console.WriteLine("BeginInit() 호출됨 - 초기화 시작");
                _initializing = true;
            }

            public void EndInit()
            {
                Console.WriteLine("EndInit() 호출됨 - 초기화 끝, 최종 연산 수행");
                _initializing = false;
                Process();
            }

            private void MaybeProcess()
            {
                if (!_initializing)
                    Process();
            }

            private void Process()
            {
                Console.WriteLine($"Process: A={_a}, B={_b}");
            }
        }

        static void override_ISupportInitialize()
        {
            /*
                ISupportInitialize

                ✅ 목적
                  - WinForms/WPF 등에서 복수 속성을 "일괄 초기화"
                  - 초기화 중간 상태에서 이벤트/로직 실행 방지
            */

            var comp = new MyComponent();

            // 초기화 없이 속성 변경 시마다 연산 발생
            comp.A = 10; // Process: A=10, B=0
            comp.B = 20; // Process: A=10, B=20

            // ISupportInitialize 사용 예시
            comp.BeginInit();
            comp.A = 30; // Process 발생 안함
            comp.B = 40; // Process 발생 안함
            comp.EndInit(); // 여기서만 최종 연산 (Process: A=30, B=40)
        }

        public static void Test()
        {
            override_ISupportInitialize();

            override_IEditableObject();

            override_IValueConverter();

            override_IDataErrorInfo();

            override_INotifyDataErrorInfo();

            override_INotifyCollectionChanged();

            override_INotifyPropertyChanged();

            override_ICommand();
        }
    }
}
