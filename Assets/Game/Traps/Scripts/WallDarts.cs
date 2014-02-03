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
		if ( other.GetComponent<Vine>() != null ) // Vines don't trigger this sensor.
			return;
		
		if ( other.GetComponent<BaseObject>() != null && !alreadyTriggered )
		{
			GameObject dartgo = (GameObject)Instantiate ( dartPrefab, transform.position, transform.rotation );
			dartgo.layer = gameObject.layer;
			dartgo.transform.parent = transform;
			
			alreadyTriggered = true;
		}
		
		if( other.gameObject.tag.Contains("Wall") || other.gameObject.name.Contains("Wall") )
		{
			Destroy ( gameObject );	
		}
	}
	
	void OnPlayerDead()
	{
		alreadyTriggered = false;
	}
}
