using UnityEngine;
using System.Collections;

public class AdvancedEffectObjectSpawner : MonoBehaviour 
{

	public GameObject[] objectList;
	
	public int randomGroup = 0;
	public float percentSpawnChance = 100;

	public GameObject effectToRun;
	public float effectTime;
	
	void Start()
	{
		//effectToRun.GetComponent<ParticleSystem>().Play();
		GameObject particle = (GameObject)Instantiate(effectToRun);
		particle.transform.parent = transform;
		particle.transform.localPosition = Vector3.zero;

		Invoke("onEffectFinished", effectTime);
	}


	void onEffectFinished()
	{
		if ( transform.parent.gameObject.layer == LayerMask.NameToLayer("Past") )
		{
			Destroy( gameObject );
			return;
		}
		
		Random.seed = (int)transform.parent.localPosition.x + (int)transform.parent.localPosition.z + randomGroup;
		
		if ( Random.Range( 0, 100f ) > percentSpawnChance )
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
