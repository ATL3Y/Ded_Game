using UnityEngine;

public class SoundController : MonoBehaviour 
{
	public AudioSource audioSource; 
	private float _volume;

	public int qSamples = 4096;
	private float[] _samples;

	void Start () 
	{
		_samples = new float[qSamples];
	}

	void Update () 
	{
		_volume = GetRMS(0) + GetRMS(1);
		AxKDebugLines.AddFancySphere (Vector3.zero, _volume * 3.0f, Color.red, 0);
		print (_volume);
	}

	float GetRMS(int channel)
	{
		audioSource.GetOutputData(_samples, channel);  
		float sum = 0;
		foreach(float f in _samples)
		{
			sum += f*f;
		}
		return Mathf.Sqrt(sum/qSamples);
	}

	public float GetVolume()
	{
		return _volume;
	}

}

//public AudioSource audioSource;
//public float updateStep = 0.1f;
//public int sampleDataLength = 1024;
//
//private float currentUpdateTime = 0f;
//
//private float clipLoudness;
//private float[] clipSampleData;
//
//// Use this for initialization
//void Awake () 
//{
//
//	if (!audioSource) 
//	{
//		Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
//	}
//	clipSampleData = new float[sampleDataLength];
//
//}
//
//// Update is called once per frame
//void Update () 
//{
//
//	currentUpdateTime += Time.deltaTime;
//	if (currentUpdateTime >= updateStep) 
//	{
//		currentUpdateTime = 0f;
//		audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
//		clipLoudness = 0f;
//		foreach (var sample in clipSampleData) //change this 
//		{
//			clipLoudness += Mathf.Abs(sample);
//		}
//		clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
//	}
//
//	print (clipLoudness);
//
//}