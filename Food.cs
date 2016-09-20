using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Food : MonoBehaviour {

	public static bool isEaten = false;
	public static bool needNewBody = false;
	public static Vector3 myPos;

	public static int score = 0;

	public Text scoreText;

	private bool clear;

	public static bool reLight;

	void Awake(){
		myPos = transform.position;
		redraw (ref myPos, ref isEaten,ref clear);
		myPos = transform.position;
	}

	// Use this for initialization
	void Start () {

		score = 0;
		textUpdate();

	}

	// Update is called once per frame
	void Update () {
		if (reLight == true){
			reLight = false;
		}

		myPos = transform.position;

		if (isEaten == true) {
			do{
				redraw (ref myPos, ref isEaten,ref clear);
				for (int i = 0; i < SnakeHead.bodyCopy.Count; i++){
					if (transform.position == SnakeHead.bodyCopy[i]){
						clear = false;
						print("foodhit");
					}
					else{
					}
				}
			}while (clear == false);
			needNewBody = true;
			isEaten = false;
		 	score = score + 100;
		 	textUpdate();
			myPos = transform.position;
			reLight = true;
		}

	}

	void redraw(ref Vector3 myPos, ref bool isEaten,ref bool clear){
		clear = true;
		Vector3 tempPos = myPos;
		tempPos.x = Mathf.Round(Random.Range(-5f,5f));
		tempPos.y = Mathf.Round(Random.Range(-4f,4f));
		tempPos.z = Mathf.Round(Random.Range(-5f,5f));

		transform.position = tempPos;

		//if( GameController.needNewBody == false){

		//}
	}

	void OnTriggerEnter(Collider me)  {

		isEaten = true;
	}

	void textUpdate(){
		scoreText.text = "Score: " + score.ToString();
	}


}
