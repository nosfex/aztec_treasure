using UnityEngine;
using System.Collections;

public class Vine : BaseObject 
{
	
	Vector3 startPosition;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start ();
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnHit( GameObject obj )
	{
		SpriteAnimator animator = GetComponentInChildren<SpriteAnimator>();
		if (animator)
		{
			//animator.transform.parent = transform.parent;
			animator.PlayAnim("Death");
		}
		
		collisionEnabled = false;
		//Destroy ( gameObject );
	}
}
