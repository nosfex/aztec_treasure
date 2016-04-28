using UnityEngine;
using System.Collections;

public class MoveOnStepFloor : MonoBehaviour 
{
	public float startingVelocity;

	Vector3 velocity;
	public AnyObjectSensor sensor;

	bool activated = false;

	void Start () 
	{
		velocity = transform.right * startingVelocity;
	}

	void Update () 
	{
		enterTriggerCooldown -= Time.deltaTime;

		if ( activated )
			transform.position += velocity * Time.deltaTime;

		if ( sensor.sensedObject != null )
		{
			Collider other = sensor.sensedObject;

			if ( other == GetComponent<Collider>() )
			{
				//print ("Error....");
				return;
			}

			if ( other.gameObject.name == "WallTile" || 
			    other.gameObject.name == "InvisibleWall" || 
			    other.gameObject.name == "FloorMoveOnStep" ||
			    other.gameObject.name.Contains("Floor") )
			{
				if ( activated )
					velocity = Vector3.zero;
			}
		}
	}

	float enterTriggerCooldown = 0.5f;

	void EnterObjectLaid( BaseObject other ) 
	{
		activated = true;
	}

	void OnTriggerEnter( Collider other )
	{
	}
}
