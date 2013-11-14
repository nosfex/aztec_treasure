using UnityEngine;
using System.Collections;

public class FollowSmooth : MonoBehaviour {
	
	public Transform target;
	public float frictionCoef = 0.99f;
	public Vector3 offset;
	
	Vector3 originalPosition;
	void Start()
	{
		originalPosition = transform.position;
		
		offset = transform.position - target.position;
	}
	
	void Update() 
	{
		transform.position += ((target.position + offset) - transform.position) * frictionCoef;
		
		transform.position = new Vector3( transform.position.x, originalPosition.y, transform.position.z );
	}
}
