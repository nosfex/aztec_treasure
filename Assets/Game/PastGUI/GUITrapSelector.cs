using UnityEngine;
using System.Collections;

public class GUITrapSelector : MonoBehaviour {
	
	
	Transform cursor;
	
	// Use this for initialization
	void Start () 
	{
	//	traps = new TextMesh[ ((AztecPlayer)(GameDirector.i.worldLeft.player)).maxTraps];	
		
		
	//	for(int i = 0;  i < traps.Length; i++)
		{
			
		}
		
		for(int i = 0; i < transform.childCount; i ++)
		{
			if(transform.GetChild(i).name == "Cursor")
			{
				cursor = transform.GetChild(i);
			}
		}
		//cursor.position = new Vector3(-6.85f, cursor.position.y, cursor.position.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = cursor.position; 
		pos.x = -31.0f + ((AztecPlayer)(GameDirector.i.playerLeft)).currentTrap;
		cursor.position = pos;
	}
}