using UnityEngine;
using System.Collections;

public class GUIHearts : MonoBehaviour {

	TextMesh text;
	// Use this for initialization
	void Start () 
	{
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		string t = "";
		
		for ( int i = 0; i < GameDirector.i.playerRight.hearts; i++ )
			t += "<3\n";
		
		text.text = t;
	}
}
