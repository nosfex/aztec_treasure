using UnityEngine;
using System.Collections;

public class GUITorch : MonoBehaviour {

	TextMesh text;
	public Renderer icon;
	// Use this for initialization
	void Start () 
	{
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		bool equipped = GameDirector.i.playerRight.hasLamp;
		renderer.enabled = equipped;
		icon.enabled = equipped;
		string t = "";
		
		t = "" + (int)GameDirector.i.playerRight.torchRatio + "%";
		
		text.text = t;
	}
}
