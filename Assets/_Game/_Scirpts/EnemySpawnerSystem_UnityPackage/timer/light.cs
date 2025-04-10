using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldLight : MonoBehaviour
{
    private Light2D light2D;

    [SerializeField]
    private Gradient lightColorOverDay;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        WorldTime.OnHourChanged += UpdateLightColor;
    }

    private void OnDisable()
    {
        WorldTime.OnHourChanged -= UpdateLightColor;
    }

    private void Update()
    {
        if (WorldTime.Instance != null)
        {
            float percent = GetPercentOfDay(WorldTime.Instance.Hour, WorldTime.Instance.Minute);
            light2D.color = lightColorOverDay.Evaluate(percent);
        }
    }

    private float GetPercentOfDay(int hour, int minute)
    {
        return (hour * 60f + minute) / 1440f; // 1440 = 24 * 60
    }

    private void UpdateLightColor(int hour)
    {
        // Có thể dùng nếu chỉ muốn cập nhật mỗi giờ
        float percent = GetPercentOfDay(hour, WorldTime.Instance.Minute);
        light2D.color = lightColorOverDay.Evaluate(percent);
    }
}
