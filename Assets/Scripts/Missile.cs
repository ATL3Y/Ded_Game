using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour 
{

	private float _speed = 6.0f; //can't change params in editor without instants
	public Transform target;
	private float _targetDelay = 6.0f;
	private Color _color;

	public void UpdateATL ( ThirdPersonUserControl player_three ) 
	{
		
		_targetDelay -= Time.deltaTime; //if commented out, disabling transition to seeking the target 

		if (_targetDelay < 0.5f) // > aims at target at outset until delay time. < aims at target after delay time
		{
			target = player_three.transform;
			Quaternion targetDirection = Quaternion.LookRotation (((target.position + player_three.transform.up) - transform.position).normalized); //aim at heart
			transform.rotation = Quaternion.Slerp (transform.rotation, targetDirection, Time.deltaTime * 2.0f); 
		}
		Vector3 velocity = (transform.forward) * _speed;
		GetComponent< Rigidbody >().MovePosition( transform.position + velocity * Time.deltaTime * 2.0f );

	}

	public void StartATL ( Transform newTarget, Vector3 startDirection, Vector3 startPosition ) 
	{
		target = newTarget;
		//transform.up = transform.forward;

		gameObject.AddComponent< TrailRenderer > (); 
		GetComponent< TrailRenderer > ().material.shader = Shader.Find ("Particles/Additive"); 

		_color = new Color ();

		gameObject.tag = "Missile";
		gameObject.layer = 8; // int of "Layer 1"
		ColorUtility.TryParseHtmlString ("#FFFFFF", out _color);
		GetComponent< MeshRenderer > ().material.color = _color;
		ColorUtility.TryParseHtmlString ("#ED67FF53", out _color);
		GetComponent< TrailRenderer >().material.SetColor("_TintColor", _color);
	

		GetComponent< TrailRenderer > ().startWidth = .12f;
		GetComponent< TrailRenderer > ().endWidth = .4f;

		Rigidbody r = gameObject.AddComponent< Rigidbody > ();
		r.isKinematic = true;
		//r.useGravity = false;

		transform.localScale *= .2f; // new Vector3 (.01f, .01f, .01f);
		transform.position = startPosition;
		transform.forward = startDirection;
	}
}
