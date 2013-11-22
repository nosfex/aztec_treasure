using UnityEngine;
using System.Collections;

public class CameraLock : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public FollowSmooth cameraController;
	
	public enum LockType
	{
		LOCK_LEFT,
		LOCK_RIGHT,
		LOCK_UP,
		LOCK_DOWN
	};
	
	public LockType type;
	
	void OnTriggerEnter( Collider other )
	{
		if ( !other.tag.Contains( "Wall" ) )
			return;
		
		//UnityEditor.Selection.activeGameObject = other.gameObject;
		
		switch ( type )
		{
		case LockType.LOCK_LEFT:
			cameraController.OnTriggerEnterLeft( collider, other );
			break;
		case LockType.LOCK_RIGHT:
			cameraController.OnTriggerEnterRight( collider, other );
			break;
		case LockType.LOCK_UP:
			cameraController.OnTriggerEnterUp( collider, other );
			break;
		case LockType.LOCK_DOWN:
			cameraController.OnTriggerEnterDown( collider, other );
			break;
		}
	}
	
	void OnTriggerStay( Collider other )
	{
		if ( !other.tag.Contains( "Wall" ) )
			return;
		
		//UnityEditor.Selection.activeGameObject = other.gameObject;
		
		switch ( type )
		{
		case LockType.LOCK_LEFT:
			cameraController.OnTriggerEnterLeft( collider, other );
			break;
		case LockType.LOCK_RIGHT:
			cameraController.OnTriggerEnterRight( collider, other );
			break;
		case LockType.LOCK_UP:
			cameraController.OnTriggerEnterUp( collider, other );
			break;
		case LockType.LOCK_DOWN:
			cameraController.OnTriggerEnterDown( collider, other );
			break;
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( !other.tag.Contains( "Wall" ) )
			return;
		
		//UnityEditor.Selection.activeGameObject = other.gameObject;

		switch ( type )
		{
		case LockType.LOCK_LEFT:
			cameraController.OnTriggerExitLeft( collider, other );
			break;
		case LockType.LOCK_RIGHT:
			cameraController.OnTriggerExitRight( collider, other );
			break;
		case LockType.LOCK_UP:
			cameraController.OnTriggerExitUp( collider, other );
			break;
		case LockType.LOCK_DOWN:
			cameraController.OnTriggerExitDown( collider, other );
			break;
		}
	}

}
