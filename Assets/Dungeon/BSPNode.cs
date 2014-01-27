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
	
	public bool isAltar = false;
	
	public bool isEndRoom = false;
	public bool isStartRoom = false;

	public static int instanceCount = 0;
	
	
	public BSPNode(int w)	
	{
		instanceCount++;
		weight = w;
		//initPosX = Vector2.zero;
		
	}
	
	public void createRoom(DungeonBSP builder)
	{
		GameObject wallTile = builder.wallTile;
		GameObject floorTile = builder.floorTile;
		
		room = new Room(initPosX, initPosY, width, height);
		room.roomHolder = builder.gameObject;
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
				
				
				
				if ( !wall )
				{
					if ( isAltar && i == 3 && j == 3 )
					{
						GameObject o = (GameObject)Object.Instantiate( builder.altarPrefab );
						o.transform.position = new Vector3(tileX * scale.x, (scale.y * Room.refCount * 0) + 0.8f, tileY * scale.z);
						o.transform.parent = room.roomHolder.transform;
						//room.tiles[ i, j ] = o;
					}
					else
					if ( width >= 10 && height >= 10 )
					{
						Decoration deco = builder.PeekDecoration();
						
						int centeroffsetX = (width  / 2) - (deco.width / 2);
						int centeroffsetY = (height  / 2) - (deco.height / 2);
						
						//Debug.Log (" CX = " + centeroffsetX );
						//Debug.Log (" CY = " + centeroffsetY );
						if ( i == centeroffsetX && j == centeroffsetY )
						{
							if ( isEndRoom )
								deco = builder.endRoomDecoration;
							else if ( isStartRoom )
								deco = builder.startRoomDecoration;
							else 
								deco = builder.DrawDecoration();
							
							GameObject o = (GameObject)Object.Instantiate( deco.gameObject );
							o.transform.position = new Vector3(tileX * scale.x, scale.y * Room.refCount * 0, tileY * scale.z);
							o.transform.parent = room.roomHolder.transform;
							
							foreach ( BoxCollider bc in o.GetComponentsInChildren<BoxCollider>() )
							{
								Transform t = bc.transform;
								int x = Mathf.RoundToInt(t.localPosition.x / 0.8f);
								int y = Mathf.RoundToInt(t.localPosition.z / 0.8f);
								//Debug.Log (" x " + x + " ... y " + y, t );
								room.tiles[ x + i, y + j ] = t.gameObject;
							}
						}
					}
					
					if ( room.tiles[ i, j ] == null ) // Skip decoration if present...
					{
						room.tiles[i, j] = (GameObject)(Object.Instantiate(wall == false ? floorTile : wallTile));	
						room.tiles[i, j].transform.position = new Vector3(tileX * scale.x, scale.y * Room.refCount * 0, tileY * scale.z);
						room.tiles[i, j].transform.parent = room.roomHolder.transform;
						room.tiles[i, j].name = room.tiles[i, j].name.TrimEnd( "(Clone)" );
					}
				}
				//if(wall)
				//	room.addWall(room.tiles[i,j]);
				
			}
			
		}
		
	}
	
/*	public void createDoors(GameObject floorTile)
	{
		room.createDoors(initPosX, initPosY, floorTile);
	}*/
	
	public bool valueInRange(int value, int min, int max)
	{ 
		return (value >= min) && (value <= max); 
	}

	public bool rectOverlap(BSPNode b)
	{
		int padding = 3; 
		
		bool xOverlap = valueInRange(initPosX, b.initPosX, b.initPosX + b.width + padding) ||
	                    valueInRange(b.initPosX, initPosX, initPosX + width + padding );
	
	    bool yOverlap = valueInRange(initPosY, b.initPosY, b.initPosY + b.height + padding ) ||
	                    valueInRange(b.initPosY, initPosY, initPosY + height + padding );
		//bool overlap = !(initPosX + width < b.initPosX || initPosY + height < b.initPosY || initPosX > b.initPosX + b.width || initPosY > b.initPosY + b.height);
		bool overlap = //(Mathf.Abs(initPosX - b.initPosX) * 2 <= Mathf.Abs(width + b.width) ) && (Mathf.Abs(initPosY - b.initPosY) * 2 <= Mathf.Abs(height + b.height) );
			initPosX < b.initPosX + b.width + padding &&
			initPosX + width + padding > b.initPosX  &&
			initPosY < b.initPosY + b.height + padding &&
			initPosY + height + padding > b.initPosY;
		
	    return overlap;
			
	}
	
	public void tryToResize(GameObject wallTile, GameObject floorTile)
	{
		int maxWidth = (int)DungeonBSP.WORLD_TILE_WIDTH / 2; 
		int maxHeight = (int)DungeonBSP.WORLD_TILE_HEIGHT / 2; 
		
		if(width >= maxWidth )
		{
			width = maxWidth;
		}
		
		if(height >= maxHeight)
		{
			height = maxHeight;
		}
			
		if(Mathf.Abs(width - height) < 2)
		{
		
			if(width >= 15)
			{
				width = 15;
			}
			
			if(height >= 15)
			{
				height = 15;
			}
			width = height;
			return;
		}
		
		if(width > height)
		{
			height = Random.Range(6, 8);
			return;
		}
		
		if(width < height)
		{
			width = Random.Range(6,8);
			return;
		}
		
	}
	

}