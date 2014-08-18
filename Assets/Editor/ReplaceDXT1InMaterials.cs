using UnityEngine;
using UnityEditor;
using System.IO;


public class ReplaceDXT1InMaterials : MonoBehaviour
{

	#region fields

	#endregion


	static void ReplaceDXT1( Transform trans )
	{
		Renderer rend = trans.renderer;
		if ( rend != null )
			foreach (Material mat in rend.sharedMaterials)
			{
				Texture2D texture = mat.mainTexture as Texture2D;
				if ( texture.format == TextureFormat.DXT1 || texture.format == TextureFormat.DXT5 )
				{
					string fullPath = AssetDatabase.GetAssetPath(texture);
					string directory = Path.GetDirectoryName(fullPath);
					string fullPathWithoutExtention = directory + "/" + Path.GetFileNameWithoutExtension(fullPath);
					string jpgPath = fullPathWithoutExtention + ".jpg";
					string pngPath = fullPathWithoutExtention + ".png";
					if ( File.Exists(jpgPath ))
						mat.mainTexture = AssetDatabase.LoadAssetAtPath( jpgPath, typeof(Texture2D) ) as Texture2D;
					else if ( File.Exists( pngPath ))
						mat.mainTexture = AssetDatabase.LoadAssetAtPath( pngPath, typeof(Texture2D) ) as Texture2D;
					else
						Debug.LogWarning("Couldn't find jpg or png texture to replace " + texture.name + " of object " + trans.name);
				}
			}

		foreach ( Transform childTrans in trans )
			ReplaceDXT1( childTrans);
	}
	

	[MenuItem ("Window/Replace DXT1 textures")]
	static void Init ()
	{
		foreach ( Transform trans in Selection.transforms )
			ReplaceDXT1( trans );
	}

}
