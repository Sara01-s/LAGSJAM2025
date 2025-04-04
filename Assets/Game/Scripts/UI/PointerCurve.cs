using System.Collections.Generic;
using UnityEngine;

public class PointerCurve : MonoBehaviour {
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private Vector3[] points;
	private float angle;
	private int numberPoints;
	private float velocity;
	private float gravity;
	private Transform startPoint;
	public float separation = 0.01f;
	public GameObject dot;
	private List<GameObject> dots = new List<GameObject> { };
	private void Awake() {
		startPoint = GetComponent<Transform>();
	}
	public void Draw(float newAngle, int quantity, float vel, float g) {
		angle = newAngle;
		if (quantity > numberPoints) {
			CreateDots(quantity - numberPoints);
		}
		if (quantity < numberPoints) {
			DeleteDots(numberPoints - quantity);
		}
		numberPoints = quantity;

		velocity = vel;
		gravity = g;
		Calculate();
		DrawDots();
	}
	private void CreateDots(int quantity) {
		for (int i = 0; i < quantity; i++) {
			GameObject newDot = Instantiate(dot, startPoint);
			dots.Add(newDot);
		}
	}
	private void DeleteDots(int quantity) {
		for (int i = 0; i < quantity; i++) {
			GameObject toDelete = dots[0];
			Destroy(toDelete);
			dots.RemoveAt(0);
		}
	}
	private void OnDestroy() {
		foreach (GameObject d in dots) {
			Destroy(d);
		}
	}
	private void Calculate() {
		points = new Vector3[numberPoints];
		float rads = angle * Mathf.Deg2Rad;
		float vX = velocity * Mathf.Cos(rads);
		float vY = velocity * Mathf.Sin(rads);
		for (int i = 1; i < numberPoints + 1; i++) {
			float t = separation * i;
			float x = vX * t;
			float y = vY * t - 0.5f * gravity * t * t;
			points[i - 1] = startPoint.position + new Vector3(x, y + 0.5f, 0);
		}
	}
	private void DrawDots() {
		if (dots.Count > 0) {
			for (int i = 0; i < numberPoints; i++) {
				GameObject dot = dots[i];
				dot.transform.position = points[i];
			}
		}
	}
}
