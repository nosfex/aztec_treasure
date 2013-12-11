using UnityEngine;
using System.Collections;

public class AztecPlayer : Player {
	
	
	public int trapCurrency = 20;
	
	public GameObject vines;
	public GameObject fFloor;
	
	
	public float maxCurrencyCooldown = 1.0f;
	float currencyCooldown = 0.0f;
	// Use this for initialization
	override protected void Start () 
	{
		base.Start();
	}
	
	
	KeyCode placeTrap = KeyCode.R;
	KeyCode cycleLeft = KeyCode.Q;
	KeyCode cycleRight = KeyCode.E;
	
	int currentTrap = 0;
	// Update is called once per frames
	override protected void Update ()  
	{
		base.Update();
		
//		print ( "ajsdas " + transform.localPosition );
		//if ( wha != null )
		//	UnityEditor.Selection.activeGameObject = wha;//Destroy ( wha );
			//wha.transform.position += Vector3.up * 0.01f;
		
		if(currencyCooldown >= maxCurrencyCooldown)
		{
			currencyCooldown = 0.0f;
			
			trapCurrency += 10;
		}
		currencyCooldown += Time.deltaTime;
		
		
		if(Input.GetKeyDown(cycleLeft))
		{
			currentTrap--;
			if(currentTrap < -1)
			{
				currentTrap = -1;
				
			}
		}
		if(Input.GetKeyDown(cycleRight))
		{
			currentTrap++;
			if(currentTrap > 1)
			{
				currentTrap = 1;
			}
			
		}
		
		if(Input.GetKeyDown(placeTrap))
		{
			if(currentTrap == -1)
			{
				if(trapCurrency > 10)
				{
			
					GameObject vine = (GameObject)MonoBehaviour.Instantiate(vines, transform.position + Vector3.up, transform.rotation);
					vine.transform.parent =  ((World)GameDirector.i.worldLeft).transform;
					GameObject vine2 = (GameObject)MonoBehaviour.Instantiate(vines, transform.position + Vector3.up, transform.rotation);
					vine2.transform.parent = ((World)GameDirector.i.worldRight).transform;
					vine2.transform.localPosition = transform.localPosition;
					trapCurrency -= 10;
					print(trapCurrency.ToString());
				}
			}
			
			if(currentTrap == 0)
			{
				if(trapCurrency > 30)
				{
					GameObject obj2 = GameDirector.i.worldRight.objFromPos( transform.localPosition );
					if(obj2 == null) return;	
					
					Transform t = obj2.transform;
									
					Destroy(obj2);
					obj2 = (GameObject)MonoBehaviour.Instantiate(fFloor, t.position, t.rotation);
					obj2.transform.parent = GameDirector.i.worldRight.transform;
					
						
				}
			}
			
			if(currentTrap == 1)
			{
				if(trapCurrency > 20)
				{
					
				}
			}
		}
	}

	
	override protected void OnPressSwitch( GameObject switchPressed )
	{
		switchPressed.SendMessage ("OnPressedPast", gameObject, SendMessageOptions.DontRequireReceiver);		
	}	
}

