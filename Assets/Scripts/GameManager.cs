using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public AmoebaManager ProtectedAmoeba;
	public DamageBar DamageBar;
	public CanvasGroup[] GameOverGroup;

	public float maxLashoutLength = 5f;

	public float lashoutDuration { get; private set; }

	private float gameOverTime = -1f;

	// Update is called once per frame
	void Update ()
	{
		if (ProtectedAmoeba.isLashingOut && gameOverTime < 0) {
			lashoutDuration += Time.deltaTime;

			if (lashoutDuration > maxLashoutLength) {
				TriggerGameOut ();
			} else {
				DamageBar.percent = 1f - lashoutDuration / maxLashoutLength;
			}
		} else if (gameOverTime > 0 && Time.timeScale > 0) {
			float newScale = 1f - Mathf.Sqrt (Time.realtimeSinceStartup - gameOverTime);
			Time.timeScale = Mathf.Max (newScale, 0f);

			// TODO fix problem where alpha doesn't technically reach 1.0 before we stop entering this branch
			float newAlpha = (Time.realtimeSinceStartup - gameOverTime);
			foreach (CanvasGroup group in GameOverGroup) {
				group.alpha = newAlpha;
			}
		}
	}

	void TriggerGameOut ()
	{
		// TODO implement effects outside of this class
		DamageBar.percent = 0;
		gameOverTime = Time.realtimeSinceStartup;
	}
}
