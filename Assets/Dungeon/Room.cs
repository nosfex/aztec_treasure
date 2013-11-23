using UnityEngine;
using System.Collections;

public class Room
{

	// Use this for initialization
	

	public GameObject roomHolder;	
	public GameObject[,] tiles;
	public int width;
	public int height;
	public int x;
	public int y;
	public static int refCount =0;
	
	
	public const int	TOP=0;
	public const int	BOTTOM=1;
	public const int	RIGHT=2;
	public const int	LEFT=3;

	public ArrayList doors;
	public ArrayList wallList;
	
	public Room(int _x, int _y, int w, int h)
	{
		refCount++;
		width = w;
		height = h;
		
		x = _x;
		y = _y;
		roomHolder = new GameObject("roomInst"+refCount.ToString() );
		tiles = new GameObject[w, h];
		//Start();
		//cutRoom();
		wallList = new ArrayList();
		doors = new ArrayList();
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
	
		MonoBehaviour.Destroy(roomHolder);
		roomHolder = null;
	}
	
	
/*	public void createDoors(int iPosX, int iPosY, GameObject floorTile) 
	{
		Vector3 scale = floorTile.transform.localScale;
		int maxDoors = Random.Range(1, 4);
		
		doors = new bool[4];
		
		for(int i = 0; i < 4 ; i++)
		{
			doors[i] = false;			
		}
		int index = -1;
		while(maxDoors != 0)
		{
			int i = Random.Range(1, width -2);
			int j = Random.Range(1, height - 2);
			bool iAxis = false;
			if(i >= j)
			{
				j = Random.Range(0, height ) > height / 2 ? height -1 : 0;
				
				if(j > height / 2)
					index = BOTTOM;
				else index = TOP;
			}
			else
			{
				i = Random.Range(0, width) > width / 2 ? width -1 : 0;
				iAxis = true;
				
				if(i > width / 2)
					index = RIGHT;
				else index = LEFT;
			}
			
			if(doors[index])
				continue;
			
			int tileX = iPosX + i;
			int tileY = iPosY + j;
		
			int modX = iAxis ? 0 : 1;
			int modY = iAxis ? 1 : 0;
			
			
			
			wallList.Remove(tiles[i,j]);
		//	wallList.Remove(tiles[i + modX ,j + modY]);
			replaceTile(floorTile, i, j, tileX, tileY);
		//	replaceTile(floorTile, i + modX, j + modY, tileX + modX, tileY + modY);
			maxDoors--;
		}
		
	}*/
	
	public void createDoorAtSide(int side, GameObject tile)
	{
		int i = 0; 
		int j = 0;
		bool iAxis = false;
		switch(side)
		{
			case TOP:
				j = height -1;
				iAxis = true;
			break;
			case LEFT:
				i = 0;
				
			break;
			case BOTTOM:
				j = 0;
				iAxis = true;
			break;
			case RIGHT:
				i = width - 1;
			break;
		}
		
		if(iAxis)
		{
			i = Random.Range(1, width - 2);
		}
		else
		{
			j = Random.Range(1, height - 2);
		}
		
		
		DoorData d = new DoorData();
		d.colRow = new Vector2(i, j);
		
		
		DungeonBSP.doors.Add(d);
		int tileX = i + x;
		int tileY = j + y;
		
		d.side = side;
		doors.Add(d);
		
		replaceTile(tile, i, j, tileX, tileY);
	}
	
	public bool checkSideFree(int side)
	{
		for(int i  = 0; i < doors.Count; i++)
		{
			DoorData d = (DoorData)doors.ToArray()[i];
			if(d.side == side)
			{
				return false;
			}
			
		}
		return true;
	}
	
	
	public void replaceTile(GameObject tile, int i, int j, int _x, int _y)
	{
		MonoBehaviour.Destroy(tiles[i, j]);	
		Vector3 scale = tile.transform.localScale;
		tiles[i, j] = (GameObject)(GameObject.Instantiate(tile));	
		tiles[i, j].transform.position = new Vector3(_x * 0.8f, scale.y * Room.refCount * 0, _y * 0.8f);
		tiles[i, j].transform.parent = roomHolder.transform;
	}
	
	public void addWall(GameObject wall)
	{
		wallList.Add(wall);
		
	}
	
	public GameObject getTile(int _x, int _y)
	{
		return tiles[_x, _y];
	}
	
	
	public DoorData getLastDoor()
	{
		return (DoorData)doors[doors.Count -1];
	}
	public Vector2 getCenter()
	{
		
		return new Vector2(x + width / 2, y + height / 2);
	}
	
	// Update is called once per frame
	
}
