using UnityEngine;
using System.Collections;

public class ShadowBlob : MonoBehaviour 
{
	new Renderer renderer;

	void Start()
	{
		renderer = GetComponentInChildren<Renderer>();
	}
	
	void LateUpdate () 
	{
		RaycastHit[] hits = Physics.RaycastAll( transform.parent.position, Vector3.down * 5f );


		for ( int i = 0; i < hits.Length; i++ )
		{
			RaycastHit hit = hits[i];
			
			if ( hit.collider.tag == "Floor" )
			{
				Vector3 t = transform.position;
				t.y = hit.collider.transform.position.y + 0.45f;
				transform.position = t;
				transform.localScale = Vector3.one * Mathf.Lerp( 1.0f, .5f, Mathf.Clamp01 ( Vector3.Distance( transform.position, transform.parent.position ) / 1.0f ) );
				renderer.enabled = true;
				return;
				//Debug.Log ("hit ",/hit.collider.gameObject );
			}
		}

		renderer.enabled = false;
	}
}
