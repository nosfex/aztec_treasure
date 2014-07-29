using UnityEngine;
using System.Collections;

public class MovingFloor : MonoBehaviour 
{
	public Vector3 startingVelocity;
	Vector3 velocity;
	// Use this for initialization
	void Start () {
		velocity = startingVelocity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		enterTriggerCooldown -= Time.deltaTime;
		transform.position += velocity * Time.deltaTime;
	}

	float enterTriggerCooldown = 0.5f;

	void OnTriggerEnter( Collider other )
	{
		//Debug.Log( "oso! ", other );
		if ( enterTriggerCooldown < 0 && (other.gameObject.name == "WallTile" || other.gameObject.name == "InvisibleWall") )
		{

			velocity *= -1;
			enterTriggerCooldown = 0.5f;
		}
	}
}
