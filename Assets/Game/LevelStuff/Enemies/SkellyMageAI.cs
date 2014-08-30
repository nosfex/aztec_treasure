using UnityEngine;
using System.Collections;

public class SkellyMageAI : EnemyController 
{
	Skelly body;
	
	float walkTimer;

	public float firingDistance = 5.852f;
	void Start()
	{
		body = GetComponent<Skelly>();
	}
	
	void TryToAttack()
	{
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		float distance = Vector3.Distance( playerPos, myPos );
		
	
		if(distance < firingDistance)
		{
			attacking = true;

			//print ("ALALALLAALLA");
		}
		else
		{
			//print("ATTACKING=FALSE");
			attacking = false;

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
