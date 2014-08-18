using UnityEngine;



public class ControlCamera : MonoBehaviour
{

	#region fields
	
				bool _useCanvas;
		public bool useCanvas {
				get { return _useCanvas; }
				set  {
					if ( value != _useCanvas )
					{
						canvasPlaneGO.SetActive( value );
						_useCanvas = value;
					}
				}
			}
			
			
		GameObject canvasPlaneGO;
		
	#endregion
	
	
	
	void Awake()
	{
		canvasPlaneGO = transform.Find("CanvasPlane").gameObject;
	}
	
	
	
}
