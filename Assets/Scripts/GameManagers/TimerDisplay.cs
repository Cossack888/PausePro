using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    public TMP_Text timerText;
    private float timeInSeconds;
    private bool timerOn;
    private void Start()
    {
        timerOn = true;
    }
    void Update()
    {
        if (timerOn)
        {
            timeInSeconds += Time.deltaTime;
        }
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        string timeFormatted = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        if (timerText != null)
        {
            timerText.text = timeFormatted;
        }
    }
    public void StopTimer()
    {
        timerOn = false;
    }
}