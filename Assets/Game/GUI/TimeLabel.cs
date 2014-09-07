using UnityEngine;
using System.Collections;

public class TimeLabel : MonoBehaviour {

	public float delay = 0;
	float timer = 0;
	void StartFade () 
	{
		renderer.enabled = true;
		enabled = false;
		iTween.MoveFrom( gameObject, iTween.Hash( "oncomplete", "OnFadeInComplete", "y", transform.position.y + 2.0f, "time", 2.0f - delay, "easetype", iTween.EaseType.easeOutQuad ) );
		iTween.FadeFrom( gameObject, iTween.Hash( "alpha", 0, "time", 2.0f - delay ) );
	}

	void Start()
	{
		renderer.enabled = false;
	}

	void OnFadeInComplete()
	{
		iTween.FadeTo ( gameObject, iTween.Hash ( "delay", 1.5f, "alpha", 0, "time", 1.0f, "oncomplete", "OnFadeInEnd") );
	}
	
	void OnFadeInEnd()
	{
	}
	
	void Update () 
	{
		timer += Time.deltaTime;
		if ( timer > delay )
		{
			StartFade();
		}
	}
}
