using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WinTotem : MonoBehaviour
{
    bool inRange;
    private PlayerAction actions;
    private int currentScore = 0;
    public TMP_Text pointsText;
    public TMP_Text prompt;
    public int winningScreenIndex;
    public bool victoryCondition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
            prompt.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
            prompt.enabled = false;
        }
    }
    public void AddPoints(int points)
    {
        currentScore += points;
        pointsText.text = currentScore.ToString();
        CheckAndUpdateHighScore();
    }
    public void Interact()
    {
        if (inRange && victoryCondition)
        {
            CheckAndUpdateHighScore();
            SceneManager.LoadScene(winningScreenIndex);
        }

    }

    private void CheckAndUpdateHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    public void EndGame()
    {
        CheckAndUpdateHighScore();
        SceneManager.LoadScene(winningScreenIndex);
    }
    private void OnEnable()
    {
        actions = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAction>();
        actions.OnInteractGlobal += Interact;
    }
    private void OnDisable()
    {
        actions.OnInteractGlobal -= Interact;
    }
}
