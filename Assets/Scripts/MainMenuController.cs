using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
	const int MainGameScene = 1;

	public Slider difficultySlider;

	public void LoadGrowthGame ()
	{
		GameManager.CurrentGameMode = GameManager.GameMode.Growth;
		GameManager.OverallDifficultyFactor = difficultySlider.value;
		SceneManager.LoadScene (MainGameScene);
	}

	public void LoadSurvivalGame ()
	{
		GameManager.CurrentGameMode = GameManager.GameMode.Survival;
		GameManager.OverallDifficultyFactor = difficultySlider.value;
		SceneManager.LoadScene (MainGameScene);
	}

}
