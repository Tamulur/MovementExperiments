using UnityEngine;
using System.Collections;

public class MotionController : MonoBehaviour
{

	#region fields
	
			bool _useCanvas;
		public bool useCanvas {
			get { return _useCanvas; }
			set {
				_useCanvas = value;
				canvasSphere.gameObject.SetActive ( _useCanvas );
			}
		}
		
		const float kForwardSpeedFactor = 1.3f;
		const float kMouseSensitivity = 1.25f;
	
		Animator animator;
		CharacterController characterController;
		SoundSource footstepSoundLeft;
		SoundSource footstepSoundRight;
//		ControlCamera controlCameraLeft;
//		ControlCamera controlCameraRight;

		CanvasSphere canvasSphere;
		
		float timeSinceLastLeftFootstep;
		float timeSinceLastRightFootstep;
	
	#endregion
	
	
	
	public void Activate()
	{
		enabled = true;
	}
	
	
	
	void Awake()
	{
		footstepSoundLeft = transform.Find("FootstepSoundLeft").GetComponent<SoundSource>();
		footstepSoundRight = transform.Find("FootstepSoundRight").GetComponent<SoundSource>();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
		canvasSphere = GetComponentInChildren<CanvasSphere>();
		
//		controlCameraLeft = transform.Find ("OVRCameraController/CameraLeft").GetComponent<ControlCamera>();
//		controlCameraRight = transform.Find ("OVRCameraController/CameraRight").GetComponent<ControlCamera>();
	}
	
	
	
	public void Deactivate()
	{
		enabled = false;
		
		animator.SetFloat("speed", 0);
	}
	
	
	
	void PlayLeftFootstep()
	{
		if (timeSinceLastLeftFootstep > 0.3f)
		{
			timeSinceLastLeftFootstep = 0;
			footstepSoundLeft.Play();
		}	
	}
	
	
	
	void PlayRightFootstep()
	{
		if ( timeSinceLastRightFootstep > 0.3f )
		{
			timeSinceLastRightFootstep = 0;
			footstepSoundRight.Play();
		}
	}
	
	
	
	void Update()
	{
		timeSinceLastLeftFootstep += Time.deltaTime;
		timeSinceLastRightFootstep += Time.deltaTime;
		
		float forwardSpeed = 0;
		float sideSpeed = 0;
		float direction = 0;
		bool isRunning  = false;
		bool isMovingOrTurning = false;
		
		//*** forward speed
		{
			if (Input.GetKey(KeyCode.W))
				forwardSpeed = 1;
			else if (Input.GetKey(KeyCode.S))
				forwardSpeed = -1;
			
			forwardSpeed *= kForwardSpeedFactor;
		}
		
		//*** side speed
		{
			if (Input.GetKey(KeyCode.A))
				sideSpeed = -1;
			else if (Input.GetKey(KeyCode.D))
				sideSpeed = 1;
		}
		
		//*** direction
		{
			float mouseDiff = Input.GetAxis("Mouse X");
			if ( Mathf.Abs( mouseDiff ) > 0 )
				direction = mouseDiff * kMouseSensitivity;
			
			if (Input.GetKey(KeyCode.Q))
				direction = -1;
			else if (Input.GetKey(KeyCode.E))
				direction = 1;
		}
		
		float totalSpeed = new Vector2( sideSpeed, forwardSpeed).magnitude;
		animator.SetFloat("speed", ((forwardSpeed >= 0) ? 1 : -1) * totalSpeed);
		
		float runFactor = isRunning ? 2.5f : 1;
		
		if ( direction != 0 )
		{
			transform.rotation = transform.rotation * Quaternion.Euler(0, 100 * direction * Time.unscaledDeltaTime, 0);
			
			isMovingOrTurning = true;
		}
		
		characterController.SimpleMove(	runFactor * forwardSpeed * transform.forward +
		                               				runFactor * sideSpeed * transform.right );
		                               
		if ( forwardSpeed != 0 || sideSpeed != 0 )
			isMovingOrTurning = true;

		canvasSphere.shouldBeShown = useCanvas && isMovingOrTurning;
			
//		controlCameraLeft.useCanvas = useCanvas && isMovingOrTurning;
//		controlCameraRight.useCanvas = useCanvas && isMovingOrTurning;
		
	}
	
	
	
}
