using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    // ���� ---------------------------------------------------------------------------------------
    // ���Կ� �ִ� ������(ItemData)
    ItemData slotItemData;

    // ������Ƽ ------------------------------------------------------------------------------------

    /// <summary>
    /// ���Կ� �ִ� ������(ItemData)
    /// </summary>
    public ItemData SlotItemData
    {
        get => slotItemData;
        private set
        {
            if (slotItemData != value)
            {
                slotItemData = value;
                onSlotItemChange?.Invoke();  // ������ �Ͼ�� ��������Ʈ ����(�ַ� ȭ�� ���ſ�)
            }
        }
    }


    // ��������Ʈ ----------------------------------------------------------------------------------
    /// <summary>
    /// ���Կ� ����ִ� �������� ������ ������ ����� �� ����Ǵ� ��������Ʈ
    /// </summary>
    public System.Action onSlotItemChange;

    // �Լ� ---------------------------------------------------------------------------------------

    /// <summary>
    /// �����ڵ�
    /// </summary>
    public ItemSlot() { }
    public ItemSlot(ItemData data)
    {
        slotItemData = data;
    }
    public ItemSlot(ItemSlot other)
    {
        slotItemData = other.SlotItemData;
    }

    /// <summary>
    /// ���Կ� �������� �����ϴ� �Լ� 
    /// </summary>
    /// <param name="itemData">���Կ� ������ ItemData</param>
    /// /// <param name="count">���Կ� ������ ������ ����</param>
    public void AssignSlotItem(ItemData itemData)
    {
        SlotItemData = itemData;
    }

    /// <summary>
    /// ������ ���� �Լ�
    /// </summary>
    public void ClearSlotItem()
    {
        SlotItemData = null;
    }


    /*
    /// <summary>
    /// �������� ����ϴ� �Լ�
    /// </summary>
    /// <param name="target">�������� ȿ���� ���� ���(���� �÷��̾�)</param>
    public void UseSlotItem(GameObject target = null)
    {
        IUsable usable = SlotItemData as IUsable;   // �� �������� ��밡���� ���������� Ȯ��
        if (usable != null)
        {
            // �������� ��밡���ϸ�
            usable.Use(target); // ������ ����ϰ�
            DecreaseSlotItem(); // ���� �ϳ� ����
        }
    }
    */

    /*
    /// <summary>
    /// �������� ����ϴ� �Լ�
    /// </summary>
    /// <param name="target">�������� ����ϴ� ���</param>
    public bool EquipSlotItem(GameObject target = null)
    {
        bool result = false;
        IEquipItem equipItem = SlotItemData as IEquipItem;  // �� ������ �������� ��� ������ ���������� Ȯ��
        if (equipItem != null)
        {
            // �������� ��񰡴��ϴ�.

            ItemData_Weapon weaponData = SlotItemData as ItemData_Weapon;   // ������ ������ ���� ����
            IEquipTarget equipTarget = target.GetComponent<IEquipTarget>(); // �������� ����� ����� �������� ����� �� �ִ��� Ȯ��
            if (equipTarget != null)
            {
                // ����� Ư�� ������ �������� ����ϰ� �ִ�. �׸��� �������� ���Ǿ� �ִ�.
                if (equipTarget.EquipItemSlot != null)    // ���⸦ ����ϰ� �մ��� Ȯ��
                {
                    // ���⸦ ����ϰ� �ִ�.

                    if (equipTarget.EquipItemSlot != this)      // ����ϰ� �ִ� �������� ������ Ŭ���ߴ��� Ȯ��
                    {
                        // �ٸ� ������ ����ϰ� �ִ�.
                        equipTarget.UnEquipWeapon();            // �ϴ� ���⸦ ���´�.
                        equipTarget.EquipWeapon(this);    // �ٸ� ���⸦ ����Ѵ�.
                        result = true;
                    }
                    else
                    {
                        equipTarget.UnEquipWeapon();            // ���� ���⸦ ����� ��Ȳ�̸� ���⸸ �Ѵ�.
                    }
                }
                else
                {
                    // ���⸦ ����ϰ� ���� �ʴ�. => �׳� ���
                    equipTarget.EquipWeapon(this);
                    result = true;
                }
            }
        }
        return result;
    }
    */

    // �Լ�(�鿣��) --------------------------------------------------------------------------------

    /// <summary>
    /// ������ ������� Ȯ�����ִ� �Լ�
    /// </summary>
    /// <returns>true�� ����ִ� �Լ�</returns>
    public bool IsEmpty()
    {
        return slotItemData == null;
    }
}