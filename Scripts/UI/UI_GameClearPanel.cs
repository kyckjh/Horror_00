using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_GameClearPanel : MonoBehaviour
{
    Button titleButton, exitButton;
    TextMeshProUGUI clearTimeText;
    

    private void Awake()
    {
        titleButton = transform.GetChild(1).GetComponent<Button>();
        exitButton = transform.GetChild(2).GetComponent<Button>();
        clearTimeText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        titleButton.onClick.AddListener(() =>
        {
            Destroy(UIManager.Inst);
            Destroy(GameManager.Inst);
            SceneManager.LoadScene("LoadingOpening");
        });
        exitButton.onClick.AddListener(Application.Quit);
    }

    private void OnEnable()
    {
        float cTime = GameManager.Inst.clearTime;
        clearTimeText.text = $"{Mathf.FloorToInt(cTime / 60):D2}:{Mathf.FloorToInt(cTime):D2}:{Mathf.FloorToInt(cTime * 100) % 100:D2}";
        UIManager.Inst.EnableMainCanvas();
    }
}
