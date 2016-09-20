using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SnakeHead : MonoBehaviour {
		public bool zChange;
	//snake parts
	public Transform tail;
	public GameObject snakeHead;
	public static Transform head;
	//head position
	public Vector3 headPos;
	//the rounded position of the head snake head and tail
	public Vector3 rdHeadPos;
	public Vector3 rdTailPos;
	//for body array and lightbar check
	public bool newHeadPos;
	//is the snake currently expanding
	public bool isExpanding;

	//list for the body transforms
	public List<Vector3> body;
	//copy that gets passed to food. this is for debugging
	public static List<Vector3> bodyCopy;

	//public Transform head2;
	//for tracking the direction and axis of the head
	public string Facing = "Right";
	public enum Axis {posX, negX, posY, negY, posZ, negZ};
	public static Axis myAxis;
	public Vector3 up;
	public float upX;
	public float upY;
	public float upZ;

	// Use this for initialization
	void Start () {
		//set the name plus the count for debugging.
		name = ("Head"+GameController.snakecount);
		//set myself as a variable
		snakeHead = GameObject.Find("Head"+GameController.snakecount);
		//store the head transform and rotation
		head = snakeHead.transform;
		head.rotation = Quaternion.LookRotation(Vector3.right);
		posRound();

		body = new List<Vector3>();
		myAxis = Axis.negX;
	}

	// Update is called once per frame
	void Update () {
		if(newHeadPos == true){
			newHeadPos = false;
		}
		//store the real position of the head
		headPos = head.position;
		//is the snake within the box?
		boundCheck();
		//which axis/direction is the snake pointing?
		findAxis ();

		//if the game is in run mode, move it and control the head
		//it is also rounded to the nearest int to stay on the grid
		//the head will always be on two ints, but not in the direction it is moving
		//when it turns it snaps back to the grid
		if (GameController.status == "Running"){
			headController(ref Facing);
		}

		//gets the expanding bool from snake script
		isExpanding = Snake.expanding;

		//this rounds the head position to the nearest int for body array
		var rdHeadPosX = Mathf.Round(transform.position.x);
		var rdHeadPosY = Mathf.Round(transform.position.y);
		var rdHeadPosZ = Mathf.Round(transform.position.z);
		rdHeadPos = new Vector3(rdHeadPosX,rdHeadPosY,rdHeadPosZ);
		//rounds the tail position
		var rdTailPosX = Mathf.Round(tail.position.x);
		var rdTailPosY = Mathf.Round(tail.position.y);
		var rdTailPosZ = Mathf.Round(tail.position.z);
		rdTailPos = new Vector3(rdTailPosX,rdTailPosY,rdTailPosZ);

		//if the rounded head position doesnt equal body[0]
		//insert the new head position there
		//if ((rdHeadPos != body[0])||(body[0] == null)){
		if (body.Count==0){
			body.Add(rdHeadPos);
		}
		if (rdHeadPos != body[0]){
			body.Insert(0,rdHeadPos);
			newHeadPos = true;
			if (body[0].z != body[1].z){
				zChange = false;
			}
			else{
				zChange = false;
			}
		}
		//if the rounded tail position equals the last position in the body array
		//delete it
		 if ((rdTailPos == body[body.Count-1]) & Snake.expanding == false){
				body.RemoveAt(body.Count-1);
		 }
		//check if the head position is hitting any of the body
		for (int i = 1; i < body.Count; i++){
			 if ((body[i] == rdHeadPos) & (GameController.status != "Done2")){
				 GameController.status = "Done1";
			 }
		}
		bodyCopy = body;
	}

	void findAxis(){
	up = head.transform.forward;
	upX = up.x;
	upY = up.y;
	upZ = up.z;

		if (System.Math.Round(upX) == 1) {
			myAxis = Axis.posX;
			if (GameController.activeCamera == "Cam1") {
				Facing = "Right";
			}
			if (GameController.activeCamera == "Cam2") {
				Facing = "Towards";
			}
		}
		if (System.Math.Round(upX) == -1) {
			myAxis = Axis.negX;
			if (GameController.activeCamera == "Cam1") {
				Facing = "Left";
			}
			if (GameController.activeCamera == "Cam2") {
				Facing = "Away";
			}
		}
		if (System.Math.Round(upY) == 1) {
			myAxis = Axis.posY;
			if ((GameController.activeCamera == "Cam1") || (GameController.activeCamera == "Cam2")) {
				Facing = "Up";
			}
		}
		if (System.Math.Round(upY) == -1) {
			myAxis = Axis.negY;
			if ((GameController.activeCamera == "Cam1") || (GameController.activeCamera == "Cam2")) {
				Facing = "Down";
			}
		}
		if (System.Math.Round(upZ) == 1) {
			myAxis = Axis.posZ;
			if (GameController.activeCamera == "Cam1") {
				Facing = "Away";
			}
			if (GameController.activeCamera == "Cam2") {
				Facing = "Right";
			}
		}
		if (System.Math.Round(upZ) == -1) {
			myAxis = Axis.negZ;
			if (GameController.activeCamera == "Cam1") {
				Facing = "Towards";
			}
			if (GameController.activeCamera == "Cam2") {
				Facing = "Left";
			}
		}
}



//controll the head
void headController(ref string Facing)
{
	//moves the head
	transform.Translate (0, 0, GameController.speed*Time.deltaTime);

		if (Input.GetKeyDown (KeyCode.LeftArrow) & Facing != "Left" & Facing != "Right") {
			TurnLeft();
		}
		//Right Arrow
		if (Input.GetKeyDown (KeyCode.RightArrow) & Facing != "Right" & Facing != "Left") {
			TurnRight();
		}
		//W
		if (Input.GetKeyDown (KeyCode.W) & Facing != "Towards" & Facing != "Away") {
			TurnAway();
		}
		//s
		if (Input.GetKeyDown (KeyCode.S) & Facing != "Away" & Facing != "Towards") {
				TurnTowards();
		}
		//Down
		if (Input.GetKeyDown (KeyCode.DownArrow) & Facing != "Down" & Facing != "Up") {
			//head.transform.rotation = Quaternion.Euler (90, 0, 0);
			TurnDown();
		}
		//Up
		if (Input.GetKeyDown (KeyCode.UpArrow) & Facing != "Up" & Facing != "Down") {
			TurnUp();
		}
}

public static void TurnRight(){
	if (GameController.activeCamera == "Cam1") {
		//head.transform.rotation = Quaternion.Euler (0, 90, 0);
		head.transform.rotation = Quaternion.LookRotation(Vector3.right);
	}
	if (GameController.activeCamera == "Cam2") {
		//head.transform.rotation = Quaternion.Euler (0, 0, 90);
		head.transform.rotation = Quaternion.LookRotation(Vector3.forward);
	}
	posRound ();
}
public static void TurnLeft(){
	if (GameController.activeCamera == "Cam1") {
		//head.transform.rotation = Quaternion.Euler (head.transform.rotation.x, -90, head.transform.rotation.y);
		head.transform.rotation = Quaternion.LookRotation(-1*Vector3.right);
	}
	if (GameController.activeCamera == "Cam2") {
		//head.transform.rotation = Quaternion.Euler (0, 180, 0);
		head.transform.rotation = Quaternion.LookRotation(-1*Vector3.forward);
	}
	posRound ();
}
public static void TurnAway(){
	if (GameController.activeCamera == "Cam1") {
		//head.transform.rotation = Quaternion.Euler (0, 0, 90);
		head.transform.rotation = Quaternion.LookRotation(Vector3.forward);
		posRound ();
	}
	if (GameController.activeCamera == "Cam2") {
		//head.transform.rotation = Quaternion.Euler (0, -90, 0);
		head.transform.rotation = Quaternion.LookRotation(-1*Vector3.right);
	}
	posRound ();
}
public static void TurnTowards(){
	if (GameController.activeCamera == "Cam1") {
		//head.transform.rotation = Quaternion.Euler (0, 180, 0);
		head.transform.rotation = Quaternion.LookRotation(-1*Vector3.forward);
	}
	if (GameController.activeCamera == "Cam2") {
		//head.transform.rotation = Quaternion.Euler (0, 90, 0);
		head.transform.rotation = Quaternion.LookRotation(Vector3.right);
	}
	posRound ();
}
public static void TurnUp(){
	//head.transform.rotation = Quaternion.Euler (-90, head.transform.rotation.y, head.transform.rotation.z);
	//head.transform.rotation = Quaternion.Euler (-90, 0,0);
	head.transform.rotation = Quaternion.LookRotation(Vector3.up);
	posRound ();
}

public static void TurnDown(){
	head.transform.rotation = Quaternion.LookRotation(-1*Vector3.up);
	posRound ();
}

public static void posRound(){
		//this snaps the head to the grid after a turn
		//head will always be on int's except for the direction it is heading.
		var headPosX = Mathf.Round(head.position.x);
		var headPosY = Mathf.Round(head.position.y);
		var headPosZ = Mathf.Round(head.position.z);
		head.position = new Vector3(headPosX,headPosY,headPosZ);
}
void boundCheck(){
		if (((Math.Abs(head.position.x) > 5) ||
				(Math.Abs(head.position.y) > 4) ||
				(Math.Abs(head.position.z) > 5)) &
				(GameController.status != "Done")){
					//MOVEMENT_RATE = 0.0f;
					GameController.status = "Done2";
				}
}
}
