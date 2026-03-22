using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private int _gameplaySceneIndex = 1;

    public void PlayGame()
    {
        SceneManager.LoadScene(_gameplaySceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
