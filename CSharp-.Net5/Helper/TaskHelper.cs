using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace Helper;

public static class TaskHelper
{
    //=============================================================================================
    // 컨텍스트 복귀 없이 await 하도록 처리 (ConfigureAwait(false))
    //=============================================================================================

    public static ConfiguredTaskAwaitable withoutContext(this Task task)
        => task.ConfigureAwait(false);

    public static ConfiguredTaskAwaitable<T> withoutContext<T>(this Task<T> task)
        => task.ConfigureAwait(false);

    public static ConfiguredValueTaskAwaitable withoutContext(this ValueTask task)
        => task.ConfigureAwait(false);

    public static ConfiguredValueTaskAwaitable<T> withoutContext<T>(this ValueTask<T> task)
        => task.ConfigureAwait(false);
}
