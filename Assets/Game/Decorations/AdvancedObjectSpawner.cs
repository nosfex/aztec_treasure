using UnityEngine;
using System.Collections;

public class AdvancedObjectSpawner : MonoBehaviour 
{
	public GameObject[] objectList;
	
	public int randomGroup = 0;
	public int percentSpawnChance = 100;
	
	void Start()
	{
		Random.seed = (int)transform.parent.localPosition.x + (int)transform.parent.localPosition.z + randomGroup;

		if ( Random.Range( 0, 100 ) > percentSpawnChance )
		{
			Destroy( gameObject );
			return;
		}
		
		if ( objectList.Length > 0 )
		{
			GameObject objectToRespawn = objectList[ Random.Range(0, objectList.Length) ];
			
			GameObject go = (GameObject)Instantiate ( objectToRespawn, transform.position, transform.rotation );
			go.transform.parent = transform.parent;
			
			Destroy ( gameObject );
			//Debug.Log ("spawning.. " + objectToRespawn, go );
		}
	}
}
