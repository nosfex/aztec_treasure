using UnityEngine;
using System.Collections;

public class DarkZone : MonoBehaviour {

	// Use this for initialization
	Color origColor;
	bool inside = false;
	float insideTimer = 1.0f;
	public GameObject objectToHide;
	void Start () 
	{
		origColor = RenderSettings.ambientLight;	
		BaseObjectSensor sensor = GetComponent<BaseObjectSensor>();
		sensor.typeFilter = typeof( Player );
	}
	
	// Update is called once per frame
	void Update () {
		BaseObjectSensor sensor = GetComponent<BaseObjectSensor>();
		bool found = false;
		if ( sensor.sensedObject != null )
		{
			found = true;
		}
		
		if ( found )
		{
			RenderSettings.ambientLight = Color.Lerp( RenderSettings.ambientLight, Color.black, 0.01f );

			if ( objectToHide != null )
				objectToHide.SetActive( false );

			inside = true;
			
			if ( GameDirector.i.playerRight.hasLamp )
				GameDirector.i.playerRight.darknessMechanic = true;
		}
		else if ( inside )
		{

			insideTimer += Time.deltaTime;

			if ( insideTimer > 3.0f )
			{
				GameDirector.i.playerRight.darknessMechanic = false;
				inside = false;
				insideTimer = 0;
				if ( objectToHide != null )
					objectToHide.SetActive( true );
			}
			
			RenderSettings.ambientLight = Color.Lerp( RenderSettings.ambientLight, origColor, 0.01f );
		}
	}
}
