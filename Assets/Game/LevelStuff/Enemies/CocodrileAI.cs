using UnityEngine;
using System.Collections;

public class CocodrileAI : EnemyController 
{
	Skelly body;
	
	float walkTimer;

	void Start()
	{
		body = GetComponent<Skelly>();
	}
	
	void TryToAttack()
	{
	
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		float distance = Vector3.Distance( playerPos, myPos );
		
		if ( distance <= 2.0f && body.isGrounded )
		{
			float thresholdNear = 0.4f;
			bool nearX = ( Mathf.Abs( playerPos.x - myPos.x ) < thresholdNear ) && ( goingUp || goingDown );
			bool nearY = ( Mathf.Abs( playerPos.z - myPos.z ) < thresholdNear ) && ( goingRight || goingLeft );
			
			if ( nearX || nearY )
			{
				attacking = true;
			}
		}
	}
	
	override public void Init()
	{
		ChangeDirectionTowardsPlayer();
	}
	
	override public void UpdateAI()
	{
		//print ("BODY IS BEHAVING WEIRD");
		if ( !body.isGrounded )
		{

			return;
		}
			

		bool stuckBack = body.stuckBack && goingDown; 
		bool stuckForward = body.stuckForward && goingUp; 
		bool stuckRight = body.stuckRight && goingRight; 
		bool stuckLeft = body.stuckLeft && goingLeft; 
		
		bool stuck = stuckBack || stuckForward || stuckRight || stuckLeft;
		
		if ( stuck )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			upDownWalkPriority = !upDownWalkPriority;
			ChangeDirectionRotate();
		}
		else 
		{
			bool playerInSight = CheckIfPlayerInSight();
			
			if ( !playerInSight || playerTarget.isImmune )
			{
				walkTimer += Time.deltaTime;
				if ( walkTimer > .66f )
				{
					walkTimer = 0;
					upDownWalkPriority = !upDownWalkPriority;
					ChangeDirectionRandom();
				}
			}
			else 
				if ( playerInSight )
			{
				goingRight = goingLeft = goingUp = goingDown = attacking = false;
				ChangeDirectionTowardsPlayer();
				TryToAttack();
			}
		}
	}
}
