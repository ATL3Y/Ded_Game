using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;

public class Missile : MonoBehaviour 
{

	private float _speed = 100.0f; 
	private float _targetDelay = 3.0f;
	private Color _color;
    private bool onceAround = false;

	public void UpdateATL ( ThirdPersonUserControl player_three, Crosshair crosshair, CurvySpline spline) 
	{
		
		_targetDelay -= Time.deltaTime;

        // aim through crosshair 
        Quaternion targetDirection = Quaternion.LookRotation(crosshair.transform.forward);

        // passed the crosshair and delay
        if (_targetDelay < 0.5f) 
		{
            if(_speed > 10.0f)
            {
                _speed -= Time.deltaTime;
            }

            if(Vector3.Distance(gameObject.transform.position, player_three.transform.position) < 5.0f)
            {
                onceAround = true;
            }

            // going around track
            if ( !onceAround )
            {

                float t = spline.GetNearestPointTF(gameObject.transform.position);
                t *= .95f;
                Vector3 splineUp = spline.GetRotatedUpFast(t, 0);
                Vector3 targetPosition = spline.Interpolate(t) + splineUp * 1.7f;

                targetDirection = Quaternion.LookRotation((targetPosition - transform.position).normalized);
            }
            // circling player - should be the gun
            else if (onceAround)
            {
                targetDirection = Quaternion.LookRotation(((player_three.transform.position + player_three.transform.up) - transform.position).normalized); //aim at heart
            }
        } 

        transform.rotation = Quaternion.Slerp(transform.rotation, targetDirection, Time.deltaTime);
        Vector3 velocity = (transform.forward) * _speed;
		GetComponent< Rigidbody >().MovePosition( transform.position + velocity * Time.deltaTime);

	}

	public void StartATL ( Vector3 startDirection, Vector3 startPosition ) 
	{

		gameObject.AddComponent< TrailRenderer > (); 
		GetComponent< TrailRenderer > ().material.shader = Shader.Find ("Particles/Additive"); 

		_color = new Color ();

		gameObject.tag = "Missile";
		gameObject.layer = 8; // int of "Layer 1"
		ColorUtility.TryParseHtmlString ("#FFFFFF", out _color);
		GetComponent< MeshRenderer > ().material.color = _color;
		ColorUtility.TryParseHtmlString ("#ED67FF53", out _color);
		GetComponent< TrailRenderer >().material.SetColor("_TintColor", _color);
	
		GetComponent< TrailRenderer > ().startWidth = .06f;
		GetComponent< TrailRenderer > ().endWidth = .2f;

		Rigidbody r = gameObject.AddComponent< Rigidbody > ();
		r.isKinematic = true;
		r.useGravity = false;
        r.angularDrag = 0.0f;

		transform.localScale *= .2f; // new Vector3 (.01f, .01f, .01f);
		transform.position = startPosition;
		transform.forward = startDirection;
	}
}
