using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;

public class Crosshair : MonoBehaviour 
{ 
	public float offsetCrosshair = 3.5f;
	public float lift = 10.0f;
	public float smooth = 40.0f;
	private float t;

	private float width = 20.0f; 
	private float height = 7.0f; 
	private Vector3 offset;
	//public float mult = 1.0f; //doesn't work yet

	private Transform _small;
	private Transform _medium;
	private Transform _large;

	bool init = false;

	public bool aim = false; //when true, turn blue 
	public bool fire = false; //when true, turn back white  

	public Material readyS;
	public Material readyM;
	public Material readyL;
	public Material aimS;
	public Material aimM;
	public Material aimL;
	public Material fireS;
	public Material fireM;
	public Material fireL;

	public float timer;

	public void StartATL( ThirdPersonUserControl player, CurvySpline spline ) 
	{
		//Note that confined cursor lock mode is only supported on the standalone player platform on Windows and Linux.
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;
		offset = new Vector3 (0, 0, 0);

		_small = transform.GetChild (0);
		_medium = transform.GetChild (1);
		_large = transform.GetChild (2);

		_small.gameObject.GetComponent<Renderer> ().material = readyS;
		_medium.gameObject.GetComponent<Renderer> ().material = readyM;
		_large.gameObject.GetComponent<Renderer> ().material = readyL;
	}

	public void UpdateATL( ThirdPersonUserControl player, CurvySpline spline )
	{

		if (!init && spline.IsInitialized)
		{
			init = true;
		}

		if (!init)
			return;

		//print("mouse x " + Input.GetAxis("Mouse X") + ", mouse y " + Input.GetAxis("Mouse Y"));
		offset += new Vector3(-Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0.0f);
		//print ("offset = " + offset);
		offset = new Vector3(Mathf.Clamp(offset.x, -width/2.0f, width/2.0f), Mathf.Clamp(offset.y, lift, height + lift), 0.0f);
		//offset *= mult; doesn't work

		//print ("offset after clamp = " + offset);

		t = spline.GetNearestPointTF (player.transform.position);
		Vector3 splineUp = spline.GetRotatedUpFast (t, 0);
		//AxKDebugLines.AddLine (player.transform.position, player.transform.position + splineUp, Color.red, .001f);
		Vector3 splineForward = spline.GetTangent (t);
		Vector3 splineRight = Vector3.Cross (splineUp, splineForward);

		Vector3 tempOffset = splineRight * offset.x + splineUp * offset.y; //changed from Vector3.up

		float targetPositionT = t - offsetCrosshair / spline.Length;
		Vector3 targetT = spline.Interpolate (targetPositionT);
		Vector3 targetPosition = targetT + tempOffset;

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth * 3.0f );
		//transform.localRotation = Quaternion.Slerp( transform.localRotation, Quaternion.LookRotation (-splineForward.normalized), Time.deltaTime * smooth );

		//transform.position = targetPosition;
		transform.localRotation = Camera.main.transform.localRotation;

		if (fire) //fire is only true for one frame
		{
			_small.gameObject.GetComponent<Renderer> ().material = fireS;
			_medium.gameObject.GetComponent<Renderer> ().material = fireM;
			_large.gameObject.GetComponent<Renderer> ().material = fireL;

			//print ("in fire, mat name = " + _medium.gameObject.GetComponent<Renderer> ().material.name);
			StartCoroutine (Delay ( 6.0f ));
		} 
		else if (aim && !fire) 
		{
			_small.localRotation = Quaternion.Euler(transform.localRotation.y, transform.localRotation.x, tempOffset.x * 300.0f);
			_medium.localRotation = Quaternion.Euler(transform.localRotation.y, transform.localRotation.x, tempOffset.y * 300.0f); //y = Time.time; // * 180.0f; // Quaternion.AngleAxis (Time.time * 180.0f , transform.forward);

			_small.gameObject.GetComponent<Renderer> ().material = aimS;
			_medium.gameObject.GetComponent<Renderer> ().material = aimM;
			_large.gameObject.GetComponent<Renderer> ().material = aimL;
			//print ("in aim, mat name = " + _medium.gameObject.GetComponent<Renderer> ().material.name);
		} 
		else
		{
			//ease back. only call on return
			_small.localRotation = Quaternion.Slerp( _small.localRotation, Quaternion.Euler (Vector3.zero), Time.deltaTime );
			_medium.localRotation = Quaternion.Slerp( _medium.localRotation, Quaternion.Euler (Vector3.zero), Time.deltaTime );

			_small.gameObject.GetComponent<Renderer> ().material = readyS;
			_medium.gameObject.GetComponent<Renderer> ().material = readyM;
			_large.gameObject.GetComponent<Renderer> ().material = readyL;
			//print ("in ready, mat name = " + _medium.gameObject.GetComponent<Renderer> ().material.name);
		}
	}
		
	IEnumerator Delay ( float delay )
	{
		yield return new WaitForSeconds( delay );
		fire = false;
		//print ("delay done and fire = " + fire);
	}

	void OnTriggerEnter(Collider other) //doesn't register the spline
	{
		Vector3 otherDir = transform.position - other.transform.position; //from other to this GO's center
		//AxKDebugLines.AddLine (transform.position, other.transform.position, Color.red, 10);
		float dot = Vector3.Dot (otherDir, transform.forward);
		//print ("dot = " + dot);
		//print ("collider name = " + other.name); //not sure why this doen't print for same collider going the opposite way
		if (dot > 0.0f) // if the two vectors are going moreso the same way
		{
			//print ("in dot");
			if (other.gameObject.tag == "Missile" || other.gameObject.tag == "SpecialMissile") 
			{
				//print ("in trig if and fire true = " + fire);
				fire = false;
				//print ("in trig if and fire false = " + fire);
			}
		}
	}
}
