﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	[HideInInspector]
	public Player playerTarget;

	virtual public void Init() {}
	virtual public void UpdateAI() {}
	
	
	public bool GetKey( KeyCode key )
	{
		switch ( key )
		{
			case KeyCode.DownArrow:
				return goingDown;
			case KeyCode.UpArrow:
				return goingUp;
			case KeyCode.LeftArrow:
				return goingLeft;
			case KeyCode.RightArrow:
				return goingRight;
			case KeyCode.Keypad0:
				return attacking;
		}
		
		return false;
	}
	
	protected bool goingLeft;
	protected bool goingRight;
	protected bool goingUp;
	protected bool goingDown;
	protected bool attacking;
	
	
	public void ClearActions()
	{
		goingUp = goingLeft = goingDown = goingUp = attacking = false;
	}

	
	public void ChangeDirectionRandom()
	{
		ClearActions ();
		int dir = Random.Range( 0, 4 );
		
		switch ( dir )
		{
			case 0:
				goingRight = true;
				break;
			case 1:
				goingLeft = true;
				break;
			case 2:
				goingUp = true;
				break;
			case 3:
				goingDown = true;
				break;
		}		
		
		//print ("change! " + dir );
	}
	public void ChangeDirectionRotate()
	{
		if ( goingRight ) { ClearActions(); goingDown = true; }
		else if ( goingDown ) { ClearActions(); goingLeft = true;  }
 		else if ( goingLeft ) { ClearActions(); goingUp = true; }
		else if ( goingUp ) { ClearActions(); goingRight = true; }
	}	
	
	public void ChangeDirectionTurnback()
	{
		if ( goingRight ) { ClearActions(); goingLeft = true; }
		else if ( goingDown ) { ClearActions(); goingUp = true;  }
 		else if ( goingLeft ) { ClearActions(); goingRight = true; }
		else if ( goingUp ) { ClearActions(); goingDown = true; }
	}

	protected bool upDownWalkPriority;
	
	public void ChangeDirectionTowardsPlayer()
	{
		ChangeDirectionTowardsPlayer( 0.04f );
	}
	
	public void ChangeDirectionTowardsPlayer( float thresholdNear )
	{
		//int dir = Random.Range( 0, 4 );
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		//float distance = Vector3.Distance( playerPos, myPos );

		if ( upDownWalkPriority )
		{
			if ( playerPos.z < myPos.z && Mathf.Abs(playerPos.z - myPos.z) > thresholdNear )
				goingDown = true;
			else if ( playerPos.z > myPos.z && Mathf.Abs(playerPos.z - myPos.z) > thresholdNear )
				goingUp = true;
			else if ( playerPos.x > myPos.x && Mathf.Abs(playerPos.x - myPos.x) > thresholdNear )
				goingRight = true;
			else if ( playerPos.x < myPos.x && Mathf.Abs(playerPos.x - myPos.x) > thresholdNear )
				goingLeft = true;
		}
		else 
		{
			if ( playerPos.x > myPos.x && Mathf.Abs(playerPos.x - myPos.x) > thresholdNear )
				goingRight = true;
			else if ( playerPos.x < myPos.x && Mathf.Abs(playerPos.x - myPos.x) > thresholdNear )
				goingLeft = true;
			else if ( playerPos.z < myPos.z && Mathf.Abs(playerPos.z - myPos.z) > thresholdNear )
				goingDown = true;
			else if ( playerPos.z > myPos.z && Mathf.Abs(playerPos.z - myPos.z) > thresholdNear )
				goingUp = true;
		}
	}
	
	
	public void ChangeDirectionAwayFromPlayer()
	{
		//int dir = Random.Range( 0, 4 );
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		//float distance = Vector3.Distance( playerPos, myPos );

		if ( upDownWalkPriority )
		{
			if ( playerPos.z < myPos.z )
				goingUp = true;
			else if ( playerPos.z > myPos.z )
				goingDown = true;
			else if ( playerPos.x > myPos.x )
				goingLeft = true;
			else if ( playerPos.x < myPos.x )
				goingRight = true;
		}
		else 
		{
			if ( playerPos.x > myPos.x )
				goingLeft = true;
			else if ( playerPos.x < myPos.x )
				goingRight = true;
			else if ( playerPos.z < myPos.z )
				goingUp = true;
			else if ( playerPos.z > myPos.z )
				goingDown = true;
		}
	}	
	
	
	public void ChangeDirectionTowardsPlayerNoXYLock( float thresholdNear )
	{
		//int dir = Random.Range( 0, 4 );
		Vector3 playerPos = playerTarget.transform.position;
		Vector3 myPos = transform.position;
		//float distance = Vector3.Distance( playerPos, myPos );

		if ( playerPos.z < myPos.z && Mathf.Abs(playerPos.z - myPos.z) > thresholdNear )
			goingDown = true;
		else if ( playerPos.z > myPos.z && Mathf.Abs(playerPos.z - myPos.z) > thresholdNear )
			goingUp = true;
		
		if ( playerPos.x > myPos.x && Mathf.Abs(playerPos.x - myPos.x) > thresholdNear )
			goingRight = true;
		else if ( playerPos.x < myPos.x && Mathf.Abs(playerPos.x - myPos.x) > thresholdNear )
			goingLeft = true;
	}
	
}
