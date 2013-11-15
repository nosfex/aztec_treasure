using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LeafData 
{
	
	public LeafData parent;
	public LeafData left;
	public LeafData right;

	public static int instanceCount;
	public Room leafData;
	
		
	static public ArrayList takenPositions;
	
	public enum RelativePosToParent 
	{
		LEFT = 0,
		RIGHT = 1, 
		TOP = 2,
		BOTTOM = 3,
		LEFT_TOP = 4,
		LEFT_BOT = 5, 
		RIGHT_TOP = 6,
		RIGHT_BOT = 7,
	};
	public RelativePosToParent RPTP;
	
	public LeafData()
	{
		RPTP = (RelativePosToParent)Mathf.CeilToInt(Random.value* (int)RelativePosToParent.BOTTOM);
		
	}
	
	void Awake() 
	{
		
		
	}
	
	// Use this for initialization
	public void Start () {
		TreeData.print("INSTANCE COUNT: " + instanceCount.ToString() );
		instanceCount++;
		
			leafData = new Room(0, 0);	
			leafData.roomHolder = new GameObject("Room_"+instanceCount.ToString() );
		
		//takenPositions = new Vector3[];
		takenPositions = new ArrayList();
		if(parent != null)
		{
			positionNode();
			
		}
		else
		{
			leafData.roomHolder.transform.position = Vector3.zero;
			takenPositions.Add(Vector3.zero);
			
		}
//		leafData.Start();
		
	}
	
	void positionNode()
	{
		int xOffset = 0;
		int zOffset = 0;
		switch(RPTP)
		{
			case RelativePosToParent.LEFT:
				xOffset = -300;
			break;
			case RelativePosToParent.LEFT_TOP:
				xOffset = -300;
			//	zOffset = 300;
			break;
			case RelativePosToParent.LEFT_BOT:
			
				xOffset = -300;
			//	zOffset = -300;
			break;
			case RelativePosToParent.RIGHT:
				xOffset = 300;
			break;
			case RelativePosToParent.RIGHT_TOP:
				xOffset = 300;
			//	zOffset = 300;
			break;
			case RelativePosToParent.RIGHT_BOT:
				xOffset = 300;
			//	zOffset = -300;
			break;		
			case RelativePosToParent.TOP:
				zOffset = 300;
			break;
			case RelativePosToParent.BOTTOM:
				zOffset = -300;
			break;
		};
		Vector3 newPos = new Vector3(parent.leafData.roomHolder.transform.position.x +xOffset, 0,  parent.leafData.roomHolder.transform.position.z + zOffset);
		
		leafData.roomHolder.transform.position = newPos;
		takenPositions.Add(newPos);
		if(parent.left != this)
		{
			if(parent.left.leafData.roomHolder.transform.position == this.leafData.roomHolder.transform.position)
			{
				RPTP = (RelativePosToParent)Mathf.CeilToInt(Random.value* (int)RelativePosToParent.RIGHT_BOT);
				positionNode();
				
			}
		
		}
		else
		{
			
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
