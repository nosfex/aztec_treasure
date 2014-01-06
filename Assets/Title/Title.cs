using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour 
{
	public Camera[] cameraWaypoints;
	public Transform[] words;
	public GameObject part2;
	
	void Start() 
	{
		timerInterval = interval;
		GoToIndex( 0 );
	}
	
	int wordIndex = 0;
	Camera currentWp;
	
	public float interval = 2.0f;
	float timerInterval = 0;
	
	void OnFovTween( float fov )
	{
		Camera.main.fieldOfView = fov;
	}
	
	void GoToIndex( int index )
	{
		//Transform camTransform = Camera.main.transform;
		currentWp = cameraWaypoints[ index ];
		
		words[ wordIndex ].gameObject.SetActive( true );
		words[ wordIndex ].position += Vector3.up * 1.0f;
		words[ wordIndex ].Rotate (  Random.insideUnitSphere * 5f );
		
		//camTransform.position += curWp.position;
	}
	
	void UpdateCamera()
	{
		Camera cam = Camera.main;
		
		cam.transform.position += (currentWp.transform.position - cam.transform.position) * 0.3f;
		cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, currentWp.transform.rotation, 0.3f);
		
		cam.fieldOfView += (currentWp.fieldOfView - cam.fieldOfView) * 0.3f;		
	}
	
	bool tweenCamera = true;
	
	void Update() 
	{
		timerInterval -= Time.deltaTime;
		if ( timerInterval < 0 && wordIndex < words.Length - 1 )
		{
			timerInterval = interval;
			wordIndex++;
			GoToIndex( wordIndex );
		}
		
		if ( timerInterval <= 0 && wordIndex == words.Length - 1 )
		{
			tweenCamera = false;
			Camera.main.transform.position += Camera.main.transform.forward * -0.001f;
			//wordIndex++;
			//timerInterval = interval * 2;
			
		}
		
		if ( timerInterval <= -4 && wordIndex == words.Length - 1 )
		{
			part2.SetActive(true);
			gameObject.SetActive(false);
		}
		
		
		if ( tweenCamera )
			UpdateCamera();
		
		//iTween.MoveTo ( Camera.main.gameObject, iTween.Hash( "position", curWp.transform.position, "time", 1.0f ) );
		//iTween.RotateTo	( Camera.main.gameObject, iTween.Hash( "rotation", curWp.transform.rotation, "time", 1.0f ) );
		//iTween.ValueTo ( gameObject, iTween.Hash( "from", Camera.main.fieldOfView, "to", curWp.fieldOfView, "time", 1.0f, "onupdate", "OnFovTween" ) );


	}
}
