using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {
	
	
	[HideInInspector]
	public Vector3 playerPosition = Vector3.zero;
	
	public float maxLifeSpan = 1.2f;
	float lifeSpan = 0;
	public float maxIdleCount = 0.3f;
	float idleCount = 0;
	
	// Use this for initialization
	void Start () {
	
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
