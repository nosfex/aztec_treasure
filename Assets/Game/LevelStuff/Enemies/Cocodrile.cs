using UnityEngine;
using System.Collections;

public class Cocodrile : Skelly {

	public AudioSource[] SFXDeath;
	Vector3 playerPosition;
	public ParticleSystem trailParticles;

	override protected void Start()
	{
		base.Start ();
		minStairClimb = 0.4f;
		attackCooldown = 1.2f;
		trailParticles.Stop();
	}
	
	// Update is called once per frame
	void Update ()
	{
		base.Update();
		trailParticles.transform.position = transform.position;
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
		}
		if ( stateTimer > 0.521f && stateTimer < 0.8f) 
		{
			trailParticles.Play();
			velocity = direction * speed * attackSpeedFactor;
			cooldown = attackCooldown;
		}
		
		if(stateTimer > 0.8f)
		{
			state = State.WALKING;
			cooldown = attackCooldown;
			trailParticles.Stop();
			animator.renderer.material.SetColor ( "_AddColor", Color.black );
		}
	}
	
	override protected void OnTriggerEnter( Collider other )
	{
		
		if ( state != State.DYING )
		{
			if(state == State.ATTACKING)
			{
				Player p = other.GetComponent<Player>();
	
				if ( p != null && !p.isImmune )
				{
					//print ("skelly ataca algo");
					p.OnHit( gameObject );
			
				}
				
				Vine v = other.GetComponent<Vine>();
				
				if ( v != null )
					v.SendMessage ("OnHit", gameObject, SendMessageOptions.DontRequireReceiver);
				
				Pottery po = other.GetComponent<Pottery>();
				if(po != null)
				{
					po.SendMessage ("Die", gameObject, SendMessageOptions.DontRequireReceiver);
				}
					
			}
		}
		
		base.OnTriggerEnter( other );
	}

}
