using System;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public static WorldTime Instance;

    public int Minute { get; private set; }
    public int Hour { get; private set; }
    public int Day { get; private set; }

    public float secondsPerGameMinute = 1f;
    private float timer;

    public static event Action<int> OnHourChanged;
    public static event Action<int> OnDayChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        Minute = 0;
        Hour = 6;
        Day = 1;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerGameMinute)
        {
            timer = 0f;
            AdvanceTime();
        }
    }

    private void AdvanceTime()
    {
        Minute++;
        if (Minute >= 60)
        {
            Minute = 0;
            Hour++;
            OnHourChanged?.Invoke(Hour);

            if (Hour >= 24)
            {
                Hour = 0;
                Day++;
                OnDayChanged?.Invoke(Day);
            }
        }
    }

    public string GetFormattedTime()
    {
        return $"Day {Day} - {Hour:00}:{Minute:00}";
    }
}
