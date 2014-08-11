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
		BaseObject p = other.gameObject.GetComponent<BaseObject>();
		if(p != null)
		{
			p.OnHit(this.gameObject);
		}	

	}

	void OnTriggerStay(Collider other)
	{
		BaseObject p = other.gameObject.GetComponent<BaseObject>();
		if(p != null)
		{
			p.OnHit(this.gameObject);
		}	
		
	}
	
	void OnCollisionEnter(Collision other)
	{
		print("collision");
		BaseObject p = other.gameObject.GetComponent<BaseObject>();
		if(p != null)
		{
			p.OnHit(this.gameObject);
		}	

		SpikeWall wall = other.gameObject.GetComponent<SpikeWall>();
		if(wall != null)
		{
			print("WALL VS WALL");
			touchingWall = true;
		}
	}
	
}
