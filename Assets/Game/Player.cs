﻿using UnityEngine;
using System.Collections;

public class Player : BaseObject 
{
	SpriteAnimator animator;
	
	public Transform helperPivot;
	public BaseObjectSensor liftSensor;
	public GameObject dropGuide;
	
	
	override protected void Start () 
	{
		animator = GetComponent<SpriteAnimator>();
		base.Start();
		direction = Vector3.right;
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
	
	bool skidEnabled = false;
	bool isSkidding = false;
	float isFlipping = 0;
	
	BaseObject liftedObject;
	Vector3 direction;
	public float speed = 0.5f;
	
	void Update () 
	{
		float dx = 0, dy = 0;
		
		
		if ( !animator.isAnimPlaying("Attack") )
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
			if ( liftedObject == null )
			{
				if ( liftSensor.sensedObject != null )
				{
					//print ("LIFT!");
					Transform lifted = liftSensor.sensedObject.transform;
					lifted.parent = transform;
					lifted.position = transform.position + new Vector3( 0, 0.4f, 0 );
					liftedObject = lifted.gameObject.GetComponent<BaseObject>();
					liftedObject.gravityEnabled = false;
					liftedObject.collisionEnabled = false;
					
					liftSensor.gameObject.SetActive( false );
					
					//dropGuide.SetActive( true );
				}
				else if ( cooldown < 0 )
				{
					animator.StopAnim();
					animator.PlayAnim("Attack" + facing );
					velocity *= 2;
				}
			}
			else
			{
				
				liftedObject.velocity += (direction * 0.1f) + (velocity * 1.5f); 
				liftedObject.transform.parent = worldOwner.transform;
				liftedObject.gravityEnabled = true;
				
				liftedObject = null;
				liftSensor.gameObject.SetActive( true );
			}
			
		}
		
		lockLeft--;
		lockRight--;
		lockDown--;
		lockUp--;
	}
	
	void OnTriggerEnter( Collider other )
	{
		//print ( "caca = " + velocity.magnitude );
		OnTrigger ( other ); 
	}

	void OnTriggerStay( Collider other ){	OnTrigger ( other ); }
	
	void OnTrigger( Collider other )
	{
		
		BaseObject bo = other.GetComponent<BaseObject>();

		if ( bo != null )
		{
			if ( !bo.collisionEnabled )
			{
				return;
			}
		}
		
		float margin = ((SphereCollider)collider).radius - 0.01f;
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
			animator.StopAnim();
			velocity *= -2.0f;
		}
		else 
		if ( straightTimer > 0.7f )
			velocity *= -2.0f;
		
		straightTimer = 0;
		
		

		transform.position = new Vector3( adjustX ? closestBoundExit.x : transform.position.x, 
										  transform.position.y, 
										  adjustZ ? closestBoundExit.z : transform.position.z );
	}
}
