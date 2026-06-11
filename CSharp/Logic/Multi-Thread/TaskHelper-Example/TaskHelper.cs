using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;



namespace MultiThread
{
	public static class TaskHelper
    {
        //=============================================================================================
        // Async/Await extensions
        //=============================================================================================

        public static async Task thenAsync(this Task task, Action successor)
        {
            await task.ConfigureAwait(false);
            successor();
        }

        public static async Task<TResult> thenAsync<TResult>(
            this Task task,
            Func<TResult> successor)
        {
            await task.ConfigureAwait(false);
            return successor();
        }

        public static async Task thenAsync(
            this Task task,
            Func<Task> successor)
        {
            await task.ConfigureAwait(false);
            await successor().ConfigureAwait(false);
        }

        public static async Task<TResult> thenAsync<TResult>(
            this Task task,
            Func<Task<TResult>> successor)
        {
            await task.ConfigureAwait(false);
            return await successor().ConfigureAwait(false);
        }

        public static async Task thenAsync<T>(
            this Task<T> task,
            Action<T> successor)
        {
            var result = await task.ConfigureAwait(false);
            successor(result);
        }

        public static async Task<TResult> thenAsync<T, TResult>(
            this Task<T> task,
            Func<T, TResult> successor)
        {
            var result = await task.ConfigureAwait(false);
            return successor(result);
        }

        public static async Task thenAsync<T>(
            this Task<T> task,
            Func<T, Task> successor)
        {
            var result = await task.ConfigureAwait(false);
            await successor(result).ConfigureAwait(false);
        }

        public static async Task<TResult> thenAsync<T, TResult>(
            this Task<T> task,
            Func<T, Task<TResult>> successor)
        {
            var result = await task.ConfigureAwait(false);
            return await successor(result).ConfigureAwait(false);
        }

        //=============================================================================================
        // Use Task.ContinueWith() with Canceled, Faulted, Succeeded
        //=============================================================================================

        private static Task continueByStatusCore<T>(
            Task<T> task,
            Action<T> onSuccess,
            Action<Exception> onFaulted,
            Action onCanceled)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    onCanceled?.Invoke();
                    return;
                }

                if (t.IsFaulted)
                {
                    onFaulted?.Invoke(t.Exception?.GetBaseException());
                    return;
                }

                onSuccess?.Invoke(t.Result);

            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task continueByStatus<T>(
            this Task<T> task,
            Action<T> onSuccess = null,
            Action<Exception> onFaulted = null,
            Action onCanceled = null)
        {
            return continueByStatusCore(task, onSuccess, onFaulted, onCanceled);
        }

        private static Task continueByStatusCore(
            Task task,
            Action onSuccess,
            Action<Exception> onFaulted,
            Action onCanceled)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    onCanceled?.Invoke();
                    return;
                }

                if (t.IsFaulted)
                {
                    onFaulted?.Invoke(t.Exception?.GetBaseException()
                                      ?? new InvalidOperationException("Task faulted without exception."));
                    return;
                }

                onSuccess?.Invoke();

            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task continueByStatus(
            this Task task,
            Action onSuccess = null,
            Action<Exception> onFaulted = null,
            Action onCanceled = null)
        {
            return continueByStatusCore(task, onSuccess, onFaulted, onCanceled);
        }

        //=============================================================================================
        // 컨텍스트 복귀 없이 await 하도록 처리 (ConfigureAwait(false))
        //=============================================================================================

        public static ConfiguredTaskAwaitable withoutContext(this Task task)
            => task.ConfigureAwait(false);

        public static ConfiguredTaskAwaitable<T> withoutContext<T>(this Task<T> task)
            => task.ConfigureAwait(false);
    }
}
