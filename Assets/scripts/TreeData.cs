using UnityEngine;
using System.Collections;

public class TreeData : MonoBehaviour {
	
	public GameObject floorTile;
	public GameObject wallTile;
	public static TreeData instance;
	
	// GH: How many nodes the tree should have
	public int treeDepth;  
	
	void Awake()
	{
		instance = this;
		
		
	}
	// Use this for initialization
	void Start () 
	{
		LeafData root = new LeafData();
		root.parent = null;
		root.Start();	
		
		insert(root);
		insert(root);
		insert(root);
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void generateData() 
	{
		
		//Instantiate
	
	}
	
	void insert(LeafData node) 
	{
	//	LeafData leaf = Instantiate();
	//	Leaf leaf = Instantiate(L);
	
		treeDepth++;
		
		if(node.left != null)
		{
			print("ASEREJE");
			insert(node.left);
			
			
		}
		else
		{
			
			node.left = new LeafData();
			node.left.parent = node;
			node.left.Start();
			print(node.left.ToString());
		}
		
		if(node.right != null)
		{
			print("ASEREJE");
			insert(node.right);
		
			
		}
		else
		{
			
			node.right = new LeafData();
			node.right.parent = node;
			node.right.Start();
			print(node.right.ToString());
		}
		
		
	//	if(ld.right )
		
		
	}
}
