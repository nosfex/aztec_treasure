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
	}
	
	void OnLifted( GameObject src )
	{
		GUIScreenFeedback.i.ShowGetToTheEntrance();
		GameDirector.i.worldRight.camera.earthquakeEnabled = true;
	}
	
}
