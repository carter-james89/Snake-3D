using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrackArray : MonoBehaviour {
	private Texture hitTexture;
	private Texture defaultTexture;
	private Texture foodTexture;
	private Texture blueTexture;
	public bool isHit;

	//target gameobject
	public GameObject snakeHead;
	public SnakeHead headScript;
	public GameObject mirrorWall;

	Transform[] barArray;
	public Renderer[] barTextures;

	public Renderer[] mirrorTextures;
	public List <Transform> Bars;
	public List <bool> isFood;

	bool isHere;
	public Dictionary<Vector2, Vector4> grid;
	Vector2 gridLocal;
	Vector4 square;
	int squareCount;

	void Awake(){
		grid =  new Dictionary<Vector2, Vector4>();
		//initialize the list
		Bars = new List <Transform> ();
		isFood = new List <bool> ();
		//get an array off all the children bars
		barArray = GetComponentsInChildren<Transform>();
		//for some reason the array also gets the parent tranform,
		//this throws off the transform/renderer array matches
		//so i put the array in a list minus the parent
		for(int i = 1; i < barArray.Length; i++){
			Bars.Add(barArray[i]);
		}

		for(int x = -5; x < 6; x++){
			for(int y = -4; y < 5; y++){
				gridLocal = new Vector2(x,y);
				square = new Vector4(0,0,0,0);
				squareCount = 0;

				for(int i = 0; i < Bars.Count; i++){
					isHere = false;
					if((Bars[i].localPosition.y == y)||
					((Bars[i].localPosition.y == (y +.5)) ||
					(Bars[i].localPosition.y == (y-.5)))){
						if ((Bars[i].localPosition.x == (x + .5))||
						(Bars[i].localPosition.x == (x - .5))){
							isHere = true;
						}else if
						(Bars[i].localPosition.x == x){
						  isHere = true;
						}
						if (isHere == true){
							if (squareCount == 0){
								square.w = i;
							}
							if (squareCount == 1){
								square.x = i;
							}
							if (squareCount == 2){
								square.y = i;
							}
							if (squareCount == 3){
								square.z = i;
							}
							squareCount++;
						}
					}
				}
				grid.Add(gridLocal,square);
			}
		}

		//set all bars to no food
		for(int i = 0; i < Bars.Count; i++){
			isFood.Add(false);
		}
		//get the textures of the two cage walls and store in arrays
		barTextures = GetComponentsInChildren<Renderer>( );
		mirrorTextures = mirrorWall.GetComponentsInChildren<Renderer>( );
	}
	// Use this for initialization
	void Start () {
		hitTexture = Resources.Load<Texture>("hit");
		foodTexture = Resources.Load<Texture>("food");
		blueTexture = GetComponentInChildren<Renderer>().material.mainTexture;
		defaultTexture = blueTexture;
		FoodCheck();
	}

	// Update is called once per frame
	void Update () {
		if (Food.reLight == true){
			FoodCheck();
		}
		//if the snakehead is null, because it has been deleted, find the new one
		if (snakeHead == null){
			ScriptFind();
		}
		//if the head has moved into a new grid position, update the cage
		if((headScript.newHeadPos == true) & (headScript.zChange == false)){
			BarCheck();
		}
	}

	public void BarCheck(){
		for(int i = 0; i < Bars.Count; i++){
			if (isFood[i] == false){
				if (barTextures[i].material.mainTexture != defaultTexture){
					barTextures[i].material.mainTexture = defaultTexture;
					mirrorTextures[i].material.mainTexture = defaultTexture;
				}
	 		}else{
				if (barTextures[i].material.mainTexture != foodTexture){
					barTextures[i].material.mainTexture = foodTexture;
					mirrorTextures[i].material.mainTexture = foodTexture;
				}
			}
		}
		var target = new Vector2(headScript.rdHeadPos.x,headScript.rdHeadPos.y);
		if (grid.ContainsKey(target))
		{
			var value = grid[target];
			barTextures[(int)value.w].material.mainTexture = hitTexture;
			mirrorTextures[(int)value.w].material.mainTexture = hitTexture;
			barTextures[(int)value.x].material.mainTexture = hitTexture;
			mirrorTextures[(int)value.x].material.mainTexture = hitTexture;
			barTextures[(int)value.y].material.mainTexture = hitTexture;
			mirrorTextures[(int)value.y].material.mainTexture = hitTexture;
			barTextures[(int)value.z].material.mainTexture = hitTexture;
			mirrorTextures[(int)value.z].material.mainTexture = hitTexture;
		}


		//runs through the bar transform(position) list
		// for(int i = 0; i < Bars.Count; i++){
		// 	//unless the bar is withen .5 of the snake on the y, stop here
		// 	if((Bars[i].position.y == headScript.rdHeadPos.y)||
		// 	((Bars[i].position.y == (headScript.rdHeadPos.y+.5)) ||
		// 	(Bars[i].position.y == (headScript.rdHeadPos.y-.5)))){
		// 		//if it is within the y range, check the x value to see if it's in range
		// 		if ((Bars[i].position.x == (headScript.rdHeadPos.x + .5))||
		// 		(Bars[i].position.x == (headScript.rdHeadPos.x - .5))){
		// 			barTextures[i].material.mainTexture = hitTexture;
		// 			mirrorTextures[i].material.mainTexture = hitTexture;
		// 		}else if
		// 			(Bars[i].position.x == headScript.rdHeadPos.x){
		// 				barTextures[i].material.mainTexture = hitTexture;
		// 				mirrorTextures[i].material.mainTexture = hitTexture;
		// 			}else{
		// 				if (isFood[i] == false){
		// 					barTextures[i].material.mainTexture = defaultTexture;
		// 					mirrorTextures[i].material.mainTexture = defaultTexture;
		// 				}else{
		// 					barTextures[i].material.mainTexture = foodTexture;
		// 					mirrorTextures[i].material.mainTexture = foodTexture;
		// 				}
		// 			}
		// 	}else{
		// 		if (isFood[i] == false){
		// 			barTextures[i].material.mainTexture = defaultTexture;
		// 			mirrorTextures[i].material.mainTexture = defaultTexture;
		// 		}else{
		// 			barTextures[i].material.mainTexture = foodTexture;
		// 			mirrorTextures[i].material.mainTexture = foodTexture;
		// 		}
		// 	}
		// }
	}

	void FoodCheck(){
	for(int i = 0; i < Bars.Count; i++){
		if(((Bars[i].transform.localPosition.x == Food.myPos.x) &
		((Bars[i].transform.position.y + .5 == Food.myPos.y)||(Bars[i].transform.position.y - .5 == Food.myPos.y)))||
		((Bars[i].transform.position.y == Food.myPos.y) &
		((Bars[i].transform.localPosition.x + .5 == Food.myPos.x)||(Bars[i].transform.localPosition.x - .5 == Food.myPos.x)))){
			isFood[i] = true;
		}else{
			isFood[i] = false;
		}
	}
	}

	public void ScriptFind(){
		//find the gameobject that matches the head G.O in the scene
		snakeHead = GameObject.Find("Head"+GameController.snakecount);
		//store it's script for reference
		headScript = snakeHead.GetComponent<SnakeHead> ();
	}
}
