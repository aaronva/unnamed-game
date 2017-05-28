using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
	public enum ControlScheme
	{
		MousePointer,
		Buttons
	}

	public ControlScheme controlScheme;
	public Vector3 orbitPoint;
	public float radius = 2.5f;
	public float latencyFactor = 0.2f;
	public float inertia = 0;
	public float maxAngularVelocity = 360f; // Degrees / sec
	public float angularAcceleration = 120f; // Degrees / sec ^ 2
	public float frictionCoefficient = 0.7f;

	// Might need to be computed in the future, but fixed is fine for now.
	public int cameraHeight = 20;

	private float angularVelocity = 0f;

	// Update is called once per frame
	void Update ()
	{
		if (controlScheme == ControlScheme.MousePointer) {
			calculateMousePointerAngle ();
		} else if (controlScheme == ControlScheme.Buttons) {
			calculateButtonsAngle ();
		}

		float angularDisplacement = angularVelocity * Time.deltaTime;
			
		Quaternion quaternion = Quaternion.AngleAxis (angularDisplacement, Vector3.up);

		this.transform.position = quaternion * this.transform.position.normalized * radius;
	}

	void calculateButtonsAngle ()
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

	void calculateMousePointerAngle ()
	{
		Vector3 targetDirectionVector = getInworldMousePosition ();

		angularVelocity = computeSignedAngle (targetDirectionVector) / latencyFactor;
	}

	private Vector3 getInworldMousePosition ()
	{
		Vector2 mousePosition = Input.mousePosition;
		Vector3 mouse3d = new Vector3 (mousePosition.x, mousePosition.y, cameraHeight);
		return Camera.main.ScreenToWorldPoint (mouse3d);
	}

	float computeSignedAngle (Vector3 targetDirectionVector)
	{
		float deltaAngle = Vector3.Angle (this.transform.position, targetDirectionVector);
		Vector3 cross = Vector3.Cross (this.transform.position, targetDirectionVector);
		if (cross.y < 0) {
			deltaAngle = -deltaAngle;
		}
		return deltaAngle;
	}
}
