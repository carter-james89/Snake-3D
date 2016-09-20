using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public Camera Cam1;
	public Camera Cam2;
	public static string activeCamera;
	public GameObject Snake;
	//snake speed
	public static float speed;

	//SWITCH THESE TWO IF YOU WANNA SKIP THE FIRST MENU
	//public static string status = "Running";
	public static string status;
	public string gameStatus;
	//for the snake in the prefab to be loaded after restart
	public Object newSnake;
	//speed dropdown on start menu
	public Dropdown DropDown;
	//boxwalls to be moved with cam change
	public GameObject cam1Wall;
	public GameObject cam2Wall;
	public GameObject cam1WallChild;
	public GameObject cam2WallChild;
	public GameObject borderWalls;
	//UIs
	public GameObject startMenu;
	public GameObject quitMenu;
	public GameObject HUD;
	//set start rotation
	public Quaternion startRotation;
	//for debugging, tracking which gameobject has been loaded after restart
	public static int snakecount = 1;
	//score for ui
	public Text finalScoreText;

	void Awake() {
			status = "Pre-Game";
    }

	void Start () {
		//store start rotation
		startRotation = Snake.transform.rotation;

		gameStatus = status;
		//set the cameras and UI
		Cam1.enabled = true;
		Cam2.enabled = false;
		activeCamera = "Cam1";
		WallCheck();
		quitMenu.SetActive(false);
		HUD.SetActive(false);
		//moves the borderwalls into position
		borderWalls.transform.rotation = Quaternion.LookRotation(Vector3.forward);
	}
	// Update is called once per frame
	void Update () {
		gameStatus = status;
		//if the snake hits itself or a wall, end round
		if ((status == "Done1") || (status == "Done2")) {
			EndGame();
		}
		//swith cameras
		if (Input.GetKeyDown (KeyCode.Space)) {
			cameraControl();
		}
	}

public void StartGame(){
		if (DropDown.value == 0){
			speed = 2.5f;
		}
		if (DropDown.value == 1){
			speed = 1.5f;
		}
		if (DropDown.value == 2){
			speed = .8f;
		}
		status = "Running";
		startMenu.SetActive(false);
		HUD.SetActive(true);

	//	Food.isEaten = true;
	}
	public void EndGame(){
		HUD.SetActive(false);
		finalScoreText.text = "Score: " + Food.score.ToString();
		quitMenu.SetActive(true);
	}


	public void RestartGame(){
			//destroy the current snake
			Destroy(Snake);
			//for debugging, makeing sure right script is referenced
			snakecount++;
			//load the prefab duplicate into the scene & set name
			Snake = Instantiate(newSnake, new Vector3(0,0,0), startRotation) as GameObject;
			Snake.name = ("Snake");
			//reset score and place food randomly
			Food.score = 0;
			Food.isEaten = true;
			status = "Pre-Game";
			//reset the scene
			Cam1.enabled = true;
			Cam2.enabled = false;
			activeCamera = "Cam1";
			WallCheck();
			startMenu.SetActive(true);
			quitMenu.SetActive(false);
		}
	public void QuitGame(){
		Application.Quit();
	}

public void cameraControl(){
		 var camChange = true;
		 if ((activeCamera == "Cam1") & (camChange)){
			 Cam2.enabled = true;
			 Cam1.enabled = false;
			 activeCamera = "Cam2";
			 camChange = false;
		 }
		 if ((activeCamera == "Cam2") & (camChange)){
			 Cam1.enabled = true;
			 Cam2.enabled = false;
			 activeCamera = "Cam1";
			 camChange = false;
		 }
		 WallCheck();
	 }

public void WallCheck(){
	if (Cam1.enabled){
		// cam1Wall.transform.position = new Vector3(0,0,-30);
		// cam2Wall.transform.position = new Vector3(5.5f,0,0);
		cam1Wall.SetActive(false);
		cam2Wall.SetActive(true);
	}
	if (Cam2.enabled){
		cam1Wall.SetActive(true);
		cam2Wall.SetActive(false);
	}
}

public void RightButton(){
	SnakeHead.TurnRight();
}
public void LeftButton(){
	SnakeHead.TurnLeft();
}
public void UpButton(){
	SnakeHead.TurnUp();
}
public void DownButton(){
	SnakeHead.TurnDown();
}
public void SButton(){
	SnakeHead.TurnTowards();
}
public void WButton(){
	SnakeHead.TurnAway();
}
}
