using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class SplineLord_two : MonoBehaviour
{

	private List < CurvySpline > _splines = new List< CurvySpline >();
	private CurvySpline _spline;
	private float _splineTimer = 0.0f;

	public void StartATL ()
	{
		_spline = GameObject.FindObjectOfType< CurvySpline > ();
		_spline.Add ();

	}

	public void UpdateATL ()
	{

	}

	public void CreateControlPoint ( Transform playerTransform )
	{
		_spline.Add();
		CurvySplineSegment newPoint = _spline.ControlPoints [_spline.ControlPointCount - 1];

		newPoint.transform.localEulerAngles = new Vector3( playerTransform.localEulerAngles.x, playerTransform.localEulerAngles.y, playerTransform.localEulerAngles.z );

		newPoint.transform.position = playerTransform.position;


	}

	public CurvySpline GetSpline ()
	{
		return _spline;
	}
}		