using UnityEngine;
using System.Collections;

public class SpikeWall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void OnCollisionEnter(Collision collision)
	{
		print("WALL IS COLLIDING");
		Player p = collision.gameObject.GetComponent<Player>();
		if(p != null)
		{
			p.OnHit(gameObject);
			
		}
	}
	
}
