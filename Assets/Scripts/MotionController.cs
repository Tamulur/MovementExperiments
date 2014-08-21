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
		
			bool _useStrobing;
		public bool useStrobing {
			get { return _useStrobing; }
			set {
				_useStrobing = value;
				isShowingFrames = true;
				lastFrameShownTime = 0;
				framesShown = framesHidden = 0;
				framesOfNotMoving = 0;
			}
		}
				
		const float kForwardSpeedFactor = 1.3f;
		const float kMouseSensitivity = 1.25f;
		
		
		const float kStrobeTime = 0.05f;
		int kShowFrames = 10;
		int kHideFrames = 30;
	
		Animator animator;
		CharacterController characterController;
		SoundSource footstepSoundLeft;
		SoundSource footstepSoundRight;
		ControlCamera controlCameraLeft;
		ControlCamera controlCameraRight;

		CanvasSphere canvasSphere;
		
		int framesShown;
		int framesHidden;
		float lastFrameShownTime;
		bool isShowingFrames;
		
		int framesOfNotMoving;
		
		
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
		
		controlCameraLeft = transform.Find ("OVR_anchor/OVRCameraController/CameraLeft").GetComponent<ControlCamera>();
		controlCameraRight = transform.Find ("OVR_anchor/OVRCameraController/CameraRight").GetComponent<ControlCamera>();
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
	
	
	
	void ShowFrameNumbers()
	{
		Singletons.guiManager.ShowMessage(	"ShowFrames: " + kShowFrames + "\n"+
															"HideFrames: " + kHideFrames);
	}
	
	
	
	void Update()
	{
		timeSinceLastLeftFootstep += Time.deltaTime;
		timeSinceLastRightFootstep += Time.deltaTime;
		
		float forwardSpeed = 0;
		float sideSpeed = 0;
		float direction = 0;
		bool isRunning = Input.GetKey (KeyCode.LeftShift );
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
		animator.SetBool ("run", isRunning);
		
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
			
		if ( false == isMovingOrTurning )
			framesOfNotMoving++;
		else
			framesOfNotMoving = 0;
			
		bool shouldStrobe = useStrobing && framesOfNotMoving <= 3;
		controlCameraLeft.useStrobing = shouldStrobe;
		controlCameraRight.useStrobing = shouldStrobe;
		
		
		if ( Input.GetKeyDown (KeyCode.Alpha7 ) )
		{
			kShowFrames = Mathf.Max(1, kShowFrames - 1 );
			ShowFrameNumbers();
		}
		else if ( Input.GetKeyDown (KeyCode.Alpha8 ))
		{
			kShowFrames++;
			ShowFrameNumbers();
		}
		else if ( Input.GetKeyDown (KeyCode.Alpha9 ) )
		{
			kHideFrames = Mathf.Max (1, kHideFrames - 1);
			ShowFrameNumbers();
		}
		else if ( Input.GetKeyDown (KeyCode.Alpha0))
		{
			kHideFrames++;
			ShowFrameNumbers();
		}
		
		
		if ( shouldStrobe )
		{
			if ( isShowingFrames )
			{
				controlCameraLeft.TurnOnForThisFrame();
				controlCameraRight.TurnOnForThisFrame();
				framesShown++;
				if ( framesShown >= kShowFrames )
				{
					isShowingFrames = false;
					framesShown = framesHidden = 0;
				}
			}
			else
			{
				controlCameraLeft.TurnOffForThisFrame();
				controlCameraRight.TurnOffForThisFrame();
				framesHidden++;
				if ( framesHidden >= kHideFrames )
				{
					isShowingFrames = true;
					framesShown = framesHidden = 0;
				}
			}
		}
	}
	
	
}
