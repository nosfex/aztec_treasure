using UnityEngine;
using System.Collections;

public class GUITrapSelector : MonoBehaviour {
	
	public static GUITrapSelector i { get { return instance; } }
	private static GUITrapSelector instance;

	public Transform cursor;

	public Trap[] traps;
	float initPos;
	void Awake () 
	{
		instance = this;
		
		Vector3 endPos = traps[ traps.Length - 1 ].transform.position;
		Vector3 startPos = traps[ 0 ].transform.position;
		
		for ( int i = 0; i < traps.Length; i++ )
		{
			traps[i].transform.position = Vector3.Lerp ( startPos, endPos, (1.0f / traps.Length) * i );
		}
		
		initPos = traps[0].transform.position.y;
		
	}
	public GameObject trapDummy;
	void Update () 
	{
		AztecPlayer p = (AztecPlayer)GameDirector.i.playerLeft;

		initPos = trapDummy.transform.position.y;		
		Vector3 pos = cursor.position; 
		
		pos.x += ((traps[ p.currentTrapIndex ].transform.position.x) - pos.x) / 2.0f;
		
		for ( int i = 0; i < traps.Length; i++ )
		{
			Vector3 supposedPos = traps[ i ].transform.position;
			supposedPos.y = initPos;
			if ( i == p.currentTrapIndex )
				traps[ i ].transform.position += ((supposedPos + (Vector3.up * 10)) - traps[ i ].transform.position) * 0.5f;
			else
				traps[ i ].transform.position += (supposedPos - traps[ i ].transform.position) * 0.5f;
		}

		cursor.position = pos;
		
/*		if(resetMaterial[resetMaterialCount] == null)
		{	
			resetMaterial[resetMaterialCount] = traps[p.currentTrap].renderer.material;
		}*/
	}
	
}