using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour 
{
	void Awake()
	{
		if ( Application.isPlaying )
			Destroy( this );
	}

	void Update () 
	{
#if UNITY_EDITOR
		if ( Application.isEditor && !Application.isPlaying )
		{
			if ( !UnityEditor.Selection.Contains( gameObject ) )
				return;
		}
#endif
		
		if ( SnapAssistant.i && SnapAssistant.i.snapEnabled )
		{
			Vector3 caca = (transform.position) / SnapAssistant.i.snapSize;
			caca = new Vector3( Mathf.RoundToInt( caca.x ), Mathf.RoundToInt( caca.y ), Mathf.RoundToInt( caca.z ) );
			transform.position = caca * SnapAssistant.i.snapSize;
			
			
			if ( SnapAssistant.i.useOffset )
				transform.position += SnapAssistant.i.snapOffset;
			
			caca = transform.localScale / SnapAssistant.i.snapSize;
			caca = new Vector3( Mathf.RoundToInt( caca.x ) , caca.y, Mathf.RoundToInt( caca.z ) );
			transform.localScale = caca * SnapAssistant.i.snapSize;
			//transform.localScale += Vector3.one * 0.0001f;
		}
	}
}
