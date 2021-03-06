﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteAnimator : MonoBehaviour 
{
	public int xcells = 1;
	public int ycells = 1; 
	
	public int frameIndex = 0;
	
	public bool isBillboard = true;
	public Camera billboardCamera;
	public bool playOnAwake = false;
	public string startingAnimationName;
	
	public enum PlayMode
	{
		DEFAULT,
		LOOP,
		PINGPONG,
		RANDOM,
		CUSTOM
	};
	
	public enum PlayHeadState
	{
		STOP,
		PLAY
	};
	
	[System.Serializable]
	public class SpriteAnimation
	{
		public string name;
		public PlayMode playMode = PlayMode.DEFAULT;
		public int startFrame = 0;
		public int endFrame = 1;
		public float frameRate = 30;
		public int[] customFrameSequence;
		public bool horizontalFlip;
		
		SpriteAnimation()
		{
			horizontalFlip = false;
			playMode = PlayMode.DEFAULT;
			startFrame = 0;
			endFrame = 1;
			frameRate = 30;
		}
	};
	
	
	private int[] frameSequence; // -1 is loop.
	
	public SpriteAnimation[] animations;
	
	//[HideInInspector]
	public SpriteAnimation currentAnimation;
	
	PlayHeadState headState;
	
	public bool isPlaying
	{
		get { return headState == PlayHeadState.PLAY; }
	}
	
	public bool isAnimPlaying( string animName ) 
	{
		return currentAnimation.name.Contains( animName ) && headState == PlayHeadState.PLAY;
	}

	string nextAnim = "";

	public void AppendAnim( string animName )
	{
		nextAnim = animName;
	}

	public void PlayAnim( string animName )
	{
		if ( animations.Length == 0 )
			return;
		
		foreach ( SpriteAnimation anim in animations )
		{
			if ( anim.name == animName )
			{
				currentAnimation = anim;
				headState = PlayHeadState.PLAY;
				
				int frameCount = anim.endFrame - anim.startFrame + 1;
				
				switch ( anim.playMode )
				{
					case PlayMode.DEFAULT:
						frameSequence = new int[ frameCount ];
						for ( int i = 0; i < frameSequence.Length; i++ )
							frameSequence[ i ] = anim.startFrame + i;
		
						break;
					case PlayMode.LOOP:
						frameSequence = new int[(frameCount) + 1];
						
						for ( int i = 0; i < frameSequence.Length - 1; i++ )
						{
							frameSequence[ i ] = anim.startFrame + i;
						}
					
						frameSequence[ frameSequence.Length - 1 ] = -1; // loop order
					
						break;
					case PlayMode.PINGPONG:
						frameSequence = new int[((frameCount) * 2) - 2 + 1];
					
						for ( int i = 0; i < frameSequence.Length  - 1; i++ )
							frameSequence[ i ] = (int)Mathf.PingPong( anim.startFrame + i, (frameCount) - 1 );
					
						frameSequence[ frameSequence.Length - 1 ] = -1; // loop order
						break;
					case PlayMode.RANDOM:
						frameSequence = new int[frameCount];
		
						for ( int i = 0; i < frameSequence.Length; i++ )
							frameSequence[ i ] = anim.startFrame + i;
		
						for ( int i = 0; i < frameSequence.Length - 1; i++ )
						{	
							int swap = frameSequence[ i ];
							int randIndex = Random.Range( i + 1, frameSequence.Length - 1 );
							frameSequence[ i ] = frameSequence[ randIndex ];
							frameSequence[ randIndex ] = swap;
						}
					
						break;
					case PlayMode.CUSTOM:
						frameSequence = new int[ anim.customFrameSequence.Length ];
		
						for ( int i = 0; i < frameSequence.Length; i++ )
							frameSequence[ i ] = anim.customFrameSequence[ i ];
					
						break;
					
				}
				
				return;
			}
		}
	}
	
	public void PauseAnim()
	{
		headState = PlayHeadState.STOP;
	}

	public void StopAnim()
	{
		headState = PlayHeadState.STOP;
		timer = 0;
		frameIndex = 0;
	}
	
	void FindBillboardCamera()
	{
		
		if ( transform.root == transform )
		{
			billboardCamera = Camera.main;
			
		}
		else 
		{
			//Detect if i'm a BaseObject, or child or grand-child of BaseObject
			BaseObject myObject = gameObject.GetComponent<BaseObject>();
			
			if ( myObject == null )
				myObject = transform.parent.GetComponent<BaseObject>();

			if ( transform.parent.root != transform.parent && myObject == null )
				myObject = transform.parent.parent.GetComponent<BaseObject>();
			
			if ( myObject != null && myObject.worldOwner != null && myObject.worldOwner.camera != null )
			{
				billboardCamera = myObject.worldOwner.camera.camera;
				
			}
			else
			{
				//Detection failed...
/*				//So, now try to detect if child or grand-child of world.
				
				World myWorld = transform.parent.GetComponent<World>();
				
				if ( transform.parent.root != transform.parent && myWorld == null )
					myWorld = transform.parent.parent.GetComponent<World>();
				else if ( transform.parent.parent.root != transform.parent.parent && myWorld == null )
					myWorld = transform.parent.parent.parent.GetComponent<World>();
								 */
				World myWorld = findWorld( transform );
				
				if ( myWorld != null )
				{
					billboardCamera = myWorld.camera.camera;
					
					
				}
				else 
				{
					
					billboardCamera = Camera.main;
				}
				
			}
		}
				
	}
	
	World findWorld( Transform t ) 
	{
		World w = t.GetComponent<World>();
		if ( w )
			return w;
		else 
		{
			if ( t.root == t )
				return null;
			else 
				return findWorld ( t.parent );
		}
	}
	
	void Start () 
	{
		if ( Application.isEditor && !Application.isPlaying )
			return;
		
		Invoke ( "FindBillboardCamera", 0.3f );
		//FindBillboardCamera();
		
		timer = 0;
		frameIndex = 0;
		
		currentAnimation = null;
		
		if ( animations.Length == 0 )
			return;
		
		if ( startingAnimationName != "" )
		{
			PlayAnim( startingAnimationName );
	
			if ( !playOnAwake )
				StopAnim();
		}
	}
	
	void EditorUpdate()
	{
		float xScale = 1.0f / xcells;
		float yScale = 1.0f / ycells;
		renderer.sharedMaterial.mainTextureScale = new Vector2( xScale, yScale );
		renderer.sharedMaterial.mainTextureOffset = new Vector2( xScale * (frameIndex % xcells), (1.0f - yScale) - (yScale * (frameIndex / xcells)) );
		
//		frameIndex++;
		//frameIndex %= xcells * ycells;
	}
	
	float timer = 0;

	public float GetPlayTime()
	{
		return timer;
	}
	
	public void GoToFrame( int frame )
	{
		float frameTime = 1f / currentAnimation.frameRate;
		timer = Mathf.Clamp( timer, 0, frameTime * frame );
		frameIndex = (int)(timer / frameTime);
	}
	
	public void GoToSequenceFrame( int frame )
	{
		float frameTime = 1f / currentAnimation.frameRate;
		timer = Mathf.Clamp( timer, 0, frameTime * frameSequence[ frame ] );
		frameIndex = (int)(timer / frameTime);
	}	
	
	void Update () 
	{
		
		if ( Application.isEditor && !Application.isPlaying )
		{
			EditorUpdate();
			return;
		}
		
		if ( isBillboard )
		{
			if ( billboardCamera  )
			{
				transform.rotation = Quaternion.LookRotation( transform.position - billboardCamera.transform.position );
				transform.rotation = Quaternion.Euler ( transform.rotation.eulerAngles.x, 0, 0 );
			}
			else 
				FindBillboardCamera();
		
		}

		//print ("animatinos.Lenght = " + animations.Length );
		if ( currentAnimation == null || animations.Length == 0 )
			return;

		float xScale = 1.0f / xcells;
		float yScale = 1.0f / ycells;
		
		// Prevent out of bounds.
		//frameIndex = Mathf.Min ( frameList.Length - 1, frameIndex );
		
		if ( headState == PlayHeadState.PLAY )
			timer += Time.deltaTime;
		
		float frameTime = 1f / currentAnimation.frameRate;
		
		timer = Mathf.Clamp( timer, 0, frameTime * (frameSequence.Length) );
		
		if ( timer >= (frameTime * (frameSequence.Length)) - 0.01f )
		{
			if ( nextAnim != "" )
			{
				StopAnim();
				PlayAnim ( nextAnim );
				nextAnim = "";
			}
			else 
			{
				headState = PlayHeadState.STOP;
			}	
		}

		frameIndex = (int)(timer / frameTime);
		
		frameIndex = Mathf.Clamp ( frameIndex, 0, (frameSequence.Length - 1) );

		
		
		//print ( " frame = " + frameIndex + " ... " + currentAnimation.frameRate );
		if ( frameSequence[ frameIndex ] == -1 ) // Loop!
		{
			frameIndex = 0;
			timer = 0;
		}

		

		for ( int i = 0; i < renderer.materials.Length; i++ )
		{
			Material m = renderer.materials[i];
			m.mainTextureScale = new Vector2( xScale * ( currentAnimation.horizontalFlip ? -1 : 1 ), yScale );
			m.mainTextureOffset = new Vector2( xScale * ((frameSequence[frameIndex] % xcells) - ( currentAnimation.horizontalFlip ? -1 : 0 )), (1.0f - yScale) - (yScale * (frameSequence[frameIndex] / xcells)) );
		}
		

	}
}
