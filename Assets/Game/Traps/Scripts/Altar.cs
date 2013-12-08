using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour 
{

	
	// Use this for initialization
	void Start () {
	
	}
	
	private float currencyCooldown;
	private bool activated = false;
	
	public float maxCurrencyCooldown = 1.0f;
		
	void Update() 
	{
		AztecPlayer p = (AztecPlayer)GameDirector.i.playerLeft;
		
		if ( activated )
		{
			currencyCooldown += Time.deltaTime;
	
			if(currencyCooldown >= maxCurrencyCooldown )
			{
				currencyCooldown = 0.0f;
				p.trapCurrency += 10;
			}
		}
	}
}
