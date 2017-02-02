using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController_two : MonoBehaviour //Singleton<PlayerController> 
{
	public float moveSpeed = 5.0f;
	public float jumpPower = 10.0f;
	public float maxVelocityChange = 10.0f;

	private float _movePower = 5.0f;
	private bool _grounded = true;

	private float _oldTime = 0.0f;
	private float _newTime = 0.0f;

	private Plane[] _planes;

	private GunController _gun;
	private GameObject _hand;

	private bool _seen = false;

	// Use this for initialization
	public void StartATL () 
	{
		_hand = GameObject.Find ("Hand");
	}
	
	// Update is called once per frame
	public void UpdateATL ( SplineLord_two splineLord_two ) {

		if (_gun != null) {
			if (Input.GetMouseButtonDown(0)) //Input.GetKeyDown (KeyCode.Mouse0)
			{
				print ("input");
				_gun.Use ( transform, transform ); //params: targetOne, targetTwo
			}
		}


		//if ( grounded )AxKDebugLines.AddBounds (GetComponent< MeshRenderer > ().bounds, Color.green);
		_planes = GeometryUtility.CalculateFrustumPlanes ( Camera.main );

		//pick up check
		GunController[] gunControllers = GameObject.FindObjectsOfType< GunController >();

		for (int i = 0; i < gunControllers.Length; i++) 
		{
			GunController temp = gunControllers [i];
			if (GeometryUtility.TestPlanesAABB (_planes, temp.GetComponent<SphereCollider> ().bounds)) 
			{ 

				RaycastHit hit = new RaycastHit ();
				Vector3 dir = transform.position - temp.transform.position;
				Ray ray = new Ray (temp.transform.position, dir);
				Physics.Raycast (ray, out hit, dir.magnitude);

				if (hit.collider == GetComponent<CapsuleCollider> () && Input.GetKeyDown (KeyCode.Mouse0)) 
				{
					temp.GetComponent<SphereCollider> ().enabled = false;
					_gun = temp.GetComponent< GunController > ();
					_gun.transform.position = _hand.transform.position;
					_gun.transform.forward = transform.forward;
					_gun.transform.parent = _hand.transform;
				}	
			}
		}
			

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		if (transform.position.y < -10.0f) 
		{
			SceneManager.LoadScene ("test1");
		}

		Vector3 newForward = Vector3.Normalize( GetComponent< Rigidbody >().velocity * 2.0f + transform.forward );
		newForward.y = 0.0f;
		newForward = Vector3.Normalize (newForward);
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (newForward, Vector3.up), Time.deltaTime * 4.0f);

		if (!_grounded) 
			return;

		//calculate how fast we should be moving based on user input
		Vector3 targetVelocity = new Vector3 (KeyToF (KeyCode.D) - KeyToF (KeyCode.A), 0.0f, -KeyToF (KeyCode.S)); //KeyToF (KeyCode.W) - KeyToF (KeyCode.S)
		targetVelocity = Camera.main.transform.TransformDirection (targetVelocity);

		float moveSpeedMult = KeyToF (KeyCode.LeftShift) * 2.0f + 1.0f;
		targetVelocity *= moveSpeed * moveSpeedMult;

		//apply a force that attempts to reach our target velocity
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 velocityChange = targetVelocity - velocity;
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0; 
		velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
		GetComponent<Rigidbody> ().AddForce (velocityChange, ForceMode.VelocityChange);

		float thrust = KeyToF(KeyCode.Space, true) * jumpPower;
		GetComponent<Rigidbody> ().AddForce (transform.up * thrust, ForceMode.Impulse );

		_newTime = Time.timeSinceLevelLoad;
		if ((_newTime - _oldTime) >= 0.1f) 
		{
			splineLord_two.CreateControlPoint (transform);
			_oldTime = _newTime;
		}


	
	}

	float KeyToF ( KeyCode keyCode, bool onDown = false )
	{
		if (onDown) 
		{ 
			return Input.GetKeyDown (keyCode) ? 1.0f : 0.0f; 
		} else 
		{
			return Input.GetKey (keyCode) ? 1.0f : 0.0f;
		}
	}

	void OnCollisionExit( Collision collisionInfo ) 
	{

//		_newTime = Time.timeSinceLevelLoad;
//		if ((_newTime - _oldTime) >= 0.5f) {
//			_grounded = false;
//		}
//		_oldTime = _newTime;

	}

	//	//use for sound fx
	//	void OnCollisionStay(Collision collisionInfo) {
	//		foreach (ContactPoint contact in collisionInfo.contacts) {
	//			Debug.DrawRay(contact.point, contact.normal, Color.white);
	//			Vector3 playerFootPos = transform.position - transform.up *  ( transform.GetComponent< CapsuleCollider > ().height / 2.0f );
	//			AxKDebugLines.AddSphere (playerFootPos, 0.3f, Color.red);
	//			if ( Vector3.Distance( contact.point, playerFootPos ) < 0.3f ) 
	//			{
	//				if(Vector3.Dot(contact.normal, Vector3.up) > .5)
	//				{
	//					grounded = true;	
	//				}
	//			}
	//		}
	//	}
	//

}
