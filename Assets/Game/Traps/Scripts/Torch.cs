using UnityEngine;
using System.Collections;

public class Torch : MonoBehaviour 
{
	bool turnedOn = false;
	public Light light;
	
	public void TurnOff()
	{
		SpriteAnimator animator = GetComponent<SpriteAnimator>();
		animator.PlayAnim("IdleOff");
		turnedOn = false;
		UpdateLightIntensity();
	}
	
	public void TurnOn()
	{
		SpriteAnimator animator = GetComponent<SpriteAnimator>();
		animator.PlayAnim("IdleOn");
		turnedOn = true;
		UpdateLightIntensity();
	}
	
	void UpdateLightIntensity()
	{
		if ( turnedOn )
			targetIntensity = 2.0f;
		else
			targetIntensity = 0f;
	}
	
	public void OutLineOfSight()
	{
		//renderer.enabled = false;
		if ( turnedOn )
			targetIntensity = 1.0f;
	}
	
	float targetIntensity;
	
	void Update()
	{
		light.intensity += (targetIntensity - light.intensity) * 0.1f;
	}
	
	public void InLineOfSight()
	{
		//renderer.enabled = true;
		UpdateLightIntensity();
	}
}
