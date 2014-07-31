using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour 
{
	public World worldOwner;
	//public Object myPrefab;
	[HideInInspector] public int hearts;
	[HideInInspector] public Vector3 accel = Vector3.zero;
	[HideInInspector] public float frictionCoef = 0.97f;
	
	public float airFrictionCoef = 1.0f;
	public float groundFrictionCoef = 0.66f;
	public float bouncyness = 0.9f;
	
	[HideInInspector] public Vector3 velocity = Vector3.zero;
	
	//[HideInInspector] 
	public bool gravityEnabled = true;
	
	public bool isLiftable = true;
	public bool isSwitch = false;
	
	public bool dontCollideWhenImmune = false;
	
	[HideInInspector] public Vector3 gravity = Vector3.zero;
	
	public bool collisionEnabled = true;
	public bool respawns = false;
	public BoxCollider currentFloor;
	
	public GameObject prefabShadow;
	
	protected float minStairClimb = 0.23f;
	
	EnemySpawner respawner;
	
	float stuckDownTimer = 0;
	float stuckUpTimer = 0;
	float stuckRightTimer = 0;
	float stuckLeftTimer = 0;
	
	public bool stuckBack
	{
		get { return stuckDownTimer > 0; }
	}
	
	public bool stuckForward
	{
		get { return stuckUpTimer > 0; }
	}
	
	public bool stuckRight
	{
		get { return stuckRightTimer > 0; }
	}
	
	public bool stuckLeft
	{
		get { return stuckLeftTimer > 0; }
	}
	
	public bool stuck
	{
		get { return stuckBack || stuckForward || stuckRight || stuckLeft; }
	}
	
	public bool isGrounded
	{
		get { return currentFloor != null; }
	}

	virtual protected void Start()
	{
		World myWorld = findWorld( transform );
			
		worldOwner = myWorld;
		
		if ( respawns )
			InitRespawn();
		
		if ( prefabShadow != null )
		{
			GameObject go = (GameObject)Instantiate ( prefabShadow, transform.position, Quaternion.identity );
			go.transform.parent = transform;
		}
	}
	
	public void InitRespawn()
	{
		if ( respawner != null )
			return;
		
		GameObject myPrefab = GameDirector.i.findMyPrefab( gameObject );
		
		if ( myPrefab == null )
		{
			Debug.LogWarning("Prefab is missing! Can't respawn!", this );
			respawns = false;
			return;
		}
		
		GameObject go = 
			(GameObject)Instantiate 
			( 
				GameDirector.i.enemySpawnerPrefab, 
				transform.position,
				transform.rotation 
			);
		
		go.transform.parent = transform.parent;
		respawner = go.GetComponent<EnemySpawner>();
		respawner.objectToRespawn = myPrefab; 
	//	print ("Spawn spawner..." + gameObject );
	}
	
	
	World findWorld( Transform t ) 
	{
		World w = t.GetComponent<World>();
		if ( w )
			return w;
		else 
		{
			if ( t.root == t )
				return null;
			else 
				return findWorld ( t.parent );
		}
	}	
	
	float floorY = 0;
	protected bool sleepPhysics = false;
	Vector3 lastFloorPos;

	protected virtual void LateUpdate()
	{
		if ( sleepPhysics )
			return;

		float frameRatio = (Time.deltaTime / 0.016f);
		//Debug.Log ("frameRatio: " + frameRatio );
		if ( gravityEnabled )
		{
			if ( currentFloor != null )
			{
				// Si el nuevo esta mas abajo o igual, lo asigna.
				float TECHODELPISO = currentFloor.transform.position.y + currentFloor.bounds.extents.y;
				
				if ( TECHODELPISO < floorY + minStairClimb || floorY == -100f )
					floorY = currentFloor.transform.position.y + currentFloor.bounds.extents.y + collider.bounds.extents.y;
			//	else 
			//		print("wtf");
			}
			else
			{
				floorY = -100f;
			}			
		}
		else 
		{
			currentFloor = null;
			floorY = -100f;
			gravity = Vector3.zero;
		}
		
		velocity += accel;
		float friction = frictionCoef;
		
		bool isGrounded = transform.position.y - 0.003f <= floorY;

		if ( gravityEnabled && !isGrounded )
		{
			friction = airFrictionCoef;
		}
		
		Vector3 velocityDif = (velocity * friction) - velocity;
		velocity += velocityDif;

		if ( gravityEnabled )
		{
			if ( !isGrounded )
			{
				gravity += ( Vector3.up * 0.003f );
			}
			else 
			{
				
				//if ( transform.position.y < floorY && (transform.position.y + STAIR_HEIGHT) > floorY )
				if ( isGrounded && (transform.position.y + minStairClimb) > floorY )
				{
					float dif = Mathf.Abs( floorY - transform.position.y ) * 1.0f;
					transform.position += Vector3.up * Mathf.Min( 0.05f, dif );
					
					if ( transform.position.y > floorY )
					{
						gravity = Vector3.zero;
						transform.position = new Vector3( transform.position.x, floorY, transform.position.z );
					}
				}
				else 
				if ( isGrounded )
				{
					transform.position = new Vector3( transform.position.x, floorY, transform.position.z );
				}
				
				if ( Mathf.Abs( gravity.y ) < 0.01f )
					gravity = Vector3.zero;
				else 
				{
					if ( gravity.y > 0 )
						gravity *= -bouncyness;//Vector3.zero;
				}
				velocity *= groundFrictionCoef;
			}
			
			transform.position -= gravity * frameRatio;
		}

		if ( currentFloor != null && gravityEnabled )
		{
			Vector3 currentFloorPos = new Vector3( currentFloor.transform.position.x, 0, currentFloor.transform.position.z );
			Vector3 floorDelta = currentFloorPos - lastFloorPos;
			
			transform.position += floorDelta;
			
			lastFloorPos = currentFloorPos;
		}

		transform.position += velocity * frameRatio;
		accel = Vector3.zero;
		
		stuckLeftTimer--; stuckRightTimer--; stuckDownTimer--; stuckUpTimer--;
		
	}
	
	
	virtual protected void OnTriggerExit( Collider other )
	{
		if ( other == currentFloor )
		{
			currentFloor = null;
			other.SendMessage( "ExitObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 
		}
	}
	
	virtual protected void TestFloor( Collider other )
	{
		float TECHODELPISO = other.transform.position.y + other.bounds.extents.y + other.bounds.center.y;
		float MISPIES = transform.position.y - collider.bounds.extents.y + collider.bounds.center.y;
		float yDif = TECHODELPISO - MISPIES;
		
		if ( yDif >= minStairClimb ) // Enough to climb
			return;

		currentFloor = (BoxCollider)other;
		lastFloorPos = new Vector3( currentFloor.transform.position.x, 0, currentFloor.transform.position.z );

		other.SendMessage( "EnterObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 	
	}
	
	virtual protected void TestWalls( Collider other )
	{
		Bounds b = ((BoxCollider)collider).bounds;
		
		float margin = ((BoxCollider)collider).bounds.extents.x;// - 0.01f;
		
		Vector3 left = other.ClosestPointOnBounds( transform.position + (Vector3.left * 100) ) + (Vector3.left * margin);
		Vector3 right = other.ClosestPointOnBounds( transform.position + (Vector3.right * 100) ) + (Vector3.right * margin);
		Vector3 up = other.ClosestPointOnBounds( transform.position + (Vector3.forward * 100) ) + (Vector3.forward * margin);
		Vector3 down = other.ClosestPointOnBounds( transform.position + (Vector3.back * 100) ) + (Vector3.back * margin);
		
		Vector3[] vv = { left, right, up, down };
		
		float minDistance = 9999999999f;
		Vector3 closestBoundExit = transform.position;

		int lockIndex = -1;
		
		for ( int i = 0 ;  i < vv.Length; i++ )
		{
			Vector3 v = vv[i];
			float distance = Vector3.Distance( v, transform.position );
			
			if ( distance < minDistance ) 
			{
				minDistance = distance;
				closestBoundExit = v;
				lockIndex = i;
			}
		}
		
		bool adjustX = false;
		bool adjustZ = false;
		
		switch( lockIndex )
		{
		case 0:
			adjustX = true;
			stuckRightTimer = 5;
			break;
		case 1:
			adjustX = true;
			stuckLeftTimer = 5;
			break;
		case 2:
			adjustZ = true;
			stuckDownTimer = 5;
			break;
		case 3:
			adjustZ = true;
			stuckUpTimer = 5;
			break;
		}
		
		//velocity *= -bouncyness;

		transform.position = new Vector3( adjustX ? closestBoundExit.x : transform.position.x, 
										  transform.position.y, 
										  adjustZ ? closestBoundExit.z : transform.position.z );		
	}
	
	virtual protected void OnTriggerStay( Collider other )
	{
		OnTriggerEnter( other );
	}

	virtual protected void OnTriggerEnter( Collider other )
	{
		if ( other.tag.Contains( "Floor" ) )
			TestFloor( other );

		BaseObject bo = other.GetComponent<BaseObject>();

		if ( bo != null && !bo.collisionEnabled )
			return;
		
		//if ( !collisionEnabled )
		//	return;
		
		if ( other.tag.Contains("Floor") )
		{
			float TECHODELPISO = other.transform.position.y + other.bounds.extents.y;
			float MISPIES = transform.position.y - collider.bounds.extents.y;
			float yDif = TECHODELPISO - MISPIES;
			
			if ( yDif >= minStairClimb ) // Enough to climb
			{
				TestWalls( other ); // Treat as wall!
			}
		}		
		
		if ( other.tag.Contains( "Wall" ) )
			TestWalls( other );
	}
	
	
	virtual public void OnPlayerDead()
	{
		if ( respawns && respawner )
		{
			//print ( "spawn..");
			Destroy( gameObject );
			respawner.Respawn();
		}
	}
}
