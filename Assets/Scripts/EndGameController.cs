using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameController : MonoBehaviour
{
	const int MainGameScene = 0;

	public void ReturnToMainMenu ()
	{
		SceneManager.LoadScene (MainGameScene);
	}
}

