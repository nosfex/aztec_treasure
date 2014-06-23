using UnityEngine;
using System.Collections;

public class Player : BaseObject 
{
	SpriteAnimator animator;
	
	public Transform helperPivot;
	public Lamplight lampLightNoFlip;
	public Lamplight lampLightFlip;
	public BaseObjectSensor liftSensor;
	public BaseObjectSensor switchSensor;
	public AttackSensor attackSensor;
	public GameObject dropGuide;
	
	private Torch[] lights;
	private Light torchLight;
	
	public bool darknessMechanic;
	public bool canAttack = true;
	public bool canUsePotion = false;
	public bool canJump = true;
	
	public float attackCooldown = 0.3f;
	
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
	
	protected BaseObject liftedObject;
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
	
	bool isAttacking = false;
	float guideTimer = 0;
	
	bool deathAwaits = false;
	
	
	public bool isImmune { get { return inmuneTimer > 0; } }
	
	void TestDarkness()
	{
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
			if ( dist > 8 )
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
				inDarkness = minLightDistance > (nearestLight.light.range * darkTestThreshold);
				
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
				speed *= 0.5f;
			}
			else
			{
				GameDirector.i.ShowTextPopup( gameObject, 0.4f, "I can see again!" );
				speed *= 2.0f;
			}
		}

	}
	
	override protected void Start () 
	{
		animator = GetComponentInChildren<SpriteAnimator>();
		base.Start();
		direction = Vector3.right;
		lights = (Torch[])FindObjectsOfType( typeof( Torch ) );
		torchLight = GetComponentInChildren<Light>();
		torchRatio = 100f;
		hearts = GameDirector.i.maxHearts;
		lives = GameDirector.i.maxLives;
		
		if ( worldOwner == null )
			Destroy ( transform.parent.gameObject );
		
		if ( darknessMechanic )
			InvokeRepeating( "TestDarkness", 0, 0.3f );
	
	}
	

	void UpdateAnims3FacesMode()
	{
		if ( !animator.isAnimPlaying("Attack") )
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
			if ( animator.isAnimPlaying("Attack") )
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
	
	
	virtual protected void Update () 
	{

		
		if ( deathAwaits )
		{
			hearts = GameDirector.i.maxHearts;
			inmuneTimer = 0;
			transform.position = worldOwner.startingPoint.position;
			
			velocity = Vector3.zero;
			gravity = Vector3.zero;
			accel = Vector3.zero;
			torchRatio = 100;
			lives --;
			
			worldOwner.BroadcastMessage( "OnPlayerDead", SendMessageOptions.DontRequireReceiver );
			deathAwaits = false;
			
			GameDirector.i.worldRight.camera.earthquakeEnabled = false;
			
			if ( lives == -1 )
				GameDirector.i.OnLeftWins();
			else 
				GUIScreenFeedback.i.ShowTriesLeft( lives );
		}
		
		if ( lives < 0 )
		{
			return;
		}
		
		dx = 0; dy = 0;
		
		if ( !animator.isAnimPlaying("Attack") )// && currentFloor != null ) 
		{
			if ( Input.GetKey(leftKey) && lockLeft < 0 )
				dx = -1;
			
			if ( Input.GetKey(rightKey) && lockRight < 0 )
				dx = 1;
			
			if ( Input.GetKey(upKey) && lockUp < 0)
				dy = 1;
			
			if ( Input.GetKey(downKey) && lockDown < 0 )
				dy = -1;
			
			if ( Input.GetKeyDown(leftKey) && lockLeft <0 )
				straightTimer = 0;
			
			if ( Input.GetKeyDown(rightKey) && lockRight  <0)
				straightTimer = 0;
			
			if ( Input.GetKeyDown(upKey) && lockUp <0)
				straightTimer = 0;
			
			if ( Input.GetKeyDown(downKey) && lockDown <0 )
				straightTimer = 0;
			
		}
		
		if ( dx != 0 || dy != 0 )
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
			
			straightTimer += Time.deltaTime;
		}

		//print ("timer = " + straightTimer );
		
		
		if ( inmuneTimer > 0 )
		{
			animator.renderer.enabled = !animator.renderer.enabled;
			inmuneTimer -= Time.deltaTime;
			//collisionEnabled = false;
			
			if ( inmuneTimer <= 0 )
			{
				animator.renderer.enabled = true;
				//collisionEnabled = true;
			}
		}
		
		float tmpSpeed = speed;
		
		if ( dx != 0 && dy != 0 )
			tmpSpeed *= 0.707f;
		
		
		if ( torchLight && darknessMechanic )
		{
			if ( inDarkness )
			{
				torchOn = true;
				
			//	darkTestThreshold = 0.6f;
			}
			
			if ( torchOn && torchRatio > 0 )
			{
				
				torchRatio -= (Time.deltaTime * 100f) / 20f; // / secs
				torchRatio = Mathf.Clamp ( torchRatio, 0, 100 );
				
				if ( torchRatio == 0 && inDarkness )
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.4f, "Find light... quickly...");
					speed *= 0.5f;				
				}
			}
			
//			else 
//			{
//				darkTestThreshold = 0.9f;
//			}
			
			//if ( inDarkness && torchRatio <= 0 )
			//	OnHit ( null );
		
			if ( torchRatio < 50 )
				torchLight.intensity = (torchRatio / 50f) * 0.8f;
			else 
				torchLight.intensity = 0.8f;
		}
		
		accel += tmpSpeed * new Vector3( dx, 0, dy );

		//if ( (dx != 0 || dy != 0) )
		//	frictionCoef += (0.66f - frictionCoef) * 0.5f;

		if ( !animator.isAnimPlaying("Attack") )
		{
			isAttacking = false;
		}
		
		if ( !isAttacking )
		{
			frictionCoef += (0.66f - frictionCoef) * 0.9f;
		}
		
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
		
		if ( cooldown > 0 )
		{
			cooldown -= Time.deltaTime;
		
			if ( cooldown <= 0 )
			{
				cooldown = 0;
				comboCount = 0;
			}
		}

		if ( Input.GetKey(leftKey) )
		{
			facing = "Left";
			direction = Vector3.left;
		}
		else if ( Input.GetKey(rightKey) )
		{
			facing = "Right";
			direction = Vector3.right;
		}
		else if ( Input.GetKey(upKey) )
		{
			facing = "Up";
			direction = Vector3.forward;
		}
		else if ( Input.GetKey(downKey) )
		{
			facing = "Down";
			direction = Vector3.back;
		}
		
		
		if(Input.GetKey(jumpKey) && canJump && currentFloor != null)
		{
			
		//	accel += direction * speed * attackSpeedFactor;
			gravity.y = attackJumpHeight;
		}
		
		UpdateAnims2FacesMode();

		helperPivot.rotation = Quaternion.LookRotation( direction );
		
		if ( dropGuide && dropGuide.activeSelf  )
		{
			//if ( SnapAssistant.i && SnapAssistant.i.snapEnabled )
			{
				Vector3 caca = (transform.position ) / 0.4f;
				caca = new Vector3( Mathf.RoundToInt(caca.x), 
					Mathf.RoundToInt(caca.y), 
					Mathf.RoundToInt(caca.z) );
				dropGuide.transform.position = (caca * 0.4f) - new Vector3(0, 0, 0);
			}
		}
		
		if ( Input.GetKey ( attackKey ) )
		{
			if ( switchSensor.sensedObject != null && switchSensor.sensedObject.isSwitch )
			{
				OnPressSwitch( switchSensor.sensedObject.gameObject );
			}
		}
				
		if ( Input.GetKeyDown( attackKey )  )
		{
			if ( torchRatio <= 0 && inDarkness )
			{
				GameDirector.i.ShowTextPopup( gameObject, 0.4f, "Can't see :(");
			}
			else
			if ( liftedObject == null ) // Trata de levantar un objeto...
			{
				if ( liftSensor != null && liftSensor.sensedObject != null && liftSensor.sensedObject.isLiftable )
				{
					Transform lifted = liftSensor.sensedObject.transform;
					lifted.parent = transform;
					
					// pone el objeto en la cabeza del flaco.
					iTween.MoveTo( lifted.gameObject, iTween.Hash ( "isLocal", true, "position", new Vector3(0,0.4f,0), "time", 0.2f, "easetype", iTween.EaseType.easeOutCirc ) );
					//iTween.MoveAdd( lifted.gameObject, iTween.Hash ( "time", 0.5f, "isLocal", true, "x", targetPos.x, "easetype", iTween.EaseType.easeInOutQuad ) );
					//lifted.position = transform.position + new Vector3( 0, 0.4f, 0 );
					
					// to keep track
					liftedObject = lifted.gameObject.GetComponent<BaseObject>();

					// Lo desactiva.
					liftedObject.gravityEnabled = false;
					liftedObject.collisionEnabled = false;
					liftedObject.SendMessage ("OnLifted", gameObject, SendMessageOptions.DontRequireReceiver);
					
					liftSensor.gameObject.SetActive( false );
					
					
					
					//dropGuide.SetActive( true );
				}
				else if ( cooldown <= attackCooldown && canAttack ) // Si no hay objeto, trata de pegar
				{
					if ( comboCount == 0 )
					{
						animator.StopAnim();
						animator.PlayAnim("Attack2" + facing );
						velocity *= 2.50f;
						cooldown = attackCooldown;
						comboCount++;
						isAttacking = true;
						frictionCoef = 0.9f;
						
					}
					else 
					if ( comboCount == 1 )
					{
						animator.StopAnim();
						animator.PlayAnim("Attack" + facing );
						velocity *= 1.5f;
						cooldown = attackCooldown * 1.25f;
						comboCount++;
						frictionCoef = 0.95f;
						isAttacking = true;
					}
					
				}
			}
			else // Ya tiene un objeto en la capocha, tirarlo!
			{
				
				liftedObject.velocity += (direction * 0.02f) + (velocity * 1.0f); 
				//liftedObject.velocity.y += 0.05f;
				liftedObject.gravity.y -= 0.025f;
				liftedObject.transform.parent = worldOwner.transform;
				liftedObject.gravityEnabled = true;
				liftedObject.collisionEnabled = true;
				
				liftedObject = null;
				liftSensor.sensedObject = null;
				liftSensor.gameObject.SetActive( true );
			}
			
		}
		
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
						floor.ResetState();
					}
				}
			}
		}
		
		if ( animator.isAnimPlaying("Attack") && attackSensor.sensedObject != null )
		{
			BaseObject attackedObject = attackSensor.CheckSensorOnce();

			if ( attackedObject )
			{
				attackedObject.SendMessage ("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);
				
				//if ( attackedObject.GetComponent<Vine>() == null )
				//	velocity *= -0.5f;
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
		
		lockLeft--; lockRight--; lockDown--; lockUp--;
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
			
			liftedObject = null;
			liftSensor.sensedObject = null;
			liftSensor.gameObject.SetActive( true );
		}
		
		deathAwaits = true;
		
		((AztecPlayer)GameDirector.i.playerLeft).trapCurrency += 500;
		GameDirector.i.ShowTextPopup( GameDirector.i.playerLeft.gameObject, 0.8f, "+" + 500 );
		
	}
	
	public void OnHit( GameObject other )
	{
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
		print("getting killed");
		hearts--;
		
		
		inmuneTimer = 1.0f;
		frictionCoef = 0.99f;
		isAttacking = false;
		
		if ( hearts == 0 )
		{
			Die();
		}
		else 
		{
			((AztecPlayer)GameDirector.i.playerLeft).trapCurrency += 200;
			GameDirector.i.ShowTextPopup( ((AztecPlayer)GameDirector.i.playerLeft).gameObject, 0.8f, "+" + 200 );
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
		
		if ( currentFloor != null && currentFloor.tag == "Floor" )
		{
			lastSafeFloor = currentFloor;
		}
	}
	
	override protected void TestWalls( Collider other )
	{
		if ( deathAwaits )
			return;

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
		
		if ( animator != null && isAttacking  )
		{
			if ( other.tag == "Wall" )
			{
				print ("bounce wall");
				isAttacking = false;
				//animator.StopAnim();
				velocity *= -.5f;
				frictionCoef = 0.99f;
				//if ( gravity.y > -0.03f )
				//	gravity.y = -0.03f;
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
