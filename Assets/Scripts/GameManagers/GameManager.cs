using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerAction action;
    private void Start()
    {
        action = GameObject.FindObjectOfType<PlayerAction>();
        action.OnResetGlobal += ResetScene;
        action.OnExitGlobal += ExitGame;
    }
    private void OnDisable()
    {
        action.OnResetGlobal -= ResetScene;
        action.OnExitGlobal -= ExitGame;
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
