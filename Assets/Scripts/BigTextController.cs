using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;

public class BigTextController : MonoBehaviour 
{
	//make sure this doesn't run after death.. ?
	private List < GameObject > _alphabet = new List<GameObject> (); //these are all the letters of the alphabet to spell with
	private List < GameObject > _letters = new List<GameObject> (); //letters in the given phrase

	private bool wasdDone = false;
	private bool wasdCleared = false;
	private bool qDone = false;
	private bool mouseAimDone = false;
	private bool mouseFireDone = false;
	private bool fireCleared = false;

	private bool wPressed = false;
	private bool aPressed = false;
	private bool sPressed = false;
	private bool dPressed = false;

	bool init = false;
	public float smooth = 40.0f;

	private int textLength;

	public void StartATL ( ThirdPersonUserControl player, CurvySpline spline ) 
	{
		string levelPath = "lettersGO";

		Object[] alphabet = Resources.LoadAll ( levelPath, typeof(GameObject));

		if (alphabet == null || alphabet.Length == 0) 
		{
			print ("no files found");
		}

		foreach (GameObject letter in alphabet) //for each letter in the array loaded from resources 
		{
			GameObject l = (GameObject)letter;
			_alphabet.Add (l); //add letter to the list // note: could just use the array...
		}

		//player.GetComponentInChildren<MakeMesh> (true).enabled = false;
		MakeTextGO("I GOTTA GET OUT OF HERE");
	}

	public void UpdateATL ( ThirdPersonUserControl player, CurvySpline spline ) 
	{
		if (!init && spline.IsInitialized)
		{
			init = true;
		}

		if (!init)
			return;

		if (!Camera.main.GetComponent<CameraController_three> ().CUbool && !wasdDone) 
		{
			//player.GetComponentInChildren<MakeMesh> (true).enabled = true;
			MakeTextGO ("TO MOVE HOLD DOWN WASD"); //0-17 before "W"
			wasdDone = true;
		}
		if(Input.GetKeyDown(KeyCode.W) && !wPressed)
		{
			SwapCharGO(' ', 14); // replace W with a space
			wPressed = true;
		}
		if(Input.GetKeyDown(KeyCode.A) && !aPressed)
		{
			SwapCharGO(' ', 15);
			aPressed = true;
		}
		if(Input.GetKeyDown(KeyCode.S) && !sPressed)
		{
			SwapCharGO(' ', 16);
			sPressed = true;
		}
		if(Input.GetKeyDown(KeyCode.D) && !dPressed)
		{
			SwapCharGO(' ', 17);
			dPressed = true;
		}
			
		// conditionals for which words to show 
		if (wPressed && sPressed && aPressed && dPressed && !wasdCleared) 
		{
			Clear ();
			wasdCleared = true;
		}
		if (player.GetComponent<ThirdPersonCharacter> ().SeenGun () && !qDone && wasdCleared) //must evade intro wide with gun in view
		{
			MakeTextGO ("PRESS Q TO PICKUP");
			qDone = true;
		}
		if (player.GetComponent<ThirdPersonCharacter> ().m_pickup == true && !mouseAimDone) 
		{
			MakeTextGO("PRESS LEFT MOUSE DOWN TO AIM");
			mouseAimDone = true;
		}
		if (player.GetComponent<ThirdPersonCharacter> ().m_doneAim == true && !mouseFireDone) 
		{
			MakeTextGO("RELEASE LEFT MOUSE TO FIRE");
			mouseFireDone = true;
			//StartCoroutine (Fade (4.0f)); //do on fireFlag below
		}
		if (player.GetComponent<ThirdPersonCharacter> ().m_fireFlag == true & !fireCleared) 
		{
			Clear ();
			fireCleared = true;
		}
			
		float t = spline.GetNearestPointTF (player.transform.position);

		//AxKDebugLines.AddSphere (spline.Interpolate (maxT), 0.3f, Color.green);
		//AxKDebugLines.AddSphere (spline.Interpolate (minT), 0.3f, Color.magenta);

		Vector3 splineUp = spline.GetRotatedUpFast (t, 0);
		Vector3 splineForward = spline.GetTangent (t);
		Vector3 splineRight = Vector3.Cross (splineUp, splineForward);

		//Vector3 rise = splineRight * offset.x + splineUp * offset.y + splineForward * offset.z;

		Vector3 lookPosition = Camera.main.transform.position;
		//Vector3 targetPosition = player.transform.position + player.transform.right * 0.8f * textLength / 4 + player.transform.up * 4.0f - player.transform.forward * 3.0f;
		Vector3 targetPosition = spline.Interpolate(t) + splineRight * 0.8f * textLength / 3 + splineUp * 4.0f - splineForward * 3.0f;
		//AxKDebugLines.AddSphere (lookPosition, .1f, Color.red);
		//AxKDebugLines.AddSphere (targetPosition, .1f, Color.blue);

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation (( lookPosition - transform.position).normalized), Time.deltaTime * smooth ); 
	}

	private void MakeTextGO ( string text ) //could have size, shader... //assuming all uppercase 
	{
		Clear ();
		textLength = text.Length;
		//print (text.Length + " that's t length");
		for (int i=0; i < text.Length; i++) 
		{
			int count = 1; 

			if (text [i] == ' ') 
			{
				count *= 2; //make a space 
				continue; //jump to the next letter 
			}

			for (int j = 0; j < _alphabet.Count; j++) 
			{
				if (text [i].ToString() == _alphabet [j].name) 
				{ 
					GameObject newLetter = Instantiate (_alphabet [j]);
					newLetter.transform.position = new Vector3 (-0.8f * count * i, 0f, 0f);
					newLetter.transform.parent = transform;
					_letters.Add (newLetter);
					count = 1;
				} 
			}
		}
		//print ("letters count after make = " + _letters.Count);
	}

	private void SwapCharGO ( char c, int item ) //could have size, shader... //assuming all uppercase 
	{
		GameObject newLetter = new GameObject ();

//		if (c == ' ') 
//		{
//			//newLetter = new GameObject ();
//			break; //jump to the next letter 
//		} 

		//assuming no match will yield an empty space
		for (int j = 0; j < _alphabet.Count; j++) 
		{
			if (c.ToString() == _alphabet [j].name) 
			{ 
				newLetter = Instantiate (_alphabet [j]);
			} 
		}

		newLetter.transform.position = _letters[item].transform.position;
		newLetter.transform.parent = transform;

		GameObject.Destroy (_letters [item]); //need?
		_letters [item] = newLetter;
	}

	private void Clear ()
	{
		foreach (Transform child in transform) 
		{
			GameObject.Destroy(child.gameObject);
		}

		_letters.Clear ();

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
	}

	IEnumerator Fade (float delay)
	{
		yield return new WaitForSeconds(delay);
		Clear ();
	}

}
