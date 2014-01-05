using UnityEngine;
using System.Collections;

public class TimeLabel : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		iTween.MoveFrom( gameObject, iTween.Hash( "oncomplete", "OnFadeInComplete", "y", transform.position.y + 2.0f, "time", 1.0f, "easetype", iTween.EaseType.easeOutQuad ) );
		iTween.FadeFrom( gameObject, iTween.Hash( "alpha", 0, "time", 1.0f ) );
	
	}
	
	void OnFadeInComplete()
	{
		iTween.FadeTo ( gameObject, iTween.Hash ( "delay", 1.5f, "alpha", 0, "time", 1.0f, "oncomplete", "OnFadeInEnd") );
	}
	
	void OnFadeInEnd()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
