﻿using UnityEngine;
using System.Collections;

public class GUITorch : MonoBehaviour {

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
		
		t = "Torch: " + (int)GameDirector.i.playerRight.torchRatio + "%";
		
		text.text = t;
	}
}