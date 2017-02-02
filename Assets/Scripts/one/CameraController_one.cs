using UnityEngine;
using System.Collections;

public class CameraController_one : MonoBehaviour 
{

	public float smooth = 40.0f; // high number = low smooth = close to 0 (current position), low number = high smooth = close to 1 (next position)
//	public float sensitivity = 60.0f;
	private float lift = 3.5f;

	public void StartATL () 
	{
		
	}

	public void UpdateATL ( PlayerController_one player_one )
	{
		Vector3 rise = new Vector3 (0, lift, 0);

		transform.position = player_one.transform.position + rise; //Vector3.Normalize(player.transform.position.y) * 1.0f; //attach camera to player's head
			
		Vector3 dir = new Vector3 (0, 0, 0);
		Quaternion rot = new Quaternion(0, 0, 0, 0);

		rot = player_one.transform.rotation * Quaternion.AngleAxis (180, player_one.transform.up); 
		rot *= Quaternion.AngleAxis (40, player_one.transform.right); 
		transform.rotation = rot;

	}
				
}