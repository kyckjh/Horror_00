using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    AsyncOperation scene1;
    AsyncOperation scene2;
    AsyncOperation scene3;
    AsyncOperation mainScene;

    private void Start()
    {
        scene1 = SceneManager.LoadSceneAsync(2);
        scene1.allowSceneActivation = false;
        scene2 = SceneManager.LoadSceneAsync(3);
        scene2.allowSceneActivation = false;
        scene3 = SceneManager.LoadSceneAsync(4);
        scene3.allowSceneActivation = false;
        mainScene = SceneManager.LoadSceneAsync("LoadingMainScene");
        mainScene.allowSceneActivation = false;
    }

    public void LoadScene(int number)
    {
        switch (number)
        {
            case 1:
                scene1.allowSceneActivation = true;
                break;
            case 2:
                scene2.allowSceneActivation = true;
                break;
            case 3:
                scene3.allowSceneActivation = true;
                break;
            case 4:
                mainScene.allowSceneActivation = true;
                break;
            default:
                break;
        }
    }

    public void LoadOpeningScene_2()
    {
        Debug.Log("Op2");
        scene1.allowSceneActivation = true;
    }

    public void LoadOpeningScene_3()
    {
        scene2.allowSceneActivation = true;
    }
}
