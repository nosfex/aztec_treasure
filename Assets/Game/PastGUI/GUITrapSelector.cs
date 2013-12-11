using UnityEngine;
using System.Collections;

public class GUITrapSelector : MonoBehaviour {
	
	
	TextMesh[] traps; 
	
	
	// Use this for initialization
	void Start () 
	{
		traps = new TextMesh[(AztecPlayer)(GameDirector.i.worldLeft.player).maxTraps];	
		
		for(int i = 0;  i < traps.Length; i++)
		{
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
