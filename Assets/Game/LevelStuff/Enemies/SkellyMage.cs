using UnityEngine;
using System.Collections;

public class SkellyMage : Skelly
{

	public AudioSource[] SFXDeath;
	Vector3 playerPosition;
	
	public GameObject attackObject;
	public float firingDistance = 1.52f;
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
	
		if(distance < firingDistance && stateTimer > 0.4f)
		{
			state = State.ATTACKING;
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
		if ( stateTimer > 0.31f )
		{
			state = State.WALKING;
			
			GameObject attack = (GameObject)Instantiate(attackObject);
			attack.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
			attack.GetComponent<AttackObject>().playerPosition = playerPosition;
		}
		
		float distance = Vector3.Distance(playerPosition, transform.position);
		if(distance >= firingDistance)
		{
			state = State.WALKING;
		}
		
	}
}
