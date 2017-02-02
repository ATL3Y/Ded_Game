using System;
using UnityEngine;
using System.Collections;

public static class AxKEasing
{

	public static Vector3 Ease( Vector3 start, Vector3 finish, float time )
	{
		Vector3 dir = finish - start;
		return start + dir * time;
	}

    public static Quaternion Ease(Quaternion start, Quaternion finish, float time)
    {
        // TODO later replace with unbounded quaternion lerp
        return Quaternion.Slerp(start, finish, time);
    }

	public static float Linear(float start, float end, float value)
	{
		return start + (end - start) * value;
	}
	
	public static float Clerp(float start, float end, float value)
	{
		float min = 0.0f;
		float max = 360.0f;
		float half = Mathf.Abs((max - min) / 2.0f);
		float retval = 0.0f;
		float diff = 0.0f;
		if ((end - start) < -half){
			diff = ((max - start) + end) * value;
			retval = start + diff;
		}else if ((end - start) > half){
			diff = -((max - end) + start) * value;
			retval = start + diff;
		}else retval = start + (end - start) * value;
		return retval;
	}
	
	public static float Spring(float start, float end, float value){
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}
	
	public static float EaseInQuad(float start, float end, float value){
		end -= start;
		return end * value * value + start;
	}
	
	public static float EaseOutQuad(float start, float end, float value){
		end -= start;
		return -end * value * (value - 2) + start;
	}
	
	public static float EaseInOutQuad(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value + start;
		value--;
		return -end / 2 * (value * (value - 2) - 1) + start;
	}
	
	public static float EaseInCubic(float start, float end, float value){
		end -= start;
		return end * value * value * value + start;
	}
	
	public static float EaseOutCubic(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}
	
	public static float EaseInOutCubic(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value + 2) + start;
	}
	
	public static float EaseInQuart(float start, float end, float value){
		end -= start;
		return end * value * value * value * value + start;
	}
	
	public static  float EaseOutQuart(float start, float end, float value){
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}
	
	public static  float EaseInOutQuart(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value * value + start;
		value -= 2;
		return -end / 2 * (value * value * value * value - 2) + start;
	}
	
	public static  float EaseInQuint(float start, float end, float value){
		end -= start;
		return end * value * value * value * value * value + start;
	}
	
	public static  float EaseOutQuint(float start, float end, float value){
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}
	
	public static  float EaseInOutQuint(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value * value * value + 2) + start;
	}
	
	public static  float EaseInSine(float start, float end, float value){
		end -= start;
		return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
	}
	
	public static  float EaseOutSine(float start, float end, float value){
		end -= start;
		return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
	}
	
	public static  float EaseInOutSine(float start, float end, float value){
		end -= start;
		return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
	}
	
	public static  float EaseInExpo(float start, float end, float value){
		end -= start;
		return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
	}
	
	public static  float EaseOutExpo(float start, float end, float value){
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
	}
	
	public static  float EaseInOutExpo(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}
	
	public static  float EaseInCirc(float start, float end, float value){
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}
	
	public static  float EaseOutCirc(float start, float end, float value){
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}
	
	public static  float EaseInOutCirc(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}
	
	/* GFX47 MOD START */
	public static  float EaseInBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		return end - EaseOutBounce(0, end, d-value) + start;
	}
	/* GFX47 MOD END */
	
	/* GFX47 MOD START */
	//private float bounce(float start, float end, float value){
	public static  float EaseOutBounce(float start, float end, float value){
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)){
			return end * (7.5625f * value * value) + start;
		}else if (value < (2 / 2.75f)){
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		}else if (value < (2.5 / 2.75)){
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		}else{
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}
	/* GFX47 MOD END */
	
	/* GFX47 MOD START */
	public static  float EaseInOutBounce(float start, float end, float value){
		end -= start;
		float d = 1f;
		if (value < d/2) return EaseInBounce(0, end, value*2) * 0.5f + start;
		else return EaseOutBounce(0, end, value*2-d) * 0.5f + end*0.5f + start;
	}
	/* GFX47 MOD END */
	
	public static  float EaseInBack(float start, float end, float value){
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}
	
	public static  float EaseOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value = (value / 1) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}
	
	public static  float EaseInOutBack(float start, float end, float value){
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1){
			s *= (1.525f);
			return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}
	
	public static  float Punch(float amplitude, float value){
		float s = 9;
		if (value == 0){
			return 0;
		}
		if (value == 1){
			return 0;
		}
		float period = 1 * 0.3f;
		s = period / (2 * Mathf.PI) * Mathf.Asin(0);
		return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
	}
	
	/* GFX47 MOD START */
	public static  float EaseInElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
		}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}		
	/* GFX47 MOD END */
	
	/* GFX47 MOD START */
	//private float elastic(float start, float end, float value){
	public static  float EaseOutElastic(float start, float end, float value){
		/* GFX47 MOD END */
		//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
		}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		
	
	/* GFX47 MOD START */
	public static  float EaseInOutElastic(float start, float end, float value){
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d/2) == 2) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p / 4;
		}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}		
	/* GFX47 MOD END */
	
	
	// jacob mod, copying from iTween
	public enum EaseType
	{
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		Linear,
		Spring,
		/* GFX47 MOD START */
		//bounce,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce,
		/* GFX47 MOD END */
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		/* GFX47 MOD START */
		//elastic,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic,
		/* GFX47 MOD END */
	}
	
	public static float Evaluate(this EaseType easeType, float t)
	{
		return Ease(0f, 1f, t, easeType);
	}
	
	public static float Evaluate(this EaseType easeType, float start, float end, float t)
	{
		return Ease(start, end, t, easeType);
	}
	
	public static float Ease(float start, float end, float t, EaseType easeType)
	{
		var easeFunc = GetEasingFunction(easeType) ?? Linear;
		return easeFunc(start, end, t);
	}
	
	public static Func<float,float,float,float> GetEasingFunction(EaseType easeType)
	{
		switch (easeType)
		{
			case EaseType.EaseInQuad:
				return EaseInQuad;
			case EaseType.EaseOutQuad:
				return EaseOutQuad;
				
			case EaseType.EaseInOutQuad:
				return EaseInOutQuad;
				
			case EaseType.EaseInCubic:
				return EaseInCubic;
				
			case EaseType.EaseOutCubic:
				return EaseOutCubic;
				
			case EaseType.EaseInOutCubic:
				return EaseInOutCubic;
				
			case EaseType.EaseInQuart:
				return EaseInQuart;
				
			case EaseType.EaseOutQuart:
				return EaseOutQuart;
				
			case EaseType.EaseInOutQuart:
				return EaseInOutQuart;
				
			case EaseType.EaseInQuint:
				return EaseInQuint;
				
			case EaseType.EaseOutQuint:
				return EaseOutQuint;
				
			case EaseType.EaseInOutQuint:
				return EaseInOutQuint;
				
			case EaseType.EaseInSine:
				return EaseInSine;
				
			case EaseType.EaseOutSine:
				return EaseOutSine;
				
			case EaseType.EaseInOutSine:
				return EaseInOutSine;
				
			case EaseType.EaseInExpo:
				return EaseInExpo;
				
			case EaseType.EaseOutExpo:
				return EaseOutExpo;
			case EaseType.EaseInOutExpo:
				return EaseInOutExpo;
			case EaseType.EaseInCirc:
				return EaseInCirc;
			case EaseType.EaseOutCirc:
				return EaseOutCirc;
			case EaseType.EaseInOutCirc:
				return EaseInOutCirc;
			case EaseType.Linear:
				return Linear;
			case EaseType.Spring:
				return Spring;
			/* GFX47 MOD START */
			/*case AxKEaseType.bounce:
				return bounce;
				break;*/
			case EaseType.EaseInBounce:
				return EaseInBounce;
			case EaseType.EaseOutBounce:
				return EaseOutBounce;
			case EaseType.EaseInOutBounce:
				return EaseInOutBounce;
			/* GFX47 MOD END */
			case EaseType.EaseInBack:
				return EaseInBack;
				
			case EaseType.EaseOutBack:
				return EaseOutBack;
				
			case EaseType.EaseInOutBack:
				return EaseInOutBack;
				
			/* GFX47 MOD START */
			/*case AxKEaseType.elastic:
				return elastic;
				break;*/
			case EaseType.EaseInElastic:
				return EaseInElastic;
				
			case EaseType.EaseOutElastic:
				return EaseOutElastic;
				
			case EaseType.EaseInOutElastic:
				return EaseInOutElastic;
				
			/* GFX47 MOD END */
		}
		return null;
	}
	
	private static MonoBehaviour _instance;
	public static MonoBehaviour Instance
	{
		get 
		{ 
			if(_instance == null) 
			{
				 _instance = new GameObject("EasingInstance").AddComponent<MonoBehaviour>();
				 GameObject.DontDestroyOnLoad(_instance);
		 	}
			return _instance;
		}
	}

    public static Coroutine EaseLocalRotationFrom(this Transform target, Quaternion start, float length, EaseType easeType,
        Action onComplete = null)
    {
        return EaseLocalRotation(target, start, target.localRotation, length, easeType, onComplete);
    }

    public static Coroutine EaseLocalRotationTo(this Transform target, Quaternion end, float length, EaseType easeType,
        Action onComplete = null)
    {
        return EaseLocalRotation(target, target.localRotation, end, length, easeType, onComplete);
    }

    public static Coroutine EaseLocalRotation(this Transform target, Quaternion start, Quaternion end, float length, EaseType easeType,
        Action onComplete = null)
    {
        return RunEase(target.GetEasingMono(), start, end, length, easeType, rotation =>
        {
            target.localRotation = rotation;
        }, onComplete);
    }
	
	public static Coroutine EaseVolumeFrom(this AudioSource source, float start, float length, EaseType easeType, Action onComplete = null)
	{
		return EaseVolume(source, start, source.volume, length, easeType, onComplete);
	}
	
	public static Coroutine EaseVolumeTo(this AudioSource source, float end, float length, EaseType easeType, Action onComplete = null)
	{
		return EaseVolume(source, source.volume, end, length, easeType, onComplete);
	}
	
	public static Coroutine EaseVolume(this AudioSource source, float start, float end, float length, EaseType easeType, Action onComplete = null)
	{
		return RunEase(source.GetEasingMono(), start, end, length, easeType, amount =>
		{
			source.volume = amount;
		}, onComplete);
	}
	
	public static Coroutine EasePositionFrom(this Transform target, Vector3 start, float length, EaseType easeType, Action onComplete = null)
	{
		return EasePosition(target, start, target.position, length, easeType, onComplete);
	} 
	
	public static Coroutine EasePositionTo(this Transform target, Vector3 end, float length, EaseType easeType, Action onComplete = null)
	{
		return EasePosition(target, target.position, end, length, easeType, onComplete);
	} 
	
	public static Coroutine EasePosition(this Transform target, Vector3 start, Vector3 end, float length, EaseType easeType, Action onComplete = null)
	{
		return RunEase(target.GetEasingMono(), start, end, length, easeType, position =>
		{
			target.position = position;
		}, onComplete);
	}
	
	public static Coroutine EaseLocalPositionFrom(this Transform target, Vector3 start, float length, EaseType easeType, Action onComplete = null)
	{
		return EaseLocalPosition(target, start, target.localPosition, length, easeType, onComplete);
	} 
	
	public static Coroutine EaseLocalPositionTo(this Transform target, Vector3 end, float length, EaseType easeType, Action onComplete = null)
	{
		return EaseLocalPosition(target, target.localPosition, end, length, easeType, onComplete);
	} 
	
	public static Coroutine EaseLocalPosition(this Transform target, Vector3 start, Vector3 end, float length, EaseType easeType, Action onComplete = null)
	{
		return RunEase(target.GetEasingMono(), start, end, length, easeType, position =>
		{
			target.localPosition = position;
		}, onComplete);
	}
	
	public static Coroutine EaseLocalScaleFrom(this Transform target, Vector3 start, float length, EaseType easeType, Action onComplete = null)
	{
		return EaseLocalScale(target, start, target.localScale, length, easeType, onComplete);
	}
	
	public static Coroutine EaseLocalScaleTo(this Transform target, Vector3 end, float length, EaseType easeType, Action onComplete = null)
	{
		return EaseLocalScale(target, target.localScale, end, length, easeType, onComplete);
	}
	
	public static Coroutine EaseLocalScale(this Transform target, Vector3 start, Vector3 end, float length, EaseType easeType, Action onComplete = null)
	{
		return RunEase(target.GetEasingMono(), start, end, length, easeType, scale =>
		{
			target.localScale = scale;
		}, onComplete);
	}

    public static Coroutine RunEase(this MonoBehaviour mono, Quaternion start, Quaternion end, float length,
        EaseType easeType, Action<Quaternion> easeApplyAction, Action onComplete = null)
    {
        return RunEase(mono, start, end, length, Ease, easeType, easeApplyAction, onComplete);
    }

	public static Coroutine RunEase(this MonoBehaviour mono, Vector3 start, Vector3 end, float length, EaseType easeType, Action<Vector3> easeApplyAction, Action onComplete = null)
	{
		return RunEase(mono, start, end, length, Ease, easeType, easeApplyAction, onComplete);
	}
	
	public static Coroutine RunEase(this MonoBehaviour mono, float start, float end, float length, EaseType easeType, Action<float> easeApplyAction, Action onComplete = null)
	{
		return RunEase(mono, start, end, length, Linear, easeType, easeApplyAction, onComplete);
	}
	
	public static Coroutine RunEase<T>(this MonoBehaviour mono, T start, T end, float length, Func<T,T,float,T> lerpFunc, EaseType easeType, Action<T> easeApplyAction, Action onComplete = null)
	{
		return mono.StartCoroutine(Co_RunEase(start, end, length, lerpFunc, GetEasingFunction(easeType), easeApplyAction, onComplete ));
	}
	
	public static IEnumerator Co_RunEase<T>(T start, T end, float length, Func<T,T,float,T> lerpFunc, Func<float,float,float,float> easeFunc, Action<T> easeApplyAction, Action onComplete = null)
	{
		float t = 0f;
		easeApplyAction(start);
		yield return null;
		
		t += Time.deltaTime;
		while(t < length)
		{
			float tempT = easeFunc(0f, 1f, t / length);
			easeApplyAction(lerpFunc(start, end, tempT));
			yield return null;
			t += Time.deltaTime;
		}
		easeApplyAction(end);
		
		if(onComplete != null)
		{
			onComplete();
		}
	}
	
	public static void StopEase(this Component component, Coroutine coroutine)
	{
		component.GetEasingMono().StopCoroutine(coroutine);
	}
	
	public static void StopEase(this MonoBehaviour mono, Coroutine coroutine)
	{
		mono.StopCoroutine(coroutine);
	}
	
	public static AxKEaseHelper GetEasingMono(this Component comp)
	{
		return comp.gameObject.GetEasingMono();
	}
	
	public static AxKEaseHelper GetEasingMono(this GameObject gobj)
	{
		return gobj.AddMissingComponent<AxKEaseHelper>();
	}
	
	public static T AddMissingComponent<T>(this GameObject gobj) where T : Component
	{
		var t = gobj.GetComponent<T>();
		if(t != null)
		{
			return t;
		}
		return gobj.AddComponent<T>();
	}
	
	// end jacob mod
}