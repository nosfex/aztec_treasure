using UnityEngine;
using System.Collections;

public class Fountain : BaseObject 
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
	
	
	public const int FOUNTAIN_LIFE = 0;
	public const int FOUNTAIN_INVISIBILITY = 1;
	public const int FOUNTAIN_SPEED = 2;
	public int fountainType = 0;
	
	//[HideInInspector] public int isFuture;
	
	void Start()
	{
		flameParticle = GetComponentInChildren<ParticleSystem>();
		flameLight = GetComponentInChildren<Light>();
		
//		if ( gameObject.layer == LayerMask.NameToLayer( "Past" ) )
//			GUIAltars.i.altarsCount++;
//		else 
//			tipContainer.SetActive( false );
		
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
		
		Player p = (Player)GameDirector.i.playerRight;
		
		if ( activated < timeToActivate )
		{
			if ( Vector3.Distance( transform.position, GameDirector.i.playerRight.transform.position ) < 3.0f )
				tipContainer.SetActive( true );
			else 
				tipContainer.SetActive( false );
		}
		
		if ( activating )
			tipText.text = "Activating...";
		else 
			tipText.text = "Hold 1 to activate.";

		
		activating = false;
		
		if ( activated < timeToActivate )
		{
			float act = Mathf.Max ( 0, activated - 1.0f );
			Color c = new Color( 1f,1f,1f, act / (timeToActivate) );
			flameParticle.GetComponent<Renderer>().material.SetColor( "_TintColor", c );
			flameLight.intensity = act / (timeToActivate);
		}
		
		if ( activated >= timeToActivate )
		{
			
		}
	}
	
	void OnPressedFuture (GameObject gameObject )
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
				activatedFX.SetActive( true );
				///efectito loco
				//flame.SetActive( true );
				Player p = (Player)GameDirector.i.playerRight;
				Color c = new Color( 1f,1f,1f, 1.0f );
				flameParticle.GetComponent<Renderer>().material.SetColor( "_TintColor", c );
				flameLight.intensity = 2f;
				
			//	int initialBonus = 1000;
				//p.trapCurrency += initialBonus;
				Debug.Break();
				p.addPotionType(fountainType);
				GameDirector.i.ShowTextPopup( gameObject, 0.8f, "POTION: " + fountainType.ToString() );
			}
		}
	}
}
