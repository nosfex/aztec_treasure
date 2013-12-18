using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayedSpawner : MonoBehaviour {
	
	public static DelayedSpawner i { get { return instance; } }
	private static DelayedSpawner instance;
	
	List<GameObject> trap;
	List<Transform> trapTransform;
	List<float> timeToSpawn;
	public ParticleSystem effect;
	
	
	void Awake() 
	{
		instance = this;	
		
	}
	// Use this for initialization
	void Start () {
		trap = new List<GameObject>();
		trapTransform = new List<Transform>();
		timeToSpawn = new List<float>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0 ; i < timeToSpawn.Count ; i++)
		{
			timeToSpawn[i] -= Time.deltaTime;
			if(timeToSpawn[i] <= 0)
			{
				GameObject t = (GameObject)Instantiate(trap[i], trapTransform[i].position, trapTransform[i].rotation);
				t.transform.parent = ((World)GameDirector.i.worldRight).transform;
				t.transform.localPosition = trapTransform[i].localPosition;
				
				trap.RemoveAt(i);
				timeToSpawn.RemoveAt(i);
				trapTransform.RemoveAt(i);
			//	effect.Play();
			
			}
		}
	
	}
	
	public void addSpawnData(GameObject prefab, Transform t, float time)
	{
	//	timeToSpawn
		trap.Add(prefab);
		trapTransform.Add(t);
		timeToSpawn.Add(time);
		ParticleSystem p = (ParticleSystem)Instantiate(effect, t.position, t.rotation);
		p.transform.parent = ((World)GameDirector.i.worldRight).transform;
		p.transform.localPosition = t.localPosition;
	}
}
