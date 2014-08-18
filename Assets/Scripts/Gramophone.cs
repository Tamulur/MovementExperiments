using UnityEngine;



public class Gramophone : MonoBehaviour
{
	#region fields
	
		Transform discXform;
//		SoundSource soundSource;
		
	#endregion
	
	
	
	void Awake()
	{
		discXform = transform.Find ("Old_gramophone_Disc_Bone");
//		soundSource = transform.Find ("Loudspeaker").GetComponent<SoundSource>();
	}
	
	
	
//	void Start()
//	{
//		soundSource.Play();
//	}
	
	
	
	void Update()
	{
		discXform.Rotate( Vector3.right, Time.deltaTime * 30 );
	}
	
}
