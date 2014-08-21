using UnityEngine;


public class ControlCamera : MonoBehaviour
{

	#region fields
	
		const bool kUseDarkness = true;
		
		int layerMaskAll;
		int layerMaskUIonly;
	
		public bool useStrobing
		{
			get {  return _useStrobing; }
			set {	
					if ( _useStrobing != value )
					{
						_useStrobing = value;

						if ( false == _useStrobing )
						{
							darknessGO.SetActive ( false );
							eyeCamera.cullingMask = layerMaskAll;
						}
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
		layerMaskUIonly = Layers.Mask( Layers.Layer.UI );
		layerMaskAll = ~Layers.Mask ( Layers.Layer.PlayerHead );
		darknessGO = transform.Find ("Darkness").gameObject;
	}
	
	
	
	public void TurnOffForThisFrame()
	{
		if ( false == isShowing )
			return;
			
		isShowing = false;
		
		if ( kUseDarkness )
			darknessGO.SetActive ( true );
		else
			eyeCamera.cullingMask = layerMaskUIonly;
	}
	
	
	
	public void TurnOnForThisFrame()
	{
		if ( isShowing )
			return;
			
		isShowing = true;
		
		if ( kUseDarkness )
			darknessGO.SetActive ( false );
		else
			eyeCamera.cullingMask = layerMaskAll;
	}
}
