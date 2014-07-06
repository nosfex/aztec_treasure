using UnityEngine;
using System.Collections;

public class SwitchBridge : BaseObject 
{
	//public float graceTime = 1.0f;
	//public GameObject prefabExplosion;
	
	public string caption;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start ();
		
		foreach( GameObject go in bridgeTiles )
		{
			go.SetActive( false );
		}
		
	}
	
	bool isOn = false;
	void OnPressedFuture( GameObject src )
	{
		if ( lockWhenPressed && isOn )
			return;
		
		GameDirector.i.ShowTextPopup( src, 0.8f, caption );
		
		SpriteAnimator animator = GetComponentInChildren<SpriteAnimator>();
		isOn = !isOn;
		if ( isOn )
			animator.PlayAnim("On");
		else 
			animator.PlayAnim("Off");
		
		if ( isOn )
		{
			//GameDirector.i.worldRight.camera.earthquakeEnabled = true;
			GameDirector.i.worldRight.camera.Shake( 0.1f, 7.0f );
			foreach( GameObject go in bridgeTiles )
			{
				go.SetActive( true );
				iTween.MoveFrom ( go, iTween.Hash ( "position", go.transform.position + Vector3.down * 20.0f, 
					"easetype", iTween.EaseType.easeOutCirc, 
					"time", Random.Range( 5.0f, 7.0f ) ) );
			}
		}
	}
	
	override protected void LateUpdate()
	{
		base.LateUpdate ();
		//graceTime -= Time.deltaTime;
	}
	
	public bool lockWhenPressed = true;
	
	public GameObject[] bridgeTiles;
	
		
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
