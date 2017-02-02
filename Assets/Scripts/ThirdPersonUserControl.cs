using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FluffyUnderware.Curvy;


public class ThirdPersonUserControl : MonoBehaviour
{
private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

	private bool m_Aim = false;
	private bool m_Fire = false;
	private bool m_Pickup = false;

	private Vector3 lastPos;
	private float t;

	public void StartATL( CurvySpline spline )
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<ThirdPersonCharacter>();
    }


	public void UpdateATL( CurvySpline spline )
    {

		t = spline.GetNearestPointTF (transform.position);
		Vector3 splineAtT = spline.Interpolate ( t );

		if ( transform.position.y < splineAtT.y ) 
		{
			lastPos = splineAtT;
			Camera.main.GetComponent<CameraController_three> ().falling = true;
			//SceneManager.LoadScene ("three");
			StartCoroutine( Fall( 5.0f, spline ) ); //pass in length of fall
		} 
		else 
		{
			Camera.main.GetComponent<CameraController_three> ().falling = false;
		}
			
        if (!m_Jump)
        {
			m_Jump = Input.GetButtonDown("Jump");
        }

		//disabling jump
		m_Jump = false;

    }

	//if using Player_Animation, this is called from end of "arms" layer animation somehow
	void FireBullet()
	{
		//m_Fire = true;
	}
		
    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);

		//disabling crouch 
		crouch = false;

        // calculate move direction to pass to character
        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v*m_CamForward + h*m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v*Vector3.forward + h*Vector3.right;
        }

		//putting here rather than update to see if m_Character.Move is sometimes missing the frame that m_Fire is true
		//the issue is double clicking before the cycle is complete. the animator cannot register the second m_Fire, because M_Fire=false sets the next frame, not when the animation is complete.
		//need to somehow pair each mouse button up with the correlating animation. 
		if (m_Aim && Input.GetMouseButtonUp (0)) 
		{
			//print ("m_Fire true in first person user");
			//print (Input.GetMouseButtonUp (0));
			m_Fire = true;
		} 
		else 
		{
			m_Fire = false;
		}

		//alt control to fire on mouse up
		if (Input.GetMouseButton (0)) 
		{
			m_Aim = true;
		} 
		else 
		{
			m_Aim = false;
		}

		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			m_Pickup = true;
		} 
		else 
		{
			m_Pickup = false;
		}



#if !MOBILE_INPUT
		// walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 1.3f;
#endif

        // pass all parameters to the character control script
		m_Character.Move(m_Move, crouch, m_Jump, m_Aim, m_Fire, m_Pickup);
        m_Jump = false;
    }

	IEnumerator Fall ( float delay, CurvySpline spline )
	{
		print ("now falling");
		yield return new WaitForSeconds( delay );

		Vector3 targetPosition = new Vector3 (transform.position.x, lastPos.y + 50.0f, transform.position.z);
		//AxKDebugLines.AddFancySphere (targetPosition, 1.0f, Color.blue, 0.0f);
		//AxKDebugLines.AddFancySphere (lastPos, 1.0f, Color.red, 0.0f);
		//AxKDebugLines.AddFancySphere ( new Vector3(lastPos.x, lastPos.y + 5.0f, lastPos.z) , 1.0f, Color.green, 0.0f);
		//AxKDebugLines.AddFancySphere (transform.position, 1.0f, Color.magenta, 0.0f);

		print ("now rising above");

		while (transform.position.y < targetPosition.y - 1.0f) 
		{
			transform.position = Vector3.Lerp ( transform.position, targetPosition, Time.deltaTime );
			transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation (( targetPosition - transform.position).normalized, new Vector3(0,1,0)), Time.deltaTime ); 
		}

		print ("now landing to spline");

		Vector3 splineForward = spline.GetTangent (t);

		while (transform.position.y > lastPos.y + 10.0f) 
		{
			transform.position = Vector3.Lerp ( transform.position, lastPos, Time.deltaTime );
			transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation (splineForward), Time.deltaTime ); 
		}

		print ("now done");


		Camera.main.GetComponent<CameraController_three> ().falling = false;
	}
}

