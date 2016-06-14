using UnityEngine;



public class PlayerGhostAvatarLook : MonoBehaviour
{

	#region fields
	
		Animator animator;
		PlayerHead playerHead;
		bool hasRecentered;
		
		GhostAvatar ghostAvatar;
		
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
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
		animator = GetComponent<Animator>();
		playerHead = GetComponent<PlayerHead>();
		
		ghostAvatar = FindObjectOfType<GhostAvatar>();
				
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
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
		ChangeToState( State.FadingToGhostMode );
		ghostAvatar.FadeIn ();
		
		Singletons.timeManager.WarpTimeIn( onComplete: OnTimeWarpedIn );
	}
	
	
	
	void FadeToNormalMode()
	{
		ChangeToState( State.FadingToNormal );
		ghostAvatar.FadeOut ();
		
		Singletons.timeManager.WarpTimeOut( onComplete: OnTimeWarpedOut );
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
		ghostAvatar.Activate();
		ChangeToState( State.GhostMode );
	}
	
	
	
	void OnTimeWarpedOut()
	{
		ghostAvatar.Deactivate();
		ChangeToState( State.Normal );
		
		transform.position = ghostAvatar.transform.position;
		transform.rotation = ghostAvatar.transform.rotation;
	}
	
	
	
	void Update()
	{
		if ( Input.GetMouseButtonDown( 1 ) || OVRInput.GetDown(OVRInput.Button.SecondaryShoulder) )
			FadeToGhostMode( );
		else if ( Input.GetMouseButtonUp( 1 ) || OVRInput.GetUp(OVRInput.Button.SecondaryShoulder) )
			FadeToNormalMode( );
	}
	
	
}
