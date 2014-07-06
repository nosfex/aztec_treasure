using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {
	
	public static GameDirector i { get { return instance; } }
	private static GameDirector instance;
	
	public World worldContainer;
	
	///public GameObject playerLeftPrefab;
	//public GameObject playerRightPrefab;
	
	public GameObject enemySpawnerPrefab;
	
	//[HideInInspector] public Player playerLeft;
	public Player playerRight;
	
	//[HideInInspector] public World worldLeft;
	public World worldRight;

	
	public int maxHearts;
	public int maxLives = 3;
	
	public GameObject textPopupPrefab;
	
	public Transform guiContainer;
	
	public GameObject prefabVictoryGiver;
	public DungeonBSP dungeonGenerator;
	
	public GameObject finalTreasure;
	public GameObject guideBlobPrefab;
	
	public GameObject[] spawnPrefabsList;
	
	public Material pastWall;
	public Material futureWall;
	public Material pastFloor;
	public Material futureFloor;
	
	public bool forceHighQuality;
	
	public AudioSource sfxEarthquake;
	
	public void SpawnGuideBlob()
	{
		//GameObject o = (GameObject)Instantiate ( guideBlobPrefab );
	}
	
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
	
	[HideInInspector] public GameObject finalTreasureRight;
	
	// Use this for initialization
	void Start () 
	{
		SnapAssistant.i.snapEnabled = false;
		
		Vector3 startRoom = Vector3.zero;
		Vector3 endRoom = Vector3.zero;

//		if ( dungeonGenerator != null )
//		{
//			dungeonGenerator.BuildDungeon( worldContainer.transform );
//			startRoom = dungeonGenerator.startRoom.room.getCenterTile().transform.position + (Vector3.up * 0.8f);
//			endRoom = dungeonGenerator.endRoom.room.getCenterTile().transform.position + (Vector3.up * 0.8f);;
//			worldContainer.startingPoint.position = startRoom;
//		}
//
//		
//		// CREATE FUTURE.
//		GameObject go = (GameObject)Instantiate( worldContainer.gameObject, 
//			worldContainer.transform.position, 
//			Quaternion.identity );
//		
//		World world2 = go.GetComponent<World>();
//
//		if ( dungeonGenerator != null )
//		{
//			world2.startingPoint.position = startRoom;
//			
////			GameObject final1 = (GameObject)Instantiate( finalTreasure, endRoom, Quaternion.identity );
////			final1.transform.parent = worldContainer.transform;
//	
//			finalTreasureRight = (GameObject)Instantiate( finalTreasure, endRoom, Quaternion.identity );
//			finalTreasureRight.transform.parent = world2.transform;
//		}
//		
//		go.transform.position += (Vector3.right * (0.2f * 500f));
//		
//		RemoveObjectsByLayerRecursively( go, LayerMask.NameToLayer("Past") );
//		SetLayerRecursively( go, LayerMask.NameToLayer( "Future" ) );
//		
//		RemoveObjectsByLayerRecursively( worldContainer.gameObject, LayerMask.NameToLayer("Future") );
//		SetLayerRecursively( worldContainer.gameObject, LayerMask.NameToLayer( "Past" ) );		
//		
//		
//		// Create past player
////		GameObject gol = (GameObject)Instantiate( playerLeftPrefab );
////		playerLeft = gol.GetComponentInChildren<Player>();
//		
//		// Create future player
//		GameObject gor = (GameObject)Instantiate ( playerRightPrefab );
//		playerRight = gor.GetComponentInChildren<Player>();
//		
//		worldContainer.playerContainer = gol.transform;
//		worldContainer.player = playerLeft;
//		worldContainer.InitPlayer();
//		
//		world2.playerContainer = gor.transform;
//		world2.player = playerRight;
//		world2.InitPlayer();
//		
//		
//		worldRight = world2;
//		worldLeft = worldContainer;
//		
//		
//		foreach ( Renderer r in world2.GetComponentsInChildren<Renderer>() )
//		{
//			if ( r.sharedMaterial == pastWall )
//				r.sharedMaterial = futureWall;
//			//else if ( r.sharedMaterial == pastFloor )
//			//	r.sharedMaterial = futureFloor;
//		}
		
		Invoke( "InitTiles", .1f );
		//SnapAssistant.i.snapEnabled = true;
		InitHighQuality( false );
	}
	
	void InitTiles()
	{
//		if ( dungeonGenerator != null )
//		{
//			worldRight.InitTiles();
//			worldLeft.InitTiles();
//		}		
	}
	
	bool highQualityEnabled = true;
	
	float hqTimer = 0;
	float fpsThreshold = 30;
	float timeThreshold = 5.0f;
	bool qualityAlreadySet = false;
	
	void InitHighQuality( bool hq )
	{
		if ( qualityAlreadySet )
			return;
		
		if ( hq == highQualityEnabled )
			return;
		
		highQualityEnabled = hq;
		hqTimer = 0;
		qualityAlreadySet = true;
		
		if ( forceHighQuality )
			highQualityEnabled=true;
		
		if ( highQualityEnabled )
		{
			QualitySettings.SetQualityLevel( 1 );	
			Debug.Log ("Setting HI-Q");
		}
		else 
		{
			QualitySettings.SetQualityLevel( 0 );	
			Debug.Log ("Setting LO-Q");
		}
	}
	
	void UpdateQualityAssistant()
	{
		float fps = 1.0f / Time.deltaTime; 
		//print ("FPS = " + fps );
		if ( highQualityEnabled )
		{
			if ( fps < fpsThreshold )
				hqTimer += Time.deltaTime;
			else
				hqTimer -= Time.deltaTime;
			
			hqTimer = Mathf.Clamp( hqTimer, 0, timeThreshold );
			
			if ( hqTimer >= timeThreshold )
				InitHighQuality( false );
		}
		else 
		{
			if ( fps >= fpsThreshold )
				hqTimer += Time.deltaTime;
			else
				hqTimer -= Time.deltaTime;
			
			hqTimer = Mathf.Clamp( hqTimer, 0, timeThreshold );

			if ( hqTimer >= timeThreshold )
				InitHighQuality( true );
		}
	}
	// Update is called once per frame
	void Update () 
	{
		//UpdateQualityAssistant();
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
	
	
//	public void OnRightWins()
//	{
//		GameObject o = (GameObject)Instantiate( prefabVictoryGiver, 
//			prefabVictoryGiver.transform.position, 
//			prefabVictoryGiver.transform.rotation );
//		
//		VictoryGiver v = o.GetComponentInChildren<VictoryGiver>();
//		
//		v.winnerCamera = worldRight.camera.camera;
//		v.loserCamera = worldLeft.camera.camera;
//	}
//
//	public void OnLeftWins()
//	{
//		GameObject o = (GameObject)Instantiate( prefabVictoryGiver, 
//			prefabVictoryGiver.transform.position, 
//			prefabVictoryGiver.transform.rotation );
//		
//		VictoryGiver v = o.GetComponentInChildren<VictoryGiver>();
//		
//		v.winnerCamera = worldLeft.camera.camera;
//		v.loserCamera = worldRight.camera.camera;
//	}
	
}
