using UnityEngine;



public class CanvasSphere : MonoBehaviour
{
	#region fields
	
		const float kShownAlpha = 73.0f/255.0f;
		
		public bool shouldBeShown { get; set;  }
		
				float _alpha;
		public float alpha {
			get { return _alpha; }
			set {
				_alpha = value;
				canvasMaterial.color = new Color ( 1,1,1,alpha );
			}
		}
		
		Material canvasMaterial;
	
		
	#endregion
	
	
	
	void Awake()
	{
		canvasMaterial = GetComponentInChildren<Renderer>().material;
		_alpha = canvasMaterial.color.a;
	}
	
	
	
	void Update()
	{
		float targetAlpha = shouldBeShown ? kShownAlpha : 0;
		float speed = targetAlpha < alpha ? 0.5f : 1;
		alpha = Mathf.Lerp ( alpha, targetAlpha, Time.unscaledDeltaTime * 10 * speed);
	}
	
}
