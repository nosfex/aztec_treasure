using UnityEngine;
using System.Collections;

public class TimeCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		textMesh = GetComponent<TextMesh>();
	}
	TextMesh textMesh;
	float timer;
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		textMesh.text = timer.ToString("N2");
	}
}
