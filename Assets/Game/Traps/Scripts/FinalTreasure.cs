using UnityEngine;
using System.Collections;

public class FinalTreasure : BaseObject 
{
	void OnLifted( GameObject src )
	{
		GameDirector.i.worldRight.camera.earthquakeEnabled = true;
	}
	
}
