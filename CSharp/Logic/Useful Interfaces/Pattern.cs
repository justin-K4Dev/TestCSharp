using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace UsefulInterfaces
{
    public class Pattern
    {
        public class SimpleObservable : IObservable<int>
        {
            private List<IObserver<int>> observers = new List<IObserver<int>>();
            public IDisposable Subscribe(IObserver<int> observer)
            {
                observers.Add(observer);
                return new Unsubscriber(observers, observer);
            }
            public void Publish(int value)
            {
                foreach (var obs in observers) obs.OnNext(value);
            }
            private class Unsubscriber : IDisposable
            {
                List<IObserver<int>> _obs; IObserver<int> _target;
                public Unsubscriber(List<IObserver<int>> obs, IObserver<int> target) { _obs = obs; _target = target; }
                public void Dispose() { _obs.Remove(_target); }
            }
        }

        // 간단한 옵저버 구현
        public class ConsoleIntObserver : IObserver<int>
        {
            private string _name;
            public ConsoleIntObserver(string name) { _name = name; }

            public void OnNext(int value) => Console.WriteLine($"{_name} received: {value}");
            public void OnError(Exception error) => Console.WriteLine($"{_name} error: {error.Message}");
            public void OnCompleted() => Console.WriteLine($"{_name} completed");
        }

        static void override_IObservableT()
        {
            /*
                IObservable<T> / IObservable<T>

                ✅ 목적
                  - "옵저버 패턴" 직접 구현
                  - 이벤트/알림/스트림 등 Rx, Signal 시스템에서 활용                
            */

            var observable = new SimpleObservable();

            // 옵저버 구독
            var observerA = new ConsoleIntObserver("ObserverA");
            var observerB = new ConsoleIntObserver("ObserverB");

            IDisposable subscriptionA = observable.Subscribe(observerA);
            IDisposable subscriptionB = observable.Subscribe(observerB);

            // 값 푸시
            observable.Publish(1);
            observable.Publish(42);

            // observerA 구독 해지
            subscriptionA.Dispose();

            observable.Publish(100);

            // observerB도 구독 해지
            subscriptionB.Dispose();

            observable.Publish(200); // 아무것도 출력 안 됨
        }

        public static void Test()
        {
            override_IObservableT();
        }
    }
}
