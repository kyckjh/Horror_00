using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuSlotUI : UIBase
{
    MenuSlot menuSlot;
        
    public GameObject settingPanel;

    protected override void OnSelect()
    {
        base.OnSelect();
        settingPanel.SetActive(true);
    }

    protected override void OnUnselect()
    {
        base.OnUnselect();
        settingPanel.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        menuSlot = transform.parent.GetComponent<MenuSlot>();
    }

    /// <summary>
    /// �������� ���콺 �����Ͱ� ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    /// <summary>
    /// ���������� ���콺 �����Ͱ� ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }


    /// <summary>
    /// ���������� ���콺 �����Ͱ� ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerMove(PointerEventData eventData)
    {

    }


    /// <summary>
    /// ������ ���콺�� Ŭ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        menuSlot.UnselectAllSlots();    // ��� ���� �������� ���·� ��ȯ
        base.OnPointerClick(eventData);
    }
}
