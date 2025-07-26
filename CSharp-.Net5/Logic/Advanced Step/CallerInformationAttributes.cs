using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;





namespace AdvancedStep;

public class CallerInformationAttributes
{
    static void CallerInfoFeature([CallerMemberName] string caller = "")
    {
        Console.WriteLine($"Called from: {caller}");
    }

    static void CallerInformationAttributes_what()
    {
        /*
            호출자(Caller)의 정보를 자동으로 전달해주는 특성(Attribute) 입니다.
            주로 로깅, 디버깅, NotifyPropertyChanged 패턴 등에 사용됩니다.
        
            ✅ Caller Info Attribute 종류
            특성(Attribute)	        전달 정보	                설명
            [CallerMemberName]	    호출한 메서드 이름	        "Test", "Main" 등
            [CallerFilePath]	    호출한 파일 경로	        전체 경로 문자열
            [CallerLineNumber]	    호출한 소스 코드 줄 번호	정수 값

        */

        {
            // CallerMemberName이 자동으로 호출한 메서드 이름 "Test"를 전달합니다.
            CallerInfoFeature(); // ← 여기서 호출, 출력: Called from: Test
        }

        Console.ReadLine();
    }

    static void log_with_CallerInformationAttributes(string message, [CallerMemberName] string caller = "")
    {
        Console.WriteLine($"[{caller}] {message}");
    }


    public class Person : INotifyPropertyChanged
    {
        // 📌 이벤트 정의 (필수)
        public event PropertyChangedEventHandler? PropertyChanged;

        // 📌 공통적으로 사용하는 Raise 메서드
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ✅ 속성 예제 1
        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(); // propertyName 생략해도 자동 추론
                }
            }
        }

        // ✅ 속성 예제 2
        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                if (_age != value)
                {
                    _age = value;
                    OnPropertyChanged(); // 자동으로 "Age" 전달
                }
            }
        }

        // ✅ 속성 예제 3
        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        // 🧩 추가 개선 : 제네릭 SetProperty
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }
    }

    static void OnPropertyChanged_with_CallerInformationAttributes()
    {
        var person = new Person();
        person.PropertyChanged += (sender, e) =>
        {
            Console.WriteLine($"변경됨: {e.PropertyName}");
        };

        person.Name = "Alice";  // 출력: 변경됨: Name
        person.Age = 30;        // 출력: 변경됨: Age

        person.Email = "honggildong@gmail.com"; // 출력: 변경됨: Email
    }


    public static void Test()
    {
        //OnPropertyChanged_with_CallerInformationAttributes();

        //log_with_CallerInformationAttributes("My log message");

        //CallerInformationAttributes_what();
    }
}

