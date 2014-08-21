using UnityEngine;
using System.Collections;

public class SkellyMage : Skelly
{

	public AudioSource[] SFXDeath;
	Vector3 playerPosition;
	
	public GameObject attackObject;
	public GameObject targetObject;
	override protected void Start()
	{
		base.Start ();
		minStairClimb = 0.4f;
		attackCooldown = 1.2f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		base.Update();
			
	}

	override protected void UpdateWalking()
	{
		base.UpdateWalking();
	}

	override protected void UpdateAttacking()
	{

		// GH: blink for attack
		if ( Time.frameCount % 4 < 2 )
			animator.renderer.material.SetColor ( "_AddColor", Color.red );
		else 
			animator.renderer.material.SetColor ( "_AddColor", Color.black );
		// GH: Launch an attack and get back to walking
		if(stateTimer == 0)
		{
			GameObject target = (GameObject)Instantiate(targetObject);
			playerPosition = GameDirector.i.playerRight.transform.position;	
			target.transform.position = new Vector3(playerPosition.x, playerPosition.y - 0.35f, playerPosition.z);
		}
		if ( stateTimer > 0.521f)
		{
		
			launchAttack();
		}
		/*float distance = Vector3.Distance(playerPosition, transform.position);
		if(distance > 2)
		{
			print ("NO SE PROGRAMAR");
			state = State.WALKING;
		}*/
	}
	
	void launchAttack()
	{
		GameObject attack = (GameObject)Instantiate(attackObject);
		attack.transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
		attack.GetComponent<AttackObject>().playerPosition = playerPosition;
		state = State.WALKING;
		cooldown = attackCooldown;
		animator.renderer.material.SetColor ( "_AddColor", Color.black );
	}
}

