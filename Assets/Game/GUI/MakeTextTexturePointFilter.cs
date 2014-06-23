using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TextMesh))]
public class MakeTextTexturePointFilter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TextMesh text = GetComponent<TextMesh>();
		text.font.material.mainTexture.filterMode = FilterMode.Point;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
