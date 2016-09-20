using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//this is code that Joe B was nice enough to write for me
//I dont understand all of it
//I stripped out the head controll and this controls the body and tail
//lots of room for optimization in the snake body
public class Snake : MonoBehaviour {
//	private const float MOVEMENT_RATE = 0.025f;
	private float MOVEMENT_RATE = .01f; //0.06f;
	//private const float ROTATION_RATE = 2.0f;
	private const float SEGMENT_DISTANCE = 0.25f;

	protected List <Segment> 	segments;
	public List <Vector3> 	vertices;
	protected List <Vector2> 	uvs;
	protected List <int> 		triangles;

	private MeshFilter meshFilter;

	public Transform tail;
	public Transform head;

	//private int 	totalSegments;
	private int 	bodyCount;

	//private float timeTillNewBodySegment;
	public static bool expanding = true;
	int i;

	// Use this for initialization
	void Awake () {
	}
	void Start () {
		segments 	= new List <Segment> ();
		vertices 	= new List <Vector3> ();
		uvs 		= new List <Vector2> ();
		triangles	= new List <int> ();

		meshFilter = gameObject.GetComponent<MeshFilter> ();
		Texture2D snakeskinTexture = Resources.Load ("green skin") as Texture2D;
		Material snakeskinMaterial = new Material (Shader.Find("Mobile/Diffuse"));
		snakeskinMaterial.mainTexture = snakeskinTexture;
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();
		meshRenderer.material = snakeskinMaterial;

		bodyCount	= 1;

		segments.Add(new Segment(this, head.position, head.eulerAngles));
		//timeTillNewBodySegment = 1.0f;
	}

	// Update is called once per frame
	void Update () {
		//if the game is running, process the tail and body
		if (GameController.status == "Running"){
			processBody ();
			processTail ();
		}

		//timeTillNewBodySegment -= Time.deltaTime;
		//if the food is eaten, add to the body
		if (Food.needNewBody == true || Input.GetKeyDown(KeyCode.N)){
		 	bodyCount++;
			Food.needNewBody = false;
		}
	}

	private void updateMesh () {
		Mesh newMesh = new Mesh ();
		newMesh.vertices 	= vertices.ToArray();
		newMesh.triangles 	= triangles.ToArray();

		Vector2 change = new Vector2(0, -(MOVEMENT_RATE));
		for (int i = 0; i < uvs.Count; i++) {
			uvs [i] += change;
		}

		newMesh.uv 			= uvs.ToArray ();
		meshFilter.mesh = newMesh;
	}

	private void processBody () {
		bool beginningUpdated = false;
		bool endingUpdated = false;

		Vector3 lastSegmentPosition = segments [segments.Count - 1].position;
		Vector3 headPosition = head.transform.position;
		float distance = (headPosition - lastSegmentPosition).magnitude;

		if (distance > SEGMENT_DISTANCE) {
			segments.Add(new Segment(this, headPosition, head.transform.rotation.eulerAngles));
			beginningUpdated = true;
			expanding = true;
		}

		if (segments.Count > bodyCount*30) {
			segments[0].removeSegment(this);
			segments.RemoveAt(0);
			endingUpdated = true;
			expanding = false;
		}

		//if (beginningUpdated || endingUpdated) {
			updateMesh ();
		//}
	}

	private void processTail () {
		if (segments.Count >= 5) {
			Vector3 nextPosition = segments[1].position;
			//Debug.Log ("Pos: "+ nextPosition);
			tail.LookAt (nextPosition, Vector3.up);

			if ((nextPosition - tail.position).magnitude > SEGMENT_DISTANCE) {
				//tail.Translate (tail.forward * MOVEMENT_RATE, Space.World);
				tail.transform.Translate (0, 0, GameController.speed*Time.deltaTime);
			}
		}
	}
	protected int segmentTicker = 0;

  public class Segment {

    public const int VERTEX_COUNT = 8;
    public const float BODY_DIAMETER = 0.85f;
    public Vector3 position { get; private set; }

    public Segment(Snake snake, Vector3 globalPosition, Vector3 globalRotation) {
      position = globalPosition;
      int offset = snake.segments.Count;

      // Create the new set of vertices.
      Vector3[] calculatedPoints = CirclePoints.getPointsOnCircle(BODY_DIAMETER, VERTEX_COUNT);
      Vector3[] rotatedPoints = CirclePoints.rotatePointsAroundAxis(calculatedPoints, globalRotation);
      Vector3[] vertices = new Vector3[VERTEX_COUNT];
      Vector2[] uvs = new Vector2[VERTEX_COUNT];

			//makes a ring of 8 points at the same point as the snake head
      for (int i = 0; i < VERTEX_COUNT; i++) {
        vertices[i] = globalPosition + rotatedPoints[i];
        uvs[i] = new Vector2(i * (1 / VERTEX_COUNT), 0 + SEGMENT_DISTANCE * snake.segmentTicker);
      }

      snake.segmentTicker++;

      snake.vertices.AddRange(vertices);
      snake.uvs.AddRange(uvs);

      if (snake.segments.Count > 0) {
        int[] triangles = new int[VERTEX_COUNT * 6];

      	int vertexOffset;

        for (int i = 0, j = 0; i < VERTEX_COUNT; i++, j += 6) {
        	vertexOffset = i + offset * VERTEX_COUNT;

          int lastVertex;
          if (vertexOffset + 1 == snake.vertices.Count) lastVertex = snake.vertices.Count - VERTEX_COUNT;
          else lastVertex = vertexOffset + 1;

          int finalFinalVertex;
          if (j + 5 == triangles.Length - 1) finalFinalVertex = vertexOffset - (VERTEX_COUNT * 2) + 1;
          else finalFinalVertex = vertexOffset - VERTEX_COUNT + 1;

          // Face 1
          triangles[j] = vertexOffset - VERTEX_COUNT;
          triangles[j + 1] = lastVertex;
          triangles[j + 2] = vertexOffset;

          // Face 2
          triangles[j + 3] = finalFinalVertex;
          triangles[j + 4] = lastVertex;
          triangles[j + 5] = vertexOffset - VERTEX_COUNT;
          }

          snake.triangles.AddRange(triangles);
      	}

            /*
			Debug.Log("SEGS: "+ snake.segments.Count);
			Debug.Log("VERTS: ");
			for (int i = 0; i < snake.vertices.Count; i++) Debug.Log("["+i+"] "+snake.vertices[i]);
			Debug.Log("TRIS: ");
			for (int i = 0; i < snake.triangles.Count; i++) Debug.Log("["+i+"] "+snake.triangles[i]);
			Debug.Log("");
			*/
    }
		public void removeSegment (Snake snake) {
			snake.triangles.RemoveRange (0, VERTEX_COUNT*6);
			snake.uvs.RemoveRange (0, VERTEX_COUNT);
			snake.vertices.RemoveRange (0, VERTEX_COUNT);
			for (int i = 0; i < snake.triangles.Count; i++) {
				snake.triangles[i] -= VERTEX_COUNT;
			}
		}
	}
}
