﻿using UnityEngine;
using System.Collections;

public class KeyItem : BaseObject 
{
	void OnLifted( GameObject src )
	{
		GameDirector.i.playerRight.keysCount++;
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
