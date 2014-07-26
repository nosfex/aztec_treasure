using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackSensor : MonoBehaviour
{
	public BaseObject sensedObject;

	//public List<BaseObject> sensedObjects;
	public HashSet<BaseObject> sensedObjects;
	bool alreadyChecked = false;

	public void Start()
	{
		sensedObjects = new HashSet<BaseObject>();
	}

	public BaseObject CheckSensorOnce()
	{
		if ( sensedObjects.Count > 0 )
		{
			HashSet<BaseObject>.Enumerator e = sensedObjects.GetEnumerator();
			e.MoveNext();
			BaseObject bo = e.Current;
			sensedObjects.Remove ( e.Current );
			return e.Current;
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
			sensedObjects.Add ( bo );
			//sensedObjects.Add ( bo );
		}
	}

	void OnTriggerExit( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();

		if ( bo && sensedObjects.Contains(bo) )
		{
			alreadyChecked = false;
			sensedObjects.Remove(bo);
		}

		if ( bo && bo == sensedObject )
		{
			alreadyChecked = false;
			sensedObject = null;
		}
	}
}
