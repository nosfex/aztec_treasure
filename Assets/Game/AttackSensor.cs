using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackSensor : MonoBehaviour
{
	public BaseObject sensedObject;

	//public List<BaseObject> sensedObjects;
	public HashSet<BaseObject> sensedObjects;
	public HashSet<BaseObject> testedObjects;

	bool alreadyChecked = false;

	public void Start()
	{
		sensedObjects = new HashSet<BaseObject>();
		testedObjects = new HashSet<BaseObject>();
	}

	public BaseObject CheckSensorOnce()
	{
		if ( sensedObjects.Count > 0 )
		{
			HashSet<BaseObject>.Enumerator e = sensedObjects.GetEnumerator();
			e.MoveNext();
			BaseObject bo = e.Current;
			testedObjects.Add ( e.Current );
			sensedObjects.Remove ( e.Current );
			return e.Current;
		}
		
		return null;
	}

	public void ResetSensorChecks()
	{
		while ( testedObjects.Count > 0 )
		{
			HashSet<BaseObject>.Enumerator e = testedObjects.GetEnumerator();
			e.MoveNext();
			BaseObject bo = e.Current;
			sensedObjects.Add ( e.Current );
			testedObjects.Remove ( e.Current );
		}
	}
	
	void OnTriggerEnter( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
			
		if ( bo && bo.collisionEnabled )
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

		if ( bo && bo.collisionEnabled && sensedObjects.Contains(bo) )
		{
			alreadyChecked = false;
			sensedObjects.Remove(bo);
		}		

		if ( bo && bo.collisionEnabled && testedObjects.Contains(bo) )
		{
			alreadyChecked = false;
			testedObjects.Remove(bo);
		}

		if ( bo && bo.collisionEnabled && bo == sensedObject )
		{
			alreadyChecked = false;
			sensedObject = null;
		}
	}
}
