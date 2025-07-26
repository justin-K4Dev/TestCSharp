using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace UsefulInterfaces
{
    public class UI
    {
        public static async Task DownloadFileAsync(string url, IProgress<int> progress = null)
        {
            for (int i = 0; i < 100; ++i)
            {
                await Task.Delay(10);
                progress?.Report(i + 1); // 1~100%
            }
        }

        static void override_IProgressT()
        {
            /*
                IProgress<T>

                ✅ 목적
                  - 비동기 작업 진행률 UI 업데이트(WinForms/WPF/콘솔)
                  - Progress<T> 기본 구현 제공
            */

            // 사용 예시
            var p = new Progress<int>(percent => Console.WriteLine($"{percent}%"));
            DownloadFileAsync("http://...", p).Wait();
        }

        public static void Test()
        {
            override_IProgressT();
        }
    }
}
