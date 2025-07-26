using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread
{
	public static class TaskAsyncHelper
	{
		private static readonly Task m_empty_task = makeEmpty();

		private static Task makeEmpty()
		{
			return fromResult<object>(null);
		}

		public static Task Empty
		{
			get
			{
				return m_empty_task;
			}
		}

		public static TTask tryCatch<TTask>(this TTask task, Action<Exception> handler) 
			where TTask : Task
		{
			if (task != null && task.Status != TaskStatus.RanToCompletion)
			{
				task.ContinueWith(innerTask =>
				{
					var e = innerTask.Exception;

					Console.WriteLine("SignalR exception thrown by Task: {0}", e);
					handler(e);

				}, TaskContinuationOptions.OnlyOnFaulted);
			}
			return task;
		}

		public static void continueWithNotComplete(this Task task, TaskCompletionSource<object> tcs)
		{
			task.ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					tcs.SetException(t.Exception); // tcs.Task도 Faulted(예외 Task)가 됨
                }
				else if (t.IsCanceled)
				{
					tcs.SetCanceled();
				}
			},
			TaskContinuationOptions.NotOnRanToCompletion); // "정상 완료가 아닐 때만" 콜백 실행됨
        }

		public static void continueWith(this Task task, TaskCompletionSource<object> tcs)
		{
			task.ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					tcs.TrySetException(t.Exception); // 예외
				}
				else if (t.IsCanceled)
				{
					tcs.TrySetCanceled(); // 취소
                }
				else
				{
					tcs.TrySetResult(null); // 정상 완료
                }
			});
		}

		public static void continueWith<T>(this Task<T> task, TaskCompletionSource<T> tcs)
		{
			task.ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					tcs.TrySetException(t.Exception); // 예외
                }
				else if (t.IsCanceled)
				{
					tcs.TrySetCanceled(); // 취소
                }
				else
				{
					tcs.TrySetResult(t.Result); // 정상 완료
                }
			});
		}

		public static Task interleave<T>( Func<T, Action, Task> before
			                            , Func<Task> after
			                            , T arg
			                            , TaskCompletionSource<object> tcs )
		{
			var tasks = new[] {
							tcs.Task,
							before(arg, () => after().continueWith(tcs))
						};

			return tasks.returnTask();
		}

		public static Task returnTask(this Task[] tasks)
		{
			return then(tasks, () => { });
		}

		// Then extesions
		public static Task then(this Task task, Action successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor);

				default:
					return runTask(task, successor);
			}
		}

		public static Task<TResult> then<TResult>(this Task task, Func<TResult> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError<TResult>(task.Exception);

				case TaskStatus.Canceled:
					return canceled<TResult>();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor);

				default:
					return TaskRunners<object, TResult>.runTask(task, successor);
			}
		}

		public static Task then(this Task[] tasks, Action successor)
		{
			if (tasks.Length == 0)
			{
				return fromMethod(successor);
			}

			var tcs = new TaskCompletionSource<object>();
			Task.Factory.ContinueWhenAll(tasks, completedTasks =>
			{
				var faulted = completedTasks.FirstOrDefault(t => t.IsFaulted);
				if (faulted != null)
				{
					tcs.SetException(faulted.Exception);
					return;
				}
				var cancelled = completedTasks.FirstOrDefault(t => t.IsCanceled);
				if (cancelled != null)
				{
					tcs.SetCanceled();
					return;
				}

				successor();
				tcs.SetResult(null);
			});

			return tcs.Task;
		}

		public static Task then<T1>(this Task task, Action<T1> successor, T1 arg1)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, arg1);

				default:
					return GenericDelegates<object, object, T1, object>.thenWithArgs(task, successor, arg1);
			}
		}

		public static Task then<T1, T2>(this Task task, Action<T1, T2> successor, T1 arg1, T2 arg2)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, arg1, arg2);

				default:
					return GenericDelegates<object, object, T1, T2>.thenWithArgs(task, successor, arg1, arg2);
			}
		}

		public static Task then<T1>(this Task task, Func<T1, Task> successor, T1 arg1)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, arg1).fastUnwrap();

				default:
					return GenericDelegates<object, Task, T1, object>.thenWithArgs(task, successor, arg1)
																	 .fastUnwrap();
			}
		}

		public static Task then<T1, T2>(this Task task, Func<T1, T2, Task> successor, T1 arg1, T2 arg2)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, arg1, arg2).fastUnwrap();

				default:
					return GenericDelegates<object, Task, T1, T2>.thenWithArgs(task, successor, arg1, arg2)
																 .fastUnwrap();
			}
		}

		public static Task<TResult> then<T, TResult>(this Task<T> task, Func<T, Task<TResult>> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError<TResult>(task.Exception);

				case TaskStatus.Canceled:
					return canceled<TResult>();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, task.Result).fastUnwrap();

				default:
					return TaskRunners<T, Task<TResult>>.runTask(task, t => successor(t.Result))
														.fastUnwrap();
			}
		}

		public static Task<TResult> then<T, TResult>(this Task<T> task, Func<T, TResult> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError<TResult>(task.Exception);

				case TaskStatus.Canceled:
					return canceled<TResult>();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, task.Result);

				default:
					return TaskRunners<T, TResult>.runTask(task, t => successor(t.Result));
			}
		}

		public static Task<TResult> then<T, T1, TResult>(this Task<T> task, Func<T, T1, TResult> successor, T1 arg1)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError<TResult>(task.Exception);

				case TaskStatus.Canceled:
					return canceled<TResult>();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, task.Result, arg1);

				default:
					return GenericDelegates<T, TResult, T1, object>.thenWithArgs(task, successor, arg1);
			}
		}

		public static Task then(this Task task, Func<Task> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor).fastUnwrap();

				default:
					return TaskRunners<object, Task>.runTask(task, successor)
													.fastUnwrap();
			}
		}

		public static Task<TResult> then<TResult>(this Task task, Func<Task<TResult>> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError<TResult>(task.Exception);

				case TaskStatus.Canceled:
					return canceled<TResult>();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor).fastUnwrap();

				default:
					return TaskRunners<object, Task<TResult>>.runTask(task, successor)
															 .fastUnwrap();
			}
		}

		public static Task then<TResult>(this Task<TResult> task, Action<TResult> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, task.Result);

				default:
					return TaskRunners<TResult, object>.runTask(task, successor);
			}
		}

		public static Task then<TResult>(this Task<TResult> task, Func<TResult, Task> successor)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError(task.Exception);

				case TaskStatus.Canceled:
					return canceled();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, task.Result).fastUnwrap();

				default:
					return TaskRunners<TResult, Task>.runTask(task, t => successor(t.Result))
													 .fastUnwrap();
			}
		}

		public static Task<TResult> then<TResult, T1>(this Task<TResult> task, Func<Task<TResult>, T1, Task<TResult>> successor, T1 arg1)
		{
			switch (task.Status)
			{
				case TaskStatus.Faulted:
					return fromError<TResult>(task.Exception);

				case TaskStatus.Canceled:
					return canceled<TResult>();

				case TaskStatus.RanToCompletion:
					return fromMethod(successor, task, arg1).fastUnwrap();

				default:
					return GenericDelegates<TResult, Task<TResult>, T1, object>.thenWithArgs(task, successor, arg1)
																			   .fastUnwrap();
			}
		}

		public static Task fastUnwrap(this Task<Task> task)
		{
			var innerTask = (task.Status == TaskStatus.RanToCompletion) ? task.Result : null;
			return innerTask ?? task.Unwrap(); // 중첩 테스트시에 여러번 await하지 않고 한 번에 최종의 결과를 가져오게 해주는 패턴
        }

        public static Task<T> fastUnwrap<T>(this Task<Task<T>> task)
		{
			var innerTask = (task.Status == TaskStatus.RanToCompletion) ? task.Result : null;
			return innerTask ?? task.Unwrap(); // 중첩 테스트시에 여러번 await하지 않고 한 번에 최종의 결과를 가져오게 해주는 패턴
        }

		public static Task delay(TimeSpan timeOut)
		{ 
			var tcs = new TaskCompletionSource<object>();

			var timer = new System.Threading.Timer( tcs.SetResult	// 타이머 만료 시 호출할 콜백
                                                  , null			// 콜백 파라미터(사용 안 함)
                                                  , timeOut         // 타이머 대기 시간
                                                  , TimeSpan.FromMilliseconds(-1) ); // 반복 없이 한 번만 실행

            return tcs.Task.ContinueWith(_ =>
			{
				timer.Dispose(); // 타이머 해제 (메모리 누수 방지)
            }, TaskContinuationOptions.ExecuteSynchronously); // 후속 작업(continuation) 코드를 가능하면 즉시, 호출된 스레드에서 실행하라
															  // (스레드풀에서 스레드를 꺼내지 않고, 현재 스레드로 처리)
        }

		public static Task allSucceeded(this Task[] tasks, Action continuation)
		{
			return allSucceeded(tasks, _ => continuation());
		}

		public static Task allSucceeded(this Task[] tasks, Action<Task[]> continuation)
		{
			return Task.Factory.ContinueWhenAll(tasks, _ =>
			{
				var cancelledTask = tasks.FirstOrDefault(task => task.IsCanceled);
				if (cancelledTask != null)
					throw new TaskCanceledException();

				var allExceptions =
					tasks.Where(task => task.IsFaulted).SelectMany(task => task.Exception.InnerExceptions).ToList();

				if (allExceptions.Count > 0)
				{
					throw new AggregateException(allExceptions);
				}

				continuation(tasks);
			});
		}

		public static Task fromMethod(Action func)
		{
			try
			{
				func();
				return Empty;
			}
			catch (Exception ex)
			{
				return fromError(ex);
			}
		}

		public static Task fromMethod<T1>(Action<T1> func, T1 arg)
		{
			try
			{
				func(arg);
				return Empty;
			}
			catch (Exception ex)
			{
				return fromError(ex);
			}
		}

		public static Task fromMethod<T1, T2>(Action<T1, T2> func, T1 arg1, T2 arg2)
		{
			try
			{
				func(arg1, arg2);
				return Empty;
			}
			catch (Exception ex)
			{
				return fromError(ex);
			}
		}

		public static Task<TResult> fromMethod<TResult>(Func<TResult> func)
		{
			try
			{
				return fromResult<TResult>(func());
			}
			catch (Exception ex)
			{
				return fromError<TResult>(ex);
			}
		}

		public static Task<TResult> fromMethod<T1, TResult>(Func<T1, TResult> func, T1 arg)
		{
			try
			{
				return fromResult<TResult>(func(arg));
			}
			catch (Exception ex)
			{
				return fromError<TResult>(ex);
			}
		}

		public static Task<TResult> fromMethod<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
		{
			try
			{
				return fromResult<TResult>(func(arg1, arg2));
			}
			catch (Exception ex)
			{
				return fromError<TResult>(ex);
			}
		}

		public static Task<T> fromResult<T>(T value)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(value);
			return tcs.Task;
		}

		public static TaskContinueWithMethod getContinueWith(Type taskType)
		{
			var continueWith = (from m in taskType.GetMethods()
								let methodParameters = m.GetParameters()
								where m.Name.Equals("ContinueWith", StringComparison.OrdinalIgnoreCase) &&
									methodParameters.Length == 1
								let parameter = methodParameters[0]
								where parameter.ParameterType.IsGenericType &&
								typeof(Func<,>) == parameter.ParameterType.GetGenericTypeDefinition()
								select new TaskContinueWithMethod
								{
									Method = m.MakeGenericMethod(typeof(Task)),
									Type = parameter.ParameterType.GetGenericArguments()[0]
								}).FirstOrDefault();

			return continueWith;
		}

		internal static Task fromError(Exception e)
		{
			var tcs = new TaskCompletionSource<object>();
			tcs.SetException(e);
			return tcs.Task;
		}

		internal static Task<T> fromError<T>(Exception e)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetException(e);
			return tcs.Task;
		}

		private static Task canceled()
		{
			var tcs = new TaskCompletionSource<object>();
			tcs.SetCanceled();
			return tcs.Task;
		}

		private static Task<T> canceled<T>()
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetCanceled();
			return tcs.Task;
		}

		public class TaskContinueWithMethod
		{
			public MethodInfo Method { get; set; }
			public Type Type { get; set; }
		}

		private static Task runTask(Task task, Action successor)
		{
			var tcs = new TaskCompletionSource<object>();
			task.ContinueWith(t =>
			{
				if (t.IsFaulted)
				{
					tcs.SetException(t.Exception);
				}
				else if (t.IsCanceled)
				{
					tcs.SetCanceled();
				}
				else
				{
					try
					{
						successor();
						tcs.SetResult(null);
					}
					catch (Exception ex)
					{
						tcs.SetException(ex);
					}
				}
			});

			return tcs.Task;
		}

		private static class TaskRunners<T, TResult>
		{
			internal static Task runTask(Task<T> task, Action<T> successor)
			{
				var tcs = new TaskCompletionSource<object>();
				task.ContinueWith(t =>
				{
					if (t.IsFaulted)
					{
						tcs.SetException(t.Exception);
					}
					else if (t.IsCanceled)
					{
						tcs.SetCanceled();
					}
					else
					{
						try
						{
							successor(t.Result);
							tcs.SetResult(null);
						}
						catch (Exception ex)
						{
							tcs.SetException(ex);
						}
					}
				});

				return tcs.Task;
			}

			internal static Task<TResult> runTask(Task task, Func<TResult> successor)
			{
				var tcs = new TaskCompletionSource<TResult>();
				task.ContinueWith(t =>
				{
					if (t.IsFaulted)
					{
						tcs.SetException(t.Exception);
					}
					else if (t.IsCanceled)
					{
						tcs.SetCanceled();
					}
					else
					{
						try
						{
							tcs.SetResult(successor());
						}
						catch (Exception ex)
						{
							tcs.SetException(ex);
						}
					}
				});

				return tcs.Task;
			}

			internal static Task<TResult> runTask(Task<T> task, Func<Task<T>, TResult> successor)
			{
				var tcs = new TaskCompletionSource<TResult>();
				task.ContinueWith(t =>
				{
					if (task.IsFaulted)
					{
						tcs.SetException(t.Exception);
					}
					else if (task.IsCanceled)
					{
						tcs.SetCanceled();
					}
					else
					{
						try
						{
							tcs.SetResult(successor(t));
						}
						catch (Exception ex)
						{
							tcs.SetException(ex);
						}
					}
				});

				return tcs.Task;
			}
		}

		private static class GenericDelegates<T, TResult, T1, T2>
		{
			internal static Task thenWithArgs(Task task, Action<T1> successor, T1 arg1)
			{
				return runTask(task, () => successor(arg1));
			}

			internal static Task thenWithArgs(Task task, Action<T1, T2> successor, T1 arg1, T2 arg2)
			{
				return runTask(task, () => successor(arg1, arg2));
			}

			internal static Task<TResult> thenWithArgs(Task task, Func<T1, TResult> successor, T1 arg1)
			{
				return TaskRunners<object, TResult>.runTask(task, () => successor(arg1));
			}

			internal static Task<TResult> thenWithArgs(Task task, Func<T1, T2, TResult> successor, T1 arg1, T2 arg2)
			{
				return TaskRunners<object, TResult>.runTask(task, () => successor(arg1, arg2));
			}

			internal static Task<TResult> thenWithArgs(Task<T> task, Func<T, T1, TResult> successor, T1 arg1)
			{
				return TaskRunners<T, TResult>.runTask(task, t => successor(t.Result, arg1));
			}

			internal static Task<Task> thenWithArgs(Task task, Func<T1, Task> successor, T1 arg1)
			{
				return TaskRunners<object, Task>.runTask(task, () => successor(arg1));
			}

			internal static Task<Task> thenWithArgs(Task task, Func<T1, T2, Task> successor, T1 arg1, T2 arg2)
			{
				return TaskRunners<object, Task>.runTask(task, () => successor(arg1, arg2));
			}

			internal static Task<Task<TResult>> thenWithArgs(Task<T> task, Func<T, T1, Task<TResult>> successor, T1 arg1)
			{
				return TaskRunners<T, Task<TResult>>.runTask(task, t => successor(t.Result, arg1));
			}

			internal static Task<Task<T>> thenWithArgs(Task<T> task, Func<Task<T>, T1, Task<T>> successor, T1 arg1)
			{
				return TaskRunners<T, Task<T>>.runTask(task, t => successor(t, arg1));
			}
		}
	}
}
