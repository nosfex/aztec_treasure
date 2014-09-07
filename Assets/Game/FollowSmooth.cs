using UnityEngine;
//using UnityEditor;
using System.Collections;

public class FollowSmooth : MonoBehaviour 
{
	
	public Transform target;
	public float frictionCoef = 0.99f;
	public Vector3 offset;
	
	Vector3 originalPosition;
	
	
	public float upLock = 0;
	public float  downLock = 0;
	public  float leftLock = 0;
	public float rightLock = 0;
	
	Collider whoUp;
	Collider whoDown;
	Collider whoLeft;
	Collider whoRight;
	
	public float upLimitZ = 0;
	public float downLimitZ = 0;
	public float leftLimitX = 0;
	public float rightLimitX = 0;
	
	
	public enum CameraState 
	{
		ON_ROOM,
		ON_CORRIDOR,
		ON_PAJA
	};
	
	CameraState state;
	
	GameObject[] debugSelect = new GameObject[4];
	
	//
	// LEFT
	//
	
	Player player;

	bool findNearestCollider( Collider who, Collider other )
	{
		float dist1 = Vector3.Distance( other.transform.position, target.position );
		float dist2 = Vector3.Distance( who.transform.position, target.position );
		return dist1 < dist2;	
	}
	
	const float LOCK_TIME = 0.01f;
	
	
	public void OnTriggerEnterLeft( Collider lockCollider, Collider other ) 
	{ 
		if ( leftLock > 0 && findNearestCollider( other, whoLeft ) )
		{
			if ( other.transform.position.z == whoLeft.transform.position.z )
			{
				leftLock = LOCK_TIME;
				return;		
			}
		}
//		
//		if ( posDelta.x < 0 )
		{
			leftLock = LOCK_TIME; 
			whoLeft = other;
			
			Vector3 right = other.ClosestPointOnBounds( target.position + (Vector3.right * 100) );
			float dif = (lockCollider.transform.position.x - lockCollider.bounds.extents.x) - right.x;
			leftLimitX = target.position.x - dif;
			debugSelect[0] = other.gameObject;
		}
	}
	
	//
	// RIGHT
	//
	public void OnTriggerEnterRight( Collider lockCollider,Collider other ) 
	{
		if ( rightLock > 0 && findNearestCollider( other, whoRight ) )
		{
			if ( other.transform.position.z == whoRight.transform.position.z )
			{
				rightLock = LOCK_TIME;
				return;		
			}
		}
		
//		if ( rightLock > 0 )
//			return;
//		
//		if ( posDelta.x > 0 )
		{
			rightLock = LOCK_TIME; 
			whoRight = other;
	
			Vector3 left = other.ClosestPointOnBounds( target.position + (Vector3.left * 100) );
			float dif = (lockCollider.transform.position.x + lockCollider.bounds.extents.x) - left.x;
			rightLimitX = target.position.x - dif;
			debugSelect[1] = other.gameObject;
			
		}
	}
	
	
	//
	// UP
	//
	public void OnTriggerEnterUp( Collider lockCollider,Collider other ) 
	{ 
		if ( upLock > 0 && findNearestCollider( other, whoUp ) )
		{
			if ( other.transform.position.x == whoUp.transform.position.x )
			{
				upLock = LOCK_TIME;
				return;		
			}
		}
		
//		if ( upLock > 0 )
//			return;
//		
		upLock = LOCK_TIME; 
		whoUp = other;
		//if ( posDelta.z > 0 )
		{
			//print("enterUp");
			//saved.z = transform.position.z;
			
			Vector3 down = other.ClosestPointOnBounds( target.position + (Vector3.back * 100) );
			float dif =   (lockCollider.transform.position.z + lockCollider.bounds.extents.z) - down.z;
			upLimitZ =  target.position.z - dif;// - offset.z;// - dif;//(transform.position.x - (lockCollider.transform.position.x + lockCollider.bounds.extents.x)) + lockCollider.transform.localPosition.x;
			debugSelect[2] = other.gameObject;

		}
	//	print("enterUp dif " + dif + " - posZ " + transform.position.z + " ... saved = " + saved.z );
		
	}
	
	//
	// DOWN
	//
	public void OnTriggerEnterDown( Collider lockCollider,Collider other ) 
	{ 
		if ( downLock > 0 && findNearestCollider( other, whoDown ) )
		{
			if ( other.transform.position.x == whoDown.transform.position.x )
			{
				downLock = LOCK_TIME;
				return;		
			}
		}
		
//		if ( downLock > 0 )
//			return;
//		
//		if ( posDelta.z < 0 )
		{
			downLock = LOCK_TIME; 
			whoDown = other;
			
			Vector3 up = other.ClosestPointOnBounds( target.position + (Vector3.forward * 100) );
			float dif =   (lockCollider.transform.position.z - lockCollider.bounds.extents.z) - up.z;
			downLimitZ =  target.position.z - dif;// - offset.z;// - dif;//(transform.position.x - (lockCollider.transform.position.x + lockCollider.bounds.extents.x)) + lockCollider.transform.localPosition.x;
			debugSelect[3] = other.gameObject;

		}
	}
	
	public void OnTriggerExitUp(Collider lockCollider, Collider other )  
	{ 
//		if ( upLock > 0 && whoUp != other )
//			return;
//		//print("exitUp");
//		upLock = 0; 
	}
	
	
	public void OnTriggerExitDown( Collider lockCollider,Collider other )  
	{ 
//		if ( downLock > 0 && whoDown != other )
//			return;
//		
//		//print("exitDown");
//		downLock = 0; 
	}
	
	public void OnTriggerExitRight( Collider lockCollider,Collider other )  
	{
//		if ( rightLock > 0 && whoRight != other )
//			return;
//
//		rightLock = 0;
	}
	
	public void OnTriggerExitLeft( Collider lockCollider, Collider other )  
	{ 
//		if ( leftLock > 0 && whoLeft != other )
//			return;
//
//		leftLock = 0; 
	}
	
	
	//
	//
	//
	
	void Start()
	{
		originalPosition = transform.position;
		offset = transform.position - target.position;
		//saved = transform.position;
		player = target.GetComponent<Player>();
	}

	Vector3 posDelta;
	Vector3 lastPos;
	
	Vector3 saved = Vector3.zero;
	
	
	void UpdateRoom()
	{
		posDelta = target.position - lastPos;
		lastPos = target.position;
		
		Vector3 tempTarget = target.position;
		
		if ( ( rightLock <= 0 ) ) // Break lock if going up.
		{
			rightLimitX = tempTarget.x + 10;
			debugSelect[1] = null;
		}
		else if ( tempTarget.x > rightLimitX  )
			tempTarget.x = rightLimitX;

		if ( ( leftLock <= 0 ) ) // Break lock if going up.
		{
			leftLimitX = tempTarget.x - 10;
			debugSelect[0] = null;
		}
		else if ( tempTarget.x < leftLimitX )
			tempTarget.x = leftLimitX;
	
		if ( ( upLock <= 0 ) ) // Break lock if going up.
		{
			upLimitZ = tempTarget.z + 10;
			debugSelect[2] = null;
		}
		else if ( tempTarget.z > upLimitZ )
			tempTarget.z = upLimitZ;

		if ( ( downLock <= 0 ) ) // Break lock if going up.
		{
			downLimitZ = tempTarget.z - 10;
			debugSelect[3] = null;
		}
		else if ( tempTarget.z < downLimitZ )
			tempTarget.z = downLimitZ;

		
		if ( downLock > 0 && upLock > 0 )
			tempTarget.z = (downLimitZ + upLimitZ) * 0.5f;

		if ( leftLock > 0 && rightLock > 0 )
			tempTarget.x = (leftLimitX + rightLimitX) * 0.5f;

		//	UnityEditor.Selection.objects = (Object[])debugSelect;
		 
		Vector3 v = ((tempTarget + offset) - transform.position) * frictionCoef;
		
		if ( Mathf.Sign(posDelta.x) != Mathf.Sign (v.x) || posDelta.x == 0 ) 
			v.x = 0;
		
		if ( Mathf.Sign(posDelta.z) != Mathf.Sign (v.z) || posDelta.z == 0  ) 
			v.z = 0;
		

		if ( downLock > 0 && upLock > 0 )
			v.z = 0;
//			tempTarget.z = (downLimitZ + upLimitZ) * 0.5f;

		if ( leftLock > 0 && rightLock > 0 )
			v.x = 0;		
		
		transform.position += v;
			
		transform.position = new Vector3( transform.position.x, originalPosition.y, transform.position.z );		
	}
	
	void UpdateCorridor()
	{
		posDelta = target.position - lastPos;
		lastPos = target.position;
		
		Vector3 tempTarget = target.position;
		
		
		Vector3 v = ((tempTarget + offset) - transform.position) * frictionCoef;
		
		if ( Mathf.Sign(posDelta.x) != Mathf.Sign (v.x) || posDelta.x == 0 ) 
			v.x = 0;
		
		if ( Mathf.Sign(posDelta.z) != Mathf.Sign (v.z) || posDelta.z == 0  ) 
			v.z = 0;

		if ( downLock > 0 && upLock > 0 )
			v.z = 0;
//			tempTarget.z = (downLimitZ + upLimitZ) * 0.5f;

		if ( leftLock > 0 && rightLock > 0 )
			v.x = 0;
			//tempTarget.x = (leftLimitX + rightLimitX) * 0.5f;
		
		/*
		if ( rightLock > 0 && posDelta.x > 0 )
			v.x = 0;

		if ( leftLock > 0 && posDelta.x < 0 )
			v.x = 0;

		if ( upLock > 0 && posDelta.z > 0 )
			v.z = 0;

		if ( downLock > 0 && posDelta.z < 0 )
			v.z = 0;*/
			
		
		transform.position += v;
			
		transform.position = new Vector3( transform.position.x, originalPosition.y, transform.position.z );		
		
	}

	Vector3 shake;
	public bool earthquakeEnabled = false;
	
	public void Shake( float amount, float time )
	{
		iTween.ShakePosition( gameObject, iTween.Hash( "amount", new Vector3(1.0f, 1.0f, 0) * amount, "time", time ) );
	}
	
	
	void UpdatePaja()
	{
		//posDelta = target.position - lastPos;
		//lastPos = target.position;
		
		Vector3 tempTarget = target.position;
		
		shake = Vector3.zero;
		if ( earthquakeEnabled )
		{
			Vector3 shakeTo = Random.insideUnitSphere * .1f;
			shakeTo.z = 0;
			shakeTo.x *= 0.01f;
			shake = shakeTo;
		}
		 
		Vector3 adjust = Vector3.zero;
		float adjustLeft = 0;
		float adjustUp = 0;
		float adjustDown = 0;
		float adjustRight = 0;

		float pointLeft = 0;
		float pointRight = 0;
		float pointUp = 0;
		float pointDown = 0;

		//Left
		RaycastHit[] rhLeft = Physics.RaycastAll( target.position, Vector3.left, 5.0f );
		
		foreach ( RaycastHit r in rhLeft )
			if ( r.collider.gameObject.name == "CameraBlockLeft" )
		{
			adjustLeft = 5.0f - (tempTarget.x - r.point.x);// - (r.collider.transform.position.x + r.collider.bounds.extents.x);
			//adjustLeft = Mathf.Max ( MAX_ADJUST_SPEED, adjustLeft );
			pointLeft = r.point.x;
		}

		//Right
		RaycastHit[] rhRight = Physics.RaycastAll( target.position, Vector3.right, 5.0f );
		
		foreach ( RaycastHit r in rhRight )
			if ( r.collider.gameObject.name == "CameraBlockRight" )
		{
			adjustRight -= 5.0f + (tempTarget.x - r.point.x);// - (r.collider.transform.position.x + r.collider.bounds.extents.x);
			//adjustRight = Mathf.Min ( -MAX_ADJUST_SPEED, adjustRight );
			pointRight = r.point.x;
		}

		//Up
		RaycastHit[] rhUp = Physics.RaycastAll( target.position, Vector3.forward, 5.0f );
		
		foreach ( RaycastHit r in rhUp )
			if ( r.collider.gameObject.name == "CameraBlockUp" )
		{
				adjustUp -= 5.0f + (tempTarget.z - r.point.z);// - (r.collider.transform.position.x + r.collider.bounds.extents.x);
			//adjustUp = Mathf.Min ( -MAX_ADJUST_SPEED, adjustUp );
			pointUp = r.point.z;
		}
		//Down
		RaycastHit[] rhDown = Physics.RaycastAll( target.position, Vector3.forward * -1, 5.0f );
		
		foreach ( RaycastHit r in rhDown )
			if ( r.collider.gameObject.name == "CameraBlockDown" )
		{
				adjustDown = 5.0f - (tempTarget.z - r.point.z);// - (r.collider.transform.position.x + r.collider.bounds.extents.x);
			//adjustDown = Mathf.Max ( MAX_ADJUST_SPEED, adjustDown );
			pointDown = r.point.z;
		}



		if ( adjustLeft != 0 && adjustRight != 0 )
			tempTarget.x = (pointLeft + pointRight) * 0.5f;
		else 
			adjust.x = (adjustLeft + adjustRight);

		if ( adjustUp != 0 && adjustDown != 0 )
			tempTarget.z = (pointUp + pointDown) * 0.5f;
		else 
			adjust.z = (adjustUp + adjustDown);

		//print ("Adjust L = " + adjustLeft + "... adjust R = " + adjustRight );

		tempTarget += adjust;

		Vector3 v = ((tempTarget + offset) - transform.position) * frictionCoef;

		float MAX_ADJUST_SPEED = 0.05f;

		if ( v.z > MAX_ADJUST_SPEED )
			v.z = MAX_ADJUST_SPEED;

		if ( v.z < -MAX_ADJUST_SPEED )
			v.z = -MAX_ADJUST_SPEED;

		if ( v.x > MAX_ADJUST_SPEED )
			v.x = MAX_ADJUST_SPEED;

		if ( v.x < -MAX_ADJUST_SPEED )
			v.x = -MAX_ADJUST_SPEED;
		//		if ( adjustUp != 0 && adjustDown != 0 )
//			v.z = 0;

		if ( GameDirector.i.playerRight.currentFloor == null )
			v.y = 0;
		else
			v.y *= 0.1f;



		transform.position += v;


		//transform.position = new Vector3( transform.position.x, originalPosition.y, transform.position.z );
		transform.position += shake;
		//transform.position += offset;
		
	}
	
	void Update() 
	{
//		if ( player.currentFloor )
//		{
//			if ( player.currentFloor.tag.Contains ( "Corridor" ) )
//				state = CameraState.ON_CORRIDOR;
//			else 
//				state = CameraState.ON_ROOM;
//		}
//		
		state = CameraState.ON_PAJA;

		switch ( state )
		{
			case CameraState.ON_ROOM:
				UpdateRoom();
				break;
			case CameraState.ON_CORRIDOR:
				UpdateCorridor();
				break;
			case CameraState.ON_PAJA:
				UpdatePaja();
				break;
		}
		/*
		rightLock -= Time.deltaTime;
		leftLock -= Time.deltaTime;
		upLock -= Time.deltaTime;
		downLock -= Time.deltaTime;
		 */
	}
}
