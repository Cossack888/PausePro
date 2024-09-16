using TMPro;
using UnityEngine;

public class Momentum : MonoBehaviour
{
    private float currentMomentum;
    [SerializeField] float maxMomentum;
    [SerializeField] TMP_Text momentum;
    private void Start()
    {
        momentum.text = currentMomentum.ToString();
    }
    public float CurrentMomentum => currentMomentum;
    public float MaxMomentum => maxMomentum;
    public void ModifyMomentum(float amount)
    {
        currentMomentum += amount;
        currentMomentum = Mathf.Clamp(currentMomentum, 0, maxMomentum);
        momentum.text = "Momentum:  " + currentMomentum.ToString("F2");
    }
    public void ResetMomentum()
    {
        currentMomentum = 0;
    }
}
