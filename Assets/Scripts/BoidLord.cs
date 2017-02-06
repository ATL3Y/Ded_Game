using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;

public class BoidLord : MonoBehaviour 
{

	private int flockSize = 10;
	public BoidFlocking prefab;

	public Vector3 flockCenter;
	public Vector3 flockVelocity;

	private List <BoidFlocking> _boids;

    public float neighborRadius = 15.0f;
	public float seperationStrength = 1.0f; 
	public float cohesionStrength = 1.0f; 
	public float alignmentStrength = 1.0f; 
	public float targetStrength = 1.0f;

	private float t;

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
			boid.GetComponent<BoidFlocking> ().StartATL ( i, this, player, spline, crosshair ); 
			_boids.Add( boid );
		}

    }
		
	public void UpdateATL ( ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair) 
	{
		
		if (!init && spline.IsInitialized)
		{
			init = true;
		}

		if (!init)
			return;

		Vector3 theCenter = Vector3.zero;
		Vector3 theVelocity = Vector3.zero;

		flockSize = _boids.Count;
      //  transform.position = player.transform.position + Camera.main.transform.forward * 10.0f;

        for (int i = 0; i < _boids.Count; i++) 
		{
			theCenter += _boids [i].transform.position;
			theVelocity += _boids [i].GetComponent<Rigidbody> ().velocity;
		}

		flockCenter = theCenter / flockSize;
        //AxKDebugLines.AddFancySphere(flockCenter, 1f, Color.red, 0);
		flockVelocity = theVelocity / flockSize;
        //AxKDebugLines.AddLine(flockCenter, flockCenter + flockVelocity, Color.red, 0);

		for (int i = 0; i < _boids.Count; i++) 
		{
            _boids[i].GetComponent<BoidFlocking> ().UpdateATL ( _boids, this, player, spline, crosshair );
		}
	}

	public void RemoveBoid( BoidFlocking boid )
	{
		_boids.Remove ( boid );
		Destroy ( boid.gameObject );
	}
}
