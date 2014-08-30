using UnityEngine;
using System.Collections;

public class LifeSpan : MonoBehaviour {
	
	public float maxLifeSpan = 1.2f;
	float currentSpan = 0;
	// Use this for initialization
	void Start () {
		iTween.FadeFrom( gameObject, 
		              iTween.Hash ( 
		             "time", .5f, 
		             "alpha", 0f ) );


	}
	
	// Update is called once per frame
	void Update () {

		transform.localScale += ((Vector3.one * 1.5f) - transform.localScale) * 0.3f;

		currentSpan += Time.deltaTime;
		if(currentSpan >= maxLifeSpan)
		{
			iTween.FadeTo( gameObject, 
			              iTween.Hash ( 
			             "time", .5f, 
			             "alpha", 0, 
			             "oncompletetarget", gameObject, 
			             "oncomplete", "Die" ) );
			enabled = false; // stop calling update
		}
	}

	void Die()
	{
		Destroy(this.gameObject);
	}
}
