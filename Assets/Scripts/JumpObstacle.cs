using UnityEngine;
using System.Collections;

public class JumpObstacle : Obstacle {
	

	public override void StartATL ()
	{
		base.StartATL ();
	}

	public override void UpdateATL () 
	{
		base.UpdateATL ();
	
	}

	public override void OnCollide ()
	{
		base.OnCollide ();
		Debug.Log ("hit");
	}

//	public override bool DidCollide( Vector3 position )
//	{
//		base.DidCollide (position);
//	}
}
