using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour 
{
	void Update () 
	{
	
		if ( SnapAssistant.i && SnapAssistant.i.snapEnabled )
		{
			Vector3 caca = transform.position / SnapAssistant.i.snapSize;
			caca = new Vector3( (int)caca.x, (int)caca.y, (int)caca.z );
			transform.position = caca * SnapAssistant.i.snapSize;
			
			caca = transform.localScale / SnapAssistant.i.snapSize;
			caca = new Vector3( (int)caca.x, (int)caca.y, (int)caca.z );
			transform.localScale = caca * SnapAssistant.i.snapSize;
			//transform.localScale += Vector3.one * 0.0001f;
		}
	}
}
