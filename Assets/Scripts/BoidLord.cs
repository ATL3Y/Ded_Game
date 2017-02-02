using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;

public class BoidLord : MonoBehaviour 
{

	public float minVelocity = 1;
	public float maxVelocity = 5;
	public float randomness = 3;

	private int flockSize = 100;
	public BoidFlocking prefab;

	public Vector3 flockCenter;
	public Vector3 flockVelocity;

	private List <BoidFlocking> _boids;
	/*
	public float seperationStrength = 0.4f; //small
	public float cohesionStrength = 0.8f; //med
	public float alignmentStrength = 1.2f; //large
	public float targetStrength = 1.8f;
	public float randomStrength = 0.2f;

	public float offset = 5.5f;
	public float lift = 10.0f;
	public float smooth = 120.0f;
	private float t;
	*/

	private bool init = false;

	public void StartATL ( ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair ) 
	{ 
		
		_boids = new List<BoidFlocking>();

		for (int i = 0; i < flockSize; i++) 
		{
			Vector3 position = new Vector3 (
				Random.value * 10.0f,
				Random.value * 10.0f,
				Random.value * 10.0f
			);

			BoidFlocking boid = Instantiate (prefab, transform.position, transform.rotation) as BoidFlocking; 

			boid.transform.parent = transform;
			boid.transform.localPosition = position;
			boid.GetComponent<BoidFlocking>().SetController ( this );
			boid.GetComponent<BoidFlocking> ().StartATL ( this, player, spline, crosshair ); 
			_boids.Add( boid );
		}


	}
		
	public void UpdateATL ( ThirdPersonUserControl player, CurvySpline spline ) 
	{
		
		if (!init && spline.IsInitialized)
		{
			init = true;
		}

		if (!init)
			return;

		/*
		t = spline.GetNearestPointTF (player.transform.position);

		float targetPositionT = t - offset / spline.Length;
		Vector3 targetT = spline.Interpolate (targetPositionT);
		Vector3 targetPosition = targetT;

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth );
		transform.localRotation = Quaternion.Slerp( transform.localRotation, Quaternion.LookRotation (-splineForward.normalized), Time.deltaTime * smooth );

		transform.position = targetPosition;
		transform.localRotation = Camera.main.transform.localRotation;
		*/

		Vector3 theCenter = Vector3.zero;
		Vector3 theVelocity = Vector3.zero;

		flockSize = _boids.Count;

		transform.position = player.transform.position + Camera.main.transform.forward * 3.0f;
			
		for (int i = 0; i < _boids.Count; i++) 
		{
			theCenter += _boids [i].transform.localPosition;
			theVelocity += _boids [i].GetComponent<Rigidbody> ().velocity;
		}

		flockCenter = theCenter / flockSize;
		flockVelocity = theVelocity / flockSize;

		/*
		for (int i = 0; i < _boids.Count; i++) 
		{
			_boids[i].GetComponent<BoidFlocking> ().UpdateATL ( this, player, spline );
		}
		*/

	}

	public void RemoveBoid( BoidFlocking boid )
	{
		_boids.Remove ( boid );
		Destroy ( boid.gameObject );
	}
}
