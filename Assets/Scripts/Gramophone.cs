using UnityEngine;



public class Gramophone : MonoBehaviour
{
	#region fields
	
		Transform discXform;
		AudioSource soundSource;
		
	#endregion
	
	
	
	void Awake()
	{
		discXform = transform.Find ("Old_gramophone_Disc_Bone");
		Singletons.gameManager.OnGameStart += OnGameStart;
		soundSource = GetComponentInChildren<AudioSource>();
	}
	
	
	
	void OnGameStart()
	{
		soundSource.Play();
	}
	
	
	
	void Update()
	{
		discXform.Rotate( Vector3.right, Time.deltaTime * 30 );
	}
	
}
