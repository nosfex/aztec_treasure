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
			Vector3 pos = obj.transform.position;
			Rect r = new Rect(pos.x, pos.z, 0.8f, 0.8f);
			if(r.Contains(tilePos))
			{
			
				print("FOUND OBJECT");
				return obj;
			}
		}
		
		return null;
	}
}
