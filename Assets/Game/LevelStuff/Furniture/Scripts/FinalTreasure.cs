using UnityEngine;
using System.Collections;

public class FinalTreasure : BaseObject 
{
	void Update()
	{
		if ( gameObject.layer == LayerMask.NameToLayer( "Past" ) )
		{
			isLiftable = false;
			
		}
		else 
		{
			GameDirector.i.finalTreasureRight = gameObject;
		}
		
		if ( transform.position.y < worldOwner.deathYLimit.position.y )
		{
			velocity = Vector3.zero;
			gravity = Vector3.zero;
			transform.position = lastSafeFloor.transform.position + new Vector3(0, .4f, 0);
		}

	}
	
	void OnLifted( GameObject src )
	{
		GUIScreenFeedback.i.ShowGetToTheEntrance();
		GameDirector.i.worldRight.camera.earthquakeEnabled = true;
		GameDirector.i.sfxEarthquake.Play ();
	}
	
	Collider lastSafeFloor;
	
	protected override void TestFloor( Collider other )
	{
		base.TestFloor( other );
		
		if ( currentFloor != null && currentFloor.tag == "Floor" && currentFloor.name != "FloorFallingFuture" )
		{
			lastSafeFloor = currentFloor;
		}
	}
	
	
}
