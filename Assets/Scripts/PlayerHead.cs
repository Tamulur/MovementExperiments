using UnityEngine;
using System;
using System.Collections.Generic;



public class PlayerHead : MonoBehaviour
{

	#region fields

		public bool useNeckBone = false;
		
		//public Vector3 euler;
		Quaternion headCorrectionRot = Quaternion.Euler (-180, 0, 90);
		
		public Vector3 eyeCenter { get { return eyeCenterTransform.position; }}
		public Quaternion lookRotation { get { return eyeCenterTransform.rotation; }}
		public Vector3 lookDirection { get; private set; }
		
		
		Transform cameraLeftTransform;
		Transform cameraRightTransform;
		Transform anchorBoneTransform;
		Transform eyeCenterTransform;
		Transform ovrXform;
		Animator animator;
		
		List<Material> headMaterials = new List<Material>();
		
	#endregion



	void Awake()
	{
		ovrXform = transform.Find ("OVR_anchor/OVRCameraController");
		cameraLeftTransform = ovrXform.Find("CameraLeft");
		cameraRightTransform = ovrXform.Find("CameraRight");
					
		animator = GetComponent<Animator>();
		anchorBoneTransform = animator.GetBoneTransform( useNeckBone	? HumanBodyBones.Neck
																											: HumanBodyBones.Head );
																									
				OVRCameraController ovrCameraController = ovrXform.GetComponent<OVRCameraController>();
		eyeCenterTransform = new GameObject("Player eye center").transform;
		eyeCenterTransform.position = ovrXform.TransformPoint(ovrCameraController.NeckPosition);
		eyeCenterTransform.rotation = ovrXform.rotation;
		
		foreach ( Renderer rend in GetComponentsInChildren<Renderer>() )
			if ( rend.gameObject.layer == (int) Layers.Layer.PlayerHead )
				foreach ( Material mat in rend.materials )
					headMaterials.Add ( mat );
	}
	


	void LateUpdate()
	{
		eyeCenterTransform.position = 0.5f * (cameraLeftTransform.position + cameraRightTransform.position);
		eyeCenterTransform.rotation = Quaternion.Slerp(cameraLeftTransform.rotation, cameraRightTransform.rotation, 0.5f);
		lookDirection = 0.5f * (cameraLeftTransform.forward + cameraRightTransform.forward);
		
		
		//anchorBoneTransform.rotation = lookRotation * Quaternion.Euler(euler);
		anchorBoneTransform.rotation = Quaternion.FromToRotation( ovrXform.forward, transform.forward) *  lookRotation * headCorrectionRot;
	}
	
	

	public void Reset()
	{  }
	
	
	
	void Update()
	{
		float distanceHeadToCameras = Vector3.Distance ( eyeCenterTransform.position, anchorBoneTransform.position );
		float alpha = Mathf.InverseLerp(0.2f, 0.5f, Mathf.Clamp( distanceHeadToCameras, 0.2f, 0.5f ) );
		Color color = new Color( 1,1,1,alpha );
		foreach( Material mat in headMaterials )
			mat.color = color;
	}
	
	
}
