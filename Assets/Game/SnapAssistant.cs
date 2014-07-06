using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapAssistant : MonoBehaviour 
{

	private static SnapAssistant instance;
	
	public static SnapAssistant i { get { return instance; } }
	
	void Awake() 
	{
		instance = this;
	}
	public Vector3 snapOffset;
	public float snapSize = 0.2f;
	public bool snapEnabled = true;
	public bool useOffset = false;
	
	void Start() 
	{
		
	}
	
	
	void Update() 
	{
	
	}
}
