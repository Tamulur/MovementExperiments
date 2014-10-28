using UnityEngine;
using System.Collections.Generic;



public class PlayerHead : MonoBehaviour
{

	#region fields

		public bool useNeckBone = false;
		
		Quaternion headCorrectionRot;
		
		public Vector3 eyeCenter { get { return eyeCenterTransform.position; }}
		public Quaternion lookRotation { get { return eyeCenterTransform.rotation; }}
		public Vector3 lookDirection { get; private set; }
		
		
		Transform anchorBoneTransform;
		Transform eyeCenterTransform;
		Transform ovrXform;
		Animator animator;
		
		readonly List<Material> headMaterials = new List<Material>();
		
	#endregion



	void Awake()
	{
		ovrXform = transform.Find ("OVR_anchor/OVRCameraRig");
					
		animator = GetComponent<Animator>();
		anchorBoneTransform = animator.GetBoneTransform( useNeckBone	? HumanBodyBones.Neck
																											: HumanBodyBones.Head );
																									
		eyeCenterTransform = GameObject.Find("CenterEyeAnchor").transform;
		
		foreach ( Renderer rend in GetComponentsInChildren<Renderer>() )
			if ( rend.gameObject.layer == (int) Layers.Layer.PlayerHead )
				foreach ( Material mat in rend.materials )
					headMaterials.Add ( mat );

		headCorrectionRot = Quaternion.Inverse(Quaternion.LookRotation(transform.forward)) * anchorBoneTransform.rotation;
	}
	


	void LateUpdate()
	{
		lookDirection = eyeCenterTransform.forward;
		anchorBoneTransform.rotation = transform.rotation * Quaternion.Inverse( ovrXform.rotation ) * lookRotation * headCorrectionRot;
	}
	
	

	void Update()
	{
		float distanceHeadToCameras = Vector3.Distance ( eyeCenterTransform.position, anchorBoneTransform.position );
		float alpha = Mathf.InverseLerp(0.2f, 0.5f, Mathf.Clamp( distanceHeadToCameras, 0.2f, 0.5f ) );
		Color color = new Color( 1,1,1,alpha );
		foreach( Material mat in headMaterials )
			mat.color = color;
	}
	
	
}
