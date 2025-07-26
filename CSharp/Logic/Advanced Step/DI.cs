using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace AdvancedStep
{
    public class DI
    {
        // 1. 서비스 인터페이스
        public interface IMessageService
        {
            void Send(string message);
        }

        // 2. 실제 서비스 구현체
        public class ConsoleMessageService : IMessageService
        {
            public void Send(string message)
            {
                Console.WriteLine($"[Console] {message}");
            }
        }

        // 3. 메시지 클라이언트 (서비스에 의존)
        public class MessageClient
        {
            private readonly IMessageService _messageService;
            // 생성자 주입
            public MessageClient(IMessageService messageService)
            {
                _messageService = messageService;
            }

            public void SendMessage(string msg)
            {
                _messageService.Send(msg);
            }
        }

        // 4. 테스트용 가짜 서비스(Mock)
        public class MockMessageService : IMessageService
        {
            public string LastMessage { get; private set; }
            public void Send(string message)
            {
                LastMessage = message;
            }
        }


        static void DI_what()
        {
            /*
                🌱 C# DI(Dependency Injection, 의존성 주입) 개요

                📌 의존성 주입이란?
                  - 클래스가 직접 필요한 객체(의존성)를 생성하지 않고
                    🔄 외부에서 주입(Injection) 받아 사용하는 설계 패턴.
                  - 클래스 간 결합도를 낮춰 유연하고 테스트하기 쉬운 구조를 만든다.

                🎯 주요 이점
                  - 🔗 결합도 감소: 코드 변경 시 영향 최소화, 확장성↑
                  - 🧪 테스트 용이: Mock/Fake 객체를 손쉽게 주입해 단위 테스트 가능
                  - ♻️ 재사용성/유지보수성 향상

                🧰 주입 방식
                  - 🏗️ 생성자 주입(Constructor): 가장 널리 사용. 불변성 보장, 명확한 의존성 표시
                  - 🏷️ 프로퍼티 주입(Property): 선택적 의존성에 활용
                  - 🛠️ 메서드 주입(Method): 특정 메서드 호출 시 의존성 전달

                ⚙️ ASP.NET Core 등에서는 DI 컨테이너가 내장되어 있어
                   등록(services.AddTransient<...> 등)만 하면 자동으로 객체를 관리/주입해준다.

                예) 서비스 인터페이스와 구현체, 클라이언트에 DI 적용 및 테스트 코드

            */

            {
                // 실제 서비스 사용
                IMessageService service = new ConsoleMessageService();
                var client = new MessageClient(service);
                client.SendMessage("Hello, DI!");

                // --- 테스트 코드 예시 ---
                var mockService = new MockMessageService();
                var testClient = new MessageClient(mockService);
                testClient.SendMessage("Test message!");

                Console.WriteLine(
                    mockService.LastMessage == "Test message!"
                    ? "테스트 성공"
                    : "테스트 실패"
                );
            }
        }


        public static void Test()
        {
            DI_what();
        }
    }
}
