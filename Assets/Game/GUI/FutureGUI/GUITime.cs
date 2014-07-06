using UnityEngine;
using System.Collections;

public class GUITime : MonoBehaviour {

	TextMesh text;
	// Use this for initialization
	void Start () 
	{
		text = GetComponent<TextMesh>();
	}
	float time;
	// Update is called once per frame
	void Update () 
	{
		string t = "";
		
		t = "" + (int)time;
		time += Time.deltaTime;
		
		text.text = t;
	}
}
