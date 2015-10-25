using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;



public class MiscUtils
{
	static Regex sFilenameCleanerReg;
	static string sSaveDirectory;
	static string sAppDirectory;
	static readonly Dictionary<string, GameObject> dummyObjects = new Dictionary<string, GameObject>();
	
	
	
	public static GameObject FindChildInHierarchy( GameObject root, string childName )
	{
		if ( root.name == childName )
			return root;
		
		foreach ( Transform t in root.transform )
		{
			GameObject foundGO = FindChildInHierarchy ( t.gameObject, childName );
			if ( foundGO != null )
				return foundGO;
		}
		
		return null;
	}
	
	

	public static T GetComponentInParents<T>(GameObject go) where T:Component
	{
		T target = null;
		while ( target == null && go != null )
		{
			target = go.GetComponent<T>();
			go = go.transform.parent != null ? go.transform.parent.gameObject : null;
		}

		return target;
	}



	public static T GetComponentSafely<T>(string objectName) where T: Component
	{
		GameObject go = GameObject.Find (objectName);
		return (go != null) ? go.GetComponent<T>() : null;
	}
	
	


	public static void CopyTransformsFromTo( Transform fromTrans, Transform toTrans )
	{
		if ( toTrans == null )
			Debug.LogError ("CopyTransformsFromTo: no such transform in to: " + fromTrans.name);
		toTrans.position = fromTrans.position;
		toTrans.rotation = fromTrans.rotation;
		toTrans.localScale = fromTrans.localScale;
		
		foreach ( Transform t in fromTrans )
			CopyTransformsFromTo ( t, toTrans.Find(t.name) );
	}
	
	
	
	public static T ChooseRandomlyBasedOnOrder<T> ( IList<T> list, int limitToCount = -1 )
	{
		if ( list.Count == 0 )
			return default (T);
			
		int count = limitToCount == -1 ? list.Count : Mathf.Min( list.Count, limitToCount );
		int[] weights = new int[ count ];
		int totalWeight = 0;
		for ( int i=0;  i<count;  i++ )
		{	
			int weight = (int) Mathf.Pow((count - i  + 1), 3);
			weights[i] = weight;
			totalWeight += weight;
		}

		int targetWeight = Random.Range( 0, totalWeight -1 );

		int weightSum = 0;
		for ( int i=0;  i<count;  i++)
		{
			weightSum += weights[i];
			if ( weightSum >= targetWeight )
				return list[i];
		}

		Debug.LogError("ChooseRandomlyBasedOnOrder shouldn't get here.");
		return list[0];
	}



    public static GameObject FindInChildren(GameObject go, string name)
    {
        if (go.name == name)
            return go;

        foreach (Transform t in go.transform)
        {
            GameObject foundGO = FindInChildren(t.gameObject, name);
            if (foundGO != null)
                return foundGO;
        }

        return null;
    }



    public static string GetAppDirectory()
	{
		if (sAppDirectory == null)
		{
			string path = Application.dataPath;
			if ( Application.isEditor )
				path += "/../";
		    else if ( Application.platform == RuntimePlatform.OSXPlayer )
		        path += "/../../";
		    else if ( Application.platform == RuntimePlatform.WindowsPlayer )
		        path += "/../";
			
			sAppDirectory = path;
		}
		
		return sAppDirectory;
	}
	
	
	
	public static string GetCleanFilename (string filename)
	{
		if (sFilenameCleanerReg == null)
		{
			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			sFilenameCleanerReg = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
		}
		
		string cleanedFilename = sFilenameCleanerReg.Replace (filename, "").Replace(" ", "_").Replace("/", "_").Replace(":", "_").Replace(".", "_");
		return cleanedFilename;
	}
	
	
	
	public static Vector3 GetHorizontalComponent( Vector3 vector )
	{
		return new Vector3( vector.x, 0, vector.z );
	}
	
	
	
	public static string[] GetLinesFromTextFile( string filename )
	{
		return Regex.Split( File.ReadAllText ( filename ), "\r\n|\r|\n" );
	}
	
	
	
	public static string[] GetLinesFromTextResource( string resourceName )
	{
		TextAsset textAsset = (TextAsset) Resources.Load(resourceName, typeof(TextAsset));
		return Regex.Split( textAsset.text, "\r\n|\r|\n" );
	}
	

	
	public static Quaternion HorizontalRotation( Quaternion rotation )
	{
		Vector3 euler = rotation.eulerAngles;
		return Quaternion.Euler ( new Vector3 ( 0, euler.y, 0 ));
	}
	
	

		// returns the angle in the range -180 to 180
	public static float NormalizedDegAngle ( float degrees )
	{
		int factor = (int) (degrees/360);
		degrees -= factor * 360;
		if ( degrees > 180 )
			return degrees - 360;
		
		if ( degrees < -180 )
			return degrees + 360;
		
		return degrees;
	}
	
	
	
	public static float NormalizedRadAngle ( float rad )
	{
		int factor = (int) (rad/(2*Mathf.PI));
		rad -= factor * 2 * Mathf.PI;
		if ( rad > Mathf.PI )
			return rad - 2 * Mathf.PI;
		
		if ( rad < -Mathf.PI )
			return rad + 2 * Mathf.PI;
		
		return rad;
	}
	
	
	
	public static void PlaceDummyObject ( string name, Vector3 pos, float scale = 0.1f, Quaternion? rotation = null )
	{
		GameObject dummyObject = null;
		
		if ( dummyObjects.ContainsKey(name) )
			dummyObject = dummyObjects[ name ];
		else
		{
			dummyObject = GameObject.CreatePrimitive( PrimitiveType.Cube );
			dummyObject.transform.localScale = scale * Vector3.one;
			dummyObject.GetComponent<Renderer>().material = Resources.Load ("DummyObjectMaterial") as Material;
			GameObject.Destroy (dummyObject.GetComponent<Collider>());
			dummyObject.name = name;
			
			dummyObjects[ name ] = dummyObject;
		}
		
		dummyObject.transform.position = pos;
		dummyObject.transform.rotation = rotation ?? Quaternion.identity;
	}
	
	
	
	public static AudioSource PlayClipAt( AudioClip clip, Vector3 point)
	{
		GameObject tempGO = new GameObject("TempAudio");
		tempGO.transform.position = point;
		AudioSource aSource = tempGO.AddComponent<AudioSource> ();
		aSource.clip = clip;
		aSource.Play();
		GameObject.Destroy(tempGO, clip.length);
		
		return aSource;
	}
	
	

	

	public static void ResetRigidbodies ( Transform t )
	{
		Rigidbody r = t.GetComponent<Rigidbody> ();
		if ( r != null )
		{
			r.velocity = Vector3.zero;
			r.angularVelocity = Vector3.zero;
		}
		
		foreach ( Transform childTrans in t )
			ResetRigidbodies ( childTrans );
	}
	
	

	public static void RotateTowards( Transform transform, Vector3 target, float deltaTime, float horizClamp, float vertClamp, float speed )
	{
		Vector3 lookAtLocalEuler = (Quaternion.Inverse(transform.parent.rotation) * Quaternion.LookRotation(target - transform.position, transform.parent.up)).eulerAngles;
		Vector3 normalizedLocalEuler = new Vector3(NormalizedDegAngle(lookAtLocalEuler.x),
																			NormalizedDegAngle(lookAtLocalEuler.y),
																			NormalizedDegAngle(lookAtLocalEuler.z));
		Vector3 clampedLocalEuler = new Vector3(Mathf.Clamp(normalizedLocalEuler.x, -vertClamp, vertClamp),
																		Mathf.Clamp(normalizedLocalEuler.y, -horizClamp, horizClamp),
																		Mathf.Clamp(normalizedLocalEuler.z, -10, 10));
		transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(clampedLocalEuler), speed * deltaTime);
	}



	public static void SaveTextureToFile(Texture2D texture, string fullFilename)
	{
		var bytes=texture.EncodeToPNG();
		FileStream stream = File.Open(fullFilename, FileMode.Create);
		var binary= new BinaryWriter(stream);
		binary.Write(bytes);
		stream.Close();
	}



	public static void SetRigidbodiesKinematics ( Transform t, bool isKinematic )
	{
		Rigidbody r = t.GetComponent<Rigidbody> ();
		if ( r != null )
			r.isKinematic = isKinematic;
		
		foreach ( Transform childTrans in t )
			SetRigidbodiesKinematics ( childTrans, isKinematic );
	}



	public static void Shuffle<T>(IList<T> list)  
	{  
	    System.Random rng = new System.Random();  
	    int n = list.Count;  
	    while (n > 1) {  
	        n--;  
	        int k = rng.Next(n + 1);  
	        T value = list[k];  
	        list[k] = list[n];  
	        list[n] = value;  
	    }  
	}




}
