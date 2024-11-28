using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class CoroutineExtensions
{
    public static Task AsTask(this IEnumerator coroutine, MonoBehaviour monoBehaviour)
    {
        var tcs = new TaskCompletionSource<object>();
        monoBehaviour.StartCoroutine(RunCoroutine(coroutine, tcs));
        return tcs.Task;
    }

    private static IEnumerator RunCoroutine(IEnumerator coroutine, TaskCompletionSource<object> tcs)
    {
        yield return coroutine;
        tcs.SetResult(null);
    }
}
