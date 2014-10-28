using System;
using System.Collections;
using UnityEngine;



public class Player : MonoBehaviour
{

	#region fields
	

		Animator animator;
		PlayerHead playerHead;
		MotionController motionController;
		Transform originalParentXform;
		Transform avatarXform;
		HoloGrid darkness;
		
		
		enum ControlMode {
			Standard,
			CanvasTexture,
			ThirdPerson,
			StepTeleport,
			Stroboscopic
		}
		ControlMode controlMode = ControlMode.Standard;
		
		
		enum State {
			Normal,
			GhostMode,
			FadingToGhostMode,
			FadingToNormal,
			Teleporting
		}
		State state = State.Normal;
		
		GoTween stateTween;
		GhostAvatar ghostAvatar;

		float teleportStepDuration = 0.05f;

	#endregion
	
	
	
	void Awake()
	{
		darkness = MiscUtils.FindChildInHierarchy(gameObject, "CenterEyeAnchor").transform.Find("Darkness").GetComponent<HoloGrid>();
		
		avatarXform = GameObject.Find("Player").transform;
		ghostAvatar = FindObjectOfType<GhostAvatar>();
		
		animator = avatarXform.GetComponent<Animator>();
		playerHead = avatarXform.GetComponent<PlayerHead>();
		motionController = avatarXform.GetComponent<MotionController>();
		
		originalParentXform = transform.parent;
		
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
		Singletons.gameManager.OnGameStart += OnGameStart;
	}
	
	
	
	void ChangeControlMode( ControlMode newControlMode )
	{
		if ( controlMode == newControlMode )
			return;

		//*** Exit old control mode
		{
			if ( controlMode == ControlMode.ThirdPerson )
			{
				TeleportViewToAvatar();
				transform.parent = originalParentXform;
				motionController.Activate();
			}
			else if ( controlMode == ControlMode.StepTeleport )
			{
				motionController.Activate();
			}
		}

		controlMode = newControlMode;
		
		motionController.useCanvas = controlMode == ControlMode.CanvasTexture;
		
		switch ( controlMode )
		{
			case ControlMode.Standard:
				ShowControlMenu( duration: 0.75f );
				break;

			case ControlMode.CanvasTexture:
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
			
			case ControlMode.StepTeleport:
				motionController.Deactivate();
				Singletons.guiManager.ShowMessage("Stepwise teleport mode:\n" +
																		"Keep right mouse button pressed and\n"+
																		"look where you want to go. Then release.\n" +
																		"7, 8: dec-/increase step size\n" +
																		"9, 0: dec-/increase step duration\n", duration: 5);
				break;

			case ControlMode.Stroboscopic:
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
	
	
	
	
	public void FadeFromDark( Action onComplete, float duration=2 )
	{
		darkness.SetUseTransparentShader( true );

		Singletons.soundManager.TweenVolumeTo(1, duration);
		darkness.TweenTo( 0, duration: duration, onComplete: () => {	darkness.Deactivate();
																										onComplete(); });
	}



	public void FadeToDark( Action onComplete )
	{
		darkness.SetUseTransparentShader( true );

				const float kDuration = 2;
		Singletons.soundManager.TweenVolumeTo(0, kDuration);
		darkness.TweenTo( 1, duration: kDuration, onComplete: () => {	darkness.SetUseTransparentShader( false );
																										onComplete(); });
	}



	void FadeToGhostMode()
	{
		if ( controlMode == ControlMode.ThirdPerson )
		{
			//*** Place camera behind the avatar, so we can see him turning in place
			{
				Vector3 horizViewDirection = playerHead.lookDirection;
				horizViewDirection.y = 0;
				horizViewDirection.Normalize();
				transform.position = transform.position -1.5f * horizViewDirection + 0.0f * Vector3.up;
			}
		}
		else if ( controlMode == ControlMode.StepTeleport )
			ghostAvatar.FadeIn ();

		ChangeToState( State.FadingToGhostMode );

		Singletons.timeManager.WarpTimeIn( onComplete: OnTimeWarpedIn );
	}
	
	
	
	void FadeToNormalMode()
	{
		ChangeToState( State.FadingToNormal );

		if ( controlMode == ControlMode.StepTeleport )
			ghostAvatar.FadeOut ();
		
		Singletons.timeManager.WarpTimeOut( onComplete: OnTimeWarpedOut );
		
		motionController.Deactivate();
	}
	
	
	
	public bool IsPlayerLayer( Layers.Layer layer )
	{
		return layer == Layers.Layer.PlayerMain || layer == Layers.Layer.PlayerHead || layer == Layers.Layer.PlayerLimbs;
	}
	
	
	
	void OnGameStart()
	{
		ShowControlMenu();
	}



	void OnTimeWarpChanged ( float timeWarp01, float absoluteTimeWarp )
	{ }
	
	
	
	void OnTimeWarpedIn()
	{
		ChangeToState( State.GhostMode );

		if ( controlMode == ControlMode.ThirdPerson )
			motionController.Activate();
		else if ( controlMode == ControlMode.StepTeleport )
			ghostAvatar.Activate();
	}
	
	
	
	void OnTimeWarpedOut()
	{
		ChangeToState( State.Normal );

		if ( controlMode == ControlMode.StepTeleport )
			ghostAvatar.Deactivate();
		
		if ( controlMode == ControlMode.ThirdPerson )
			TeleportViewToAvatar();
		else if ( controlMode == ControlMode.StepTeleport && ghostAvatar.stepPoints.Count > 0 )
			TeleportStepwiseAlongPath();
	}
	
	
	
	public void SetToDark()
	{
		darkness.Activate( 1 );
		darkness.SetUseTransparentShader( false );
		Singletons.soundManager.volume = 0;
	}	



	void ShowControlMenu( float duration = 3)
	{
		string oT1, cT1, oT2, cT2, oT3, cT3, oT4, cT4, oT5, cT5;
		oT1 = cT1 = oT2 = cT2 = oT3 = cT3 = oT4 = cT4 = oT5 = cT5 = "";
		
				const string kSelectedOpenTag = "<b><color=\"#00CCFF\">";
				const string kSelectedCloseTag = "</color></b>";
		switch (controlMode)
		{
			case ControlMode.Standard:
				oT1 = kSelectedOpenTag;
				cT1 = kSelectedCloseTag;
				break;
			case ControlMode.CanvasTexture:
				oT2 = kSelectedOpenTag;
				cT2 = kSelectedCloseTag;
				break;
			case ControlMode.ThirdPerson:
				oT3 = kSelectedOpenTag;
				cT3 = kSelectedCloseTag;
				break;
			case ControlMode.StepTeleport:
				oT4 = kSelectedOpenTag;
				cT4 = kSelectedCloseTag;
				break;
			case ControlMode.Stroboscopic:
				oT5 = kSelectedOpenTag;
				cT5 = kSelectedCloseTag;
				break;
		}
		
		Singletons.guiManager.ShowMessage (	"Control Mode:\n\n" +	
																	oT1 + "1: Standard" + cT1 + "\n" +
																	oT2 + "2: Canvas" + cT2 + "\n" +
																	oT3 + "3: Third person" + cT3 + "\n" +
																	oT4 + "4: Stepwise teleport" + cT4 +
																	oT5 + "4: Stroboscopic" + cT5,
																	duration: duration );
	}
	
	
	
	void TeleportStepwiseAlongPath()
	{
		ChangeToState( State.Teleporting );

		StartCoroutine( TeleportToStepStone( 0 ) );
	}



	IEnumerator TeleportToStepStone( int stepStoneIndex )
	{
		Transform stepStoneXform= ghostAvatar.stepPoints[ stepStoneIndex ];
		avatarXform.position = stepStoneXform.position;

		if ( stepStoneIndex + 1 < ghostAvatar.numStepPoints )
		{
			yield return new WaitForSeconds( teleportStepDuration );
			avatarXform.rotation = Quaternion.Euler(0, stepStoneXform.rotation.eulerAngles.y, 0);
			StartCoroutine( TeleportToStepStone( stepStoneIndex + 1) );
		}
		else
		{
			ChangeToState( State.Normal );
			avatarXform.rotation = ghostAvatar.transform.rotation;
			yield break;
		}
	}



	void TeleportViewToAvatar()
	{
		transform.position = originalParentXform.position;
		transform.rotation = originalParentXform.rotation;
	}
	
	
	
	void Update()
	{
		if ( false == Singletons.gameManager.isGameStarted )
			return;

				bool controlModeUsesRightMouseButton = controlMode == ControlMode.ThirdPerson || controlMode == ControlMode.StepTeleport;
		if ( controlModeUsesRightMouseButton && Input.GetMouseButtonDown( 1 ) )
			FadeToGhostMode( );
		else if ( controlModeUsesRightMouseButton && Input.GetMouseButtonUp( 1 ) )
			FadeToNormalMode( );
			
		bool isNormalMode = state == State.Normal;
		
		if ( isNormalMode && Input.GetKeyDown ( KeyCode.Alpha1 ) )
			ChangeControlMode( ControlMode.Standard );
		else if ( isNormalMode && Input.GetKeyDown ( KeyCode.Alpha2 ) )
			ChangeControlMode ( ControlMode.CanvasTexture );
		else if ( isNormalMode && Input.GetKeyDown ( KeyCode.Alpha3 ) )
			ChangeControlMode ( ControlMode.ThirdPerson );
		else if ( isNormalMode && Input.GetKeyDown ( KeyCode.Alpha4 ) )
			ChangeControlMode ( ControlMode.StepTeleport );
		else if ( isNormalMode && Input.GetKeyDown ( KeyCode.Alpha5 ) )
			ChangeControlMode ( ControlMode.Stroboscopic );

		else if ( controlMode == ControlMode.StepTeleport && Input.GetKeyDown( KeyCode.Alpha9 ) )
		{
			teleportStepDuration = Mathf.Max( 0.01f, teleportStepDuration - 0.01f );
			Singletons.guiManager.ShowMessage("Teleport step duration: " + teleportStepDuration);
		}
		else if ( controlMode == ControlMode.StepTeleport && Input.GetKeyDown( KeyCode.Alpha0 ) )
		{
			teleportStepDuration = Mathf.Min( 0.2f, teleportStepDuration + 0.01f );
			Singletons.guiManager.ShowMessage("Teleport step duration: " + teleportStepDuration);
		}
		else if ( controlMode == ControlMode.StepTeleport && Input.GetKeyDown( KeyCode.Alpha7 ) )
		{
			ghostAvatar.stepSize = Mathf.Max( 0.1f, ghostAvatar.stepSize - 0.2f );
			Singletons.guiManager.ShowMessage("Stepsize: " + ghostAvatar.stepSize);
		}
		else if ( controlMode == ControlMode.StepTeleport && Input.GetKeyDown( KeyCode.Alpha8 ) )
		{
			ghostAvatar.stepSize = Mathf.Min( 10f, ghostAvatar.stepSize + 0.2f );
			Singletons.guiManager.ShowMessage("Stepsize: " + ghostAvatar.stepSize);
		}
	}
	
	
}
