using UnityEngine;
using UnityEngine.SceneManagement;

public class Playbutton : MonoBehaviour
{
	public void StartGame()
	{
		SceneManager.LoadScene(1);
	}

	public void Exitgame()
	{
		Application.Quit();
	}
}
