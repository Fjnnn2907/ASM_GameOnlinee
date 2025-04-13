using Photon.Pun;
using System;
using UnityEngine;
using Photon.Pun;

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
    private PhotonView view;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        view = GetComponent<PhotonView>();
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
        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;
        if (timer >= secondsPerGameMinute)
        {
            timer = 0f;
            AdvanceTime();

            PhotonView view = GetComponent<PhotonView>();
            if (view != null)
            {
                view.RPC("SyncTime", RpcTarget.Others, Minute, Hour, Day);
            }
            else
            {
                Debug.LogWarning("PhotonView is missing on WorldTime GameObject!");
            }

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

    [PunRPC]
    void SyncTime(int minute, int hour, int day)
    {
        Minute = minute;
        if (Hour != hour)
        {
            Hour = hour;
            OnHourChanged?.Invoke(hour);
        }

        if (Day != day)
        {
            Day = day;
            OnDayChanged?.Invoke(day);
        }
    }

    public string GetFormattedTime()
    {
        return $"Day {Day} - {Hour:00}:{Minute:00}";
    }
}
