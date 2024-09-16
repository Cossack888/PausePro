using TMPro;
using UnityEngine;

public class Prompts : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;
    private void Start()
    {
        ErasePrompt();
    }
    public void ChangePrompt(int promptNumber)
    {
        switch (promptNumber)
        {
            case 0:
                promptText.text = "Jump - Space/Button South";
                break;
            case 1:
                promptText.text = "Wall Run - press Jump next to wall. Press Z/button East to switch sides";
                break;
            case 2:
                promptText.text = "Crouch - hold LCtrl/LShoulder";
                break;
            case 3:
                promptText.text = "Press X/button West to DASH";
                break;
        }
        Invoke("ErasePrompt", 2);
    }
    public void ErasePrompt()
    {
        promptText.text = "";
    }
}
