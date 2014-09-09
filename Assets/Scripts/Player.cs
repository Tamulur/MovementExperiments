using UnityEngine;



public class Player : MonoBehaviour
{

	#region fields
	
		Animator animator;
		PlayerHead playerHead;
		bool hasRecentered;
		MotionController motionController;
		Transform originalParentXform;
		Transform avatarXform;
		OVRCameraController ovrCameraController;
		
		
		enum ControlMode {
			Standard,
			CanvasTexture,
			ThirdPerson,
			Stroboscopic
		}
		ControlMode controlMode = ControlMode.Standard;
		
		
		enum State {
			Normal,
			GhostMode,
			FadingToGhostMode,
			FadingToNormal
		}
		State state = State.Normal;
		
		GoTween stateTween;
		
		
	#endregion
	
	
	
	void Awake()
	{
		Screen.showCursor = false;
		Screen.lockCursor = true;
		
		avatarXform = GameObject.Find("Player").transform;
		
		animator = avatarXform.GetComponent<Animator>();
		playerHead = avatarXform.GetComponent<PlayerHead>();
		motionController = avatarXform.GetComponent<MotionController>();
		
		originalParentXform = transform.parent;
		ovrCameraController = GetComponentInChildren<OVRCameraController>();
		
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
	}
	
	
	
	void ChangeControlMode( ControlMode newControlMode )
	{
		controlMode = newControlMode;
		
		motionController.useCanvas = controlMode == ControlMode.CanvasTexture;
		
		switch ( controlMode )
		{
			case ControlMode.Standard:
				TeleportViewToAvatar();
				transform.parent = originalParentXform;
				motionController.Activate();
				ShowControlMenu( duration: 0.75f );
			break;

			case ControlMode.CanvasTexture:
				TeleportViewToAvatar();
				transform.parent = originalParentXform;
				motionController.Activate();
				Singletons.guiManager.ShowMessage("Canvas mode:\n" +
																		"Press G to cycle through\n" +
																		"different canvas textures.", duration: 5);
			break;
				
			case ControlMode.ThirdPerson:
				transform.parent = null;
				motionController.Deactivate();
				Singletons.guiManager.ShowMessage(	"Third Person Control:\n" +
																		"Keep right mouse button\n"+	
																		"pressed and move your avatar,\n" +
																		"then release to teleport your view.", duration: 5);
			break;
			
			case ControlMode.Stroboscopic:
				TeleportViewToAvatar();
				transform.parent = originalParentXform;
				motionController.Activate();
				Singletons.guiManager.ShowMessage (	"Stroboscopic:\n" +
																			"7: decrease ShownFrames\n" +
																			"8: increase ShownFrames\n" +
																			"9: decrease HiddenFrames\n" +
																			"0: increase HiddenFrames\n", duration: 5);
			break;
		}
		
		motionController.useStrobing = controlMode == ControlMode.Stroboscopic;
	}
	
	
	
	void ChangeToState( State newState )
	{
		state = newState;
	}
	
	
	
	public void DisableMovement()
	{
		animator.SetFloat("speed", 0);
	}
	
	
	
	
	void FadeToGhostMode()
	{
			//*** Place camera behind the avatar, so we can see him turning in place
			{
				Vector3 horizViewDirection = playerHead.lookDirection;
				horizViewDirection.y = 0;
				horizViewDirection.Normalize();
				transform.position = transform.position -1.5f * horizViewDirection + 0.0f * Vector3.up;
			}

		ChangeToState( State.FadingToGhostMode );
		
		Singletons.timeManager.WarpTimeIn( onComplete: OnTimeWarpedIn );
	}
	
	
	
	void FadeToNormalMode()
	{
		ChangeToState( State.FadingToNormal );
		
		Singletons.timeManager.WarpTimeOut( onComplete: OnTimeWarpedOut );
		
		motionController.Deactivate();
	}
	
	
	
	public bool IsPlayerLayer( Layers.Layer layer )
	{
		return layer == Layers.Layer.PlayerMain || layer == Layers.Layer.PlayerHead || layer == Layers.Layer.PlayerLimbs;
	}
	
	
	
	void OnTimeWarpChanged ( float timeWarp01, float absoluteTimeWarp )
	{
	}
	
	
	
	void OnTimeWarpedIn()
	{
		ChangeToState( State.GhostMode );

		motionController.Activate();
	}
	
	
	
	void OnTimeWarpedOut()
	{
		ChangeToState( State.Normal );
		
		TeleportViewToAvatar();
	}
	
	
	
	void Reset()
	{
		OVRDevice.ResetOrientation();
		playerHead.Reset();
	}
	
	
	
	void ShowControlMenu( float duration = 3)
	{
		string oT1, cT1, oT2, cT2, oT3, cT3, oT4, cT4;
		oT1 = cT1 = oT2 = cT2 = oT3 = cT3 = oT4 = cT4 = "";
		
				const string kSelectedOpenTag = "<b><color=\"#00CCFF\">";
				const string kSelectedCloseTag = "</color></b>";
		if ( controlMode == ControlMode.Standard )
		{
			oT1 = kSelectedOpenTag;
			cT1 = kSelectedCloseTag;
		}
		else if ( controlMode == ControlMode.CanvasTexture )
		{
			oT2 = kSelectedOpenTag;
			cT2 = kSelectedCloseTag;
		}
		else if ( controlMode == ControlMode.ThirdPerson )
		{
			oT3 = kSelectedOpenTag;
			cT3 = kSelectedCloseTag;
		}
		else if ( controlMode == ControlMode.Stroboscopic )
		{
			oT4 = kSelectedOpenTag;
			cT4 = kSelectedCloseTag;
		}
		
		Singletons.guiManager.ShowMessage (	"Control Mode:\n\n" +	
															oT1 + "1: Standard" + cT1 + "\n" +
															oT2 + "2: Canvas" + cT2 + "\n" +
															oT3 + "3: Third person" + cT3 + "\n" +
															oT4 + "4: Stroboscopic" + cT4,
															duration: duration,
															notificationMode: GUIManager.NotificationMode.FadeInOut );
	}
	
	
	
	void TeleportViewToAvatar()
	{
		transform.position = originalParentXform.position;
		transform.rotation = originalParentXform.rotation;
		
//		ovrCameraController.SetYRotation( transform.rotation.eulerAngles.y );
	}
	
	
	
	void Update()
	{
		if ( controlMode == ControlMode.ThirdPerson && Input.GetMouseButtonDown( 1 ) )
			FadeToGhostMode( );
		else if ( controlMode == ControlMode.ThirdPerson && Input.GetMouseButtonUp( 1 ) )
			FadeToNormalMode( );
			
		bool isNormalMode = state == State.Normal;
		
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
		else if ( isNormalMode && hasRecentered && Input.GetKeyDown ( KeyCode.Alpha1 ) )
			ChangeControlMode( ControlMode.Standard );
		else if ( isNormalMode && hasRecentered && Input.GetKeyDown ( KeyCode.Alpha2 ) )
			ChangeControlMode ( ControlMode.CanvasTexture );
		else if ( isNormalMode && hasRecentered && Input.GetKeyDown ( KeyCode.Alpha3 ) )
			ChangeControlMode ( ControlMode.ThirdPerson );
		else if ( isNormalMode && hasRecentered && Input.GetKeyDown ( KeyCode.Alpha4 ) )
			ChangeControlMode ( ControlMode.Stroboscopic );
		else if ( Input.GetKeyDown (KeyCode.Space))
		{
			Reset();
			
			if ( false == OVRDevice.HMD.GetHSWDisplayState().Displayed && false == hasRecentered )
			{
				ShowControlMenu( duration: 5 );
				hasRecentered = true;
			}
		}
	}
	
	
}
