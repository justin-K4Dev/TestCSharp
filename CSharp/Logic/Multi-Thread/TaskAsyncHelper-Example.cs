using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MultiThread
{
	public class TaskAsyncHelperExample
	{
        static async Task runAllTests()
        {
            Console.WriteLine("===== TaskAsyncHelper 테스트 시작 =====");

            // 1. Empty Task 테스트
            {
                Console.WriteLine("\n[Test] TaskAsyncHelper.Empty");
                await TaskAsyncHelper.Empty; // 그냥 바로 완료됨
                Console.WriteLine("Empty Task 완료");
            }

            // 2. Task + try-catch (기본) 테스트 - 예외 발생 Task
            {
                Console.WriteLine("\n[Test] try-catch (기본) - 예외 Task");
                var errorTask = Task.Run(() => { throw new InvalidOperationException("Test Exception"); });
                try { await errorTask; } catch (Exception e) { Console.WriteLine($"Exception:{e}"); }
                Console.WriteLine("try-catch 테스트 완료 (콘솔에 예외 출력 확인)");
            }

            // 3. tryCatch (핸들러 전달) 테스트
            {
                Console.WriteLine("\n[Test] tryCatch (핸들러)");
                var errorTask2 = Task.Run(() => { throw new ApplicationException("Custom Handler"); }).tryCatch(
                e => Console.WriteLine("핸들러에서 예외 처리: " + e?.Message));
                try { await errorTask2; } catch { /* swallow */ }
            }


            // 4. continueWithNotComplete 테스트 (비완료 Task만 처리)
            {
                Console.WriteLine("\n[Test] continueWithNotComplete");

                // TaskCompletionSource : Task의 완료/실패/취소를 제어하는 일종의 "비동기 약속 객체"
                var tcs = new TaskCompletionSource<object>();
                var errorTask3 = Task.Run(() => { throw new Exception("for continueWithNotComplete"); });
                errorTask3.continueWithNotComplete(tcs);
                try { await tcs.Task; } catch (Exception e) { Console.WriteLine("TaskCompletionSource 예외 캡처: " + e.Message); }
            }


            // 5. continueWith(Task)
            {
                Console.WriteLine("\n[Test] continueWith(Task)");
                var tcs2 = new TaskCompletionSource<object>();
                var okTask = Task.CompletedTask; // 이미 "정상 완료된" Task 객체, 실제로 비동기 작업 없이 바로 완료된 Task
                okTask.continueWith(tcs2);
                await tcs2.Task; // tcs2.Task가 완료될 때까지 대기, okTask가 이미 정상 완료였으니, tcs2도 정상 완료됨
                Console.WriteLine("continueWith(Task) 정상 완료");
            }


            // 6. continueWith<T>(Task<T>)
            {
                Console.WriteLine("\n[Test] continueWith<T>(Task<T>)");
                var tcs3 = new TaskCompletionSource<int>();
                var taskWithValue = Task.FromResult(777); // 즉시 완료된 Task<int> 생성 (값은 777)
                taskWithValue.continueWith(tcs3);
                Console.WriteLine("continueWith<T> 결과: " + await tcs3.Task); // 777, tcs.TrySetResult(t.Result) 정상 완료 !!!
            }


            // 7. interleave 테스트 (before, after)
            {
                Console.WriteLine("\n[Test] interleave");
                var tcs4 = new TaskCompletionSource<object>();

                // 여러개의 Task를 실행 한다. 순차적을 실행 !!!
                var interleaved = TaskAsyncHelper.interleave<int>(
                    (arg, next) => Task.Run(() =>
                    {
                        Console.WriteLine($"interleave before({arg})");
                        next(); // after를 호출
                        return Task.CompletedTask;
                    }),
                    () => Task.Run(() => Console.WriteLine("interleave after()")),
                    123,
                    tcs4);
                tcs4.SetResult(null); // 인위적으로 완료시킴
                await interleaved;
                Console.WriteLine("interleave 완료");
            }


            // 8. then - 기본 연속 실행
            {
                Console.WriteLine("\n[Test] then(Task, Action)");
                await Task.CompletedTask.then(() => Console.WriteLine("then 연속 액션!"));
            }


            // 9. then<T, TResult>(Task<T>, Func<T, TResult>)
            {
                Console.WriteLine("\n[Test] then<T, TResult>");
                var result = await Task.FromResult(42).then(x => x * 2);
                Console.WriteLine("then 결과: " + result); // 84
            }

            // 10. then(Task, Func<Task>)
            {
                Console.WriteLine("\n[Test] then(Task, Func<Task>)");
                await Task.CompletedTask.then(async () =>
                {
                    await Task.Delay(100);
                    Console.WriteLine("then Task -> Task");
                });
            }


            // 11. fastUnwrap 테스트
            {
                Console.WriteLine("\n[Test] fastUnwrap");
                var wrapped = Task.FromResult(Task.FromResult("unwrapped!"));
                var unwrapped = wrapped.fastUnwrap();
                Console.WriteLine("fastUnwrap 결과: " + await unwrapped); // unwrapped!
            }


            // 12. delay 테스트
            {
                Console.WriteLine("\n[Test] delay");
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await TaskAsyncHelper.delay(TimeSpan.FromMilliseconds(300));
                Console.WriteLine($"delay 300ms 완료, 실제: {sw.ElapsedMilliseconds}ms");
            }


            // 13. allSucceeded 테스트 (모두 성공)
            {
                Console.WriteLine("\n[Test] allSucceeded (모두 성공)");
                var allOk = new[] { Task.CompletedTask, Task.CompletedTask };
                await allOk.allSucceeded(() => Console.WriteLine("모두 성공!"));

                // 14. allSucceeded (실패 Task 포함)
                Console.WriteLine("\n[Test] allSucceeded (실패 Task)");
                var oneFail = new[] { Task.CompletedTask, Task.Run(() => { throw new Exception("fail"); }) };
                try
                {
                    await oneFail.allSucceeded(() => Console.WriteLine("실패해도 안나옴"));
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine("allSucceeded 예외 캡처: " + ex.InnerException.Message);
                }
            }


            // 15. fromMethod/fromResult/fromError 테스트
            {
                Console.WriteLine("\n[Test] fromMethod/fromResult/fromError");
                var fromOk = TaskAsyncHelper.fromMethod(() => Console.WriteLine("fromMethod 실행"));
                await fromOk;

                var fromVal = TaskAsyncHelper.fromResult(12345);
                Console.WriteLine("fromResult 값: " + await fromVal);

                var fromError = TaskAsyncHelper.fromError(new Exception("fromError 예외"));
                try { await fromError; } catch (Exception e) { Console.WriteLine("fromError 캡처: " + e.Message); }
            }


            // 16. canceled 테스트
            {
                {
                    var nonGenericMethod = typeof(TaskAsyncHelper).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                                                                  .FirstOrDefault(m => m.Name == "canceled" && !m.IsGenericMethod && m.GetParameters().Length == 0);

                    var canceledTask = nonGenericMethod.Invoke(null, null);
                }

                {
                    var genericMethod = typeof(TaskAsyncHelper).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                                                               .FirstOrDefault(m => m.Name == "canceled" && m.IsGenericMethod && m.GetParameters().Length == 0);
                    var canceledIntTask = genericMethod.MakeGenericMethod(typeof(int));

                    canceledIntTask.Invoke(null, null); // Task<int>
                }
            }


            // 17. getContinueWith 테스트 (메타프로그래밍)
            {
                Console.WriteLine("\n[Test] getContinueWith");
                var contInfo = TaskAsyncHelper.getContinueWith(typeof(Task));
                if (contInfo != null) Console.WriteLine("ContinueWith method found: " + contInfo.Method.Name);
            }

            Console.WriteLine("\n===== 모든 TaskAsyncHelper 테스트 완료 =====");

            Console.ReadLine();
        }

        public static void Test()
        {
            //runAllTests().GetAwaiter().GetResult();
        }
    }
}
