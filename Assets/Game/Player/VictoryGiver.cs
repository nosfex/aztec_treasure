using UnityEngine;
using System.Collections;

public class VictoryGiver : MonoBehaviour 
{

	public Camera winnerCamera;
	public Camera loserCamera;
	
	Transform guiContainer;
	
	public enum State
	{
		IDLE,
		CAMERA_ANIM,
		SHOW_STUFF
	}
	
	State state = State.IDLE;
	
	void Start () 
	{
		guiContainer = GameDirector.i.guiContainer;
	}
	
	public void StartAnimation()
	{
		state = State.CAMERA_ANIM;
	}
	
	void Update () 
	{
		switch ( state )
		{
			case State.IDLE:
//				winnerCamera = GameDirector.i.worldRight.camera.camera;
//				loserCamera = GameDirector.i.worldLeft.camera.camera;
			
				if ( winnerCamera != null && loserCamera != null )
				{
					state = State.CAMERA_ANIM;
					winnerCamera.depth = loserCamera.depth + 1;
				}
			
				break;
			
			case State.CAMERA_ANIM:
			
				guiContainer.localPosition += (new Vector3( guiContainer.localPosition.x, guiContainer.localPosition.y, 5 ) - guiContainer.localPosition) * 0.1f;
			
				Rect rect = winnerCamera.rect;
				
				rect.x += -rect.x * 0.2f;
				rect.y += -rect.y * 0.2f;
				
				rect.width += (1.0f - rect.width) * 0.2f;
				rect.height += (1.0f - rect.height) * 0.2f;
				
				winnerCamera.rect = rect;
			
				if ( IsCloseEnough( rect, new Rect( 0, 0, 1, 1 ) ) )
				{
					state = State.SHOW_STUFF;
				}
			
				break;
			case State.SHOW_STUFF:
			
			
				break;
			
		}
	}
			
	bool IsCloseEnough( Rect r1, Rect r2 )
	{
		return Mathf.Abs ( r1.x - r2.x ) < 0.0001f &&
			Mathf.Abs ( r1.y - r2.y ) < 0.0001f &&
			Mathf.Abs ( r1.width - r2.width ) < 0.0001f &&
			Mathf.Abs ( r1.height - r2.height ) < 0.0001f;
	}
}
