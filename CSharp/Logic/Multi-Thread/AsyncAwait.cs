using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread
{
	public class AsyncAwait
	{
        static void MainSync()
        {
            Console.WriteLine("A - 동기 진입");
            var task = AsyncFunc1(); // [B] 비동기 Task 반환
            task.Wait();             // [B-1] Task 완료까지 동기 대기 (블록)
            Console.WriteLine("E - 동기 리턴");
        }

        static async Task AsyncFunc1()
        {
            Console.WriteLine("B - await 진입");
            await AsyncFunc2(); // [C] 비동기 함수 대기
            Console.WriteLine("D - await 후 동기 코드");
        }

        static async Task AsyncFunc2()
        {
            Console.WriteLine("C - 비동기 진입");
            // Task.Delay(1000)은 TaskPool에서 타이머 완료 후 콜백 실행
            await Task.Delay(1000);
            Console.WriteLine("C2 - 비동기 완료 후 동기 코드");
        }

        static void myAsyncFuncBasic()
        {
            MainSync(); // ↓ 아래 흐름 참조

            /*
				┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
				┃ [전체 실행 흐름도 및 내부 동작 상세 + 상태머신/Continuation/MoveNext 연결 설명]                                ┃
				┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
				┃ [A] MainSync()                                                                                  
				┃     |                                                                                           
				┃     |-- "A - 동기 진입"                                                                         
				┃     |                                                                                           
				┃     |-- AsyncFunc1() 호출                                                                       
				┃     |     |                                                                                     
				┃     |     |-- "B - await 진입"                                                                  
				┃     |     |                                                                                     
				┃     |     |-- await AsyncFunc2() 호출                                                           
				┃     |     |     |                                                                               
				┃     |     |     |-- "C - 비동기 진입"                                                           
				┃     |     |     |                                                                               
				┃     |     |     |-- await Task.Delay(1000)                                                      
				┃     |     |     |     |                                                                         
				┃     |     |     |     |-- [1] Task 생성                                                         
				┃     |     |     |     |-- [2] 비동기 상태머신 클래스(<AsyncFunc2>d__0 등) 인스턴스 생성         
				┃     |     |     |     |      - C# 컴파일러가 자동 생성                                          
				┃     |     |     |     |      - IAsyncStateMachine 인터페이스 구현 (MoveNext 메서드 포함)        
				┃     |     |     |     |-- [3] 상태/콜백(continuation) 등록                                      
				┃     |     |     |     |      - await Task.Delay 시, TaskAwaiter의 continuation 큐(Queue+List)에       
				┃     |     |     |     |        '상태머신 인스턴스의 MoveNext'가 Action으로 등록됨               
				┃     |     |     |     |-- [4] 현재 TaskScheduler/SynchronizationContext 캡처                    
				┃     |     |     |     |-- [5] TaskPool 워커 스레드 반환 (메인 스레드는 대기/해제)               
				┃     |     |     |     |-- [6] TaskScheduler가 관리하는 스레드풀에서                             
				┃     |     |     |     |      1초 타이머 완료 후, 콜백 큐에 등록된 상태머신 인스턴스의           
				┃     |     |     |     |      MoveNext()가 (캡처된 컨텍스트에 따라) 실행 예약                    
				┃     |     |     |     |-- [7] 워커 스레드가 "C2 - 비동기 완료 후 동기 코드" 실행                
				┃     |     |     |     |-- [8] 워커 스레드는 작업 종료 후 다시 TaskPool로 반환                   
				┃     |     |     |                                                                               
				┃     |     |     |-- AsyncFunc2() 종료(Task 완료 반환)                                           
				┃     |     |                                                                                     
				┃     |     |-- "D - await 후 동기 코드"                                                          
				┃     |     |-- AsyncFunc1() 종료(Task 완료 반환)                                                 
				┃     |                                                                                           
				┃     |-- task.Wait() : AsyncFunc1의 Task 완료까지 동기 대기(블록)                                
				┃     |-- "E - 동기 리턴"                                                                         
				┃     |-- 함수 전체 종료                                                                          
				┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
				┃ ※ Continuation(콜백)은 반드시 '상태머신 클래스 인스턴스의 MoveNext'가 Task의 콜백 큐에 Action으로 등록됨  
				┃   - C# async/await 컴파일 시: private sealed class <AsyncFunc2>d__0 : IAsyncStateMachine {...}             
				┃   - await Task에서: awaiter.OnCompleted(this.MoveNext); (this는 상태머신 인스턴스)                         
				┃   - Task 완료 시: 콜백 큐에서 Action으로 등록된 MoveNext 실행 → await 뒤 코드 이어서 실행                 
				┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

				 - await 진입 시, 현재 TaskScheduler와 SynchronizationContext를 캡처.
				 - Task.Delay의 타이머가 만료되면,
				   TaskScheduler가 관리하는 스레드풀(TaskPool)에서 워커 스레드를 할당하여,
				   상태머신의 MoveNext() 콜백이 등록된 컨텍스트에서 실행됨.
				 - 동기 함수(MainSync)는 task.Wait()에서 비동기 Task가 끝날 때까지 블로킹됨.
            */
        }

        static async Task myAsyncVoidFunc()
		{
			// caller 스레드 1
			Console.WriteLine($"myAsyncVoidFunc() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// do nothing !!!

			// caller 스레드 1
			Console.WriteLine($"myAsyncVoidFunc() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 1
		}

		static async Task myAsyncVoidFuncDelay()
		{
			// caller 스레드 1
			Console.WriteLine($"myAsyncVoidFuncDelay() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncVoidFuncDelay >> Task.Delay(1000) 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			// caller 스레드 1
			var t1 = Task.Delay(1000);
			// caller 스레드 1
			Console.WriteLine($"\tEnd myAsyncVoidFuncDelay >> Task.Delay(1000) 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncVoidFuncDelay >> await Task.Delay(1000) 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			
			// return caller 스레드 1
			await Task.Delay(1000);
			// pop caller 스레드 4 from ThreadPool

			// caller 스레드 4
			Console.WriteLine($"\tEnd myAsyncVoidFuncDelay >> await Task.Delay(1000) 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// caller 스레드 4
			Console.WriteLine($"\tStart myAsyncVoidFuncDelay >> Task.ContinueWith() 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			var t2 = t1.ContinueWith((arg) =>
			{
				// pop caller 스레드 5 from ThreadPool

				// caller 스레드 5
				Console.WriteLine($"\tEnd myAsyncVoidFuncDelay >> Task.ContinueWith() 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// caller 스레드 5 push to ThreadPool
			});

			// caller 스레드 4
			Console.WriteLine($"\tStart myAsyncVoidFuncDelay >> await Task.Delay(1000) 3 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 4
			await Task.Delay(1000); 
			// pop caller 스레드 5 from ThreadPool

			// caller 스레드 5
			Console.WriteLine($"\tEnd myAsyncVoidFuncDelay >> await Task.Delay(1000) 3 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// caller 스레드 5
			Console.WriteLine($"\tStart myAsyncVoidFuncDelay >> Task.ContinueWith() 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			var t3 = t2.ContinueWith((arg) =>
			{
				// pop caller 스레드 6 from ThreadPool

				// caller 스레드 6
				Console.WriteLine($"\tEnd myAsyncVoidFuncDelay >> Task.ContinueWith() 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// return caller 스레드 6 to ThreadPool
			});

			// caller 스레드 5
			Console.WriteLine($"myAsyncVoidFuncDelay() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 5
		}

		static async Task myAsyncTaskFunc()
		{
			// caller 스레드 1
			Console.WriteLine($"myAsyncTaskFunc() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncTaskFunc >> Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			Task.Factory.StartNew(() =>
			{
				// pop caller 스레드 3 from ThreadPool
								
				Console.WriteLine($"\tEnd myAsyncTaskFunc >> Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// return caller 스레드 3 to ThreadPool
			});

			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncTaskFunc >> await Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 1
			await Task.Factory.StartNew(() =>
			{
				// pop caller 스레드 5 from ThreadPool

				Console.WriteLine($"\tEnd myAsyncTaskFunc >> await Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 5 to ThreadPool
			});
			// pop caller 스레드 5 from ThreadPool

			// caller 스레드 5
			Console.WriteLine($"\tStart myAsyncTaskFunc >> var t1 = Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			var t1 = Task.Factory.StartNew(() =>
			{
				// pop caller 스레드 4 from ThreadPool

				Console.WriteLine($"\tEnd myAsyncTaskFunc >> var t1 = Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 4 to ThreadPool
			});

			// caller 스레드 5
			Console.WriteLine($"\t\tStart myAsyncTaskFunc >> Task.ContinueWith() 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			var t2 = t1.ContinueWith((arg) =>
			{
				// pop caller 스레드 4 from ThreadPool

				Console.WriteLine($"\t\tEnd myAsyncTaskFunc >> Task.ContinueWith() 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 4 to ThreadPool
			});

			// caller 스레드 5
			Console.WriteLine($"\t\t\tStart myAsyncTaskFunc >> Task.ContinueWith() 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// push caller 스레드 5 to ThreadPool
			await t2.ContinueWith((arg) =>
			{
				// pop caller 스레드 4 from ThreadPool

				Console.WriteLine($"\t\t\tEnd myAsyncTaskFunc >> Task.ContinueWith() 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 4 to ThreadPool
			});
			// pop caller 스레드 4 from ThreadPool

			// caller 스레드 4
			Console.WriteLine($"myAsyncTaskFunc() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// push caller 스레드 4 to ThreadPool
		}

		static async Task<Int32> myAsyncTaskInt32Func()
		{
			// caller 스레드 1
			Console.WriteLine($"myAsyncTaskInt32Func() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncTaskInt32Func >> Task.Factory.StartNew() 1 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 1
			var result_1 = await Task<Int32>.Factory.StartNew(() =>
			{
				// pop caller 스레드 3 from ThreadPool

				var result = 10;
				Console.WriteLine($"\tEnd myAsyncTaskInt32Func >> Task.Factory.StartNew() 1 : result:{result} - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				return result;

				// push caller 스레드 3 to ThreadPool
			});
			// pop caller 스레드 3 from ThreadPool

			// caller 스레드 3
			Console.WriteLine($"\tStart myAsyncTaskInt32Func >> Task.Factory.StartNew() 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			Task.Factory.StartNew(() =>
			{
				// pop caller 스레드 4 from ThreadPool

				Console.WriteLine($"\tEnd myAsyncTaskInt32Func >> Task.Factory.StartNew() 2 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 4 to ThreadPool
			});
			// pop caller 스레드 3 from ThreadPool

			// caller 스레드 3
			Console.WriteLine($"\tStart myAsyncTaskInt32Func >> Task.Factory.StartNew() 3 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			// push caller 스레드 3 to ThreadPool
			await Task.Factory.StartNew(() =>
			{
				// pop caller 스레드 6 from ThreadPool

				Console.WriteLine($"\tEnd myAsyncTaskInt32Func >> Task.Factory.StartNew() 3 - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 6 to ThreadPool
			});
			// pop caller 스레드 6 from ThreadPool

			// caller 스레드 6
			Console.WriteLine($"myAsyncTaskInt32Func() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return 스레드 6
			return result_1;
		}

		public class MyResult
		{
			public MyResult(bool isSuccess)
			{
				IsSuccess = isSuccess;
			}

			public MyResult(string userId)
			{
				UserId = userId;
			}

			public bool IsSuccess { get; set; }

			public string UserId { get; set; }
		}

		static async Task<MyResult> myAsyncTaskCustomResultFunc()
		{
			// caller 스레드 1
			Console.WriteLine($"myAsyncTaskCustomResultFunc() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncTaskCustomResultFunc >> Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 1
			var result = await Task<MyResult>.Factory.StartNew(() =>
			{
				// pop caller 스레드 3 from ThreadPool

				var value = new MyResult("Hi justin");
				Console.WriteLine($"\tEnd myAsyncTaskCustomResultFunc >> Task.Factory.StartNew() : result:{value} - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				return value;

				// push caller 스레드 3 to ThreadPool
			});
			// pop caller 스레드 3 from ThreadPool

			// caller 스레드 3
			Console.WriteLine($"myAsyncTaskCustomResultFunc() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return 스레드 3
			return new MyResult(true);
		}

		static async Task<MyResult> myAsyncTaskCustomResultLambdaFunc()
		{
			// caller 스레드 1
			Console.WriteLine($"myAsyncTaskCustomResultLambdaFunc() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			var result = new MyResult(true);

			// caller 스레드 1
			Console.WriteLine($"\tStart myAsyncTaskCustomResultLambdaFunc >> Task<MyResult>.Run() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return caller 스레드 1
			result = await Task<MyResult>.Run(async delegate
			{
				// pop caller 스레드 3 from ThreadPool

				Console.WriteLine($"\t\tStart await myAsyncTaskInt32Func() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				var task_result = await myAsyncTaskInt32Func();
				if (task_result == 1)
				{
					result.UserId = "Justin Best !!!";
					result.IsSuccess = true;
				}
				Console.WriteLine($"\t\tEnd await myAsyncTaskInt32Func() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				Console.WriteLine($"\tEnd myAsyncTaskCustomResultLambdaFunc >> Task<MyResult>.Run() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// push caller 스레드 5 to ThreadPool
				return result;
			});
			// pop caller 스레드 5 from ThreadPool

			// caller 스레드 5
			Console.WriteLine($"myAsyncTaskCustomResultLambdaFunc() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			// return 스레드 5
			return result;
		}

        static async Task async_and_await_what()
		{
            /*
				📚 async + await
				
				  - C# 5.0부터 새로운 C# 키워드로 async와 await가 추가되었다.
				  - 이 키워드들은 기존의 비동기 프로그래밍 (asynchronous programming)을 보다
				    손쉽게 지원하기 위해 C# 5.0에 추가된 중요한 기능이다.

				  - async는 컴파일러에게 해당 메서드가 await를 가지고 있음을 알려주는 역활을 한다.
				    async 라고 표시된 메서드는 await를 1개 이상 가질 수 있는데, 하나도 없는 경우라도 컴파일은 가능하지만 Warning 메시지를 표시한다.
				    async를 표시한다고 해서 자동으로 비동기 방식으로 프로그램을 수행하는 것은 아니고,
				    일종의 보조 역활을 하는 컴파일러 지시어로 볼 수 있다.

				  - 실제 핵심 키워드는 await인데, 이 await는 일반적으로 Task 혹은 Task<T> 객체와 함께 사용된다.
			        Task 이외의 클래스도 사용 가능한데, awaitable 클래스, 즉 GetAwaiter() 라는 메서드를 갖는 클래스이면 함께 사용 가능하다.

				  - UI 프로그램에서 await는 Task와 같은 awaitable 클래스의 객체가 완료되기를 기다리는데,
				    여기서 중요한 점은 UI 쓰레드가 정지되지 않고 메시지 루프를 계속 돌 수 있도록
				    필요한 코드를 컴파일러가 await 키워드를 만나면 자동으로 추가한다는 점이다.
				  - 메시지 루프가 계속 돌게 만든다는 것은 마우스 클릭이나 키보드 입력 등과 같은 윈도우 메시지들을 계속 처리할 수 있다는 것을 의미한다.
				    await는 해당 Task가 끝날 때까지 기다렸다가 완료 후, await 바로 다음 실행문부터 실행을 계속한다.
				    await가 기다리는 Task 혹은 실행 메서드는 별도의 Worker Thread에서 돌 수도 있고, 또는 UI Thread에서 돌 수도 있다.

				
				✅ 동작 비교	
				  | 항목                    | 기본 `await`                 | `await ...ConfigureAwait(false)`            | `await` 생략                                       | `.Result` / `.Wait()`                                |
				  |-------------------------|------------------------------|---------------------------------------------|----------------------------------------------------|------------------------------------------------------|
				  | 컨텍스트 캡처           | O (기본)                     | X (안함)                                    | 해당 없음                                          | 해당 없음                                            |
				  | continuation 실행 위치  | 원래 컨텍스트(UI 스레드 등)  | 스레드풀(또는 아무 스레드)                  | 바로 다음 코드(비동기 작업 백그라운드)             | 같은 스레드(블로킹)                                  |
				  | UI 접근 안전성          | 안전(복귀하면 UI 접근 가능)  | 안전하지 않음 (UI 접근 불가 가능)           | 안전하지 않음(작업 끝나기 전에 UI 갱신하면 문제)   | 안전(하지만 UI 정지)                                 |
				  | 성능/교착 위험          | 컨텍스트 복귀 비용 있음      | 컨텍스트 복귀 비용 없음 → 성능 유리        | 호출자 관리 필요                                   | 데드락 위험 (특히 SynchronizationContext 있는 환경)  |
				  | 추천 사용처             | UI 코드, 필요한 경우         | 라이브러리/백엔드 코드(컨텍스트 불필요시)   | 병렬 작업 시작 시                                  | 거의 사용 금지 (UI/서버)                             |


				Caller 스레드가 await로 작성된 Callee 함수를 만날 경우 다음과 같이 구분하여 처리 한다.
				
			    1. Callee 함수 내부에서 Task 생성 및 실행을 하는 경우
				  1.1. Callee 함수가 Task 반환일 경우 (리턴값 없음) 
				     - Caller 스레드는 Callee 함수 내부에 Task 생성 및 실행 함수를 처리 하게되면
			           ThreadPool 에서 대기중 스레드를 꺼내어 Task 에 등록된 함수를 실행 시키고,
			           함수 처리가 완료 되면 ThreadPool 에 스레드를 반환 한다.			           
			           Callee 함수 처리가 완료 되면 Callee 함수가 반환해 준 스레드로 다음 로직을 실행 한다.
					   현재 Caller 함수에서 리턴된 Caller 스레드가 없는 경우 리턴하여 다음 로직을 실행하고,
			                                       Caller 스레드가 있는 경우 ThreadPool 에 반환 한다.			           
			        				
				  1.2. Callee 함수가 Task<T> 반환일 경우 (리턴값 있음)
				     1.2.1. 현재 함수를 호출한 Caller 함수가 로직에서 await + Callee 함수 + Task<T>값을 반환 받는 경우
			              - Caller 스레드는 Callee 함수 내부에 Task 생성 및 실행 함수를 처리 하게되면
			                ThreadPool 에서 대기중 스레드를 꺼내어 Task 에 등록된 함수를 실행 시켜주고,
						    함수 처리가 완료 되면 ThreadPool 에 스레드를 반환 한다.	
				            Callee 함수 처리가 완료될 때까지 대기 하고 처리가 완료 되면
			                ThreadPool 에서 대기중인 스레드를 꺼내어 다음 로직을 실행 한다. 
						    현재 Caller 함수에서 리턴된 Caller 스레드가 없는 경우 리턴 하고
			                                            Caller 스레드가 있는 경우 ThreadPool 에 반환 한다.
					      
				     1.2.2. 현재 함수를 호출한 Caller 함수가 await + Callee 함수 작성만 한 경우
			              - Caller 스레드는 Callee 함수 내부에 Task 생성 및 실행 함수를 처리 하게되면
			                ThreadPool 에서 대기중 스레드를 꺼내어 Task 에 등록된 함수를 실행 시켜주고,
							함수 처리가 완료 되면 ThreadPool 에 스레드를 반환 하고,
			                ThreadPool 에서 대기중인 스레드를 꺼내어 다음 로직을 실행 한다. 
						    현재 Caller 함수에서 리턴된 Caller 스레드가 없는 경우 리턴하여 다음 로직을 실행하고,
			                                            Caller 스레드가 있는 경우 ThreadPool 에 반환 한다.
			      
			         1.2.3. 현재 함수를 호출한 Caller 함수가 Callee 함수 작성만 한 경우
			              - Caller 스레드는 Callee 함수 내부에 Task 생성 및 실행 함수를 처리 하게되면
			                ThreadPool 에서 대기중 스레드를 꺼내어 Task 에 등록된 함수를 실행 시켜주고,
			                함수 처리가 완료 되면 ThreadPool 에 스레드를 반환 한다.	
			                현재 Caller 스레드로 다음 로직을 실행 한다.
						    현재 Caller 함수에서 리턴된 Caller 스레드가 없는 경우 리턴하여 다음 로직을 실행하고,
			                                            Caller 스레드가 있는 경우 ThreadPool 에 반환 한다.

			    2. Callee 함수 내부에서 Task 생성 및 실행을 하지 않는 경우
			     - 현재 Caller 스레드로 다음 로직을 실행 하고, 반환 한다.
			       

				| 구분 | Caller 호출 방식              | Callee 내부 `await`   | 실행 특성                                                    | 설명                                                                            |
				|------|-------------------------------|-----------------------|--------------------------------------------------------------|---------------------------------------------------------------------------------|
				| ①   | `await Callee()`              | ❌ 없음              | 즉시 완료, 동기 실행                                         | 내부 `await` 없음 → `Task.CompletedTask` 반환됨, 스레드 점유 거의 없음         |
				| ②   | `await Callee()`              | ✅ 있음              | 비동기 처리 `await` 지점에서 중단 후 재개                    | 실제로 Task 스레드로 분리되어, 중단 후 복귀 시점까지 대기                       |
				| ③   | `Callee();` (`await` 안 함)   | ✅ 있음              | Fire-and-forget, 예외 처리 안 됨 (주의)                      | `Caller`는 결과 기다리지 않고 넘어감. 비동기 작업은 백그라운드에서 실행됨       |
				| ④   | `Callee();` (`await` 안 함)   | ❌ 없음              | 즉시 완료, 일반 함수 호출과 동일                             | `Task.CompletedTask`만 반환. 별도의 비동기 처리는 없음                          |


				🔹 await로 호출하더라도, Callee 내부에 await이 없으면 즉시 동기 처리 된다.
				🔹 Callee 내부에 await이 있을 때만 실제로 비동기 전환이 일어나며,
				🔹 Caller가 await하지 않으면 해당 비동기 작업은 fire-and-forget으로 실행된다.



				✅ async + await와 멀티 스레드의 차이점

				  1. 개념상의 차이

				    - 멀티 스레드(Thread)
					  * OS 스레드를 직접 여러 개 만들어서 동시에 돌리는 것.
					  * "실제로 코어 여러 개에 올려서 동시에 실행"하는 걸 노린다.
					  * 스레드 수만큼 실행 흐름이 실제로 존재한다.

				    - async + await
					  * "비동기 흐름을 표현"하는 문법/추상화.
					  * 스레드를 하나 더 만드는 게 아니라,
					    "이 작업은 여기까지 하고, 나머지는 나중에 이어서" 라고 등록하는 것.
					  * 필요한 시점에만 스레드풀 스레드를 빌려와서 이어서 실행한다.

				  2. 스레드 생성/관리 관점

				    - Thread
					  * new Thread(...) 하면 OS 스레드가 실제로 하나 더 생김.
					  * 스택 공간도 따로 있고, OS 스케줄링 대상이 하나 더 생김.
					  * 많이 만들면 컨텍스트 스위칭 비용이 바로 증가.

				    - Task / async
					  * 보통 기존 ThreadPool 스레드를 재사용.
					  * "작업"만 늘어나고 "스레드"는 크게 안 늘어난다.
					  * 대기 중에는 스레드를 점유하지 않는다.

				  3. I/O 대기 처리 방식
				   
				    - Thread 방식 (블로킹)
					  * 스레드가 소켓/파일/DB를 기다리는 동안에도 그 스레드는 살아서 "기다리고" 있음.
					  * 요청이 많아지면 스레드도 그만큼 필요 → 메모리/스케줄링 오버헤드 증가.

				    - async/await 방식 (논블로킹)
					  * await에서 대기할 때 스레드를 풀어준다.
					  * I/O가 끝났을 때만 스레드가 다시 붙는다.
					  * 같은 수의 스레드로 더 많은 동시 요청을 처리할 수 있음.

				  4. CPU-bound 작업일 때
				   
				    - Thread
					  * CPU를 계속 쓰는 계산 작업을 코어 수만큼 돌리고 싶을 때 적합.
					  * 스레드를 코어 수와 맞춰두면 진짜 병렬 계산이 된다.

				    - async/await
					  * CPU 계산 자체를 빠르게 하지는 못한다.
					  * 계산을 비동기로 만들고 싶으면 결국 Task.Run(...)으로 스레드풀에 던져야 한다.
					  * 즉, CPU-bound는 원래부터 멀티스레드/병렬 처리 문제다.

				  5. 실행 위치 보장
				   
				    - Thread
					  * 내가 만든 스레드에서 그 코드가 계속 돈다. (그 스레드 컨텍스트에 묶여 있음)

				    - async/await
					  * await 이후에는 "어떤 스레드에서 이어서 실행될지"가 컨텍스트/SynchronizationContext에 따라 달라진다.
					  * 콘솔/서버 환경에서는 다른 스레드풀 스레드가 이어받을 수 있다.
					  * 그래서 await 뒤 코드는 "스레드가 같을 거다"라고 가정하면 안 된다.

				  6. 코드 복잡도 / 예외 처리
				   
				    - Thread
					  * 수명 제어(시작/종료), 예외 처리, 동기화(lock) 등을 직접 신경써야 함.
					  * 스레드가 폭주하거나 안 죽으면 관리가 어려움.

				    - Task / async
					  * 예외는 Task에 담기고 await에서 다시 throw 되므로 흐름이 자연스럽다.
					  * 수명 관리가 상대적으로 쉽다. (await가 끝나면 그 작업은 끝난 것)

				  7. 스케일링 관점 요약

				    - Thread 방식:
					  * 요청 수 = 스레드 수 구조로 가면 금방 한계.
					  * 스레드가 많아질수록 컨텍스트 스위칭/메모리 소모 증가.

				    - async/await 방식:
					  * 요청 수 >> 스레드 수 구조가 가능
					  * "많이 기다리는" 서버(IO 위주 웹/게임 API)에서 특히 유리

				  8. 한 줄로 비교해보면
				     
				    - 멀티 스레드: "실제로 동시에 돌릴 스레드를 여러 개 둔다"
				    - async/await: "기다리는 동안 스레드를 빼놨다가, 끝나면 다시 이어서 돌린다"

				  9. 그래서 어디에 뭘 쓰나?
				   
				    - CPU를 갈아 넣는 계산, 물리, 대량 암호화:			멀티스레드/Parallel
				    - 네트워크/DB/API 호출이 아주 많음:					async/await
				    - UI 스레드 하나만 있는 앱에서 I/O 대기 없애기:		async/await
            */
            {
                //Console.WriteLine($"async_and_await_what() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

                //await myAsyncVoidFunc();

                //await myAsyncVoidFuncDelay();

                //await myAsyncTaskFunc();

                //await myAsyncTaskInt32Func();

                //await myAsyncTaskCustomResultFunc();

                //await myAsyncTaskCustomResultLambdaFunc();

                //myAsyncFuncBasic();

                //Console.WriteLine($"async_and_await_what() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

                //Console.ReadLine();
            }
        }

		static async Task<int> ExampleAsync()
		{
            int x = 1;
            await Task.Delay(1000);
            return x + 1;

            /*
				컴파일러는 async_await() 메서드를 다음과 같이 상태머신 클래스로 변환합니다:
		
				private sealed class <ExampleAsync>d__0 : IAsyncStateMachine
				{
					public int <>1__state;								// 상태 변수 (state)
					public AsyncTaskMethodBuilder<int> <>t__builder;	// Task 생성 및 완료 관리
					public int x;										// 지역 변수, 상태로 보존
					private TaskAwaiter <>u__1;							// awaiter 보존

					public void MoveNext()
					{
						// 상태(state)에 따라 실행 분기
						try
						{
							int result;
							if (<>1__state == 0)
							{
								// 재개: await 뒤 코드 실행
								goto label_AWAIT_RESUME;
							}

							// 초기 진입: 동기 코드 실행
							x = 1;

							// await Task.Delay(1000) → Awaiter 생성
							<>u__1 = Task.Delay(1000).GetAwaiter();

							if (!<>u__1.IsCompleted)
							{
								<>1__state = 0;				// state=0: 첫 await 이후
								<>t__builder.AwaitUnsafeOnCompleted(ref <>u__1, ref this);
								return;
							}

						label_AWAIT_RESUME:
							<>u__1.GetResult();
							result = x + 1;
							// Task를 완료시키고 상태머신 종료
							<>t__builder.SetResult(result);
						}
						catch (Exception ex)
						{
							<>t__builder.SetException(ex);
						}
					}

					public void SetStateMachine(IAsyncStateMachine stateMachine) { ... }
				}

				// IL: async 상태머신의 MoveNext() 메서드
				.method public hidebysig virtual instance void MoveNext() cil managed
				{
					.maxstack 3
					.locals init (
						[0] int32 result,            // 지역 변수: 반환할 값 (int result)
						[1] System.Exception ex      // 지역 변수: 예외 처리용 (Exception ex)
					)

					// try 블록 진입 (모든 async 상태머신은 예외를 Task로 포착)
					.try
					{
						// 상태(state) 확인
						// if (this.<>1__state == 0) goto label_AWAIT_RESUME;
						ldarg.0                                  // this
						ldfld int32 <>1__state                   // 상태 변수 로드
						brfalse.s     LABEL_AWAIT_RESUME         // 0이면(즉, await에서 resume) 아래 동기 코드 skip하고 label로 점프

						// ----------- [초기 진입 코드] -----------

						// x = 1;
						ldarg.0
						ldc.i4.1                                 // 1
						stfld int32 x                            // this.x = 1 (상태머신에 지역 변수 저장)

						// <>u__1 = Task.Delay(1000).GetAwaiter();
						ldc.i4 1000                              // 1000
						call class [System.Threading.Tasks]System.Threading.Tasks.Task [System.Threading.Tasks]System.Threading.Tasks.Task::Delay(int32)
						callvirt instance valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter [System.Threading.Tasks]System.Threading.Tasks.Task::GetAwaiter()
						stfld valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter <>u__1 // awaiter 보관

						// if (!<>u__1.IsCompleted)
						ldarg.0
						ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter <>u__1 // awaiter의 주소(참조) 로드
						call instance bool [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
						brtrue.s LABEL_AWAIT_RESUME                  // 완료되었으면 resume로

						// ----------- [비동기 미완료: 일시정지/콜백 등록] -----------

						// 상태 설정(0: 첫 await 직후로 돌아오도록)
						ldarg.0
						ldc.i4.0
						stfld int32 <>1__state                       // this.<>1__state = 0

						// 콜백(continuation) 등록:
						// AwaitUnsafeOnCompleted는 'awaiter'가 완료되면 this.MoveNext()를 호출하도록 콜백 큐에 등록
						ldarg.0
						ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter <>u__1 // ref awaiter
						ldarg.0                                     // ref 상태머신(this)
						ldftn instance void <ExampleAsync>d__0::MoveNext() // MoveNext 함수 포인터
						// 내부적으로 TaskAwaiter.OnCompleted(this.MoveNext)와 동일하게 동작
						newobj instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>::AwaitUnsafeOnCompleted<class [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter, class [this]Namespace.<ExampleAsync>d__0>(...)
						ret                                         // 일시정지 (비동기 대기) - 이후 Task 완료시 콜백으로 복귀

					LABEL_AWAIT_RESUME:
						// ----------- [await 뒤로 resume되는 코드] -----------
						// <>u__1.GetResult();
						ldarg.0
						ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter <>u__1
						call instance void [System.Runtime]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
						// (예외가 있으면 여기서 throw되어 catch로 이동)

						// result = x + 1;
						ldarg.0
						ldfld int32 x
						ldc.i4.1
						add
						stloc.0                                    // result = x + 1

						// Task 완료 통지: <>t__builder.SetResult(result)
						ldarg.0
						ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> <>t__builder
						ldloc.0
						call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>::SetResult(int32)
						leave.s END_TRY                            // try/catch 블록 종료

					} // end .try

					// ----------- [예외 처리 분기] -----------
					// catch (Exception ex)
					catch [System.Runtime]System.Exception
					{
						stloc.1 // ex
						// Task를 예외로 완료
						ldarg.0
						ldflda valuetype [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> <>t__builder
						ldloc.1
						call instance void [System.Runtime]System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>::SetException(class [System.Runtime]System.Exception)
						leave.s END_TRY
					}

					END_TRY:
					ret
				}
			*/
        }

        static void async_await_IL_code()
		{
            ExampleAsync().Wait();

            /*
                ⏳ 실행 흐름 (런타임)

                  - 전체 흐름 도식
                    [호출자] ------> ExampleAsync() ----> ( 상태머신 인스턴스 생성 )
                                             |
                                             |----[ 동기 코드: x=1 실행 ]
                                             |
                                             |----[ await Task.Delay(1000) ]
                                             |        |
                                             |        |--( Task 생성, awaiter 준비 )
                                             |        |--( awaiter.IsCompleted? 대부분 false )
                                             |        |--[ 상태 0로 저장
			                                 |        |  , 현재 SynchronizationContext(TaskScheduler) 캡처
                                             |        |  , 상태머신 인스턴스의 MoveNext 콜백(Continuation) 등록
			                                 |        |  , return ]
                                             |        |
                                [Task.Delay 완료되면]
                                             |--( 캡처된 컨텍스트/ThreadPool 워커에서 MoveNext() 실행 )
                                             |--[ await 뒤 코드 resume, awaiter.GetResult(), return x+1 ]
                                             |--[ Task 완료 및 Result=2 ]


                  (1) ExampleAsync() 호출
                    - 호출 즉시
                      → 상태머신 인스턴스 생성 (Program+ExampleAsync>d__0) // 클래스명이 Program, 메서드명이 ExampleAsync 라면
                      → 반환값은 Task<int> (아직 미완료 Task)

                  (2) 상태 -1(초기): 동기 코드 실행
                    - 상태: <>1__state = -1
                    - int x = 1; 실행

                  (3) await Task.Delay(1000)
                    - Task.Delay(1000) 실행 → 비동기 Task 반환
                      → Awaiter(TaskAwaiter) 생성
                    - awaiter.IsCompleted 검사 (대부분 false)

                  (4) 미완료 시: 콜백 등록 & 상태 보존
                    - if (!awaiter.IsCompleted)
                      → 상태머신의 상태를 0으로 변경(<>1__state = 0)
                      → 현재 SynchronizationContext(TaskScheduler)를 캡처해 상태머신에 저장
                      → TaskAwaiter에 상태머신 인스턴스의 MoveNext를 Continuation(콜백)으로 등록
                      → 현재 호출 스레드는 return!
                         (ExampleAsync는 “아직 끝나지 않은 Task”를 반환)

                  (5) Task.Delay가 완료되면
                    - 타이머가 끝나고 TaskAwaiter가 등록해둔 MoveNext() 콜백 실행
                    - (ThreadPool 워커 스레드, 또는 캡처된 SynchronizationContext/TaskScheduler에서 실행)

                  (6) 상태 0: await 뒤 코드 재개
                    - 상태: <>1__state == 0
                    - await 뒤 resume label(IL 분기)로 이동
                    - awaiter.GetResult() 호출 (예외 있으면 throw)
                    - return x + 1; 실행 → 2 반환

                  (7) Task 완료 및 반환
                    - AsyncTaskMethodBuilder<int>.SetResult(2) 호출
                    - ExampleAsync에서 반환한 Task<int>가 완료(Task 상태 Success) 및 Result = 2

                  (8) 예외 발생시
                    - 중간에 예외 발생
                      → AsyncTaskMethodBuilder<int>.SetException(ex) 호출
                         → Task가 Faulted 상태로 완료됨

                ---------------------------------
                ⭐️ 보충 설명:
                  - await 시점(4)에서 반드시 현재 컨텍스트(SynchronizationContext/TaskScheduler)를 캡처해둠
                    (UI 앱: UI 스레드 컨텍스트, ASP.NET: 요청 컨텍스트 등)
                  - Task가 끝나면(5), 콜백(MoveNext)이 캡처된 컨텍스트에서 실행됨 (동일 스레드 보장)
                    → UI/ASP.NET 등에서 await 뒤 코드가 "반드시" 같은 컨텍스트(스레드)에서 실행되는 이유!
                  - ConfigureAwait(false) 사용 시 이 캡처/복귀 과정이 생략됨 (ThreadPool에서 바로 실행)


				🌟 await 기본 절차
				  1. 현재 SynchronizationContext(또는 TaskScheduler) 캡처
				    - await를 만나는 시점의 "현재 컨텍스트"를 기억해둡니다.
                    - 예) WinForms/WPF: UI SynchronizationContext, ASP.NET: HttpContext 등
				  2. 비동기 작업(Task) 시작
                    - 예: await Task.Delay(1000);
				    - 이 Task가 아직 끝나지 않았으면
				      ▶️ 현재 async 메서드의 상태머신이 “일시 정지
                      ▶️ 나머지 코드는 “콜백(Continuation)”으로 등록
				  3. Task가 완료될 때까지 컨트롤 반환
			        - 호출한 스레드는 await 뒤의 코드를 실행하지 않고 메서드(또는 호출 스레드)에 제어권 반환
   				  4. Task가 완료되면
				    - 등록된 콜백(상태머신의 MoveNext)이
				      캡처된 SynchronizationContext/TaskScheduler에서 실행되도록 예약됩니다.
				  5. 컨텍스트에서 await 뒤 코드 실행
				    - Dispatcher나 메시지 루프, SynchronizationContext.Post 등으로
				     “기존 컨텍스트”에서 await 뒤 코드가 반드시 실행됨
				     (예: UI 앱은 무조건 UI 스레드, ASP.NET은 요청 컨텍스트)
            */
        }

        static async void async_await_schedule()
		{
			Task t = myAsyncTaskFunc();

			for (int i = 0; i < 10; i++)
			{
				System.Console.WriteLine("Do Something Before myAsyncTaskFunc");
			}

			await t;

			for (int i = 0; i < 10; i++)
			{
				System.Console.WriteLine("Do Something after myAsyncTaskFunc");
			}

			Console.ReadLine();
		}

		static async Task<MyResult> getUserId()
		{
			Console.WriteLine("Try call getUserId() from DB");
			await Task.Delay(5000);
			Console.WriteLine("completed getUserId()");

			return new MyResult("Justin Best !!!");
		}

		static async void async_await_transaction()
		{
			var result = await getUserId();

			Console.WriteLine("Waiting user id !!!");

			var user_id = result.UserId;

			Console.WriteLine("taken value - userId:{0}", user_id);

			Console.ReadLine();

			/*
				Try call getUserId() from DB
				Waiting user id !!!
				completed getUserId()
				taken value - userId:10000        
			*/
		}

		static void Task_or_async_method()
		{
			/*
				C# 5.0과 함께 선보인 .NET 4.5는 기존의 동기화(Synchronous) 메서드들과
				구분하여 C#의 await (혹은 VB의 Await)를 지원하기 위해 많은 Async 메서드들을 추가하였다.
				이 새 메서드들은 기본적으로 기존의 Synchronous 메서드명 뒤에 Async를 붙여 명명되었는데,
				만약 기존에 Async로 끝나는 함수가 이미 있었던 경우에는 TaskAsync를 메서드명에 붙여 명명하였다.

					System.IO.Stream.Read() : 기존 동기 메서드
					System.IO.Stream.ReadAsync() : 4.5 Async 메서드

					WebClient.DownloadStringAsync() : 기존 비동기 메서드
					WebClient.DownloadStringTaskAsync() : 4.5 TaskAsync 메서드
			*/
			{
				Console.ReadLine();
			}
		}

		static void doSleepSync(string caller)
		{
			System.Threading.Thread.Sleep(10000);

			Console.WriteLine($"return doSleepSync() - caller:{caller}");
		}

		static async Task doDelayAsync(string caller)
		{
			await Task.Delay(10000);

			Console.WriteLine($"return doDelayAsync() - caller:{caller}");
		}

		static void doTaskAwaitAfterBlockFunc(string caller)
		{
			var t = Task.Factory.StartNew(() => doSleepSync(caller));
			t.Wait();

			// Task.Wait은 Task가 끝날 때까지 기다리는 블로킹 방식 !!!
			// doSleepSync() 는 블로킹 함수라서 지연 시간 만큼 블록킹 된다.
		}

		static void doTaskAwaitAfterNonBlockFunc(string caller)
		{
			var t = Task.Factory.StartNew(() => doDelayAsync(caller));
			t.Wait();

			// Task.Wait은 Task가 끝날 때까지 기다리는 블로킹 동기화 방식 !!!
			// doDelayAsync() 는 논블로킹 함수라서 지연 시간 만큼 블록되지 않고 반환 된다.
			// 따라서 Task.Wait() 가 블로킹 함수이지만 doDelayAsync() 함수가 즉시 반환되어
			// Task.Wait() 도 바로 반환 하게 된다.
		}

		static async Task doAwaitAfterBlockFunc(string caller)
		{
			var t = Task.Factory.StartNew(() => doSleepSync(caller));
			await t;

			// await은 해당 Task가 완료될 때까지 다른 작업을 진행하다가 작업이 완료되면
			// 해당 작업 이후 남겨진 루틴을 처리하는 논블록킹 방식 !!!

			// doSleepSync() 가 블로킹 함수이지만 호출자 async 와 await 구문에 의해
			// await t 는 논블로킹으로 바로 반환 하게 된다.
		}

		static async Task doAwaitAfterNonBlockFunc(string caller)
		{
			var t = Task.Factory.StartNew(async () => await doDelayAsync(caller));
			await t;

			// await은 해당 Task가 완료될 때까지 다른 작업을 진행하다가 작업이 완료되면
			// 해당 작업 이후 남겨진 루틴을 처리하는 논블록킹 방식 !!!

			// 함수에 남겨진 루틴을 처리하기 위해 await 에 해당되는 작업이 완료되었을 때,
			// 해당 구문으로 돌아가라는 명령을 작업 항목 큐에 삽입하게 된다 !!!
		}

		static async Task TaskWait_vs_await()
		{
			https://stackoverflow.com/questions/9519414/whats-the-difference-between-task-start-wait-and-async-await

			doAwaitAfterNonBlockFunc("doAwaitAfterNonBlockFunc").Wait(); // 함수 내부에서 NonBlockFunc 함수를 호출 및 Task 를 await 하고 있어서 즉시 반환 된다 !!!
			Console.WriteLine("doAwaitAfterNonBlockFunc().Wait() completed");

			await doAwaitAfterNonBlockFunc("await doAwaitAfterNonBlockFunc");
			Console.WriteLine("await doAwaitAfterNonBlockFunc() completed");

			doTaskAwaitAfterNonBlockFunc("doTaskAwaitAfterNonBlockFunc");
			Console.WriteLine("doTaskAwaitAfterNonBlockFunc() completed");

			doAwaitAfterBlockFunc("doAwaitAfterBlockFunc");
			Console.WriteLine("doAwaitAfterBlockFunc() completed");

			doAwaitAfterBlockFunc("doAwaitAfterBlockFunc").Wait(); // 여기서 블록킹 처리 된다. !!!
			Console.WriteLine("doAwaitAfterBlockFunc().Wait() completed");

			doTaskAwaitAfterBlockFunc("doTaskAwaitSync"); // 여기서 블록킹 처리 된다. !!!
			Console.WriteLine("doTaskAwaitSync() completed");

			Console.ReadLine();
		}

		public class EntityParam : IDisposable
		{
			public string Name { get; set; }

			public void Dispose()
			{
                Console.WriteLine("Called Dispose of EntityParam");
            }
		}

		public class MyEntity
		{
            public delegate bool EventHandler(EntityParam param);

			public async Task<bool> callEventHandler(Func<EntityParam, bool> func)
			{
				using (var entity_param = new EntityParam())
				{
                    Console.WriteLine($"Current Name:{entity_param.Name}");

                    if (false == func.Invoke(entity_param))
                    {
                        Console.WriteLine($"Failed to EventHandler !!!");
                        return false;
                    }

                    Console.WriteLine($"Changed Name:{entity_param.Name}");
                }

                return true;
            }
        }

		static async Task async_await_with_delegate()
		{
            Func<EntityParam, bool> changeName = (param) =>
            {
				param.Name = "Good Man";

                return true;
			};

            var entity = new MyEntity();

			var is_success = await entity.callEventHandler(changeName);
			if(false == is_success)
			{
                Console.WriteLine($"Failed to callEventHandler !!!");
            }

            Console.ReadLine();
        }

        public static async void Test()
		{
			//Console.WriteLine($"AsyncAwait.Test() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

			//async_await_with_delegate().Wait();

			//TaskWait_vs_await().Wait();

			//Task_or_async_method();

			//async_await_transaction();

			//async_await_schedule();

			//async_await_IL_code();

            //await async_and_await_what();

            //Console.WriteLine($"AsyncAwait.Test() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
