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
    /// 슬롯위에 마우스 포인터가 들어왔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    /// <summary>
    /// 슬롯위에서 마우스 포인터가 나갔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }


    /// <summary>
    /// 슬롯위에서 마우스 포인터가 움직일 때
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerMove(PointerEventData eventData)
    {

    }


    /// <summary>
    /// 슬롯을 마우스로 클릭했을 때
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerClick(PointerEventData eventData)
    {
        menuSlot.UnselectAllSlots();    // 모든 슬롯 선택해제 상태로 전환
        base.OnPointerClick(eventData);
    }
}
