using UnityEngine;
using System.Collections;

public class GUITrapSelector : MonoBehaviour {
	
	public static GUITrapSelector i { get { return instance; } }
	private static GUITrapSelector instance;

	public Transform cursor;

	public Trap[] traps;
	
	void Awake () 
	{
		instance = this;
	}
	
	void Update () 
	{
		Vector3 pos = cursor.position; 
		
		AztecPlayer p = (AztecPlayer)GameDirector.i.playerLeft;
		pos.x = traps[ p.currentTrapIndex ].transform.position.x;
		cursor.position = pos;
		
/*		if(resetMaterial[resetMaterialCount] == null)
		{	
			resetMaterial[resetMaterialCount] = traps[p.currentTrap].renderer.material;
		}*/
	}
}