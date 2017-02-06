using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;

public class CameraController_three: MonoBehaviour 
{
	public float buffer = 8f;
	public float offsetCam = 10f;
	public float offsetLook = 10f;

	private float minT = .00001f;
	private float maxT = .00001f;

	private float t = .02f;
    public float GetPlayerT() { return t; }
    Vector3 splineUp = Vector3.zero;
    Vector3 splineForward = Vector3.zero;
    Vector3 splineRight = Vector3.zero;
    public Vector3 GetSplineUp() { return splineUp; }
    public Vector3 GetSplineForward() { return splineForward; }
    public Vector3 GetSplineRight() { return splineRight; }

    public Vector3 offset; //ideally this is just right to left offset
	public float smooth = 20.0f;

	public bool CUbool = true;
	public bool falling = false;

	bool init = false;

	public void StartATL ( ThirdPersonUserControl player, CurvySpline spline ) 
	{
		//print ("start called");
		StartCoroutine (CU (5.0f));
	}

	public void UpdateATL ( ThirdPersonUserControl player, CurvySpline spline )
	{
		if (!init && spline.IsInitialized)
		{
			init = true;
			t = spline.GetNearestPointTF (player.transform.position);

			minT = t - ( buffer / spline.Length ) / 2.0f;
			maxT = t + ( buffer / spline.Length ) / 2.0f;
		}

		if (!init)
			return;

		//print ("spline name = " + spline.name);

		float playerT = spline.GetNearestPointTF (player.transform.position);
		//print ("playerT = " + playerT);
		//AxKDebugLines.AddSphere (player.transform.position, 0.3f, Color.green);
		//AxKDebugLines.AddSphere (spline.Interpolate (player.transform.position), 0.3f, Color.magenta);
		//AxKDebugLines.AddSphere (spline.Interpolate (playerT), 0.3f, Color.blue);

		if (playerT > maxT) 
		{
			maxT = playerT;
			minT = maxT - ( buffer / spline.Length );
			t = playerT;
		} 
		else if (playerT < minT) 
		{
			minT = playerT;
			maxT = minT + ( buffer / spline.Length );
			t = playerT;
		}

		//AxKDebugLines.AddSphere (spline.Interpolate (maxT), 0.3f, Color.green);
		//AxKDebugLines.AddSphere (spline.Interpolate (minT), 0.3f, Color.magenta);
			
		splineUp = spline.GetRotatedUpFast (t, 0);
		splineForward = spline.GetTangent (t);
		splineRight = Vector3.Cross (splineUp, splineForward);
		//print ("offset = " + offset);

		Vector3 rise = splineUp * offset.y + splineForward * offset.z + splineRight * offset.x * Mathf.Sin(Mathf.Sqrt(Time.time));

		float targetPositionT = t + offsetCam / spline.Length;
		Vector3 targetPosition = spline.Interpolate (targetPositionT) + rise;

		float lookPositionT = t - offsetLook / spline.Length;
		Vector3 lookPosition = spline.Interpolate (lookPositionT);
		targetPosition = ((targetPosition * 4f) + player.transform.position) / 5f;

		if (CUbool) 
		{
			lookPosition = player.transform.position + player.transform.up * 2.55f;
			targetPosition = lookPosition + player.transform.forward * .4f;
			//print ("in bool");
		} else if (falling) 
		{
			lookPosition = player.transform.position;
			targetPosition = transform.position;
		}

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation (( lookPosition - transform.position).normalized), Time.deltaTime * smooth ); 

	}

	IEnumerator CU (float delay)
	{
		CUbool = true;
		yield return new WaitForSeconds(delay);
		CUbool = false;
	}

}
