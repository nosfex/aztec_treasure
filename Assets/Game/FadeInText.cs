using UnityEngine;
using System.Collections;

public class FadeInText : MonoBehaviour 
{

	public float delay;
	public float duration;
	
	float delayTimer;
	float progressTimer;
	
	void Awake () 
	{
		foreach ( Renderer r in GetComponentsInChildren<Renderer>() )
			r.enabled = false;
		
		foreach ( TextMesh t in GetComponentsInChildren<TextMesh>() )
			t.color = new Color( 1.0f, 1.0f, 1.0f, 0 );
					
		
		delayTimer = delay;
		progressTimer = duration;
	}
	
	void Update () 
	{
		if ( delayTimer > 0 )
		{
			delayTimer -= Time.deltaTime;
	
			if ( delayTimer <= 0 )
			{
				foreach ( Renderer r in GetComponentsInChildren<Renderer>() )
					r.enabled = true;
			}
		}
		else 
		{
			progressTimer -= Time.deltaTime;
			
			if ( progressTimer > 0 )
			{
				foreach ( TextMesh t in GetComponentsInChildren<TextMesh>() )
					t.color = new Color( 1.0f, 1.0f, 1.0f, 1.0f - (progressTimer / duration) );
			}
			else 
			{
				foreach ( TextMesh t in GetComponentsInChildren<TextMesh>() )
					t.color = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
				
				enabled = false;
			}
			
		}
	
	}
}
