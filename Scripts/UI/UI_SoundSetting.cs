using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SoundSetting : MonoBehaviour
{
    public enum Sound
    {
        BGM,
        SFX
    }

    public Sound sound;

    Scrollbar bar;

    private void Awake()
    {
        bar = GetComponent<Scrollbar>();
    }

    private void Start()
    {
        bar.onValueChanged.AddListener(ScrollbarCallback);
    }

    void ScrollbarCallback(float volume)
    {
        if(sound == Sound.BGM)
        {
            // BGM 볼륨 설정
            AudioManager.Inst.MusicVolume = volume;
        }
        else
        {
            // 효과음 볼륨 설정
            AudioManager.Inst.SoundVolume = volume;
        }
    }
}
