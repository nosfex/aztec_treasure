using UnityEngine;
using System.Collections;

public class GUICompass : MonoBehaviour 
{
	public TextMesh distanceText;
	public GameObject arrow;
	
	bool arrowVisible = false;
	bool fading = false;
	
	void TurnOff()
	{
		if ( !arrowVisible )
			return;
		
		arrowVisible = false;
		fading = true;
	}
	
	void TurnOn()
	{
		if ( arrowVisible )
			return;
		Vector3 dif = GameDirector.i.finalTreasureRight.transform.position - GameDirector.i.playerRight.transform.position;
		float angleDest = Mathf.Atan2 ( dif.z, dif.x ) * Mathf.Rad2Deg;
		angle = angleDest + (90 * (Random.Range(1,3) * 2 - 3));
		easeTimer = 0;
		arrowVisible = true;
		fading = true;
	}
	
	void Start()
	{
		Color targetColor = new Color( 1,1,1, arrowVisible ? 1 : 0 );
		
		distanceText.renderer.material.color = targetColor;
		arrow.renderer.material.color = targetColor;
	}
	
	
	float onTimer = 0;
	float offTimer = 0;
	float angle;
	float easeTimer = 0;
	void Update () 
	{
		bool isPlayerStanding = GameDirector.i.playerRight.velocity.magnitude < 0.01f;
		
		//isPlayerStanding = false;
		
		if ( arrowVisible )
		{
			if ( isPlayerStanding )
				offTimer = 0;
			else 
				offTimer += Time.deltaTime;
			
			if ( offTimer > 2.0f )
				TurnOff ();
		}
		else 
		{
			if ( isPlayerStanding )
				onTimer += Time.deltaTime;
			else
				onTimer = 0;
			
			if ( onTimer > 2.0f )
				TurnOn ();
		}
			
		
		if ( fading )
		{
			Color targetColor = new Color( 1,1,1, arrowVisible ? 1 : 0 );
			
			distanceText.renderer.material.color = Color.Lerp ( distanceText.renderer.material.color, targetColor, 0.1f );
			arrow.renderer.material.color = Color.Lerp ( arrow.renderer.material.color, targetColor, 0.1f );
		
			if ( distanceText.renderer.material.color == targetColor )
				fading = false;
		}
		
		if ( GameDirector.i.playerRight == null )
			return;

		if ( GameDirector.i.finalTreasureRight == null )
			return;
		
		Vector3 dif = GameDirector.i.finalTreasureRight.transform.position - GameDirector.i.playerRight.transform.position;
		float angleDest = Mathf.Atan2 ( dif.z, dif.x ) * Mathf.Rad2Deg;
		float distance = dif.magnitude;
		
		distanceText.transform.position = arrow.transform.position + Vector3.up * 20f;
		distanceText.text = distance.ToString("N2") + "mts";
		
		arrow.renderer.enabled = distance > 3.0f;
		distanceText.renderer.enabled = distance > 3.0f;
		
		easeTimer += 0.008f;
		

//		v = easeOutElastic( angle, angleDest, easeTimer );
//		prevV = v;
//
//		float delta = v - prevV;
//		
		
		
		transform.localRotation = Quaternion.Euler( 0, 0, easeOutElastic( angle, angleDest, easeTimer )  );
	}
	
	float v, prevV;
	
	private float easeOutElastic(float start, float end, float value)
	{
	/* GFX47 MOD END */
		//Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0) return start;
		
		if ((value /= d) == 1) return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)){
			a = end;
			s = p * 0.25f;
			}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -15 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}
}
