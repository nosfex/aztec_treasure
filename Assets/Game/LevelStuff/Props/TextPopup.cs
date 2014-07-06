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

		iTween.ScaleFrom ( gameObject, iTween.Hash ( "scale", Vector3.one * 0.001f, "time", 0.3f, "easetype", iTween.EaseType.easeOutBack ) );
	}
	float timer = 0;
	// Update is called once per frame
	void Update () 
	{
		transform.position += Vector3.up * 0.002f;
		transform.rotation = Quaternion.LookRotation( transform.position - Camera.main.transform.position );
		
		timer += Time.deltaTime;
		if ( timer > 1.0f )
			alpha -= 0.02f;
		
		alpha = Mathf.Clamp01 ( alpha );
		
		text.color = new Color( text.color.r, text.color.g, text.color.b, alpha );
		shadow.color = new Color( shadow.color.r, shadow.color.g, shadow.color.b, alpha );
		
		if ( alpha == 0 )
			Destroy ( gameObject );

		shadow.text = text.text;
		
		
	}
}
