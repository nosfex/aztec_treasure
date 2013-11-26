using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour 
{
	public World worldOwner;
	//public Object myPrefab;

	[HideInInspector] public Vector3 accel = Vector3.zero;
	[HideInInspector] public float frictionCoef = 0.97f;
	
	public float airFrictionCoef = 1.0f;
	public float groundFrictionCoef = 0.66f;
	public float bouncyness = 0.9f;
	
	[HideInInspector] public Vector3 velocity = Vector3.zero;
	
	//[HideInInspector] 
	public bool gravityEnabled = true;
	
	public bool isLiftable = true;
	
	[HideInInspector] public Vector3 gravity = Vector3.zero;
	
	public bool collisionEnabled = true;
	public bool respawns = false;
	public BoxCollider currentFloor;
	
	EnemySpawner respawner;

	virtual protected void Start()
	{
//#if UNITY_EDITOR
//		if ( myPrefab == null )
//		{
//			Object prefab = UnityEditor.EditorUtility.GetPrefabParent(gameObject);
//	
//			if ( prefab != null )
//			{
//				myPrefab = (GameObject)prefab;
//				myPrefab.GetComponent<BaseObject>().myPrefab = myPrefab;
//			}
//		}
//#endif
		
		velocity = Vector3.zero;
		accel = Vector3.zero;
		
		World myWorld = findWorld( transform );
			
		worldOwner = myWorld;
		
		if ( respawns )
			InitRespawn();
		
		
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
		respawner.objectToRespawn = myPrefab; //(GameObject)myPrefab;
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
	
	protected void LateUpdate()
	{
		
		float frameRatio = Mathf.Clamp01(Time.deltaTime / 0.016f);
		
		if ( gravityEnabled )
		{
			if ( currentFloor != null )
			{
				// Si el nuevo esta mas abajo o igual, lo asigna.
				if ( ( currentFloor.transform.position.y + currentFloor.bounds.extents.y ) < floorY + 0.4f || floorY == -100f )
					floorY = currentFloor.transform.position.y + currentFloor.bounds.extents.y + collider.bounds.extents.y;
				else 
					print("wtf");
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
		
		
		
		velocity += accel * frameRatio;
		float friction = frictionCoef;
		
		if ( gravityEnabled && transform.position.y > floorY )
			friction = airFrictionCoef;
		
		Vector3 velocityDif = (velocity * friction) - velocity;
		velocity += velocityDif * frameRatio;
		//print ("coef = " + velocityDif );
		//velocity *= frictionCoef;
		transform.position += velocity * frameRatio;
		
//		if ( velocity.x != 0 )
		//	print (" v = "  + velocity.x  + " * " + frameRatio );
		if ( gravityEnabled )
		{
			transform.position -= gravity * frameRatio;
			//print ("floorY " + floorY );
			if ( transform.position.y > floorY )
			{
				
				gravity += ( Vector3.up * 0.003f );
				//gravity *= 0.99f;
			}
			else 
			{
				if ( floorY > transform.position.y && floorY < (transform.position.y + 0.4f) )
				{
					//if ( gravity.y >= 0 )
					{
						float dif = Mathf.Abs( floorY - transform.position.y ) * 1.0f;
						//gravity -= Vector3.up * dif;
						transform.position += Vector3.up * Mathf.Min( 0.1f, dif );
					}
					//iTween.MoveTo( gameObject, iTween.Hash( "y", floorY, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack ) );
				}
				else 
				{
					transform.position = new Vector3( transform.position.x, floorY, transform.position.z );
				}
				//transform.position = new Vector3( transform.position.x, floorY, transform.position.z );

				
				if ( gravity.magnitude < 0.05f )
					gravity = Vector3.zero;
				else 
				{
					
					if ( gravity.y > 0 )
					{
						//print("WHAT");
						gravity *= -bouncyness;//Vector3.zero;
					}
				}
				
				//collisionEnabled = true;				
			}
			
		}
		
		accel = Vector3.zero;
	}
	
	
	virtual protected void OnTriggerExit( Collider other )
	{
		//if ( other.tag.Contains( "Floor" ) )
		{
			if ( other == currentFloor )
			{
				currentFloor = null;
				other.SendMessage( "ExitObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 
			}
		}
	}
	
	virtual protected void TestFloor( Collider other )
	{
		float TECHODELPISO = other.transform.position.y + other.bounds.extents.y;
		float MISPIES = transform.position.y - collider.bounds.extents.y;
		float yDif = TECHODELPISO - MISPIES;
		
		if ( yDif >= 0.3f ) // Enough to climb
			return;

		currentFloor = (BoxCollider)other;
		other.SendMessage( "EnterObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 	
	}
	
	virtual protected void TestWalls( Collider other )
	{
		BaseObject bo = other.GetComponent<BaseObject>();

		if ( bo != null && !bo.collisionEnabled )
			return;
		
		Bounds b = ((BoxCollider)collider).bounds;
		
		Vector3 left = other.ClosestPointOnBounds( transform.position + (Vector3.left * 100) ) + (Vector3.left * b.extents.x);
		Vector3 right = other.ClosestPointOnBounds( transform.position + (Vector3.right * 100) ) + (Vector3.right * b.extents.x);
		Vector3 up = other.ClosestPointOnBounds( transform.position + (Vector3.forward * 100) ) + (Vector3.forward * b.extents.z);
		Vector3 down = other.ClosestPointOnBounds( transform.position + (Vector3.back * 100) ) + (Vector3.back * b.extents.z);
		
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
			break;
		case 1:
			adjustX = true;
			break;
		case 2:
			adjustZ = true;
			break;
		case 3:
			adjustZ = true;
			break;
		}
		velocity *= -bouncyness;
		
		transform.position = new Vector3( adjustX ? closestBoundExit.x : transform.position.x, 
										  transform.position.y, 
										  adjustZ ? closestBoundExit.z : transform.position.z );		
	}
	
	virtual protected void OnTriggerStay( Collider other )
	{
		if ( other.tag.Contains( "Floor" ) )
			TestFloor( other );

		
		if ( other.tag.Contains("Floor") )
		{
			float TECHODELPISO = other.transform.position.y + other.bounds.extents.y;
			float MISPIES = transform.position.y - collider.bounds.extents.y;
			float yDif = TECHODELPISO - MISPIES;
			
			if ( yDif >= 0.3f ) // Enough to climb
			{
				TestWalls( other ); // Treat as wall!
			}
		}
		
		if ( other.tag.Contains( "Wall" ) )
			TestWalls( other );
	}

	virtual protected void OnTriggerEnter( Collider other )
	{
		if ( other.tag.Contains( "Floor" ) )
			TestFloor( other );
		
		if ( other.tag.Contains("Floor") )
		{
			float TECHODELPISO = other.transform.position.y + other.bounds.extents.y;
			float MISPIES = transform.position.y - collider.bounds.extents.y;
			float yDif = TECHODELPISO - MISPIES;
			
			if ( yDif >= 0.3f ) // Enough to climb
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
