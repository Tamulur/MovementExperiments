using UnityEngine;
using System.Collections;
using System;



public class GUIManager : MonoBehaviour
{
	#region fields
	
		public float kFadeDuration = 1.0f;
	
		const float kMessageInDuration = 0.25f;
	
		public float notification {
				get { return _notification; }
				set { 
						_notification = value;
						if ( notificationMode == NotificationMode.FadeInOut || notificationMode == NotificationMode.FadeOut )
							notificationText.color = new Color (notificationColor.r, notificationColor.g, notificationColor.b, value);
						else
							Debug.LogError("Unknown notification mode: " + notificationMode);
					}
		}
			float _notification;
	
	
		System.Action onMessageFinished;
		
		GoTween notificationTween;
	
		float notificationRestDuration;
	
		enum NotificationStatus
		{
			Nothing,
			MovingIn,
			Showing,
			MovingOut
		}
		NotificationStatus notificationStatus = NotificationStatus.Nothing;
	
		public enum NotificationMode
		{
			FadeInOut,
			FadeOut
		}
		NotificationMode notificationMode;
	
		Color notificationColor;
		TextMesh notificationText;
	
	#endregion
	
	
	
	void Awake()
	{
		notificationText = GetComponentInChildren<TextMesh>();
		notificationColor = notificationText.color;
	}
	
	
	
	public void HideGUI()
	{
		if ( notificationStatus != NotificationStatus.Nothing )
		{
			if (  notificationTween != null )
			{
				notificationTween.destroy ();
				notificationTween = null;
			}
			notificationText.text = "";
			notificationStatus = NotificationStatus.Nothing;
		}
		
		notificationText.gameObject.SetActive( false );
	}
	
	
	
	void OnMessageMovedIn()
	{
		notificationStatus = NotificationStatus.Showing;
	}
	
	
	
	void OnMessageMovedOut()
	{
		notificationStatus = NotificationStatus.Nothing;
		notificationText.gameObject.SetActive( false );
		
		if ( onMessageFinished != null )
			onMessageFinished();
	}
	
	
	
	public void ShowGUI()
	{
		notificationText.gameObject.SetActive( true );
	}
	
	
	
	public void ShowMessage(	string message,
										float duration = 3.0f,
										float delay = 0,
	                        			NotificationMode notificationMode = NotificationMode.FadeInOut,
										System.Action onComplete = null )
	{
		ShowGUI ();
		onMessageFinished = onComplete;
			
		notificationText.gameObject.SetActive( true );
		notificationText.text = message;
		notificationRestDuration = duration;
		this.notificationMode = notificationMode;
		
		if ( notificationTween != null )
		{
			notificationTween.destroy ();
			notificationTween = null;
		}
		
		if ( notificationMode == NotificationMode.FadeOut )
		{
			notification = 1;
			notificationStatus = NotificationStatus.Showing;
			OnMessageMovedIn();
		}
		else
		{
			notification = 0;
			notificationTween = Go.to ( this, kMessageInDuration, new GoTweenConfig()
											.floatProp("notification", 1)
											.setDelay (delay)
											.onComplete( t => { OnMessageMovedIn(); notificationTween = null; } ));
			notificationStatus = NotificationStatus.MovingIn;
		}
	}
	
	
	
	void Update()
	{
		if ( notificationStatus == NotificationStatus.Showing )
		{
			if (notificationRestDuration > 0 )
			{
				float deltaTime = Mathf.Min( 0.02f, Time.unscaledDeltaTime );
				notificationRestDuration -= deltaTime;
				if ( notificationRestDuration <= 0 )
				{
					if ( notificationTween != null )
						notificationTween.destroy ();
					
					notificationTween = Go.to (this, kMessageInDuration, new GoTweenConfig()
													.floatProp("notification", 0)
													.onComplete( t => { OnMessageMovedOut(); notificationTween = null; } ));
					notificationStatus = NotificationStatus.MovingOut;
				}
			}
		}
	}
	
	
}
