using UnityEngine;
using System.Collections;

public class GuideBlob : MonoBehaviour 
{
	Material blobMaterial;
	void Start()
	{
		transform.parent = GameDirector.i.worldLeft.transform;
		transform.localPosition = GameDirector.i.playerRight.transform.localPosition;
		blobMaterial = GetComponentInChildren<Renderer>().material;
		transform.localScale = Vector3.one * 0.01f;
	}
	float timer= 0;
	void LateUpdate () 
	{
		timer += Time.deltaTime;
		const float timeLimit = 10.0f;
		blobMaterial.color = new Color( 1.0f, 1.0f, 1.0f, Mathf.Clamp01(((timeLimit - timer) / timeLimit) - 0.5f ) );
		if ( transform.localScale.x < 1.0f )
			transform.localScale += Vector3.one * 0.1f;
		if ( timer >= timeLimit )
			Destroy ( gameObject );
		RaycastHit[] hits = Physics.RaycastAll( transform.parent.position, Vector3.down * 5f );
		
		//Debug.DrawRay( transform.position, Vector3.down * 10f );
	
		for ( int i = 0; i < hits.Length; i++ )
		{
			RaycastHit hit = hits[i];
			
			if ( hit.collider.tag == "Floor" )
			{
				Vector3 t = transform.position;
				t.y = hit.collider.transform.position.y + 0.45f;
				transform.position = t;
				transform.localScale = Vector3.one * Mathf.Lerp( 1.0f, .5f, Mathf.Clamp01 ( Vector3.Distance( transform.position, transform.parent.position ) / 1.0f ) );
				//Debug.Log ("hit ",/hit.collider.gameObject );
			}
		}
	}
}
