using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_GameOverPanel : MonoBehaviour
{
    CanvasGroup group;
    Button restartButton, exitButton;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        restartButton = transform.GetChild(1).GetComponent<Button>();
        exitButton = transform.GetChild(2).GetComponent<Button>();
        restartButton.onClick.AddListener(() =>
        {
            Destroy(UIManager.Inst);
            Destroy(GameManager.Inst);
            SceneManager.LoadScene("LoadingMainScene");
        });
        exitButton.onClick.AddListener(Application.Quit);
    }

    private void OnEnable()
    {
        group.alpha = 0.0f;
        group.interactable = true;
        StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        group.alpha = 0.0f;
        group.interactable = false;
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(2.0f);
        while(true)
        {
            group.alpha += Time.deltaTime;
            if (group.alpha >= 1.0f)
                break;
            yield return null;
        }
        yield return null;
    }
}
