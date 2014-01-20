using UnityEngine;
using System.Collections;

public class Vine : BaseObject 
{
	Vector3 startPosition;

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
		SpriteAnimator animator = GetComponentInChildren<SpriteAnimator>();
		if (animator)
		{
			//animator.transform.parent = transform.parent;
			animator.PlayAnim("Death");
		}
		
		collisionEnabled = false;
		//Destroy ( gameObject );
		Destroy ( gameObject );
	}
}
