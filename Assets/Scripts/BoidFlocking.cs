using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;

public class BoidFlocking : MonoBehaviour
{
	private bool destroyMe = false;
	private AudioSource audioSource;
    private Vector3 seed;

    Vector3 cohesion = Vector3.zero;
    Vector3 separation = Vector3.zero;
    Vector3 alignment = Vector3.zero;
    Vector3 target = Vector3.zero;
    private float colliderRadius = 100.0f;
    private int iterator = 0;

    public void StartATL( int i, BoidLord boidLord, ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair )
	{
		audioSource = GetComponent<AudioSource> ();
        seed = Random.insideUnitSphere;
        iterator = i;

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

    public void UpdateATL( List <BoidFlocking> _boids, BoidLord boidLord, ThirdPersonUserControl player, CurvySpline spline, Crosshair crosshair )
	{
		if (destroyMe) 
		{
			boidLord.RemoveBoid ( this ); 
		}

        alignment = ComputeAlignment(_boids, boidLord) * boidLord.alignmentStrength;
        cohesion = ComputeCohesion(_boids, boidLord) * boidLord.cohesionStrength;
        separation = ComputeSeparation(_boids, boidLord) * boidLord.seperationStrength;

        Vector3 wiggle = new Vector3(Mathf.Sin(Time.timeSinceLevelLoad + seed[2]), Mathf.Sin(Time.timeSinceLevelLoad + seed[1]), Mathf.Sin(Time.timeSinceLevelLoad + seed[0]));

        // add variation of target
        if (( iterator+1) % 3 == 0)
        {
            Vector3 offset = crosshair.transform.position - player.transform.position;
            target = crosshair.transform.position + offset * 5.0f + crosshair.transform.right * Mathf.Sin(Time.timeSinceLevelLoad + seed[0]) * 5.0f;
        }
        else if ((iterator+1) % 2 == 0)
        {
            float d = Vector3.Distance(player.transform.position, Camera.main.transform.position);
            target = Camera.main.transform.position + Camera.main.transform.forward * d * 7.0f + Camera.main.transform.up * d * 3.0f;
            target += Camera.main.transform.right * Mathf.Cos(Time.timeSinceLevelLoad + seed[2]) * 15.0f;
        }
        else
        {
            float t = Camera.main.GetComponent<CameraController_three>().GetPlayerT();
            //interpolate position on spline behind player
            target = spline.Interpolate(t * .6f);
            //vectors are taken from point on spline where the player is 
            target += Camera.main.GetComponent<CameraController_three>().GetSplineUp() * 8.0f;
            target += Camera.main.GetComponent<CameraController_three>().GetSplineRight() * Mathf.Sin(Time.timeSinceLevelLoad + seed[1]) * 20.0f;
        }

        target -= transform.position; // make target position a vector from this boid to that position
        Vector3 velocityChange = alignment + cohesion + separation + target; 

        GetComponent< Rigidbody >().AddForce( velocityChange * Time.deltaTime * 0.3f, ForceMode.VelocityChange ); // changed from Acceleration 
        GetComponent<Rigidbody>().drag = 0.8f;

        Quaternion look = Quaternion.LookRotation(player.transform.position - transform.position);
        GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 3.0f));
        GetComponent<Rigidbody>().AddForce( transform.forward * 0.2f, ForceMode.VelocityChange );

        //AxKDebugLines.AddLine(transform.position, transform.position + cohesion, Color.cyan, 0);
        //AxKDebugLines.AddLine(transform.position, transform.position + alignment, Color.green, 0);
        //AxKDebugLines.AddLine(transform.position, transform.position + separation, Color.yellow, 0);
        
        /*
        if ((iterator+1) % 3 == 0)
        {
            AxKDebugLines.AddLine(transform.position, transform.position + target, Color.white, 0);
        }
        else if ((iterator+1) % 2 == 0)
        {
            AxKDebugLines.AddLine(transform.position, transform.position + target, Color.red, 0);
        }
        else
        {
            AxKDebugLines.AddLine(transform.position, transform.position + target, Color.black, 0);
        }
        */

    }

    Vector3 ComputeAlignment( List <BoidFlocking> _boids, BoidLord boidLord)
    {
        int neighborCount = 0;
        Vector3 velocityChange = Vector3.zero;
        for (int i = 0; i < _boids.Count; i++)
        {
            if (_boids[i] != gameObject && Vector3.Distance(transform.position, _boids[i].transform.position) < boidLord.neighborRadius)
            {
                velocityChange += _boids[i].GetComponent<Rigidbody>().velocity;
                neighborCount++;
            }
        }

        if(neighborCount == 0)
        {
            return Vector3.zero;
        }

        velocityChange /= neighborCount;
        return velocityChange;
    }

    Vector3 ComputeCohesion(List<BoidFlocking> _boids, BoidLord boidLord)
    {
        int neighborCount = 0;
        Vector3 velocityChange = Vector3.zero;
        for (int i = 0; i < _boids.Count; i++)
        {
            if (_boids[i] != gameObject && Vector3.Distance(transform.position, _boids[i].transform.position) < boidLord.neighborRadius)
            {
                velocityChange += _boids[i].transform.position;
                neighborCount++;
            }
        }

        if (neighborCount == 0)
        {
            return Vector3.zero;
        }

        velocityChange /= neighborCount;
        velocityChange = velocityChange - transform.position;
        return velocityChange;
    }

    Vector3 ComputeSeparation(List<BoidFlocking> _boids, BoidLord boidLord)
    {
        int neighborCount = 0;
        Vector3 velocityChange = Vector3.zero;
        for (int i = 0; i < _boids.Count; i++)
        {
            if (_boids[i] != gameObject && Vector3.Distance(transform.position, _boids[i].transform.position) < boidLord.neighborRadius)
            {
                
                Vector3 toBoid = transform.position - _boids[i].transform.position; // steer away
                float strength = Mathf.Clamp01( ( boidLord.neighborRadius * 0.3f ) - toBoid.magnitude );
                toBoid = toBoid.normalized * strength;

                velocityChange += toBoid;
                neighborCount++;
            }
        }

        if (neighborCount == 0)
        {
            return Vector3.zero;
        }

        velocityChange /= neighborCount;
        return velocityChange;
    }

}
