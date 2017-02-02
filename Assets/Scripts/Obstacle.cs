using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;

public class Obstacle : MonoBehaviour {


	public void Start()
	{
//		FindObjectOfType< ObstacleLord >().RegisterObject( this );
	}

	// Use this for initialization
	public virtual void StartATL ( )
	{

	}
	
	// Update is called once per frame
	public virtual void UpdateATL () 
	{
		
	}

	public virtual void OnCollide()
	{
			
	}

	public virtual bool DidCollide( Vector3 position )
	{
		//Debug.Log(position);
	
		if (GetComponent<MeshRenderer> ().bounds.Contains (position)) 
		{
			return true;
		}

		return false;
		
	}

}
