using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;


public class Startup : MonoBehaviour
{
	
	bool isFadedIn;


	void Start()
	{
		FindObjectOfType<ScreenFader>().FadeIn(onComplete: () => { isFadedIn = true; });
	}



	void Update()
	{
		if ( false == isFadedIn )
			return;

		if ( Input.GetKeyDown(KeyCode.Escape) )
			Application.Quit();
		else if ( Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.One) )
		{
			InputTracking.Recenter();

			FindObjectOfType<ScreenFader>().FadeOut(onComplete: () => SceneManager.LoadScene(1) );
		}
	}


}
