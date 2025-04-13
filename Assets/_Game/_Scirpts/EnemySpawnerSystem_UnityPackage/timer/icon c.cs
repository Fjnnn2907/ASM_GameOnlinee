using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class IconScheduleManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image iconImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<Schedule> schedules;
    [SerializeField] private float fadeDuration = 1f;

    private Sprite currentIcon;
    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        WorldTime.OnHourChanged += CheckSchedule;
        WorldTime.OnDayChanged += CheckSchedule;
    }

    private void OnDisable()
    {
        WorldTime.OnHourChanged -= CheckSchedule;
        WorldTime.OnDayChanged -= CheckSchedule;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CheckSchedule(0); // Master kiểm tra lúc bắt đầu
        }
    }

    private void CheckSchedule(int _)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int currentHour = WorldTime.Instance.Hour;
        int currentMinute = WorldTime.Instance.Minute;

        var schedule = schedules.FirstOrDefault(s => s.Hour == currentHour && s.Minute == currentMinute);

        if (schedule != null)
        {
            photonView.RPC("ApplyScheduleRPC", RpcTarget.All,
                schedule.Icon != null ? schedule.Icon.name : "",
                schedule.Sound != null ? schedule.Sound.name : "");
        }
    }

    [PunRPC]
    void ApplyScheduleRPC(string iconName, string soundName)
    {
        Sprite newIcon = schedules.FirstOrDefault(s => s.Icon != null && s.Icon.name == iconName)?.Icon;
        AudioClip newSound = schedules.FirstOrDefault(s => s.Sound != null && s.Sound.name == soundName)?.Sound;

        if (newIcon != null && iconImage != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeIcon(newIcon));
        }

        if (newSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(newSound);
        }
    }

    private IEnumerator FadeIcon(Sprite newIcon)
    {
        if (iconImage == null) yield break;

        // Fade out
        float t = 0f;
        Color originalColor = iconImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            iconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        iconImage.sprite = newIcon;

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            iconImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        currentIcon = newIcon;
    }

    [Serializable]
    public class Schedule
    {
        [Range(0, 23)] public int Hour;
        [Range(0, 59)] public int Minute;
        public Sprite Icon;
        public AudioClip Sound;
    }
}
