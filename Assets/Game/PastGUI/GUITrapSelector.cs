using UnityEngine;
using System.Collections;

public class GUITrapSelector : MonoBehaviour {
	
	
	public Transform cursor;

	public Transform[] traps;
	// Use this for initialization
	void Start () 
	{
	//	traps = new TextMesh[ ((AztecPlayer)(GameDirector.i.worldLeft.player)).maxTraps];	
		
		
	//	for(int i = 0;  i < traps.Length; i++)
		{
			
		}
		
		//cursor.position = new Vector3(-6.85f, cursor.position.y, cursor.position.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = cursor.position; 
		pos.x = traps[ ((AztecPlayer)(GameDirector.i.playerLeft)).currentTrap ].position.x;
		cursor.position = pos;
	}
}