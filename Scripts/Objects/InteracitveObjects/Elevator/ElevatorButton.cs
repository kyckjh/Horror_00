using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour, IUsableObject
{
    ElevatorManager manager;

    Renderer buttonRenderer;

    [SerializeField]
    Material buttonActivate, buttonInactivate;

    public Action onButtonClick;    // 버튼을 눌렀을 때 실행되는 델리게이트

    private void Awake()
    {
        buttonRenderer = GetComponent<Renderer>();
        manager = GetComponentInParent<ElevatorManager>();
    }

    private void Start()
    {
        ButtonOff();
    }

    public bool Use(ItemData _)
    {
        if (manager.isActivate)
        {
            onButtonClick?.Invoke();
            ButtonOn();
        }
        else
        {
            UIManager.Inst.SetMessagePanel("엘리베이터 전원이 꺼져있습니다");
        }
        return false;
    }

    public void ButtonOn()
    {
        buttonRenderer.material = buttonActivate;
    }

    public void ButtonOff()
    {
        buttonRenderer.material = buttonInactivate;
    }
}
