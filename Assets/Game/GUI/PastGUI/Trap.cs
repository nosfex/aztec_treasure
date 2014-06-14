using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour 
{
	public string name;
	public GameObject signPrefab;
	public GameObject trapPrefab;
	public int price;
	public int altarUnlock;
	public int spawnDelay;
	
	TextMesh priceTag;
	// Use this for initialization
	
	void Start () 
	{
		priceTag = GetComponentInChildren<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		priceTag.text = price.ToString();
		
		renderer.material.SetFloat("_EffectAmount", GUIAltars.i.altarsFound >= altarUnlock ? 0f : 1.0f );

	}
	
	public bool CanBePlaced()
	{
		int currency = ((AztecPlayer)GameDirector.i.playerLeft).trapCurrency;
		if ( currency < price )
		{
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, "No money!" );
			return false;
		}
		
		if ( GUIAltars.i.altarsFound < altarUnlock )
		{
			GameDirector.i.ShowTextPopup( gameObject, 0.8f, "Need " + altarUnlock + " altars!" );
			return false;
		}
		
		return true;
	}
}
