using UnityEngine;
using System.Collections;

public class UseGun : MonoBehaviour 
{
	private Plane[] _planes;
	private GunController _gun;
	private GameObject _hand;
	private bool _seen = false;
	private bool _fire = false;

	// Use this for initialization
	void Start () 
	{
		_hand = GameObject.Find ("GunHolder");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//alt control to fire on mouse up
		if (Input.GetMouseButtonUp (0)) 
		{
			_fire = true;
		}

		if (_gun != null & _fire) 
		{
			
			_gun.Use ( transform, transform ); //params: targetOne, targetTwo
			_fire = false;
		}

		_planes = GeometryUtility.CalculateFrustumPlanes ( Camera.main );

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

				//use Q to pick up objects. pick up gun when it's in sight
				if (hit.collider == GetComponent<CapsuleCollider> () && Input.GetKeyDown (KeyCode.Q)) 
				{
					temp.GetComponent<SphereCollider> ().enabled = false;
					_gun = temp.GetComponent< GunController > ();
					_gun.transform.parent = _hand.transform;
					_gun.transform.localScale *= .1f;
					_gun.transform.localPosition = Vector3.zero;
					_gun.transform.localRotation = Quaternion.identity;

				}	
			}
		}
	
	}

	//if using Player_Animation, this is called from end of "arms" layer animation somehow
	void FireBullet()
	{
		_fire = true;
	}
}
