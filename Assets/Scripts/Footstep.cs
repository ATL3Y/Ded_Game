using UnityEngine;
using System.Collections;

public class Footstep : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter ( Collider other )
	{
        //print (other.gameObject.name);
		if (other.gameObject.transform.parent.gameObject != null && other.gameObject.transform.parent.gameObject.tag == "Terrain") 
		{
			GetComponent<ParticleSystem> ().Emit (1);
			GetComponent<AudioSource> ().Play ();
		}
	}
}
