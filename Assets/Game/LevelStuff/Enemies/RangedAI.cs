using UnityEngine;
using System.Collections;

public class RangedAI : EnemyController 
{
	EnemyRanged body;
	
	float walkTimer;
	float drawTimer;
	
	bool drawAndShoot = false;
	bool nearX = false, nearY = false;
	void Start()
	{
		body = GetComponent<EnemyRanged>();
	}
		
	void TryToAttack()
	{
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		float distance = Vector3.Distance( playerPos, myPos );
		
		if ( distance <= 5f && body.isGrounded && !drawAndShoot )
		{
			float thresholdNear = 0.2f;
			nearX = ( Mathf.Abs( playerPos.x - myPos.x ) < thresholdNear );// && ( goingUp || goingDown );
			nearY = ( Mathf.Abs( playerPos.z - myPos.z ) < thresholdNear );// && ( goingRight || goingLeft );
			
			if ( nearX || nearY )
			{
				if ( !attacking && body.cooldown <= 0 )
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
					drawAndShoot = true;
				}
			}
		}
		
		if ( drawAndShoot )
		{
			drawTimer += Time.deltaTime;
			if ( drawTimer > 1.0f )
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
				drawTimer = 0;
				attacking = true;
				drawAndShoot = false;
				body.animator.renderer.material.SetColor( "_AddColor", new Color(0,0,0,1) );
			}
			else 
			{
				goingRight = goingLeft = goingUp = goingDown = false;
				
				if ( Time.frameCount % 4 < 2 )
					body.animator.renderer.material.SetColor( "_AddColor", new Color(0.5f,0.5f,0.5f,1) );
				else
					body.animator.renderer.material.SetColor( "_AddColor", new Color(0,0,0,1) );
			}
		}
	}
	
	override public void Init()
	{
		ChangeDirectionTowardsPlayer();
	}
	
	override public void UpdateAI()
	{
		if ( !body.isGrounded )
			return;

		walkTimer += Time.deltaTime;

		if ( walkTimer > 1.0f && !drawAndShoot )
		{
			walkTimer = 0;
			upDownWalkPriority = !upDownWalkPriority;
			
			if ( playerTarget.isImmune || body.cooldown > 0 )
			{
				goingRight = goingLeft = goingUp = goingDown = attacking = false;				
				ChangeDirectionAwayFromPlayer();
			}
		}
		
		if ( body.stuck )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			upDownWalkPriority = !upDownWalkPriority;
			ChangeDirectionRotate();
		}
		else if ( !playerTarget.isImmune && body.cooldown <= 0 )
		{
			goingRight = goingLeft = goingUp = goingDown = attacking = false;
			TryToAttack();
			
			if ( !attacking && !drawAndShoot )
				ChangeDirectionTowardsPlayer();
		}
		
	}
}
