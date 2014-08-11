using UnityEngine;
using System.Collections;

public class SkellyMage : Skelly
{

	public AudioSource[] SFXDeath;
	Vector3 playerPosition;
	
	public GameObject attackObject;
	public GameObject targetObject;
	public float firingDistance = 1.852f;
	override protected void Start()
	{
		base.Start ();
		minStairClimb = 0.4f;
	}
	
	// Update is called once per frame
	void Update ()
	{
			
		base.Update();
		playerPosition = GameDirector.i.playerRight.transform.position;
		
		
		float distance = Vector3.Distance(playerPosition, transform.position);
	
		if(distance < firingDistance && stateTimer > 0.4f && state == State.WALKING)
		{
		//	Attack();
			if(state != State.ATTACKING)
				state = State.ATTACKING;			
		}
		
		if(distance >= firingDistance && state == State.ATTACKING)
		{
			state = State.WALKING;
			animator.renderer.material.SetColor ( "_AddColor", Color.black );
		}
	}
	
	override protected void UpdateAttacking()
	{
		// GH: blink for attack
		if ( Time.frameCount % 4 < 2 )
			animator.renderer.material.SetColor ( "_AddColor", Color.red );
		else 
			animator.renderer.material.SetColor ( "_AddColor", Color.black );
		// GH: Launch an attack and get back to walking
		if ( stateTimer > 0.521f )
		{
			state = State.WALKING;
			//Invoke("launchAttack", 0.05f);
			launchAttack();
			//stateTimer = 0;
			GameObject target = (GameObject)Instantiate(targetObject);
			target.transform.position = new Vector3(playerPosition.x, playerPosition.y + 0.2f, playerPosition.z);
		}
		
		float distance = Vector3.Distance(playerPosition, transform.position);
		if(distance >= firingDistance)
		{
			state = State.WALKING;
			animator.renderer.material.SetColor ( "_AddColor", Color.black );
		}
		
	}
	
	void launchAttack()
	{
		GameObject attack = (GameObject)Instantiate(attackObject);
		attack.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
		attack.GetComponent<AttackObject>().playerPosition = playerPosition;
	}
}

