using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {
	
	public World worldContainer;
	
	public GameObject playerLeftPrefab;
	public GameObject playerRightPrefab;
	
	GameObject playerLeft;
	GameObject playerRight;
	// Use this for initialization
	void Start () 
	{
		SnapAssistant.i.snapEnabled = false;
		GameObject go = (GameObject)Instantiate( worldContainer.gameObject, worldContainer.transform.position + (Vector3.right * (0.2f * 200f)), Quaternion.identity );
		World world2 = go.GetComponent<World>();
		
		playerLeft = (GameObject)Instantiate ( playerLeftPrefab );
		playerRight = (GameObject)Instantiate ( playerRightPrefab );
		
		worldContainer.playerContainer = playerLeft.transform;
		worldContainer.InitPlayer();
		
		world2.playerContainer = playerRight.transform;
		world2.InitPlayer();
		//SnapAssistant.i.snapEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
