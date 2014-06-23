using UnityEngine;
using System.Collections;

public class GUIHearts : MonoBehaviour {

	public SpriteAnimator[] hearts;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		for ( int i = 0;  i < hearts.Length; i++ )
		{
			if ( GameDirector.i.playerRight.hearts >= i + 1 )
				hearts[i].PlayAnim("HeartFull");
			else
				hearts[i].PlayAnim("HeartEmpty");
		}
	}
}
