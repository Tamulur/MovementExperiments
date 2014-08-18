using UnityEngine;
using UnityEditor;



public class SetAllShadersToAO : MonoBehaviour
{

	#region fields
		static Shader sDiffuseAOshader;
	#endregion


	static void SetAllShadersToAO_recursively( Transform trans )
	{
		Renderer rend = trans.renderer;
		if ( rend != null )
			foreach (Material mat in rend.sharedMaterials)
				if ( mat.shader.name == "Diffuse" )
					mat.shader = sDiffuseAOshader;
				else
					Debug.LogWarning("No appropriate shader found for " + mat.shader.name + " of object " + trans.name, trans.gameObject);

		foreach ( Transform childTrans in trans )
			SetAllShadersToAO_recursively( childTrans);
	}
	

	[MenuItem ("Window/Set all shaders to AO")]
	static void Init ()
	{
		if ( sDiffuseAOshader == null )
			sDiffuseAOshader = Shader.Find("Diffuse-AO");
		foreach ( Transform trans in Selection.transforms )
			SetAllShadersToAO_recursively( trans );
	}

}
