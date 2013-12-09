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
	
	
	KeyCode trapA = KeyCode.F1;
	KeyCode trapB = KeyCode.F2;
	// Update is called once per frame
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
		
		if(Input.GetKeyDown(trapA))
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
		
		if(Input.GetKeyDown(trapB))
		{
			if(trapCurrency > 30)
			{
				
				//Vector2 tilePos = new  Vector2(transform.position.x+ 0.4f, transform.position.z+ 0.4f);
				
				//GameObject obj = (GameObject)GameDirector.i.dungeonGenerator.getClosestObjToPoint(tilePos);
				GameObject obj = GameDirector.i.worldLeft.objFromPos( transform.localPosition );

				if(obj == null) return;	
				
				trapCurrency -= 30;
				
				Transform t = obj.transform;
								
				Destroy(obj);
				obj = (GameObject)MonoBehaviour.Instantiate(fFloor, t.position, t.rotation);
				obj.transform.parent = GameDirector.i.worldLeft.transform;

				
				GameObject obj2 = GameDirector.i.worldRight.objFromPos( transform.localPosition );

				if(obj2 == null) return;	
				
				t = obj2.transform;
								
				Destroy(obj2);
				obj2 = (GameObject)MonoBehaviour.Instantiate(fFloor, t.position, t.rotation);
				obj2.transform.parent = GameDirector.i.worldRight.transform;
				
					
			}
		}
	}

	
	override protected void OnPressSwitch( GameObject switchPressed )
	{
		switchPressed.SendMessage ("OnPressedPast", gameObject, SendMessageOptions.DontRequireReceiver);		
	}	
}

