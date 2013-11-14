using UnityEngine;
using System.Collections;

public class FallingFloor : MonoBehaviour 
{
	enum State
	{
		IDLE,
		PLAYER_ON_TOP,
		CRUMBLING,
		FALLING
	};
	
	State state;
	
	const float TIME_TO_CRUMB = 0.25f;
	const float TIME_TO_FALL = 0.5f;
	const float CRUMB_AMOUNT = 0.1f;
	
	Player playerOnTop;
	float playerOnTopTimer = TIME_TO_CRUMB;
	float crumblingTimer = TIME_TO_FALL;
	Vector3 gravity;
	
	
	void Update () 
	{
		float frameRatio = Mathf.Clamp01(Time.deltaTime / 0.016f);
		
		switch ( state )
		{
			case FallingFloor.State.IDLE:
				playerOnTopTimer = TIME_TO_CRUMB;
			
				if ( GameDirector.i.playerLeft && GameDirector.i.playerLeft.currentFloor == collider )
				{
					state = FallingFloor.State.PLAYER_ON_TOP;
					playerOnTop = GameDirector.i.playerLeft;
				}
		
				if ( GameDirector.i.playerRight && GameDirector.i.playerRight.currentFloor == collider )
				{
					state = FallingFloor.State.PLAYER_ON_TOP;
					playerOnTop = GameDirector.i.playerRight;
				}
			
				break;
			case FallingFloor.State.PLAYER_ON_TOP:
				playerOnTopTimer -= Time.deltaTime;

				if ( playerOnTopTimer < 0 )
				{
					state = FallingFloor.State.CRUMBLING;
					iTween.ShakePosition( gameObject, iTween.Hash( "y", CRUMB_AMOUNT, "time", TIME_TO_FALL ) );
				}
				else
				if ( playerOnTop && playerOnTop.currentFloor != collider )
				{
					state = FallingFloor.State.IDLE;
					playerOnTop = null;
				}
		
				break;
			
			case FallingFloor.State.CRUMBLING:
				crumblingTimer -= Time.deltaTime;

				if ( crumblingTimer < 0 )
				{
					state = FallingFloor.State.FALLING;
				}
			
				break;
			case FallingFloor.State.FALLING:
				gravity += ( Vector3.up * 0.04f );
				gravity *= 0.8f;
				
				transform.position -=  gravity * frameRatio;
				break;
		}
		

	}

	bool triggered = false;
	
	void OnEnterTrigger( Collider other )
	{
		//if ( other.GetComponent<Player>() != null )
			triggered = true;
	}
}
