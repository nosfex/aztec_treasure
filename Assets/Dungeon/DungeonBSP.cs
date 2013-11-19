using UnityEngine;
using System.Collections;

public class DungeonBSP : MonoBehaviour
{
	public const int ROOM_WIDTH = 100;
	public const int ROOM_HEIGHT = 100;
	
	public int minRoomSize = 12;

	
	public GameObject[,] tiles;
	public GameObject wallTile;
	public GameObject floorTile;
	public BSPNode trunk;
	public System.Collections.ArrayList r;
	public System.Collections.ArrayList final;
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
		minRoomSize = 12;
		
		tiles = new GameObject[ROOM_WIDTH, ROOM_HEIGHT];
		
		
		BSPNode root = null;
		trunk = root;
		insertNode(ref trunk, 0);
		
		root = trunk;
		r = new ArrayList();
		trunk.width = ROOM_WIDTH;
		trunk.height = ROOM_HEIGHT;
		final = new ArrayList();
		//createRoom(trunk, ROOM_WIDTH, ROOM_HEIGHT);
		while(partitionate(trunk, trunk.initPosX, trunk.width , trunk.initPosY, trunk.height ))
		{
			int index = Random.Range(0, r.Count - 1);
			trunk = (BSPNode)r.ToArray()[index];
			r.RemoveAt(index);
		}
		while(r.Count != 0)
		{
			int index = Random.Range(0, r.Count - 1);
			trunk = (BSPNode)r.ToArray()[index];
			r.RemoveAt(index);
			partitionate(trunk, trunk.initPosX, trunk.width , trunk.initPosY, trunk.height );
			
			
		}
		trunk = root;
		createRoomsFromRoot(trunk);
	
 		resizeNodes();
		
		removeOverlappedNodes();
		removeOverlappedNodes();
		
		
		for(int i = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			a.createRoom(wallTile, floorTile);
		}
	//	resizeNodes();
	}
	
	void removeOverlappedNodes()
	{
		
		for(int i = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			for(int j = final.Count -1; j >= 0; j--)
			{
				
				BSPNode b = (BSPNode)final.ToArray()[j];
				if(a == b)
					continue;
				if(b.rectOverlap(a))
				{
					/*if(b.width + b.height > a.width + a.height)
					{
						b.room.deleteRoom();
						final.Remove(b);
						
					}*/
					//b.room.deleteRoom();
						final.Remove(b);
					
				}
			}	
		}
	}
	
	public bool partitionate(BSPNode root, int minW, int maxW, int minH, int maxH)
	{
		BSPNode temp = root;
		
		if(BSPNode.instanceCount >= 800)
		{
			print("STOPPED AT REF COUNT");
			return false;
		}
		
			//createRoom(temp, maxW, maxH);
		
		
		bool horizontalCut = Random.Range(0.0f, 20.0f) < 10.0f ? true : false;
		
	
		int cut = 0;
		
		
		int max = (horizontalCut ?  maxW : maxH) ;
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
			temp.left.parent = temp;
			r.Add(temp.left);
			
			generateCut(temp.left, minW, minH, maxW , cut);
			
			temp.right = new BSPNode(0);
			temp.right.parent = temp;
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
			temp.left.parent = temp;
			r.Add(temp.left);
			
			generateCut(temp.left, minW, minH, cut, maxH);
		
			temp.right = new BSPNode(0);
			temp.right.parent = temp;
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
		
		if(maxW < minRoomSize)
			return;
		if(maxH < minRoomSize)
			return;
		
		//maxW = Random.Range(minRoomSize, maxW);
		//maxH = Random.Range(minRoomSize, maxH);
		print("INIT PARTITIONING: " + "W: MIN: " + node.initPosX.ToString() + " MAX: " + maxW.ToString() + "\n\t H: MIN: " + node.initPosY.ToString() + " MAX: " + maxH.ToString() );
		
		//node.room = new Room(maxW, maxH);
		
		
		
		for(int i = 0; i < final.Count ; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			if(a.initPosX == node.initPosX && a.initPosY == node.initPosY && a.width == node.width && a.height == node.height)
				return;
			
		/*	if(a.rectOverlap(node))
				return;*/
			
		}
		//node.createRoom(wallTile, floorTile);
		final.Add(node);
	
		
	}
	
	private void createRoomsFromRoot(BSPNode root)
	{
		BSPNode temp = root;
		while(temp.left != null)
		{
			temp = temp.left;
			
		}
		
		while(temp.parent != null)
		{
			
			temp = temp.parent;	
			createRoom(temp.left, temp.left.width, temp.left.height);
			createRoom(temp.right, temp.right.width, temp.right.height);
			
			
		}
		temp = root;
		while(temp.right != null)
		{
			temp = temp.right;
			
		}
		
		while(temp.parent != null)
		{
			temp = temp.parent;
			createRoom(temp.left, temp.left.width, temp.left.height);
			createRoom(temp.right, temp.right.width, temp.right.height);
		}
			
	}
	
	public void resizeNodes()
	{
		for(int i  = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			a.tryToResize(wallTile, floorTile);
		}
		
	}
	
	public void generateDoors() 
	{
		for(int i = 0; i < final.Count ; i++)
		{
			for(int j = 0; j < final.Count ; j++)
			{
				BSPNode a = (BSPNode)final.ToArray()[i];
				BSPNode b = (BSPNode)final.ToArray()[j];
				//if(a.rectOverlap())
				if(a.rectOverlap(b))
				{
					
					
				}
			}
			
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
