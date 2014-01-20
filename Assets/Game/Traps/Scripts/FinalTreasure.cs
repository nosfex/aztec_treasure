using UnityEngine;
using System.Collections;

public class FinalTreasure : BaseObject 
{
	void OnLifted( GameObject src )
	{
		GUIScreenFeedback.i.ShowGetToTheEntrance();
		GameDirector.i.worldRight.camera.earthquakeEnabled = true;
	}
	
}
