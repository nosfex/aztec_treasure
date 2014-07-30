using UnityEngine;
using System.Collections;

public class Skelly : BaseObject 
{
	protected SpriteAnimator animator;
	
	public Transform helperPivot;
	public BaseObjectSensor liftSensor;
	public BaseObjectSensor playerSensor;
	
	public GameObject prefabExplosion;
	
	[HideInInspector]
	public int hearts;
	
	public int maxHearts = 2;
	
	public float speed = 0.5f;
	public float attackJumpHeight = -0.045f;
	public float attackSpeedFactor = 2.0f;
	public float attackCooldown = 2.0f;
	
	public float playerKnockbackHitFactor = 0;

	//BoxCollider lastSafeFloor;
	
	Vector3 startPosition;
	
	override protected void Start () 
	{
		animator = GetComponentInChildren<SpriteAnimator>();
		base.Start();
		direction = Vector3.right;
		hearts = maxHearts;
		state = State.SLEEPING;
		sleepPhysics = true;
		playerSensor.typeFilter = typeof( Player );
		startPosition = transform.position;
		controller = GetComponent<EnemyController>();
	}	

	protected string facing = "Right";

	protected float cooldown = 0;
	
	float straightTimer = 0;
	protected float inmuneTimer = 0;
	
	bool skidEnabled = false;
	bool isSkidding = false;
	float isFlipping = 0;
	
	BaseObject liftedObject;
	protected EnemyController controller;
	protected Vector3 direction;
	
	//bool sleeping = true;
	float prevdx, prevdy;
	
	float walkTimer;
	
	public enum State 
	{
		SLEEPING,
		ATTACKING,
		WALKING,
		DYING
	}
	
	public State stateValue;
	protected float stateTimer = 0;
	
	protected State state
	{ 
		set { if ( stateValue != value ) { stateValue = value; stateTimer = 0; } } 
		get { return stateValue; } 
	}
	
	
	void UpdateSleeping()
	{
		if ( playerSensor.sensedObject != null && playerSensor.sensedObject.GetComponent<Player>() != null  
			&&  ((Player)(playerSensor.sensedObject)).invisible == false)
		{
			//print ("activate");
			controller.playerTarget = (Player)playerSensor.sensedObject;
			state = State.WALKING;
			sleepPhysics = false;
			controller.Init();
		}		
	}

	virtual protected void UpdateDying()
	{

	}
	bool jumpAttacking = false;
	virtual protected void UpdateAttacking()
	{
		//if ( animator.isPlaying )
		//	animator.StopAnim();
		if ( stateTimer > 1.0f )
		{ 
			if ( !jumpAttacking )
			{
				jumpAttacking = true;
				//animator.PlayAnim("Attack" + facing );
				velocity = direction * speed * attackSpeedFactor;
				cooldown = attackCooldown;
				gravity.y = attackJumpHeight;
				animator.PlayAnim("Walk" + facing );
			}
			else 
			{
				animator.StopAnim();

				if ( Time.frameCount % 4 < 2 )
					animator.renderer.material.SetColor ( "_AddColor", Color.red );
				else 
					animator.renderer.material.SetColor ( "_AddColor", Color.black );
				
				if ( isGrounded && gravity.y > 0 )
				{
					animator.renderer.material.SetColor ( "_AddColor", Color.black );
					jumpAttacking = false;
					state = State.WALKING;
				}
			}
			//state = State.WALKING;
		}
		else 
		{
			if ( facing == "Left" )
				animator.PlayAnim("AttackLeft");
			else if ( facing == "Right" )
				animator.PlayAnim("AttackRight");
			else 
				animator.PlayAnim("AttackRight");
		}
	}
	
	virtual protected void UpdateWalking()
	{
		controller.UpdateAI ();
		
		float dx = 0, dy = 0;

		if ( !animator.isAnimPlaying("Attack") && (isGrounded || !gravityEnabled)  )
		{
			if ( controller.GetKey( KeyCode.LeftArrow ) && !stuckLeft )
				dx = -1;
			
			if ( controller.GetKey( KeyCode.RightArrow ) && !stuckRight )
				dx = 1;
			
			if ( controller.GetKey( KeyCode.UpArrow ) && !stuckForward )
				dy = 1;
			
			if ( controller.GetKey( KeyCode.DownArrow ) && !stuckBack )
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
			/*else 
			{
				animator.PlayAnim("Walk" + facing );
				animator.PauseAnim();
				animator.GoToFrame(2);
			}*/
		}
	}
	
	virtual protected void LiftObject()
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
	
	virtual protected void ThrowObject()
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

	float hitFeedbackTimer = 0;
	
	virtual protected void Update () 
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
			case State.DYING:
				UpdateDying();
				break;
		}

		if ( hitFeedbackTimer > 0 )
		{
			if ( Time.frameCount % 4 < 2 )
				animator.renderer.material.SetColor ( "_AddColor", Color.yellow );
			else 
				animator.renderer.material.SetColor ( "_AddColor", Color.black );

			hitFeedbackTimer -= Time.deltaTime;

			if ( hitFeedbackTimer <= 0 )
				animator.renderer.material.SetColor ( "_AddColor", Color.black );
		}
		
		stateTimer += Time.deltaTime;
		
		if ( inmuneTimer > 0 )
		{
			animator.renderer.enabled = Time.frameCount % 4 < 2;
			inmuneTimer -= Time.deltaTime;
			
			if ( inmuneTimer <= 0 )
				animator.renderer.enabled = true;
		}
		
		if ( controller.GetKey( KeyCode.Keypad0 ) )
		{
			if ( liftedObject == null ) // Trata de levantar un objeto...
			{
//				if ( liftSensor.sensedObject != null && liftSensor.sensedObject.isLiftable && liftSensor.sensedObject.isGrounded )
//				{
//					LiftObject();
//				}
//				else 
				if ( cooldown < 0 ) // Si no hay objeto, trata de pegar
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

	}
	
	virtual protected void Die()
	{
	
		if ( prefabExplosion != null )
		{
			GameObject explosion = (GameObject)Instantiate( prefabExplosion, transform.position, Quaternion.identity );
			explosion.transform.parent = transform.parent;
		}
		
		Destroy( gameObject );

		worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );

	}
	
	public virtual void OnHit( GameObject other )
	{
		if ( inmuneTimer > 0 )
			return;

		hearts--;

		hitFeedbackTimer = 0.2f;
		//inmuneTimer = 0.3f;

		Player p = other.GetComponentInChildren<Player>();

		if ( p != null )//&& playerKnockbackHitFactor > 0 )
		{	
			p.velocity *= -playerKnockbackHitFactor;
			p.frictionCoef = 0.999f;
		}

		if ( hearts == 0 )
		{
			Die();
		}
		else 
		{
			velocity = direction * -0.05f;
			animator.renderer.material.SetColor ( "_AddColor", Color.black );
			jumpAttacking = false;
			state = State.WALKING;
		}
		
	}
	
	override protected void OnTriggerEnter( Collider other )
	{
		
		if ( state != State.DYING )
		{
			if ( !isGrounded )
			{
				Player p = other.GetComponent<Player>();
	
				if ( p != null && !p.isImmune )
				{
					//print ("skelly ataca algo");
					p.OnHit( gameObject );
					p.velocity += direction * speed * attackSpeedFactor * 1.5f;
					p.frictionCoef = 0.999f;
					velocity *= -1.2f;
				}
				
				BaseObject v = other.GetComponent<BaseObject>();
				
				if ( v != null )
					v.SendMessage ("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);
			}
				
		}
		
		base.OnTriggerEnter( other );
	}
	
	override protected void TestFloor( Collider other )
	{
		base.TestFloor( other );
		
//		if ( currentFloor != null && currentFloor.tag == "Floor" )
//		{
//			lastSafeFloor = currentFloor;
//		}
	}
	
	override protected void TestWalls( Collider other )
	{
		base.TestWalls( other );
		
		if ( animator != null && !isGrounded )
		{
			if ( other.tag == "Wall" )
			{
				//print ("bounce wall");
				if ( state != State.DYING )
				{
					state = State.WALKING;
					animator.renderer.material.SetColor ( "_AddColor", Color.black );
					jumpAttacking = false;
				}

				velocity *= -.5f;
				frictionCoef = 0.99f;
			}
		}
	}
}
