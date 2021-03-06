﻿using UnityEngine;
using System.Collections;

public class ExplosionSkelly : MonoBehaviour 
{
	public GameObject[] prefabDebris;
	
	public bool silent = false;

	void Start () 
	{
		if ( silent )
		{
			AudioSource audio = GetComponent<AudioSource>();
			
			if ( audio != null )
				Destroy( audio );
		}
		
		if ( !silent )
			GameDirector.i.worldRight.camera.Shake( 0.1f, 0.2f );
		
		int partIndex = 1;
		
		foreach ( GameObject go in prefabDebris )
		{
			GameObject instance = (GameObject)Instantiate( go, transform.position + (Vector3.up * 0.4f), Quaternion.identity );
			BaseObject bo = instance.GetComponentInChildren<BaseObject>();
			float force = 0.025f;
			bo.velocity.x = Random.Range ( -force, force );
			bo.velocity.z = Random.Range ( -force, force );
			bo.gravity.y = Random.Range( -0.01f, -0.03f );
			
			SpriteAnimator spr = instance.GetComponentInChildren<SpriteAnimator>();
			spr.startingAnimationName = "Part" + partIndex;
			partIndex++;
			
			instance.transform.parent = transform;
		}
	}
	
	void Update () 
	{
	
	}
}
