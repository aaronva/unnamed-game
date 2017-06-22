using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

	public float DisplacementRatio = 0.4f;
	public float AngularDisplacementRatio = 0.4f;
	public OrbitController movingTarget;
	public Transform centralTarget;

	private Vector3 centralPosition;

	public float MaxDistanceOutsideOfCamera { get; private set; }

	public float InWorldHeight {
		get {
			return centralPosition.y - centralTarget.position.y;
		}
		set {
			this.centralPosition.y = centralTarget.position.y + value;
		}
	}

	void Start ()
	{
		centralPosition = this.transform.position;
		UpdateMaxDistanceOutsideOfCamera ();
	}

	void Update ()
	{
		Vector3 targetDisplacement = ComputeNewPosition (movingTarget.transform.position);
		this.transform.position = centralPosition + targetDisplacement;
	}

	private Vector3 ComputeNewPosition (Vector3 lookingPosition)
	{
		return lookingPosition * DisplacementRatio;
	}

	// This distance accounts for the object orbit displacement effect, but not if the central point camera
	// moves at all or if the orbit radius changes. While there are no plans to have these properties change
	// live, if they do, this function needs to be notified (likely through Events)
	private void UpdateMaxDistanceOutsideOfCamera ()
	{
		Camera camera = this.GetComponent<Camera> ();

		// Get the top right corner of the camera in worldview
		Vector3 inWorldPosition = camera.ViewportToWorldPoint (new Vector3 (1, 1, InWorldHeight));
		Vector3 inWorldDelta = inWorldPosition - (this.transform.position - new Vector3 (0, InWorldHeight, 0));

		// Add 30% buffer to account for adjustments in the camera movement.
		MaxDistanceOutsideOfCamera = inWorldDelta.magnitude * 1.3f;
	}
}