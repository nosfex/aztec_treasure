using UnityEngine;
using System.Collections;

public class BSPNode
{

	
	public BSPNode parent;
	public BSPNode left;
	public BSPNode right;
	public int weight;
	public int width = 0;
	public int height = 0;
	public int initPosX = 0;
	public int initPosY = 0;
	
	public Room room;

	public static int instanceCount =0 ;
	
	
	public BSPNode(int w)	
	{
		instanceCount++;
		weight = w;
		//initPosX = Vector2.zero;
		
	}
	
	public void createRoom(GameObject wallTile, GameObject floorTile)
	{
		
		room = new Room(width, height);
		Vector3 scale = wallTile.transform.localScale;
		for(int i = 0; i < width; i++)
		{
			for (int j = 0; j < height ; j++)
			{
				int tileX = initPosX + i;
				int tileY = initPosY + j;
				bool wall = false;
				if(i == 0 || i == width  - 1)
					wall = true;
				if(j == 0 || j == height -1)
					wall = true;
				
				room.tiles[i, j] = (GameObject)(GameObject.Instantiate(wall == false ? floorTile : wallTile));	
				room.tiles[i, j].transform.position = new Vector3(tileX * scale.x, scale.y * Room.refCount * 0, tileY * scale.z);
				room.tiles[i, j].transform.parent = room.roomHolder.transform;
				if(wall)
					room.addWall(room.tiles[i,j]);
				
			}
			
		}
		
	}
	
	public bool valueInRange(int value, int min, int max)
	{ return (value >= min) && (value <= max); }

	public bool rectOverlap(BSPNode b)
	{
	

	    bool xOverlap = valueInRange(initPosX, b.initPosX, b.initPosX + b.width) ||
	                    valueInRange(b.initPosX, initPosX, initPosX + b.width);
	
	    bool yOverlap = valueInRange(initPosY, b.initPosY, b.initPosY + b.height) ||
	                    valueInRange(b.initPosY, initPosY, initPosY + b.height);
		//bool overlap = !(initPosX + width < b.initPosX || initPosY + height < b.initPosY || initPosX > b.initPosX + b.width || initPosY > b.initPosY + b.height);
		bool overlap = (Mathf.Abs(initPosX - b.initPosX) < Mathf.Abs(width + b.width) / 2) && (Mathf.Abs(initPosY - b.initPosY) < Mathf.Abs(height + b.height) / 2);
	    return overlap;
			
	}
	
	public void tryToResize(GameObject wallTile, GameObject floorTile)
	{
		if(width > 7 || height > 7)
		{
			
			width = Random.Range(4, width);
			height = Random.Range(4, height);
			
			
		}
		
		
		
	}
	

}