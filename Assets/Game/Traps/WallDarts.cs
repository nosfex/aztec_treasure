using UnityEngine;
using System.Collections;

public class WallDarts : MonoBehaviour {
	
	ParticleSystem darts;
	// Use this for initialization
	void Start () 
	{
		darts = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	bool alreadyTriggered = false;
	
	void OnTriggerEnter( Collider other )
	{
		if ( other.GetComponent<BaseObject>() != null && !alreadyTriggered )
		{
			darts.Play ();
			alreadyTriggered = true;
		}
	}
	
	void OnPlayerDead()
	{
		alreadyTriggered = false;
	}
}
