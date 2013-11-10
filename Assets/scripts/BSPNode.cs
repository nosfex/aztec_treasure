using UnityEngine;
using System.Collections;

public class BSPNode
{

	
	public BSPNode parent;
	public BSPNode left;
	public BSPNode right;
	public int weight;
	public int width = 0;
	public int height = 0;
	public int initPosX;
	public int initPosY;
	
	public Room room;

	public static int instanceCount =0 ;
	
	
	public BSPNode(int w)	
	{
		instanceCount++;
		weight = w;
		//initPosX = Vector2.zero;
		
	}
}
