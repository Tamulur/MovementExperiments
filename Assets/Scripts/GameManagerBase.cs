using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;



public class GameManagerBase : MonoBehaviour
{
	# region fields

		public bool isGameStarted { get; private set; }

		enum FadeState {
			Normal,
			FadingIn,
			Darkness,
			FadingOut
		}
		FadeState fadeState = FadeState.Darkness;

		ScreenFader screenFader;

	# endregion



	protected virtual void Awake()
	{
		if ( false == Application.isEditor )
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}



	protected virtual void CheckKeyboardForRunningGame()
	{ }
	


	public void FadeIn( float duration=2, Action then = null )
	{
		if ( fadeState != FadeState.Darkness )
		{
			Debug.LogWarning("FadeIn called in state " + fadeState);
			return;
		}

		fadeState = FadeState.FadingIn;
		Singletons.soundManager.TweenVolumeTo(1, duration);
		screenFader.FadeIn( duration:duration,
														onComplete: () => {
														fadeState = FadeState.Normal;
														if ( then != null )
															then();
														});
	}


		
	public void FadeOut( Action then = null )
	{
		if ( fadeState != FadeState.Normal )
		{
			Debug.LogWarning("FadeOut called in state " + fadeState);
			return;
		}

		fadeState = FadeState.FadingOut;
		const float kDuration = 2;
		Singletons.soundManager.TweenVolumeTo(0, kDuration);
		screenFader.FadeOut(duration: kDuration, onComplete: () => {
											fadeState = FadeState.Darkness;
											if ( then != null )
												then();
											});
	}



	void Reset()
	{
		OVRManager.display.RecenterPose();
	}
	

	
	void Start()
	{
		screenFader = FindObjectOfType<ScreenFader>();

		screenFader.FadeIn( duration: Application.isEditor ? 2 : 7, onComplete: StartGame );
	}



	protected virtual void StartGame()
	{
		isGameStarted = true;
	}



	protected virtual void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		if ( fadeState == FadeState.FadingIn || fadeState == FadeState.FadingOut )
			return;


		//*** Check keyboard
		{
			if ( Input.GetKeyDown (KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.One) )
				Reset();

			if ( isGameStarted )
				CheckKeyboardForRunningGame();
		}
	}

}
