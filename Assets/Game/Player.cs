using UnityEngine;
using System.Collections;

public class Player : BaseObject 
{
	SpriteAnimator animator;
	
	public Transform helperPivot;
	public BaseObjectSensor liftSensor;
	public GameObject dropGuide;
	
	private Light[] lights;
	private Light torchLight;
	public bool inDarkness = false;
	
	[HideInInspector]
	public int hearts;
	
	public float torchRatio;
	BoxCollider lastSafeFloor;
	
	void TestDarkness()
	{
		float minLightDistance = 9999999f;
		Light nearestLight = null;
		
		foreach ( Light light in lights )
		{
			if ( light == null || light == torchLight )
				continue;
			
			if ( light.type != LightType.Point )
				continue;
			
			float dist = Vector3.Distance( transform.position, light.transform.position );
			
			if ( dist < minLightDistance )
			{	
				minLightDistance = dist;
				nearestLight = light;
			}
		}
		float threshold = 1.5f;

		if ( nearestLight != null )
		{				
			inDarkness = minLightDistance > nearestLight.range * threshold;
			
			if ( minLightDistance < nearestLight.range * 0.75f )
			{
				torchRatio = 100f;
			}
		}
	}
	
	override protected void Start () 
	{
		animator = GetComponentInChildren<SpriteAnimator>();
		base.Start();
		direction = Vector3.right;
		lights = (Light[])FindObjectsOfType( typeof( Light ) );
		torchLight = GetComponentInChildren<Light>();
		torchRatio = 100f;
		hearts = GameDirector.i.maxHearts;
		InvokeRepeating( "TestDarkness", 0, 0.2f );
	
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
	
	BaseObject liftedObject;
	Vector3 direction;
	public float speed = 0.5f;
	
	void Update () 
	{
		
		float dx = 0, dy = 0;
		
		
		if ( !animator.isAnimPlaying("Attack") && currentFloor != null  )
		{
			if ( Input.GetKey(leftKey) && lockLeft < 0 )
			{
				dx = -1;
			}
			
			if ( Input.GetKey(rightKey) && lockRight < 0 )
			{
				dx = 1;
			}
			
			if ( Input.GetKey(upKey) && lockUp < 0)
			{
				dy = 1;
			}
			
			if ( Input.GetKey(downKey) && lockDown < 0 )
			{	
				dy = -1;
			}
			
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
			
			if ( inmuneTimer <= 0 )
				animator.renderer.enabled = true;
		}
		
		
		accel += speed * new Vector3( dx, 0, dy );
		
		if ( torchLight )
		{
			if ( inDarkness )
			{
				torchRatio -= (Time.deltaTime * 100f) / 15f; // / secs
				torchRatio = Mathf.Clamp ( torchRatio, 0, 100 );
			}
			
			if ( torchRatio <= 0 )
				OnHit ( null );
		
			torchLight.intensity = (torchRatio / 100f) * 0.66f;
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
			cooldown = 0.1f;
		}
		else 
		{
			cooldown -= Time.deltaTime;
		}
		

		if ( Input.GetKey(leftKey)  )
		{
			facing = "Left";
			direction = Vector3.left;
		}
		else if ( Input.GetKey(rightKey)  )
		{
			facing = "Right";
			direction = Vector3.right;
		}
		else if ( Input.GetKey(upKey)  )
		{
			facing = "Up";
			direction = Vector3.forward;
		}
		else if ( Input.GetKey(downKey)  )
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
		
		
		if ( Input.GetKeyDown( attackKey )  )
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
			
			velocity = Vector3.zero;
			gravity = Vector3.zero;
			transform.position = lastSafeFloor.transform.position + new Vector3(0, .4f, 0);
			OnHit ( null );
		}
		
		lockLeft--;
		lockRight--;
		lockDown--;
		lockUp--;
	}
	

	
	void die()
	{
		hearts = GameDirector.i.maxHearts;
		inmuneTimer = 0;
		transform.position = worldOwner.startingPoint.position;
		velocity = Vector3.zero;
		gravity = Vector3.zero;
		torchRatio = 100;
		worldOwner.BroadcastMessage( "OnPlayerDead", SendMessageOptions.DontRequireReceiver );
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
