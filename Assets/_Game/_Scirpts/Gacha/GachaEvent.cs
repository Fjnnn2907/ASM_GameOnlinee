using TMPro;
using UnityEngine;

public class GachaEvent : Gacha
{
    [SerializeField] private TextMeshProUGUI eventTimerText;
    [SerializeField] private float eventTimer;

    private void Update()
    {
        if (eventTimer > 0)
            eventTimer -= Time.deltaTime;
        else
            eventTimer = 0;

        int totalSeconds = Mathf.FloorToInt(eventTimer);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        eventTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
