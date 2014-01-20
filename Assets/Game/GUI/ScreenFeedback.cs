using UnityEngine;
using System.Collections;

public class ScreenFeedback : MonoBehaviour 
{
	void Start () 
	{
		iTween.MoveFrom( gameObject, iTween.Hash( "oncomplete", "OnFadeInComplete", "y", transform.position.y + 2.0f, "time", 1.0f, "easetype", iTween.EaseType.easeOutQuad ) );
		iTween.FadeFrom( gameObject, iTween.Hash( "alpha", 0, "time", 1.0f ) );
	
	}
	
	void OnFadeInComplete()
	{
		iTween.FadeTo ( gameObject, iTween.Hash ( "delay", 1.5f, "alpha", 0, "time", 1.0f, "oncomplete", "OnFadeOutEnd") );
	}
	
	void OnFadeOutEnd()
	{
		Destroy( gameObject );
	}
}
