using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LightController : MonoBehaviour 
{

	private List <Light> _lights;
	private float _lightStep = 0.003f;
	private bool once = false;
	private float intensity = 0.0f;
	private Color color;
	public bool begun = false;
	public bool towards = false;

	// Use this for initialization
	void Start () 
	{
		_lights = new List<Light>();
		RenderSettings.skybox.SetFloat ("_Exposure", 0.0f);

		ColorUtility.TryParseHtmlString ("00FFFFFF", out color);
		//print ("color = " + color);

		foreach (Light light in GameObject.FindObjectsOfType<Light>()) 
		{
			Light l = light;
			l.color = Color.cyan + Color.white;
			_lights.Add (l);
			l.intensity = 0.0f;
			//print ("l color in start = " + l.color);
		}

		//print ("count = " + _lights.Count);

		StartCoroutine (Bright( 1.0f, 0.01f, 0.0f ));
	}
	//do custom update so this doesn't start right away? or bool to begin? 
	void Update () 
	{
		if (towards && begun) 
		{
			StartCoroutine (Bright( .9f, 0.05f, 0.0f ));
		}
		else if (!towards && begun)  
		{
			StartCoroutine (Dark ( .5f, 0.05f, 0.0f ));
		}
	}

	IEnumerator Bright ( float limit, float timeStep, float delay )
	{
		yield return new WaitForSeconds( delay );

		while (intensity <= limit ) 
		{
			intensity += _lightStep;

			for (int i = 0; i < _lights.Count; i++) 
			{
				_lights [i].intensity = intensity / ( i * 4 + 2 );
				//ColorUtility.TryParseHtmlString ("#4499BB5B", out color);
				//_lights [i].color = color;
				RenderSettings.skybox.SetFloat ("_Exposure", intensity);
				//print ("l color in Bright = " + _lights [i].color);
			}

			yield return new WaitForSeconds ( timeStep );
		}
	}

	IEnumerator Dark ( float limit, float timeStep, float delay )
	{
		yield return new WaitForSeconds( delay );

		while ( intensity >= limit ) 
		{
			intensity -= _lightStep;

			for (int i = 0; i < _lights.Count; i++) 
			{
				_lights [i].intensity = intensity;
				//ColorUtility.TryParseHtmlString ("#A9FFF3", out color);
				//_lights [i].color = Color.cyan + Color.white;
				RenderSettings.skybox.SetFloat ("_Exposure", intensity);
			}

			yield return new WaitForSeconds ( timeStep );
		}
	}

	public void End()
	{
		if ( !once ) 
		{
			StartCoroutine (Dark ( 0.01f, 0.5f, 1.0f ));
		}
	}
}
