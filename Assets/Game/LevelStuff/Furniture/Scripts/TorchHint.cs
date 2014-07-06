using UnityEngine;
using System.Collections;

public class TorchHint : MonoBehaviour 
{
	//float timer;
	float gravity, vely;
	
	void Start()
	{
	}
	
	void Update()
	{
		//timer -= Time.deltaTime;
/*		
		if ( timer <= 0 )
		{
			timer = .5f;
		}
				 */
		vely += gravity;
		gravity += 0.001f;
		
		if ( vely > 0 && gravity > 0 )
		{
			gravity = -0.02f;
			vely = 0;
		}
		//	gravity *= -0.5f;
		
		//if ( gravity < 0.02f && gravity > -0.02f )
		//	gravity = 0;
		
		transform.localPosition = new Vector3( 0, 0.2f -vely, 0 );
	}
}
