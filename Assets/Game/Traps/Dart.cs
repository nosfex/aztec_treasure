using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour {
	
	public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
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
		
		if ( other.tag == "WallDestructable" )
		{	
			life--;
			print ( "life = " + life );
		}
		else if ( other.tag.Contains("Wall") )
		{
			life = 0;
		}

		
		
		other.SendMessage( "OnHit", gameObject, SendMessageOptions.DontRequireReceiver );
		
		if ( life <= 0 )
			Destroy ( gameObject );
	}
}
