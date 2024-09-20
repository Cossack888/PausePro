using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerAction action;
    private PlayerHealth health;
    public GameObject menu;
    public TMP_Text bottlesText;
    bool menuActive;
    int bottles;
    private void Start()
    {
        action = GameObject.FindObjectOfType<PlayerAction>();
        health = GameObject.FindObjectOfType<PlayerHealth>();
        action.OnExitGlobal += Menu;
    }
    private void OnDisable()
    {
        action.OnExitGlobal -= Menu;
    }
    public void ChangeAmountOfBottles(int amount)
    {
        bottles += amount;
        bottlesText.text = bottles.ToString();
    }
    public int GetBottles()
    {
        return bottles;
    }
    public void ResetScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Menu()
    {
        if (!health.deathScreen.activeSelf)
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

    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
