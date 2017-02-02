using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;

public class AttackController : MonoBehaviour 
{
	public float t = 0.0f;
	private Vector3 _lift;

	// Use this for initialization
	public void StartATL () 
	{
		_lift = new Vector3 (0, 9, 0);
	}
	
	// Update is called once per frame
	public void UpdateATL ( ThirdPersonUserControl _playerController_three ) 
	{
//		CurvySpline spline = splineLord_three.GetSpline (); 
//
//		t = spline.GetNearestPointTF( _playerController_three.transform.position ); 
//
//		transform.position = spline.Interpolate ( t - spline.Length / 100.0f );
		transform.position = _playerController_three.transform.position - 15.0f * transform.forward + _lift;

		//transform.rotation = Quaternion.Slerp( transform.rotation, spline.GetOrientationFast( t ), Time.deltaTime * 5.0f );
	
	}
}
