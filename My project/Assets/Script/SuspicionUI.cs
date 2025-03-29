using UnityEngine;
using UnityEngine.UI;

public class SuspicionUI : MonoBehaviour
{
    // This class manage the suspicion UI.

    [SerializeField] private Slider suspicionSlider;

    private void Start()
    {
        suspicionSlider.value = 0;
        SuspicionManager.Instance.OnSuspicionChanged += UpdateUI;
    }

    private void UpdateUI(float suspiciousAmount)
    {
        //print("UI CHANGE");
        suspicionSlider.value = suspiciousAmount;
    }

    private void OnDisable()
    {
        SuspicionManager.Instance.OnSuspicionChanged -= UpdateUI;
    }
}
