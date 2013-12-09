using UnityEngine;
using System.Collections;

public class World : MonoBehaviour 
{
	
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
	
	public GameObject getObjAtTilePos(Vector2 tilePos)
	{
		
		for(int i =0 ; i < transform.childCount ; i++)
		{
			
			
			GameObject obj = (GameObject)(transform.GetChild(i)).gameObject;
			if(obj.transform.position.x == tilePos.x && obj.transform.position.z == tilePos.y)
			{
				print("FOUND OBJECT");
				return obj;
			}
		}
		
		return null;
	}
}
