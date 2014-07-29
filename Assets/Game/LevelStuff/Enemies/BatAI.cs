using UnityEngine;
using System.Collections;

public class BatAI : EnemyController 
{
	Bat body;
	
	float walkTimer;
	
	int state = 0;
	
	void Start()
	{
		body = GetComponent<Bat>();
		walkTimer = Random.Range (0, 3.0f);
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
	
	Vector3 offset;
	
	override public void UpdateAI()
	{
		if ( playerTarget == null )
			return;

		walkTimer += Time.deltaTime;
		
		bool stuckRight = goingRight && body.stuckRight;
		bool stuckLeft = goingLeft && body.stuckLeft;
		bool stuckDown = goingDown && body.stuckBack;
		bool stuckUp = goingUp && body.stuckForward;
		
		bool stuck = stuckRight || stuckLeft || stuckUp || stuckDown;
		
		if ( stuck )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			ChangeDirectionRandom();
			state = 3;
		}
		
		if ( walkTimer > 1.0f )
		{
			if ( CheckIfPlayerInSight() && !playerTarget.isImmune )
			{
				// Set attack state
				offset = new Vector3( Random.insideUnitCircle.x * 2, 0, Random.insideUnitCircle.y * 2 );
				state = 1;
			}
			else 
			{
				
				state = 2;
				offset = new Vector3( Random.insideUnitCircle.x * 2, 0, Random.insideUnitCircle.y * 2 );
				
			}
			
			walkTimer = 0;
		}
		
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		float distance = Vector3.Distance( playerPos, myPos );
		
		bool isFar = distance > 2.1f;
		
		if ( playerTarget.isImmune && state != 3 && !isFar )
		{
			// "Close to player, but he is immune"
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			ChangeDirectionRandom();	
			state = 3;
		}
		else 
		{
			if ( state == 1 )
			{
				// Attacking state 
				if ( !CheckIfPlayerInSight() || isFar )
				{
					offset = new Vector3( Random.insideUnitCircle.x * 2, 0, Random.insideUnitCircle.y * 2 );
					state = 2;
				}
				
				if ( Time.frameCount % 10 == 0 )
				{
					goingRight = goingLeft = goingUp = goingDown = attacking = false;
					ChangeDirectionTowardsPlayerNoXYLock( 0.1f );
				}
			}
			else if ( state == 2 )
			{
				// Trying to attack but blocked state
				goingRight = goingLeft = goingUp = goingDown = attacking = false;
				ChangeDirectionTowardsPlayerOffset( offset );
			}
		}
	}
}
