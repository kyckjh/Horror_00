using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlot : MonoBehaviour
{
    MenuSlotUI[] slots;

    private void OnEnable()
    {
        slots[0].IsSelected = true; // 첫 번째 메뉴슬롯(graphics) 활성화
    }

    private void OnDisable()
    {
        UnselectAllSlots();
    }

    private void Awake()
    {
        slots = GetComponentsInChildren<MenuSlotUI>();
    }

    /// <summary>
    /// 모든 메뉴 슬롯을 선택해제 상태로 전환하는 함수
    /// </summary>
    public void UnselectAllSlots()
    {
        foreach(var slot in slots)
        {
            slot.IsSelected = false;
        }
    }
}
