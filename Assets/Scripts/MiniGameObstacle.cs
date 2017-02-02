using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MiniGameObstacle : Obstacle {


	public override void StartATL ()
	{
		base.StartATL ();
		transform.position += new Vector3 (0.0f, 0.0f, 0.0f); //change position. for now, must be centered
		transform.localScale *= 2.0f;
		GetComponent<MeshRenderer> ().material.color = Color.magenta;
		print ( "this is a minigame obstacle" );
	}
		
	public override void UpdateATL () 
	{
		base.UpdateATL ();
	}

	public override void OnCollide ()
	{
		base.OnCollide ();
		//Debug.Log ("hit");
		SceneManager.LoadScene ("two");
	}

//	public override bool DidCollide( Vector3 position )
//	{
//		base.DidCollide (position);
//	}
}

