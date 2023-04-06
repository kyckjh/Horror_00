using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Sensitivity : MonoBehaviour
{
    TMP_InputField field;
    Scrollbar bar;
    Player player;

    private void Awake()
    {
        field = GetComponentInChildren<TMP_InputField>();
        bar = GetComponentInChildren<Scrollbar>();
        player = GameManager.Inst.MainPlayer;
    }

    private void Start()
    {
        field.text = $"{player.TurnSpeed:F1}";
        bar.value = player.TurnSpeed / 100;
        field.onEndEdit.AddListener(OnFieldInput);
        bar.onValueChanged.AddListener(OnScrollInput);
    }

    void OnFieldInput(string val)
    {
        if(float.TryParse(val, out float value))
        {
            player.TurnSpeed = value;
        }
        field.text = $"{player.TurnSpeed:F1}";
        bar.value = player.TurnSpeed / 100;
    }

    void OnScrollInput(float val)
    {
        player.TurnSpeed = val * 100;
        field.text = $"{player.TurnSpeed:F1}";
    }
}
