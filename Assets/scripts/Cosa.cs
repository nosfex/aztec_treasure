using UnityEngine;
using System.Collections;

public class Cosa : MonoBehaviour {
	
	
	void OnTriggerEnter( Collider other )
	{
		particleSystem.Play();
	}

	void OnTriggerExit( Collider other )
	{
		particleSystem.Stop();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
