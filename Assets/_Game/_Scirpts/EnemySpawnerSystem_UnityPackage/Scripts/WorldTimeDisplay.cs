using TMPro;
using UnityEngine;

public class WorldTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    void Update()
    {
        if (WorldTime.Instance != null)
        {
            timeText.text = WorldTime.Instance.GetFormattedTime();
        }
    }
}
