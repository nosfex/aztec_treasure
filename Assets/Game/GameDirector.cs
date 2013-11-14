using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {
	
	public static GameDirector i { get { return instance; } }
	private static GameDirector instance;
	
	public World worldContainer;
	
	public GameObject playerLeftPrefab;
	public GameObject playerRightPrefab;
	
	[HideInInspector] public Player playerLeft;
	[HideInInspector] public Player playerRight;
	
	
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
	
	void Awake ()
	{
		instance = this;
	}
	// Use this for initialization
	void Start () 
	{
		SnapAssistant.i.snapEnabled = false;
		
		// CREATE FUTURE.
		GameObject go = (GameObject)Instantiate( worldContainer.gameObject, 
			worldContainer.transform.position + (Vector3.right * (0.2f * 50f)), 
			Quaternion.identity );
		
		World world2 = go.GetComponent<World>();
		
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
		
		SetLayerRecursively( go, LayerMask.NameToLayer( "Future" ) );
		SetLayerRecursively( worldContainer.gameObject, LayerMask.NameToLayer( "Past" ) );
		//SnapAssistant.i.snapEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
