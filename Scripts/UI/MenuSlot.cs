using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSlot : MonoBehaviour
{
    MenuSlotUI[] slots;

    private void OnEnable()
    {
        slots[0].IsSelected = true; // ù ��° �޴�����(graphics) Ȱ��ȭ
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
    /// ��� �޴� ������ �������� ���·� ��ȯ�ϴ� �Լ�
    /// </summary>
    public void UnselectAllSlots()
    {
        foreach(var slot in slots)
        {
            slot.IsSelected = false;
        }
    }
}
