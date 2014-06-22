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

		arrowVisible = true;
		fading = true;
	}
	
	void Start()
	{
		Color targetColor = new Color( 1,1,1, arrowVisible ? 1 : 0 );
		
		distanceText.renderer.material.color = targetColor;
		arrow.renderer.material.SetColor ("_Emission", targetColor );
		arrow.renderer.material.color = targetColor;
	}
	
	void Update () 
	{
		if ( Input.GetKey( "p" ) )
			TurnOn ();

		if ( Input.GetKey( "o" ) )
			TurnOff ();
			
		if ( fading )
		{
			Color targetColor = new Color( 1,1,1, arrowVisible ? 1 : 0 );
			
			distanceText.renderer.material.color = Color.Lerp ( distanceText.renderer.material.color, targetColor, 0.1f );
			arrow.renderer.material.SetColor ("_Emission", Color.Lerp ( arrow.renderer.material.GetColor("_Emission"), targetColor, 0.1f ) );
			arrow.renderer.material.color = Color.Lerp ( arrow.renderer.material.color, targetColor, 0.1f );
		
			if ( distanceText.renderer.material.color == targetColor )
				fading = false;
		}
		
		if ( GameDirector.i.playerRight == null )
			return;

		if ( GameDirector.i.finalTreasureRight == null )
			return;
		
		Vector3 dif = GameDirector.i.finalTreasureRight.transform.position - GameDirector.i.playerRight.transform.position;
		float distance = dif.magnitude;
		
		distanceText.transform.position = arrow.transform.position + Vector3.up * 20f;
		distanceText.text = distance.ToString("N2") + "mts";
		
		arrow.renderer.enabled = distance > 3.0f;
		distanceText.renderer.enabled = distance > 3.0f;
		
		float angle = Mathf.Atan2 ( dif.z, dif.x ) * Mathf.Rad2Deg;
		
		transform.localRotation = Quaternion.Euler( 0, 0, angle );
	}
}
