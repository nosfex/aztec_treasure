using UnityEngine;
using System.Collections;

public class Bat : Skelly 
{
	public AudioSource[] SFXDeath;

	override protected void Start()
	{
		base.Start ();
		minStairClimb = 0.4f;
	}

	override protected void Update()
	{
		Vector3 myXZ = transform.position;
		Vector3 hisXZ = transform.position + (Vector3.down * 30);
		Vector3 dir = myXZ - hisXZ;
		float dist = Vector3.Distance( myXZ, hisXZ );
		
		RaycastHit[] info = Physics.RaycastAll( myXZ, dir, dist );
		bool floorBelow = false;

		foreach ( RaycastHit i in info )
		{
			if ( i.collider.gameObject.name.Contains("Tile") )
			{
				floorBelow = true;
				break;
			}
		}
		
		gravityEnabled = true;
		
		if ( state != State.DYING )
		{
			if ( !floorBelow || isGrounded )
				gravityEnabled = false;

			animator.renderer.enabled = true;
			transform.position += new Vector3( 0, ( (GameDirector.i.playerRight.transform.position.y + 0.2f) - transform.position.y) * 0.5f, 0 );
		}



		base.Update();
	}
	
	override protected void UpdateDying()
	{
		if ( hearts > 0 )
		{
			//Vector3 animOrigY = new Vector3( animator.transform.position.x, GameDirector.i.playerRight.transform.position.y+ 0.4f, animator.transform.position.z );
			
			if ( stateTimer > .25f )
			{
				//animator.transform.position += (animOrigY - animator.transform.position) * 0.2f;
			}
			
			if ( stateTimer > .75f )
			{
				//transform.position = animOrigY + (Vector3.down * 0.4f);
				//animator.transform.position = animOrigY;
				gravityEnabled = true;
				collisionEnabled = true;	
				
				state = State.WALKING;
				
			}
			return;
		}
		
		if ( stateTimer > 1.0f )
		{
			animator.renderer.enabled = Time.frameCount % 4 < 2;
		}
		
		if ( stateTimer > 2.0f )
		{
			worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );
			Destroy( gameObject );
		}
	}
	
//	override protected void TestFloor( Collider other )
//	{
//		currentFloor = (BoxCollider)other;
//	}
	
	override protected void OnTriggerEnter( Collider other )
	{
		base.OnTriggerEnter( other );
		
//		if ( other.GetComponent<Player>() != null && state != State.DYING )
//		{
//			Player p = other.GetComponent<Player>();
//			animator.StopAnim();
//			animator.PlayAnim("Attack" + facing );
//			
////			if ( p != null && !p.isImmune )
////			{
////				p.OnHit( gameObject );
////				p.velocity += direction * speed * attackSpeedFactor * .5f;
////				velocity *= -1.2f;
////			}
//		}
	}
	
	float animatorOriginalY;
	
	public override void OnHit( GameObject other )
	{
		if ( inmuneTimer > 0 )
			return;

		if ( state == State.DYING )
			return;

		hearts--;

		if ( other.GetComponentInChildren<Pottery>() != null )
			hearts -= 2;

		hitFeedbackTimer = 0.2f;
		//inmuneTimer = 0.3f;

		Player p = other.GetComponentInChildren<Player>();
		
		if ( p != null )//&& playerKnockbackHitFactor > 0 )
		{	
			p.velocity *= -playerKnockbackHitFactor;
			p.frictionCoef = 0.999f;
			velocity = p.direction * 0.03f;
		}
		
		if ( hearts <= 0 )
		{
			gravity.y = -0.05f;
			Die();
			state = State.DYING;
			animator.PlayAnim( "Death" );
			collisionEnabled = false;
		}
		else 
		{

			animator.renderer.material.SetColor ( "_AddColor", Color.black );
			jumpAttacking = false;
			state = State.WALKING;
		}

		gravityEnabled = true;


		if ( SFXDeath.Length > 0 )
		{
			AudioSource sfxdeath = SFXDeath[ Random.Range(0, SFXDeath.Length) ];
			sfxdeath.pitch = Random.Range (0.9f, 1.1f);
			sfxdeath.Play();
		}
	}
	
	
	override protected void UpdateAttacking()
	{
		if ( stateTimer > 0.01f )
		{
			state = State.WALKING;
		}
	}	
	

	
	override protected void Die()
	{
	}
}
