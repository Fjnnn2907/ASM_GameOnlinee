using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;

    [Header("Audio Clips")]
    public AudioClip[] backgroundMusicClips;
    public List<Sound> soundClips;

    [Header("Audio Mixer")]
    [SerializeField] protected AudioMixer audioMixer;
    [SerializeField] protected Slider[] slider;
    public void AudioButton(AudioClip audioClip)
    {
        if (!soundEffectSource)
            return;

        soundEffectSource.PlayOneShot(audioClip);
    }

    public void PlayBackgroundMusic(int index)
    {
        if (index < 0 || index >= backgroundMusicClips.Length)
        {
            Debug.LogWarning("Thieu nhac nen");
            return;
        }

        backgroundMusicSource.clip = backgroundMusicClips[index];
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();
    }

    public void PlaySoundEffect(string name)
    {
        Sound sound = soundClips.Find(s => s.nameSound == name);
        if (sound != null && sound.audioClip != null)
        {
            soundEffectSource.PlayOneShot(sound.audioClip);
        }
        else
        {
            Debug.LogWarning($"Thieu effect music {name}");
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }
    public void SetBackGroundVolume()
    {
        float volume = slider[0].value;
        audioMixer.SetFloat("BackGround", Mathf.Log10(volume) * 20);
    }
    public void SetEffectVolume()
    {
        float volume = slider[1].value;
        audioMixer.SetFloat("Effect", Mathf.Log10(volume) * 20);
    }
}

[Serializable]
public class Sound
{
    public string nameSound;
    public AudioClip audioClip;
}
