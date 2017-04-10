using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBar : MonoBehaviour
{

	public enum RelativePosition { Left, Center, Right }

	public float percent { get; set; }

	public RelativePosition dockedPosition = RelativePosition.Left;

	public Vector3 startingPosition { get; private set; }

	public float startingXScale { get; private set; }

	void Start ()
	{
		percent = 1f;
		startingPosition = this.transform.localPosition;
		startingXScale = this.transform.localScale.x;
	}

	void Update ()
	{
		float newXScale = startingXScale * percent;
		this.transform.localScale = new Vector3 (newXScale, this.transform.localScale.y, this.transform.localScale.z);

		UpdatePosition ();
	}

	void UpdatePosition ()
	{
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();

		float newXPosition = startingPosition.x;

		if (dockedPosition == RelativePosition.Left) {
			// Don't need to do anything
			return; 
		} else if (dockedPosition == RelativePosition.Center) {
			newXPosition += (startingXScale - this.transform.localScale.x) * spriteRenderer.sprite.bounds.size.x / 2;
		} else if (dockedPosition == RelativePosition.Right) {
			newXPosition += (startingXScale - this.transform.localScale.x) * spriteRenderer.sprite.bounds.size.x;
		}

		this.transform.localPosition = new Vector3 (newXPosition, this.transform.localPosition.y, this.transform.localPosition.z);
	}
}
