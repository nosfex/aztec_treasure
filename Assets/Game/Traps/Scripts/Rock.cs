using UnityEngine;
using System.Collections;

public class Rock : BaseObject 
{
	public int maxHearts;

	int hearts;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start ();
		hearts = maxHearts;
		
	}
	
	void OnTriggerEnter( Collider other )
	{
		
	}
		
	void OnHit( GameObject obj )
	{
		/*
		SpriteAnimator animator = GetComponentInChildren<SpriteAnimator>();
		if (animator)
		{
			animator.transform.parent = transform.parent;
			animator.PlayAnim("Death");
		}*/
		
		if ( obj.GetComponent<Player>() != null )
			return;
		
		hearts--;
		
		if ( hearts <= 0 )
			Destroy ( gameObject );
	}
}
