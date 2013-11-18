using UnityEngine;
using System.Collections;

public class Rock : BaseObject 
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
		Destroy ( gameObject );
	}
}
