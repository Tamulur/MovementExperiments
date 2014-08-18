using UnityEngine;
using System.Collections.Generic;



public class EnvironmentAO : MonoBehaviour
{
	#region fields
	
		List<Material> aoMaterials = new List<Material>();
		Transform playerXform;
		Transform goalXform;
	
	#endregion
	
	
	
	void Awake ()
	{
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
		
		GameObject goalGO = GameObject.Find ("Goal");
		if ( goalGO != null )
			goalXform = goalGO.transform;
			
		playerXform = GameObject.FindObjectOfType<Player>().transform;
		
		foreach ( Renderer rend in GetComponentsInChildren<Renderer>() )
			foreach ( Material mat in rend.materials )
			{
				aoMaterials.Add ( mat );
				if ( goalXform != null )
					mat.SetVector("_Goal", goalXform.position);
			}
	}

	
		
	void OnTimeWarpChanged ( float timeWarp01, float absoluteTimeWarp )
	{
		foreach ( Material mat in aoMaterials )
			mat.SetFloat( "_textureSaturation", timeWarp01 );
	}
	
	
	
	void Update()
	{
		if ( goalXform != null )
		{
			float radius = Vector3.Distance ( playerXform.position, goalXform.position );
			foreach ( Material mat in aoMaterials )
				mat.SetFloat( "_Radius", radius );
		}
	}
	
	
	
}
