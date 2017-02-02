using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class ObstacleLord : MonoBehaviour {

	private List < Obstacle > _obstacles = new List < Obstacle >();
	float timer = 0.0f;
	private int _count = 0;

	public void StartATL ()
	{
		
	}

	public void UpdateATL (PlayerController_one player_one, SplineLord_one splineLord_one ) 
	{

		for (int i = 0; i < _obstacles.Count; i++) 
		{
			_obstacles [i].UpdateATL ();

			if ( _obstacles[i].DidCollide(player_one.transform.position) )
			{
				print ("collision"); //collision happening multiple times
				_obstacles[ i ].OnCollide();
			}
		}

		//print (player.t);

		if ( player_one.t > .15f && player_one.t <= .25f && _count < 1 ) 
		{
			_count++; //create only one obstacle at this point ... would be easier to base this off markers in the text - TAGS - find what t will be at TAG
			CreateObstacle(splineLord_one.GetSpline());
		}

		timer += Time.deltaTime;

		if (timer > 5.0f) 
		{
			timer = 0.0f;
			//CreateObstacle(splineLord.GetSpline());

		}
	}

	public void RegisterObject (Obstacle obstacleToAdd)
	{
		_obstacles.Add(obstacleToAdd);
	}

//	public Object[] prefabs;
	public void CreateObstacle(CurvySpline curSpline)
	{
		GameObject newGameObject = GameObject.CreatePrimitive (PrimitiveType.Cube); //create GO, add mesh filter with cube mesh and adds mesh renderer

		newGameObject.transform.position = curSpline.Interpolate (0.3f);  // place obstacle ahead of player 
	
		//Obstacle obstacle = newGameObject.AddComponent<JumpObstacle>();
		Obstacle obstacle = newGameObject.AddComponent<MiniGameObstacle>();

		obstacle.StartATL ();

		RegisterObject (obstacle);

		print ("cube");
	}
}
