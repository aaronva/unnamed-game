using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	const int MainGameScene = 1;

	public void LoadGrowthGame ()
	{
		GameManager.CurrentGameMode = GameManager.GameMode.Growth;
//		GameManager.OverallDifficultyFactor = 0.5f + difficultyScaling;
		SceneManager.LoadScene (MainGameScene);
	}

	public void LoadSurvivalGame ()
	{
		GameManager.CurrentGameMode = GameManager.GameMode.Survival;
//		GameManager.OverallDifficultyFactor = 0.5f + difficultyScaling;
		SceneManager.LoadScene (MainGameScene);
	}

}
