using UnityEngine;
using System.Collections;

public class Room
{

	// Use this for initialization
	

	public GameObject roomHolder;	
	public GameObject[,] tiles;
	int width;
	int height;
	public static int refCount =0;
	
	public Room(int w, int h)
	{
		width = w;
		height = h;
		roomHolder = new GameObject("roomInst"+refCount.ToString() );
		tiles = new GameObject[w, h];
		//Start();
		cutRoom();
		refCount++;
	}
	
	public void cutRoom()
	{
		for(int i = 0 ; i < width; i++)
		{
			for(int j = 0; j <height; j++)
			{
				MonoBehaviour.Destroy(tiles[i,j]);
				
			}	
		}
	}
	
	// Update is called once per frame
	
}
