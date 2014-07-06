using UnityEngine;
using System.Collections;

public class Vine : BaseObject 
{
	Vector3 startPosition;
	public GameObject prefabExplosion;

	override protected void Start () 
	{
		base.Start ();
		startPosition = transform.position;
	}
	
	void EnterObjectLaid( BaseObject other )
	{
		sleepPhysics = true;
	}
	
	void OnHit( GameObject obj )
	{
		sleepPhysics = false;

		collisionEnabled = false;
		
		GameObject instance = (GameObject)Instantiate( prefabExplosion, transform.position, Quaternion.identity );
		instance.transform.parent = transform.parent;

		Destroy ( gameObject );
	}
}
