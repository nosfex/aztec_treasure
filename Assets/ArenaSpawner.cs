using UnityEngine;
using System.Collections;

public class ArenaSpawner : MonoBehaviour {
	
	
	public GameObject[] enemiesToSpawn; 
	public GameObject trapDoor;
	public GameObject spikeWallLeft;
	public GameObject spikeWallRight; 
	public float wallSpeed = 0;
	
	int currentActive = 0;
	bool advanceNextWaveCheck = false;
	double nextWaveCheckTimer = 0.0;
	
	bool activated = false;
	
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(advanceNextWaveCheck)
		{
			// GH: If we are checking for the next wave, we shouldn't have any enemies from the previous one
			Skelly enemyCheck = enemiesToSpawn[currentActive].GetComponentInChildren<Skelly>();
			if(enemyCheck == null)
			{
				advanceNextWaveCheck = false;
				enemiesToSpawn[currentActive].SetActive(false);
				// GH: Start the next wave
				currentActive++;
				if(currentActive >= enemiesToSpawn.Length)
				{
					// GH: We don't have more waves, YAY!
					return;
				}
				if(enemiesToSpawn[currentActive].activeSelf == false)	
				{
					// GH: Start the next wave
					enemiesToSpawn[currentActive].SetActive(true);
				}
			}		
			nextWaveCheckTimer += Time.deltaTime;
			
			if(nextWaveCheckTimer >= 2.0)
			{
				advanceNextWaveCheck = false;
			}
		}
		
		if(spikeWallLeft.activeSelf == true && spikeWallRight.activeSelf == true)
		{
			Vector3 posLeft = spikeWallLeft.transform.position;
			posLeft.x  += wallSpeed * Time.deltaTime;
			spikeWallLeft.transform.position = posLeft;
			
			Vector3 posRight = spikeWallRight.transform.position;
			posRight.x -= wallSpeed * Time.deltaTime;
			spikeWallRight.transform.position = posRight;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(activated)
		{
			return;
			
		}
		Player player = other.gameObject.GetComponentInChildren<Player>();
		
		if(player == null)
		{
			return;
		}
		else
		{
			int activeCount = enemiesToSpawn.Length;
			activated = true;
			
			for(int i = 0 ; i < enemiesToSpawn.Length; i++)
			{
				if(enemiesToSpawn[i].activeSelf == false)	
				{
					activeCount--;
				}
			}
			
			
			if(activeCount == 0)
			{
				currentActive = 0;
				
				enemiesToSpawn[currentActive].SetActive(true);
				
			}
			trapDoor.SetActive(true);
			iTween.MoveFrom(trapDoor, iTween.Hash("y", trapDoor.transform.position.y  - 3, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
			
			spikeWallLeft.SetActive(true);
			iTween.MoveFrom(spikeWallLeft, iTween.Hash("y", spikeWallLeft.transform.position.y - 3, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
			
			spikeWallRight.SetActive(true);
			iTween.MoveFrom(spikeWallRight, iTween.Hash("y", spikeWallRight.transform.position.y - 3, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
			
		}
	}
	
	public void OnEnemyDead(Skelly enemy)
	{
		// GH: Force the loop check to see if we have to advance the wave
		advanceNextWaveCheck = true;
		
	}

}
