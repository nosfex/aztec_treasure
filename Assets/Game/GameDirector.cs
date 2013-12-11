﻿using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {
	
	public static GameDirector i { get { return instance; } }
	private static GameDirector instance;
	
	public World worldContainer;
	
	public GameObject playerLeftPrefab;
	public GameObject playerRightPrefab;
	
	public GameObject enemySpawnerPrefab;
	
	[HideInInspector] public Player playerLeft;
	[HideInInspector] public Player playerRight;
	
	[HideInInspector] public World worldLeft;
	[HideInInspector] public World worldRight;

	
	public int maxHearts;
	
	public GameObject textPopupPrefab;
	
	public DungeonBSP dungeonGenerator;
	
	public GameObject[] spawnPrefabsList;
	
	public void ShowTextPopup( GameObject source, float yOffset, string text )
	{
		GameObject prefab = (GameObject)(GameObject.Instantiate ( textPopupPrefab, source.transform.position + (Vector3.up * yOffset), Quaternion.identity ));
		TextPopup popup = prefab.GetComponentInChildren<TextPopup>();
		popup.caption = text;
	}
	
	static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
            return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;

            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
	
	static void RemoveObjectsByLayerRecursively( GameObject obj, int layer )
	{
        if (null == obj)
            return;

        if ( obj.layer == layer )
		{
			Destroy( obj );
			return;
		}

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;

            RemoveObjectsByLayerRecursively(child.gameObject, layer);
        }
	}
	
	void Awake ()
	{
		instance = this;
	}
	
	
	// Use this for initialization
	void Start () 
	{
		SnapAssistant.i.snapEnabled = false;
		
		if ( dungeonGenerator != null )
		{
			dungeonGenerator.BuildDungeon( worldContainer.transform );
			worldContainer.startingPoint.position = dungeonGenerator.startRoom.room.getCenterTile().transform.position + (Vector3.up * 2);
		}
		
		// CREATE FUTURE.
		GameObject go = (GameObject)Instantiate( worldContainer.gameObject, 
			worldContainer.transform.position, 
			Quaternion.identity );
		
		World world2 = go.GetComponent<World>();

		world2.startingPoint.position = dungeonGenerator.endRoom.room.getCenterTile().transform.position + (Vector3.up * 2);

		
		go.transform.position += (Vector3.right * (0.2f * 500f));
		
		RemoveObjectsByLayerRecursively( go, LayerMask.NameToLayer("Past") );
		SetLayerRecursively( go, LayerMask.NameToLayer( "Future" ) );
		
		RemoveObjectsByLayerRecursively( worldContainer.gameObject, LayerMask.NameToLayer("Future") );
		SetLayerRecursively( worldContainer.gameObject, LayerMask.NameToLayer( "Past" ) );		
		
		
		// Create past player
		GameObject gol = (GameObject)Instantiate( playerLeftPrefab );
		playerLeft = gol.GetComponentInChildren<Player>();
		
		// Create future player
		GameObject gor = (GameObject)Instantiate ( playerRightPrefab );
		playerRight = gor.GetComponentInChildren<Player>();
		
		worldContainer.playerContainer = gol.transform;
		worldContainer.player = playerLeft;
		worldContainer.InitPlayer();
		
		world2.playerContainer = gor.transform;
		world2.player = playerRight;
		world2.InitPlayer();
		
		
		worldRight = world2;
		worldLeft = worldContainer;
		
		if ( dungeonGenerator )
		{
			worldRight.InitTiles();
			worldLeft.InitTiles();
		}
		//SnapAssistant.i.snapEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public GameObject findMyPrefab( GameObject which )
	{
		int length = spawnPrefabsList.Length;
		
		for ( int i = 0; i < length; i++ )
		{
			GameObject go = spawnPrefabsList[i];
			
			string name = which.name;
			int cloneIndex = name.LastIndexOf("(Clone)");
			
			if ( cloneIndex > -1 )
				name = name.Remove( cloneIndex );
			
			if ( name == go.name )
				return go;		
		}
		
		return null;
	}
	
}
