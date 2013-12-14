using UnityEngine;
using System.Collections;

public class GUICompass : MonoBehaviour {
	
	public TextMesh distanceText;
	public GameObject arrow;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( GameDirector.i.playerRight == null )
			return;

		if ( GameDirector.i.finalTreasureRight == null )
			return;
		
		Vector3 dif = GameDirector.i.finalTreasureRight.transform.position - GameDirector.i.playerRight.transform.position;
		
		distanceText.transform.position = arrow.transform.position + Vector3.up * 0.4f;
		distanceText.text = dif.magnitude.ToString("N2") + "mts";
		
		
		float angle = Mathf.Atan2 ( dif.z, dif.x ) * Mathf.Rad2Deg;
		
		transform.localRotation = Quaternion.Euler( 55, 0, angle );
	}
}
