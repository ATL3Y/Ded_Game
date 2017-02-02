using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;

public class BoidFlocking : MonoBehaviour
{

	private bool destroyMe = false;
	private AudioSource audioSource;

	private bool inited = false;
	private float minVelocity;
	private float maxVelocity;
	private float randomness;

	public void StartATL( BoidLord boidLord, ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair )
	{
		audioSource = GetComponent<AudioSource> ();
		StartCoroutine (BoidSteering( boidLord, player, spline, crosshair ));
	}

	IEnumerator BoidSteering ( BoidLord boidLord, ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair )
	{
		while (true) 
		{
			if (inited)
			{
				if (destroyMe) 
				{
					boidLord.RemoveBoid ( this ); 
				}

				GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + Calc ( boidLord, player, spline, crosshair ) * Time.deltaTime;

				// enforce minimum and maximum speeds for the boids
				float speed = GetComponent<Rigidbody>().velocity.magnitude;
				if (speed > maxVelocity)
				{
					GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * maxVelocity;
				}
				else if (speed < minVelocity)
				{
					GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * minVelocity;
				}
			}

			float waitTime = Random.Range(0.3f, 0.5f);
			yield return new WaitForSeconds (waitTime);
		}
	}

	private Vector3 Calc ( BoidLord boidLord, ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair )
	{
		Vector3 randomize = new Vector3 ((Random.value * 2) -1, (Random.value * 2) -1, (Random.value * 2) -1);

		randomize.Normalize();
		Vector3 flockCenter = boidLord.flockCenter;
		Vector3 flockVelocity = boidLord.flockVelocity;
		Vector3 follow = crosshair.transform.position;

		flockCenter = flockCenter - transform.localPosition;
		flockVelocity = flockVelocity - GetComponent<Rigidbody>().velocity;
		follow = follow - transform.position;

		return (flockCenter + flockVelocity + follow + randomize * randomness);
	}

	// called before start to set values 
	public void SetController ( BoidLord boidLord )
	{
		minVelocity = boidLord.minVelocity;
		maxVelocity = boidLord.maxVelocity;
		randomness = boidLord.randomness;
		inited = true;
	}

	void OnCollisionEnter (Collision collisionInfo )
	{
		if (collisionInfo.collider.CompareTag("Missile")) 
		{
			print ("bird down");
			audioSource.Play ();
			destroyMe = true;
		}
	}


	/*
	public void UpdateATL( List <BoidFlocking> _boids, BoidLord boidLord, ThirdPersonUserControl player, CurvySpline spline )
	{
		if (destroyMe) 
		{
			boidLord.RemoveBoid ( this ); 
		}
			
		Vector3 cohesion = Vector3.zero;
		Vector3 separation = Vector3.zero;
		Vector3 alignment = Vector3.zero;

		Vector3 averagePosition = Vector3.zero; //should this be the target?

		for (int i = 0; i < _boids.Count; i++) 
		{
			if (_boids [i] != gameObject) 
			{ 
				//averagePosition += _boids [i].transform.position;

				alignment += _boids [i].GetComponent<Rigidbody> ().velocity;

				float d = Vector3.Distance (transform.position, _boids [i].transform.position);
				d = Mathf.Clamp (5.0f - d, 0.0f, 5.0f) / 5.0f;

				separation += (transform.position - _boids [i].transform.position).normalized * AxKEasing.EaseOutCubic (0.0f, 1.0f, d) * 5.0f; 
			}
		}

		//averagePosition /= (_boids.Count - 1); 
		//cohesion = (averagePosition - transform.position).normalized;

		cohesion = (transform.position - boidLord.transform.position).normalized; //move towards the boidlord
		cohesion *= boidLord.cohesionStrength;

		alignment /= (_boids.Count - 1); //average all velocities 
		alignment = (alignment - GetComponent<Rigidbody> ().velocity).normalized; //normalize the difference between this velocity and the average
		alignment *= boidLord.alignmentStrength;

		//target *= boidLord.targetStrength;

		separation *= boidLord.seperationStrength;

		Vector3 random = new Vector3 ((Random.value*2)*-1, (Random.value*2)-1, (Random.value*2)-1);
		random *= boidLord.randomStrength;

		Vector3 velocity = GetComponent<Rigidbody> ().velocity + cohesion + separation + alignment; // + random + target

		Quaternion targetDirection = Quaternion.LookRotation (velocity, spline.transform.up);
		transform.rotation = Quaternion.Slerp (transform.rotation, targetDirection, Time.deltaTime * 2.0f); 

		GetComponent< Rigidbody >().AddForce( velocity, ForceMode.Acceleration );

	}
	*/
}
