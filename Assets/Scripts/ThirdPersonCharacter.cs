using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonCharacter : MonoBehaviour
{
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 2f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = .5f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.2f; // increase from .12 for no jump at seams 

	Rigidbody m_Rigidbody;
	Animator m_Animator;
	bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;

	private float m_RightHandPositionWeight = 0.0f;
	public GameObject m_Crosshair;

	//to use gun(s) 
	private Plane[] _planes;
	private GunController _gun;
	private GameObject _hand;

	private Color _color = Color.white;
	public bool m_pickup = false;
	public bool m_doneAim = false;
	public bool m_fireFlag = false;

	private int count = 0;

	void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;

		// identify where the gun will go
		_hand = GameObject.Find ("GunHolder");
	}
		
	public void Move(Vector3 move, bool crouch, bool jump, bool aim, bool fire, bool pickup) 
	{
		// convert the world relative moveInput vector into a local-relative turn amount and forward amount required to head in the desired direction.
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);
		CheckGroundStatus();
		move = Vector3.ProjectOnPlane(move, m_GroundNormal);

		if ( m_doneAim ) //|| m_doneFire ) //prolly trigger doneAim at end of animation, or have this been a "whole animation" parameter
		{
			m_TurnAmount = 0.0f;
			Vector3 ourForward = transform.forward;
			ourForward.y = 0.0f;
			ourForward = Vector3.Normalize (ourForward);
			ourForward = Quaternion.AngleAxis (28, Vector3.up) * ourForward; // set to forward of the gun

			Vector3 dirToCrossHair = Vector3.Normalize ( m_Crosshair.transform.position - transform.position );
			dirToCrossHair.y = 0.0f;
			dirToCrossHair = Vector3.Normalize (dirToCrossHair);

			//AxKDebugLines.AddLine ( transform.position,  );
			float dot = Vector3.Dot (ourForward, dirToCrossHair);
			m_TurnAmount = dot * 30;

			//dot = Mathf.Abs(dot);
			//float acos = Mathf.Acos (dot);
			//print( m_TurnAmount );
			//float angle = acos * 180 / Mathf.PI;
			//angle += -65; //offset from shoot animation 
//			Quaternion targetRotation = transform.rotation.ToAngleAxis (angle, Vector3.up); 
//			Quaternion targetRotation = Quaternion.LookRotation (m_Crosshair.transform.position - transform.position, Vector3.up);
//			targetRotation *= Quaternion.AngleAxis (65, Vector3.up);
//			Quaternion.Slerp (transform.rotation, targetRotation, Time.time * 2.0f );
		} 
		else 
		{
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
		}
			
		m_ForwardAmount = move.z;

		ApplyExtraTurnRotation();

		// control and velocity handling is different when grounded and airborne:
		if (m_IsGrounded)
		{
			HandleGroundedMovement(crouch, jump); 
		}
		else
		{
			HandleAirborneMovement();
		}

		ScaleCapsuleForCrouching(crouch);
		PreventStandingInLowHeadroom();

		// send input and other state parameters to the animator
		UpdateAnimator(move, aim, fire);

		if (_gun != null & fire) 
		{
			_gun.Use ( m_Crosshair.transform, transform ); //simply send "transform" for it to seek player over time
		}

		if (pickup && !m_pickup) 
		{
			PickupGun();
			m_pickup = true; //can only pickup once
		}

		//AxKDebugLines.AddFancySphere (this.transform.position + this.transform.forward * 2.0f, 0.2f, _color, 0);
	}

	void ScaleCapsuleForCrouching(bool crouch)
	{
		if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Crouching = true;
		}
		else
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
				return;
			}
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Crouching = false;
		}
	}

	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
		}
	}
		
	void UpdateAnimator(Vector3 move, bool aim, bool fire)
	{
		// update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		m_Animator.SetBool("Crouch", m_Crouching);
		m_Animator.SetBool("OnGround", m_IsGrounded);

		if (!m_IsGrounded)
		{
			m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
		}

		if (m_pickup) 
		{
			if (aim && !m_doneAim) //refactor: need m_doneAim anymore?
			{
				m_Animator.SetTrigger ("Aim");
				m_Crosshair.GetComponent<Crosshair> ().aim = true;
				m_doneAim = true;
				_color = Color.blue;
				//print ("aim = (T)" + aim ); //m_doneAim);
			}
			else if (!aim && m_doneAim)
			{
				m_doneAim = false;
				m_Crosshair.GetComponent<Crosshair> ().aim = false;
				_color = Color.white;
				m_RightHandPositionWeight = 0.0f;
				//print ("aim = (F)" + aim ); //m_doneAim);
			}
				
			//print ("is the current state named Fire_Reset? " + m_Animator.GetCurrentAnimatorStateInfo (-1).IsName("Fire_Reset"));

			if (fire) 
			{
				count++;
			} 
			//print ("count1 = " + count);
			if (count > 0 && !m_Animator.GetBool ("Fire")) 
			{
				m_fireFlag = true;
				m_Animator.SetBool ("Fire", true);
				m_Crosshair.GetComponent<Crosshair> ().fire = true;
				_color = Color.red;
				count--;
			}
		}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;

		if (m_IsGrounded)
		{
			m_Animator.SetFloat("JumpLeg", jumpLeg);
		}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (m_IsGrounded && move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
	}

	void FireIsFalse()
	{
		//print ("FireIsFalse called");
		m_Animator.SetBool ("Fire", false);
	}

	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);

		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}
		
	void HandleGroundedMovement(bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))  
		{
			//jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
			m_IsGrounded = false;
			m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}
		
	public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (m_IsGrounded && Time.deltaTime > 0)
		{
			Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = v;
		}
	}

	void OnAnimatorIK(int layerIndex) 
	{
		if (m_doneAim) 
		{
			m_RightHandPositionWeight = Mathf.Clamp01 ( m_RightHandPositionWeight + Time.deltaTime / 6.0f );
		} 
		else 
		{
			m_RightHandPositionWeight = Mathf.Clamp01( m_RightHandPositionWeight - Time.deltaTime / 6.0f );
		}
			
		m_Animator.SetIKPositionWeight( AvatarIKGoal.RightHand, m_RightHandPositionWeight );
		m_Animator.SetIKPosition( AvatarIKGoal.RightHand, m_Crosshair.transform.position );
		//print ("in IK " + m_RightHandPositionWeight);
	}

	void PickupGun()
	{
		_planes = GeometryUtility.CalculateFrustumPlanes ( Camera.main );

		GunController[] gunControllers = GameObject.FindObjectsOfType< GunController >();

		for (int i = 0; i < gunControllers.Length; i++) 
		{
			//print ("in for");
			GunController temp = gunControllers [i];

			if (GeometryUtility.TestPlanesAABB (_planes, temp.GetComponent<SphereCollider> ().bounds)) 
			{ 
				//print ("in testplanes");
				RaycastHit hit = new RaycastHit ();
				Vector3 dir = transform.position - temp.transform.position;
				Ray ray = new Ray (temp.transform.position, dir);
				Physics.Raycast (ray, out hit, dir.magnitude);

				if (hit.collider == GetComponent<CapsuleCollider> ()) 
				{
					temp.GetComponent<SphereCollider> ().enabled = false;
					_gun = temp.GetComponent< GunController > ();
					_gun.transform.SetParent(_hand.transform);
					//_gun.transform.parent = _hand.transform;
					_gun.transform.localScale *= .085f;
					_gun.transform.localPosition = Vector3.zero;
					_gun.transform.localRotation = Quaternion.identity;

				}	
			}
		}
	}

	public bool SeenGun()
	{
		_planes = GeometryUtility.CalculateFrustumPlanes ( Camera.main );

		GunController[] gunControllers = GameObject.FindObjectsOfType< GunController >();

		for (int i = 0; i < gunControllers.Length; i++) 
		{
			//print ("in for");
			GunController temp = gunControllers [i];

			if (GeometryUtility.TestPlanesAABB (_planes, temp.GetComponent<SphereCollider> ().bounds)) 
			{ 
				//print ("in testplanes");
				RaycastHit hit = new RaycastHit ();
				Vector3 dir = transform.position - temp.transform.position;
				Ray ray = new Ray (temp.transform.position, dir);
				Physics.Raycast (ray, out hit, dir.magnitude);

				if (hit.collider == GetComponent<CapsuleCollider> ()) 
				{
					return true;

				}	
			}
		}
		return false;
	}

	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			m_Animator.applyRootMotion = false;
		}
	}
}

