using UnityEngine;
using System.Collections;

public class CirclePoints {


	public static Vector3[] getPointsOnCircle (float diameter, int pointCount) {
		Vector3[] points = new Vector3[pointCount];
		float separationDegrees = (Mathf.PI*360/180) / pointCount;
		float radius = diameter / 2;
		for (int i = 0; i < pointCount; i++) {
			float nextAngle = 0 + separationDegrees * i;
			float x = radius * Mathf.Cos (nextAngle);
			float y = radius * Mathf.Sin (nextAngle);
			points [i] = new Vector3 (x, y, 0);
			//Debug.Log (points[i]);
		}

		return points;
	}


	public static Vector3[] rotatePointsAroundAxis (Vector3[] originalPoints, Vector3 rotation) {
		Vector3[] rotatedPoints = new Vector3[originalPoints.Length];

		for (int i = 0; i < rotatedPoints.Length; i++) {
			rotatedPoints [i] = Quaternion.Euler (rotation) * originalPoints [i];
		}

		return rotatedPoints;
	}
}
