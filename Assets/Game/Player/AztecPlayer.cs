using UnityEngine;
using System.Collections;

public class AztecPlayer : Player {
	
	
	public int trapCurrency = 20;
	
	public GameObject vines;
	public GameObject fFloor;
	public GameObject skelly;
	public GameObject ranged;
	public GameObject darts;
	
	
	public GameObject vinesSign;
	public GameObject fFloorSign;
	public GameObject skellySign;
	public GameObject rangedSign;
	public GameObject dartsSign;
	
	
	public float maxCurrencyCooldown = 1.0f;
	int MaxTraps = 5;
	float currencyCooldown = 0.0f;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start();
		trapCurrency = 1000;
	}
	
	
	public int vinesCooldown = 0;
	public int fallingFloorCooldown = 0;
	public int skellyCooldown = 0;
	public int rangedCooldown = 0;
	public int wallDartCooldown = 0;
	
	
	bool vinesLock = false;
	bool fallingFloorLock = false;
	bool skellyLock = false;
	bool rangedLock = false;
	bool wallDartLock = false;
	
	float vinesCooldownTimer = 0;
	float fallingFloorCooldownTimer = 0;
	float skellyCooldownTimer = 0;
	float rangedCooldownTimer = 0;
	float wallDartCooldownTimer = 0;
		
	KeyCode placeTrap = KeyCode.T;
	KeyCode cycleLeft = KeyCode.Q;
	KeyCode cycleRight = KeyCode.E;
	
	public int currentTrap = 0;
	
	
	public int vinesPrice = 70;
	public int fallingFloorPrice = 100;
	public int skellyPrice = 200;
	public int rangedPrice = 400;
	public int wallDartPrice = 300;
	
	public void PlaceTrap( GameObject prefab )
	{
		//GameObject vine = (GameObject)MonoBehaviour.Instantiate(prefab, transform.position + Vector3.up, transform.rotation);
		//vine.transform.parent =  ((World)GameDirector.i.worldLeft).transform;
		
		GameObject vine2 = (GameObject)MonoBehaviour.Instantiate(prefab, transform.position + Vector3.up, transform.rotation);
		vine2.transform.parent = ((World)GameDirector.i.worldRight).transform;
		vine2.name = vine2.name.TrimEnd( "(Clone)" );
		vine2.transform.localPosition = transform.localPosition;
		
	}

	void PlaceSign(GameObject prefab, Transform posAt)
	{
		GameObject obj = (GameObject)MonoBehaviour.Instantiate( prefab, posAt.position, Quaternion.identity );
		obj.transform.parent = GameDirector.i.worldLeft.transform;
		
	}
	
	
	public void PlaceTrapAtPos(GameObject prefab, Transform t)
	{
		GameObject vine2 = (GameObject)MonoBehaviour.Instantiate(prefab, t.position + Vector3.up, t.rotation);
		vine2.transform.parent = ((World)GameDirector.i.worldRight).transform;
		vine2.name = vine2.name.TrimEnd( "(Clone)" );
		vine2.transform.localPosition = t.localPosition;
	}
	
	public string[] trapNames;
	// Update is called once per frames
	override protected void Update ()  
	{
		base.Update();
		
//		print ( "ajsdas " + transform.localPosition );
		//if ( wha != null )
		//	UnityEditor.Selection.activeGameObject = wha;//Destroy ( wha );
			//wha.transform.position += Vector3.up * 0.01f;

		
		if ( liftedObject == null ) // Trata de levantar un objeto...
		{
			if ( liftSensor.sensedObject != null && liftSensor.sensedObject.isSwitch )
			{
				return;
			}
		}
		
		if(currencyCooldown >= maxCurrencyCooldown)
		{
			currencyCooldown = 0.0f;
			trapCurrency += 10;
		}

		currencyCooldown += Time.deltaTime;
		
		if(Input.GetKeyDown(cycleLeft))
		{
			currentTrap--;

			if(currentTrap < 0)
				currentTrap = 4;
			
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, trapNames[ currentTrap ] );
		}
		if(Input.GetKeyDown(cycleRight))
		{
			currentTrap++;

			if(currentTrap > 4)
				currentTrap = 0;
			
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, trapNames[ currentTrap ] );
		}
		
		if(Input.GetKeyDown(placeTrap))
		{
			if(currentTrap == 0 && !vinesLock)
			{
				if(trapCurrency > vinesPrice)
				{
					Transform t = new GameObject().transform;
					t.position = transform.position;
					t.rotation = transform.rotation;
					t.localPosition = transform.localPosition;
					//PlaceTrap( vines );			
					DelayedSpawner.i.addSpawnData(vines, t, 3, null);
					
					PlaceSign(vinesSign, t);
					
					trapCurrency -= vinesPrice;
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + vinesPrice );
					vinesLock = true;
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
				
			}
			
			if(currentTrap == 1 && !fallingFloorLock)
			{
				if(trapCurrency > fallingFloorPrice)
				{
					
					GameObject obj2 = GameDirector.i.worldRight.objFromPos( transform.localPosition );
					
					//Debug.Log ("obj2 = " + obj2 );
					if(obj2 == null) 
						return;
					
					//Debug.Log ("obj2.name = " + obj2.name );
					if ( obj2.name == fFloor.name )
					{
						GameDirector.i.ShowTextPopup( gameObject, 0.8f, "Already placed." );
						return;
					}
					
					Transform t = obj2.transform;
									
					Destroy(obj2);
					obj2 = (GameObject)MonoBehaviour.Instantiate(fFloor, t.position, t.rotation);
					obj2.transform.parent = GameDirector.i.worldRight.transform;
					obj2.name = obj2.name.TrimEnd( "(Clone)" );

					Vector2 tileXY = GameDirector.i.worldRight.coordsFromPos( obj2.transform.localPosition );
					GameDirector.i.worldRight.globalTiles[ (int)tileXY.x, (int)tileXY.y ] = obj2;

					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + fallingFloorPrice );
					trapCurrency -= fallingFloorPrice;
					fallingFloorLock = true;
					
					PlaceSign(fFloorSign, t);
						
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
				
			}
			
			if(currentTrap == 2 && !skellyLock)
			{
				if(trapCurrency > skellyPrice)
				{
					Transform t = new GameObject().transform;
					t.position = transform.position;
					t.rotation = transform.rotation;
					t.localPosition = transform.localPosition;
					//PlaceTrap( skelly );			
					DelayedSpawner.i.addSpawnData(skelly, t, 3, null);
					trapCurrency -= skellyPrice;
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + skellyPrice );
					skellyLock = true;
					
					PlaceSign(skellySign, t);

				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
				
			}

		
			if(currentTrap == 3 && !rangedLock)
			{
				if(trapCurrency > rangedPrice)
				{
					
					Transform t = new GameObject().transform;
					t.position = transform.position;
					t.rotation = transform.rotation;
					t.localPosition = transform.localPosition;
					DelayedSpawner.i.addSpawnData(ranged, t, 3, null);
					//PlaceTrap( ranged );			
					trapCurrency -= rangedPrice;
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + rangedPrice );
					rangedLock = true;
					
					PlaceSign(rangedSign, t);
					
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
				
			}
			
			if(currentTrap == 4 && !wallDartLock)
			{
				if( trapCurrency > wallDartPrice)
				{
					RaycastHit r;
					
					if(direction != Vector3.forward) 
					{
							GameDirector.i.ShowTextPopup( gameObject, 0.8f, "Can't place this way!" );
						return;
					}
					if(Physics.Raycast(transform.position + Vector3.up  *0.4f,  this.direction, out r))
					{
						GameObject obj2 = GameDirector.i.worldRight.objFromPos( r.point + this.direction * 0.4f );
					
						if(obj2 == null) 
							return;
					//	if(obj2.name != "WallTile")
					//		return;
						
						
						
						Transform t = new GameObject().transform;
						t.position = obj2.transform.position;
						t.rotation = obj2.transform.rotation;
						t.localPosition = obj2.transform.localPosition;
						
						
						//PlaceTrapAtPos(darts, tObj);
						DelayedSpawner.i.addSpawnData(darts, t, 3, null);
						PlaceSign(dartsSign, t);

						GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + wallDartPrice );
						wallDartLock= true;
//						Destroy
					}
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
				
				
				
			}
		
		
		}
		
		if(vinesLock)
		{
			vinesCooldownTimer += Time.deltaTime;
			if(vinesCooldownTimer >= vinesCooldown)
			{
				
				vinesCooldownTimer = 0;
				vinesLock = false;
			}
		}
		
		
		if(wallDartLock)
		{
			wallDartCooldownTimer += Time.deltaTime;
			if(wallDartCooldownTimer >= wallDartCooldown)
			{
				
				wallDartCooldownTimer = 0;
				wallDartLock = false;
			}
		}
		
		if(rangedLock)
		{
			rangedCooldownTimer += Time.deltaTime;
			if(rangedCooldownTimer >= rangedCooldown)
			{
				
				rangedCooldownTimer = 0;
				rangedLock = false;
			}
		}
		
		if(skellyLock)
		{
			skellyCooldownTimer += Time.deltaTime;
			if(skellyCooldownTimer >= skellyCooldown)
			{
				
				skellyCooldownTimer = 0;
				skellyLock = false;
			}
		}
		
		if(fallingFloorLock)
		{
			fallingFloorCooldownTimer += Time.deltaTime;
			if(fallingFloorCooldownTimer >= fallingFloorCooldown)
			{
				
				fallingFloorCooldownTimer = 0;
				fallingFloorLock = false;
			}
		}
	}
	

	
	override protected void OnPressSwitch( GameObject switchPressed )
	{
		switchPressed.SendMessage ("OnPressedPast", gameObject, SendMessageOptions.DontRequireReceiver);		
	}	
	
	public int maxTraps { get{ return MaxTraps; }}
}

