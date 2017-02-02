using UnityEngine;
using System.Collections;

public class SkyController : MonoBehaviour {

	// Use this for initialization
	public void StartATL () 
	{
		//GetComponent<ReverseNormals> ().StartATL ();
		GetComponent<Noise> ().StartATL ();
	
	}
	
	// Update is called once per frame
	public void UpdateATL () 
	{
		GetComponent<Noise> ().UpdateATL ();
	
	}
}
