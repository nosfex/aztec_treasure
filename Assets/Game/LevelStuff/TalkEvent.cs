using UnityEngine;
using System.Collections;

public class TalkEvent : MonoBehaviour {
	
	public string caption;
	BaseObjectSensor sensor;
	
	public bool triggerWhenTargetIsDead = false;
	public GameObject target;
	// Use this for initialization
	void Start () 
	{
		sensor = GetComponent<BaseObjectSensor>();
		sensor.typeFilter = typeof(Player);
	}
	
	// Update is called once per frame
	void Update () {
		if ( sensor.sensedObject != null )
		{
			if ( (triggerWhenTargetIsDead && target == null) || !triggerWhenTargetIsDead )
			{
				GameDirector.i.ShowTextPopup( GameDirector.i.playerRight.gameObject, 0.8f, caption );
				Destroy ( gameObject );
			}
		}
	}
}
