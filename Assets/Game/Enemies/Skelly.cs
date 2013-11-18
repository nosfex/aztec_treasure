using UnityEngine;
using System.Collections;

public class Skelly : BaseObject 
{
	SpriteAnimator animator;
	
	public Transform helperPivot;
	public BaseObjectSensor liftSensor;
	//public GameObject dropGuide;
	
	//private Light[] lights;
	//private Light torchLight;
	//public bool inDarkness = false;
	
	[HideInInspector]
	public int hearts;
	
	public int maxHearts = 2;
	
	//public float torchRatio;
	BoxCollider lastSafeFloor;
	
	override protected void Start () 
	{
		animator = GetComponentInChildren<SpriteAnimator>();
		base.Start();
		direction = Vector3.right;
		hearts = maxHearts;
		//ChangeDirection();
	}	

	string facing = "Right";
	
	float cooldown = 0;
	
	float lockDown = 0;
	float lockUp = 0;
	float lockRight = 0;
	float lockLeft = 0;
	
	float straightTimer = 0;
	float inmuneTimer = 0;
	
	bool skidEnabled = false;
	bool isSkidding = false;
	float isFlipping = 0;
	
	BaseObject liftedObject;
	Vector3 direction;
	public float speed = 0.5f;
	
	bool goingLeft;
	bool goingRight;
	bool goingUp;
	bool goingDown;
	bool attacking;
	
	void ChangeDirection()
	{
		goingRight = goingLeft = goingUp = goingDown = attacking = false;
		int dir = Random.Range( 0, 4 );
		
		switch ( dir )
		{
			case 0:
				goingRight = true;
				break;
			case 1:
				goingLeft = true;
				break;
			case 2:
				goingUp = true;
				break;
			case 3:
				goingDown = true;
				break;
		}		
		
		//print ("change! " + dir );
	}
	
	void UpdateIA()
	{
		bool stuckRight = goingRight && lockRight > 0;
		bool stuckLeft = goingLeft && lockLeft > 0;
		bool stuckDown = goingDown && lockDown > 0;
		bool stuckUp = goingUp && lockUp > 0;
		
		bool stuck = stuckRight || stuckLeft || stuckUp || stuckDown;
		
		if ( straightTimer > 1.0f || stuck )
		{
			
			ChangeDirection();
		}
	}
	
	bool sleeping = true;
	
	void Update () 
	{
		if ( sleeping )
		{
			if ( Vector3.Distance( worldOwner.player.transform.position, transform.position ) < 4.0f )
			{
				ChangeDirection();
				sleeping = false;
			}
		}
		else 
		{
			UpdateIA ();
		}
		
		float dx = 0, dy = 0;
		
		if ( !animator.isAnimPlaying("Attack") && currentFloor != null  )
		{
			if ( goingLeft && lockLeft < 0 )
			{
				dx = -1;
			}
			
			if ( goingRight && lockRight < 0 )
			{
				dx = 1;
			}
			
			if ( goingUp && lockUp < 0)
			{
				dy = 1;
			}
			
			if ( goingDown && lockDown < 0 )
			{	
				dy = -1;
			}
			
			if ( goingLeft && lockLeft <0 )
				straightTimer = 0;
			
			if ( goingRight && lockRight  <0)
				straightTimer = 0;
			
			if ( goingUp && lockUp <0)
				straightTimer = 0;
			
			if ( goingDown && lockDown <0 )
				straightTimer = 0;
			
		}
		
		if ( dx != 0 || dy != 0 )
			straightTimer += Time.deltaTime;
			
		//print ("timer = " + straightTimer );
		
		
		if ( inmuneTimer > 0 )
		{
			animator.renderer.enabled = !animator.renderer.enabled;
			inmuneTimer -= Time.deltaTime;
			
			if ( inmuneTimer <= 0 )
				animator.renderer.enabled = true;
		}
		
		
		accel += speed * new Vector3( dx, 0, dy );
		
		frictionCoef += (0.66f - frictionCoef) * 0.75f;
		
		float threshold = 0.001f;
		
		//print ( "vel " + velocity.x );
		if ( !skidEnabled )
			skidEnabled = straightTimer > 0.3f;
		
		isFlipping -= Time.deltaTime;
		
		if ( isFlipping < 0 && !isSkidding )
		{
			if ( ( dx < 0 && velocity.x > 0 ) || ( dx > 0 && velocity.x < 0 ) )
				isFlipping = 0.05f;
	
			if ( ( dy < 0 && velocity.z > 0 ) || ( dy > 0 && velocity.z < 0 ) )
				isFlipping = 0.05f;
		}

		
		if ( skidEnabled )
		{
			if ( (( dx < 0 && velocity.x > 0 ) || ( dx > 0 && velocity.x < 0 )) 
				||( dy < 0 && velocity.z > 0 ) || ( dy > 0 && velocity.z < 0 )  )
			{
				//skidEnabled = false;
				isSkidding = true;
				frictionCoef = 0.9f;
				accel *= 0.05f;
			}
			else if ( isSkidding )
			{
				isSkidding = false;
				skidEnabled = false;
			}
		}
		
		if ( animator.isAnimPlaying("Attack") )
		{
			frictionCoef = 0.95f;
			cooldown = 0.1f;
		}
		else 
		{
			cooldown -= Time.deltaTime;
		}
		

		if ( goingLeft  )
		{
			facing = "Left";
			direction = Vector3.left;
		}
		else if ( goingRight  )
		{
			facing = "Right";
			direction = Vector3.right;
		}
		else if ( goingUp  )
		{
			facing = "Up";
			direction = Vector3.forward;
		}
		else if ( goingDown )
		{
			facing = "Down";
			direction = Vector3.back;
		}

		helperPivot.rotation = Quaternion.LookRotation( direction );
		
		if ( !animator.isAnimPlaying("Attack") )
		{
			if ( dx != 0 || dy != 0 )
			{
				animator.PlayAnim("Walk" + facing );
			}
			else 
			{
				animator.PlayAnim("Walk" + facing );
				animator.PauseAnim();
				animator.GoToFrame(2);
			}
		}
		
		
		if ( attacking  )
		{
			if ( liftedObject == null ) // Trata de levantar un objeto...
			{
				if ( liftSensor.sensedObject != null && liftSensor.sensedObject.isLiftable )
				{
					//print ("LIFT!");
					Transform lifted = liftSensor.sensedObject.transform;
					lifted.parent = transform;
					
					
					
					
					// pone el objeto en la cabeza del flaco.
					
					iTween.MoveTo( lifted.gameObject, iTween.Hash ( "isLocal", true, "position", new Vector3(0,0.45f,0), "time", 0.2f, "easetype", iTween.EaseType.easeOutCirc ) );
					//iTween.MoveAdd( lifted.gameObject, iTween.Hash ( "time", 0.5f, "isLocal", true, "x", targetPos.x, "easetype", iTween.EaseType.easeInOutQuad ) );
					//lifted.position = transform.position + new Vector3( 0, 0.4f, 0 );
					
					// to keep track
					liftedObject = lifted.gameObject.GetComponent<BaseObject>();

					// Lo desactiva.
					liftedObject.gravityEnabled = false;
					liftedObject.collisionEnabled = false;
					
					liftSensor.gameObject.SetActive( false );
					
					//dropGuide.SetActive( true );
				}
				else if ( cooldown < 0 ) // Si no hay objeto, trata de pegar
				{
					animator.StopAnim();
					animator.PlayAnim("Attack" + facing );
					velocity *= 2;
				}
			}
			else // Ya tiene un objeto en la capocha, tirarlo!
			{
				
				liftedObject.velocity += (direction * 0.02f) + (velocity * 1.0f); 
				liftedObject.velocity.y += 0.05f;
				liftedObject.transform.parent = worldOwner.transform;
				liftedObject.gravityEnabled = true;
				
				liftedObject = null;
				liftSensor.sensedObject = null;
				liftSensor.gameObject.SetActive( true );
			}
			
		}
		
		// DEATH BY FALL
		if ( transform.position.y < worldOwner.deathYLimit.position.y )
		{
			die ();
		}
		
		lockLeft--;
		lockRight--;
		lockDown--;
		lockUp--;
	}
	

	
	void die()
	{
		hearts = maxHearts;
		inmuneTimer = 0;
		//transform.position = worldOwner.startingPoint.position;
		velocity = Vector3.zero;
		gravity = Vector3.zero;
		
		worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );
		
		gameObject.SetActive(false);
	}
	
	public void OnHit( GameObject other )
	{
		if ( inmuneTimer > 0 )
			return;
		
		hearts--;
		
		inmuneTimer = 0.5f;
		
		if ( hearts == 0 )
			die();
	}
	
	override protected void TestFloor( Collider other )
	{
		base.TestFloor( other );
		
		if ( currentFloor != null && currentFloor.tag == "Floor" )
		{
			lastSafeFloor = currentFloor;
		}
	}
	
	override protected void TestWalls( Collider other )
	{
		if ( animator.isAnimPlaying("Attack") )
		{
			other.SendMessage ("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);
		}
		

		
		BaseObject bo = other.GetComponent<BaseObject>();

		if ( bo != null )
		{
			if ( !bo.collisionEnabled )
			{
				return;
			}
		}
		
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
			lockRight = 2;
			break;
		case 1:
			adjustX = true;
			lockLeft = 2;
			break;
		case 2:
			adjustZ = true;
			lockDown = 2;
			break;
		case 3:
			adjustZ = true;
			lockUp = 2;
			break;
		}
		
		if ( animator.isAnimPlaying("Attack") )
		{
			if ( other.tag == "Wall" )
			{
				animator.StopAnim();
				velocity *= -2.0f;
			}
		}
//		else 
//		if ( straightTimer > 0.7f )
//			velocity *= -2.0f;
		
		straightTimer = 0;
		
		

		transform.position = new Vector3( adjustX ? closestBoundExit.x : transform.position.x, 
										  transform.position.y, 
										  adjustZ ? closestBoundExit.z : transform.position.z );
	}

		
		
		
}
