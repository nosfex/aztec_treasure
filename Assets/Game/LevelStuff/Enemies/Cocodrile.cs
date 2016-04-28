using UnityEngine;
using System.Collections;

public class Cocodrile : Skelly {

	public AudioSource[] SFXDeath;
	Vector3 playerPosition;
	public ParticleSystem trailParticles;
	ParticleSystem particles;
	GameObject trailP;
	override protected void Start()
	{
		base.Start ();
		minStairClimb = 0.4f;
		attackCooldown = 1.2f;
		trailP = (GameObject)Instantiate(trailParticles.gameObject);
		particles = trailP.GetComponent<ParticleSystem>();
		trailP.transform.parent = this.transform;
		particles.transform.parent = this.transform;

		particles.Stop();
	}
	
	// Update is called once per frame
	void Update ()
	{
		base.Update();
		if(state != Skelly.State.ATTACKING)
		{
			particles.Stop();
			
		}
		trailP.transform.position = transform.position;
	}
	
	override protected void UpdateWalking()
	{
		base.UpdateWalking();
	}
	
	override protected void UpdateAttacking()
	{
		
		// GH: blink for attack
		if ( Time.frameCount % 4 < 2 )
			animator.GetComponent<Renderer>().material.SetColor ( "_AddColor", Color.red );
		else 
			animator.GetComponent<Renderer>().material.SetColor ( "_AddColor", Color.black );
		// GH: Launch an attack and get back to walking
		if(stateTimer == 0)
		{
		}
		if ( stateTimer > 0.521f && stateTimer < 0.8f) 
		{
			if(!particles.isPlaying)
			{
			
				particles.GetComponent<Renderer>().material.mainTextureOffset = animator.GetComponent<Renderer>().material.mainTextureOffset;
				particles.GetComponent<Renderer>().material.mainTextureScale = animator.GetComponent<Renderer>().material.mainTextureScale;

				particles.Play();
			}
			velocity = direction * speed * attackSpeedFactor;
			cooldown = attackCooldown;
		}
		
		if(stateTimer > 0.8f)
		{
			state = State.WALKING;
			cooldown = attackCooldown;
			if(particles.isPlaying)
			{
				particles.Stop();
				
			}
			animator.GetComponent<Renderer>().material.SetColor ( "_AddColor", Color.black );
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

	override protected void TestWalls( Collider other )
	{
		base.TestWalls( other );
		if(state == State.ATTACKING)
		{
			state = State.WALKING;
			particles.Stop();
			cooldown = attackCooldown;
			velocity = Vector3.zero;
		}
	}

}
