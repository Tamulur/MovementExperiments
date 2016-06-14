using UnityEngine;



public class Gramophone : MonoBehaviour
{
	#region fields
	
		Transform discXform;
		
	#endregion
	
	
	
	void Awake()
	{
		discXform = transform.Find ("Old_gramophone_Disc_Bone");
	}
	
	
	
	void Update()
	{
		discXform.Rotate( Vector3.right, Time.deltaTime * 30 );
	}
	
}
