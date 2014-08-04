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
		transform.rotation = renderer.transform.rotation;
		renderer.material.mainTextureOffset = copySource.material.mainTextureOffset;
		renderer.material.mainTextureScale = copySource.material.mainTextureScale;
	}
}
