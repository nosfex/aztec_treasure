using UnityEngine;
using System.Collections;

public class TextPopup : MonoBehaviour 
{
	public string caption 
	{ 
		set { text.text = value; } 
		get { return text.text; } 
	}
	
	TextMesh text;
	public TextMesh shadow;
	
	float alpha;
	
	void Awake()
	{
		alpha = 1.0f;
		text = GetComponent<TextMesh>();		
	}
	// Use this for initialization
	void Start() 
	{

		iTween.ScaleFrom ( gameObject, iTween.Hash ( "scale", Vector3.one * 0.001f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBack ) );
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position += Vector3.up * 0.01f;
		alpha -= 0.02f;
		
		alpha = Mathf.Clamp01 ( alpha );
		
		if ( alpha == 0 )
			Destroy ( gameObject );
		
		text.color = new Color( text.color.r, text.color.g, text.color.b, alpha );
		
		shadow.text = text.text;
		
		
	}
}
