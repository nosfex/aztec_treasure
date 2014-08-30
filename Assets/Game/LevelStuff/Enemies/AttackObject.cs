using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {
	
	public ParticleSystem explosionParticles;
	public ParticleSystem trailParticles;

	[HideInInspector]
	public Vector3 playerPosition = Vector3.zero;
	
//	public float maxLifeSpan = 2.2f;
//	float lifeSpan = 0;
//	public float maxIdleCount = 0.3f;
//	float idleCount = 0;

	public float gravityMod = 0.5f;
	float ySpeed = 8.8421f ;
	Vector3 tempPos;
	float initDist;
	Vector3 advanceVector;
	// Use this for initialization
	void Start ()
	{
		transform.position += Vector3.up * 0.4f;
		tempPos = transform.position;
		initDist = Vector3.Distance ( playerPosition, tempPos );
		advanceVector = (playerPosition - transform.position) / 120f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		tempPos += advanceVector;
		float dist = Vector3.Distance ( playerPosition, tempPos );
		float x = (1.0f - (dist / initDist)) * 10.5f - 5.0f;
		float invertedCatenaryY = Mathf.Clamp(10 - Catenary( x, 2.0f ), 0, 10f) * 0.2f;
		
		transform.position = tempPos + Vector3.up * invertedCatenaryY;// + (0.4f * Vector3.up * Catenary( (1.0f - (dist / initDist)) * 10.0f - 5.0f, 2.0f ));
		
		if ( Vector3.Distance ( playerPosition, transform.position ) < 0.1f )
			Explode();
	}

	void Explode()
	{
		explosionParticles.gameObject.SetActive( true );
		trailParticles.Stop ();
		renderer.enabled = false;
		this.enabled = false;

		// Hack asqueroso... 
		Component halo = GetComponent("Halo"); 
		halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
		

		Invoke ( "Die", 1.5f );

		Vector3 playerPos = GameDirector.i.playerRight.transform.position;

		if ( Vector3.Distance( playerPos, transform.position ) < 1.0f )
		{
			GameDirector.i.playerRight.OnHit( gameObject );
		}
	}

	void Die()
	{
		Destroy(this.gameObject);
	}

	float Catenary( float x, float a )
	{
		return a * cosh( x / a );
	}

	float cosh( float x )
	{
		return 0.5f * (Mathf.Exp(x) + Mathf.Exp (-x));
	}
	
	public void OnTriggerEnter(Collider other)
	{
//		Player player = other.gameObject.GetComponentInChildren<Player>();
//
//		if(player != null)
//		{
//			player.OnHit(this.gameObject);
//			
//		}
	}
}
