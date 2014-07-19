using UnityEngine;
using System.Collections;

public class CopyTextureTiling : MonoBehaviour 
{
	public Renderer copySource;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.mainTextureOffset = copySource.material.mainTextureOffset;
		renderer.material.mainTextureScale = copySource.material.mainTextureScale;
	}
}
