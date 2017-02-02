using UnityEngine;
using System.Collections;

public class Game_one : MonoBehaviour 
{

	SplineLord_one splineLord_one;
	PlayerController_one player_one;
	ObstacleLord obstacleLord;
	CameraController_one cameraController_one;
	BookController bookController;


	// Use this for initialization
	void Start () 
	{

		splineLord_one = GameObject.FindObjectOfType< SplineLord_one > ();
		splineLord_one.StartATL ();

		player_one = GameObject.FindObjectOfType< PlayerController_one > ();
		player_one.StartATL ();

		obstacleLord = GameObject.FindObjectOfType< ObstacleLord > ();
		obstacleLord.StartATL ();

		cameraController_one = GameObject.FindObjectOfType< CameraController_one> ();
		cameraController_one.StartATL ();

		bookController = GameObject.FindObjectOfType<BookController> ();
		bookController.StartATL (splineLord_one);

	}
	
	// Update is called once per frame
	void Update () 
	{
		splineLord_one.UpdateATL ();
		player_one.UpdateATL (splineLord_one, bookController);
		obstacleLord.UpdateATL (player_one, splineLord_one );
		cameraController_one.UpdateATL (player_one);
		bookController.UpdateATL (player_one, splineLord_one);

	}
}
