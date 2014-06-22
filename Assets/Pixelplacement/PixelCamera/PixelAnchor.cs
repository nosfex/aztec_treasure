/*---------------------------------------------------------------------------------
Anchors attached object to screen corners and center. If your object's localScale
isn't being used to create pixel size (i.e. a scale of 10x10x0 to get a 10 x 10
pixel on-screen object) or if you want to create a marger from the anchored
position turn off sizeFromScale and punch in your manual size values.

Author:	Bob Berkebile
Email:	bobb@pixelplacement.com
---------------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

[ ExecuteInEditMode ]
[ AddComponentMenu ( "Pixelplacement/PixelAnchor" ) ]
public class PixelAnchor : MonoBehaviour {
	
	//-----------------------------------------------------------------------------
	// Public Variables
	//-----------------------------------------------------------------------------
	
	public TextAnchor anchor;
	//public bool sizeFromScale = true;
	public Vector3 normalizedOffset;
	
	//-----------------------------------------------------------------------------
	// Init
	//-----------------------------------------------------------------------------
	
	void Awake(){
		Calculate();
	}
	
	//-----------------------------------------------------------------------------
	// Update
	//-----------------------------------------------------------------------------
	
	float screenWidth
	{
		get 
		{
			if ( Application.isPlaying )
				return Screen.width;
			else 
				return 1050f;
		} 
	}

	float screenHeight
	{
		get 
		{
			if ( Application.isPlaying )
				return Screen.height;
			else 
				return 600f;
		} 
	}
	
	void LateUpdate(){
		
		if ( Time.frameCount % 10 != 0 ) {
			return;
		}
		

		Calculate();
	}
	
	//-----------------------------------------------------------------------------
	// Private Methods
	//-----------------------------------------------------------------------------
	void Calculate()
	{
		float currentZ = transform.localPosition.z;
		Vector3 offset = /*sizeFromScale ? transform.localScale : */normalizedOffset;
		offset = new Vector3( offset.x * screenWidth, offset.y * screenHeight, 0 );
		Vector3 anchorPosition = new Vector3( 0, 0, currentZ );
		
		switch ( anchor )
		{
		case TextAnchor.UpperLeft:
			anchorPosition = new Vector3( 0, 0, currentZ );
			break;
			
		case TextAnchor.UpperCenter:
			anchorPosition = new Vector3( screenWidth / 2, 0, currentZ );
			break;
			
		case TextAnchor.UpperRight:
			anchorPosition = new Vector3( screenWidth, 0, currentZ );
			break;
		
		case TextAnchor.MiddleLeft:
			anchorPosition = new Vector3( 0, ( screenHeight / 2 ) * -1, currentZ );
			break;
			
		case TextAnchor.MiddleCenter:
			anchorPosition = new Vector3( screenWidth / 2, ( screenHeight / 2) * -1, currentZ );
			break;
			
		case TextAnchor.MiddleRight:
			anchorPosition = new Vector3( screenWidth, ( screenHeight / 2) * -1, currentZ );
			break;
			
		case TextAnchor.LowerLeft:
			anchorPosition = new Vector3( 0, ( screenHeight  ) * -1, currentZ );
			break;
			
		case TextAnchor.LowerCenter:
			anchorPosition = new Vector3( screenWidth / 2, ( screenHeight ) * -1, currentZ );
			break;
			
		case TextAnchor.LowerRight:
			anchorPosition = new Vector3( screenWidth, ( screenHeight ) * -1, currentZ );
			break;
		}	
		anchorPosition += offset;
		transform.localPosition = anchorPosition;
	}
}