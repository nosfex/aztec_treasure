using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	
	[HideInInspector] public Transform playerContainer;
	public Transform startingPoint;
	new public Camera camera;
	public Player player;
	
	public Transform deathYLimit;
	
	public void InitPlayer() 
	{
		//player = playerContainer.GetComponentInChildren<Player>();
		camera = playerContainer.GetComponentInChildren<Camera>();

		playerContainer.transform.parent = transform;
		playerContainer.transform.position = startingPoint.position - player.transform.position;
		
		player.transform.parent = transform;
	}
}
