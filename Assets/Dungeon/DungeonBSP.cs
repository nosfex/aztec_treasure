using UnityEngine;
using System.Collections;

public class DungeonBSP : MonoBehaviour
{
	public const int ROOM_WIDTH = 60;
	public const int ROOM_HEIGHT = 60;
	
	public int minRoomSize = 9;

	public int bigRoomsCount = 3;
	public int hRoomsCount = 4;
	public int vRoomsCount = 4;
	
	public bool createOnAwake = true;
	
	public static ArrayList doors = new ArrayList();

	public GameObject[,] globalTiles;
	public GameObject invisibleWallTile;
	public GameObject wallTile;
	public GameObject floorTile;
	public GameObject doorTile;
	public BSPNode trunk;
	public System.Collections.ArrayList r;
	public System.Collections.ArrayList final;

	
	int bailLimit = 20;
	
	void Awake()
	{
		wallPattern = new GameObject[] { null,invisibleWallTile,null,
						     wallTile,null,wallTile,
							 wallTile,wallTile,wallTile	};
		
		if ( createOnAwake )
		{
			BuildDungeon();
		}
	}
	
	BSPNode GenerateHRoom()
	{
		BSPNode node = new BSPNode(0);
		int bailout = bailLimit;
		do
		{
			bailout--;
			if ( bailout == 0 ) return null;
			
			node.width = Random.Range(10, 15);
			node.height = Random.Range(6, 8);
			node.initPosX = Random.Range (0, ROOM_WIDTH-node.width);
			node.initPosY = Random.Range (0, ROOM_HEIGHT-node.height);
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
			node.width = Random.Range(6, 8);
			node.height = Random.Range(10, 15);
			node.initPosX = Random.Range (0, ROOM_WIDTH-node.width);
			node.initPosY = Random.Range (0, ROOM_HEIGHT-node.height);
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
		
			int size = Random.Range ( 8, 12 );
			node.width = Mathf.Max ( 8, size + Random.Range(0, 3) );
			node.height = Mathf.Max ( 8, size + Random.Range(0, 3) );
			node.initPosX = Random.Range (0, ROOM_WIDTH-node.width);
			node.initPosY = Random.Range (0, ROOM_HEIGHT-node.height);
		}
		while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;	
		
		return node;
	}
	
	
	Transform container;
	
	public void BuildDungeon()
	{
		BuildDungeon( transform );
	}
	
	public void BuildDungeon( Transform container )
	{
		this.container = container;
		globalTiles = new GameObject[ROOM_WIDTH, ROOM_HEIGHT];
		
		for(int i = 0; i < ROOM_WIDTH ; i++)
		{
			for(int j = 0; j < ROOM_HEIGHT; j++)
			{
				globalTiles[i, j] = null;
			}
		}
		
		BSPNode root = null;
		trunk = root;
		trunk = new BSPNode(0);
		
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
		
		//removeOverlappedNodes();
	
		for(int i = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final[i];
			a.createRoom(wallTile, floorTile);
		//	a.createDoors(doorTile);
			printToGlobalTiles(a);
		}
		
		makeDoors();
		connectDoors();
		wallFill();
		
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
				
				data.room.roomHolder.transform.parent = container;
				//if ( tile != null )
				//	tile.transform.parent = transform;
				
				//int val = 0; 
				/*if(tile.CompareTag(floorTile.tag))
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
				}*/
				
				globalTiles[posX, posY] = data.room.tiles[i, j];
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
	
	BSPNode cornerSmallestNode;
	public Room initialRoom { get { return cornerSmallestNode.room; } }
	
	public void makeDoors()
	{
		cornerSmallestNode = getCornerSmallestRoom();
		BSPNode walker = cornerSmallestNode;
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
			
			if(walker.initPosX >= min.initPosX + min.width)
			{
				bool left = walker.initPosX > min.initPosX;
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
				
			}
			
			walker.room.getLastDoor().target = min.room.getLastDoor();
			min.room.getLastDoor().target = walker.room.getLastDoor();
			
			runner.Remove(walker);
			walker = min;
		}
		
	}
	
	public void connectDoors()
	{
		for(int i = 0 ; i < doors.Count - 1; i++)
		{
			DoorData a = (DoorData)doors[i];
			DoorData b = (DoorData)doors[i + 1];
			int	aX = (int)(a.pos.x);
			int aY = (int)(a.pos.y);
			
			int bX = (int)(b.pos.x);
			int bY = (int)(b.pos.y);
			
			
			int dX = (int)Mathf.Abs(aX - bX);
			int dY = (int)Mathf.Abs(aY - bY);
			
			bool walkPriority = true;	
			switch(a.side)
			{
				case Room.TOP:
					walkPriority = false;
				break;
				case Room.BOTTOM:
					walkPriority = false;
				break;
				case Room.LEFT:
				break;
				case Room.RIGHT:
				break;
			}
	
			bool alreadyDidMyPrioritySwitch = false;
			
			while(aX != bX || aY != bY)
			{	
				if(globalTiles[aX, aY] == null)
				{
			
					generateTileN8(floorTile, aX, aY, true);
				}
				
				else
				{
				
					generateTileN8(floorTile, aX, aY, true);
					
				}
				
				
				int offset =  i % 2 == 0 ? -1 : 0;
				if ( !alreadyDidMyPrioritySwitch )
				{
					if ( Mathf.Abs(aX - bX) < (dX * 0.5f) + offset)
					{
						walkPriority = !walkPriority;
						alreadyDidMyPrioritySwitch = true;
					}
					else if ( Mathf.Abs(aY - bY) < (dY * 0.5f) + offset)
					{
						walkPriority = !walkPriority;
						alreadyDidMyPrioritySwitch = true;
					}
						
				}
				
				if ( walkPriority )
				{
					if(aX > bX)
						aX--;
					else if (aX < bX)
						aX++;				
					else if(aY > bY)
						aY--;
					else if(aY < bY)
						aY++;
				}
				else 
				{
					if(aY > bY)
						aY--;
					else if(aY < bY)
						aY++;
					else if(aX > bX)
						aX--;
					else if (aX < bX)
						aX++;				
					
				}
			}
		}
	}
	
	public void generateTileN8(GameObject tile, int col, int row, bool replace )
	{
		generateTileN8(tile, col, row, replace, Vector3.zero );
	}
	
	public void generateTileN8(GameObject tile, int col, int row, bool replace, Vector3 posOffset)
	{
		GameObject[] pattern = {tile,tile,tile,tile,tile,tile,tile,tile,tile};
		generateTileN8( pattern, col, row, replace, posOffset );
	}

	
	public void generateTileN8(GameObject[] pattern, int col, int row, bool replace, Vector3 posOffset)
	{
		for(int i = -1; i <= 1 ; i++)
		{
			for(int j = -1; j <= 1 ; j++)
			{
				int _x = (j + 1) * 3;
				int _y = (i + 1);
					
				GameObject tile = pattern[ _y + _x ];
				
				if ( tile == null )
					continue;
				
				Vector3 scale = new Vector3(0.8f, 0.8f, 0.8f);
				if((row + j) < 0)
					continue;
				if((row + j) > ROOM_HEIGHT - 1)
					continue;
				
				if(col + i < 0)
					continue;
				if(col + i > ROOM_WIDTH - 1 )
					continue;
				
				if(globalTiles[col + i, row + j] != null && replace == false)
				{
					if(col + i == 0 || col + i == ROOM_WIDTH - 1)
					{
						Destroy(globalTiles[col + i, row + j]);
						GameObject fix = (GameObject)Instantiate(tile);
						
						fix.transform.position = new Vector3( (col + i) * scale.x, scale.y * Room.refCount * 0, (row + j) * scale.z);
						fix.transform.position += posOffset;
						fix.transform.parent = container;
						
						globalTiles[(col + i), (row + j)] = fix;
						continue;
					}
					
					if(row + j == 0 || row + j == ROOM_HEIGHT - 1)
					{
						Destroy(globalTiles[col + i, row + j]);
						GameObject fix = (GameObject)Instantiate(tile);
						
						fix.transform.position = new Vector3( (col + i) * scale.x, scale.y * Room.refCount * 0, (row + j) * scale.z);
						fix.transform.position += posOffset;
						fix.transform.parent = container;
						
						globalTiles[(col + i), (row + j)] = fix;
						continue;
					}
					
					continue;
				}
				
				if(globalTiles[col + i, row + j] == null)
				{
					GameObject obj = (GameObject)Instantiate(tile);
					
					obj.transform.position = new Vector3( (col + i) * scale.x, scale.y * Room.refCount * 0, (row + j) * scale.z);
					obj.transform.position += posOffset;
					obj.transform.parent = container;
					globalTiles[(col + i), (row + j)] = obj;
				}
			}
		}
	}

	public int countTilesN8( int x, int y, GameObject tile )
	{
		int count = 0;
		for(int i = -1; i <= 1 ; i++)
		{
			for(int j = -1; j <= 1 ; j++)
			{
				if ( j == 0 && i == 0 )
					continue;
				
				
				if((y + j) < 0)
					continue;
				if((y + j) > ROOM_HEIGHT - 1)
					continue;
				if(x + i < 0)
					continue;
				if(x + i > ROOM_WIDTH - 1 )
					continue;
				
				if(globalTiles[x + i, y + j] != null)
				{
					if ( globalTiles[x + i, y + j].name == tile.name + "(Clone)" )
						count++;
				}
			}
		}
		
		//print ("count "+ count );
		return count;
	}
	
	GameObject[] wallPattern; 

	void wallFill()
	{
		for(int y = 0; y < ROOM_HEIGHT; y++)
		//for(int y = ROOM_HEIGHT - 1; y >= 0; y--)
		{
			for(int x = 0; x < ROOM_WIDTH ; x++)
			{
				if(globalTiles[x, y] != null && globalTiles[x, y].name == floorTile.name + "(Clone)")
				{
					Vector3 pos = new Vector3(0, 1.2f, 0);
					//generateTileN8(wallPattern, x, y, false, pos);
					generateTileN8(wallTile, x, y, false, pos);
				}
				
//				if ( globalTiles[x, y] == null )
//					continue;
//				
//				if ( globalTiles[x, y].name == floorTile.name + "Clone" &&
//					 globalTiles[x, y+1] == null )
//				{
//				}
				
			}
			
		}

		
		for(int y = ROOM_HEIGHT - 3; y >= 0; y--)
		{
			for(int x = 0; x < ROOM_WIDTH ; x++)
			{
				if(globalTiles[x, y] != null && globalTiles[x, y].name == wallTile.name + "(Clone)")
				{
					if(globalTiles[x, y + 1] != null && globalTiles[x, y + 1].name == floorTile.name + "(Clone)")
					{
						globalTiles[x, y].transform.position -= Vector3.up * 1.0f;
					}
					else 
					if(globalTiles[x, y + 2] != null && globalTiles[x, y + 2].name == floorTile.name + "(Clone)")
					{
						globalTiles[x, y].transform.position -= Vector3.up * 0.6f;
					}
						
						
				}
				
				
				if(globalTiles[x, y] != null && globalTiles[x, y].name == wallTile.name + "(Clone)")
				{
					int caca = countTilesN8( x, y, wallTile );
					
					if ( caca == 0 )
					{
						print ("Destroying...");
						DestroyImmediate(globalTiles[x, y]);
						//globalTiles[x,y].transform.localScale = Vector3.one * 0.3f;
						globalTiles[x, y] = null;
						/*GameObject fix = (GameObject)Instantiate( invisibleWallTile );
						
						Vector3 scale = new Vector3(0.8f, 0.8f, 0.8f);

						fix.transform.position = new Vector3( x * scale.x, 1.2f, y * scale.z );
						fix.transform.parent = container;
						globalTiles[x, y] = fix;*/
					}
				}
			}
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
}
