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
	
	public GameObject dartPrefab;
	
	void OnTriggerEnter( Collider other )
	{
		if ( other.tag.Contains("Wall") ) // Non-destructible walls don't trigger this sensor.
			return;
		
		if ( other.GetComponent<BaseObject>() != null && !alreadyTriggered )
		{
			GameObject dartgo = (GameObject)Instantiate ( dartPrefab, transform.position, transform.rotation );
			dartgo.layer = gameObject.layer;
			dartgo.transform.parent = transform;
			
			alreadyTriggered = true;
		}
	}
	
	void OnPlayerDead()
	{
		alreadyTriggered = false;
	}
}
