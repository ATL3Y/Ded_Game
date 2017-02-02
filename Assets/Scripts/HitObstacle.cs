using UnityEngine;
using System.Collections;

public class HitObstacle : Obstacle {

	/*
	// Use this for initialization
	public override void StartATL () {
	
	}
	
	// Update is called once per frame
	public override void UpdateATL () {
	
	}
	*/

	public override void OnCollide()
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			AxKDebugLines.AddSphere( transform.position, 0.8f, Color.green, 1 );
		}

	}

}
