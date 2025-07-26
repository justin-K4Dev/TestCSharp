using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace UsefulInterfaces
{
    public class Service
    {
        public interface IMyService
        {
            void DoWork();
        }

        public class MyService : IMyService
        {
            public void DoWork()
            {
                Console.WriteLine("MyService.DoWork() called!");
            }
        }

        public class MyServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IMyService))
                    return new MyService();

                return null;
            }
        }

        static void override_IServiceProvider()
        {
            /*
                IServiceProvider

                ✅ 목적
                  - 의존성 주입(DI) 컨테이너의 핵심 인터페이스
                  - 서비스 인스턴스 반환
            */

            IServiceProvider provider = new MyServiceProvider();

            // 서비스 요청
            var service = provider.GetService(typeof(IMyService)) as IMyService;

            if (service != null)
            {
                service.DoWork();
            }
            else
            {
                Console.WriteLine("Service not found!");
            }

            // 출력:
            // MyService.DoWork() called!
        }

        public static void Test()
        {
            override_IServiceProvider();
        }
    }
}
