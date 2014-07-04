using UnityEngine;
using System.Collections;

public class Pottery : BaseObject 
{
	public GameObject prefabExplosion;
	
	// Use this for initialization
	override protected void Start () 
	{
		base.Start ();
	}

		
	void Die()
	{
		//worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );

		if ( prefabExplosion != null )
		{
			GameObject explosion = (GameObject)Instantiate( prefabExplosion, transform.position, Quaternion.identity );
			explosion.transform.parent = transform.parent;
		}
		
		Destroy( gameObject );
	}
	
	override protected void TestWalls( Collider other )
	{
		Player p = other.GetComponent<Player>();
		
		if ( p != null )
			return;

		other.SendMessage("OnHit", gameObject, SendMessageOptions.DontRequireReceiver );
		Die ();		
	}

}
