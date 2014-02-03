using UnityEngine;
using System.Collections;

public class Projectile : BaseObject 
{
	
	public float speed;
	public float lifeSpan = 5.0f;
//	public override void Start () 
	//{
	//}
	
	void Update () 
	{
		float frameRatio = Mathf.Clamp01(Time.deltaTime / 0.016f);
		transform.position -= transform.forward * speed * frameRatio;
		lifeSpan -= Time.deltaTime;
		if(lifeSpan <= 0.0f)
			Destroy(gameObject);
	}
	
	void OnHit( GameObject other )
	{
		//print("projectile vs " + other.ToString() );
		Destroy ( gameObject );
	}
	
	int life = 2;
	
	void OnTriggerEnter( Collider other )
	{
		
		if ( other.gameObject == transform.parent.gameObject )
		{
			//print("Projectile vs Parent");
			return;
		}
		
		if ( other.GetComponent<EnemyRanged>() != null )
		{
			//print("Projectile vs enemyRanged");
			return;
		}
		
		if ( other.tag.Contains( "Destructable" ) )
		{	
			life--;
			//print ( "life = " + life );
		}
		else if ( other.gameObject.tag.Contains("Wall") || other.gameObject.name.Contains("Wall"))
		{
			life = 0;
			//print ( "Projectile vs Wall = " + life );
		}
		
		other.SendMessage( "OnHit", gameObject, SendMessageOptions.DontRequireReceiver );
		
		BaseObject bo = other.GetComponent<BaseObject>();
		
		if ( null != bo )
		{
			bo.velocity -= transform.forward * speed;
		}
		
		if ( life <= 0 )
			Destroy ( gameObject );
	}
}
