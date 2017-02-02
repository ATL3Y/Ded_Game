using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class SplineLord_one : MonoBehaviour
{
    
	private List < CurvySpline > _splines = new List< CurvySpline >();
	private CurvySpline _spline;
	//private Quaternion _offsetQuat = Quaternion.LookRotation(Vector3.right);
	private float _splineTimer = 0.0f;

	public void StartATL ()
    {
		_spline = GameObject.FindObjectOfType< CurvySpline > ();
		_spline.Add ();

		for (int i = 0; i < 30; i++) 
		{
			CreateControlPoint ();
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

		//not working??
		//_offsetQuat *= Quaternion.Euler (0.0f, 0.0f, Random.Range (-4.0f, 4.0f));
		//newPoint.transform.localRotation = _offsetQuat;

		//newPoint.transform.localEulerAngles = new Vector3( (_spline.ControlPointCount * 10.0f ) % 360.0f, newPoint.transform.localEulerAngles.y, newPoint.transform.localEulerAngles.z );
		//newPoint.transform.localEulerAngles = new Vector3( 10.0f * Mathf.Sin(_spline.ControlPointCount / 2.0f), newPoint.transform.localEulerAngles.y, Mathf.Cos(Random.Range (-0.03f, 0.03f)) );
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

    
//	public void RegisterSpline( CurvySpline splineToAdd )
//	{
//		m_splines.Add (splineToAdd);
//    }

//	public void RegisterSegment( Vector3 controlPointToAdd)
//	{
//		CurvySplineSegment.Add ();
//	}

//	public Bounds GetSplineBounds( CurvySpline s )
//	{
//		Vector3 sCenter = Vector3.zero;
//		
//		for (int j = 0; j < s.SplineNodes.Length; j++)
//		{
//			sCenter += s.SplineNodes[j].Position;
//		}
//		
//		sCenter /= s.SplineNodes.Length;
//		
//		Bounds sBounds = new Bounds(sCenter, Vector3.zero); 
//		
//		for (int j = 0; j < s.SplineNodes.Length; j++)
//		{
//			sBounds.Encapsulate(s.SplineNodes[j].Position);
//		}
//
//		return sBounds;
//	}
//
//	public CurvySpline QueryNearestSpline( Vector3 position )
//	{
//		for (int i = 0; i < m_splines.Count; i++) 
//		{
//			CurvySpline s = m_splines[ i ];
//			Bounds sBounds = GetSplineBounds( s );
//
//			if(sBounds.Contains( position ))
//			{
//				return s;
//			} 
//
//		}
//
//
//		return m_splines[ 0 ]; //need dummy return
//
//	}

//	public void GenerateSpline( CurvySpline curSpline)
//	{
//		GameObject splineInstance;
//		Transform splineInstanceEndNode;
//		int i = 1;
//		splineInstance = Instantiate ( splinePrefab );
//		splineInstanceEndNode = splineInstance.transform.Find ("0003");
//		splineInstanceEndNode = i;
//		i++;
//	}
}
