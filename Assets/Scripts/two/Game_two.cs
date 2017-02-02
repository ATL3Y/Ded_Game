using UnityEngine;
using System.Collections;

public class Game_two: MonoBehaviour 
{

	SplineLord_two splinelord_two;
	PlayerController_two player_two;
	CameraController_two cameraController_two;

	void Start () 
	{ 
		splinelord_two = GameObject.FindObjectOfType< SplineLord_two > ();
		splinelord_two.StartATL ();

		player_two = GameObject.FindObjectOfType< PlayerController_two > ();
		player_two.StartATL ();

		cameraController_two = GameObject.FindObjectOfType< CameraController_two > ();
		cameraController_two.StartATL ();

	}

	void Update () 
	{
		splinelord_two.UpdateATL ();
		player_two.UpdateATL ( splinelord_two );
		cameraController_two.UpdateATL ( player_two ); 
	}
}
