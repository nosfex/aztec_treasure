using UnityEngine;
using System.Collections;

public class AztecPlayer : Player {
	
	
	public int trapCurrency = 20;
	
	public GameObject vines;
	public GameObject trap0;
	
	
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
		
		if(currencyCooldown >= maxCurrencyCooldown)
		{
			currencyCooldown = 0.0f;
			
			trapCurrency += 10;
		}
		currencyCooldown += Time.deltaTime;
		
		if(Input.GetKeyDown(trapA))
		{
			if(trapCurrency > 100)
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
	}

}
