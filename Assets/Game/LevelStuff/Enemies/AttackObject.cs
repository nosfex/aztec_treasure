using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {
	
	
	[HideInInspector]
	public Vector3 playerPosition = Vector3.zero;
	
	public float maxLifeSpan = 2.2f;
	float lifeSpan = 0;
	public float maxIdleCount = 0.3f;
	float idleCount = 0;

	float ySpeed = 0.8421f ;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(idleCount < maxIdleCount)
		{
			idleCount += Time.deltaTime;
			return;
		}
		
		UpdateAttack();
		
		if(lifeSpan >= maxLifeSpan)
		{
			Destroy(this.gameObject);
			
		}
		lifeSpan += Time.deltaTime;
	}
	
	public void  UpdateAttack()
	{

		transform.position +=  (playerPosition - transform.position) * 0.08125f;

		float y  = (transform.position.y * Mathf.Sin(transform.position.x / transform.position.y )) * 0.001f;
		y = Mathf.Atan2(transform.position.y, transform.position.x) / 8 ; 

		//y *= -0.00181f;
		ySpeed *= y;
		ySpeed -= (float)(0.15f * Time.deltaTime);
		print(y.ToString());
		transform.position = new Vector3(transform.position.x, transform.position.y  +ySpeed , transform.position.z);
	}
	
	public void OnTriggerEnter(Collider other)
	{
		Player player = other.gameObject.GetComponentInChildren<Player>();
		
		if(player == null)
		{
		
		}
		else
		{
			player.OnHit(this.gameObject);
			
		}
	}
}
