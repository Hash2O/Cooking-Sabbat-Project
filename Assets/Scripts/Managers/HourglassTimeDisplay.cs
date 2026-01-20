using TMPro;
using UnityEngine;

public class HourglassTimeDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged += UpdateTimerUI;

            // Mise à jour immédiate à l’apparition
            UpdateTimerUI(TimeManager.Instance.CurrentTime);
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged -= UpdateTimerUI;
        }
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText == null) return;

        timerText.text = TimeManager.Instance.GetFormattedTime();
    }
}
