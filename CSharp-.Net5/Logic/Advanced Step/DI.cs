using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;


namespace AdvancedStep;


// 1️. 서비스 인터페이스 및 구현체
public interface IMessageService
{
    string GetMessage();
}
public class HelloMessageService : IMessageService
{
    public string GetMessage() => "Hello, Dependency Injection!";
}

/*
    [ApiController]와 [Route("[controller]")] 애트리뷰트로
    URL 라우팅과 DI 대상임을 명시합니다.
*/
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly IMessageService _messageService;
    public HomeController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public string Get() => _messageService.GetMessage();
}

public class DI
{
    static void ASPDotNET_Core_built_in()
    {
        /*
            🏗️ ASP.NET Core Built-in DI Example

              1️. 서비스(인터페이스/구현체) 정의
              2️. Program.cs 또는 Startup.cs에서 서비스 등록
              3️. 생성자 주입을 통해 컨트롤러 또는 서비스에서 사용


            🛠️ 작동 절차
              1. 애플리케이션 시작 시 DI 컨테이너에 서비스 등록  
                 - AddTransient<IMessageService, HelloMessageService>()로 규칙 등록

              2. 클라이언트가 HTTP 요청(/home 등)을 보냄

              3. 라우팅 미들웨어가 URL을 HomeController로 매핑

              4. DI 컨테이너가 HomeController 인스턴스 생성  
                 - HomeController 생성자에서 IMessageService 의존성 요구  
                 - DI 컨테이너는 등록된 규칙에 따라 HelloMessageService 인스턴스 생성 및 주입  
                   (아래 구조 참고)  
                   HomeController ----(IMessageService 요구)----> [DI 컨테이너]  
                          ▲                                             │  
                          └----------(규칙 적용) <---- AddTransient<IMessageService, HelloMessageService>();

              5. HomeController의 액션 메서드(Get 등) 실행

              6. 응답 반환 및 컨트롤러/서비스 인스턴스 폐기(필요 시 Dispose)
        */

        /*
            var args = new[] { "--urls=http://localhost:5005" }; // 예시

            // 2️. 서비스 등록 (ASP.NET Core 6+ 방식, Program.cs 예시)
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddTransient<IMessageService, HelloMessageService>();
            builder.Services.AddControllers();

            var app = builder.Build();
            app.MapControllers();
            app.Run();
        */
    }

    public static void Test()
    {
        ASPDotNET_Core_built_in();
    }
}

