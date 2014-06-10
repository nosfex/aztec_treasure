using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AztecPlayer : Player 
{
	public int trapCurrency = 20;
	
	GameObject highlightedFloor = null;
	GameObject highlightedWall = null;
	
	public GameObject constructionCursor = null;
	GameObject cursorObj = null;
	Material floorMat = null;
	Material wallMat = null;
	
	public float maxCurrencyCooldown = 1.0f;
	int MaxTraps = 6;
	float currencyCooldown = 0.0f;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start();
		trapsPlaced = new  List<Vector2>();
	}
	
	KeyCode placeTrap = KeyCode.T;
	KeyCode cycleLeft = KeyCode.Q;
	KeyCode cycleRight = KeyCode.E;
	
	public int currentTrapIndex = 0;
	
	List<Vector2> trapsPlaced;
	
	public void PlaceTrap( Trap trap )
	{
		if( trap.CanBePlaced() )
		{
			Transform t = new GameObject().transform;
			t.position = transform.position + (Vector3.up  * 0.2f);
			t.rotation = transform.rotation;
			t.localPosition = transform.localPosition;
			
			Vector2 tileXY = GameDirector.i.worldRight.coordsFromPos(t.localPosition);
			t.position = GameDirector.i.worldRight.globalTiles[(int)tileXY.x, (int)tileXY.y].transform.position +  (Vector3.up  * 0.2f);
			t.localPosition = GameDirector.i.worldRight.globalTiles[(int)tileXY.x, (int)tileXY.y].transform.localPosition +  (Vector3.up  * 0.8f);
			if(checkFreeArea(t))
			{
				
				Transform signT = new GameObject().transform;
				Vector2 signTileXY = GameDirector.i.worldLeft.coordsFromPos(transform.localPosition);
				signT.position = new Vector3(signTileXY.x *0.8f, transform.position.y, signTileXY.y * 0.8f);
				signT.rotation = transform.rotation;
				DelayedSpawner.i.addSpawnData( trap.trapPrefab, t, 3, null);
				PlaceSign( trap.signPrefab, signT.position, Quaternion.Euler(90, 0, 0));
				trapsPlaced.Add(GameDirector.i.worldRight.coordsFromPos(t.transform.position));
				trapCurrency -= trap.price;
				GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + trap.price );
			}
			else
			{
				GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No room for another trap!" );	
			}	
		}
	}

	void PlaceSign(GameObject prefab, Vector3 posAt, Quaternion eulerRot, Vector3 offset = new Vector3())
	{
		GameObject obj = (GameObject)MonoBehaviour.Instantiate( prefab, posAt, eulerRot );
		obj.transform.parent = GameDirector.i.worldLeft.transform;
		obj.transform.position -= (Vector3.up * 0.1f) + offset;
		
		GameDirector.i.worldLeft.camera.Shake( 0.1f, 0.2f );
	}
	
	
	public void PlaceTrapAtPos(GameObject prefab, Transform t)
	{
		GameObject vine2 = (GameObject)MonoBehaviour.Instantiate(prefab, t.position + Vector3.up, t.rotation);
		vine2.transform.parent = ((World)GameDirector.i.worldRight).transform;
		vine2.name = vine2.name.TrimEnd( "(Clone)" );
		vine2.transform.localPosition = t.localPosition;
	}
	
	override protected void Update ()  
	{
		canJump = false;
		base.Update();

		if ( switchSensor.sensedObject != null && switchSensor.sensedObject.isSwitch )
		{
			return;
		}
		
		if(currencyCooldown >= maxCurrencyCooldown)
		{
			currencyCooldown = 0.0f;
			trapCurrency += 10;
		}

		currencyCooldown += Time.deltaTime;
		
		Trap trap = GUITrapSelector.i.traps[ currentTrapIndex ];
		
		
		if(Input.GetKeyDown(cycleLeft))
		{
			currentTrapIndex--;

			if(currentTrapIndex < 0)
				currentTrapIndex = 5;
			
			trap = GUITrapSelector.i.traps[ currentTrapIndex ];
			
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, trap.name );
		}
		if(Input.GetKeyDown(cycleRight))
		{
			currentTrapIndex++;

			if(currentTrapIndex > 5)
				currentTrapIndex = 0;
			
			trap = GUITrapSelector.i.traps[ currentTrapIndex ];
			
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, trap.name );
		}
		
		if(Input.GetKeyDown(placeTrap))
		{
			{
				GameObject obj2 = GameDirector.i.worldRight.objFromPos( transform.localPosition );
				
				if(obj2 != null)
				{
					if ( obj2.name == "HardFloorTile" )
					{
						GameDirector.i.ShowTextPopup( gameObject, 0.8f, "Can't place here." );
						return;
					}
				}
			}
			
			if(trap.name == "Vines" || 
				trap.name == "Skelly" || 
				trap.name == "Bat" ||
				trap.name == "Ranged" ) 
			{
				PlaceTrap( trap );
			}		
			
			if(trap.name == "Falling Floor")
			{
				if(trap.CanBePlaced())
				{
					
					GameObject obj2 = GameDirector.i.worldRight.objFromPos( transform.localPosition );
					
					//Debug.Log ("obj2 = " + obj2 );
					if(obj2 == null) 
						return;
					
					//Debug.Log ("obj2.name = " + obj2.name );
					if ( obj2.name == trap.trapPrefab.name )
					{
						GameDirector.i.ShowTextPopup( gameObject, 0.8f, "Already placed." );
						return;
					}
					
					Transform t = obj2.transform;
									
					Destroy(obj2);
					obj2 = (GameObject)MonoBehaviour.Instantiate(trap.trapPrefab, t.position, t.rotation);
					obj2.transform.parent = GameDirector.i.worldRight.transform;
					obj2.name = obj2.name.TrimEnd( "(Clone)" );
					
					Vector2 tileXY = GameDirector.i.worldRight.coordsFromPos( obj2.transform.localPosition );
					GameDirector.i.worldRight.globalTiles[ (int)tileXY.x, (int)tileXY.y ] = obj2;
					if(checkFreeArea(tileXY))
					{
						
						Transform signT = new GameObject().transform;
						Vector2 signTileXY = GameDirector.i.worldLeft.coordsFromPos(transform.localPosition);
						signT.position = new Vector3(signTileXY.x *0.8f, transform.position.y, signTileXY.y * 0.8f);
						signT.rotation = transform.rotation;
						
						
						trapsPlaced.Add(tileXY);
						GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + trap.price );
						trapCurrency -= trap.price;
						
					
						PlaceSign(trap.signPrefab, signT.position,  Quaternion.Euler(90, 0, 0));
					}
					else
					{
						GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No room for another trap!" );	
					}
						
				}
			}
			

			
			if(trap.name == "Wall Dart")
			{
				if ( trap.CanBePlaced() )
				{
					RaycastHit r;
					

					if(Physics.Raycast(transform.position + Vector3.forward  *0.4f,  Vector3.forward, out r))
					{
						GameObject obj2 = GameDirector.i.worldRight.objFromPos( r.point + Vector3.forward * 0.3f );
					
						if(obj2 == null) 
							return;
						
						if(obj2.name == "WallTorchTile")
						{
							GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No room for another trap!" );	
							return;
						}
				
						Transform t = new GameObject().transform;
						t.position = obj2.transform.position;
						t.rotation = obj2.transform.rotation;
						t.localPosition = obj2.transform.localPosition; 
						if(checkFreeArea(t))
						{
							trapsPlaced.Add(GameDirector.i.worldRight.coordsFromPos(t.transform.position));
							DelayedSpawner.i.addSpawnData(trap.trapPrefab, t, 3, obj2);
							
							PlaceSign(trap.signPrefab, r.collider.transform.position,  Quaternion.identity, new Vector3(0, 0, 0.45f) );
							GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + trap.price );
							
						}
						else
						{
							GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No room for another trap!" );	
						}
						
					}
				}
				
			}
		
		
		}
		
		if(currentTrapIndex  == 5)
		{
			RaycastHit r;
			
			if(Physics.Raycast(transform.position + Vector3.up  *0.4f,  Vector3.forward, out r))
			{
				GameObject obj = GameDirector.i.worldLeft.objFromPos( r.point + Vector3.forward * 0.4f );
			
				if(obj != null) 
				{
					if(obj.name == "WallTorchTile")
					{
						return;
					}
					//highlightArea(obj2, Color.green, ref highlightedWall, ref wallMat);
					PlaceCursor(obj.transform.position, obj.transform.rotation, Vector3.back);
					//PlaceCursor(obj.transform.position, Quaternion.Euler (90,0,0), Vector3.back);
				}
			}
		}
		else if(currentTrapIndex !=5)
		{
			GameObject obj = GameDirector.i.worldLeft.objFromPos(transform.position);
			if(obj != null)
			{
			//	highlightArea(obj, Color.green, ref highlightedFloor, ref floorMat);
				//PlaceCursor(obj.transform.position, obj.transform.rotation, Vector3.up * 0.6f);
				PlaceCursor(obj.transform.position, Quaternion.Euler (90,0,0), Vector3.up * 0.6f);

			}
		}
//		
//		else //if(direction != Vector3.forward)
//		{
//	
//			if(highlightedWall != null)
//			{
//				
//				highlightedWall.renderer.material = wallMat;
//		
//			}
//		
//		}
//		
//		
//		if(currentTrapIndex !=5)
//		{
//			GameObject obj = GameDirector.i.worldLeft.objFromPos(transform.position);
//			if(obj != null)
//			{
//				highlightArea(obj, Color.green, ref highlightedFloor, ref floorMat);
//			}
//		}
//		else
//		{
//			if(highlightedFloor != null && highlightedFloor.renderer.material != floorMat)
//			{
//				
//				highlightedFloor.renderer.material = floorMat;
//		
//			}
//		}
	}
	
	void PlaceCursor(Vector3 posAt, Quaternion eulerRot, Vector3 offset = new Vector3())
	{
		if(cursorObj == null)
		{
			cursorObj  = (GameObject)Instantiate(constructionCursor, posAt, eulerRot);
			cursorObj.transform.position += (Vector3.up * 0.1f) + offset;
			cursorObj.transform.parent = GameDirector.i.worldLeft.transform;
		}
		else
		{
			
			cursorObj.transform.position = posAt + (Vector3.up * 0.1f) + offset;
			cursorObj.transform.rotation = eulerRot;
			//cursorObj.transform.parent = GameDirector.i.worldLeft.transform;
		}
	}
	
	
	bool checkFreeArea(Transform t) 
	{
		Vector2 coords = GameDirector.i.worldRight.coordsFromPos(t.position);
		
		for(int i = 0 ; i < trapsPlaced.Count ; i++)
		{
			if(trapsPlaced[i] == coords)
			{
				return false;
			}
			
		}
		return true;
	}
	
	bool checkFreeArea(Vector2 tileCoords)
	{
		for(int i = 0 ; i < trapsPlaced.Count ; i++)
		{
			if(trapsPlaced[i] == tileCoords)
			{
				return false;
			}
			
		}
		return true;
	}
	
	void highlightArea(GameObject obj,  Color highlightColor, ref GameObject refKeep, ref Material originalMat)
	{
		foreach ( Transform t in obj.GetComponentsInChildren<Transform>() )
		{
			if ( t.gameObject.name == "Tapa" )
			{
				obj = t.gameObject;
				break;
			}
		}
		
		if( originalMat == null)
		{
			originalMat = obj.renderer.sharedMaterial;
		}
				
		
		Material tempMat = obj.renderer.material;
		tempMat.SetColor("_Color", Color.Lerp(tempMat.color, highlightColor, 0.5f));
		
		if(refKeep != obj)
		{
			if(refKeep != null)
			{
				
				refKeep.renderer.material = originalMat;
				refKeep = obj;	
			}
			else
			{
				refKeep = obj;		
			}
		}
		
	}
	
	override protected void OnPressSwitch( GameObject switchPressed )
	{
		switchPressed.SendMessage ("OnPressedPast", gameObject, SendMessageOptions.DontRequireReceiver);		
	}	
	
	public int maxTraps { get{ return MaxTraps; }}
}

