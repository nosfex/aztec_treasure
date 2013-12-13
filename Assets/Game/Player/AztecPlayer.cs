using UnityEngine;
using System.Collections;

public class AztecPlayer : Player {
	
	
	public int trapCurrency = 20;
	
	public GameObject vines;
	public GameObject fFloor;
	public GameObject skelly;
	public GameObject ranged;
	
	
	public float maxCurrencyCooldown = 1.0f;
	int MaxTraps = 4;
	float currencyCooldown = 0.0f;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start();
		trapCurrency = 1000;
	}
	
	
	KeyCode placeTrap = KeyCode.T;
	KeyCode cycleLeft = KeyCode.Q;
	KeyCode cycleRight = KeyCode.E;
	
	int currentTrap = 0;
	
	
	public int vinesPrice = 70;
	public int fallingFloorPrice = 100;
	public int skellyPrice = 200;
	public int rangedPrice = 400;
	
	public void PlaceTrap( GameObject prefab )
	{
		//GameObject vine = (GameObject)MonoBehaviour.Instantiate(prefab, transform.position + Vector3.up, transform.rotation);
		//vine.transform.parent =  ((World)GameDirector.i.worldLeft).transform;
		
		GameObject vine2 = (GameObject)MonoBehaviour.Instantiate(prefab, transform.position + Vector3.up, transform.rotation);
		vine2.transform.parent = ((World)GameDirector.i.worldRight).transform;
		vine2.name = vine2.name.TrimEnd( "(Clone)" );
		vine2.transform.localPosition = transform.localPosition;
		
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
				currentTrap = 3;
			
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, trapNames[ currentTrap ] );
		}
		if(Input.GetKeyDown(cycleRight))
		{
			currentTrap++;

			if(currentTrap > 3)
				currentTrap = 0;
			
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, trapNames[ currentTrap ] );
		}
		
		if(Input.GetKeyDown(placeTrap))
		{
			if(currentTrap == 0)
			{
				if(trapCurrency > vinesPrice)
				{
					PlaceTrap( vines );			

					trapCurrency -= vinesPrice;
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + vinesPrice );
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
			}
			
			if(currentTrap == 1)
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

						
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
			}
			
			if(currentTrap == 2)
			{
				if(trapCurrency > skellyPrice)
				{
					PlaceTrap( skelly );			
					trapCurrency -= skellyPrice;
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + skellyPrice );
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
			}

		
			if(currentTrap == 3)
			{
				if(trapCurrency > rangedPrice)
				{
					PlaceTrap( ranged );			
					trapCurrency -= rangedPrice;
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "-" + rangedPrice );
				}
				else 
				{
					GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
				}
			}
		}
	}

	
	override protected void OnPressSwitch( GameObject switchPressed )
	{
		switchPressed.SendMessage ("OnPressedPast", gameObject, SendMessageOptions.DontRequireReceiver);		
	}	
	
	public int maxTraps { get{ return MaxTraps; }}
}

