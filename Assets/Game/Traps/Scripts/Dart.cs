using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour {
	
	public float speed;

	void Start () 
	{
	}
	
	void Update () 
	{
		float frameRatio = Mathf.Clamp01(Time.deltaTime / 0.016f);
		transform.position -= transform.forward * speed * frameRatio;
	}
	
	int life = 2;
	
	void OnTriggerEnter( Collider other )
	{
		
		if ( other.gameObject == transform.parent.gameObject )
			return;
		
		if ( other.tag.Contains( "Destructable" ) )
		{	
			life--;
			print ( "life = " + life );
		}
		
		if ( other.tag.Contains("Wall") 
			|| other.name.Contains("Wall")
			|| other.gameObject.tag.Contains("Wall") 
			|| other.gameObject.name.Contains("Wall") )
		{
			life = 0;
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
