using UnityEngine;
using System.Collections;

public class FollowSmooth : MonoBehaviour {
	
	public Transform target;
	public float frictionCoef = 0.99f;
	public Vector3 offset;
	
	void Start()
	{
		offset = transform.position - target.position;
	}
	
	void Update () 
	{
		transform.position += ((target.position + offset) - transform.position) * frictionCoef;
	}
}
