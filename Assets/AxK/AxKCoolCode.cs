using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public static class CoolExtensions
{
	public static void WaitAndCallback(this MonoBehaviour mono, float delay, Action callback)
	{
		mono.StartCoroutine(Co_WaitAndCallback(delay, callback));
	}
	
	public static IEnumerator Co_WaitAndCallback(float delay, Action callback)
	{
		yield return new WaitForSeconds(delay);
		callback();
	}

    public static void WaitAndCallback(this MonoBehaviour mono, int frames, Action callback)
    {
        mono.StartCoroutine(Co_WaitAndCallback(frames, callback));
    }

    public static IEnumerator Co_WaitAndCallback(int frames, Action callback)
    {
        while (frames > 0)
        {
            frames--;
            yield return null;
        }
        callback();
    }

	public static T InstantiateThis<T>(this T t) where T : Component
	{
		return GameObject.Instantiate(t) as T;
	}
	
	public static GameObject InstantiateThis(this GameObject t)
	{
		return GameObject.Instantiate(t) as GameObject;
	}
	
	public static void SafeCall(this Action action)
	{
		if(action != null)
		{
			action();
		}
	}
	
	public static void SafeCall<T>(this Action<T> action, T arg)
	{
		if(action != null)
		{
			action(arg);
		}
	}
	
	public static void SafeCall<T1, T2>(this Action<T1,T2> action, T1 arg1, T2 arg2)
	{
		if(action != null)
		{
			action(arg1, arg2);
		}
	}
	
	public static void SafeCall<T1, T2, T3>(this Action<T1,T2,T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		if(action != null)
		{
			action(arg1, arg2, arg3);
		}
	}
	
	public static void SafeCall<T1, T2, T3, T4>(this Action<T1,T2,T3,T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if(action != null)
		{
			action(arg1, arg2, arg3, arg4);
		}
	}
	
	public static bool IsSafeIndex<T>(this T[] array, int index)
	{
		return index >= 0 && index < array.Length;
	}
	
	public static T SafeValueOrNull<T>(this T[] array, int index) where T : class
	{
		return array.IsSafeIndex(index) ? array[index] : null;
	}
	
	public static Action ToAction(this UnityEvent unityEvent)
	{	
		return () =>
		{
			if(unityEvent != null)
			{
				unityEvent.Invoke();
			}
		};
	}
	
	public static UnityEvent ToUnityEvent(this Action action)
	{
		var unityEvent = new UnityEvent();
		unityEvent.AddListener(() => action());
		return unityEvent;
	}
	
	public static Action ToAction(this UnityAction unityAction)
	{
		return () => unityAction.SafeCall();
	}
	
	public static UnityAction ToUnityAction(this Action action)
	{
		return () => action.SafeCall();
	}
	
	public static void SafeCall(this UnityAction action)
	{
		if(action != null)
		{
			action();
		}
	}
	
	public static void SafeCall<T>(this UnityAction<T> action, T arg)
	{
		if(action != null)
		{
			action(arg);
		}
	}
	
	public static void SafeCall<T1, T2>(this UnityAction<T1,T2> action, T1 arg1, T2 arg2)
	{
		if(action != null)
		{
			action(arg1, arg2);
		}
	}
	
	public static void SafeCall<T1, T2, T3>(this UnityAction<T1,T2,T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		if(action != null)
		{
			action(arg1, arg2, arg3);
		}
	}
	
	public static void SafeCall<T1, T2, T3, T4>(this UnityAction<T1,T2,T3,T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if(action != null)
		{
			action(arg1, arg2, arg3, arg4);
		}
	}
    
    #region UI Extensions
    public static void Enable(this CanvasGroup item, bool forceAlpha = false)
    {
        item.interactable = true;
        item.blocksRaycasts = true;
        
        if (forceAlpha)
        {
            item.alpha = 1;
        }
    }
    
    public static void Disable(this CanvasGroup item, bool forceAlpha = false)
    {
        item.interactable = false;
        item.blocksRaycasts = false;
        
        if (forceAlpha)
        {
            item.alpha = 0;
        }
    }
    #endregion
}