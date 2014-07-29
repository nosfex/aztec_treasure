using UnityEngine;
using System.Collections;

public class ScrollUVs : MonoBehaviour 
{	
	public Material material;
	void Start () 
	{
	
	}

	public Vector2 scrollBy;
	
	void Update () 
	{
		material.mainTextureOffset += scrollBy;
	
	}
}
