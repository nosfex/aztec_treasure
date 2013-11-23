using UnityEngine;
using System.Collections;

public class BaseObjectSensor : MonoBehaviour 
{
	public BaseObject sensedObject;
	public System.Type typeFilter;
	
	void OnTriggerEnter( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
		
		if ( typeFilter != null )
		{
			//Debug.Log ("typefilter != null" + typeFilter );
			bo = (BaseObject)other.GetComponent( typeFilter );
		}
			
		if ( bo )
		{
			sensedObject = bo;
		}
	}
	
	void OnTriggerStay( Collider other )
	{
		OnTriggerEnter( other );
	}
	
	void Update()
	{
		if ( typeFilter != null && sensedObject != null ) 
		{
			if ( sensedObject.GetComponent( typeFilter ) == null )
				sensedObject = null;
		}
	}
	

	void OnTriggerExit( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
		
		if ( bo && bo == sensedObject )
			sensedObject = null;
	}
}
