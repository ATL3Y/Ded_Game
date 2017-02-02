using UnityEngine;
using System.Collections;

public class CameraController_two: MonoBehaviour 
{

	public float distance = 5.0f; // target distance between player and camera 
	public float distanceBuffer = 4.0f;
	public float targetHeight = 3.0f;
	public float smooth = 40.0f; // high # --> 1 --> !smooth, low # --> 0 --> smooth
	public float sensitivity = 60.0f;

	private float _h, _w = 1.0f;

	public void StartATL () 
	{
		NearClipCornerD ();
	}

	public void UpdateATL ( PlayerController_two player_two )
	{

		Vector3 playerPosition = player_two.transform.position + Vector3.up * 0.5f; //make player's position at player's head
		Vector3 currentDirectionFromPlayer = transform.position - playerPosition; //vector from player to camera
		float currentDistanceFromPlayer = currentDirectionFromPlayer.magnitude;
		//print (currentDistanceFromPlayer);
		currentDirectionFromPlayer = currentDirectionFromPlayer.normalized;
		float angleY = Input.GetAxisRaw ("Mouse X") * sensitivity * Time.deltaTime; //make change in terms of time rather than frames. scale angle to degrees

		//X Angle
		Vector3 flatCurrentDirectionFromPlayer = currentDirectionFromPlayer;
		flatCurrentDirectionFromPlayer[ 1 ] = 0.2f;
		flatCurrentDirectionFromPlayer = flatCurrentDirectionFromPlayer.normalized;
//		AxKDebugLines.AddRay (playerPosition, flatCurrentDirectionFromPlayer * 3.0f, Color.green);
//		AxKDebugLines.AddRay (playerPosition, currentDirectionFromPlayer * 3.0f, Color.yellow);

		float angleX = ( ( 1.0f - Vector3.Dot (flatCurrentDirectionFromPlayer, currentDirectionFromPlayer) ) / Time.deltaTime ) * 2.0f;
		angleX *= Vector3.Dot (Vector3.Cross (flatCurrentDirectionFromPlayer, currentDirectionFromPlayer), transform.right) > 0.0f ? 1.0f : -1.0f;

		float targetDistanceFromPlayer = Mathf.Max (distance - distanceBuffer, Mathf.Min (distance + distanceBuffer, currentDistanceFromPlayer)); //if current distance is in donut buffer, distance doesn't change
		//targetDistanceFromPlayer = currentDistanceFromPlayer;
		Vector3 targetDirectionFromPlayer = Quaternion.AngleAxis (angleY, Vector3.up) * currentDirectionFromPlayer; //rotate current direction by mouse input

//		AxKDebugLines.AddRay (playerPosition, targetDirectionFromPlayer * 3.4f, Color.red);
		targetDirectionFromPlayer = Quaternion.AngleAxis( -angleX, transform.right ) * targetDirectionFromPlayer;
//		AxKDebugLines.AddRay (playerPosition, flatCurrentDirectionFromPlayer * 3.0f, Color.green);
//		AxKDebugLines.AddRay (playerPosition, currentDirectionFromPlayer * 3.0f, Color.yellow);
//		AxKDebugLines.AddRay (playerPosition, targetDirectionFromPlayer * 3.0f, Color.blue);

		//transform.position = playerPosition + targetDirectionFromPlayer * currentDistanceFromPlayer; //why setting transform here? is it rendering here?
		Vector3 targetPosition = playerPosition + targetDirectionFromPlayer * targetDistanceFromPlayer; 
		int count = 0;  

		Vector3 pointNearClipCenter = transform.forward * GetComponent<Camera> ().nearClipPlane;
		Vector3[] points = new Vector3[ 4 ];
		points[ 0 ] = pointNearClipCenter - transform.right * _w/2 - transform.up * _h/2;
		points[ 1 ] = pointNearClipCenter - transform.right * _w/2 + transform.up * _h/2;
		points[ 2 ] = pointNearClipCenter + transform.right * _w/2 + transform.up * _h/2;
		points[ 3 ] = pointNearClipCenter + transform.right * _w/2 - transform.up * _h/2;

		RaycastHit hit = new RaycastHit ();
		for (int i = 0; i < points.Length; i++) 
		{
			count++;
			Vector3 dir = targetPosition + points [i] - playerPosition;
			Ray ray = new Ray (playerPosition, dir );

			while (Physics.Raycast(ray, out hit, dir.magnitude)) 
			{
				count++;
//				print (count);
				if (count > 50) 
				{
					break;
				}

				//AxKDebugLines.AddSphere( hit.point, 0.2f, Color.red );
				//transform.position = hit.point + hit.normal * 0.5f - points [i];
				targetPosition = hit.point + hit.normal * 0.1f - points [i]; 
				dir = targetPosition + points [i] - playerPosition;
				ray = new Ray (playerPosition, dir );
//				AxKDebugLines.AddSphere( hit.point, 0.1f, Color.blue, 100.1f );
				//AxKDebugLines.AddRay (ray.origin, dir, Color.red, 100.1f );
			}
		}

		//transform.position = targetPosition;
		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth); // smooth camera movement from last position to target position by t  
		transform.forward = Vector3.Normalize (player_two.transform.position - transform.position); // orient the camera rotation to face player
	}

	private void NearClipCornerD ()
	{
		_h = 2 * Mathf.Tan(GetComponent<Camera>().fieldOfView / 2 * GetComponent<Camera>().nearClipPlane);
		_w = GetComponent<Camera> ().aspect * _h;
//		float biDiagonalD = Mathf.Sqrt (Mathf.Pow ((h / 2), 2) + Mathf.Pow ((w / 2), 2));
//		float nearClipCornerD = Mathf.Sqrt (Mathf.Pow (GetComponent<Camera> ().nearClipPlane, 2) + Mathf.Pow (biDiagonalD, 2));
	}
				
}
