using UnityEngine;
using System.Collections;

public class DungeonBSP : MonoBehaviour
{
	public const int ROOM_WIDTH = 20;
	public const int ROOM_HEIGHT = 20;
	
	public int minRoomSize = 6;

	public GameObject roomHolder ;	
	public GameObject[,] tiles;
	public GameObject wallTile;
	public GameObject floorTile;
	public BSPNode trunk;
	public System.Collections.ArrayList r;
	
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
		minRoomSize = 6;
		
		tiles = new GameObject[ROOM_WIDTH, ROOM_HEIGHT];
		roomHolder = new GameObject("Room");
		
		BSPNode root = null;
		trunk = root;
		insertNode(ref trunk, 0);
		
		root = trunk;
		r = new ArrayList();
		trunk.width = ROOM_WIDTH;
		trunk.height= ROOM_HEIGHT;
		
		//createRoom(trunk, ROOM_WIDTH, ROOM_HEIGHT);
		while(partitionate(trunk, trunk.initPosX, trunk.width , trunk.initPosY, trunk.height ))
		{
			int index = Random.Range(0, r.Count - 1);
			trunk = (BSPNode)r.ToArray()[index];
			r.RemoveAt(index);
		}
		trunk = root;
		createRoomsFromRoot(trunk);
		
	}
	
	public bool partitionate(BSPNode root, int minW, int maxW, int minH, int maxH)
	{
		print("INIT PARTITIONING: " + "W: MIN: " + minW.ToString() + " MAX: " + maxW.ToString() + "\n\t H: MIN: " + minH.ToString() + " MAX: " + maxH.ToString() );
		
		BSPNode temp = root;
		print("MIN ROOM SIZEW:" + minRoomSize.ToString() );
		Vector3 scale = wallTile.transform.localScale;
		if(Room.refCount >= 20)
		{
			print("STOPPED AT REF COUNT");
			return false;
		}
		
			//createRoom(temp, maxW, maxH);
		
		
		bool horizontalCut = Random.Range(0.0f, 20.0f) < 10.0f ? true : false;
		
	
		int cut = 0;
		
		
		int max = (horizontalCut ?  maxW : maxH) - minRoomSize;
		if(max <= minRoomSize)
		{
			print("CAN'T CUT");
			return false;
		}
		// GH: Initialize horizontal cut
		if(horizontalCut)
		{
			cut = Random.Range(minRoomSize, maxH);
			
			if( maxH - cut < minRoomSize)
			{
				cut = minRoomSize;	
			}
			
			if(temp.left != null)
				return false;
			
			print("HORIZONTAL CUT");
			temp.left = new BSPNode(0);
			r.Add(temp.left);
			generateCut(temp.left, minW, minH, maxW , cut);
			
			temp.right = new BSPNode(0);
			r.Add(temp.right);
			int variant = Mathf.Abs(cut - maxH);
		
			generateCut(temp.right, minW, minH + cut, maxW, variant);
		
		 	//return false;
		} 
		else
		{
			cut = Random.Range(minRoomSize, maxW);
		
			if( maxW - cut < minRoomSize)
			{
				cut = minRoomSize;	
			}
		
			if(temp.left != null)
				return false;
			
			print("VERTICAL CUT");
			temp.left = new BSPNode(0);
			r.Add(temp.left);
			generateCut(temp.left, minW, minH, cut, maxH);
		
			temp.right = new BSPNode(0);
			r.Add(temp.right);
			int variant = Mathf.Abs(cut - maxW);
			generateCut(temp.right, minW + cut, minH, variant, maxH);
		
			//return false;
		}	
		return true;
		
		
			
	}
	
	private void generateCut(BSPNode node, int fromX, int fromY, int width, int height)
	{
		print("CUT X: " + fromX.ToString() + " Y: " + fromY.ToString() + " W: " + width.ToString() + " H: " + height.ToString() );
		node.initPosX = fromX;
		node.initPosY = fromY;
		node.width 	  = width;
		node.height   = height;
		
	}
	
	private void createRoom(BSPNode node, int maxW, int maxH)
	{
		
		
		node.room = new Room(maxW, maxH);
		Vector3 scale = wallTile.transform.localScale;
		for(int i = 0; i < maxW; i++)
		{
			for (int j = 0; j < maxH ; j++)
			{
				int tileX = node.initPosX + i;
				int tileY = node.initPosY + j;
				bool wall = false;
				if(i == 0 || i == maxW  - 1)
					wall = true;
				if(j == 0 || j == maxH -1)
					wall = true;
				
				node.room.tiles[i, j] = (GameObject)(GameObject.Instantiate(wall == false ? floorTile : wallTile));	
				node.room.tiles[i, j].transform.position = new Vector3(tileX * scale.x, scale.y * Room.refCount * 0, tileY * scale.z);
				node.room.tiles[i, j].transform.parent = node.room.roomHolder.transform;
				
			}
			
		}
	}
	
	private void createRoomsFromRoot(BSPNode root)
	{
		BSPNode temp = root;
		while(temp.left != null)
		{
			temp = temp.left;
			createRoom(temp, temp.width, temp.height);
		}
		temp = root;
		while(temp.right != null)
		{
			temp = temp.right;
			createRoom(temp, temp.width, temp.height);
		}
			
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
		
		if(val < 0)
		{
			print("ADDING NODE TO THE LEFT");
			//node.left.parent = node;
			insertNode(ref node.left, val);
		}
		else if( val > 0)
		{
			print("ADDING NODE TO THE RIGHT");
			
			insertNode(ref node.right, val);
		}
	}
}
