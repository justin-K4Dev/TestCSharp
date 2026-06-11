using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


using CSharp;


namespace MultiThread
{
    public class Tasking
    {
		static void Run(object data)
		{
			Console.WriteLine(data == null ? "NULL" : data);
		}

		static void Run()
		{
			Console.WriteLine("Long running method");
		}

		static void Task_what()
        {
            /*
                📚 Task 클래스와 비동기 작업 실행

                  1. 개요
                    - Task / Task<T>는 .NET 4.0에서 도입된 비동기 작업 처리 클래스이다.
                    - ThreadPool의 스레드를 사용하여 작업을 비동기적으로 실행한다.
                    - TPL(Task Parallel Library)의 핵심 구성 요소이다.
                    - 다중 CPU 환경에서 병렬 작업이나 오래 걸리는 작업을 처리할 때 사용한다.

                  2. 기본 개념
                    - Task는 반환값이 없는 비동기 작업을 표현한다.
                    - Task<T>는 반환값이 있는 비동기 작업을 표현한다.
                    - Task.Factory.StartNew()를 사용하면 Task 생성과 동시에 실행된다.
                    - new Task(...) 생성자를 사용하면 Task 객체만 생성되고, Start() 호출 전까지 실행되지 않는다.
                    - new Task(...) 시점에는 아직 스레드가 생성되거나 할당되지 않는다.
                    - Start() 호출 시 TaskScheduler에 작업이 등록되며,
                      이후 ThreadPool 스레드가 작업을 가져가 실행한다.

                  3. 핵심 특징
                    - 내부적으로 ThreadPool을 사용한다.
                    - 직접 Thread를 생성하는 것보다 관리가 쉽고 효율적이다.
                    - StartNew()는 작업을 생성하자마자 실행한다.
                    - Task 생성자는 실행 시점을 직접 제어할 수 있다.
                    - Wait()를 호출하면 해당 Task가 종료될 때까지 현재 스레드가 대기한다.
                    - Task는 스레드를 직접 보유하지 않고,
                      실행 시점에 ThreadPool 스레드를 할당받아 사용한다.

                  4. 실행 흐름
                    - Task.Factory.StartNew(...) 호출
                    - Task 생성 및 즉시 실행 예약
                    - ThreadPool 작업 큐 등록
                    - ThreadPool 스레드 할당
                    - 작업 실행
                    - 작업 종료
                    - 사용한 스레드는 ThreadPool로 반환

                    - new Task(...) 호출
                    - Task 객체만 생성 (실행 안 됨)
                    - Start() 호출
                    - TaskScheduler에 작업 등록
                    - ThreadPool 작업 큐 등록
                    - ThreadPool 스레드 할당
                    - 작업 실행
                    - 작업 완료
                    - 사용한 스레드는 ThreadPool로 반환
                    - Wait() 호출 시 작업 완료까지 대기

                  5. 대표 메서드 또는 주요 코드
                    - Task.Factory.StartNew(new Action<object>(Run), null)
                      : object 매개변수를 받는 Run 메서드를 비동기 실행한다.

                    - Task.Factory.StartNew(new Action<object>(Run), "1st")
                      : "1st" 문자열을 Run 메서드의 인자로 전달하여 실행한다.

                    - Task.Factory.StartNew(Run, "2nd")
                      : 메서드 그룹 방식으로 Run 메서드를 실행한다.

                    - new Task(new Action(Run))
                      : Run 메서드를 실행할 Task 객체만 생성한다.
                        이 시점에는 아직 실행되지 않으며 스레드도 할당되지 않는다.

                    - new Task(() => { ... })
                      : 람다식을 실행할 Task 객체를 생성한다.

                    - Start()
                      : Task를 TaskScheduler에 등록하여 실행을 시작한다.
                        이후 ThreadPool 스레드가 작업을 가져가 실행한다.

                    - Wait()
                      : Task가 완료될 때까지 현재 스레드를 대기시킨다.

                  6. 멀티 스레드 환경에서 작동 특징
                    - Task는 기본적으로 ThreadPool의 작업 큐에 등록된다.
                    - ThreadPool은 사용 가능한 스레드를 선택하여 작업을 실행한다.
                    - 여러 Task는 동시에 실행될 수 있으므로 실행 순서는 보장되지 않는다.
                    - Console.WriteLine 같은 출력 작업도 여러 스레드에서 호출될 수 있다.
                    - Wait()를 사용하면 호출한 스레드는 해당 Task 완료 전까지 블로킹된다.
                    - Task 객체 생성만으로는 스레드가 점유되지 않는다.
                    - 실제 스레드 사용은 Start() 이후 스케줄링 시점에 결정된다.

                    - 실제 스레드 할당 시점
                      : ThreadPool이 작업 큐에서 Task를 가져가 실행하기 직전

                    - 실제 스레드 반환 시점
                      : Task 실행이 종료된 직후
                        사용된 스레드는 제거되지 않고 ThreadPool 내부로 반환되어 재사용된다.

                  7. 주의점
                    - Task 실행 순서는 코드 작성 순서와 다를 수 있다.
                    - Wait() 사용 시 현재 스레드가 멈추므로 UI 스레드에서는 주의해야 한다.
                    - Task 내부에서 발생한 예외는 Wait() 또는 Result 접근 시 AggregateException으로 전달될 수 있다.
                    - 공유 자원에 접근하는 경우 lock 등 동기화 처리가 필요하다.
                    - StartNew()는 상황에 따라 Task.Run()보다 의도가 불명확할 수 있으므로
			          단순 비동기 실행에는 Task.Run() 사용도 고려할 수 있다.
                    - Task는 논리적인 작업 단위이며,
                      실제 물리 스레드는 ThreadPool이 관리한다.

                  8. 예상 결과
                    - null이 전달된 경우 "NULL"이 출력된다.
                    - "1st", "2nd"가 각각 출력된다.
                    - 매개변수가 없는 Run 메서드는 "Long running method"를 출력한다.
                    - 람다식 Task는 "Long query"를 출력한다.
                    - 여러 Task가 병렬로 실행되므로 출력 순서는 매번 달라질 수 있다.


                📚 Task 전체 처리 흐름도
				
				  1. 주요 처리 절차

					new Task(...) 또는 Task.Run(...)
					→ TaskScheduler 등록
					→ ThreadPool 작업 큐 등록
					→ Worker Thread가 Task 획득
					→ 설정된 메서드 실행

					→ 일반 동기 메서드
					   또는
					→ async 메서드

					→ await 사용 여부 확인

					   ├─ await 사용
					   │   → await 대상 Task 상태 확인
					   │   → 완료됨 : 즉시 계속 실행
					   │   → 실행 중 : StateMachine 저장 후 일시 중단
					   │             → Continuation 등록
					   │             → await 대상 Task 완료
					   │             → Context 복귀 또는 ThreadPool 재개
					   │             → StateMachine 복원
					   │             → await 이후 코드 실행
					   │
					   └─ await 없이 async 호출
						   → Task만 반환받음
						   → 현재 메서드는 계속 실행
						   → 호출한 Task는 독립적으로 실행

					→ async 메서드 또는 동기 메서드 종료
					→ Task 상태 완료
					   (RanToCompletion / Faulted / Canceled)

					→ Thread 반환 및 재사용
					→ Task 객체 유지
					→ 참조가 없어지면 GC 대상
				
				  2. 전체 처리 절차 도식화 

				    ┌─────────────────┐	┌──────────────────┐
					│ [01] new Task(...)               │	│ [02] Task.Run(...)                 │
					│                                  │	│                                    │
					│ - Task 객체만 생성               │	│ - Task 객체 생성                   │
					│ - 상태: Created                  │	│ - 즉시 실행 예약                   │
					│ - 아직 실행 안 됨                │	│ - 기본 TaskScheduler 사용		  │
					│                                  │	│ - 상태: WaitingToRun 또는 Running  │
					└───────┬─────────┘  └───────┬──────────┘
									│										│
									│ [03] task.Start()                    │
									▼										│
						┌──────────────────────┐	│
						│ [04] TaskScheduler 등록                    │ ◄─┘
						│ 실행 예약 상태                             │
						└─────┬────────────────┘	
									│										
									▼										
						┌──────────────────────┐	
						│ [05] ThreadPool 작업 큐 등록               │ 
						└─────┬────────────────┘	
									│
									│ Worker Thread 대기
									▼
						┌────────────────────────┐
						│ [06] Worker Thread가 Task 획득				  │
						└─────┬──────────────────┘
									│
									▼
						┌──────────────────────┐
						│ [07] Queue에서 Task 제거                   │
						│ 실제 실행 시작                             │
						│ Thread 할당 시점                           │
						└─────┬────────────────┘
									│
									▼
						┌──────────────────────┐
						│ [08] 설정된 메서드 / Lambda 실행           │
						│ 작업 수행                                  │
						└─────┬────────────────┘
									│
			                        │ 			        
					    ┌─────┴──────────────────┐
                        │                                                │
                        ▼                                                ▼
					┌───────────────┐		┌───────────────┐
					│ [09] 일반 동기 메서드        │		│ [10] async 메서드            │──────────────────┐
					└─┬─────────────┘		└──────┬────────┘									  │		
						│												  │													  │
						│												  ▼													  ▼
						│									┌─────────────────┐                  ┌─────────────────┐
						│									│ [11] await 사용 호출             │ 				    │ [24] await 없이 async 함수 호출	│
						│									└──────┬──────────┘					└──────┬──────────┘
						│												  │													  │	
						│												  ▼													  ▼
						│									┌─────────────────┐                  ┌─────────────────┐
						│									│ [12] await SomeTaskAsync() 도달  │ 					│ [25] Task 객체만 반환받음	    │
						│									└──────┬──────────┘					│ 현재 메서드는 대기 안함			│
						│												  │										└──────┬──────────┘
						│											      ▼													  ▼
						│									┌─────────────────┐					┌─────────────────┐
						│									│ [13] await 대상 Task 상태 확인   │					│ [26] 다음 코드 즉시 실행			│
						│									└──────┬──────────┘					│ fire-and-forget 상태				│
						│												  │										└──────┬──────────┘
						│								┌────────┴────────┐									  ▼
						│								│								    │						┌─────────────────────┐
						│								▼								    ▼						│ [27] 호출한 Task가 계속 실행되는 동안	│
						│				┌─────────────┐		┌──────────────┐		│ 현재 async 메서드는 먼저 완료 가능		│
						│				│ [14] 이미 완료됨			│		│ [15] 아직 실행 중          │		└──────────┬──────────┘
						│				└───┬─────────┘		└───┬──────────┘							  ▼
						│						│									│								┌──────────────────────────┐
						│						│ 즉시 계속 실행					│ continuation 등록			│ [28] 예외가 현재 try/catch에서 잡히지 않을 수 있음 │
						│						│									▼								└──────────┬───────────────┘
            			│						│							┌──────────────────┐					  │
            			│						│							│ [16] 현재 async 메서드 일시 중단	  │					  │
            			│						│							│ 상태(StateMachine) 저장			  │					  │
            			│						│							│ 지역 변수 / 실행(진행) 위치 저장   │					  │
            			│						│							│ 현재 Thread 반환 가능			  │					  │
            			│						│							└───┬──────────────┘					  │
						│						│									│													  │
						│						│									▼													  │
            			│						│							┌───────────────┐							  │
            			│						│							│ [17] await 대상 Task 완료	│							  │
			            │						│							│ 상태: 완료 / 예외 / 취소     │							  │
            			│						│							└───┬───────────┘							  │
						│						│									│													  │
						│						│									▼													  │
						│						│							┌─────────────────┐						  │
						│						│							│ [18] await 이후 코드 실행 예약	│						  │
						│						│							│ (Continuation Scheduling)		│						  │
						│						│							└───┬─────────────┘						  │
						│						│									│													  │
						│						│					┌───────┴──────────┐							  │
						│						│					▼									  ▼							  │
						│						│		┌─────────────────┐	┌───────────────┐		  │
						│						│		│ [19] SynchronizationContext 복귀 │	│ [20] ThreadPool Thread 재개	│		  │
						│						│		│								    │	│ 								│		  │
						│						│		│ - 원래 Context 복귀			    │	│ - 아무 Worker Thread 사용	│		  │
						│						│		│ - UI/Main Thread 유지		    │	│ - Context 복귀 없음			│		  │
						│						│		│ - Context가 있으면 기본 복귀     │	│ - ConfigureAwait(false) 주로 │		  │
						│						│	    └─────┬───────────┘	└────┬──────────┘		  │
						│						│					└───────┬──────────┘							  │
						│						│									▼													  │
						│						│					┌─────────────────┐								  │
						│						│					│ [21] async StateMachine 복원		│								  │
						│						│					│ await 시점 상태 복구				│								  │
						│						│					│ 지역 변수 / 실행 위치 복원		│								  │
						│						│					└───────┬─────────┘								  │
						│						└────────────┬────┘												      │
						│									 			  ▼															  │
						│									┌────────────────┐										  │
			 			│									│ [22] await 이후 코드 재개	  │										  │
						│									│ 메서드 처음부터 아님			  │										  │
			  			│									│ try/catch 예외 전달 가능		  │										  │
						│									└──────┬─────────┘										  │
						│												  ▼															  │
						│									┌────────────────┐										  │
			 			│									│ [29] async 메서드 계속 실행	  │ ◄────────────────────┘
			  			│									│ 다음 await가 있으면 동일 반복  │										  
						│									└──────┬─────────┘
						│												  │
						▼											      ▼
					┌──────────────┐		┌────────────────┐
					│[30] 동기 메서드 끝까지 실행│		│ [31] async 메서드 끝까지 실행  │
					│중간 중단 없음 			  │		│ 모든 await 처리 후 최종 완료	  │
					└───────┬──────┘  		└──────┬─────────┘
									└────────┬─────────┘
													  ▼
										┌───────────────┐
										│ [32] 작업 완료               │
										│ 상태 변경                    │
										│ - RanToCompletion            │
										│ - Faulted                    │
										│ - Canceled                   │
										└──────┬────────┘
													  ▼
										┌──────────────────────────┐
										│ [33] Thread 반환 / 재사용						  │
										│ - 동기 메서드: 작업 종료 후 반환					  │
										│ - async 메서드: await 중단시 이미 반환될 수 있음   │
			                            │ - await 이후 재개 Thread도 작업 종료 후 반환		  │
										└──────┬───────────────────┘
													  ▼
										┌───────────────────────────────┐
										│ [34] Task 객체 유지											│
										│ - 완료 후에도 참조가 있으면 유지								│
										│ - 참조가 없으면 GC 대상										│
										│ - Scheduler/Queue에서 제거되었다고 즉시 제거되는 것은 아님	│
										└───────────────────────────────┘


					[ 구성 요소 역할 ]

						Task
							- 비동기 작업(작업 단위)을 표현하는 객체
							- 실행할 메서드, 상태(Status), 결과(Result), 예외(Exception) 등을 관리
							- 실제 코드를 실행하는 주체가 아님
							- Worker Thread를 직접 소유하지 않음

						TaskScheduler
							- Task 실행을 예약(Scheduling)하는 구성 요소
							- Task를 어떤 방식으로 실행할지 결정
							- 기본 Scheduler는 ThreadPool 기반으로 동작
							- Task를 ThreadPool Queue에 등록하여 실행을 요청

						ThreadPool
							- Worker Thread를 생성 및 관리
							- Worker Thread를 재사용하여 Thread 생성 비용 감소
							- 실행 대기 중인 작업 Queue를 관리
							- 사용 가능한 Worker Thread를 Task 실행에 할당

						Worker Thread
							- 실제 사용자 코드를 실행하는 Thread
							- Task를 실행하기 위해 ThreadPool에서 제공됨
							- 작업 완료 후 제거되지 않고 ThreadPool로 반환되어 재사용됨


					[ 핵심 처리 흐름 요약 ]

						new Task(...)
							= Task 객체만 생성
							= 상태 : Created
							= 아직 실행되지 않음
							= TaskScheduler에 등록되지 않음
							= ThreadPool Queue에 등록되지 않음
							= Worker Thread 할당 없음

						task.Start()
							= TaskScheduler에 실행 요청
							= TaskScheduler가 실행 예약
							= ThreadPool Queue 등록
							= 상태 : WaitingToRun

						Task.Run(...)
							= Task 객체 생성
							= 즉시 TaskScheduler에 실행 요청
							= ThreadPool Queue 등록
							= 상태 : WaitingToRun 또는 Running
							= 별도의 Start() 호출 불필요

						Worker Thread가 Task 획득
							= ThreadPool Queue에서 Task 가져옴
							= Queue에서 Task 제거
							= Worker Thread 할당
							= 상태 : Running
							= 사용자 코드 실행 시작

						동기 메서드 실행
							= Worker Thread가 메서드를 처음부터 끝까지 실행
							= 중간 중단 없음
							= 작업 완료 시 Task 종료

						async 메서드 실행
							= await 이전까지는 일반 메서드처럼 실행

							await 대상 Task가 이미 완료된 경우
								→ 즉시 다음 코드 실행

							await 대상 Task가 아직 실행 중인 경우
								→ 현재 async 메서드 일시 중단
								→ StateMachine 상태 저장
								→ 현재 Thread 반환 가능
								→ await 대상 Task 완료 대기

							await 대상 Task 완료 후
								→ Continuation 실행 예약
								→ Context 복귀 또는 ThreadPool 재개
								→ StateMachine 복원
								→ await 이후 코드부터 실행 재개

						await 없이 async 메서드 호출
							= Task 객체만 반환받음
							= 현재 메서드는 대기하지 않음
							= 다음 코드 즉시 실행

							호출한 async 메서드는
							독립적으로 계속 실행됨

							(Fire-and-Forget 형태)

						Task 실행 완료
							= 상태 변경

							RanToCompletion
								정상 완료

							Faulted
								예외 발생

							Canceled
								취소됨

						Thread 반환
							= Worker Thread가 제거되는 것이 아님
							= ThreadPool 내부로 복귀
							= 이후 다른 Task 실행에 재사용
			
						Task 객체 수명							
							= Task 완료와 객체 제거는 별개

							Task 완료 후에도
							참조가 남아 있으면 계속 유지

							Scheduler 또는 Queue에서 제거되었다고
							즉시 메모리에서 제거되는 것은 아님

							참조가 없어지면
							GC 대상이 되어 정리됨


					📚 한 줄 요약

						Task 생성
						→ TaskScheduler 등록
						→ ThreadPool Queue 등록
						→ Worker Thread 할당
						→ 사용자 코드 실행
						→ (필요 시 await 일시 중단 및 재개)
						→ Task 완료
						→ ThreadPool로 Thread 반환
						→ Task 객체는 참조가 없어질 때 GC 대상
            */

            //-------------------------------------------------------------------------------------
            // Task.Factory.StartNew()
            //   - Task 생성과 동시에 실행
            //-------------------------------------------------------------------------------------
            {
                // object 파라미터 전달
                Task.Factory.StartNew(new Action<object>(Run), null);

                // 문자열 파라미터 전달
                Task.Factory.StartNew(new Action<object>(Run), "1st");

                // 메서드 그룹 방식 사용
                Task.Factory.StartNew(Run, "2nd");

                Console.ReadLine();
            }

            //-------------------------------------------------------------------------------------
            // Task 생성자를 이용한 방식
            //   - Task 객체만 생성
            //   - Start() 호출시 실행
            //-------------------------------------------------------------------------------------
            {
                // Task 생성자에 람다(Run)를 지정하여 Task 객체 생성
                var t1 = new Task(new Action(Run));

                // 람다식을 이용 Task 객체 생성
                var t2 = new Task(() =>
                {
                    Console.WriteLine("Long query");
                });

                // Task 쓰레드 시작
                t1.Start();
                t2.Start();

                // Task가 끝날 때까지 대기
                t1.Wait();
                t2.Wait();

                Console.ReadLine();
            }
		}

		static void Task_with_attrib_check()
		{
			// Task는 쓰레드 풀을 사용하는가?
			Console.WriteLine($"IsThreadPoolThread() => {System.Threading.Thread.CurrentThread.IsThreadPoolThread}");
			// Task는 백그라운드 스레드인가?
			Console.WriteLine($"IsBackground() => {System.Threading.Thread.CurrentThread.IsBackground}");

			Console.ReadLine();
		}

		static void doThread4TaskCreationOptions(object param)
		{
			var curr_tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
			Console.WriteLine($"called ThreadFunc !!! - TID:{curr_tid}");
		}

		static void Task_with_TaskCreationOptions()
		{
            /*
                📚 TaskCreationOptions 정리

                  1. 개요
                    - TaskCreationOptions는 Task 생성 시 실행 방식에 대한 힌트 또는 옵션을 지정한다.
                    - 대표 옵션:
                      - DenyChildAttach
                      - AttachedToParent
                      - LongRunning

                  2. DenyChildAttach
                    - 현재 Task에 자식 Task가 연결되는 것을 막는 옵션이다.
                    - 주의: 자식 Task에 DenyChildAttach를 지정하는 것이 아니라,
                      부모 Task에 지정되어야 자식 Task의 AttachedToParent 연결을 막는다.
                    - Task.Run(...)은 내부적으로 DenyChildAttach 성격을 가지므로,
                      그 안에서 AttachedToParent Task를 만들어도 부모에 연결되지 않을 수 있다.

                    정리:
                      - 부모 Task가 DenyChildAttach 상태
                        → 자식 Task는 AttachedToParent를 사용해도 부모에 붙지 않음
                        → 부모는 자식 종료를 기다리지 않음

                  3. AttachedToParent
                    - 자식 Task를 부모 Task에 연결하는 옵션이다.
                    - 부모 Task는 연결된 자식 Task가 모두 끝날 때까지 완료 상태가 되지 않는다.
                    - 즉, parentTask.Wait()는 부모 코드뿐 아니라 연결된 자식 Task 완료까지 기다린다.

                    정리:
                      - 자식 Task에 AttachedToParent 지정
                      - 부모 Task가 DenyChildAttach 상태가 아니어야 함
                      - 부모 Task는 자식 Task 완료까지 대기

                  4. LongRunning
                    - 오래 실행되는 작업임을 TaskScheduler에 알려주는 옵션이다.
                    - 기본 TaskScheduler에서는 보통 ThreadPool이 아닌 별도 Thread 생성을 유도한다.
                    - ThreadPool 스레드를 장시간 점유하지 않도록 할 때 사용한다.
                    - 단, 반드시 항상 새 Thread를 만든다고 보장하는 옵션은 아니며,
                      Scheduler 구현에 따라 동작이 달라질 수 있다.

                  5. 현재 코드 기준 주의점
                    - Task.Run(...)은 부모 Task에 DenyChildAttach가 적용된 형태로 동작한다.
                    - 따라서 Task.Run 내부에서 AttachedToParent를 사용해도
                      자식 Task가 부모에 붙지 않을 수 있다.
                    - AttachedToParent 동작을 확인하려면 Task.Factory.StartNew(...)를 사용하는 것이 더 명확하다.
                    - DenyChildAttach는 자식 Task에 지정하는 옵션이 아니라,
                      부모가 자식 연결을 거부하는 옵션으로 이해해야 한다.
                    - LongRunning Task 안의 while(true)는 Thread를 계속 점유하므로 테스트 목적 외에는 위험하다.

                  6. 예상 실행 결과

                    DenyChildAttach 또는 일반 자식 Task
                      - 부모 Task는 자식 Task 종료를 기다리지 않는다.
                      - parentTask.Wait() 이후 바로 "Main thread finished."가 출력될 수 있다.
                      - 자식 Task 출력은 나중에 출력될 수 있다.

                    AttachedToParent
                      - 부모 Task가 자식 Task 완료까지 기다린다.
                      - parentTask.Wait()는 자식 Task가 끝난 뒤 반환된다.
                      - "Child task finished !!!" 출력 후 "Main thread finished."가 출력된다.

                    LongRunning
                      - 장시간 실행 작업을 별도 Thread에서 실행하도록 유도한다.
                      - ThreadPool의 일반 Worker Thread 고갈을 줄이는 데 사용할 수 있다.
            */

            //-------------------------------------------------------------------------------------
            // TaskCreationOptions.DenyChildAttach
            //  : 부모 Task가 자식 Task의 Attach(연결)를 허용하지 않는 옵션
            //
            //  - 자식 Task가 AttachedToParent 옵션을 사용하더라도
            //    현재 부모 Task에는 연결되지 않는다.
            //  - 부모 Task는 자식 Task 종료를 기다리지 않는다.
            //  - parentTask.Wait()는 부모 Task 작업만 완료되면 반환된다.
            //  - 자식 Task는 독립적으로 계속 실행될 수 있다.
            //-------------------------------------------------------------------------------------
            {
                var parentTask = Task.Run(() =>
				{
					var childTask = new Task(() =>
					{
						System.Threading.Thread.Sleep(10000);
						Console.WriteLine("Child task finished !!!");

					}, TaskCreationOptions.DenyChildAttach);

					childTask.Start();

					Console.WriteLine("Parent task finished.");
				});

				parentTask.Wait();
				Console.WriteLine("Main thread finished.");

                /*
					Parent task finished.
					Main thread finished.
					Child task finished !!!

					- parentTask.Wait()는 childTask를 기다리지 않는다.
					- childTask는 부모 Task와 분리되어 실행된다.
					- 따라서 Main thread finished.가 먼저 출력될 수 있다.
				*/
            }

            //-------------------------------------------------------------------------------------
            // TaskCreationOptions.AttachedToParent
            //  : 현재 Task를 부모 Task에 연결(Attach)하는 옵션
            //
            //  - 부모 Task는 연결된 자식 Task가 모두 종료될 때까지 완료되지 않는다.
            //  - parentTask.Wait()는 자식 Task 종료까지 함께 대기한다.
            //  - 부모/자식 Task를 하나의 작업 그룹처럼 동작시키고 싶을 때 사용한다.
            //  - 단, 부모 Task가 DenyChildAttach 상태이면 연결되지 않는다.
            //-------------------------------------------------------------------------------------
            {
                var parentTask = Task.Factory.StartNew(() =>
                {
                    var childTask = new Task(() =>
                    {
                        System.Threading.Thread.Sleep(10000);
                        Console.WriteLine("Child task finished !!!");

                    }, TaskCreationOptions.AttachedToParent);

                    childTask.Start();

                    Console.WriteLine("Parent task finished.");
                });

                parentTask.Wait();
				Console.WriteLine("Main thread finished.");

                /*
					Parent task finished.
					Child task finished !!!
					Main thread finished.

					- childTask는 AttachedToParent 옵션으로 생성되었다.
					- 따라서 childTask는 parentTask에 연결(Attach)된다.
					- parentTask는 childTask가 종료될 때까지 완료 상태가 되지 않는다.
					- 결과적으로 parentTask.Wait()는 childTask 종료까지 대기한다.
				*/
            }

            //-------------------------------------------------------------------------------------
            // TaskCreationOptions.LongRunning
            //  : 오래 실행되는 작업(Long Running Task)임을 Scheduler에 알리는 옵션
            //
            //  - 기본 ThreadPool Worker Thread를 장시간 점유하지 않도록 하기 위해 사용한다.
            //  - 일반적으로 별도의 전용 Thread 생성을 유도한다.
            //  - ThreadPool Queue 기반 실행 대신 독립 Thread 방식으로 동작할 수 있다.
            //  - ThreadPool 최대 Worker Thread 수와 별도로 Thread가 생성될 수 있다.
            //  - 장시간 실행 작업, 무한 루프, 지속 실행 서비스 등에 사용한다.
            //
            //  ※ 주의
            //    - 반드시 항상 새 Thread를 생성한다고 보장되지는 않는다.
            //    - 실제 동작은 TaskScheduler 구현에 따라 달라질 수 있다.
            //    - 과도하게 사용하면 Thread 생성 비용 및 Context Switching 비용이 증가할 수 있다.
            //-------------------------------------------------------------------------------------
            {
                System.Threading.ThreadPool.SetMinThreads(1, 1);
				System.Threading.ThreadPool.SetMaxThreads(5, 1);

				for(var i = 0; i < 5; i++)
				{
					var t = new System.Threading.Tasks.Task(() =>
					{
						var curr_tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
						Console.WriteLine($"called ChildTask !!! - TID:{curr_tid}");

						while (true) {
						}

					}, TaskCreationOptions.LongRunning); // 스레드 생성시 TaskCreationOptions.LongRunning 설정 한다.

					t.Start();

					Console.WriteLine($"CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");
				}

				System.Threading.ThreadPool.QueueUserWorkItem(doThread4TaskCreationOptions); // 스레드를 생성하고 새로 추가 한다. (1개추가)
				System.Threading.ThreadPool.QueueUserWorkItem(doThread4TaskCreationOptions); // 스레드를 생성하고 새로 추가 한다. (1개추가)

                var added_task_1 = Task.Factory.StartNew( doThread4TaskCreationOptions
				  						                , TaskCreationOptions.LongRunning ); // 스레드를 생성하고 새로 추가 한다. (1개추가)

                var added_task_2 = Task.Factory.StartNew(doThread4TaskCreationOptions
													    , TaskCreationOptions.LongRunning ); // 위에 생성했던 스레드가 다시 호출될 수 있다.

                /*
					called ChildTask !!! - TID:7
					CurrThreadCount:18
					called ChildTask !!! - TID:8
					CurrThreadCount:19
					called ChildTask !!! - TID:9
					CurrThreadCount:20
					called ChildTask !!! - TID:10
					CurrThreadCount:21
					called ChildTask !!! - TID:11
					CurrThreadCount:22
					called ThreadFunc !!! - TID:4
					called ThreadFunc !!! - TID:3
					called ThreadFunc !!! - TID:5
					called ThreadFunc !!! - TID:4 <= 이미 생성한 TID:4 스레드에 의해 함수가 호출되었다 !!!
				*/
            }

            Console.ReadLine();
		}

		static async Task startEventTimer(Int32 intervalMS)
		{
			await Task.Factory.StartNew(async () =>
			{
				while (true)
				{
					await Task.Delay(intervalMS);

					while (true)
					{
						var curr_tick = Environment.TickCount;
						if ((curr_tick % 10) == 0)
						{
							break;
						}
					}
				}
			}, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
		}

		static async Task onStartEventTimer(Int32 delayMS, Int32 intervalMS)
		{
			await Task.Delay(delayMS);

			await startEventTimer(intervalMS);
		}

		static void Task_StartNew_with_ThreadPool()
		{
			Console.WriteLine($"Start ThreadPool Check - CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");

			for (int i = 0; i < 2000; i++)
			{
				onStartEventTimer(100, 5000);

				Console.WriteLine($"LoopCount:{i}, CurrThreadCount:{Process.GetCurrentProcess().Threads.Count}");
			}

			Console.ReadLine();
		}

		static async void Task_StartNew_vs_Run()
		{
			/*
				Task.Factory.StartNew( ...
			                         , CancellationToken.None
								     , TaskCreationOptions.DenyChildAttach
				                     , TtaskScheduler.Default )
				와
				Task.Run() 은 동일한 기능으로 작동 한다 !!!				
			*/
			{
				var t_new = Task.Factory.StartNew( (object obj) => doThread4TaskCreationOptions(obj)
												 , null
												 , CancellationToken.None
												 , TaskCreationOptions.DenyChildAttach
												 , System.Threading.Tasks.TaskScheduler.Default);

				var t_run = Task.Run(() => doThread4TaskCreationOptions(null));
			}

			/*
				아래와 같이 Task<TResult> 반환시 return_value 는 Task<Int32> 로 반환 된다.
				이렇게 복잡한 반환 타입 처리를 위해 Unwrap() 함수를 제공 하며
				return_value 은 Int32 로 반환 된다.
			*/
			{
				var t = Task.Factory.StartNew(async delegate
				{
					await Task.Delay(1000);
					return 42;
				});

				var return_value = t.Result; // Task<Int32> 반환
			}
			{
				var t = Task.Factory.StartNew(async delegate
				{
					await Task.Delay(1000);
					return 42;

				}).Unwrap();

				var return_value = t.Result; // Int32 반환
			}
			/*
				위의 코드를 Task.Run() 으로 구현 한다면 return_value 은 Int32 로 반환 되고,
				Task.Factory.StartNew() 로 동일하게 구현할 경우
				코드가 복잡하다 !!!
			*/
			{
				var t = Task.Run(async delegate
				{
					await Task.Delay(1000);
					return 42;
				});

				var return_value = t.Result; // Int32 반환
			}
			{
				var t = Task.Factory.StartNew(async delegate
				{
					await Task.Delay(1000);
					return 42;

				}, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default).Unwrap();

				var return_value = t.Result; // Int32 반환
			}
			/*
				위의 코드를 await 하기 원한다면 간단하게 수정 가능하다.
			*/
			{
				Int32 result = await Task.Run(async () =>
				{
					await Task.Delay(1000);
					return 42;
				});
			}
			/*
				위의 코드를 Task.Factory.StartNew 로 수정 한다면 마찬가지로 Unwrap() 함수를 이용해야 한다.
			*/
			{
				Int32 result = await Task.Factory.StartNew(async delegate
				{
					await Task.Delay(1000);
					return 42;

				}, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default).Unwrap();
			}
			/*
				Unwrap() 함수를 이용하고 싶지 않다면 이상한 코드로 수정을 해야 한다.
				await await 를 두번 작성해야 한다.
			*/
			{
				Int32 result = await await Task.Factory.StartNew(async delegate
				{
					await Task.Delay(1000);
					return 42;

				}, CancellationToken.None, TaskCreationOptions.DenyChildAttach, System.Threading.Tasks.TaskScheduler.Default);
			}

			Console.ReadLine();
		}

		static void Task_with_not_use_ThreadPool()
		{
			/*
				ThreaPool 을 사용 하지 않고 Task 생성 하기
			*/
			{
				var t = new Task(() => {
					var curr_tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
					if (false == System.Threading.Thread.CurrentThread.IsThreadPoolThread)
					{
						Console.WriteLine($"Not found Task in ThreadPool !!! : TaskTID:{curr_tid}, CurrThread");
					}

					System.Threading.Thread.Sleep(1000);
				}, TaskCreationOptions.LongRunning);

				t.Start();

				t.Wait();
			}

			Console.ReadLine();
		}

		static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			Console.WriteLine("Unobserved exception !!!");

			foreach (var inner in e.Exception.Flatten().InnerExceptions)
			{
				Console.WriteLine(inner.Message);
				Console.WriteLine("Type : {0}", inner.GetType());
			}
			e.SetObserved();
		}

		static void Task_with_exception()
		{
            /*
				Task 내부에서 예외가 발생하면, 예외는 즉시 호출 스레드로 전달되지 않고,
				Task 객체 내부에 캡처되어 저장된다.
				이 Task는 Faulted 상태가 된다.

			
				미처리 예외
				: 아래의 상황에서 예외가 발생하면 확인할 수 없다.
				  Task.Wait() 메소드를 호출 하지 않는다.
				  Task.Result 프로퍼티를 호출 하지 않는다.
				  Task.Exception 프로퍼티를 호출하지 않는다.
				  그러나 방법은 있다.
			      Task 인스턴스가 GC 에 의해 회수될 때 Task.Finalize() 메소드는 자신의 예외가 확인되고 있는지 확인한다.
				  확인되지 않았다고 판단된 경우 Finalize 메소드에서 AggregateException 가 Throw 된다.
				  Finalize 메소드는 CLR 전용 스레드인 Finalizer 스레드 상에서 실행되기 때문에
				  그 예외는 포착할수 없고 프로세스가 즉각 종료된다.
			*/
            {
                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
				
				Task.Factory.StartNew(() => 
				{ 
					throw new InvalidOperationException("Task1"); 
				});
				Task.Factory.StartNew(() => 
				{ 
					throw new InvalidCastException("Task2"); 
				});

				System.Threading.Thread.Sleep(500); //--- 태스크 종류를 대기
				
				GC.Collect();                     //--- Task 인스턴스를 회수
				GC.WaitForPendingFinalizers();    //--- Finalize() 를 강제적으로 호출한다

				System.Threading.Thread.Sleep(5000);

				Console.WriteLine("Done");

				Console.ReadLine();
			}

			/*
				자식 Task 에서 발생한 예외 포착
				: Flatten 메소드 사용
			*/
			{
				var task = Task.Factory.StartNew(() =>
				{
					Task.Factory.StartNew(() =>
					{
						throw new Exception("Task2 : Exception");
					}, TaskCreationOptions.AttachedToParent);
					throw new InvalidOperationException("Task1 : Exception");
				});

				try
				{
					task.Wait();
				}
				catch (AggregateException exception)
				{
					foreach (var inner in exception.Flatten().InnerExceptions)
					{
						Console.WriteLine(inner.Message);
						Console.WriteLine("Type : {0}", inner.GetType());
					}
				}

				Console.ReadLine();
			}

			/*
				예외 포착
				: Task 중에서 처리되지 않은 예외는 Task 자신이 포착하여 컬렉션으로 보존한다.
				  Wait 혹은 Result 프로퍼티가 실행되면 이 멤버들에서 System.AggregateException가 Throw 된다.
				  Task 가 포착한 예외는 스로우된 AggregateException의 InnerExceptions Property 에서 취득할 수 있다. 
			*/
			{
				var task = Task.Factory.StartNew(() =>
				{
					// 예외 발생 !!!
					throw new Exception("Test Exception");
				});

				try
				{
					task.Wait();
				}
				// 예외 포착
				catch (AggregateException exception)
				{
					foreach (var inner in exception.InnerExceptions)
					{
						Console.WriteLine(inner.Message);
					}
				}

				Console.ReadLine();
			}
		}

		static int CalcSize(string data)
		{
			string s = data == null ? "" : data.ToString();
			// 복잡한 계산 가정

			return s.Length;
		}

		static void Task_generic_use()
		{
			/*
                Non-Generic 타입인 Task 클래스는 ThreadPool.QueueUserWorkItem()과 같이 리턴값을 쉽게 돌려 받지 못한다.
                비동기 델리게이트(Asynchronous Delegate)와 같이 리턴값을 돌려 받기 위해서는 Task<T> 클래스를 사용한다.
                Task<T> 클래스의 T는 리턴 타입을 가리키는 것으로 리턴값은 Task객체 생성 후 Result 속성을 참조해서 얻게 된다.
                Result 속성을 참조할 때 만약 작업 쓰레드가 계속 실행 중이면,
                결과가 나올 때까지 해당 쓰레드를 기다리게 된다. 
            */
			{
				// Task<T>를 이용하여 쓰레드 생성과 시작
				var task = Task<Int32>.Factory.StartNew(() => CalcSize("Hello World"));

				// 메인쓰레드에서 다른 작업 실행
				Task.Delay(1000);

				// 쓰레드 결과 리턴. 쓰레드가 계속 실행중이면
				// 이곳에서 끝날 때까지 대기함
				int result = task.Result;

				Console.WriteLine("Result={0}", result);
			}
		}

		static void doSync()
		{
			System.Threading.Thread.Sleep(1000);
		}

		static Task doAsyncByTask()
		{
			// Task 를 이용해서 논블로킹을 처리 할 수 있다.
			Task t = new Task(() => System.Threading.Thread.Sleep(1000));
			t.Start();

			return t;
		}

		static Task doAsyncByTaskCompletionSource()
		{
			// TaskCompletionSource 를 이용해서 변칙적으로 논블로킹을 처리 할 수 있다.
			TaskCompletionSource<object> src = new TaskCompletionSource<object>();

			try
			{
				Console.WriteLine("doAsyncByTaskCompletionSource called !!!");
				src.SetResult(null);
			}
			catch (Exception e)
			{
				src.SetException(e);
			}

			return src.Task;
		}

		class Factorial
		{
			private Func<Int64, Int64> m_func;

			public Factorial()
			{
				m_func = this.CalculateFactorial; 
			}

			public Int64 CalculateFactorial(Int64 p)
			{
				if (p <= 0)
				{
					return -1;
				}

				try {
					Int64 n = 1;

					for (var i = 1; i <= p; i++)
					{
						n = n * i;

						System.Threading.Thread.Sleep(100);
					}

					return n;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);

					return -1;
				}
			}

			public IAsyncResult BeginCalculateFactorial(Int64 p, AsyncCallback asyncCallback, object state)
			{
				var param = new object[] { state, p };

				return m_func.BeginInvoke(p, asyncCallback, param);

			}

			public Int64 EndCalculateFactorial(IAsyncResult asyncResult)
			{
				return m_func.EndInvoke(asyncResult);
			}
		}

		static void CalculateDone(IAsyncResult asyncResult)
		{
			var param = asyncResult.AsyncState as object[];

			if (param == null)
			{
				return;
			}

			var factorial = param[0] as Factorial;

			var input = (int)param[1];
			var result = factorial.EndCalculateFactorial(asyncResult);

		}


		static bool m_is_read_completed = false;

		static Task<int> ReadFileAsync(string filePath)
		{
			System.IO.FileStream fileStream = System.IO.File.OpenRead(filePath);

			byte[] bufferByteArray = new Byte[fileStream.Length];

			var task = Task<Int32>.Factory.FromAsync(
				(Func<byte[], int, int, AsyncCallback, object, IAsyncResult>)fileStream.BeginRead
			,	(Func<IAsyncResult, int>)fileStream.EndRead
			,	bufferByteArray
			,	0
			,	(int)fileStream.Length
			,	null
			);
			
			task.ContinueWith(
				_ =>
				{
					if (task.Status == TaskStatus.RanToCompletion)
					{
						m_is_read_completed = true;

						Console.WriteLine("파일명 : {0}, 바이트 수 : {1}", filePath, task.Result);
					}

					fileStream.Dispose();
				}
			);

			return task;
		}

		static Task doAsyncByFromAsync()
		{
			Task task = null;

			{
				var input = 10;

				var factorial = new Factorial();

				Func<Int64, AsyncCallback, object, IAsyncResult> begin = factorial.BeginCalculateFactorial;

				Func<IAsyncResult, Int64> end = factorial.EndCalculateFactorial;

				var t = Task.Factory.FromAsync(begin, end, input, factorial);

				t.Wait();

				task = t;
			}

			{
				var t = ReadFileAsync("to_load_file_path");

				int i = 0;

				do
				{
					Console.WriteLine("타이머 카운터 : {0}", ++i);
				}
				while (false == m_is_read_completed);

				t.Wait();

				task = t;
			}

			return task;
		}

		static async Task Task_with_awaitable()
		{
			/*
				비동기 프로그래밍 모델
			
				* APM(Asynchronouse Programming Model)
				* EAP(Event Based Asynchronouse programming Pattern)
				* TAP(Task Based Asynchronouse programming Pattern)
			*/

			doSync();

			await doAsyncByTask();

			await doAsyncByTaskCompletionSource();

			await doAsyncByFromAsync();

			Console.ReadLine();
		}

		static void Task_with_Wait()
		{
			/*
                해당 Task 의 실행이 완료될 때까지 기다린다.
            */
			{
				// Task<T>를 이용하여 쓰레드 생성과 시작
				var task = Task<Int32>.Factory.StartNew(() => CalcSize("Hello World"));
				task.Wait(); // task 가 처리 완료될때 까지 기다린다. 호출자의 thread 는 대기하게 된다.

				Console.ReadLine();
			}
		}

		static void Task_with_WaitAny()
		{
			/*
				복수의 Task 중 어느 하나가 완료될 때까지만 대기하고 싶은 경우는 WaitAny() 를 사용한다.
				Timeout 설정이 가능하고, Timeout 이 발생한 경우 -1을 반환한다.
			*/
			{
				Console.WriteLine("Start");
				var tasks = new[]
				{
					Task.Factory.StartNew(() => Console.WriteLine("Task1 is running")),
					Task.Factory.StartNew(() => Console.WriteLine("Task2 is running")),
					Task.Factory.StartNew(() => Console.WriteLine("Task3 is running")),
				};

				int index = Task.WaitAny(tasks);
				Console.WriteLine("Index = {0}", index);

				Console.WriteLine("End");
			}


			Console.ReadLine();
		}


		private static void writeLog(string text)
		{
			Console.WriteLine($"{text} - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
		}

		static void Task_with_ContinueWith()
		{
            /*
                📚 ContinueWith 개요

                ContinueWith는 어떤 Task가 완료된 후 실행할 후속 작업을 등록하는 메서드이다.

                    task.ContinueWith(t =>
                    {
                        // task 완료 후 실행
                    });

                await와 개념적으로 비슷하게 볼 수 있지만 완전히 같지는 않다.

                    await task;
                    다음 코드 실행

                위 코드는 개념적으로는 다음과 비슷하다.

                    task.ContinueWith(t =>
                    {
                        다음 코드 실행
                    });

                하지만 await는 SynchronizationContext를 캡처하여,
                UI 애플리케이션에서는 기본적으로 await 이후 코드를 UI 스레드에서 계속 실행한다.

                ContinueWith에서 UI 스레드로 돌아오려면 명시적으로 Scheduler를 지정해야 한다.

                    task.ContinueWith(t =>
                    {
                        label1.Text = "완료";
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                주의할 점

                1. ContinueWith는 원본 Task가 완료된 후 실행된다.

                2. 여러 개의 ContinueWith를 같은 Task에 등록할 수 있다.

                3. 같은 Task에 여러 ContinueWith를 등록해도 실행 순서는 보장되지 않는다.

                4. 기본 ContinueWith는 보통 ThreadPool에서 실행된다.

                5. Task.Delay(...)만 호출하면 대기하지 않는다.
                   반드시 await, Wait(), Result 등을 사용해야 실제로 기다린다.

                6. Thread.Sleep(...)은 현재 스레드를 블로킹한다.
            */

            {
                writeLog("=== 예제 1: 하나의 Task에 여러 ContinueWith 등록 ===");

                Task<int> task = Task.Run(async () =>
                {
                    writeLog("TEST-0");

                    await Task.Delay(5000).ConfigureAwait(false);

                    return 1;
                });

                task.ContinueWith(t =>
                {
                    writeLog("TEST-1");
                });

                task.ContinueWith(t =>
                {
                    writeLog("TEST-2");
                });

                /*
                    TEST-1, TEST-2는 모두 task가 완료된 후 실행된다.
                    하지만 TEST-1이 먼저 실행된다는 보장은 없다.
                */

                Console.ReadLine();
            }

            {
                writeLog("=== 예제 2: Task.Delay만 호출하면 대기하지 않음 ===");

                Task<int> task = Task.Run(async () =>
                {
                    writeLog("TEST-0");

                    await Task.Delay(5000).ConfigureAwait(false);

                    return 1;
                });

                Task.Delay(6000);

                /*
                    위 코드는 기다리지 않는다.
                    Task.Delay(6000)은 Task를 생성할 뿐,
                    await 또는 Wait을 하지 않으면 즉시 다음 줄로 넘어간다.
                */

                task.ContinueWith(t =>
                {
                    writeLog("TEST-1");
                });

                Console.ReadLine();
            }

            {
                writeLog("=== 예제 3: Thread.Sleep은 현재 스레드를 블로킹함 ===");

                Task<int> task = Task.Run(async () =>
                {
                    writeLog("TEST-0");

                    await Task.Delay(5000).ConfigureAwait(false);

                    return 1;
                });

                task.ContinueWith(t =>
                {
                    writeLog("TEST-1");
                });

                System.Threading.Thread.Sleep(6000);

                /*
                    Thread.Sleep(6000)은 현재 스레드를 6초 동안 멈춘다.

                    이 시점에는 task가 이미 완료되었을 가능성이 높다.
                    따라서 아래 ContinueWith는 이미 완료된 Task에 등록된다.

                    기본 ContinueWith는 ExecuteSynchronously 옵션이 없으므로
                    보통 ThreadPool에 예약되어 실행된다.
                */

                task.ContinueWith(t =>
                {
                    writeLog("TEST-2");
                });

                Console.ReadLine();
            }
        }

		static async Task completedTask()
		{
            // 기본값: await 뒤에 바로 이어서 실행될 수 있음 (동기적으로 최적화)
            await Task.CompletedTask; // 바로 이어서 동기 실행될 수 있음
        }

        static async Task<int> completedTaskWithResult()
        {
			return await Task.FromResult<int>(100);
        }

        static void Task_with_CompletedTask()
		{
            /*
				📚 즉시 완료 처리를 해도 될때 사용하는 Task  함수 !!!

				| 항목              | Task.FromResult<T>(value)                | Task.CompletedTask                          
				|-------------------|------------------------------------------|---------------------------------------------
				| 용도              | 결과(T)가 있는 Task를 즉시 완료시킬 때   | 결과가 없는(Task) 걸 즉시 완료시킬 때       
				| 리턴 타입         | Task<T>                                  | Task                                        
				| 값 반환 가능 여부 | O (value를 담아서 반환)                  | X (값 없음)                                 
				| 인스턴스 생성     | 호출할 때마다 Task<T> 하나 만들어짐      | .NET이 만든 하나를 재사용 (GC 대상아님)     
				| 완료 상태         | 만든 순간 TaskStatus.RanToCompletion     | 처음부터 TaskStatus.RanToCompletion         
				| 스케줄링 비용     | 거의 없음 (이미 완료된 Task)             | 거의 없음 (이미 완료된 Task)                
				| 언제 쓰나         | async 메서드가 값을 돌려줘야 할 때       | async 메서드가 할 일 없고 바로 끝낼 때      
				| 예시              | return Task.FromResult(42);              | return Task.CompletedTask;                  
			*/
            {
                completedTask().GetAwaiter();

                var result = completedTaskWithResult().GetAwaiter().GetResult();
                Console.WriteLine(result);
            }

            Console.ReadLine();
        }

        static void Task_with_TaskContinuationOptions()
		{
            /*
                📚 System.Threading.Tasks.TaskContinuationOptions 옵션

                OnlyOnRanToCompletion :
                    앞의 Task가 예외나 취소 없이 정상 완료(RanToCompletion)된 경우에만
                    continuation Task를 실행한다.

                OnlyOnFaulted :
                    앞의 Task가 처리되지 않은 예외로 인해 Faulted 상태가 된 경우에만
                    continuation Task를 실행한다.

                OnlyOnCanceled :
                    앞의 Task가 취소되어 Canceled 상태가 된 경우에만
                    continuation Task를 실행한다.

                ExecuteSynchronously :
                    continuation Task를 별도 큐에 넣지 않고,
                    가능하면 선행 Task를 완료 처리하는 스레드에서 바로 인라인 실행하도록 요청한다.

                    단, 항상 같은 스레드에서 즉시 실행되는 것은 아니다.
                    TaskScheduler 정책, 선행 Task의 완료 시점, continuation 등록 시점에 따라
                    큐에 들어갈 수도 있고, ContinueWith를 호출한 스레드에서 실행될 수도 있다.

                작동 원리
                :
                    ContinueWith()를 호출하면 continuation Task가 생성된다.

                    선행 Task가 아직 완료되지 않았다면,
                    continuation은 선행 Task의 continuation 목록에 등록된다.

                    선행 Task가 완료되면,
                    Task 내부 완료 처리 과정에서 등록된 continuation들의 옵션 조건을 검사한다.

                    선행 Task의 상태가 RanToCompletion이면 OnlyOnRanToCompletion 대상이 실행되고,
                    Faulted이면 OnlyOnFaulted 대상이 실행되고,
                    Canceled이면 OnlyOnCanceled 대상이 실행된다.

                    실행 대상이 된 continuation은 지정된 TaskScheduler에 의해 실행된다.

                    ExecuteSynchronously 옵션이 있으면,
                    가능하면 선행 Task를 완료 처리하는 스레드에서 인라인 실행된다.
                    하지만 이것은 강제 보장이 아니라 Scheduler에 대한 실행 힌트에 가깝다.
            */
            {
                /*
                    TEST-0 실행 후 TEST-1, TEST-2를 순서대로 실행하고 싶다면
                    ContinueWith()를 각각 task에 붙이면 안 된다.

                    아래처럼 continuation을 체인으로 연결해야 한다.

                    실행 흐름:

                        task(TEST-0)
                            -> continuation(TEST-1)
                                -> continuation(TEST-2)
                */
                var task = new Task(() =>
                {
                    /*
                        이 코드는 task.Start() 이후 TaskScheduler에 의해 실행된다.

                        기본적으로 별도 TaskScheduler를 지정하지 않았으므로
                        TaskScheduler.Default가 사용되고,
                        실제 실행은 ThreadPool Worker Thread에서 이루어진다.
                    */
                    writeLog("TEST-0");
                });


                /*
                    task.Start()

                    Created 상태의 Task를 실행 예약한다.

                    이 시점에 Task 객체가 TaskScheduler에 전달되고,
                    기본 Scheduler는 ThreadPool에 작업을 등록한다.

                    즉, Start()를 호출한 스레드가 TEST-0 코드를 직접 실행하는 것이 아니라,
                    일반적으로 ThreadPool Worker Thread가 TEST-0 코드를 실행한다.
                */
                task.Start();

                /*
                    ContinueWith()

                    선행 Task인 task가 완료된 후 실행할 continuation Task를 등록한다.

                    여기서는 첫 번째 ContinueWith()의 반환값에 다시 ContinueWith()를 붙이고 있다.

                    따라서 구조는 다음과 같다.

                        task
                          -> TEST-1 continuation
                              -> TEST-2 continuation

                    이 구조에서는 TEST-1이 끝난 후 TEST-2가 실행되므로
                    TEST-1, TEST-2의 실행 순서가 보장된다.

                    ExecuteSynchronously

                    continuation을 가능하면 선행 Task를 완료 처리하는 스레드에서
                    바로 인라인 실행하도록 요청한다.

                    단, 항상 같은 스레드에서 실행된다는 보장은 없다.
                    TaskScheduler 정책이나 실행 타이밍에 따라 달라질 수 있다.
                */
                task
                    .ContinueWith(_ =>
                    {
                        /*
                            previousTask는 선행 Task, 즉 TEST-0을 실행한 task다.

                            이 continuation은 task가 완료된 후 실행된다.

                            ExecuteSynchronously가 지정되어 있으므로,
                            가능하면 TEST-0을 완료 처리한 스레드에서
                            곧바로 이어서 실행될 수 있다.
                        */
                        writeLog("TEST-1");
                    }, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ =>
                    {
                        /*
                            previousTask는 바로 앞 continuation,
                            즉 TEST-1을 실행한 Task다.

                            따라서 이 코드는 TEST-1이 완료된 후 실행된다.

                            이 구조에서는 TEST-2가 TEST-1보다 먼저 실행될 수 없다.
                        */
                        writeLog("TEST-2");
                    }, TaskContinuationOptions.ExecuteSynchronously);

                Console.ReadLine();
            }

            {
                /*
                    하나의 선행 Task에 대해
                    정상 완료, 예외 발생, 취소 상태별로 서로 다른 continuation을 등록하는 예제다.

                    실행 흐름:

                        task
                         ├─ OnlyOnRanToCompletion
                         ├─ OnlyOnFaulted
                         └─ OnlyOnCanceled

                    이 세 continuation은 모두 같은 선행 Task에 붙어 있다.

                    선행 Task의 최종 상태에 따라
                    세 개 중 조건에 맞는 continuation만 실행된다.
                */
                Console.WriteLine("Start");

                /*
                    Task.Run()

                    ThreadPool에서 실행할 비동기 작업을 예약한다.

                    여기서는 1부터 10000까지의 합계를 계산하고,
                    결과를 int로 반환하는 Task<int>를 생성한다.

                    정상적으로 계산이 끝나면 task의 상태는 RanToCompletion이 된다.
                    예외가 발생하면 Faulted가 된다.
                    취소가 발생하면 Canceled가 된다.
                */
                var task = Task.Run(() => Enumerable.Range(1, 10000).Sum());

                /*
                    정상 완료 continuation

                    선행 Task가 RanToCompletion 상태가 된 경우에만 실행된다.

                    이 continuation 안에서는 task1.Result를 안전하게 읽을 수 있다.
                    왜냐하면 OnlyOnRanToCompletion 조건 때문에
                    이 코드가 실행될 때는 이미 정상 결과가 존재하기 때문이다.
                */
                task.ContinueWith(task1 =>
                {
                    Console.WriteLine("Success : {0}", task1.Result);

                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                /*
                    예외 발생 continuation

                    선행 Task가 Faulted 상태가 된 경우에만 실행된다.

                    주의:
                        OnlyOnFaulted는 "예외가 catch된 경우"가 아니다.

                        선행 Task 내부에서 예외가 처리되지 않고 밖으로 전파되어
                        Task가 Faulted 상태가 되었을 때 실행된다.

                    task1.Exception은 AggregateException 타입이다.
                    실제 원인 예외 메시지를 보려면 GetBaseException() 또는 InnerException을 확인하는 것이 좋다.
                */
                task.ContinueWith(task1 =>
                {
                    Console.WriteLine("Error : {0}", task1.Exception?.GetBaseException().Message);

                }, TaskContinuationOptions.OnlyOnFaulted);

                /*
                    취소 continuation

                    선행 Task가 Canceled 상태가 된 경우에만 실행된다.

                    단순히 예외가 발생했다고 Canceled가 되는 것은 아니다.

                    보통 CancellationToken을 사용하고,
                    해당 Token과 연결된 OperationCanceledException이 발생해야
                    Task가 Canceled 상태로 전환된다.
                */
                task.ContinueWith(task1 =>
                {
                    Console.WriteLine("Task was canceled.");

                }, TaskContinuationOptions.OnlyOnCanceled);

                /*
                    여기서 "End"는 위 continuation들이 끝난 뒤 출력되는 것이 아니다.

                    ContinueWith()는 continuation을 등록하고 바로 반환한다.

                    따라서 보통 출력 순서는 다음처럼 나올 수 있다.

                        Start
                        End
                        Success : 50005000

                    즉, End가 먼저 출력될 수 있다.
                */
                Console.WriteLine("End");

                System.Threading.Thread.Sleep(1000);

                Console.ReadLine();
            }
        }

        //=========================================================================================

        static void writeLogWithTime(string message)
        {
            Console.WriteLine(
                $"[{DateTime.Now:HH:mm:ss.fff}] " +
                $"Thread={System.Threading.Thread.CurrentThread.ManagedThreadId}, " +
                $"{message}");
        }

        static void ExecuteSynchronously_BeforeTaskCompleted()
        {
            writeLogWithTime("=== Example 1: 완료 전에 ContinueWith 등록 ===");

            var task = Task.Run(async () =>
            {
                writeLogWithTime("Task 시작");

                await Task.Delay(1000).ConfigureAwait(false);

                writeLogWithTime("Task 완료 직전");
                return 1;
            });

            task.ContinueWith(t =>
            {
                // ExecuteSynchronously:
                // 선행 Task가 완료되는 시점의 스레드에서 continuation이 바로 실행될 수 있다.
                // 단, 항상 같은 스레드라고 보장되는 것은 아니다.
                writeLogWithTime($"Continuation 실행, Result={t.Result}");

            }, TaskContinuationOptions.ExecuteSynchronously);

            task.Wait();

            writeLogWithTime("Example 1 종료");
        }

        static void ExecuteSynchronously_AfterTaskCompleted()
        {
            writeLogWithTime("=== Example 2: 완료 후 ContinueWith 등록 ===");

            var task = Task.Run(() =>
            {
                writeLogWithTime("Task 실행");
                return 10;
            });

            task.Wait();

            writeLogWithTime("Task는 이미 완료됨");

            task.ContinueWith(t =>
            {
                // 이미 완료된 Task에 ExecuteSynchronously를 사용하면
                // ContinueWith를 호출한 현재 스레드에서 바로 실행될 가능성이 높다.
                writeLogWithTime($"Continuation 실행, Result={t.Result}");

            }, TaskContinuationOptions.ExecuteSynchronously);

            writeLogWithTime("Example 2 종료");
        }

        static void TaskScheduler_Current()
        {
            writeLogWithTime("=== Example 3: TaskScheduler.Current ===");

            var task = Task.Run(async () =>
            {
                var items = new List<int>();

                for (int i = 0; i < 5; i++)
                {
                    items.Add(i);
                    writeLogWithTime($"load value = {i}");

                    await Task.Delay(500).ConfigureAwait(false);
                }

                return items;
            });

            writeLogWithTime($"TaskScheduler.Current = {System.Threading.Tasks.TaskScheduler.Current}");

            task.ContinueWith(t =>
            {
                // 콘솔 앱의 일반 스레드에서 TaskScheduler.Current는 보통 TaskScheduler.Default이다.
                // 즉, continuation은 ThreadPool에서 실행된다.
                foreach (var data in t.Result)
                {
                    writeLogWithTime($"value processed = {data}");
                }

            }, System.Threading.Tasks.TaskScheduler.Current);

            task.Wait();

            writeLogWithTime("Example 3 종료");
        }

        static void UI_ThreadScheduler()
        {
            writeLogWithTime("=== Example 4: UI 스레드로 continuation 전환 ===");

            var form = new Form
            {
                Text = "ContinueWith UI Scheduler Example",
                Width = 500,
                Height = 300
            };

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill
            };

            form.Controls.Add(listBox);

            form.Load += (sender, args) =>
            {
                writeLogWithTime("Form.Load");

                var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

                var task = Task.Run(async () =>
                {
                    // 이 코드는 ThreadPool에서 실행된다.
                    var items = new List<int>();

                    for (int i = 0; i < 5; i++)
                    {
                        items.Add(i);
                        writeLogWithTime($"load value = {i}");

                        await Task.Delay(500).ConfigureAwait(false);
                    }

                    return items;
                });

                task.ContinueWith(t =>
                {
                    // 이 코드는 UI 스레드에서 실행된다.
                    foreach (var data in t.Result)
                    {
                        var item = $"value to ui = {data}";
                        listBox.Items.Add(item);
                        writeLogWithTime(item);
                    }

                }, uiScheduler);
            };

            System.Windows.Forms.Application.Run(form);
        }

        static void Task_with_ContinueWith_for_thread_switch()
        {
            ExecuteSynchronously_BeforeTaskCompleted();

            Console.WriteLine();
            ExecuteSynchronously_AfterTaskCompleted();

            Console.WriteLine();
            TaskScheduler_Current();

            Console.WriteLine();
            UI_ThreadScheduler();

			Console.ReadLine();
        }

        //=========================================================================================

        static async Task Task_with_TaskAwaiter()
		{
			Task<Int32> t = Task<Int32>.Run(() =>
			{
				writeLog("TEST-0");
				return 1000;
			});

			TaskAwaiter<Int32> awaiter = t.GetAwaiter();	// Task 객체로 부터 TaskAwaiter 객체 얻어 옴
			awaiter.OnCompleted(() => 
			{
				writeLog("TEST-1");
			}); // TaskAwaiter의 Complete 메소드에 연속 작업 등록, 인자 없는 Action 타입

			awaiter.OnCompleted(() =>
			{
				writeLog("TEST-2");
				Console.WriteLine($"Result by TakeAwaiter - resultValue:{awaiter.GetResult()}");
			});

			Console.ReadLine();
		}

		static async Task Task_with_WhenAll()
		{
			List<string> strings = new List<string> { "s1", "s2", "s3" };
			List<Task> Tasks = new List<Task>();

			foreach (var s in strings)
			{
				Tasks.Add(Task.Run(() => Console.WriteLine(s)));
			}

			// Parallel.ForEach() 동일한 효과를 얻을 수 있다.
			await Task.WhenAll(Tasks);

			Console.ReadLine();
		}

		static async Task doWaitAll()
		{
			Task task1 = new Task(() => {
				for (int i = 0; i < 5; i++)
				{
					Console.WriteLine("Task 1 - iteration {0}", i);
					Task.Delay(1000);
				}
				Console.WriteLine("Task 1 complete");
			});
			Task task2 = new Task(() => {
				Console.WriteLine("Task 2 complete");
			});

			task1.Start();
			task2.Start();
			Console.WriteLine("Waiting for tasks to complete by Task.WaitAll()");

			Task.WaitAll(task1, task2); // 모든 task 가 처리 완료될때 까지 기다린다. 호출자의 thread 는 대기 하게 된다.
			Console.WriteLine("Task.WaitAll() Completed.");
		}

		static async Task doWhenAll()
		{
			Task task1 = new Task(() => {
				for (int i = 0; i < 5; i++)
				{
					Console.WriteLine("Task 1 - iteration {0}", i);
					Task.Delay(1000);
				}
				Console.WriteLine("Task 1 complete");
			});
			Task task2 = new Task(() => {
				Console.WriteLine("Task 2 complete");
			});

			task1.Start();
			task2.Start();
			Console.WriteLine("Waiting for tasks to complete by Task.WhenAll()");

			// 모든 Task 의 완료를 기다리지 않고 함수를 반환 한다.
			await Task.WhenAll(task1, task2);
			Console.WriteLine("Task.WhenAll() Completed.");
		}

		static async Task Task_with_WaitAll_and_WhenAll()
		{
			await doWaitAll();

			await doWhenAll();

			Console.ReadLine();
		}

        static async void Task_with_cancel()
		{
			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			tokenSource.Cancel();

			var t = Task.Run(() => Console.WriteLine("Hello from Task"), token);

			t.Wait();

			Console.ReadLine();
		}

		static void Task_with_CancellationTokenSource()
        {
			var cts = new System.Threading.CancellationTokenSource();
			CancellationToken token = cts.Token;
			token.Register(
			   () => Console.WriteLine("Cancelling....")
			);

			Console.WriteLine("Press any key to start");
			Console.ReadLine();

			// Run a task so that we can cancel from another thread.
			Task.Factory.StartNew(() =>
			{
				Console.WriteLine("Press 'c' + 'Enter' to cancel");
				if (Console.ReadLine() == "c")
				{
					cts.Cancel();
				}
			});

			Task<Int32> task = Task.Factory.StartNew( () =>
			{
				Int32 sum = 0;

				while(true)
				{
					if (token.IsCancellationRequested)
					{
						Console.WriteLine($"checked CancelToken !!! - LastSum:{sum}");
						break;
					}

					sum += 10;

					Console.WriteLine($"Sum:{sum} on {System.Threading.Thread.CurrentThread.ManagedThreadId}");

					Task.Delay(1000);
				}

				return sum;
			});

			task.Wait();

			Console.ReadLine();
		}


		public struct Rectangle
		{
			public int columns;
			public int rows;
		}

		static void NestedLoops(Rectangle rect, CancellationToken token)
		{
			var canceled_col = 0;

			for (var col = 0; col < rect.columns && !token.IsCancellationRequested; col++)
			{
				// Assume that we know that the inner loop is very fast.
				// Therefore, polling once per column in the outer loop condition
				// is sufficient.
				for (int row = 0; row < rect.rows; row++)
				{
					// Simulating work.
					System.Threading.Thread.SpinWait(5000);
					Console.Write("{0},{1} ", col, row);
				}

				canceled_col = col;
			}

			if (token.IsCancellationRequested)
			{
				// Cleanup or undo here if necessary...
				Console.WriteLine("\r\nCancelling before column {0}.", canceled_col);
				Console.WriteLine("Press any key to exit.");

				// If using Task:
				// token.ThrowIfCancellationRequested();
			}
		}

		static void Task_with_canel_by_polling()
		{
			var tokenSource = new CancellationTokenSource();

			// Toy object for demo purposes
			Rectangle rect = new Rectangle() { columns = 1000, rows = 500 };

			// Simple cancellation scenario #1. Calling thread does not wait
			// on the task to complete, and the user delegate simply returns
			// on cancellation request without throwing.
			Task.Run(() => NestedLoops(rect, tokenSource.Token), tokenSource.Token);

			// Simple cancellation scenario #2. Calling thread does not wait
			// on the task to complete, and the user delegate throws
			// OperationCanceledException to shut down task and transition its state.
			// Task.Run(() => PollByTimeSpan(tokenSource.Token), tokenSource.Token);

			Console.WriteLine("Press 'c' to cancel");
			if (Console.ReadKey(true).KeyChar == 'c')
			{
				tokenSource.Cancel();
				Console.WriteLine("Press any key to exit.");
			}

			Console.ReadKey();
			tokenSource.Dispose();

			Console.ReadLine();
		}

		static void StartWebRequest(CancellationToken token)
		{
			WebClient wc = new WebClient();
			wc.DownloadStringCompleted += (s, e) => Console.WriteLine("Request completed.");

			// Cancellation on the token will
			// call CancelAsync on the WebClient.
			token.Register(() =>
			{
				wc.CancelAsync();
				Console.WriteLine("Request cancelled!");
			});

			Console.WriteLine("Starting request.");
			wc.DownloadStringAsync(new Uri("http://www.contoso.com"));
		}


		static void WebClient_with_cancel_by_callback()
		{
			CancellationTokenSource cts = new CancellationTokenSource();

			StartWebRequest(cts.Token);

			// cancellation will cause the web
			// request to be cancelled
			cts.Cancel();

			Console.ReadLine();
		}

		static void DoCheckWithManualResetEvent(CancellationToken token, System.Threading.ManualResetEvent manualResetEvent)
		{
			while (true)
			{
				// Wait on the event if it is not signaled.
				int eventThatSignaledIndex = WaitHandle.WaitAny(new WaitHandle[] { manualResetEvent, token.WaitHandle }
															   , new TimeSpan(0, 0, 20));

				// Were we canceled while waiting?
				if (eventThatSignaledIndex == 1)
				{
					Console.WriteLine("The wait operation was canceled.");
					throw new OperationCanceledException(token);
				}
				// Were we canceled while running?
				else if (token.IsCancellationRequested)
				{
					Console.WriteLine("I was canceled while running.");
					token.ThrowIfCancellationRequested();
				}
				// Did we time out?
				else if (eventThatSignaledIndex == WaitHandle.WaitTimeout)
				{
					Console.WriteLine("I timed out.");
					break;
				}
				else
				{
					Console.Write("Working... ");

					// Simulating work.
					System.Threading.Thread.SpinWait(5000000);
				}
			}
		}

		static void Task_with_cancel_by_WaitHandle_and_ManualResetEvent()
		{
			// Old-style MRE that doesn't support unified cancellation.
			var manual_reset_event = new System.Threading.ManualResetEvent(false);

			var cts = new CancellationTokenSource();

			// Pass the same token source to the delegate and to the task instance.
			Task.Run(() => DoCheckWithManualResetEvent(cts.Token, manual_reset_event), cts.Token);
			Console.WriteLine("Press s to start/restart, p to pause, or c to cancel.");
			Console.WriteLine("Or any other key to exit.");

			// Old-style UI thread.
			bool goAgain = true;
			while (goAgain)
			{
				char ch = Console.ReadKey(true).KeyChar;

				switch (ch)
				{
					case 'c':
						cts.Cancel();
						break;
					case 'p':
						manual_reset_event.Reset();
						break;
					case 's':
						manual_reset_event.Set();
						break;
					default:
						goAgain = false;
						break;
				}

				System.Threading.Thread.Sleep(100);
			}
			cts.Dispose();

			Console.ReadLine();
		}

		static void DoCheckWithManualResetEventSlim(CancellationToken token, System.Threading.ManualResetEventSlim manualResetEventSlim)
		{
			while (true)
			{
				if (token.IsCancellationRequested)
				{
					Console.WriteLine("Canceled while running.");
					token.ThrowIfCancellationRequested();
				}

				// Wait on the event to be signaled
				// or the token to be canceled,
				// whichever comes first. The token
				// will throw an exception if it is canceled
				// while the thread is waiting on the event.
				try
				{
					// mres is a ManualResetEventSlim
					manualResetEventSlim.Wait(token);
				}
				catch (OperationCanceledException)
				{
					// Throw immediately to be responsive. The
					// alternative is to do one more item of work,
					// and throw on next iteration, because
					// IsCancellationRequested will be true.
					Console.WriteLine("The wait operation was canceled.");
					throw;
				}

				Console.Write("Working...");
				// Simulating work.
				System.Threading.Thread.SpinWait(500000);
			}
		}

		static void Task_with_cancel_by_WaitHandle_and_ManualResetEventSlim()
		{
			// New-style MRESlim that supports unified cancellation
			// in its Wait methods.
			var manual_reset_event_slim = new System.Threading.ManualResetEventSlim(false);

			var cts = new CancellationTokenSource();

			// Pass the same token source to the delegate and to the task instance.
			Task.Run(() => DoCheckWithManualResetEventSlim(cts.Token, manual_reset_event_slim), cts.Token);
			Console.WriteLine("Press c to cancel, p to pause, or s to start/restart,");
			Console.WriteLine("or any other key to exit.");

			// New-style UI thread.
			bool goAgain = true;
			while (goAgain)
			{
				char ch = Console.ReadKey(true).KeyChar;

				switch (ch)
				{
					case 'c':
						// Token can only be canceled once.
						cts.Cancel();
						break;
					case 'p':
						manual_reset_event_slim.Reset();
						break;
					case 's':
						manual_reset_event_slim.Set();
						break;
					default:
						goAgain = false;
						break;
				}

				System.Threading.Thread.Sleep(100);
			}
			cts.Dispose();

			Console.ReadLine();
		}

		class WorkerWithTimer
		{
			CancellationTokenSource internalTokenSource = new CancellationTokenSource();
			CancellationToken internalToken;
			CancellationToken externalToken;
			System.Threading.Timer timer;

			public WorkerWithTimer()
			{
				// A toy cancellation trigger that times out after 3 seconds
				// if the user does not press 'c'.
				timer = new System.Threading.Timer(new TimerCallback(CancelAfterTimeout), null, 3000, 3000);
			}

			public void DoWork(CancellationToken externalToken)
			{
				// Create a new token that combines the internal and external tokens.
				this.internalToken = internalTokenSource.Token;
				this.externalToken = externalToken;

				using (CancellationTokenSource linkedCts =
						CancellationTokenSource.CreateLinkedTokenSource(internalToken, externalToken))
				{
					try
					{
						DoWorkInternal(linkedCts.Token);
					}
					catch (OperationCanceledException)
					{
						if (internalToken.IsCancellationRequested)
						{
							Console.WriteLine("Operation timed out.");
						}
						else if (externalToken.IsCancellationRequested)
						{
							Console.WriteLine("Cancelling per user request.");
							externalToken.ThrowIfCancellationRequested();
						}
					}
				}
			}

			private void DoWorkInternal(CancellationToken token)
			{
				for (int i = 0; i < 1000; i++)
				{
					if (token.IsCancellationRequested)
					{
						// We need to dispose the timer if cancellation
						// was requested by the external token.
						timer.Dispose();

						// Throw the exception.
						token.ThrowIfCancellationRequested();
					}

					// Simulating work.
					System.Threading.Thread.SpinWait(7500000);
					Console.Write("working... ");
				}
			}

			public void CancelAfterTimeout(object state)
			{
				Console.WriteLine("\r\nTimer fired.");
				internalTokenSource.Cancel();
				timer.Dispose();
			}
		}

		static void Task_with_multiple_cancel_by_CreateLinkedTokenSource()
		{
			WorkerWithTimer worker = new WorkerWithTimer();
			CancellationTokenSource cts = new CancellationTokenSource();

			// Task for UI thread, so we can call Task.Wait wait on the main thread.
			Task.Run(() =>
			{
				Console.WriteLine("Press 'c' to cancel within 3 seconds after work begins.");
				Console.WriteLine("Or let the task time out by doing nothing.");
				if (Console.ReadKey(true).KeyChar == 'c')
				{
					cts.Cancel();
				}
			});

			// Let the user read the UI message.
			System.Threading.Thread.Sleep(1000);

			// Start the worker task.
			Task task = Task.Run(() => worker.DoWork(cts.Token), cts.Token);

			try
			{
				task.Wait(cts.Token);
			}
			catch (OperationCanceledException e)
			{
				if (e.CancellationToken == cts.Token)
				{
					Console.WriteLine("Canceled from UI thread throwing OCE.");
				}
			}
			catch (AggregateException ae)
			{
				Console.WriteLine("AggregateException caught: " + ae.InnerException);
				foreach (var inner in ae.InnerExceptions)
				{
					Console.WriteLine(inner.Message + inner.Source);
				}
			}

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
			cts.Dispose();

			Console.ReadLine();
		}

		static void WorkOfTask(int taskNum, CancellationToken ct)
		{
			// Was cancellation already requested?
			if (ct.IsCancellationRequested)
			{
				Console.WriteLine("Task {0} was cancelled before it got started.",
								  taskNum);
				ct.ThrowIfCancellationRequested();
			}

			int maxIterations = 100;

			// NOTE!!! A "TaskCanceledException was unhandled
			// by user code" error will be raised here if "Just My Code"
			// is enabled on your computer. On Express editions JMC is
			// enabled and cannot be disabled. The exception is benign.
			// Just press F5 to continue executing your code.
			for (int i = 0; i <= maxIterations; i++)
			{
				// Do a bit of work. Not too much.
				var sw = new System.Threading.SpinWait();
				for (int j = 0; j <= 100; j++)
				{
					sw.SpinOnce();
				}

				if (ct.IsCancellationRequested)
				{
					Console.WriteLine("Task {0} cancelled", taskNum);
					ct.ThrowIfCancellationRequested();
				}
			}
		}

		static async Task Task_with_CancellationTokenSource_and_SpinWait()
		{
			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			// Store references to the tasks so that we can wait on them and
			// observe their status after cancellation.
			Task t;
			var tasks = new ConcurrentBag<Task>();

			Console.WriteLine("Press any key to begin tasks...");
			Console.ReadKey(true);
			Console.WriteLine("To terminate the example, press 'c' to cancel and exit...");
			Console.WriteLine();

			// Request cancellation of a single task when the token source is canceled.
			// Pass the token to the user delegate, and also to the task so it can
			// handle the exception correctly.
			t = Task.Run(() => WorkOfTask(1, token), token);
			Console.WriteLine("Task {0} executing", t.Id);
			tasks.Add(t);

			// Request cancellation of a task and its children. Note the token is passed
			// to (1) the user delegate and (2) as the second argument to Task.Run, so
			// that the task instance can correctly handle the OperationCanceledException.
			t = Task.Run(() =>
			{
				// Create some cancelable child tasks.
				Task tc;
				for (int i = 3; i <= 10; i++)
				{
					// For each child task, pass the same token
					// to each user delegate and to Task.Run.
					tc = Task.Run(() => WorkOfTask(i, token), token);					
					Console.WriteLine("Task {0} executing", tc.Id);
					tasks.Add(tc);
					// Pass the same token again to do work on the parent task.
					// All will be signaled by the call to tokenSource.Cancel below.
					WorkOfTask(2, token);
				}
			}, token);

			Console.WriteLine("Task {0} executing", t.Id);
			tasks.Add(t);

			// Request cancellation from the UI thread.
			char ch = Console.ReadKey().KeyChar;
			if (ch == 'c' || ch == 'C')
			{
				tokenSource.Cancel();
				Console.WriteLine("\nTask cancellation requested.");

				// Optional: Observe the change in the Status property on the task.
				// It is not necessary to wait on tasks that have canceled. However,
				// if you do wait, you must enclose the call in a try-catch block to
				// catch the TaskCanceledExceptions that are thrown. If you do
				// not wait, no exception is thrown if the token that was passed to the
				// Task.Run method is the same token that requested the cancellation.
			}

			try
			{
				await Task.WhenAll(tasks.ToArray());
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
			}
			finally
			{
				tokenSource.Dispose();
			}

			// Display status of all tasks.
			foreach (var task in tasks)
			{
				Console.WriteLine("Task {0} status is now {1}", task.Id, task.Status);
			}

			Console.ReadLine();
		}

		class Updater
		{
			public void Run()
			{
				Update();
			}

			// async를 붙인다.
			private async void Update()
			{
				Console.WriteLine($"Updater.Update() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				Console.WriteLine($"\tStart Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				// 긴 계산을 하는 메서드 비동기로 호출
				var task = Task.Factory.StartNew(async delegate
				{
					Console.WriteLine($"\t\tCall await LongCalc() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
					var result = await LongCalc(10);

					Console.WriteLine($"\t\tEnd Task.Factory.StartNew() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
					return result;

				}).Unwrap();

				// Caller 스레드 여기서 리턴 되고, Callee 스레드가 Task 를 완료 후 Caller 입장이 되어 아래 로직들을 수행 한다.
				await task;

				Console.WriteLine($"Updater.Update() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				// 이 문장은 UI 쓰레드에서 실행되므로 Invoke()가 필요 없다.
				Console.WriteLine("Result: {0}", task.Result.ToString());
			}

			async Task<double> LongCalc(double r)
			{
				Console.WriteLine($"Updater.LongCalc() Start - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

				Console.WriteLine($"\tStart LongCalc >> Task.Delay(3000) - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				// 3초간 긴 계산
				await Task.Delay(3000).ContinueWith((arg) =>
				{
					Console.WriteLine($"\t\tEnd LongCalc >> Task.Delay(3000).ContinueWith() - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				});

				Console.WriteLine($"Updater.LongCalc() End - TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
				return 3.14 * r * r;
			}

		}

		static async void Task_with_TaskCompletionSource()
		{
			var tcs = new TaskCompletionSource<bool>();
			var tcs_task = tcs.Task;

			Console.WriteLine("Press any key to start");
			Console.ReadLine();

			// Start a background task that will complete tcs.Task
			var update_task = Task.Run(() =>
			{
				while(true)
				{
					Console.WriteLine("Press 'c' + 'Enter' to cancel");
					if (Console.ReadLine() == "c")
					{
						tcs.SetResult(true); // 반환값을 설정 한다.
						break;
					}

					Task.Delay(1000);
				}
			});

			update_task.Wait();
			bool is_success = await tcs_task; // 설정한 반환값이 반환 된다 !!!

			Console.WriteLine($"Task Completed !!! - result:{is_success}");

			Console.ReadLine();
		}

        static async Task Task_with_PromiseLike_Pattern()
        {
			/*
				C#의 Task는 원칙적으로 실행과 동시에 결과가 정해지지만,
				TaskCompletionSource<T>는 외부에서 직접 Task의 완료(성공/실패/취소)를 “약속”할 수 있다.

				즉, JS의 Promise에서 resolve/reject를 호출하듯
				C#에서는 SetResult, SetException, SetCanceled로 Task를 마감할 수 있다.

				* Promise-like 패턴
				- 나중에 외부에서 완료/실패를 지정할 수 있는 비동기 약속 객체
				- C#에서는 TaskCompletionSource<T>가 바로 그 역할
				- JS의 Promise와 거의 1:1로 대응하는 기능과 개념

				* Promise-like 패턴의 특징
				- 생성과 완료 타이밍을 분리할 수 있다.
				- 콜백/이벤트/외부 신호에 따라 Task의 결과를 정할 수 있다.
				- 비동기 래핑/추상화/인터페이스 연결이 쉽다.
				- 이벤트, 콜백 기반을 Task 기반 API로 감쌀 때 매우 편리하다.
			*/

			{
                var tcs = new TaskCompletionSource<int>();

                Task<int> myTask = tcs.Task; // 약속된 결과(Task), 이 Task는 처음에는 "진행중(대기중, WaitingForActivation)" 상태
                myTask.ContinueWith(t => Console.WriteLine("결과: " + t.Result));

                // 언제든 외부에서 결과를 정할 수 있음:
                tcs.SetResult(123); // 성공
                                    // 또는 tcs.SetException(new Exception("실패")); // 실패
            }

			{
				await waitForEventAsync();
            }

            Console.ReadLine();
        }

        public class MyPublisher
        {
            // 이벤트 멤버 선언
            public event EventHandler SomeEvent;

            public void Trigger()
            {
                // 이벤트 발생
                SomeEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        static Task<string> waitForEventAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            // 예: 어떤 이벤트 핸들러
            var publisher = new MyPublisher();
            EventHandler handler = (s, e) =>
            {
                tcs.SetResult("이벤트 발생!");
            };
            publisher.SomeEvent += handler;

            // 타임아웃 예외 처리 등도 가능
            Task.Delay(5000).ContinueWith(_ => tcs.TrySetException(new TimeoutException()));

            return tcs.Task; // 이 Task는 나중에 완료됨
        }

        static void Task_with_ui_thread()
        {
            /*
				await가 기다리는 Task는 항상 Background Thread에서 돌 필요는 없다.
				물론 많은 경우 Background Thread에서 실행될 것이지만... 다음 예제는 Task가 UI 쓰레드에서 실행되는 예이다.
				(물론 이러한 경우에도 중간에 Background Thread에서 도는 문장들을 넣는 것이 일반적이다.)
				하나의 CPU에서 멀티태스킹이 가능하듯이 하나의 UI Thread에서도 시분할을 통해 비동기 실행이 가능하다.
			*/
            {
                Updater u = new Updater();

                u.Run();

                Console.ReadLine();
            }
        }


        static async Task doAsyncFunc1()
        {
            Console.WriteLine(AppDomain.GetCurrentThreadId() + " doAsyncFunc1 start");
            await doAsyncFunc2().withoutContext();
            Console.WriteLine(AppDomain.GetCurrentThreadId() + " doAsyncFunc1 end");
        }

        static async Task doAsyncFunc2()
        {
            Console.WriteLine(AppDomain.GetCurrentThreadId() + " doAsyncFunc2 start");
            await doAsyncFunc3().withoutContext();
            Console.WriteLine(AppDomain.GetCurrentThreadId() + " doAsyncFunc2 end");
        }

        static async Task doAsyncFunc3()
        {
			/*
				- UI 스레드에서 doAsyncFunc3()을 호출할 때, 
				  만약 await 이후에 UI 컨트롤을 갱신할 필요가 없다면 ConfigureAwait(false)를 권장합니다.
				  (즉, UI 복귀가 불필요한 순수 백그라운드 작업만 있을 때)

				- 하지만 await 이후에 UI 컨트롤을 갱신해야 한다면 
				  ConfigureAwait(false)를 사용하지 않고, 기본 await로 두어야 합니다.
				  (기본 await는 SynchronizationContext를 캡처하여, await 이후에 자동으로 UI 스레드로 복귀합니다.)

				- 모든 UI 컨트롤은 반드시 UI 스레드(즉, SynchronizationContext가 적용된 스레드)에서만 안전하게 접근 및 갱신할 수 있습니다.

				- UI 스레드는 기본적으로 SynchronizationContext를 가지고 있으며,
				  SynchronizationContext 클래스는 다양한 동기화 모델(UI, ASP.NET 등)에서
				  작업의 실행 위치(스레드)를 제어할 수 있는 기능을 제공합니다.
			*/

			using (var httpClient = new HttpClient())
			{
				// 현재 SynchronizationContext를 저장 (UI 앱일 경우, UI 컨텍스트가 캡처됨)
				var currentContext = SynchronizationContext.Current;
				// HTTP 비동기 요청을 백그라운드에서 실행 (컨텍스트 복원 안 함)
				var httpResponse = await httpClient.GetAsync("https://www.bynder.com").withoutContext();

				// 응답 처리 (UI 컨텍스트에서 처리해야 할지 판별)
				if (currentContext != null)
				{
					// UI 작업이 필요하다면 SynchronizationContext.Post로 UI 스레드에 작업 등록
					currentContext.Post(async _ =>
					{
						try
						{
							// Post의 델리게이트는 async void가 됨에 주의! (예외 반드시 try/catch로 감싸기)
							string content = await httpResponse.Content.ReadAsStringAsync();
							// 여기에 UI 컨트롤 갱신 코드 (예시)
							// this.textBox.Text = content;
						}
						catch (Exception ex)
						{
							// UI 스레드에서 에러 처리
							// this.label.Text = "에러 발생: " + ex.Message;
						}
					}, null);
				}
				else
				{
					// UI 컨텍스트가 없을 경우 (ex. 콘솔 앱), 바로 결과 사용
					string content = await httpResponse.Content.ReadAsStringAsync();
					Console.WriteLine(content); // 예시: 콘솔에 출력
				}
			}

            Console.WriteLine(AppDomain.GetCurrentThreadId() + " doAsyncFunc3 start");
            await Task.Factory.StartNew(() => { Console.WriteLine("Task TID:" + AppDomain.GetCurrentThreadId()); System.Threading.Thread.Sleep(5000); }).withoutContext();
            Console.WriteLine(AppDomain.GetCurrentThreadId() + " doAsyncFunc3 end");
        }

        static async Task<int> ExampleAsync()
        {
            int x = 1;
			await Task.Delay(1000).withoutContext();
            return x + 1;
        }

        static async Task Task_with_ConfigureAwait()
		{
			/*
				🖥️ ConfigureAwait() 기능 정의

				- 비동기 메서드(async/await)에서, await 이후의 코드가 어느 "스레드" 또는 "컨텍스트"에서 실행될지 제어하는 기능입니다.
				- 기본적으로 await는 현재 "SynchronizationContext" (예: UI 스레드, ASP.NET 요청 스레드)를 캡처해서,
				  await 이후 코드도 같은 컨텍스트(즉, 같은 스레드)에서 실행되도록 만듭니다.

				- ConfigureAwait(false)를 쓰면
				  SynchronizationContext(컨텍스트)를 캡처하지 않으므로,
				  await 이후의 코드는 스레드풀 등 임의의 스레드에서 실행됩니다.


				⏳ 실행 흐름 (런타임): ConfigureAwait(false) 사용 시

				  - 전체 흐름 도식
					[호출자] ------> ExampleAsync() ----> ( 상태머신 인스턴스 생성 )
											 |
											 |----[ 동기 코드: x=1 실행 ]
											 |
											 |----[ await Task.Delay(1000).ConfigureAwait(false) ]
											 |        |
											 |        |--( Task 생성, awaiter 준비 )
											 |        |--( awaiter.IsCompleted? 대부분 false )
											 |        |--[ 상태 0로 저장
											 |        |  , 컨텍스트(SynchronizationContext/TaskScheduler) 캡처 및 복귀를 건너뜀
											 |        |  , 상태머신 인스턴스의 MoveNext 콜백(Continuation) 등록
			                                 |        |  , return ]
											 |        |
								[Task.Delay 완료되면]
											 |--( ThreadPool 워커 스레드에서 MoveNext() 실행 )
											 |--[ await 뒤 코드 resume, awaiter.GetResult(), return x+1 ]
											 |--[ Task 완료 및 Result=2 ]


				  (1) ExampleAsync() 호출
					- 호출 즉시
					  → 상태머신 인스턴스 생성 (Program+ExampleAsync>d__0 등)
					  → 반환값은 Task<int> (아직 미완료 Task)

				  (2) 상태 -1(초기): 동기 코드 실행
					- 상태: <>1__state = -1
					- int x = 1; 실행

				  (3) await Task.Delay(1000).ConfigureAwait(false)
					- Task.Delay(1000) 실행 → 비동기 Task 반환
					  → Awaiter(TaskAwaiter) 생성
					- awaiter.IsCompleted 검사 (대부분 false)

				  (4) 미완료 시: 콜백 등록 & 상태 보존
					- if (!awaiter.IsCompleted)
					  → 상태머신의 상태를 0으로 변경(<>1__state = 0)
					  → **컨텍스트(SynchronizationContext/TaskScheduler) 캡처 및 복귀 건너뜀
					  → TaskAwaiter에 상태머신 인스턴스의 MoveNext를 Continuation(콜백)으로 등록
					  → 현재 호출 스레드는 return!
						 (ExampleAsync는 “아직 끝나지 않은 Task”를 반환)

				  (5) Task.Delay가 완료되면
					- 타이머가 끝나고 TaskAwaiter가 등록해둔 MoveNext() 콜백 실행
					- 항상 ThreadPool 워커 스레드에서 실행 (컨텍스트 복귀 없음)

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
				⭐️ 요점
				  - ConfigureAwait(false)는 await 시점의 컨텍스트(예: UI 스레드, ASP.NET 요청 컨텍스트) 캡처 및 복귀 과정을 생략 !!!
				  - Task가 완료되면 바로 ThreadPool 워커 스레드에서 MoveNext() 실행 (컨텍스트 전환 X)
				  - await 뒤 코드에서 UI 등 특정 컨텍스트에 안전하게 접근해야 하는 경우는 반드시 주의!


				🟦 언제, 왜 쓰는가?
			
				1. 라이브러리, ASP.NET Core, 백엔드:
				  - 항상 ConfigureAwait(false) 권장
					
			        🚨 이유:
					  - 라이브러리나 백엔드 서버는
			            "특정 SynchronizationContext(예: UI 스레드, ASP.NET 요청 컨텍스트)"에서 코드를
			            계속 실행해야 할 필요가 없음.

					📈 효과:
					  ✔️ 더 빠른 실행(불필요한 컨텍스트 전환 없음)
			            - SynchronizationContext(컨텍스트) 캡처 생략
			            - 컨텍스트 복귀를 위한 작업 재등록(스케줄링) 생략
			            - 불필요한 스레드 전환/스위칭 생략
					  ✔️ 데드락 위험 감소
                      ✔️ 불필요한 스레드 점유 방지

					  🔹 정리표
					    | await                     | 컨텍스트 캡처 | 컨텍스트 복귀 | 스레드 전환 | 워커 스레드 실행
						|---------------------------|---------------|---------------|-------------|------------------
						| 기본 await (true)         | ✅	  		| ✅			| ✅		  | ❌ (컨텍스트에 따라)
						| ConfigureAwait(false)     | ❌			| ❌			| ❌		  | ✅ 
			
					public async Task<string> LoadDataAsync()
					{
						var data = await SomeApiCallAsync().ConfigureAwait(false);
						return data;
					}

				2. UI 코드(WinForms, WPF, MAUI 등)
				  - UI 갱신이 필요 없는 백그라운드 처리라면 ConfigureAwait(false) 권장
                    예: 대용량 파일 읽기, 네트워크 요청 후 UI와 무관한 로직 처리 등

                  - await 이후 UI 컨트롤을 갱신해야 한다면 기본 await(즉, ConfigureAwait(true)/미지정)를 사용
				  - UI 컨트롤을 갱신하려면 UI 스레드에서 실행되어야 하므로,
				    await 뒤에 UI 코드가 있다면 기본 await(또는 ConfigureAwait(true))를 써야 함
			
					// UI 갱신 필요 없음
					await Task.Run(() => LongRunningWork()).ConfigureAwait(false);
					this.Text = "UI 접근 Fail"; // InvalidOperationException (크로스 스레드 접근 예외)가 발생 !!!

					// UI 갱신 필요, default (컨텍스트 유지, 기본 동작)
					await SomeUiWorkAsync(); // 기본 await (UI 컨텍스트 복귀), await Task.Delay(1000).ConfigureAwait(true)와 동일 !!!
					this.Text = "UI 접근 OK"; // UI 스레드
			*/

			await ExampleAsync();

            await doAsyncFunc1();

			Console.ReadLine();
		}


		static void Task_with_FromCurrentSynchronizationContext()
		{
            /*
				UI 컴포넌트 조작
			
				* TPL(Task Parallel Library) 에는 스케쥴이라는 개념이 있으며 이것이 Task의 실행 순서를 정하고 Thread로 실행되도록 한다.
			      TPL 표준에서는 스레드 풀 태스크 스케쥴러와 동기 컨텍스트 스케쥴러가 있다.
                * TPL 의 기본은 스레드 풀 스케쥴러 이며 정적인 TaskScheduler.Default 프로퍼티에서 얻을 수 있다.
			      스레드 풀 스케줄러는 그 이름대로 태스크를 스레드 풀의 워커스레드로 등록하여 처리한다.
                * 동기 컨텍스트 스케쥴러는 정적인 TaskScheduler.FromCurrentSynchronizationContext 메소드에서 얻을 수 있다.
			      이 스케쥴러는 Windows Forms 나 WPF 등의 GUI 애플리케이션에서 사용되며,
			      Button 이나 Menu 등의 UI 컴포넌트를 Task 상에서 갱신하도록 Task 를 애플리케이션 UI Thread 에 등록하여 처리하도록 한다.
			      이 스케줄러를 이용해야만 안전하게 UI 컨트롤 접근이 가능하다.
			    * 스레드 풀(TaskScheduler.Default)에서는 UI 컨트롤에 직접 접근할 수 없습니다.
			      UI 컨트롤을 갱신하려면 반드시 동기 컨텍스트(TaskScheduler.FromCurrentSynchronizationContext) 기반의 스케줄러를 사용해야 합니다.
			*/
            {
                // 하기 코드는 Form 객체내의 맴버 함수 !!!
                // 아래 코드는 반드시 Form, WPF 등 UI 컨트롤 클래스의 멤버 함수에서 사용하세요!
                // 콘솔 앱에서는 InvalidOperationException 발생

                var t = Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(3000); // 긴 작업
                });

                // 이 부분은 반드시 UI 스레드에서 실행됨 (안전하게 컨트롤 접근 가능)
                t.ContinueWith(parent =>
                {
                    // this.button.Enabled = true; // UI 컨트롤 갱신
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }

			Console.ReadLine();
		}


		public static void Test()
		{
			//Task_with_FromCurrentSynchronizationContext();

			//Task_with_ConfigureAwait().Wait();

			//Task_with_ui_thread();

			//Task_with_PromiseLike_Pattern();

			//Task_with_TaskCompletionSource();

			//Task_with_CancellationTokenSource_and_SpinWait().Wait();

			//Task_with_multiple_cancel_by_CreateLinkedTokenSource();

			//Task_with_cancel_by_WaitHandle_and_ManualResetEventSlim();

			//Task_with_cancel_by_WaitHandle_and_ManualResetEvent();

			//WebClient_with_cancel_by_callback();

			//Task_with_canel_by_polling();

			//Task_with_CancellationTokenSource();

			//Task_with_cancel();

			//Task_with_WaitAll_and_WhenAll().Wait();

			//Task_with_WhenAll().Wait();

			//Task_with_WaitAny();

			//Task_with_Wait();

			//Task_with_awaitable().Wait();

			//Task_with_TaskAwaiter();

			Task_with_ContinueWith_for_thread_switch();

			//Task_with_TaskContinuationOptions();

			//Task_with_ContinueWith();

			//Task_with_CompletedTask();

			//Task_generic_use();

			//Task_with_exception();

			//Task_with_not_use_ThreadPool();

			//Task_StartNew_vs_Run();

			//Task_StartNew_with_ThreadPool();

			//Task_with_TaskCreationOptions();

			//Task_with_attrib_check();

			//Task_what();
		}
	}
}


