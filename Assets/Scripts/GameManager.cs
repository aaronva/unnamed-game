using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	const int MainGameScene = 0;

	public enum GameMode
	{
		Growth,
		Survival
	}

	public AmoebaManager ProtectedAmoeba;
	public DamageBar DamageBar;
	public UnityEngine.UI.Text GrowthLevelText;
	public UnityEngine.UI.Text GameOverScoreText;
	public CanvasGroup GameOverCanvas;

	public static float MaxLashoutLength = 1f;

	public static float LashoutDuration { get; private set; }

	public static float GrowthLevelIncrement = 10f;
	public static float OverallDifficultyFactor = 0.5f;
	public static GameMode CurrentGameMode = GameMode.Survival;

	public static int GrowthLevel { get; private set; }

	public static float CurrentDifficulty { get; private set; }

	private float gameOverTime = -1f;

	void Start ()
	{
		GameOverCanvas.alpha = 0f;
		if (CurrentGameMode == GameMode.Survival) {
			GrowthLevelText.text = "";
		}
	}

	void Update ()
	{
		CurrentDifficulty = ComputeTimeBasedDifficulty ();

		GrowthLevel = (int)Mathf.Floor (ProtectedAmoeba.MaxStressLevel / GrowthLevelIncrement);
		
		if (CurrentGameMode == GameMode.Growth) {
			GrowthLevelText.text = GrowthLevel.ToString ();
		}

		if (ProtectedAmoeba.IsLashingOut && gameOverTime < 0) {
			LashoutDuration += Time.deltaTime;

			if (LashoutDuration > MaxLashoutLength) {
				TriggerGameOver ();
			} else {
				DamageBar.percent = 1f - LashoutDuration / MaxLashoutLength;
			}
		} else if (gameOverTime > 0 && (Time.timeScale > 0 || GameOverCanvas.alpha < 1)) {
			if (Time.timeScale > 0) {
				float newScale = 1f - Mathf.Sqrt (Time.realtimeSinceStartup - gameOverTime);
				Time.timeScale = Mathf.Max (newScale, 0f);
			}

			// TODO fix problem where alpha doesn't technically reach 1.0 before we stop entering this branch
			float newAlpha = (Time.realtimeSinceStartup - gameOverTime);
			GameOverCanvas.alpha = newAlpha;
		}
	}

	void TriggerGameOver ()
	{
		const string SurvivalString = "Time: ";
		const string GrowthString = "Size: ";

		// TODO implement effects outside of this class
		DamageBar.percent = 0;
		gameOverTime = Time.realtimeSinceStartup;
		if (CurrentGameMode == GameMode.Survival) {
			float seconds = gameOverTime % 60;
			int minutes = (int)gameOverTime / 60;

			GameOverScoreText.text = SurvivalString + minutes.ToString() + ':' + seconds.ToString ("00");
		} else if (CurrentGameMode == GameMode.Growth) {
			GameOverScoreText.text = GrowthString + GrowthLevel.ToString ();
		}
	}

	private static float ComputeTimeBasedDifficulty ()
	{
		// Period (in sec) between difficulty spikes (first spike is at period / 2)
		const float period = 25f;
		const float periodAmplitude = 8f;
		const float startingDifficulty = 10f;

		const float difficultyIncreasePerPeriod = 4f;
		const float difficultyIncreasePerGrowthLevel = 10f;

		if (CurrentGameMode == GameMode.Growth) {
			return OverallDifficultyFactor *
			(startingDifficulty + difficultyIncreasePerGrowthLevel * GrowthLevel
			- periodAmplitude * Mathf.Cos (Time.time * 2 * Mathf.PI / period));
		} else {
			return OverallDifficultyFactor *
			(startingDifficulty + Time.time * difficultyIncreasePerPeriod / period
			- periodAmplitude * Mathf.Cos (Time.time * 2 * Mathf.PI / period));
		}
	}

	public void ReturnToMainMenu ()
	{
		SceneManager.LoadScene (MainGameScene);
	}

}
