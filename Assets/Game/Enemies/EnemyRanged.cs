using UnityEngine;
using System.Collections;

public class EnemyRanged : BaseObject {

	SpriteAnimator animator;
	
	public Transform helperPivot;
	public BaseObjectSensor liftSensor;
	public BaseObjectSensor playerSensor;
	
	public GameObject projectile;
	
	[HideInInspector]
	public int hearts;
	
	public int maxHearts = 2;
	
	public float speed = 0.5f;
	public float attackJumpHeight = -0.045f;
	public float attackSpeedFactor = 2.0f;
	public float attackCooldown = 2.0f;

	BoxCollider lastSafeFloor;
	
	Vector3 startPosition;
	
	override protected void Start () 
	{
		animator = GetComponentInChildren<SpriteAnimator>();
		base.Start();
		direction = Vector3.right;
		hearts = maxHearts;
		state = State.SLEEPING;
		playerSensor.typeFilter = typeof( Player );
		startPosition = transform.position;
		controller = GetComponent<EnemyController>();
	}	

	string facing = "Right";
	
	float cooldown = 0;
	
	float lockDown = 0;
	float lockUp = 0;
	float lockRight = 0;
	float lockLeft = 0;
	
	public bool canGoDown { get { return lockDown > 0; } }
	public bool canGoUp { get { return lockUp > 0; } }
	public bool canGoRight { get { return lockRight > 0; } }
	public bool canGoLeft { get { return lockLeft > 0; } }
	
	
	float straightTimer = 0;
	float inmuneTimer = 0;
	
	bool skidEnabled = false;
	bool isSkidding = false;
	float isFlipping = 0;
	
	BaseObject liftedObject;
	EnemyController controller;
	Vector3 direction;
	
	//bool sleeping = true;
	float prevdx, prevdy;
	
	float walkTimer;
	
	public enum State 
	{
		SLEEPING,
		ATTACKING,
		WALKING
	}
	
	public State stateValue;
	float stateTimer = 0;
	
	State state
	{ 
		set { if ( stateValue != value ) { stateValue = value; stateTimer = 0; } } 
		get { return stateValue; } 
	}
	
	
	void UpdateSleeping()
	{
		if ( playerSensor.sensedObject != null && playerSensor.sensedObject.GetComponent<Player>() != null )
		{
			controller.playerTarget = (Player)playerSensor.sensedObject;
			state = State.WALKING;
			controller.Init();
		}		
	}
	
	public virtual void UpdateAttacking()
	{
		if ( stateTimer > 0.99f )
		{
			animator.StopAnim();
			animator.PlayAnim("Attack" + facing );
		//	velocity = direction * speed * attackSpeedFactor;
		//	cooldown = attackCooldown;
		//	gravity.y = attackJumpHeight;
			
			GameObject dart = (GameObject)Instantiate(projectile, transform.position + direction * 0.4f, Quaternion.Euler(direction));
			dart.transform.parent = this.transform.parent;						
			state = State.WALKING;
		}
	}
	
	
	
	public virtual void UpdateWalking()
	{
		controller.UpdateAI ();
		
		float dx = 0, dy = 0;
		
		if ( !animator.isAnimPlaying("Attack") && currentFloor != null  )
		{
			if ( controller.GetKey( KeyCode.LeftArrow ) && lockLeft < 0 )
				dx = -1;
			
			if ( controller.GetKey( KeyCode.RightArrow ) && lockRight < 0 )
				dx = 1;
			
			if ( controller.GetKey( KeyCode.UpArrow ) && lockUp < 0)
				dy = 1;
			
			if ( controller.GetKey( KeyCode.DownArrow ) && lockDown < 0 )
				dy = -1;
		}
		
		if ( dx == prevdx && dy == prevdy )
		{
			straightTimer += Time.deltaTime;
		}
		else 
		{
			straightTimer = 0;
		}
		
		prevdx = dx;
		prevdy = dy;

		//print ("dx = "+dx+" dy = "+dy+" timer = " + straightTimer  + " ........ " + currentFloor );
		accel += speed * new Vector3( dx, 0, dy );
		
		frictionCoef += (groundFrictionCoef - frictionCoef) * 0.75f;
		
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
		}
		else 
		{
			cooldown -= Time.deltaTime;
		}
		

		if ( controller.GetKey ( KeyCode.LeftArrow ) )
		{
			facing = "Left";
			direction = Vector3.left;
		}
		else if ( controller.GetKey ( KeyCode.RightArrow ) )
		{
			facing = "Right";
			direction = Vector3.right;
		}
		else if ( controller.GetKey ( KeyCode.UpArrow ) )
		{
			facing = "Up";
			direction = Vector3.forward;
		}
		else if ( controller.GetKey ( KeyCode.DownArrow )  )
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
	}
	
	public virtual void LiftObject()
	{
		Transform lifted = liftSensor.sensedObject.transform;
		lifted.parent = transform;
		
		// pone el objeto en la cabeza del flaco.
		iTween.MoveTo( lifted.gameObject, iTween.Hash ( "isLocal", true, "position", new Vector3(0,0.45f,0), "time", 0.2f, "easetype", iTween.EaseType.easeOutCirc ) );
		
		// to keep track
		liftedObject = lifted.gameObject.GetComponent<BaseObject>();

		// Lo desactiva.
		liftedObject.gravityEnabled = false;
		liftedObject.collisionEnabled = false;
		
		liftSensor.gameObject.SetActive( false );		
	}
	
	public virtual void ThrowObject()
	{
		liftedObject.velocity += (direction * 0.02f) + (velocity * 1.0f); 
		liftedObject.velocity.y += 0.05f;
		liftedObject.transform.parent = worldOwner.transform;
		liftedObject.gravityEnabled = true;
		
		liftedObject = null;
		liftSensor.sensedObject = null;
		liftSensor.gameObject.SetActive( true );		
	}
	
	void Attack()
	{
		state = State.ATTACKING;
	}
	
	void Update () 
	{
		switch ( state )
		{
			case State.SLEEPING:
				UpdateSleeping();
				break;
			case State.WALKING:
				UpdateWalking();
				break;
			case State.ATTACKING:
				UpdateAttacking();
				break;
		}
		
		stateTimer += Time.deltaTime;
		
		if ( inmuneTimer > 0 )
		{
			animator.renderer.enabled = !animator.renderer.enabled;
			inmuneTimer -= Time.deltaTime;
			
			if ( inmuneTimer <= 0 )
				animator.renderer.enabled = true;
		}
		
		if ( controller.GetKey( KeyCode.Keypad0 ) )
		{
			if ( liftedObject == null ) // Trata de levantar un objeto...
			{
				if ( liftSensor.sensedObject != null && liftSensor.sensedObject.isLiftable )
				{
					LiftObject();
				}
				else if ( cooldown < 0 ) // Si no hay objeto, trata de pegar
				{
					Attack();
				}
			}
			else // Ya tiene un objeto en la capocha, tirarlo!
			{
				ThrowObject();
			}
			
		}
		
		// DEATH BY FALL
		if ( transform.position.y < worldOwner.deathYLimit.position.y )
			Die ();
		
		lockLeft--;
		lockRight--;
		lockDown--;
		lockUp--;
	}
	
	void Die()
	{
		worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );
		Destroy( gameObject );
	}
	
	public void OnHit( GameObject other )
	{
		if ( inmuneTimer > 0 )
			return;
		
		hearts--;
		
		inmuneTimer = 0.5f;
		
		if ( hearts == 0 )
			Die();
	}
	
	override protected void OnTriggerEnter( Collider other )
	{
		if ( other.GetComponent<Player>() != null )
		{
			Player p = other.GetComponent<Player>();

			if ( !p.isImmune )
			{
				p.OnHit( gameObject );
				p.velocity += direction * speed * attackSpeedFactor * 3f;
				p.gravity.y = -0.01f;
				velocity *= -1.2f;
			}
		}
		
		base.OnTriggerEnter( other );
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
			lockRight = 5;
			break;
		case 1:
			adjustX = true;
			lockLeft = 5;
			break;
		case 2:
			adjustZ = true;
			lockDown = 5;
			break;
		case 3:
			adjustZ = true;
			lockUp = 5;
			break;
		}
		
		straightTimer = 0;
		transform.position = new Vector3( adjustX ? closestBoundExit.x : transform.position.x, 
										  transform.position.y, 
										  adjustZ ? closestBoundExit.z : transform.position.z );
	}

		
	
}
