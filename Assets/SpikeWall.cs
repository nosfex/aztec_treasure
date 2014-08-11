using UnityEngine;
using System.Collections;

public class SpikeWall : MonoBehaviour {

	// Use this for initialization
	
	[HideInInspector] public bool touchingWall = false;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		Player p = other.gameObject.GetComponent<Player>();
		if(p != null)
		{
			p.OnHit(this.gameObject);
		}	

	}

	void OnTriggerStay(Collider other)
	{
		Player p = other.gameObject.GetComponent<Player>();
		if(p != null)
		{
			p.OnHit(this.gameObject);
		}	
		
	}
	
	void OnCollisionEnter(Collision other)
	{
		print("collision");
		Player p = other.gameObject.GetComponent<Player>();
		if(p != null)
		{
			p.OnHit(this.gameObject);
		}	

		
	}
	
}
