using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeSignal : MonoBehaviour
{
    public int sceneNumber;

    public void LoadScene()
    {
        SceneChangeManager.Inst.LoadScene(sceneNumber);
    }

    public void DestroyObjects()
    {
        DontDestroyObject obj = FindObjectOfType<DontDestroyObject>();
        Destroy(obj.gameObject);
    }

    public void Car_Door_SFX()
    {
        AudioManager.Inst.PlaySFX("Car_Door_Close");
    }

    public void Car_Go_SFX()
    {
        AudioManager.Inst.PlaySFX("Car_Go");
    }

    public void ChangeBGM()
    {
        AudioManager.Inst.PlayBGM("Beethoven_BGM3", MusicTransition.CrossFade, 5.0f);
    }
}
