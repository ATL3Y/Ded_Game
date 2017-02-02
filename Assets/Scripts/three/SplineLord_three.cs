using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class SplineLord_three : MonoBehaviour
{

	private List < CurvySpline > _splines = new List< CurvySpline >();
	private CurvySpline _spline;
	private float _splineTimer = 0.0f;

	public void StartATL ()
	{
		_spline = GameObject.FindObjectOfType< CurvySpline > ();
		_spline.Add ();

		for (int i = 0; i < 10; i++) 
		{
			//CreateControlPoint ();
		}
	}

	public void UpdateATL ()
	{

	}

	public void CreateControlPoint ( )
	{

		CurvySplineSegment prevPoint = _spline.ControlPoints [_spline.ControlPointCount - 1];
		_spline.Add();
		CurvySplineSegment newPoint = _spline.ControlPoints [_spline.ControlPointCount - 1];

		float angle = Mathf.Sin (Time.timeSinceLevelLoad);
		Vector3 axis = prevPoint.transform.forward;
		int distance = 4;

		newPoint.transform.localEulerAngles = new Vector3( newPoint.transform.localEulerAngles.x, 10.0f * Mathf.Sin(_spline.ControlPointCount / 2.0f), Mathf.Cos(Random.Range (-0.03f, 0.03f)));
		newPoint.transform.position = prevPoint.transform.position + newPoint.transform.forward * distance;

		//draw red debug lines out 
		Vector3 rightPt = new Vector3 (newPoint.transform.position.x + 20.0f, newPoint.transform.position.y, newPoint.transform.position.z);
		Vector3 leftPt = new Vector3 (newPoint.transform.position.x - 20.0f, newPoint.transform.position.y, newPoint.transform.position.z);
		AxKDebugLines.AddLine (newPoint.transform.position, rightPt, Color.red, 9999999.0f); 
		AxKDebugLines.AddLine (newPoint.transform.position, leftPt, Color.red, 9999999.0f); 

	}

	public CurvySpline GetSpline ()
	{
		return _spline;
	}
}
