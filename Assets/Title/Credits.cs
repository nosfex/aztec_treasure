using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour 
{
	void Start() 
	{
		GetComponentInChildren<Camera>().tag = "MainCamera";
	}
	public float timer = 2.0f;
	void Update() 
	{
		Camera.main.transform.position += Camera.main.transform.forward * -0.002f;
		timer -= Time.deltaTime;
		if ( timer <= 0 )
		{
			Application.LoadLevel( 1 );
		}
	}
}
