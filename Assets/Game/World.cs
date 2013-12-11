using UnityEngine;
using System.Collections;

public class World : MonoBehaviour 
{
	
	[HideInInspector] public Transform playerContainer;
	public Transform startingPoint;
	new public Camera camera;
	public Player player;
	
	public Transform deathYLimit;
	
	public GameObject[,] globalTiles;
	
	void Awake()
	{
		globalTiles = new GameObject[DungeonBSP.WORLD_TILE_WIDTH, DungeonBSP.WORLD_TILE_HEIGHT];
	}
	
	public void InitTiles()
	{
		foreach ( Transform t in GetComponentsInChildren<Transform>() )
		{
			if ( !t.gameObject.name.Contains("Tile") )
				continue;
			
			int tileX = Mathf.RoundToInt(t.localPosition.x / 0.8f);
			int tileY = Mathf.RoundToInt(t.localPosition.z / 0.8f);
			
			globalTiles[ tileX, tileY ] = t.gameObject;
			//print (" x " + tileX + " ... y " + tileY );
		}		
	}
	
	public void InitPlayer() 
	{
		//player = playerContainer.GetComponentInChildren<Player>();
		camera = playerContainer.GetComponentInChildren<Camera>();

		playerContainer.transform.parent = transform;
		playerContainer.transform.position = startingPoint.position - player.transform.position;
		
		player.transform.parent = transform;
	}
	
	public GameObject objFromPos(Vector3 pos)
	{
		int tileX = Mathf.RoundToInt(pos.x / 0.8f);
		int tileY = Mathf.RoundToInt(pos.z / 0.8f);
		
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
