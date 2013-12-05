﻿using UnityEngine;
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
			
			trapCurrency ++;
		}
		currencyCooldown += Time.deltaTime;
		
		if(Input.GetKeyDown(trapA))
		{
			if(trapCurrency > 10)
			{
				MonoBehaviour.Instantiate(vines, transform.position, transform.rotation);
				trapCurrency -= 10;
				print(trapCurrency.ToString());
			}
		}
	}
	
	void OnGui()
	{
		
		GUI.Box(new Rect(10, 10, 200, 200) , "trap money: " + trapCurrency.ToString());
	}
}