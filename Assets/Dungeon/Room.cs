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
	
	public ArrayList wallList;
	
	public Room(int w, int h)
	{
		refCount++;
		width = w;
		height = h;
		roomHolder = new GameObject("roomInst"+refCount.ToString() );
		tiles = new GameObject[w, h];
		//Start();
		//cutRoom();
		wallList = new ArrayList();
	}
	
	public void deleteRoom()
	{
		for(int i = 0 ; i < width; i++)
		{
			for(int j = 0; j <height; j++)
			{
				MonoBehaviour.Destroy(tiles[i,j]);
				
			}	
		}
		
		
	//	MonoBehaviour.Destroy(tiles);
	//	tiles = null;
		MonoBehaviour.Destroy(roomHolder);
		roomHolder = null;
	}
	
	
	public void addWall(GameObject wall)
	{
		wallList.Add(wall);
		
	}
	
	
	// Update is called once per frame
	
}
