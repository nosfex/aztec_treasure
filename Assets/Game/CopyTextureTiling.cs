using UnityEngine;
using System.Collections;

public class CopyTextureTiling : MonoBehaviour 
{
	public Renderer copySource;
	public Player p;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = p.transform.position + (Vector3.up * 0.2f) + (p.direction * 0.3f);
		transform.rotation = GetComponent<Renderer>().transform.rotation;
		GetComponent<Renderer>().material.mainTextureOffset = copySource.material.mainTextureOffset;
		GetComponent<Renderer>().material.mainTextureScale = copySource.material.mainTextureScale;
	}
}
