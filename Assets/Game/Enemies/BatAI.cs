using UnityEngine;
using System.Collections;

public class BatAI : EnemyController 
{
	Bat body;
	
	float walkTimer;
	
	void Start()
	{
		body = GetComponent<Bat>();
	}
		
	void TryToAttack()
	{
//		Vector3 playerPos = playerTarget.transform.position;
//		Vector3 myPos = transform.position;
//		float distance = Vector3.Distance( playerPos, myPos );
//		
//		if ( distance <= 1.0f && body.currentFloor != null )
//		{
//			float thresholdNear = 0.4f;
//			bool nearX = ( Mathf.Abs( playerPos.x - myPos.x ) < thresholdNear ) && ( goingUp || goingDown );
//			bool nearY = ( Mathf.Abs( playerPos.z - myPos.z ) < thresholdNear ) && ( goingRight || goingLeft );
//			
//			if ( nearX || nearY )
//			{
//				attacking = true;
//			}
//		}
	}
	
	override public void Init()
	{
		ChangeDirectionTowardsPlayer();
	}
	
	override public void UpdateAI()
	{
//		if ( body.currentFloor == null )
//			return;
		
		walkTimer += Time.deltaTime;
//
//		if ( walkTimer > .66f )
//		{
//			walkTimer = 0;
//			
//			if ( playerTarget.isImmune )
//				ChangeDirectionRandom();
//		}
		
		bool stuckRight = goingRight && body.stuckRight;
		bool stuckLeft = goingLeft && body.stuckLeft;
		bool stuckDown = goingDown && body.stuckBack;
		bool stuckUp = goingUp && body.stuckForward;
		
		bool stuck = stuckRight || stuckLeft || stuckUp || stuckDown;
		
		if ( stuck )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			ChangeDirectionTowardsPlayerNoXYLock( 0.1f );
			walkTimer = 0;
		}
		else if ( !playerTarget.isImmune )
		{
			Vector3 playerPos = playerTarget.transform.position;
			Vector3 myPos = transform.position;
			float distance = Vector3.Distance( playerPos, myPos );
			
			if ( distance > 2.5f )
			{
				goingRight = goingLeft = goingUp = goingDown = attacking = false;
				ChangeDirectionTowardsPlayerNoXYLock( 0.1f );
				walkTimer = 0;
			}
			else if ( distance <= 2.5f ) 
			{
//				
//				if ( distance < 1.0f )
//				{
//					attacking = true;
//				}
				
				if ( walkTimer < 1.0f )
				{
					goingRight = goingLeft = goingUp = goingDown = attacking = false;
					ChangeDirectionTowardsPlayerNoXYLock( 0.1f );
				}
			}
				//TryToAttack();
		}
	}
}
