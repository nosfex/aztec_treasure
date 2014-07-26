using UnityEngine;
using System.Collections;

public class LampItem : BaseObject 
{
	public GameObject ambush;
	void OnLifted( GameObject src )
	{
		ambush.SetActive( true );
		GameDirector.i.playerRight.hasLamp = true;
		Destroy( gameObject );
	}
	
	void OnDestroy()
	{
		GameDirector.i.playerRight.ResetLiftSensor();
	}
		
//	void Die()
//	{
//		//worldOwner.BroadcastMessage( "OnEnemyDead", this, SendMessageOptions.DontRequireReceiver );
//
//		if ( prefabExplosion != null )
//		{
//			GameObject explosion = (GameObject)Instantiate( prefabExplosion, transform.position, Quaternion.identity );
//			explosion.transform.parent = transform.parent;
//		}
//		
//		Destroy( gameObject );
//	}
//	
//	override protected void TestFloor( Collider other )
//	{
//		if ( graceTime > 0 )
//			return;
//		Die ();		
//	}
//	
//	override protected void TestWalls( Collider other )
//	{
//		if ( graceTime > 0 )
//			return;
//		Player p = other.GetComponent<Player>();
//		
//		if ( p != null )
//			return;
//
//		Skelly s = other.GetComponent<Skelly>();
//
//		if ( s != null )
//			return;
//
//		other.SendMessage("OnHit", gameObject, SendMessageOptions.DontRequireReceiver );
//		Die ();		
//	}

}
