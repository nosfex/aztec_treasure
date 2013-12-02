using UnityEngine;
using System.Collections;

public class RangedAI : EnemyController 
{
	EnemyRanged body;
	
	float walkTimer;
	
	
	void Start()
	{
		body = GetComponent<EnemyRanged>();
	}
		
	void TryToAttack()
	{
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		float distance = Vector3.Distance( playerPos, myPos );
		
		if ( distance <= 2.5f && body.currentFloor != null )
		{
			float thresholdNear = 0.2f;
			bool nearX = ( Mathf.Abs( playerPos.x - myPos.x ) < thresholdNear );// && ( goingUp || goingDown );
			bool nearY = ( Mathf.Abs( playerPos.z - myPos.z ) < thresholdNear );// && ( goingRight || goingLeft );
			
			if ( nearX || nearY )
			{
				if ( !attacking )
				{
					if ( body.cooldown <= 0 )
					{
						if ( nearX )
						{
							if ( playerPos.z > myPos.z )
								goingUp = true;
							else if ( playerPos.z < myPos.z )
								goingDown = true;
						}
						else if ( nearY )
						{
							if ( playerPos.x > myPos.x )
								goingRight = true;
							else if ( playerPos.x < myPos.x )
								goingLeft = true;
						}
					}

					attacking = true;
				}
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
		
		

		
		bool stuckRight = goingRight && body.canGoRight;
		bool stuckLeft = goingLeft && body.canGoLeft;
		bool stuckDown = goingDown && body.canGoDown;
		bool stuckUp = goingUp && body.canGoUp;
		
		bool stuck = stuckRight || stuckLeft || stuckUp || stuckDown;
		
		if ( stuck )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			upDownWalkPriority = !upDownWalkPriority;
			ChangeDirectionRotate();
		}
		else if ( !playerTarget.isImmune && body.cooldown <= 0 )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			TryToAttack();
			
			if ( !attacking )
				ChangeDirectionTowardsPlayer();
		}
		
		walkTimer += Time.deltaTime;

		if ( walkTimer > .66f )
		{
			walkTimer = 0;
			upDownWalkPriority = !upDownWalkPriority;
			
			if ( playerTarget.isImmune || body.cooldown > 0 )
			{
				goingRight = goingLeft = goingUp = goingDown = attacking = false;				
				ChangeDirectionAwayFromPlayer();
			}
		}		
	}
}
