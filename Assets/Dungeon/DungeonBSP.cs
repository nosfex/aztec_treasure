using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonBSP : MonoBehaviour
{
	public const int WORLD_TILE_WIDTH = 50;
	public const int WORLD_TILE_HEIGHT = 50;
	
	public int minRoomSize = 9;

	public int bigRoomsCount = 3;
	public int altarRoomsCount = 4;
	public int hRoomsCount = 4;
	public int vRoomsCount = 4;
	
	public bool createOnAwake = true;
	
	public static ArrayList doors = new ArrayList();

	public GameObject invisibleWallTile;
	public GameObject wallTile;
	public GameObject floorTile;
	public GameObject corridorTile;
	public GameObject torchWallTile;
	public GameObject altarPrefab;
	
	public BSPNode trunk;
	public System.Collections.ArrayList r;
	public System.Collections.ArrayList final;
	
	public Decoration[] decorations;
	
	public Decoration startRoomDecoration;
	public Decoration endRoomDecoration;
	
	
	//public World[] targetContainers;
	
	
	private List<Decoration> shuffledDecorations;
	
	int bailLimit = 500;

	Light[] lights;
	
	Transform container;

	BSPNode cornerSmallestNode;
	public Room initialRoom;
	
	GameObject[] wallPattern; 
	
	public GameObject[,] globalTiles;
	
	void Awake()
	{
		wallPattern = new GameObject[] { wallTile, wallTile, wallTile,
						     			 torchWallTile, null, torchWallTile,
							 			 null, torchWallTile, null	};
		
		shuffledDecorations = new List<Decoration>();
		
		for ( int i = 0; i < decorations.Length; i++ )
			shuffledDecorations.Add ( decorations[i] );

		// shuffle decorations
		for ( int i = 0; i < shuffledDecorations.Count - 2; i++ )
		{
			Decoration swap = shuffledDecorations[ i ];
			int rand = Random.Range( i + 1, shuffledDecorations.Count - 1 );
			shuffledDecorations[ i ] = shuffledDecorations[ rand ];
			shuffledDecorations[ rand ] = swap;
		}
		
		if ( createOnAwake )
			BuildDungeon();
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
			node.initPosX = Random.Range (0, WORLD_TILE_WIDTH-node.width-0);
			node.initPosY = Random.Range (0, WORLD_TILE_HEIGHT-node.height-0);
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
			node.initPosX = Random.Range (0, WORLD_TILE_WIDTH-node.width-0);
			node.initPosY = Random.Range (0, WORLD_TILE_HEIGHT-node.height-0);
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
		
			int size = Random.Range ( 11, 14 );
			node.width = Mathf.Max ( 10, size + Random.Range(0, 3) );
			node.height = Mathf.Max ( 10, size + Random.Range(0, 3) );
			
			int marginX1 = (int)((float)WORLD_TILE_WIDTH * 0.2f);
			int marginX2 = (int)((float)WORLD_TILE_WIDTH * 0.8f);

			int marginY1 = (int)((float)WORLD_TILE_HEIGHT * 0.2f);
			int marginY2 = (int)((float)WORLD_TILE_HEIGHT * 0.8f);
			
			node.initPosX = Random.Range ( marginX1, marginX2 - node.width);
			node.initPosY = Random.Range ( marginY1, marginY2 - node.height);
		}
		while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;	
		
		return node;
	}
	
	
	BSPNode GenerateAltarRoom2( int dx, int dy )
	{
		BSPNode node = new BSPNode(0);
		int bailout = bailLimit * 20;
		do
		{
			bailout--;
			if ( bailout == 0 ) return null;
		
			//int size = Random.Range ( 11, 14 );
			node.width = 7;//Mathf.Max ( 10, size + Random.Range(0, 3) );
			node.height = 7;//Mathf.Max ( 10, size + Random.Range(0, 3) );
			
			node.isAltar = true;
			node.initPosX = (WORLD_TILE_WIDTH / 2) - 3 + Random.Range ( WORLD_TILE_WIDTH / 4, WORLD_TILE_WIDTH / 2 ) * dx;
			node.initPosY = (WORLD_TILE_HEIGHT / 2) - 3 + Random.Range ( WORLD_TILE_HEIGHT / 4, WORLD_TILE_HEIGHT / 2 ) * dy;
		
			node.initPosX = Mathf.Clamp( node.initPosX, 0, WORLD_TILE_WIDTH - node.width );
			node.initPosY = Mathf.Clamp( node.initPosY, 0, WORLD_TILE_HEIGHT - node.height );
		}
		while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;	
		
		return node;
	}		
	
	BSPNode GenerateRoom( int dx, int dy, int sizeX, int sizeY )
	{
		BSPNode node = new BSPNode(0);
		//int bailout = bailLimit * 20;
		//do
		{
		//	bailout--;
		//	if ( bailout == 0 ) return null;
		
			node.width = sizeX;
			node.height = sizeY;
			
			//node.isAltar = true;
			node.initPosX = (WORLD_TILE_WIDTH - 3)  * dx;
			node.initPosY = (WORLD_TILE_HEIGHT - 3)  * dy;
		
			node.initPosX = Mathf.Clamp( node.initPosX, 0, WORLD_TILE_WIDTH - node.width );
			node.initPosY = Mathf.Clamp( node.initPosY, 0, WORLD_TILE_HEIGHT - node.height );
		}
		//while( doesThisNodeOverlapWithAnotherNodeOrNot( node ) ); // Repeat if overlap;	
		
		if ( doesThisNodeOverlapWithAnotherNodeOrNot( node )  )
		{
			print ("ouch");
			return null;
		}
		
		return node;
	}		
	
	
	public GameObject CreateTile( GameObject tile, int tileX, int tileY )
	{
		return CreateTile( tile, tileX, tileY, Vector3.zero, Quaternion.identity,0,0 );
	}

	public GameObject CreateTile( GameObject tile, int tileX, int tileY, Vector3 offset )
	{
		return CreateTile( tile, tileX, tileY, offset, Quaternion.identity,0,0 );
	}
	
	public GameObject CreateTile( GameObject tile, int tileX, int tileY, Vector3 offset,  Quaternion rotation  )
	{
		return CreateTile( tile, tileX, tileY, offset, rotation,0,0 );
	}

	static int caca = 0;
	public GameObject CreateTile( GameObject tile, int tileX, int tileY, Vector3 offset, Quaternion rotation, float VX, float VZ )
	{
		if ( globalTiles[ tileX, tileY ] != null )
			DestroyImmediate(globalTiles[tileX, tileY]);
		
		GameObject go = (GameObject)Instantiate(tile);
		
		go.transform.position = new Vector3( tileX * 0.8f, 0, tileY * 0.8f );
		go.transform.position += offset;
		go.transform.rotation = rotation;
		go.transform.parent = container;
		go.name = go.name.TrimEnd( "(Clone)" );
		MeshFilter[] mfa = go.GetComponentsInChildren<MeshFilter>();
		
		foreach( MeshFilter mf in mfa)
		{
			Vector3[] tris = mf.mesh.vertices;
			for ( int i = 0; i < tris.Length; i++ )
			{
				if ( tile != wallTile )
					break;
					
				
				//if ( caca < 10)
			//		print ( tris[i].y );
				if ( tris[i].y  < 0 )
					continue;
				Random.seed = (int)( ((tris[i].x + tileX) ) +  ((tris[i].z + tileY) ) );
				float a = 0.02f;
				tris[i] += new Vector3( Random.Range (-a*2, a*2),Random.Range (-a, 0), Random.Range (-a, a));
			}
			
			mf.mesh.vertices = tris;
			mf.mesh.RecalculateNormals();
		}
		caca++;
		globalTiles[tileX, tileY] = go;
		
		return go;
	}
	
	public void DestroyTile( int tileX, int tileY )
	{
		if ( globalTiles[ tileX, tileY ] != null )
			DestroyImmediate(globalTiles[tileX, tileY]);
		
		globalTiles[ tileX, tileY ] = null;
	}
	
	public void BuildDungeon()
	{
		BuildDungeon( transform );
	}
	
	[HideInInspector] public BSPNode startRoom, endRoom;
	
	
	public void BuildDungeon( Transform container )
	{
		this.container = container;
		globalTiles = new GameObject[DungeonBSP.WORLD_TILE_WIDTH, DungeonBSP.WORLD_TILE_HEIGHT];

		BSPNode root = null;
		trunk = root;
		trunk = new BSPNode(0);
		
		root = trunk;
		r = new ArrayList();
		final = new ArrayList();

		trunk.width = WORLD_TILE_WIDTH;
		trunk.height = WORLD_TILE_HEIGHT;
			
		int bailout;
		
		int totalRooms = hRoomsCount + vRoomsCount;
		
		int[] rooms = new int[ totalRooms ];
		
		for ( int i = 0; i < totalRooms; i++ )
		{
			int v = -1;

			if ( hRoomsCount > 0 )
			{
				hRoomsCount--;
				v = 0;
			}
			else if ( vRoomsCount > 0 )
			{
				hRoomsCount--;
				v = 1;
			}
			
			if ( v != -1 ) rooms[i] = v;
		}
		
		// shuffle cabeza
		for ( int i = 0; i < totalRooms - 2; i++ )
		{
			int swap = rooms[ i ];
			int rand = Random.Range( i + 1, totalRooms - 1 );
			rooms[ i ] = rooms[ rand ];
			rooms[ rand ] = swap;
		}
		
		int startRoomX = -1, startRoomY = -1;
		
		switch ( Random.Range (0, 3) )
		{
			case 0:
				startRoomX = -1; startRoomY = -1;
				break;
			case 1:
				startRoomX = 1; startRoomY = 1;
				break;
			case 2:
				startRoomX = -1; startRoomY = 1;
				break;
			case 3:
				startRoomX = 1; startRoomY = -1;
				break;
		}
		
		startRoom = GenerateRoom( startRoomX, startRoomY, 7, 7 );
		startRoom.isStartRoom = true;
		final.Add( startRoom );

		endRoom = GenerateRoom( startRoomX * -1, startRoomY * -1, 11, 11 );
		endRoom.isEndRoom = true;
		final.Add( endRoom );
		
		BSPNode altar1 = GenerateAltarRoom2( 0, 1 );
		if ( altar1 != null ) final.Add( altar1 );
		
		BSPNode altar2 = GenerateAltarRoom2( 1, 0 );
		if ( altar2 != null ) final.Add( altar2 );
		
		BSPNode altar3 = GenerateAltarRoom2( 0, -1 );
		if ( altar3 != null ) final.Add( altar3 );

		BSPNode altar4 = GenerateAltarRoom2( -1, 0 );
		if ( altar4 != null ) final.Add( altar4 );

		
		for ( int i = 0; i < bigRoomsCount; i++ )
		{
			BSPNode node = null;

			node = GenerateBigRoom();

			if ( node != null )	final.Add( node );
		}
		
		//Lechon
		for ( int i = 0; i < totalRooms; i++ )
		{
			int roomType = rooms[i];
			
			BSPNode node = null;
			
			switch ( roomType )
			{
				case 1:
					node = GenerateHRoom();	break;
				case 2:
					node = GenerateVRoom();	break;
			}

			if ( node != null )
				final.Add( node );
		}

		for(int i = 0; i < final.Count; i++)
		{
			BSPNode a = (BSPNode)final[i];
			a.createRoom( this );
		//	a.createDoors(doorTile);
			printToGlobalTiles(a);
		}
		
		makeDoors();
		
		if ( altar1 != null ) createDoorConnectingWithNearest( altar1 );
		if ( altar2 != null ) createDoorConnectingWithNearest( altar2 );
		if ( altar3 != null ) createDoorConnectingWithNearest( altar3 );
		if ( altar4 != null ) createDoorConnectingWithNearest( altar4 );
		
		initialRoom = startRoom.room;
		Debug.Log ("added: " + initialRoom );
		
		connectDoors();
		wallFill();
		
	}
	

	
	public void createDoorConnectingWithNearest( BSPNode who )
	{
		BSPNode nearest = findNearestRoom( who );
		
		if ( nearest != null )
		{
			createDoorConnectingWith( who, nearest );
		}
	}
	
	
	public void createDoorConnectingWith( BSPNode walker, BSPNode min )
	{
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
			walker.room.createDoorAtSide(sideA,  floorTile);
			min.room.createDoorAtSide(sideB, floorTile);
			
			
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
			walker.room.createDoorAtSide(sideA, floorTile);
			min.room.createDoorAtSide(sideB, floorTile);
			
		}
		
		walker.room.getLastDoor().target = min.room.getLastDoor();
		min.room.getLastDoor().target = walker.room.getLastDoor();		
	}
	
	public void makeDoors()
	{
		ArrayList runner = (ArrayList)final.Clone();
		
		for ( int i = 0; i < runner.Count; i++ )
		{
			if ( ((BSPNode)runner[i]).isAltar )
			{
				runner.RemoveAt( i );
				i--;
			}
		}
		
		cornerSmallestNode = getCornerSmallestRoom( runner );
		BSPNode walker = cornerSmallestNode;
		BSPNode min = null;
		BSPNode prev = null;

		while(runner.Count != 1)
		{
			float maxDistance = 999999.0f;
			for(int i = 0; i < runner.Count; i ++)
			{
				BSPNode a = (BSPNode)runner[i];
				
				if(a == walker)
					continue;
				
				float d = distance(a, walker);
				if(d < maxDistance)
				{
					maxDistance = d;
					min = a;
				}
			}
			
			createDoorConnectingWith( walker, min );
			
			runner.Remove(walker);
			walker = min;
		}
		
	}
	
	public void connectDoors()
	{
		for(int i = 0 ; i < doors.Count; i++)
		{
			DoorData a = (DoorData)doors[i];
			DoorData b = ((DoorData)doors[i]).target;//(DoorData)doors[i + 1];
			
			if ( a.connected )
				continue;
			
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
			}
	
			bool alreadyDidMyPrioritySwitch = false;
			
			while(aX != bX || aY != bY)
			{	
				if(globalTiles[aX, aY] == null)
					generateTileN8(corridorTile, aX, aY, true);
				else
					generateTileN8(corridorTile, aX, aY, true);
				
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
					if(aX > bX) aX--;
					else if (aX < bX) aX++;				
					else if(aY > bY) aY--;
					else if(aY < bY) aY++;
				}
				else 
				{
					if(aY > bY)	aY--;
					else if(aY < bY) aY++;
					else if(aX > bX) aX--;
					else if (aX < bX) aX++;				
				}
			}
			
			a.connected = true;
			b.connected = true;
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
				
				if ( tile == null ) continue;
				
				if((row + j) < 0) continue;
				if((row + j) > WORLD_TILE_HEIGHT - 1) continue;
				if(col + i < 0) continue;
				if(col + i > WORLD_TILE_WIDTH - 1 )	continue;

				Vector3 scale = new Vector3(0.8f, 0.8f, 0.8f);
				
				if(globalTiles[col + i, row + j] != null && replace == false)
				{
					// Fix for floors on ends of the map.
					if(col + i == 0 || col + i == WORLD_TILE_WIDTH - 1)
					{
						if ( tile == torchWallTile )
							tile = wallTile;
						
						
						
						CreateTile ( tile, col + i, row + j, posOffset, Quaternion.identity, i * 0.5f, j * 0.5f );
//						Destroy(globalTiles[col + i, row + j]);
//						GameObject fix = (GameObject)Instantiate(tile);
//						
//						fix.transform.position = new Vector3( (col + i) * scale.x, scale.y * Room.refCount * 0, (row + j) * scale.z);
//						fix.transform.position += posOffset;
//						fix.transform.parent = container;
//						fix.name = fix.name.TrimEnd( "(Clone)" );
//
//						
//						globalTiles[(col + i), (row + j)] = fix;
						continue;
					}
					
					if(row + j == 0 || row + j == WORLD_TILE_HEIGHT - 1)
					{
						if ( tile == torchWallTile )
							tile = wallTile;

						CreateTile ( tile, col + i, row + j, posOffset, Quaternion.identity, i * 0.5f, j * 0.5f  );
//						Destroy(globalTiles[col + i, row + j]);
//						GameObject fix = (GameObject)Instantiate(tile);
//						
//						fix.transform.position = new Vector3( (col + i) * scale.x, scale.y * Room.refCount * 0, (row + j) * scale.z);
//						fix.transform.position += posOffset;
//						fix.transform.parent = container;
//						fix.name = fix.name.TrimEnd( "(Clone)" );
//
//						globalTiles[(col + i), (row + j)] = fix;
						continue;
					}
					
					continue;
				}
				
				if(globalTiles[col + i, row + j] == null)
				{
					Vector3 pos = new Vector3( col * scale.x + posOffset.x, posOffset.y, row * scale.z + posOffset.z);

					if ( tile == torchWallTile )
					{
						float lightDistThreshold = 0.8f * 16;
						
						if ( globalTiles[col, row].name == floorTile.name )
							lightDistThreshold = 0.8f * 3;
						
						RefreshLightsArray();
						Light l = FindPointLightByDistance( pos, lightDistThreshold );
						
						if ( l != null )
						{
							tile = wallTile;
						}
					}
					
					Quaternion rotation = Quaternion.identity;
		
					Vector3 tmpPos = new Vector3( (col + i) * 0.8f + posOffset.x, posOffset.y,  (row + j) * 0.8f + posOffset.z );
					
					if ( tile == torchWallTile )
						rotation = Quaternion.LookRotation( tmpPos - pos );
//		
					GameObject obj = CreateTile ( tile, col + i, row + j, posOffset, rotation, i * 0.5f, j * 0.5f );

					//GameObject obj = (GameObject)Instantiate(tile);
//					
//					obj.transform.position = new Vector3( (col + i) * scale.x, scale.y * Room.refCount * 0, (row + j) * scale.z);
//					obj.transform.position += posOffset;
//					obj.transform.parent = container;
//					obj.name = obj.name.TrimEnd( "(Clone)" );
					

	//				globalTiles[(col + i), (row + j)] = obj;
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
				if ( j == 0 && i == 0 )	continue;
				
				if((y + j) < 0)	continue;
				if((y + j) > WORLD_TILE_HEIGHT - 1)	continue;
				if(x + i < 0)	continue;
				if(x + i > WORLD_TILE_WIDTH - 1 )	continue;
				
				if(globalTiles[x + i, y + j] != null)
				{
					if ( globalTiles[x + i, y + j].name == tile.name )
						count++;
				}
			}
		}
		
		return count;
	}
	
	void wallFill()
	{
		RefreshLightsArray();
		
		for(int y = 0; y < WORLD_TILE_HEIGHT; y++)
		{
			for(int x = 0; x < WORLD_TILE_WIDTH; x++)
			{
				if  ( globalTiles[x, y] != null && 
				    ( globalTiles[x, y].name == floorTile.name
			       || globalTiles[x, y].name == corridorTile.name ) )
				{
					Vector3 pos = new Vector3(0, 1.2f, 0);
					generateTileN8(wallPattern, x, y, false, pos);
				}
			}
		}
		
		for(int y = WORLD_TILE_HEIGHT - 3; y >= 0; y--)
		{
			for(int x = 0; x < WORLD_TILE_WIDTH ; x++)
			{
				if(globalTiles[x, y] != null && 
					globalTiles[x, y].name == wallTile.name )
				{
					if(globalTiles[x, y + 1] != null 
						&& 
						( globalTiles[x, y + 1].name == floorTile.name 
						|| globalTiles[x, y + 1].name == corridorTile.name ) )
					{
						globalTiles[x, y].transform.position -= Vector3.up * 1.0f;
					}
					else 
					if(globalTiles[x, y + 2] != null && 
							( globalTiles[x, y + 2].name == floorTile.name
						|| globalTiles[x, y + 2].name == corridorTile.name ) )
							
					{
						globalTiles[x, y].transform.position -= Vector3.up * 0.6f;
					}
				}
				
				
				if(globalTiles[x, y] != null && 
					( globalTiles[x, y].name == wallTile.name
					|| globalTiles[x, y].name == corridorTile.name ) )
				{
					int caca = countTilesN8( x, y, wallTile );
					caca += countTilesN8( x, y, corridorTile );
					
					
					if ( caca == 0 )
					{
						print ("Destroying...");
						DestroyTile( x, y );						
						//DestroyImmediate(globalTiles[x, y]);
						//globalTiles[x,y].transform.localScale = Vector3.one * 0.3f;
						//globalTiles[x, y] = null;
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
	

	
	public BSPNode getCornerSmallestRoom( ArrayList nodesArray )
	{
		ArrayList cornerRooms = new  ArrayList();
		
		int smallestRoomSize = 99999;
		BSPNode cornerRoom = null;
		
		int cornerMod = 10;
		for(int i = 0 ; i < nodesArray.Count; i++)
		{
			BSPNode a = (BSPNode)nodesArray[i];
			if(	(a.initPosX > WORLD_TILE_WIDTH / 2 + cornerMod && a.initPosY > WORLD_TILE_HEIGHT / 2 + cornerMod) ||
				(a.initPosX > WORLD_TILE_WIDTH / 2 + cornerMod && a.initPosY < WORLD_TILE_HEIGHT / 2 - cornerMod) || 
				(a.initPosX < WORLD_TILE_WIDTH / 2 - cornerMod && a.initPosY > WORLD_TILE_HEIGHT / 2 + cornerMod) ||
				(a.initPosX < WORLD_TILE_WIDTH / 2 - cornerMod && a.initPosY < WORLD_TILE_HEIGHT / 2 - cornerMod) )
			{
				cornerRooms.Add(a);
			}
			
		}
		
		for(int i = 0 ; i < cornerRooms.Count ; i++)
		{
			BSPNode a = (BSPNode)cornerRooms[i];
			if(a.width + a.height < smallestRoomSize)
			{
				cornerRoom = a;
				smallestRoomSize = a.width + a.height;
			}
		}
		
		return cornerRoom;
	}
	
	void printToGlobalTiles(BSPNode data)
	{
		for(int i = 0; i < data.width ; i++)
		{
			for(int j = 0; j < data.height ; j++)	
			{
				int posX = data.initPosX + i;
				if(posX >= WORLD_TILE_WIDTH)
				{
					print("MAX ROOM WIDTH WTF");
					posX = 99;
				}
				
				int posY = data.initPosY + j;
				if(posY >= WORLD_TILE_HEIGHT)
				{
					print("MAX ROOM HEIGHT WTF");
					posY = 99;
				}
				GameObject tile = data.room.getTile(i, j);
				
				data.room.roomHolder.transform.parent = container;
				
				globalTiles[posX, posY] = data.room.tiles[i, j];
			}
		}
	}
	

	public BSPNode findNearestRoom( BSPNode who )
	{
		float minDistance = 999999.0f;
		BSPNode min = null;
		
		for(int i = 0; i < final.Count; i ++)
		{
			BSPNode a = (BSPNode)final[i];
			
			if(a == who)
				continue;
			
			float d = distance(a, who);
			if(d < minDistance)
			{
				minDistance = d;
				min = a;
			}
		}
		
		return min;
	}	
	
	public float distance(BSPNode a, BSPNode b)
	{
		int dx = (b.initPosX - a.initPosX);
		int dy = b.initPosY - a.initPosY;
		float val = Mathf.Sqrt( dx * dx + dy * dy );
		
		return val;
	}
	
	void RefreshLightsArray()
	{
		lights = (Light[])FindObjectsOfType( typeof( Light ) );
	}
	
	Light FindPointLightByDistance( Vector3 point, float distance )
	{
		foreach ( Light light in lights )
		{
			if ( light == null ) continue;
			if ( light.type != LightType.Point ) continue;
			
			float dist = Vector3.Distance( point, light.transform.position );
			
			if ( dist < distance ) return light;
		}

		return null;
	}
	
	public GameObject getClosestObjToPoint(Vector2 point)
	{
		for(int i = 0 ; i < WORLD_TILE_WIDTH; i++)
		{
			for(int j = 0; j < WORLD_TILE_HEIGHT; j++)
			{
				GameObject tile = (GameObject)(globalTiles[i,j]);
				
				if(tile == null) continue;
				
				Vector2 tilePos = new Vector2(tile.transform.position.x, tile.transform.position.z);
				Rect r = new Rect(tilePos.x, tilePos.y, 0.8f, 0.8f);

				if(r.Contains(point))
					return tile;
			}
		}
		
		return null;
	}
	
	public Decoration PeekDecoration()
	{
		return shuffledDecorations[ shuffledDecorations.Count - 1 ];
	}
	
	public Decoration DrawDecoration()
	{
		if ( shuffledDecorations.Count == 0 )
			return decorations[0];
		
		Decoration result = shuffledDecorations[ shuffledDecorations.Count - 1 ];
		shuffledDecorations.RemoveAt( shuffledDecorations.Count - 1 );
		return result;
	}	
	
	bool doesThisNodeOverlapWithAnotherNodeOrNot( BSPNode itdoesntright )
	{
		for(int i = 0; i < final.Count; i++)
		{
			BSPNode notsure = (BSPNode)final[i]; 
			
			if( notsure.rectOverlap(itdoesntright) )
				return true;
		}

		return false;
	}	
	
}
