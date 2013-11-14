using UnityEngine;
using System.Collections;

public class DungeonBSP : MonoBehaviour
{
	public const int ROOM_WIDTH = 20;
	public const int ROOM_HEIGHT = 20;
	
	public int minRoomSize = 2;

	public GameObject roomHolder ;	
	public GameObject[,] tiles;
	public GameObject wallTile;
	public GameObject floorTile;
	public BSPNode trunk;
	
	public DungeonBSP()
	{
	
	}
	
	void Start()
	{
		init();
		
	}
	
	void Update()
	{
		
	}
	
	public void init()
	{
		
		tiles = new GameObject[ROOM_WIDTH, ROOM_HEIGHT];
		roomHolder = new GameObject("Room");
		Vector3 scale = wallTile.transform.localScale;
		for(int i = 0 ; i < ROOM_WIDTH; i++)
		{
			for (int j = 0 ; j < ROOM_HEIGHT; j++)
			{
				tiles[i, j] = (GameObject)(GameObject.Instantiate(wallTile));	
				tiles[i, j].transform.position = new Vector3(i * scale.x,  0, j * scale.z);
				tiles[i, j].transform.parent = roomHolder.transform;
			}
		}
		BSPNode root = null;
		trunk = root;
		insertNode(ref trunk, 25);
		
		insertNode(ref trunk, 8);
	
		insertNode(ref trunk, 5);
		insertNode(ref trunk, 10);
		insertNode(ref trunk, 8);
		insertNode(ref trunk, 5);
		insertNode(ref trunk, 23);
		
		
		partitionate(trunk, 0, 20, 0, 20);
	}
	
	public void partitionate(BSPNode root, int minW, int maxW, int minH, int maxH)
	{
		print("INIT PARTITIONING");
		
		BSPNode temp = root;
		Vector3 scale = wallTile.transform.localScale;
		if(temp != null)
		{
		/*	temp.room = new Room(maxW, maxH);
			for(int i = 0; i < maxW; i++)
			{
				for (int j = 0; j < maxH; j++)
				{
					temp.room.tiles[i,j] = tiles[temp.initPosX + i, temp.initPosY + j];
				}
				
			}*/
		
			int horizontalCut = Random.value * 4 > 2 ? 1 : 0;
			
			int hCut = 0;
			int vCut = 0;
			// GH: Initialize horizontal cut
			if(horizontalCut ==  1 && Mathf.Abs(minH - maxH) >= 4 )
			{
				vCut = Random.Range(minH, maxH);
				for(int i = minW ; i < maxW ; i++)
				{
					Destroy(tiles[i, vCut]);
					tiles[i, vCut] = (GameObject)(GameObject.Instantiate(floorTile));	
					tiles[i, vCut].transform.position = new Vector3(i *scale.x, 0, vCut * scale.z);
					tiles[i, vCut].transform.parent = roomHolder.transform;
					
					
					
				}
				if(temp.left != null && temp.left.weight <= temp.weight)
				{
					temp.left.initPosX = minW ;
					temp.left.initPosY = minH;
					temp.left.width = maxW;//Mathf.Abs( maxH - vCut);
					temp.left.height =  vCut;//Mathf.Abs( maxH - vCut);
					createRoom(temp, maxW, maxH);
					partitionate(temp.left, temp.left.initPosX , temp.left.width,  temp.left.initPosY,  temp.left.height);
				}	
				if(temp.right != null && temp.right.weight >= temp.weight)
				{
					temp.right.initPosX = minW ;
					temp.right.initPosY = vCut;
					temp.right.width = maxW;//Mathf.Abs( maxH - vCut);
					temp.right.height =  Mathf.Abs( maxH - vCut);
					createRoom(temp, maxW, maxH);
					partitionate(temp.right, temp.right.initPosX , temp.right.width,  temp.right.initPosY,  temp.right.height);
				}
			}
			else if(Mathf.Abs(minH - maxH) < 4)
			{
				print("COULDN'T MAKE AN HCUT");
			}
			// GH: Initialize vertical cut
			else if(Mathf.Abs(minW - maxW) >= 4)
			{
				hCut = Random.Range(minW, maxW);
				for(int i = minH ; i < maxH ; i++)
				{
					Destroy(tiles[hCut, i]);
					tiles[hCut, i] = (GameObject)(GameObject.Instantiate(floorTile));	
					tiles[hCut, i].transform.position = new Vector3(hCut * scale.x,  0, i * scale.z);
					tiles[hCut, i].transform.parent = roomHolder.transform;
					
				}
				
				if(temp.left != null && temp.left.weight <= temp.weight)
				{
					temp.left.initPosX = minW ;
					temp.left.initPosY = minH;
					temp.left.width = hCut;//Mathf.Abs( maxH - vCut);
					temp.left.height =  maxH;//Mathf.Abs( maxH - vCut);
					createRoom(temp, maxW, maxH);
					partitionate(temp.left, temp.left.initPosX , temp.left.width,  temp.left.initPosY,  temp.left.height);
					
				}
				if(temp.right != null && temp.right.weight >= temp.weight)
				{
					temp.right.initPosX = hCut ;
					temp.right.initPosY = minH;
					temp.right.width = Mathf.Abs( maxW - hCut);//Mathf.Abs( maxH - vCut);
					temp.right.height =  maxH;
					createRoom(temp, maxW, maxH);
					partitionate(temp.right, temp.right.initPosX , temp.right.width,  temp.right.initPosY,  temp.right.height);
					
				}
			}
			else if(Mathf.Abs(minW - maxW) < 4)
			{
				print("COULDN'T MAKE AN HCUT");
			}
			
			//int widthRange  = Random.Range(minRoomSize * 3, ROOM_WIDTH);
			
		}
		/*if(root.left != null)
		{
			print("bang");
			for(int i = 0; i < root.left.width ; i++)
			{
				for (int j = 0 ; j < root.left.height; j++)
				{
					Destroy(tiles[i,j]);
					tiles[i, j] = (GameObject)(GameObject.Instantiate(floorTile));	
					tiles[i, j].transform.position = new Vector3(i *100, 0, j * 100);
					tiles[i, j].transform.parent = roomHolder.transform;
				}
				
			}
		}*/
			
		
	}
	
	private void createRoom(BSPNode node, int maxW, int maxH)
	{
		node.room = new Room(maxW, maxH);
		Vector3 scale = wallTile.transform.localScale;
	/*	for(int i = 0; i < maxW; i++)
		{
			for (int j = 0; j < maxH; j++)
			{
				node.room.tiles[i,j] = tiles[node.initPosX + i, node.initPosY + j];
				Destroy(tiles[i, j]);
				tiles[i, j] = (GameObject)(GameObject.Instantiate(floorTile));	
				tiles[i, j].transform.position = new Vector3(i *scale.x, 0, j * scale.z);
				tiles[i, j].transform.parent = roomHolder.transform;
			}
			
		}*/
	}
	
	public void insertNode(ref BSPNode node, int val)
	{
		// GH: First generation node, the root
		print(BSPNode.instanceCount.ToString() + " NODES");
	
		if(node == null)
			
		{
			node = new BSPNode(val);
			node.left = node.right = null;
			return;
			
		}
		
		if(val < node.weight)
		{
			print("ADDING NODE TO THE LEFT");
			//node.left.parent = node;
			insertNode(ref node.left, val);
		}
		else if( val >= node.weight)
		{
			print("ADDING NODE TO THE RIGHT");
			//node.right.parent = node;
			insertNode(ref node.right, val);
		}
	}
}
