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
	
	
	public void createDoors(int iPosX, int iPosY, GameObject floorTile) 
	{
		Vector3 scale = floorTile.transform.localScale;
		int maxDoors = Random.Range(1, 4);
		
		
				
			
		while(maxDoors != 0)
		{
			int i = Random.Range(1, width -2);
			int j = Random.Range(1, height - 2);
			bool iAxis = false;
			if(i >= j)
			{
				j = Random.Range(0, height ) > height / 2 ? height -1 : 0;
				
			}
			else
			{
				i = Random.Range(0, width) > width / 2 ? width -1 : 0;
				iAxis = true;
			}
					
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
		
	}
	
	public void replaceTile(GameObject tile, int i, int j, int x, int y)
	{
		MonoBehaviour.Destroy(tiles[i, j]);	
		Vector3 scale = tile.transform.localScale;
		tiles[i, j] = (GameObject)(GameObject.Instantiate(tile));	
		tiles[i, j].transform.position = new Vector3(x * 0.8f, scale.y * Room.refCount * 0, y * 0.8f);
		tiles[i, j].transform.parent = roomHolder.transform;
	}
	
	public void addWall(GameObject wall)
	{
		wallList.Add(wall);
		
	}
	
	public GameObject getTile(int x, int y)
	{
		return tiles[x, y];
	}
	
	// Update is called once per frame
	
}
