using DG.Tweening;
using UnityEngine;

public class Utils
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }


}

public static class DOTweenAwaitableExtensions
{
    public static Awaitable Awaiting(this Tween tween)
    {
        var acs = new AwaitableCompletionSource();
        bool isCompleted = false;
        tween.OnComplete(() =>
        {
            if (isCompleted) return;
            isCompleted = true;
            acs.SetResult();

        });
        tween.OnKill(() =>
        {
            if (isCompleted) return;
            isCompleted = true;
            acs.SetResult();
        });

        return acs.Awaitable;
    }
}