﻿using UnityEngine;
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

	BoxCollider lastSafeFloor;
	
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
	
	float lockDown = 0;
	float lockUp = 0;
	float lockRight = 0;
	float lockLeft = 0;
	
	public bool CantGoDown { get { return lockDown > 0; } }
	public bool CantGoUp { get { return lockUp > 0; } }
	public bool CantGoRight { get { return lockRight > 0; } }
	public bool CantGoLeft { get { return lockLeft > 0; } }
	
	
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
			controller.playerTarget = (Player)playerSensor.sensedObject;
			state = State.WALKING;
			sleepPhysics = false;
			controller.Init();
		}		
	}

	virtual protected void UpdateDying()
	{}
	
	virtual protected void UpdateAttacking()
	{
		if ( stateTimer > 0.33f )
		{
			animator.StopAnim();
			animator.PlayAnim("Attack" + facing );
			velocity = direction * speed * attackSpeedFactor;
			cooldown = attackCooldown;
			gravity.y = attackJumpHeight;
			state = State.WALKING;
		}
	}
	
	virtual protected void UpdateWalking()
	{
		controller.UpdateAI ();
		
		float dx = 0, dy = 0;
		
		if ( !animator.isAnimPlaying("Attack") && (currentFloor != null || !gravityEnabled)  )
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
	
	virtual protected void Die()
	{
		worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );

		if ( prefabExplosion != null )
		{
			GameObject explosion = (GameObject)Instantiate( prefabExplosion, transform.position, Quaternion.identity );
			explosion.transform.parent = transform.parent;
		}
		
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
		
		if ( state != State.DYING )
		{
			if ( !isGrounded )
			{
				Player p = other.GetComponent<Player>();
	
				if ( p != null && !p.isImmune )
				{
					p.OnHit( gameObject );
					p.velocity += direction * speed * attackSpeedFactor * .5f;
					//p.gravity.y = -0.03f;
					//p.transform.position += Vector3.up * 0.03f;
					//p.frictionCoef = 0.999f;
					velocity *= -1.2f;
				}
				
				Vine v = other.GetComponent<Vine>();
				
				if ( v != null )
					v.SendMessage ("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);
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
		base.TestWalls( other );
		
		if ( animator != null && !isGrounded )
		{
			if ( other.tag == "Wall" )
			{
				print ("bounce wall");
				state = State.WALKING;

				velocity *= -.5f;
				frictionCoef = 0.99f;
			}
		}
	}
}
