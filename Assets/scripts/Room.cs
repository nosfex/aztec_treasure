using UnityEngine;
using System.Collections;

public class Room
{

	// Use this for initialization
	

	public GameObject roomHolder;	
	public GameObject[,] tiles;
	int width;
	int height;
	
	public Room(int w, int h)
	{
		width = w;
		height = h;
		tiles = new GameObject[w, h];
		//Start();
	}
	
	public void cutRoom(GameObject tiles)
	{
		for(int i = 0 ; i < width; i++)
		{
			for(int j = 0; j <height; j++)
			{
				//MonoBehaviour.Destroy(tiles[i]
				
			}	
		}
	}
	public void Start () 
	{
	
	}
	
	// Update is called once per frame
	
}
