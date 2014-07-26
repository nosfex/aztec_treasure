using UnityEngine;
using System.Collections;

public class WallDoorKeyTile : MonoBehaviour {

	public BaseObjectSensor playerSensor;
	public GameObject darkZone;
	// Use this for initialization
	void Start () {
		playerSensor.typeFilter = typeof( Player );
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( playerSensor.sensedObject != null )
		{
			if ( GameDirector.i.playerRight.keysCount > 0 )
			{
				GameDirector.i.playerRight.keysCount--;
				iTween.ColorTo ( darkZone, new Color(0,0,0,0), .5f );
				darkZone.transform.parent = transform.parent;
				//darkZone.SetActive( false );
				gameObject.SetActive( false );
			}
		}
	}
}
