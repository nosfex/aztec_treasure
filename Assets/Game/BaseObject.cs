using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour 
{
	
	public World worldOwner;
	
	[HideInInspector] public Vector3 accel = Vector3.zero;
	public float frictionCoef = 0.97f;
	
	[HideInInspector] public Vector3 velocity = Vector3.zero;
	
	//[HideInInspector] 
	public bool gravityEnabled = true;
	
	public bool isLiftable = true;
	
	Vector3 gravity = Vector3.zero;
	
	public bool collisionEnabled = true;
	public BoxCollider currentFloor;

	virtual protected void Start()
	{
		velocity = Vector3.zero;
		accel = Vector3.zero;
		
		if ( transform.root != transform )
		{
			
			//So, now try to detect if child or grand-child of world.
			World myWorld = transform.parent.GetComponent<World>();
			
			if ( myWorld == null )
				myWorld = transform.parent.parent.GetComponent<World>();
			
			if ( myWorld != null )
			{
				
				worldOwner = myWorld;					
			}
		}
	}
	
	float floorY = 0;
	
	protected void LateUpdate()
	{
		
		float frameRatio = Mathf.Clamp01(Time.deltaTime / 0.016f);
		
		if ( gravityEnabled )
		{
			transform.position -= gravity * frameRatio;
			
			if ( currentFloor != null )
			{
				// Si el nuevo esta mas abajo o igual, lo asigna.
				if ( ( currentFloor.transform.position.y + currentFloor.bounds.extents.y ) < floorY + 0.4f || floorY == -100f )
					floorY = currentFloor.transform.position.y + currentFloor.bounds.extents.y + collider.bounds.extents.y;
			}
			else
			{
				floorY = -100f;
			}
			
			//print ("floorY " + floorY );
			if ( transform.position.y > floorY )
			{
				
				gravity += ( Vector3.up * 0.01f );
				gravity *= 0.9f;
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
				
				gravity *= -0.5f;//Vector3.zero;
				
				if ( gravity.magnitude < 0.1f )
					gravity = Vector3.zero;
				
				collisionEnabled = true;				
			}
			
		}
		
		

		velocity += accel * frameRatio;
		Vector3 velocityDif = (velocity * frictionCoef) - velocity;
		velocity += velocityDif * frameRatio;
		//print ("coef = " + velocityDif );
		//velocity *= frictionCoef;
		transform.position += velocity * frameRatio;
		
//		if ( velocity.x != 0 )
		//	print (" v = "  + velocity.x  + " * " + frameRatio );
		
		accel = Vector3.zero;
	}
	
	
	virtual protected void OnTriggerExit( Collider other )
	{
		if ( other.tag == "Floor" )
		{
			if ( other == currentFloor )
			{
				currentFloor = null;
				other.SendMessage( "ExitObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 
			}
		}
	}
	
	virtual protected void OnTriggerStay( Collider other )
	{
		if ( other.tag == "Floor" )
		{
			float TECHODELPISO = other.transform.position.y + other.bounds.extents.y;
			float MISPIES = transform.position.y - collider.bounds.extents.y;
			float yDif = TECHODELPISO - MISPIES;
			
			if ( yDif >= 0.3f ) // Enough to climb
				return;

			currentFloor = (BoxCollider)other;
			other.SendMessage( "EnterObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 
		}		
	}
	
	
	virtual protected void OnTriggerEnter( Collider other )
	{
		if ( other.tag == "Floor" )
		{
			float TECHODELPISO = other.transform.position.y + other.bounds.extents.y;
			float MISPIES = transform.position.y - collider.bounds.extents.y;
			float yDif = TECHODELPISO - MISPIES;
			
			if ( yDif >= 0.3f ) // Enough to climb
				return;
			
			currentFloor = (BoxCollider)other;
			gameObject.BroadcastMessage( "EnterObjectLaid", this, SendMessageOptions.DontRequireReceiver ); 
		}
	}
}
