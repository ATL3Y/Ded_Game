using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class GunController : MonoBehaviour 
{

	private List < Missile > _missiles = new List < Missile > ();
	public Transform muzzle;
	float kickBackTime = 1.0f;
	public AnimationCurve kickBackCurve;
	private AudioSource _audiosource;

	// Use this for initialization
	public void StartATL () 
	{
		_audiosource = GetComponent<AudioSource> ();
		
	}
	
	// Update is called once per frame
	public void UpdateATL ( ThirdPersonUserControl player_three, Crosshair crosshair, CurvySpline spline ) 
	{
		kickBackTime = Mathf.Clamp01 (kickBackTime + Time.deltaTime * 2.0f);
		transform.localEulerAngles = new Vector3 (kickBackCurve.Evaluate (kickBackTime) * -70, transform.localEulerAngles.y, transform.localEulerAngles.z); 

		for (int i = 0; i < _missiles.Count; i++) 
		{
			_missiles [i].UpdateATL ( player_three, crosshair, spline);
		}
			
	}

	public void Use( ThirdPersonUserControl player_three, Crosshair crosshair )
    {
		_audiosource.Play ();
		GameObject newGameObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		Missile _missile = newGameObject.AddComponent< Missile > ();
	
		Vector3 startDirection = Vector3.Normalize(crosshair.transform.position - muzzle.position);
		_missile.StartATL( startDirection, muzzle.position ) ; // endTarget, startDirection, startPosition
		kickBackTime = 0.0f;
		RegisterMissile (_missile);
	}

	void RegisterMissile ( Missile missileToAdd )
	{
		_missiles.Add (missileToAdd);
	}
}
