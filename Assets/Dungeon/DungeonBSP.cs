﻿using UnityEngine;
using System.Collections;

public class DungeonBSP : MonoBehaviour
{
	public const int ROOM_WIDTH = 60;
	public const int ROOM_HEIGHT = 60;
	
	public int minRoomSize = 9;

	
	
	public static ArrayList doors = new ArrayList();

	public int[,] globalTiles;
	public GameObject wallTile;
	public GameObject floorTile;
	public GameObject doorTile;
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
	
	int bailLimit = 20;
	
	BSPNode GenerateHRoom()
	{
		BSPNode node = new BSPNode(0);
		int bailout = bailLimit;
		do
		{
			bailout--;
			if ( bailout == 0 ) return null;
			
			node.width = Random.Range(10, 20);
			node.height = Random.Range(5, 6);
			node.initPosX = Random.Range (node.width, ROOM_WIDTH-node.width);
			node.initPosY = Random.Range (node.height, ROOM_HEIGHT-node.height);
		}
		while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;		
		
		return node;
	}

	BSPNode GenerateVRoom()
	{
		BSPNode node = new BSPNode(0);
		int bailout = bailLimit;
		do
		{
			bailout--;
			if ( bailout == 0 ) return null;
			node.width = Random.Range(5, 6);
			node.height = Random.Range(10, 20);
			node.initPosX = Random.Range (node.width, ROOM_WIDTH-node.width);
			node.initPosY = Random.Range (node.height, ROOM_HEIGHT-node.height);
		}
		while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;	
		
		return node;
	}
	
	BSPNode GenerateBigRoom()
	{
		BSPNode node = new BSPNode(0);
		int bailout = bailLimit;
		do
		{
			bailout--;
			if ( bailout == 0 ) return null;
		
			int size = Random.Range ( 15, 20 );
			node.width = size + Random.Range(-3, 3);
			node.height = size + Random.Range(-3, 3);
			node.initPosX = Random.Range (node.width, ROOM_WIDTH-node.width);
			node.initPosY = Random.Range (node.height, ROOM_HEIGHT-node.height);
		}
		while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;	
		
		return node;
	}
	
	
	
	public void init()
	{
		minRoomSize = 8;
		
		globalTiles = new int[ROOM_WIDTH, ROOM_HEIGHT];
		
		for(int i = 0; i < ROOM_WIDTH ; i++)
		{
			for(int j = 0; j < ROOM_HEIGHT; j++)
			{
				globalTiles[i, j] = 0;
			}
		}
		
		BSPNode root = null;
		trunk = root;
		insertNode(ref trunk, 0);
		
		root = trunk;
		r = new ArrayList();
		trunk.width = ROOM_WIDTH;
		trunk.height = ROOM_HEIGHT;
		final = new ArrayList();
			
//		r.Add(trunk);
//		while(r.Count != 0)
//		{
//			int index = Random.Range(0, r.Count - 1);
//			trunk = (BSPNode)r.ToArray()[index];
//			r.RemoveAt(index);
//			partitionate(trunk, trunk.initPosX, trunk.width, trunk.initPosY, trunk.height );	
//		}
//		
//		
//		trunk = root;
//		createRoomsFromRoot(trunk);
		int bailout;
		
		
		int bigRoomsCount = 3;
		int hRoomsCount = 5;
		int vRoomsCount = 5;
		
		int totalRooms = bigRoomsCount + hRoomsCount + vRoomsCount;
		
		int[] rooms = new int[ totalRooms ];
		
		for ( int i = 0; i < totalRooms; i++ )
		{
			int v = -1;
			if ( bigRoomsCount > 0 )
			{
				bigRoomsCount--;
				v = 0;
			}
			else if ( hRoomsCount > 0 )
			{
				hRoomsCount--;
				v = 1;
			}
			else if ( vRoomsCount > 0 )
			{
				hRoomsCount--;
				v = 2;
			}
			
			if ( v != -1 )
			rooms[i] = v;
		}
		
		// shuffle cabeza
		for ( int i = 0; i < totalRooms - 2; i++ )
		{
			int swap = rooms[ i ];
			int rand = Random.Range( i + 1, totalRooms - 1 );
			rooms[ i ] = rooms[ rand ];
			rooms[ rand ] = swap;
		}
		//Lechon
		for ( int i = 0; i < totalRooms; i++ )
		{
			int roomType = rooms[i];
			
			BSPNode node = null;
			
			switch ( roomType )
			{
				case 0:
					node = GenerateBigRoom();
					
					break;
				case 1:
					node = GenerateHRoom();
					break;
				case 2:
					node = GenerateVRoom();
					break;
			}
			//node.initPosX = Random.Range (0, ROOM_WIDTH);
			if ( node != null )
				final.Add( node );
		}
 		//resizeNodes();
		
		removeOverlappedNodes();
	
		for(int i = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final[i];
			a.createRoom(wallTile, floorTile);
		//	a.createDoors(doorTile);
			printToGlobalTiles(a);
		}
		
		makeDoors();
		
	}
	
	void printToGlobalTiles(BSPNode data)
	{
		for(int i = 0; i < data.width ; i++)
		{
			for(int j = 0; j < data.height ; j++)	
			{
				int posX = data.initPosX + i;
				if(posX >= ROOM_WIDTH)
				{
					print("MAX ROOM WIDTH WTF");
					posX = 99;
				}
				
				int posY = data.initPosY + j;
				if(posY >= ROOM_HEIGHT)
				{
					print("MAX ROOM HEIGHT WTF");
					posY = 99;
				}
				GameObject tile = data.room.getTile(i, j);
				int val = 0; 
				if(tile.CompareTag(floorTile.tag))
				{
					val = 1;
				}
				
				else if(tile.CompareTag(wallTile.tag))
				{
					val = 2;
				}
				
				else if(tile.CompareTag(doorTile.tag))
				{
					val = 3;
				}
				
				globalTiles[posX, posY] = val;
			}
		}
	}
	
	bool doesThisNodeOverlapWithAnotherNodeOrNot( BSPNode itdoesntright )
	{
		//BSPNode[] finalNodes = (BSPNode[])final.ToArray ();
			
		for(int i = 0; i < final.Count; i++)
		{
			BSPNode notsure = (BSPNode)final[i]; 
			
			if( notsure.rectOverlap(itdoesntright) )
				return true;
		}

		return false;
	}
	
	void removeOverlappedNodes()
	{
		//BSPNode[] finalNodes = (BSPNode[])final.ToArray ();
		
		for(int i = 0; i < final.Count; i++)
		{
			for(int j = i + 1; j < final.Count; j++)
			{
				BSPNode a = (BSPNode)final[i]; 
				BSPNode b = (BSPNode)final[j];
				
				if( a.rectOverlap(b) )
				{
					i--;
				
					final.Remove(a);
					break;
				}
			}
			
		}
	}
	
	public void makeDoors()
	{
		BSPNode initialRoom = getCornerSmallestRoom();
		BSPNode walker = initialRoom;
		BSPNode min = null;
		BSPNode prev = null;

		
		ArrayList runner = (ArrayList)final.Clone();	
		while(runner.Count != 1)
		{
			float maxDistance = 999999.0f;
			for(int i = 0; i < runner.Count; i ++)
			{
				BSPNode a = (BSPNode)runner.ToArray()[i];
				if(a == walker)
					continue;
				
				float d = distance(a, walker);
				if(d < maxDistance)
				{
					maxDistance = d;
					min = a;
				}
			}
			
			Vector2 walkerCenter = walker.room.getCenter();
			Vector2 minCenter = min.room.getCenter();
		
			int compX = (int)Mathf.Abs(walkerCenter.x - minCenter.x);
			int compY = (int)Mathf.Abs(walkerCenter.y - minCenter.y);
			
			if(compX >= compY)
			{
				bool left = walker.initPosX > min.initPosY;
				int sideA = 99;
				int sideB = 99;
				if(left)
				{
					sideA = walker.room.checkSideFree(Room.LEFT) ? Room.LEFT : Room.RIGHT;
					sideB = min.room.checkSideFree(Room.RIGHT) ? Room.RIGHT : Room.LEFT;
				}
				else
				{
					sideA = walker.room.checkSideFree(Room.RIGHT) ? Room.RIGHT : Room.LEFT;
					sideB = min.room.checkSideFree(Room.LEFT) ? Room.LEFT : Room.RIGHT;
				}
				walker.room.createDoorAtSide(sideA,  doorTile);
				min.room.createDoorAtSide(sideB, doorTile);
			}
			else
			{
				bool top = walker.initPosY > min.initPosY;
				int sideA = 99;
				int sideB = 99;
				if(top)
				{
					sideA = walker.room.checkSideFree(Room.BOTTOM) ? Room.BOTTOM : Room.TOP;
					sideB = min.room.checkSideFree(Room.TOP) ? Room.TOP : Room.BOTTOM;
				}
				else
				{
					sideA = walker.room.checkSideFree(Room.TOP) ? Room.TOP : Room.BOTTOM;
					sideB = min.room.checkSideFree(Room.BOTTOM) ? Room.BOTTOM : Room.TOP;
					
				}
				walker.room.createDoorAtSide(sideA, doorTile);
				min.room.createDoorAtSide(sideB, doorTile);
				//walker.room.createDoorAtSide(top  ? Room.BOTTOM : Room.TOP, doorTile);
				//min.room.createDoorAtSide(top  ? Room.TOP : Room.BOTTOM, doorTile);
			}
			runner.Remove(walker);
			walker = min;
		}
		
	}
	
	

	
	public BSPNode getCornerSmallestRoom()
	{
		ArrayList cornerRooms = new  ArrayList();
		
		
		int smallestRoomSize = 99999;
		BSPNode cornerRoom = null;
		
		int cornerMod = 10;
		for(int i = 0 ; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			if(
				(a.initPosX > ROOM_WIDTH / 2 + cornerMod && a.initPosY > ROOM_HEIGHT / 2 + cornerMod) ||
				(a.initPosX > ROOM_WIDTH / 2 + cornerMod && a.initPosY < ROOM_HEIGHT / 2 - cornerMod) || 
				(a.initPosX < ROOM_WIDTH / 2 - cornerMod && a.initPosY > ROOM_HEIGHT / 2 + cornerMod) ||
				(a.initPosX < ROOM_WIDTH / 2 - cornerMod && a.initPosY < ROOM_HEIGHT / 2 - cornerMod) 
				)
			{
				cornerRooms.Add(a);
			}
			
		}
		
		for(int i = 0 ; i < cornerRooms.Count ; i++)
		{
			BSPNode a = (BSPNode)cornerRooms.ToArray()[i];
			if(a.width + a.height < smallestRoomSize)
			{
				cornerRoom = a;
				smallestRoomSize = a.width + a.height;
			}
		}
		
		return cornerRoom;
	}
	
	
	public float distance(BSPNode a, BSPNode b)
	{
		int dx = (b.initPosX - a.initPosX) ;
		int sdx = dx*dx;
		
		int dy = b.initPosY - a.initPosY;
		int sdy = dy*dy;
		
		float val = Mathf.Sqrt(sdx+sdy);
		
		return val;
		
	}
	
	int caca = 0;
	
	public bool partitionate(BSPNode root, int minW, int maxW, int minH, int maxH)
	{
		BSPNode temp = root;
		
		if(BSPNode.instanceCount >= 800)
		{
			print("STOPPED AT REF COUNT");
			return false;
		}
		
			//createRoom(temp, maxW, maxH);
		
		caca++;
		bool horizontalCut = caca % 2 == 0 ? true : false; //Random.Range(0.0f, 20.0f) < 8.0f ? true : false;
		
	
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
			
			if(minH + cut >= ROOM_HEIGHT)
			{
				print("OUT OF BOUNDS AT VERTICAL CUT");
				return false;
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
			
			if(minW + cut >= ROOM_WIDTH)
			{
				print("OUT OF BOUNDS AT VERTICAL CUT");
				return false;
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
		
	
		for(int i = 0; i < final.Count ; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			if(a.initPosX == node.initPosX && a.initPosY == node.initPosY && a.width == node.width && a.height == node.height)
				return;

			
		}
	
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
		final.Reverse();
		for(int i  = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final.ToArray()[i];
			a.tryToResize(wallTile, floorTile);
		}
		
	}
	
	
	public void insertNode(ref BSPNode node, int val)
	{
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
