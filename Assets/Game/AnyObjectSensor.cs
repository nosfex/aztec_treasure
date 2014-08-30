using UnityEngine;
using System.Collections;

public class AnyObjectSensor : MonoBehaviour 
{
	public Collider sensedObject;

	void OnTriggerEnter( Collider other )
	{
		if ( !transform.IsChildOf( other.transform ) )
			sensedObject = other;
	}

	void Update()
	{
	}

	void OnTriggerExit( Collider other )
	{
		if ( other == sensedObject )
			sensedObject = null;
	}
}
