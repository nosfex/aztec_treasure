using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackSensor : MonoBehaviour
{
	//public BaseObject sensedObject;

	public HashSet<BaseObject> sensedObjects;
	//public HashSet<BaseObject> testedObjects;

//	bool alreadyChecked = false;

	public void Start()
	{
		sensedObjects = new HashSet<BaseObject>();
//		testedObjects = new HashSet<BaseObject>();
	}

//	public BaseObject[] CheckSensedObjects()
//	{
//		RaycastHit[] r = rigidbody.SweepTestAll( Vector3.right * 0.003f );
//
//		if ( r == null )
//			return null;
//
//		if ( r.Length == 0 )
//			return null;
//
//		BaseObject[] result = new BaseObject[r.Length];
//
//		for ( int i = 0; i < r.Length; i++ )
//		{
//			BaseObject bo = r[i].collider.gameObject.GetComponentInChildren<BaseObject>();
//
//			if ( bo != null ) 
//				result[i] = bo;
//			else 
//				result[i] = null;
//
//			Debug.Log ( "result", r[i].collider );
//		}
//
//		return result;
//	}



//	public BaseObject CheckSensorOnce()
//	{
//		if ( sensedObjects.Count > 0 )
//		{
//			HashSet<BaseObject>.Enumerator e = sensedObjects.GetEnumerator();
//			e.MoveNext();
//			BaseObject bo = e.Current;
//			testedObjects.Add ( e.Current );
//			sensedObjects.Remove ( e.Current );
//			return e.Current;
//		}
//		
//		return null;
//	}

//	public void ResetSensorChecks()
//	{
//		while ( testedObjects.Count > 0 )
//		{
//			HashSet<BaseObject>.Enumerator e = testedObjects.GetEnumerator();
//			e.MoveNext();
//			BaseObject bo = e.Current;
//			sensedObjects.Add ( e.Current );
//			testedObjects.Remove ( e.Current );
//		}
//	}

	public void Cleanup()
	{
		HashSet<BaseObject>.Enumerator e = sensedObjects.GetEnumerator();
	
		while ( e.MoveNext() )
		{
			if ( e.Current == null )
			{
				sensedObjects.Remove ( e.Current );
				e = sensedObjects.GetEnumerator();
				//print ("collecting trash...");
			}
		}
	}

	public bool IsObjectInside( BaseObject other )
	{
		bool sensed = sensedObjects.Contains( other );
		return sensed;
	}
	
	void OnTriggerEnter( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();
			
		if ( bo && bo.collisionEnabled )
		{
			//alreadyChecked = false;			
			//sensedObject = bo;
			sensedObjects.Add ( bo );
			//sensedObjects.Add ( bo );
		}
	}

	void OnTriggerExit( Collider other )
	{
		BaseObject bo = other.GetComponentInChildren<BaseObject>();

		if ( bo && bo.collisionEnabled && sensedObjects.Contains(bo) )
		{
			//alreadyChecked = false;
			sensedObjects.Remove(bo);
		}		

//		if ( bo && bo.collisionEnabled && bo == sensedObject )
//		{
//			alreadyChecked = false;
//			sensedObject = null;
//		}
	}
}
