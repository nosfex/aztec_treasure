using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {
	
	public World worldContainer;
	
	public GameObject playerLeftPrefab;
	public GameObject playerRightPrefab;
	
	GameObject playerLeft;
	GameObject playerRight;
	
	
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
		playerLeft = (GameObject)Instantiate ( playerLeftPrefab );
		
		// Create future player
		playerRight = (GameObject)Instantiate ( playerRightPrefab );
		
		worldContainer.playerContainer = playerLeft.transform;
		worldContainer.InitPlayer();
		
		world2.playerContainer = playerRight.transform;
		world2.InitPlayer();
		
		SetLayerRecursively( go, LayerMask.NameToLayer( "Future" ) );
		SetLayerRecursively( worldContainer.gameObject, LayerMask.NameToLayer( "Past" ) );
		//SnapAssistant.i.snapEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
