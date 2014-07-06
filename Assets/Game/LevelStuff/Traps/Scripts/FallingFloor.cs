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
	const float CRUMB_AMOUNT = 0.09f;
	
	BaseObject objectOnTop;
	float playerOnTopTimer = TIME_TO_CRUMB;
	float crumblingTimer = TIME_TO_FALL;
	Vector3 gravity;
	
	Vector3 startPosition;
	Vector3 startScale;
	float resetTimer;

	void Start ()
	{
		startPosition = transform.position;
		startScale = transform.localScale;
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
					resetTimer = 5.0f;
				}
			
				break;
			case FallingFloor.State.FALLING:
				gravity += ( Vector3.up * 0.04f );
				gravity *= 0.8f;
				
				transform.position -=  gravity * frameRatio;
				resetTimer -= Time.deltaTime;
			
				if ( resetTimer <= 0 )
				{
					ResetState();
				}
				break;
		}
	}

	bool triggered = false;
	
	void OnEnterTrigger( Collider other )
	{
		//if ( other.GetComponent<Player>() != null )
			triggered = true;
	}
	
	public void ResetState()
	{
		transform.position = startPosition;
		transform.localScale = startScale;
		iTween.ScaleFrom( gameObject, iTween.Hash ( "scale", Vector3.zero, "easetype", iTween.EaseType.easeOutBack, "time", 0.33f, "delay", Random.Range (0, 0.5f) ) );
		
		state = FallingFloor.State.IDLE;
		objectOnTop = null;		
	}
	
	public void OnPlayerDead()
	{
		ResetState ();
	}
}
