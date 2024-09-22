using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public void ChangeScene(int sceneBuildIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
