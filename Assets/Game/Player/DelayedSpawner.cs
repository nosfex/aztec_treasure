using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayedSpawner : MonoBehaviour {
	
	public static DelayedSpawner i { get { return instance; } }
	private static DelayedSpawner instance;
	
	class DelayedTraps
	{
		public GameObject trap;
		public Transform trapTransform;
		public float timeToSpawn;
		public ParticleSystem effect;
		public GameObject objToDel;
	};
	
	List<DelayedTraps> traps;
	
	public ParticleSystem effect;
	
	
	void Awake() 
	{
		instance = this;	
		
	}

	void Start () 
	{
		traps = new List<DelayedTraps>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0 ; i < traps.Count ; i++)
		{
			traps[i].timeToSpawn -= Time.deltaTime;
			
			if( traps[i].timeToSpawn <= 0 )
			{
				GameObject t = (GameObject)Instantiate(traps[i].trap, traps[i].trapTransform.position, traps[i].trapTransform.rotation);
				t.transform.parent = ((World)GameDirector.i.worldRight).transform;
				t.transform.localPosition = traps[i].trapTransform.localPosition;
				
				Destroy( traps[i].effect );
				if(traps[i].objToDel != null)
				{
					Destroy( traps[i].objToDel);
				}
				traps.RemoveAt(i);
				i--;
			}
		}
	
	}
	
	public void addSpawnData(GameObject prefab, Transform t, float time, GameObject del = null)
	{
		DelayedTraps trap = new DelayedTraps();
		
		ParticleSystem p = (ParticleSystem)Instantiate(effect, t.position, effect.transform.rotation );
		p.transform.parent = ((World)GameDirector.i.worldRight).transform;
		p.transform.localPosition = t.localPosition;
		
		trap.trap = prefab;
		trap.trapTransform = t;
		trap.timeToSpawn = time;
		trap.effect = p;
		trap.objToDel = del;
		traps.Add ( trap );
	}
}
