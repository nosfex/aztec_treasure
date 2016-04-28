using UnityEngine;
using System.Collections;

public class Lamplight : MonoBehaviour 
{
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", new Color( 1.0f, 1.0f, 1.0f, Mathf.Clamp01 ( GameDirector.i.playerRight.torchRatio / 100f ) * 0.3f ) );
	}
}
