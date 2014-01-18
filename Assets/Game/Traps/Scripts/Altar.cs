using UnityEngine;
using System.Collections;

public class Altar : BaseObject 
{

	public GameObject flame;
	public GameObject activatedFX;

	private float currencyCooldown;
	private float activated = 0;
	
	
	public float maxCurrencyCooldown = 1.0f;
	public float timeToActivate = 2.0f;
	
	ParticleSystem flameParticle;
	Light flameLight;
	
	public TextMesh tipText;
	public GameObject tipContainer;
	
	//[HideInInspector] public int isFuture;
	
	void Start()
	{
		flameParticle = GetComponentInChildren<ParticleSystem>();
		flameLight = GetComponentInChildren<Light>();
		
		if ( gameObject.layer == LayerMask.NameToLayer( "Past" ) )
			GUIAltars.i.altarsCount++;
		else 
			tipContainer.SetActive( false );
		
	}
	
	override protected void LateUpdate()
	{
		// No physics!
	}
	
	bool activating = false;
	
	void Update() 
	{
		if ( activated > 0 && activated < timeToActivate )
			activated -= Time.deltaTime * 0.5f;
		
		AztecPlayer p = (AztecPlayer)GameDirector.i.playerLeft;
		
		if ( activated < timeToActivate )
		{
			if ( Vector3.Distance( transform.position, GameDirector.i.playerLeft.transform.position ) < 3.0f )
				tipContainer.SetActive( true );
			else 
				tipContainer.SetActive( false );
		}
		
		if ( activating )
			tipText.text = "Activating...";
		else 
			tipText.text = "Hold T to activate.";

		
		activating = false;
		
		if ( activated < timeToActivate )
		{
			float act = Mathf.Max ( 0, activated - 1.0f );
			Color c = new Color( 1f,1f,1f, act / (timeToActivate) );
			flameParticle.renderer.material.SetColor( "_TintColor", c );
			flameLight.intensity = act / (timeToActivate);
		}
		
		if ( activated >= timeToActivate )
		{
			currencyCooldown += Time.deltaTime;
	
			if(currencyCooldown >= maxCurrencyCooldown )
			{
				currencyCooldown = 0.0f;
				p.trapCurrency += 10;
				
				GameDirector.i.ShowTextPopup( gameObject, 0.8f, "+" + 10 );
			}
		}
	}
	
	void OnPressedPast( GameObject gameObject )
	{
		//Debug.Log ("Pressing..." + activated );
		if ( activated < timeToActivate )
		{
			activating = true;
			activated += Time.deltaTime;
			
			if ( activated < 1.0f ) activated = 1.0f;
			
			if ( activated >= timeToActivate )
			{
				tipContainer.SetActive(false);
				GUIAltars.i.altarsFound++;
				activatedFX.SetActive( true );
				///efectito loco
				//flame.SetActive( true );
				AztecPlayer p = (AztecPlayer)GameDirector.i.playerLeft;
				p.activatedAltar++;
				Color c = new Color( 1f,1f,1f, 1.0f );
				flameParticle.renderer.material.SetColor( "_TintColor", c );
				flameLight.intensity = 2f;
			}
		}
	}
}
