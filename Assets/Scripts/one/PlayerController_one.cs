using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;

public class PlayerController_one : MonoBehaviour
{
	//public float speed = 3.0f;
	private Vector3 _offset = Vector3.zero;
	private float _jumpTimer = 0.0f;
	private float _sensitivity = 0.15f;
	public float speed = 0.0f;
	public float t = 0.0f;

	// Use this for initialization
	public void StartATL () 
	{
        
	}

	// Update is called once per frame
	public void UpdateATL (SplineLord_one splineLord_one, BookController bookController)
	{
		//print (Input.mouseScrollDelta.y);

		speed = -Input.mouseScrollDelta.y * _sensitivity;
		transform.position -= transform.up * _offset.y; 
		_jumpTimer = Mathf.Clamp01( _jumpTimer - Time.deltaTime );

		if (Input.GetKeyDown (KeyCode.Space) && _jumpTimer <= 0.0f ) //getkeydown only true first frame down, getkeyup true first frame released, getkey is true if down and false if up
		{
			_jumpTimer = 1.0f;
		}
		_offset = new Vector3 (0.0f, Mathf.Sin( AxKEasing.EaseOutQuad( 0.0f, 1.0f, 1.0f - _jumpTimer ) * Mathf.PI ), 0.0f);

		CurvySpline curSpline = splineLord_one.GetSpline (); 

		//this way gets nearest point on spline to current position
		t = curSpline.GetNearestPointTF( transform.position ); 
		t += ( speed / curSpline.Length ) * Time.deltaTime * 5.0f;
		transform.position = curSpline.Interpolate (t);

		//Vector3 sForward = curSpline.GetTangentToSpline( t );
		//Vector3 sNormal = curSpline.GetOrientationOnSpline (t);
		//Quaternion lookRotation = Quaternion.LookRotation (sForward, sNormal);

		transform.rotation = Quaternion.Slerp( transform.rotation, curSpline.GetOrientationFast( t ), Time.deltaTime * 5.0f );

		transform.position += transform.up * _offset.y; //places player over spline

		int indexOffset = 4;
		Vector3 v1 = curSpline.ControlPoints [0 + indexOffset].transform.position - curSpline.ControlPoints [1 + indexOffset].transform.position;
		Vector3 v2 = transform.position - curSpline.ControlPoints [1 + indexOffset].transform.position;
		float dot = Vector3.Dot (v1, v2);

		//print ( (Time.timeSinceLevelLoad % 2.0f).ToString( "F7" ) + "  " + ( Time.timeSinceLevelLoad ).ToString( "F5" ) + "  " + ( Time.timeSinceLevelLoad / 2.0f ).ToString( "F6") );
		if ( Time.timeSinceLevelLoad % 2.0f <= 0.1f )// dot < -.2) 
		{
			//splineLord.CreateControlPoint ();
		}

		if (t > .5) 
		{
			//SplineLord.GenerateSpline(curSpline);
		}

		/*
		 * if (t > .5)
		 * {
		 * splineLord.RegisterSpline() (next: get end pt from data stream)
		 * delete last spline
		 * }
		 */
	}
}
