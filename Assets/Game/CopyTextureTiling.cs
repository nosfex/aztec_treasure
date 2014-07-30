using UnityEngine;
using System.Collections;

public class CopyTextureTiling : MonoBehaviour 
{
	public Renderer copySource;
	public Transform position;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = position.position + (Vector3.up * 0.2f);
		transform.rotation = renderer.transform.rotation;
		renderer.material.mainTextureOffset = copySource.material.mainTextureOffset;
		renderer.material.mainTextureScale = copySource.material.mainTextureScale;
	}
}
