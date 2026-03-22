using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _stageIndexText;

    [SerializeField]
    private int _mainMenuIndex = 0;

    public void SetStageIndex(int stageIndex)
    {
        _stageIndexText.text = "Stage " + (stageIndex + 1).ToString();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(_mainMenuIndex);
    }

    private void OnDisable()
    {
        GameStateManager.Instance.ChangeGameState(GameState.Gameplay);
    }
}
