using UnityEngine;
using System.Collections;

public class LifeSpan : MonoBehaviour {
	
	public float maxLifeSpan = 1.2f;
	float currentSpan = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currentSpan += Time.deltaTime;
		if(currentSpan >= maxLifeSpan)
		{
			Destroy(this.gameObject);
		}
	}
}
