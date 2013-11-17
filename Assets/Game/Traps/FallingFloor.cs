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
	const float CRUMB_AMOUNT = 0.15f;
	
	BaseObject objectOnTop;
	float playerOnTopTimer = TIME_TO_CRUMB;
	float crumblingTimer = TIME_TO_FALL;
	Vector3 gravity;
	
	Vector3 startPosition;
	
	void Start ()
	{
		startPosition = transform.position;
	}
	
	void EnterObjectLaid( BaseObject other )
	{
		objectOnTop = other;
	}

	void ExitObjectLaid( BaseObject other )
	{
		if ( other == objectOnTop )
			objectOnTop = null;
	}
	
	void Update () 
	{
		float frameRatio = Mathf.Clamp01(Time.deltaTime / 0.016f);
		
		switch ( state )
		{
			case FallingFloor.State.IDLE:
				playerOnTopTimer = TIME_TO_CRUMB;
			
				if ( objectOnTop != null )
					state = FallingFloor.State.PLAYER_ON_TOP;
		
				break;
			case FallingFloor.State.PLAYER_ON_TOP:
				playerOnTopTimer -= Time.deltaTime;

				if ( playerOnTopTimer < 0 )
				{
					state = FallingFloor.State.CRUMBLING;
					iTween.ShakePosition( gameObject, iTween.Hash( "y", CRUMB_AMOUNT, "time", TIME_TO_FALL ) );
				}
				else
				if ( objectOnTop == null )
				{
					state = FallingFloor.State.IDLE;
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
	
	public void OnPlayerDead()
	{
		transform.position = startPosition;
		state = FallingFloor.State.IDLE;
		objectOnTop = null;
	}
}
