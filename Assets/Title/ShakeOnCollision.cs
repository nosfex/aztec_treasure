using UnityEngine;
using System.Collections;

public class ShakeOnCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public GameObject prefabSmoke;
	
	void OnCollisionEnter( Collision other )
	{
		iTween.ShakePosition( Camera.main.gameObject, iTween.Hash ( "amount", Vector3.one * 0.1f, "time", 0.3f ) );
		Debug.Log ("Collided!!!");
		
		GameObject smoke = (GameObject)Instantiate( prefabSmoke, other.collider.transform.position, prefabSmoke.transform.rotation ); 
		
	}
}
