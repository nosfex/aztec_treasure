using UnityEngine;
using System.Collections;

public class GUITrapSelector : MonoBehaviour {
	
	
	public Transform cursor;

	public Transform[] traps;
	
	// Use this for initialization
	void Start () 
	{
	//	traps = new TextMesh[ ((AztecPlayer)(GameDirector.i.worldLeft.player)).maxTraps];	
		
/*		resetMaterial = new Material[3]();
		for(int i = 0;  i < 3; i++)
		{
			resetMaterial[i] = null;	
		}*/
		
		//cursor.position = new Vector3(-6.85f, cursor.position.y, cursor.position.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = cursor.position; 
		
		AztecPlayer p = (AztecPlayer)GameDirector.i.playerLeft;
		pos.x = traps[ p.currentTrap ].position.x;
		if(p.activatedAltar >= 1)
		{
			traps[5].renderer.material.SetFloat("_EffectAmount", 0);
		}
		
		if(p.activatedAltar >= 2)
		{
			traps[3].renderer.material.SetFloat("_EffectAmount", 0);
			traps[4].renderer.material.SetFloat("_EffectAmount", 0);
		}
		cursor.position = pos;
		
/*		if(resetMaterial[resetMaterialCount] == null)
		{	
			resetMaterial[resetMaterialCount] = traps[p.currentTrap].renderer.material;
		}*/
	}
}