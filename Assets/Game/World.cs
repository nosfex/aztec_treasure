using UnityEngine;
using System.Collections;

public class World : MonoBehaviour 
{
	
	[HideInInspector] public Transform playerContainer;
	public Transform startingPoint;
	new public FollowSmooth camera;
	public Player player;
	
	public Transform deathYLimit;
	
	public GameObject[,] globalTiles;
	
	void Awake()
	{
		globalTiles = new GameObject[DungeonBSP.WORLD_TILE_WIDTH, DungeonBSP.WORLD_TILE_HEIGHT];

		camera = GetComponentInChildren<FollowSmooth>();
		player = GetComponentInChildren<Player>();
	}
	
	public void InitTiles()
	{
		InitTiles ( transform );
		
//		foreach ( GameObject go in globalTiles )
//		{
//			if ( go != null )
//				go.transform.localScale *= 0.1f;
//		}
	}
	
	public void InitTiles( Transform t )
	{
		foreach ( Transform transform in t )
		{
			//if ( transform.childCount > 0 )
			InitTiles ( transform );

			if ( !transform.gameObject.name.Contains("Tile") )
				continue;
			
			Vector3 pos = transform.position - this.transform.position;
			
			int tileX = Mathf.RoundToInt(pos.x / 0.8f);
			int tileY = Mathf.RoundToInt(pos.z / 0.8f);
			
			globalTiles[ tileX, tileY ] = transform.gameObject;
		}
	}
	
	public void InitPlayer() 
	{
		//player = playerContainer.GetComponentInChildren<Player>();
		camera = playerContainer.GetComponentInChildren<FollowSmooth>();

		playerContainer.transform.parent = transform;
		playerContainer.transform.position = startingPoint.position - player.transform.position;
		
		player.transform.parent = transform;
	}

	public Vector2 coordsFromPos(Vector3 pos)
	{
		int tileX = Mathf.RoundToInt(pos.x / 0.8f);
		int tileY = Mathf.RoundToInt(pos.z / 0.8f);

		return new Vector2( tileX, tileY );
	}
	
	public GameObject objFromPos(Vector3 pos)
	{
		if ( globalTiles == null )
			return null;
		
		int tileX = Mathf.RoundToInt(pos.x / 0.8f);
		int tileY = Mathf.RoundToInt(pos.z / 0.8f);
		
		if ( tileX > globalTiles.GetLength( 0 ) || tileX < 0 ) return null; 
		if ( tileY > globalTiles.GetLength( 1 ) || tileY < 0 ) return null; 
		
		return globalTiles[ tileX, tileY ];
	}
	
//	public GameObject getObjAtTilePos(Vector2 tilePos)
//	{
//		
//		for(int i =0 ; i < transform.childCount ; i++)
//		{
//			
//			
//			GameObject obj = (GameObject)(transform.GetChild(i)).gameObject;
//			Vector3 pos = obj.transform.position;
//			Rect r = new Rect(pos.x, pos.z, 0.8f, 0.8f);
//			if(r.Contains(tilePos))
//			{
//			
//				print("FOUND OBJECT");
//				return obj;
//			}
//		}
//		
//		return null;
//	}
}
