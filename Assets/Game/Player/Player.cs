﻿using UnityEngine;
using System.Collections;

public class Player : BaseObject 
{
	SpriteAnimator animator;
	
	public Transform helperPivot;
	public Lamplight lampLightNoFlip;
	public Lamplight lampLightFlip;
	public BaseObjectSensor liftSensor;
	public AttackSensor attackSensor;
	public GameObject dropGuide;
	
	private Torch[] lights;
	private Light torchLight;
	
	public bool darknessMechanic;
	public bool canAttack = true;
	
	[HideInInspector] public bool inDarkness = true;
	[HideInInspector] public int hearts;
	
	public bool isImmune { get { return inmuneTimer > 0; } }
	
	public float torchRatio;
	BoxCollider lastSafeFloor;
	float darkTestThreshold = 1.3f;
	bool torchOn = false;
	
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
	
			if ( dist < minLightDistance )
			{	
				Vector3 myXZ = transform.position;
				myXZ.y = 0;
	
				Vector3 hisXZ = torch.transform.position;
				hisXZ.y = 0;
				
				Vector3 dir = myXZ - hisXZ;
				dir.Normalize();
				
				RaycastHit[] info = Physics.RaycastAll( torch.transform.position, dir, dist );
				Debug.DrawRay( torch.transform.position, dir, Color.white, 0.6f );
				bool obstructed = false;
	
				foreach ( RaycastHit i in info )
				{
					if ( i.collider.gameObject.name.Contains("Tile") )
					{
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
				
				minLightDistance = dist;
				nearestLight = torch;
			}
		}
		
		if ( nearestLight != null )
		{	
			if ( nearestLight.light.intensity > .2f )
				inDarkness = minLightDistance > nearestLight.light.range * darkTestThreshold;
			else 
				inDarkness = true;
			
			if ( minLightDistance < 1.2f ) //nearestLight.range * 0.33f )
			{
				torchOn = false;
				nearestLight.TurnOn();
				torchRatio = 100f;
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
		
		if ( worldOwner == null )
			Destroy ( transform.parent.gameObject );
		
		if ( darknessMechanic )
			InvokeRepeating( "TestDarkness", 0, 0.3f );
	
	}
	

	Vector3 cameraTarget;
	
	public KeyCode leftKey; 
	public KeyCode rightKey; 
	public KeyCode upKey; 
	public KeyCode downKey; 
	
	public KeyCode attackKey = KeyCode.Z;
	//public KeyCode liftKey = KeyCode.G;
	
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
			
			worldOwner.BroadcastMessage( "OnPlayerDead", SendMessageOptions.DontRequireReceiver );
			deathAwaits = false;
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
			straightTimer += Time.deltaTime;


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
		
		accel += tmpSpeed * new Vector3( dx, 0, dy );
		
		if ( torchLight && darknessMechanic )
		{
			if ( inDarkness )
			{
				torchOn = true;
			//	darkTestThreshold = 0.6f;
			}
			
			if ( torchOn )
			{
				torchRatio -= (Time.deltaTime * 100f) / 30f; // / secs
				torchRatio = Mathf.Clamp ( torchRatio, 0, 100 );
			}
//			else 
//			{
//				darkTestThreshold = 0.9f;
//			}
			
			//if ( inDarkness && torchRatio <= 0 )
			//	OnHit ( null );
		
			if ( torchRatio < 50 )
				torchLight.intensity = (torchRatio / 50f) * 0.4f;
			else 
				torchLight.intensity = 0.4f;
		}
		
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
			if ( liftedObject == null ) // Trata de levantar un objeto...
			{
				if ( liftSensor.sensedObject != null && liftSensor.sensedObject.isSwitch )
				{
					OnPressSwitch( liftSensor.sensedObject.gameObject );
				}
			}
		}
		
		if ( Input.GetKeyDown( attackKey )  )
		{
			if ( liftedObject == null ) // Trata de levantar un objeto...
			{
				if ( liftSensor.sensedObject != null && liftSensor.sensedObject.isLiftable )
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
				else if ( cooldown <= 0.4f && canAttack ) // Si no hay objeto, trata de pegar
				{
					if ( comboCount == 0 )
					{
						animator.StopAnim();
						animator.PlayAnim("Attack2" + facing );
						velocity *= 2.50f;
						cooldown = 0.4f;
						comboCount++;
					}
					else 
					if ( comboCount == 1 )
					{
						animator.StopAnim();
						animator.PlayAnim("Attack" + facing );
						velocity *= 1.25f;
						cooldown = 0.6f;
						comboCount++;
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
				if ( attackedObject.GetComponent<Vine>() == null )
					velocity *= -0.5f;
			}
		}
		
		lockLeft--; lockRight--; lockDown--; lockUp--;
	}
	
	bool deathAwaits = false;
	
	void Die()
	{
		deathAwaits = true;
	}
	
	public void OnHit( GameObject other )
	{
		if ( inmuneTimer > 0 )
			return;

		if ( deathAwaits )
			return;
		
		hearts--;
		
		inmuneTimer = 1.0f;
		frictionCoef = 0.99f;
		
		if ( hearts == 0 )
			Die();
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
		
		if ( animator != null && animator.isAnimPlaying("Attack") )
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
