using UnityEngine;
using System.Collections;

public class GUIScreenFeedback : MonoBehaviour 
{
	public static GUIScreenFeedback i { get { return instance; } }
	private static GUIScreenFeedback instance;

	public GameObject prefabTriesLeft;
	public GameObject prefabGetToTheEntrance;
	public GameObject prefabTorch;
	
	void Awake()
	{
		instance = this;
	}
	
	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.Alpha0) )
		{
			ShowTriesLeft( 2 );
		}
	}
	
	public void ShowTriesLeft( int tries )
	{
		GameObject go = ShowFeedback( prefabTriesLeft );
		
		TextMesh text = go.GetComponentInChildren<TextMesh>();
		text.text = "" + tries + "tries left.";
	}

	public void ShowGetToTheEntrance()
	{
		ShowFeedback( prefabGetToTheEntrance );
	}

	public void ShowTorch()
	{
		ShowFeedback( prefabTorch );
	}
	
	public GameObject ShowFeedback( GameObject prefab )
	{
		GameObject go = (GameObject)Instantiate( prefab, prefab.transform.position, prefab.transform.rotation );
		go.transform.parent = transform;
		go.transform.localPosition = prefab.transform.localPosition;
		go.transform.localRotation = prefab.transform.localRotation;
		
		return go;
	}
}
