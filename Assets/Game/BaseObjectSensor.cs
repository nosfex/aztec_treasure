using UnityEngine;
using System.Collections;

public class BaseObjectSensor : MonoBehaviour 
{
	public BaseObject sensedObject;
	
	void OnTriggerEnter( Collider other )
	{
		//print ("enter");
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
		
		if ( bo )
			sensedObject = bo;
	}

	void OnTriggerExit( Collider other )
	{
		//print ("exit");
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
		
		if ( bo && bo == sensedObject )
			sensedObject = null;
	}
}
