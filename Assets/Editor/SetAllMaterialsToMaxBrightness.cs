using UnityEngine;
using UnityEditor;



public class SetAllMaterialsToMaxBrightness : MonoBehaviour
{

	#region fields

	#endregion


	static void BrightenColors( Transform trans )
	{
		Renderer rend = trans.renderer;
		if ( rend != null )
			foreach (Material mat in rend.sharedMaterials)
				mat.color = Color.white;

		foreach ( Transform childTrans in trans )
			BrightenColors( childTrans);
	}
	

	[MenuItem ("Window/Brighten all material colors")]
	static void Init ()
	{
		foreach ( Transform trans in Selection.transforms )
			BrightenColors( trans );
	}

}
