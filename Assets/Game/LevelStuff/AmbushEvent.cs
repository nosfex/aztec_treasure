using UnityEngine;
using System.Collections;

public class AmbushEvent : MonoBehaviour {
	
	public BaseObjectSensor trigger;
	public GameObject disableOnTrigger;
	public GameObject enableOnTrigger;
	// Use this for initialization
	void Start () 
	{
		trigger.typeFilter = typeof( Player );
	}
	
	// Update is called once per frame
	void Update () {
		if ( trigger.sensedObject != null )
		{
			disableOnTrigger.SetActive( false );
			enableOnTrigger.SetActive( true );
		}
	}
}
