using UnityEngine;
using System.Collections;

public class AttackSensor : MonoBehaviour
{
	public BaseObject sensedObject;
	
	bool alreadyChecked = false;
	
	public BaseObject CheckSensorOnce()
	{
		if ( sensedObject && !alreadyChecked )
		{
			alreadyChecked = true;
			return sensedObject;
		}
		
		return null;
	}
	
	void OnTriggerEnter( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
			
		if ( bo )
		{
			alreadyChecked = false;			
			sensedObject = bo;
		}
	}

	void OnTriggerExit( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
		
		if ( bo && bo == sensedObject )
		{
			alreadyChecked = false;
			sensedObject = null;
		}
	}
}
