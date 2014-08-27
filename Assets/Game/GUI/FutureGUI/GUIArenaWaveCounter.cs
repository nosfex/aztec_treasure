using UnityEngine;
using System.Collections;

public class GUIArenaWaveCounter : MonoBehaviour {

	// Use this for initialization
	TextMesh txtHandle;
	ArenaController arenaHandle;
	void Start () 
	{
		txtHandle = this.gameObject.GetComponent<TextMesh>();
		arenaHandle = GameDirector.i.worldRight.GetComponentInChildren<ArenaController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		txtHandle.text = "Wave: " + (arenaHandle.currentMajorActive + 1).ToString() + " - " +  (arenaHandle.currentMinorActive + 1).ToString();
	}
}
