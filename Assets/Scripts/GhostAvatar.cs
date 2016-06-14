using UnityEngine;
using System.Collections.Generic;



public class GhostAvatar : MonoBehaviour
{

	#region fields
	
		public float stepSize { get; set; }
		public List<Transform> stepPoints { get; private set; }
		public int numStepPoints { get; private set; }

		GameObject stepPointPrefab;

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
		
		readonly Vector3 kUpOffset = new Vector3(0, 0.5f, 0);

		readonly Vector3 kPathDrawVerticalOffset = new Vector3( 0, 0.4f, 0);
		
		int nonPlayerLayerMask;
		NavMeshPath path;
		LineRenderer lineRenderer;


	#endregion
	
	
	
	public void Activate()
	{
		state = State.Active;
		transform.rotation = playerXform.rotation;
		numStepPoints = 0;
	}
	
	
	
	void Awake()
	{
		stepSize = 3;
		stepPoints = new List<Transform>();
		Singletons.timeManager.OnTimeWarpChangedEvent += OnTimeWarpChanged;
		
		ghostBodyGO = transform.Find ("GhostBody").gameObject;
		playerHead = FindObjectOfType<PlayerHead>();
		playerXform = playerHead.transform;
		playerRadius = playerXform.GetComponent<CharacterController>().radius;
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.enabled = false;
		
		stepPointPrefab = Resources.Load<GameObject>("StepPoint");

		capsuleSphere1Xform = playerXform.Find ("CapsuleSphere1");
		capsuleSphere2Xform = playerXform.Find ("CapsuleSphere2");
		
		nonPlayerLayerMask = ~(1 << (int) Layers.Layer.PlayerHead | 1 << (int) Layers.Layer.PlayerMain | 1 << (int) Layers.Layer.PlayerLimbs);
	}
	
	
	
	public void Deactivate()
	{
		ghostBodyGO.SetActive( false );
		HidePath();
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
			NavMeshPath attemptPath = new NavMeshPath();
			if (NavMesh.CalculatePath( playerXform.position, hitInfo.point, -1, attemptPath ) && Vector3.Distance(attemptPath.corners[ attemptPath.corners.Length-1], hitInfo.point) < 0.3f )
			{
				path = attemptPath;
				transform.position = hitInfo.point;
			}
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
	
	
	
	void HidePath()
	{
		foreach ( Transform stepXform in stepPoints )
			if ( stepXform.gameObject.activeSelf )
				stepXform.gameObject.SetActive( false );

		lineRenderer.enabled = false;
		path = null;
	}



	void LateUpdate()
	{
		if ( state == State.Active )
		{
					RaycastHit hitInfo;
			//if ( Physics.CapsuleCast(	capsuleSphere1Xform.position + kUpOffset,
			//									capsuleSphere2Xform.position + kUpOffset,
			//									capsuleSphere1Xform.localScale.x,
			//									playerHead.lookDirection,
			//									out hitInfo,
			//									Mathf.Infinity,
			//									nonPlayerLayerMask ) )
			if ( Physics.Raycast( playerHead.eyeCenter, playerHead.lookDirection, out hitInfo, Mathf.Infinity, nonPlayerLayerMask ) )
			{
				DropAt( capsuleSphere1Xform.position + kUpOffset + playerHead.lookDirection * hitInfo.distance );

				if ( path != null )
					ShowPath();
			}
			
			if ( path == null && lineRenderer.enabled )
				HidePath();

			Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

			transform.Rotate( Vector3.up, Time.deltaTime * 200 * secondaryAxis.x );
			transform.Rotate( Vector3.up, Time.deltaTime * 1000 * Input.GetAxis("Mouse X") );
		}
	}
	


	void OnTimeWarpChanged ( float timeWarp01, float absoluteTimeWarp )
	{
		// TODO change transparency?
	}
	
	

	void PlaceStepPointOnSegment( Vector3 pointFrom, Vector3 pointTo, float distance )
	{
		Vector3 direction = (pointTo - pointFrom).normalized;
		Vector3 point = pointFrom + distance * direction;
		Quaternion rot = Quaternion.LookRotation( direction );

		PlaceStepPointAtPoint( point, rot );
	}



	void PlaceStepPointAtPoint( Vector3 point, Quaternion rotation )
	{
		numStepPoints++;
		Transform stepPointXform;
		if ( numStepPoints > stepPoints.Count )
		{
			stepPointXform = (Instantiate(stepPointPrefab, point, rotation) as GameObject).transform;
			stepPoints.Add( stepPointXform );
		}
		else
		{
			stepPointXform = stepPoints[ numStepPoints-1 ];
			stepPointXform.position = point;
			stepPointXform.rotation = rotation;
		}

		stepPointXform.name = "StepPoint " + numStepPoints;
		if ( false == stepPointXform.gameObject.activeSelf )
			stepPointXform.gameObject.SetActive( true );
	}



	void ShowPath()
	{
		if ( path == null )
			return;

		if ( lineRenderer.enabled == false )
			lineRenderer.enabled = true;

		lineRenderer.SetVertexCount( path.corners.Length );
		for ( int i=0;  i<path.corners.Length;  i++ )
			lineRenderer.SetPosition( i, path.corners[i] + kPathDrawVerticalOffset );

		float totalLength = 0;
				for ( int i=1;  i<path.corners.Length;  i++ )
				{
					float segmentLength = Vector3.Distance( path.corners[i], path.corners[i-1]);
					totalLength += segmentLength;
				}

		numStepPoints = 0;

		int numSteps = Mathf.CeilToInt( totalLength/stepSize );
		float actualStepSize = totalLength / numSteps;

		int indexOfSegmentEnd = 1;
		float lengthOfPreviousSegments = 0;
		for ( int i=1;  i<=numSteps;  ++i )
		{
			float targetPathLengthForCurrentStepPoint = i * actualStepSize;
			for ( ;  indexOfSegmentEnd<path.corners.Length;  indexOfSegmentEnd++ )
			{
				float segmentLength = Vector3.Distance( path.corners[indexOfSegmentEnd], path.corners[indexOfSegmentEnd-1]);
				if ( lengthOfPreviousSegments + segmentLength >= targetPathLengthForCurrentStepPoint )
					break;
				lengthOfPreviousSegments += segmentLength;
			}
			if ( indexOfSegmentEnd < path.corners.Length )
				PlaceStepPointOnSegment( path.corners[indexOfSegmentEnd-1], path.corners[indexOfSegmentEnd], targetPathLengthForCurrentStepPoint - lengthOfPreviousSegments);
		}

		for ( int i=numStepPoints;  i<stepPoints.Count;  i++ )
		{
			GameObject stepGO = stepPoints[i].gameObject;
			if ( stepGO.activeSelf )
				stepPoints[i].gameObject.SetActive( false );
		}
	}
	
	
	
	void Start()
	{
		Deactivate();
	}


	

}
