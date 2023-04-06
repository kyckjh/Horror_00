using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadOpening : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    AsyncOperation opening;

    float loadingSpeed = 0.2f;

    private void Start()
    {
        opening = SceneManager.LoadSceneAsync("Opening");
        opening.allowSceneActivation = false;
    }

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, opening.progress + 0.1f, Time.deltaTime * loadingSpeed);
        if (slider.value > 0.5f && loadingSpeed < 0.4f)
        {
            loadingSpeed = 0.5f;
        }
        else if(slider.value > 0.8f && loadingSpeed < 0.9f)
        {
            loadingSpeed = 1.0f;
        }
        else if(slider.value > 0.99f)
        {
            opening.allowSceneActivation = true;
        }
    }
}
