using UnityEngine;
using System.Collections;

public class GUIAltars : MonoBehaviour 
{
	public static GUIAltars i { get { return instance; } }
	private static GUIAltars instance;

	[HideInInspector] public int altarsCount = 0;
	[HideInInspector] public int altarsFound = 0;
	
	TextMesh text;
	// Use this for initialization
	void Start () 
	{
		instance = this;
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		string t = "";
		
		//for ( int i = 0; i < GameDirector.i.playerRight.hearts; i++ )
		//	t += "<3\n";
		t += "Altars Found " + altarsFound + "/" + altarsCount;
		
		text.text = t;
	}
}
