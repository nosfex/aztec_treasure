using UnityEngine;
using System.Collections;

public class SkellyAI : EnemyController 
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
		
		if ( distance <= 1.0f && body.currentFloor != null )
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
		if ( body.currentFloor == null )
			return;
		
		walkTimer += Time.deltaTime;

		if ( walkTimer > .66f )
		{
			walkTimer = 0;
			upDownWalkPriority = !upDownWalkPriority;
			
			if ( playerTarget.isImmune )
				ChangeDirectionRandom();
		}
		
		bool stuckRight = goingRight && body.CantGoRight;
		bool stuckLeft = goingLeft && body.CantGoLeft;
		bool stuckDown = goingDown && body.CantGoDown;
		bool stuckUp = goingUp && body.CantGoUp;
		
		bool stuck = stuckRight || stuckLeft || stuckUp || stuckDown;
		
		if ( stuck )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			upDownWalkPriority = !upDownWalkPriority;
			ChangeDirectionRotate();
		}
		else if ( !playerTarget.isImmune )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			ChangeDirectionTowardsPlayer();
			TryToAttack();
		}
	}
}
