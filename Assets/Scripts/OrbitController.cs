using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public Vector3 orbitPoint;
	public float radius = 2.5f;
	public float latencyFactor = 0.2f;
	public float inertia = 0;
	public float maxAngularVelocity = 360f; // Degrees / sec
	public float angularAcceleration = 120f; // Degrees / sec ^ 2
	public float frictionCoefficient = 0.7f;

	// Might need to be collected from camera in the future, but fixed is fine for now.
	public int cameraHeight = 20;

	private float angularVelocity = 0f;
	private Vector3 targetPosition;

	void Start () {
		targetPosition = this.transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		calculateAngularVelocity ();

		float angularDisplacement = angularVelocity * Time.deltaTime;
			
		Quaternion quaternion = Quaternion.AngleAxis (angularDisplacement, Vector3.up);

		this.transform.position = quaternion * this.transform.position.normalized * radius;
	}

	private void calculateAngularVelocity ()
	{
		if (GameManager.controlType == GameManager.ControlType.MousePosition) {
			calculateAngularVelocityMousePosition ();
		} else if (GameManager.controlType == GameManager.ControlType.Click) {
			calculateAngularVelocityClick ();
		} else if (GameManager.controlType == GameManager.ControlType.Buttons) {
			calculateAngularVelocityButtons ();
		}
	}

	private void calculateAngularVelocityMousePosition ()
	{
		Vector2 mousePosition = Input.mousePosition;
		Vector3 targetPositionVector = getInworldPosition (mousePosition);

		angularVelocity = calculateAngularDeltaSigned (targetPositionVector) / latencyFactor;
	}

	private void calculateAngularVelocityClick ()
	{
		if (Input.GetMouseButton(0)) {
			Vector2 screenPosition = Input.mousePosition;
			targetPosition = getInworldPosition (screenPosition);
		} else if (Input.touchCount == 1) {
			Touch touch = Input.GetTouch (0);
			Vector2 screenPosition = touch.position;
			targetPosition =  getInworldPosition (screenPosition);
		}

		angularVelocity = calculateAngularDeltaSigned(targetPosition) / latencyFactor;
	}

	private void calculateAngularVelocityButtons ()
	{
		if (Input.GetKey ("right")) {
			angularVelocity += angularAcceleration * Time.deltaTime;
			angularVelocity = Mathf.Min (angularVelocity, maxAngularVelocity);
		} else if (Input.GetKey ("left")) {
			angularVelocity -= angularAcceleration * Time.deltaTime;
			angularVelocity = Mathf.Max (angularVelocity, -maxAngularVelocity);
		} else if (angularVelocity > 0) {
			angularVelocity -= angularVelocity * frictionCoefficient * Time.deltaTime;
			angularVelocity = Mathf.Max (angularVelocity, 0);
		} else if (angularVelocity < 0) {
			angularVelocity += -angularVelocity * frictionCoefficient * Time.deltaTime;
			angularVelocity = Mathf.Min (angularVelocity, 0);
		}
	}

	private float calculateAngularDeltaSigned (Vector3 targetDirectionVector)
	{
		float deltaAngle = Vector3.Angle (this.transform.position, targetDirectionVector);
		Vector3 cross = Vector3.Cross (this.transform.position, targetDirectionVector);
		if (cross.y < 0) {
			deltaAngle = -deltaAngle;
		}
		return deltaAngle;
	}

	private Vector3 getInworldPosition (Vector2 position)
	{
		Vector3 mouse3d = new Vector3 (position.x, position.y, cameraHeight);
		return Camera.main.ScreenToWorldPoint (mouse3d);
	}
}
