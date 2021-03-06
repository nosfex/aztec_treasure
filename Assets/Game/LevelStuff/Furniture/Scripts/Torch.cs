﻿using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour 
{
	public static int tutorialCount = 3;
	
	bool turnedOn = false;
	public Light light;
	public GameObject tipContainer;
	
	public bool isTurnedOn()
	{
		return turnedOn;
	}
	
	public void TurnOff()
	{
		SpriteAnimator animator = GetComponent<SpriteAnimator>();
		
		if ( animator == null ) 
			return;
		
		animator.PlayAnim("IdleOff");
		turnedOn = false;
		UpdateLightIntensity();
	}
	
	public void TurnOn()
	{
		SpriteAnimator animator = GetComponent<SpriteAnimator>();
		if ( animator == null ) 
			return;

//		if ( !turnedOn )
//		{
//			tutorialCount--;
//			
//			if ( tutorialCount == 0 )
//				GUIScreenFeedback.i.ShowTorch();
//		}
		
		animator.PlayAnim("IdleOn");
		turnedOn = true;
		UpdateLightIntensity();
	}
	
	void UpdateLightIntensity()
	{
		if ( turnedOn )
			targetIntensity = .5f;
		else
			targetIntensity = 0f;
	}
	
	public void OutLineOfSight()
	{
		//renderer.enabled = false;
		if ( turnedOn )
			targetIntensity = .5f;
	}
	
	float targetIntensity;
	public bool startOn;
	void Start()
	{
		SpriteAnimator animator = GetComponent<SpriteAnimator>();
		if ( animator == null ) 
			return;

		startOn = true;

		
		if ( gameObject.layer == LayerMask.NameToLayer( "Past" ) || startOn )
		{
			TurnOn();
			tutorialCount++; // grasa pero bueh.
		}
		else 
		{
			if ( GameDirector.i != null )
			{
				if ( Vector3.Distance( transform.position, GameDirector.i.playerRight.transform.position ) < 3.0f )
				{
					TurnOn ();
					tutorialCount++; // grasa pero bueh.
				}
			}
		}
	}
	
	void Update()
	{
		light.intensity += (targetIntensity - light.intensity) * 0.5f;

		if ( turnedOn )
		{
			if ( Time.frameCount % 16 < 8 )
				targetIntensity = 0.48f;
			else 
				targetIntensity = 0.5f;
		}
		
//		if ( tipContainer == null )
//			return;
//		
//		if ( !turnedOn )
//		{
//			if ( Vector3.Distance( transform.position, GameDirector.i.playerRight.transform.position ) < 3.0f )
//				tipContainer.SetActive( tutorialCount > 0 );
//			else 
//				tipContainer.SetActive( false );
//		}
//		else 
//		{
//			if ( tipContainer.activeSelf )
//				tipContainer.SetActive( false );
//		}
	}
	
	public void InLineOfSight()
	{
		//renderer.enabled = true;
		UpdateLightIntensity();
	}
}
