using UnityEngine;
using UnityEngine.UI;

public class TimeStatusIconUI : MonoBehaviour
{
    public Image iconImage;
    public Sprite morningIcon;
    public Sprite afternoonIcon;
    public Sprite eveningIcon;
    public Sprite nightIcon;

    private void Update()
    {
        if (WorldTime.Instance == null) return;

        int hour = WorldTime.Instance.Hour;

        if (hour >= 6 && hour < 12)
            iconImage.sprite = morningIcon;
        else if (hour >= 12 && hour < 18)
            iconImage.sprite = afternoonIcon;
        else if (hour >= 18 && hour < 21)
            iconImage.sprite = eveningIcon;
        else
            iconImage.sprite = nightIcon;
    }
}
