using UnityEngine;
using System.Collections;

public class Player : BaseObject 
{
	protected enum State
	{
		IDLE,
		WALKING,
		ATTACKING,
		HIT,
		DYING
	};
	
	State _state = State.IDLE;
	float stateTimer = 0;
	protected State state 
	{
		get { return _state; }
		
		set 
		{ 
			_state = value;
			stateTimer = 0;
			switch ( _state )
			{
				case State.ATTACKING:
					testedAttacks = false;

					if ( Random.Range (0,2) == 0)
					{
						sfxAttack1.pitch = Random.Range (0.8f, 1.2f);
						sfxAttack1.Play();
					}
					else 
					{
						sfxAttack2.pitch = Random.Range (0.8f, 1.2f);
						sfxAttack2.Play();
					}
				
					break;
				case State.HIT:
					frictionCoef = 0.999f;
					break;
			}
		}
	}

	public bool hasLamp = true;
	[HideInInspector] public bool hasGhostSword = false;
	public int keysCount = 0;
	
	bool testedAttacks = false;

	SpriteAnimator animator;
	
	public Transform helperPivot;
	public Lamplight lampLightNoFlip;
	public Lamplight lampLightFlip;
	public BaseObjectSensor liftSensor;
	public BaseObjectSensor switchSensor;

	public AttackSensor attackSensorForward;
	public AttackSensor attackSensorBack;
	public AttackSensor attackSensorLeft;
	public AttackSensor attackSensorRight;

	private AttackSensor attackSensor;
	public GameObject dropGuide;
	
	private Torch[] lights;
	private Light torchLight;
	
	public bool darknessMechanic;
	public bool canAttack = true;
	public bool canUsePotion = false;
	public bool canJump = true;
	
	public float comboCooldown = 0.6f;
	
	[HideInInspector] public bool inDarkness = true;
	[HideInInspector] public int hearts;
	[HideInInspector] public int lives;

	public float torchRatio;
	BoxCollider lastSafeFloor;

	float darkTestThreshold = 4.0f;
	bool torchOn = false;

	Vector3 cameraTarget;
	
	public KeyCode leftKey; 
	public KeyCode rightKey; 
	public KeyCode upKey; 
	public KeyCode downKey; 
	
	public KeyCode attackKey = KeyCode.Z;
	public KeyCode jumpKey = KeyCode.Keypad1;
	public KeyCode potionKey = KeyCode.K;
	
	public AudioSource sfxAttack1;
	public AudioSource sfxAttack2;
	public AudioSource sfxAttackEnemy;
	public AudioSource sfxAttackWall;
	string facing = "Right";
	
	float cooldown = 0;
	
	
	float straightTimer = 0;
	float inmuneTimer = 0;
	
	bool skidEnabled = false;
	bool isSkidding = false;
	float isFlipping = 0;
	
	[HideInInspector] public BaseObject liftedObject;
	public Vector3 direction;
	public float speed = 0.5f;
	
	float comboCount;
	
	float dx = 0, dy = 0;
	bool walkAnimFacingUp = false;
	bool walkAnimFlipped = false;
	
	public float attackJumpHeight = -0.045f;
	public float attackSpeedFactor = 30.0f;
	
	int potionType = 0;
	bool holdingPotion = false;
	
	float invisibilityCooldown = 0;
	public bool invisible = false;
	float speedCooldown = 0;
	
	float guideTimer = 0;
	
	bool deathAwaits = false;
	
	
	public bool isImmune { get { return inmuneTimer > 0; } }


	private BaseObject objectToIgnore;
	private float objectToIgnoreTimer;

	
	override protected void Start () 
	{
		base.Start();

		animator = GetComponentInChildren<SpriteAnimator>();
		direction = Vector3.right;
		lights = (Torch[])FindObjectsOfType( typeof( Torch ) );
		torchLight = GetComponentInChildren<Light>();
		torchRatio = 100f;
		hearts = GameDirector.i.maxHearts;
		lives = GameDirector.i.maxLives;
		minStairClimb = 0.23f;
		climbStairs = true;
		
		if ( worldOwner == null )
			Destroy ( transform.parent.gameObject );
		
		//if ( darknessMechanic )
			InvokeRepeating( "TestDarkness", 0, 0.3f );
	}
	

	void UpdateAnims3FacesMode()
	{
		
		if ( state != State.ATTACKING )
		{
			if ( dx != 0 || dy != 0 )
			{
				animator.PlayAnim("Walk" + facing );
			}
			else 
			{
				animator.PlayAnim("Idle" + facing );
			}
		}
	}
	
	void UpdateAnims2FacesMode()
	{
		
		if ( dy > 0 )
			walkAnimFacingUp = true;
		else if ( dy < 0 )
			walkAnimFacingUp = false;

		if ( facing == "Left" )
		{
			if ( dy == 0 )
				walkAnimFacingUp = false;
			
			walkAnimFlipped = false;
		}
		else if ( facing == "Right" )
		{
			if ( dy == 0 )
				walkAnimFacingUp = false;
			
			walkAnimFlipped = true;
		}
		
		if ( lampLightFlip != null && lampLightNoFlip != null )
		{
			//GetComponentInChildren<Light>().enabled = torchOn;

			if ( animator.isAnimPlaying("Attack") || !darknessMechanic )//|| !torchOn )
			{
				lampLightNoFlip.gameObject.SetActive( false );
				lampLightFlip.gameObject.SetActive( false );
			}
			else 
			{				
				if ( walkAnimFlipped )
				{
					lampLightNoFlip.gameObject.SetActive( false );
					lampLightFlip.gameObject.SetActive( true );
				}
				else 
				{
					lampLightNoFlip.gameObject.SetActive( true );
					lampLightFlip.gameObject.SetActive( false );
				}
			}
		}
		
		string anim = "Idle";
		
		if ( !animator.isAnimPlaying("Attack") )
		{
			if ( dx != 0 || dy != 0 )
				anim = "Walk";
			else
				anim = "Idle";
			
			if ( walkAnimFacingUp )
			{
				if ( facing == "Left" )
					animator.PlayAnim( anim + "Left" );
				else 
				{
					if ( walkAnimFlipped )
						animator.PlayAnim( anim + "Up" );
					else 
						animator.PlayAnim( anim + "Left" );
				}
			}
			else 
			{
				if ( facing == "Right" )
					animator.PlayAnim( anim + "Right" );
				else 
				{
					if ( walkAnimFlipped )
						animator.PlayAnim( anim + "Right" );
					else 
						animator.PlayAnim( anim + "Down" );
				}
			}			
		}
	}	
	
	void UpdateDarknessMechanic()
	{
		if ( torchLight && !darknessMechanic )
		{
			torchLight.enabled = false;			
		}
		
		if ( torchLight && darknessMechanic )
		{
			torchLight.enabled = true;
			
			if ( inDarkness )
				torchOn = true;

			if ( torchOn && torchRatio > 0 )
			{
				torchRatio -= (Time.deltaTime * 100f) / 30f; // / secs
				torchRatio = Mathf.Clamp ( torchRatio, 0, 100 );
				
				if ( torchRatio == 0 && inDarkness )
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.4f, "Find light... quickly...");
					speed *= 0.7f;				
				}
			}
		
			if ( torchRatio < 50 )
				torchLight.intensity = (torchRatio / 50f) * 0.8f;
			else 
				torchLight.intensity = 0.8f;
		}		
	}
	
	public void ResetLiftSensor()
	{
		liftedObject = null;
		liftSensor.sensedObject = null;
		liftSensor.gameObject.SetActive( true );		
	}
	// 
 	// Returns true if any object was lifted or thrown. Return false otherwise. 
	//
	bool TryToLiftObject()
	{
		//
		// Trata de levantar un objeto...
		//
		if ( liftedObject == null ) 
		{
			if ( liftSensor != null && liftSensor.sensedObject != null && liftSensor.sensedObject.isLiftable )
			{
				Transform lifted = liftSensor.sensedObject.transform;
				lifted.parent = transform;
				
				// Pone el objeto en la cabeza del flaco.
				iTween.MoveTo( lifted.gameObject, iTween.Hash ( "isLocal", true, "position", new Vector3(0,0.6f,0), "time", 0.2f, "easetype", iTween.EaseType.easeOutCirc ) );
				
				// To keep track.
				liftedObject = lifted.gameObject.GetComponent<BaseObject>();
				
				// Lo desactiva.
				liftedObject.gravityEnabled = false;
				liftedObject.collisionEnabled = false;
				liftedObject.SendMessage ("OnLifted", gameObject, SendMessageOptions.DontRequireReceiver);
				
				liftSensor.gameObject.SetActive( false );
				
				return true;
			}
		}
		else
		{
			liftedObject.velocity += (direction * 0.02f) + (velocity * 1.0f); 
			
			liftedObject.gravity.y -= 0.025f;
			liftedObject.transform.parent = worldOwner.transform;
			liftedObject.gravityEnabled = true;
			liftedObject.collisionEnabled = true;

			objectToIgnore = liftedObject;
			objectToIgnoreTimer = 0.3f;

			ResetLiftSensor();
			
			return true;
		}
		
		return false;
	}
	
	void UpdateSkidding()
	{
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
			if ( ( ( dx < 0 && velocity.x > 0 ) || ( dx > 0 && velocity.x < 0 ) ) || 
				   ( dy < 0 && velocity.z > 0 ) || ( dy > 0 && velocity.z < 0 ) )
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
	}
	
	void UpdateCompass()
	{
		
		if ( worldOwner == GameDirector.i.worldRight )
		{
			guideTimer += Time.deltaTime;
			if ( guideTimer > 0.2f && invisible == false )
			{
				guideTimer -= 0.2f;
				GameDirector.i.SpawnGuideBlob();
			}
		}		
	}
	
	void UpdateAttack()
	{
		if ( cooldown > 0 )
		{
			cooldown -= Time.deltaTime;
		
			if ( cooldown <= 0 )
			{
				cooldown = 0;
				comboCount = 0;
			}
		}
		
		if ( Input.GetKeyDown( attackKey )  )
		{
			if ( torchRatio <= 0 && inDarkness )
			{
				GameDirector.i.ShowTextPopup( gameObject, 0.4f, "Can't see :(" );
			}
			else
			if ( !TryToLiftObject() && cooldown <= comboCooldown && canAttack )
			{
				//if ( comboCount == 1 )
				//	print ("que..." + animator.GetPlayTime() );
				if ( comboCount == 0 )
				{
					if ( animator.isAnimPlaying("Attack") )
					{
						animator.AppendAnim("Attack2" + facing );
					}
					else 
					{
						animator.StopAnim();
						animator.PlayAnim("Attack2" + facing );
					}

					velocity *= 2.50f;
					cooldown = comboCooldown;
					comboCount++;
					frictionCoef = 0.9f;
					state = State.ATTACKING;
					//attackSensor.ResetSensorChecks();
				}
				else 
				if ( comboCount == 1 )// && animator.GetPlayTime() > 0.18f )
				{
					if ( animator.isAnimPlaying("Attack") )
					{
						animator.AppendAnim("Attack" + facing );
					}
					else 
					{
						animator.StopAnim();
						animator.PlayAnim("Attack" + facing );
					}
					velocity *= 2.50f;
					cooldown = comboCooldown * 1.25f;
					comboCount++;
					frictionCoef = 0.95f;
					state = State.ATTACKING;
					//attackSensor.ResetSensorChecks();
				} 
				else 
				if ( comboCount == 2 )// && animator.GetPlayTime() > 0.18f )
				{
					if ( animator.isAnimPlaying("Attack") )
					{
						animator.AppendAnim("Attack2" + facing );
					}
					else 
					{
						animator.StopAnim();
						animator.PlayAnim("Attack2" + facing );
					};
					velocity *= 3.0f;
					cooldown = comboCooldown * 1.25f;
					comboCount++;
					frictionCoef = 0.95f;
					state = State.ATTACKING;
					//attackSensor.ResetSensorChecks();
				}
			}
		}
	}

	virtual protected void Update () 
	{

		objectToIgnoreTimer -= Time.deltaTime;

		if ( objectToIgnoreTimer < 0 )
			objectToIgnore = null;

		velocity = Vector3.Normalize( velocity ) * Mathf.Clamp ( velocity.magnitude, 0, speed * 2 );
		
		if ( deathAwaits )
		{
			hearts = GameDirector.i.maxHearts;
			inmuneTimer = 0;
			transform.position = worldOwner.startingPoint.position;
			
			velocity = gravity = accel = Vector3.zero;
			torchRatio = 100;
			lives --;
			
			worldOwner.BroadcastMessage( "OnPlayerDead", SendMessageOptions.DontRequireReceiver );
			deathAwaits = false;
			
			GameDirector.i.worldRight.camera.earthquakeEnabled = false;
			
//			if ( lives == -1 )
//				GameDirector.i.OnLeftWins();
//			else 
				GUIScreenFeedback.i.ShowTriesLeft( lives );
		}
		
		if ( lives < 0 )
			return;
		
		dx = 0; dy = 0;
		
		if ( state != State.ATTACKING  )
		{
			if ( Input.GetKey(leftKey) && !stuckLeft )
				dx = -1;
			
			if ( Input.GetKey(rightKey) && !stuckRight )
				dx = 1;
			
			if ( Input.GetKey(upKey) && !stuckForward )
				dy = 1;
			
			if ( Input.GetKey(downKey) && !stuckBack )
				dy = -1;
			
			if ( Input.GetKeyDown(leftKey) || 
				 Input.GetKeyDown(rightKey) || 
				 Input.GetKeyDown(upKey) || 
				 Input.GetKeyDown(downKey)  )
			{
				if ( dx != 0 || dy != 0 )
					straightTimer = 0;
			}
		}
		
		if ( dx != 0 || dy != 0 )
		{
			UpdateCompass();
			straightTimer += Time.deltaTime;
		}

		if ( inmuneTimer > 0 )
		{
			inmuneTimer -= Time.deltaTime;

			if ( inmuneTimer <= 0 )
				animator.renderer.material.SetColor ( "_AddColor", Color.black );
			else 
			{
				if ( Time.frameCount % 4 < 2 )
					animator.renderer.material.SetColor ( "_AddColor", new Color(0.1f,0.1f,0.1f) );
				else 
					animator.renderer.material.SetColor ( "_AddColor", Color.black );
			}
		}
		
		float tmpSpeed = speed;
		
		if ( dx != 0 && dy != 0 )
			tmpSpeed *= 0.707f;
		
		
		UpdateDarknessMechanic();
		
		accel += tmpSpeed * new Vector3( dx, 0, dy );

		//if ( (dx != 0 || dy != 0) )
		//	frictionCoef += (0.66f - frictionCoef) * 0.5f;

		
		float threshold = 0.001f;
		
		UpdateSkidding();

		
		if ( state != State.ATTACKING )
		{
			if ( Input.GetKey(leftKey) )
			{
				attackSensor = attackSensorLeft;
				facing = "Left";
				direction = Vector3.left;
			}
			else if ( Input.GetKey(rightKey) )
			{
				attackSensor = attackSensorRight;
				facing = "Right";
				direction = Vector3.right;
			}
			else if ( Input.GetKey(upKey) )
			{
				attackSensor = attackSensorForward;

				facing = "Up";
				direction = Vector3.forward;
			}
			else if ( Input.GetKey(downKey) )
			{
				attackSensor = attackSensorBack;

				facing = "Down";
				direction = Vector3.back;
			}
		}
		
		stateTimer += Time.deltaTime;
		
		switch( state )
		{
			case State.IDLE:
				if ( dx != 0 || dy != 0 )
				{
					state = State.WALKING;
					break;
				}
			
				if ( Input.GetKey ( attackKey ) )
				{
					if ( switchSensor.sensedObject != null && switchSensor.sensedObject.isSwitch )
					{
						OnPressSwitch( switchSensor.sensedObject.gameObject );
					}
				}
			
				frictionCoef += (0.66f - frictionCoef) * 0.9f;

				break;
			case State.ATTACKING:
			{
				if ( !animator.isAnimPlaying("Attack") )
				{
					state = State.IDLE;
					break;
				}

				if ( !testedAttacks )
				{
					testedAttacks = true;
					sfxAttackEnemy.Play();
				}

				attackSensor.Cleanup();

				if ( attackSensor.sensedObjects.Count > 0 && animator.GetPlayTime() < 0.3f )
				{
					foreach ( BaseObject bo in attackSensor.sensedObjects )
					{
						bo.OnHit( gameObject );
					}
				}
			}
			break;
			case State.WALKING:
				if ( dx == 0 && dy == 0 )
					state = State.IDLE;
			
				frictionCoef += (0.66f - frictionCoef) * 0.9f;

				break;
			case State.HIT:
				frictionCoef += (0.66f - frictionCoef) * 0.1f;
				canAttack = false;

				if ( stateTimer > 0.3f )
				{
					state = State.IDLE;
					canAttack = true;
				}

				break;
		}

		if(Input.GetKey(jumpKey) && canJump && currentFloor != null)
		{
			gravity.y = attackJumpHeight;
		}
		
		UpdateAnims2FacesMode();

		helperPivot.rotation = Quaternion.LookRotation( direction );
		
		if ( dropGuide && dropGuide.activeSelf  )
		{
			Vector3 caca = (transform.position ) / 0.4f;
			caca = new Vector3( Mathf.RoundToInt(caca.x), 
				Mathf.RoundToInt(caca.y), 
				Mathf.RoundToInt(caca.z) );
			dropGuide.transform.position = (caca * 0.4f) - new Vector3(0, 0, 0);
		}
		
				
		UpdateAttack();
		
		// DEATH BY FALL
		if ( transform.position.y < worldOwner.deathYLimit.position.y )
		{
			if ( lastSafeFloor == null )
			{
				Debug.Log("Nunca hubo un lastSafeFloor... Saca el gameObject del vacio, macho.");
			}
			else 
			{
				velocity = Vector3.zero;
				gravity = Vector3.zero;
				transform.position = lastSafeFloor.transform.position + new Vector3(0, .4f, 0);
				OnHit ( null );
				
				if ( hearts > 0 )
				{
					foreach( FallingFloor floor in worldOwner.GetComponentsInChildren<FallingFloor>() )
					{
						floor.OnPlayerDead();
					}
				}
			}
		}
		
		// GH: Adding potion effects
		
		if(holdingPotion && Input.GetKey(potionKey) && canUsePotion)
		{
			consumePotion();
		}
		
		// GH: invisbility checks
		if(invisibilityCooldown > 0.0f)
		{
			
			invisibilityCooldown -= Time.deltaTime;
		}
		else if(invisibilityCooldown < 0.0f)
		{
			invisibilityCooldown = 0.0f;
			
			// GH: Restore to normalcy
			reverseInvisibility();
		}
		
		// GH: Speed checks 
		if(speedCooldown > 0.0f)
		{
			speedCooldown -= Time.deltaTime;
		}
		else if(speedCooldown < 0.0f)
		{
			speedCooldown = 0.0f;
			//speed -= 0.01f;
			speed -= 0.01f;
		}
		
	}
	
	private void reverseInvisibility()
	{
		Material mat = animator.renderer.material;
		mat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		invisible = false;
	}
	
	public void consumePotion()
	{
		switch(potionType)
		{
			case Fountain.FOUNTAIN_LIFE: 
				hearts = GameDirector.i.maxHearts;
			break;
			
			case Fountain.FOUNTAIN_INVISIBILITY:
				invisibilityCooldown = 20.0f;
				invisible = true;
				//renderer.material.SetColor();
				Material mat = animator.renderer.material;
				mat.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
			break;
			
			case Fountain.FOUNTAIN_SPEED:
				speed += 0.01f;
				speedCooldown = 20;
			break;
			
			
		}
	
		holdingPotion = false;
	}
	
	public void addPotionType(int type)
	{
		potionType = type;
		holdingPotion = true;
		canUsePotion = true;
	}
	
	void Die()
	{
		if ( deathAwaits )
			return;
		
		if ( liftedObject != null ) // It was carrying something.
		{
			// TODO: Meter efecto de particulas aca
			
			liftedObject.transform.position = lastSafeFloor.transform.position + new Vector3(0, .4f, 0);
			
			liftedObject.transform.parent = worldOwner.transform;
			liftedObject.gravityEnabled = true;
			liftedObject.collisionEnabled = true;
			
			ResetLiftSensor();
		}
		
		deathAwaits = true;
		
//		((AztecPlayer)GameDirector.i.playerLeft).trapCurrency += 500;
//		GameDirector.i.ShowTextPopup( GameDirector.i.playerLeft.gameObject, 0.8f, "+" + 500 );
		
	}
	
	public override void OnHit( GameObject other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();

		if ( state == State.ATTACKING && attackSensor.IsObjectInside( bo ) )
		{
			if ( bo is Skelly || bo is Bat )
				return;
		}

		if ( inmuneTimer > 0 )
		{
			print("inmune");
			return;
		}

		if ( deathAwaits )
		{
			print("death awaits");
			return;
		}	
		//if ( state == State.ATTACKING )
		//	return;

		print("getting killed");
		hearts--;
		
		inmuneTimer = 2.0f;
		frictionCoef = 0.99f;

		state = State.HIT;
		
		if ( hearts == 0 )
		{
			Die();
		}
		else 
		{
//			((AztecPlayer)GameDirector.i.playerLeft).trapCurrency += 200;
//			GameDirector.i.ShowTextPopup( ((AztecPlayer)GameDirector.i.playerLeft).gameObject, 0.8f, "+" + 200 );
		}
	}
	
	virtual protected void OnPressSwitch( GameObject switchPressed )
	{
		switchPressed.SendMessage ("OnPressedFuture", gameObject, SendMessageOptions.DontRequireReceiver);		
	}
	
	override protected void TestFloor( Collider other )
	{
		if ( deathAwaits )
			return;

		base.TestFloor( other );
		
		if ( currentFloor != null && currentFloor.tag == "Floor" && currentFloor.name != "FloorFallingFuture" )
		{
			if ( !currentFloor.name.Contains( "Unsafe" ) ) 
				lastSafeFloor = currentFloor;
		}
	}
	
	override protected void TestWalls( Collider other )
	{
		if ( deathAwaits )
			return;
		
		BaseObject bo = other.GetComponent<BaseObject>();

		if ( objectToIgnore != null && bo == objectToIgnore )
			return;

		//bool collidedAgainstEnemyImmune = bo != null && bo.dontCollideWhenImmune && isImmune;
		
		//if ( !collidedAgainstEnemyImmune )
		{
			base.TestWalls( other );
		}
		
		if ( animator != null && state == State.ATTACKING )
		{
			if ( other.tag == "Wall" )
			{
				//print ("bounce wall");
				state = State.HIT;

				//velocity *= -.5f;
				//frictionCoef = 0.999f;
			}
		}
	}


	void TestDarkness()
	{
		if ( !darknessMechanic )
			return;
		
		float minLightDistance = 9999999f;
		Torch nearestLight = null;
		
		foreach ( Torch torch in lights )
		{
			if ( torch == null )
				continue;
			
			//if ( torch.light.intensity <= 0.1f )
			//	continue;
			
			float dist = Vector3.Distance( transform.position, torch.transform.position );
	
			if ( dist < 1.2f ) //nearestLight.range * 0.33f )
			{
				torchOn = false;
				
				if ( torchRatio > 0 )
				{
					if ( !torch.isTurnedOn() && torchRatio > 0 )
					{
						torch.TurnOn();
						
					}
				}
				
				if ( torch.isTurnedOn() )
				{
					torchRatio = 100f;
				}
				
			}			
			if ( dist > 10 )
				continue;
			
			if ( dist < minLightDistance )
			{	
				Vector3 myXZ = transform.position;
				//myXZ.y = 0;
	
				Vector3 hisXZ = torch.transform.position;
				//hisXZ.y = 0;
				
				Vector3 dir = myXZ - hisXZ;
				//dir.Normalize();
				
				RaycastHit[] info = Physics.RaycastAll( torch.transform.position, dir, dist );
				bool obstructed = false;
	
				foreach ( RaycastHit i in info )
				{
					if ( i.collider.gameObject.name.Contains("Tile") )
					{
						//Debug.DrawRay( torch.transform.position, i.point - torch.transform.position, Color.red, 0.2f );

						obstructed = true;
						break;
					}
				}
				
				if ( obstructed )
				{
					torch.OutLineOfSight();
					continue;
				}
				
				torch.InLineOfSight();
				
				if ( torch.light.intensity > .2f )
				{
					/*
					if ( dist < 8 )
						Debug.DrawRay( torch.transform.position, dir, Color.white, 0.2f );
					else
						Debug.DrawRay( torch.transform.position, dir, Color.blue, 0.2f );
					 */
					minLightDistance = dist;
					nearestLight = torch;
				
				}
			}
		}
		
		
		bool preInDarkness = inDarkness;
		
		if ( nearestLight != null )
		{	
			//Debug.DrawRay( nearestLight.transform.position+ Vector3.one*0.1f, transform.position - (nearestLight.transform.position + Vector3.one*0.1f), Color.green, 0.3f );
			if ( nearestLight.light.intensity > .2f )
			{
				inDarkness = minLightDistance > (2 * darkTestThreshold);
				
				if ( !inDarkness )
					torchOn = false;					
			}
			else 
				inDarkness = true;
		}
		else 
			inDarkness = true;
		
		
		if ( torchRatio == 0 && preInDarkness != inDarkness )
		{
			if ( inDarkness )
			{
				GameDirector.i.ShowTextPopup( gameObject, 0.4f, "Find light... quickly...");
				speed *= 0.7f;
			}
			else
			{
				GameDirector.i.ShowTextPopup( gameObject, 0.4f, "I can see again!" );
				speed *= 1.4f;
			}
		}

	}
}
