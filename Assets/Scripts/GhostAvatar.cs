using UnityEngine;



public class GhostAvatar : MonoBehaviour
{

	#region fields
	
		enum State {
			Inactive,
			FadingIn,
			Active,
			FadingOut
		}
		
		State state = State.Inactive;
		GameObject ghostBodyGO;
		
		PlayerHead playerHead;
		Transform playerXform;
		float playerRadius;
		
		Transform capsuleSphere1Xform;
		Transform capsuleSphere2Xform;
		
		Vector3 kUpOffset = new Vector3(0, 0.5f, 0);
		
		int nonPlayerLayerMask;
		
	#endregion
	
	
	
	public void Activate()
	{
		state = State.Active;
	}
	
	
	
	void Awake()
	{
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
		
		ghostBodyGO = transform.Find ("GhostBody").gameObject;
		playerHead = GameObject.FindObjectOfType<PlayerHead>();
		playerXform = playerHead.transform;
		playerRadius = playerXform.GetComponent<CharacterController>().radius;
		
		capsuleSphere1Xform = playerXform.Find ("CapsuleSphere1");
		capsuleSphere2Xform = playerXform.Find ("CapsuleSphere2");
		
		nonPlayerLayerMask = ~(1 << (int) Layers.Layer.PlayerHead | 1 << (int) Layers.Layer.PlayerMain | 1 << (int) Layers.Layer.PlayerLimbs);
	}
	
	
	
	public void Deactivate()
	{
		ghostBodyGO.SetActive( false );
		state = State.Inactive;
	}
	
					
					
	void DropAt( Vector3 point )
	{
		ghostBodyGO.SetActive( true );
		
		RaycastHit hitInfo;
		if ( Physics.SphereCast(	point,
										playerRadius,
		                         		Vector3.down,
		                         		out hitInfo,
		                         		10,
		                        		nonPlayerLayerMask ) )
		{
			transform.position = hitInfo.point;
		}
	}
	
	
	
	public void FadeIn()
	{
		transform.position= playerXform.position;
		transform.rotation = playerXform.rotation;
		state = State.FadingIn;
	}
	
	
	
	public void FadeOut()
	{
		state = State.FadingOut;
	}
	
	
	
	void OnTimeWarpChanged ( float timeWarp01, float absoluteTimeWarp )
	{
		// TODO change transparency?
	}
	
	
	
	void Start()
	{
		Deactivate();
	}
	
	
	
	void Update()
	{
		if ( state == State.Active )
		{
					RaycastHit hitInfo;
			if ( Physics.CapsuleCast(	capsuleSphere1Xform.position + kUpOffset,
			                         			capsuleSphere2Xform.position + kUpOffset,
			                         			capsuleSphere1Xform.localScale.x,
			                         			playerHead.lookDirection,
			                         			out hitInfo,
			                         			Mathf.Infinity,
			                         			nonPlayerLayerMask ) )
			{
				DropAt( capsuleSphere1Xform.position + kUpOffset + playerHead.lookDirection * hitInfo.distance );
			}
			
			transform.Rotate( Vector3.up, Time.deltaTime * 1000 * Input.GetAxis("Mouse X") );
		}
	}
	
	
}
