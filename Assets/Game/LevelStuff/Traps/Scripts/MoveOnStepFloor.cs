using UnityEngine;
using System.Collections;

public class MoveOnStepFloor : MonoBehaviour 
{
	public Vector3 startingVelocity;
	Vector3 velocity;

	bool activated = false;
	// Use this for initialization
	void Start () {
		velocity = startingVelocity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		enterTriggerCooldown -= Time.deltaTime;

		if ( activated )
			transform.position += velocity * Time.deltaTime;
	}

	float enterTriggerCooldown = 0.5f;

	void EnterObjectLaid( BaseObject other ) 
	{
		activated = true;
	}

	void OnTriggerEnter( Collider other )
	{
		Debug.Log( "oso! ", other );
		if ( other.gameObject.name == "WallTile" || 
		     other.gameObject.name == "InvisibleWall" || 
		     other.gameObject.name == "FloorMoveOnStep"  )
		{
			if ( activated )
				velocity = Vector3.zero;
			//enterTriggerCooldown = 0.5f;
		}
	}
}
