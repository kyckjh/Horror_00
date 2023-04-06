using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    AsyncOperation mainScene;

    private void Start()
    {
        mainScene = SceneManager.LoadSceneAsync("MainScene2");
        mainScene.allowSceneActivation = false;
    }

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, mainScene.progress + 0.1f, Time.deltaTime);
        if(slider.value > 0.99f)
        {
            mainScene.allowSceneActivation = true;
        }
    }
}
