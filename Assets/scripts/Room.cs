using UnityEngine;
using System.Collections;

public class Room
{

	// Use this for initialization
	

	public GameObject roomHolder;	
	public GameObject[,] tiles;

	
	public Room(int w, int h)
	{
	
		tiles = new GameObject[w, h];
		//Start();
	}
	
	public void Start () 
	{
	
	}
	
	// Update is called once per frame
	
}
