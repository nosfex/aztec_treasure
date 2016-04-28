using UnityEngine;
using System.Collections;

public class Cosa : MonoBehaviour {
	
	
	void OnTriggerEnter( Collider other )
	{
		GetComponent<ParticleSystem>().Play();
	}

	void OnTriggerExit( Collider other )
	{
		GetComponent<ParticleSystem>().Stop();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
