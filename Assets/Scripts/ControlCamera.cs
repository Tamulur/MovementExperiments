using UnityEngine;


public class ControlCamera : MonoBehaviour
{

	#region fields
	
		public bool useStrobing
		{
			get {  return _useStrobing; }
			set {	
					if ( _useStrobing != value )
					{
						_useStrobing = value;

						if ( false == _useStrobing )
							darknessGO.SetActive ( false );
					}
				}
		}
			bool _useStrobing;
	
		Camera eyeCamera;
		GameObject darknessGO;
		bool isShowing = true;
				
	#endregion
	
	
	
	void Awake()
	{
		eyeCamera = camera;
		darknessGO = transform.Find ("Darkness").gameObject;
	}
	
	
	
	public void TurnOffForThisFrame()
	{
		if ( false == isShowing )
			return;
			
		isShowing = false;
		
		darknessGO.SetActive ( true );
	}
	
	
	
	public void TurnOnForThisFrame()
	{
		if ( isShowing )
			return;
			
		isShowing = true;
		
		darknessGO.SetActive ( false );
	}
}
