﻿using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
	public GameObject objectToRespawn;
	
	public void Respawn()
	{
		print("Trying to respawn");
		if ( objectToRespawn )
		{
		

			GameObject go = (GameObject)Instantiate ( objectToRespawn, transform.position, transform.rotation );
			go.transform.parent = transform.parent;
			
			Destroy ( gameObject );
			Debug.Log ("spawning.. " + objectToRespawn, go );
		}
	}
}
