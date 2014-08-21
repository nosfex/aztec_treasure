using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {
	
	
	[HideInInspector]
	public Vector3 playerPosition = Vector3.zero;
	
	public float maxLifeSpan = 2.2f;
	float lifeSpan = 0;
	public float maxIdleCount = 0.3f;
	float idleCount = 0;

	float ySpeed = 10.8421f ;
	
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

		transform.position +=  ((playerPosition - transform.position) * 0.058125f) ;

		float y  = (transform.position.y * Mathf.Sin(transform.position.x / transform.position.y )) * 0.001f;
		y = Mathf.Atan2(transform.position.y, transform.position.x) * Time.deltaTime  ; 

		//y *= -0.00181f;
		ySpeed *= y / 8 ;
		ySpeed -= (float)(0.01f * Time.deltaTime);
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
