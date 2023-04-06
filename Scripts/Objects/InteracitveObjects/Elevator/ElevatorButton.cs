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

    public Action onButtonClick;    // ��ư�� ������ �� ����Ǵ� ��������Ʈ

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
            UIManager.Inst.SetMessagePanel("���������� ������ �����ֽ��ϴ�");
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
