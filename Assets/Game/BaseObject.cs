using UnityEngine;
using System.Collections;

public class BaseObject : MonoBehaviour 
{
	
	public World worldOwner;
	
	[HideInInspector] public Vector3 accel = Vector3.zero;
	public float frictionCoef = 0.97f;
	
	[HideInInspector] public Vector3 velocity = Vector3.zero;
	
	//[HideInInspector] 
	public bool gravityEnabled = true;
	
	Vector3 gravity;
	
	public bool collisionEnabled = true;
	
	virtual protected void Start()
	{
	
		if ( transform.root != transform )
		{
			
			//So, now try to detect if child or grand-child of world.
			World myWorld = transform.parent.GetComponent<World>();
			
			if ( myWorld == null )
				myWorld = transform.parent.parent.GetComponent<World>();
			
			if ( myWorld != null )
			{
				
				worldOwner = myWorld;					
			}
		}
	}
	
	protected void LateUpdate()
	{
		float frameRatio = (Time.deltaTime / 0.016f);
		
		if ( gravityEnabled )
		{
			transform.position -= gravity * frameRatio;

			if ( transform.position.y > 0.2f )
			{
				
				gravity += ( Vector3.up * 0.0015f );
			}
			else 
			{
				transform.position = new Vector3( transform.position.x, 0.2f, transform.position.z );
				gravity *= -0.7f;//Vector3.zero;
				
				if ( gravity.magnitude < 0.001f )
					gravity = Vector3.zero;
				
				collisionEnabled = true;				
			}
			
		}

		velocity += accel * frameRatio;
		Vector3 velocityDif = (velocity * frictionCoef) - velocity;
		velocity += velocityDif * frameRatio;
		transform.position += velocity * frameRatio;

		accel = Vector3.zero;
	}
}
