using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerAction action;
    public GameObject menu;
    bool menuActive;
    private void Start()
    {
        action = GameObject.FindObjectOfType<PlayerAction>();
        action.OnExitGlobal += Menu;
    }
    private void OnDisable()
    {
        action.OnExitGlobal -= Menu;
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Menu()
    {
        menuActive = !menuActive;
        menu.SetActive(menuActive);
        if (menuActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
