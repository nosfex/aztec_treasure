using UnityEngine;
using System.Collections;

public class CosaCube : MonoBehaviour {
	
	public GameObject cosaPrefab;
	public GameObject cameraCenter;
	// Use this for initialization
	void Start () {
		Camera.main.transform.position = new Vector3( 10, 10, 10 );
		for ( int x = 0; x < 10; x ++ )
		{
			for ( int y = 0; y < 10; y ++ )
			{
				for ( int z = 0; z < 10; z ++ )
				{
					GameObject o  = (GameObject )Instantiate ( cosaPrefab, new Vector3( x * 2,  y * 2, z * 2 ), Quaternion.identity );
					o.transform.parent = cameraCenter.transform;
				}
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
