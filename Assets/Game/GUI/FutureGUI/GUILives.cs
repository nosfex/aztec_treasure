using UnityEngine;
using System.Collections;

public class GUILives : MonoBehaviour {
	
	private TextMesh text;
	// Use this for initialization
	void Start () {
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "x" + GameDirector.i.playerRight.lives;	
	}
}
