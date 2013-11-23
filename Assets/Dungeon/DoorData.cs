using UnityEngine;
using System.Collections;

public class DoorData
{
	public Vector2 colRow = Vector2.zero;
	
	public bool connected;
	public bool open;
	public int side = 99;
	
	public DoorData target;
	
	public DoorData()
	{
	}
}
