using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class Game_three : MonoBehaviour 
{
	public bool attacker = false;

	CurvySpline spline;
	ThirdPersonUserControl player;
	ConcaveHullController concaveHullController;
	CameraController_three cameraController_three;
	SkyController skyController;
	AttackController attackerController;
	GunController gunController;
	List <MakeMesh> makeMesh = new List<MakeMesh>();
	Crosshair crosshair;
	BigTextController bigText;
	BoidLord boidlord;

	void Start () 
	{ 
		
		spline = GameObject.FindObjectOfType<CurvySpline> ();

		player = GameObject.FindObjectOfType< ThirdPersonUserControl > ();
		player.StartATL ( spline );

		concaveHullController = GameObject.FindObjectOfType<ConcaveHullController> ();
		concaveHullController.StartATL ( player, spline );

		skyController = GameObject.FindObjectOfType<SkyController> ();
		skyController.StartATL ();

		if (attacker) 
		{
			attackerController = GameObject.FindObjectOfType<AttackController> ();
			attackerController.StartATL ();
		}

		gunController = GameObject.FindObjectOfType<GunController> ();
		gunController.StartATL ();

		//print ("tryingn to make mesh in game");
		foreach (MakeMesh mesh in Resources.FindObjectsOfTypeAll(typeof(MakeMesh)) as MakeMesh[]) 
		{
			makeMesh.Add (mesh);
		}

		for (int i = 0; i < makeMesh.Count; i++) 
		{
			makeMesh[i].StartATL ();
		}

		cameraController_three = GameObject.FindObjectOfType< CameraController_three > ();
		cameraController_three.StartATL ( player, spline );

		crosshair = GameObject.FindObjectOfType<Crosshair> ();
		crosshair.StartATL ( player, spline );

		bigText = GameObject.FindObjectOfType<BigTextController> ();
		bigText.StartATL ( player, spline );

		boidlord = GameObject.FindObjectOfType<BoidLord> ();
		boidlord.StartATL (player, spline, crosshair);

	}

	void Update () 
	{
		player.UpdateATL ( spline );

		concaveHullController.UpdateATL ( player, spline );
		
		skyController.UpdateATL ();

		if (attacker) 
		{
			attackerController.UpdateATL ( player );
		}

		gunController.UpdateATL ( player, crosshair );

		for (int i = 0; i < makeMesh.Count; i++) 
		{
			makeMesh[i].UpdateATL ();
		}

		cameraController_three.UpdateATL ( player, spline ); 

		crosshair.UpdateATL ( player, spline );

		bigText.UpdateATL ( player, spline );

		boidlord.UpdateATL (player, spline);

	}
}
