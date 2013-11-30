using UnityEngine;
using System.Collections;

public class Bat : Skelly 
{
	override protected void Start()
	{
		base.Start ();
	}

	override protected void Update()
	{
		base.Update();
		
		if ( currentFloor && state != Skelly.State.DYING )
		{
			gravityEnabled = false;
		}
	}
	
	override protected void UpdateDying()
	{
		if ( stateTimer > 1.0f )
			animator.renderer.enabled = !animator.renderer.enabled;
		
		if ( stateTimer > 2.0f )
		{
			worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );
			Destroy( gameObject );
		}
	}
	
	override protected void TestFloor( Collider other )
	{
		currentFloor = (BoxCollider)other;
	}
	
	override protected void OnTriggerEnter( Collider other )
	{
		base.OnTriggerEnter( other );
		
		if ( other.GetComponent<Player>() != null && state != Skelly.State.DYING )
		{
			Player p = other.GetComponent<Player>();
			animator.StopAnim();
			animator.PlayAnim("Attack" + facing );
			//cooldown = attackCooldown;
			
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
		animator.transform.position -= Vector3.up * 0.4f;
		transform.position += Vector3.up * 0.4f;
		gravity.y = -0.05f;
		velocity *= -1.2f;
		
		gravityEnabled = true;
		collisionEnabled = false;
		state = State.DYING;
		animator.PlayAnim( "Death" );
	}
}
