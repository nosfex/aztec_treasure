using UnityEngine;
using System.Collections;

public class Vine : BaseObject {
	
	Vector3 startPosition;
	// Use this for initialization
	void Start () {
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
	
	public void OnPlayerDead()
	{
		transform.position = startPosition;
		collisionEnabled = true;
		SpriteAnimator animator = GetComponentInChildren<SpriteAnimator>();
		animator.PlayAnim("Idle");
		animator.StopAnim();
	}
}
