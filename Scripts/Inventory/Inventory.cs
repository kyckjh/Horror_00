using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    // ���� ---------------------------------------------------------------------------------------    

    /// <summary>
    /// �κ��丮�� ������ �� ������ ĭ.
    /// </summary>
    ItemSlot[] slots = null;

    /// <summary>
    /// �������� �ű�ų� ����� ����� �ӽ� ����.
    /// </summary>
    ItemSlot tempSlot = null;

    // ��� ---------------------------------------------------------------------------------------

    /// <summary>
    /// �κ��丮 �⺻ ũ��
    /// </summary>
    public const int Default_Inventory_Size = 6;

    /// <summary>
    /// TempSlot�� ���̵�
    /// </summary>
    public const uint TempSlotID = 99999;   //���ڴ� �ǹ̰� ����. Slot Index�� �������� ���� ���̸� �ȴ�.

    // ������Ƽ  -----------------------------------------------------------------------------------

    /// <summary>
    /// �κ��丮�� ũ��
    /// </summary>
    public int SlotCount => slots.Length;

    /// <summary>
    /// �ӽ� ����(�б�����)
    /// </summary>
    public ItemSlot TempSlot => tempSlot;

    /// <summary>
    /// �ε���. �κ��丮���� ���� ��������
    /// </summary>
    /// <param name="index">������ ������ �ε���</param>
    /// <returns>index��°�� ������ ����</returns>
    public ItemSlot this[int index] => slots[index];


    // �Լ�(�ֿ���) ------------------------------------------------------------------------------    

    /// <summary>
    /// �κ��丮 ������
    /// </summary>
    /// <param name="size">�κ��丮�� ���� ��. �⺻ ������ Default_Inventory_Size(6) ��� </param>
    public Inventory(int size = Default_Inventory_Size)
    {
        slots = new ItemSlot[size];     // �Է¹��� ������ ���Ը����
        for (int i = 0; i < size; i++)
        {
            slots[i] = new ItemSlot();
        }
        tempSlot = new ItemSlot();      // �ӽ� �뵵�� ����ϴ� ������ ���� ����
    }

    // AddItem�� �Լ� �����ε�(Overloading)�� ���� �̸��� ���ϰ��� ������ �پ��� ������ �Ķ���͸� �Է� ���� �� �ְ� �ߴ�.
    /// <summary>
    /// ������ �߰��ϱ� (������ ��ĭ�� �ֱ�)
    /// </summary>
    /// <param name="id">�߰��� �������� ���̵�</param>
    /// <returns>������ �߰� ���� ����(true�� �κ��丮�� �������� �߰���)</returns>
    public bool AddItem(uint id)
    {
        return AddItem(GameManager.Inst.ItemData[id]);
    }

    /// <summary>
    /// ������ �߰��ϱ� (������ ��ĭ�� �ֱ�)
    /// </summary>
    /// <param name="code">�߰��� �������� �ڵ�</param>
    /// <returns>������ �߰� ���� ����(true�� �κ��丮�� �������� �߰���)</returns>
    public bool AddItem(ItemIDCode code)
    {
        return AddItem(GameManager.Inst.ItemData[code]);
    }

    /// <summary>
    /// ������ �߰��ϱ� (������ ��ĭ�� �ֱ�)
    /// </summary>
    /// <param name="data">�߰��� �������� ������ ������</param>
    /// <returns>������ �߰� ���� ����(true�� �κ��丮�� �������� �߰���)</returns>
    public bool AddItem(ItemData data)
    {
        bool result = false;
        
        ItemSlot empty = FindEmptySlot();    // ������ �� ���� ã��
        if (empty != null)
        {
            empty.AssignSlotItem(data);      // ������ �Ҵ�
            result = true;
            //Debug.Log($"������ ���Կ� {data.itemName}�� �Ҵ��մϴ�.");
        }
        else
        {
            // ��� ���Կ� �������� ����ִ�.(�κ��丮�� ����á��.)
            //Debug.Log($"���� : �κ��丮�� ����á���ϴ�.");
        }
        

        return result;
    }

    /// <summary>
    /// ������ �߰��ϱ� (Ư���� ���Կ� �ֱ�)
    /// </summary>
    /// <param name="id">�߰��� �������� ���̵�</param>
    /// <param name="index">�������� �߰��� ������ �ε���</param>
    /// <returns>�������� �߰��ϴµ� �����ϸ� true. �ƴϸ� false</returns>
    public bool AddItem(uint id, uint index)
    {
        return AddItem(GameManager.Inst.ItemData[id], index);
    }

    /// <summary>
    /// ������ �߰��ϱ� (Ư���� ���Կ� �ֱ�)
    /// </summary>
    /// <param name="code">�߰��� �������� �������ڵ�</param>
    /// <param name="index">�������� �߰��� ������ �ε���</param>
    /// <returns>�������� �߰��ϴµ� �����ϸ� true. �ƴϸ� false</returns>
    public bool AddItem(ItemIDCode code, uint index)
    {
        return AddItem(GameManager.Inst.ItemData[code], index);
    }

    /// <summary>
    /// ������ �߰��ϱ� (Ư���� ���Կ� �ֱ�)
    /// </summary>
    /// <param name="data">�߰��� �������� ������ ������</param>
    /// <param name="index">�������� �߰��� ������ �ε���</param>
    /// <returns>�������� �߰��ϴµ� �����ϸ� true. �ƴϸ� false</returns>
    public bool AddItem(ItemData data, uint index)
    {
        bool result = false;

        //Debug.Log($"�κ��丮�� {index} ���Կ�  {data.itemName}�� �߰��մϴ�");
        ItemSlot slot = slots[index];   // index��°�� ���� ��������

        if (slot.IsEmpty())              // ã�� ������ ������� Ȯ��
        {
            slot.AssignSlotItem(data);  // ��������� ������ �߰�
            result = true;
            //Debug.Log($"�߰��� �����߽��ϴ�.");
        }
        else
        {
            Debug.Log($"���� : ������ ���� á���ϴ�.");
            /*
            if (slot.SlotItemData == data)  // ���� ������ �������ΰ�?
            {
                if (slot.IncreaseSlotItem() == 0)  // �� �ڸ��� �ִ°�?
                {
                    result = true;
                    //Debug.Log($"������ ���� ������ �����߽��ϴ�.");
                }
                else
                {
                    //Debug.Log($"���� : ������ ���� á���ϴ�.");
                }
            }
            else
            {
                //Debug.Log($"���� : {index} ���Կ��� �ٸ� �������� ����ֽ��ϴ�.");
            }
            */
        }
        return result;
    }

    /// <summary>
    /// Ư�� ������ �������� ��� ������ �Լ�
    /// </summary>
    /// <param name="slotIndex">�������� ���� ������ �ε���</param>
    /// <returns>�����µ� �����ϸ� true, �ƴϸ� false</returns>
    public bool ClearItem(uint slotIndex)
    {
        bool result = false;

        //Debug.Log($"�κ��丮���� {slotIndex} ������ ���ϴ�.");
        if (IsValidSlotIndex(slotIndex))        // slotIndex�� ������ �������� Ȯ��
        {
            ItemSlot slot = slots[slotIndex];
            //Debug.Log($"{slot.SlotItemData.itemName}�� �����մϴ�.");
            slot.ClearSlotItem();               // ������ �����̸� ���� ó��
            //Debug.Log($"������ �����߽��ϴ�.");
            result = true;
        }
        else
        {
            //Debug.Log($"���� : �߸��� �ε����Դϴ�.");
        }

        return result;
    }

    /// <summary>
    /// ��� ������ ������ ���� �Լ�
    /// </summary>
    public void ClearInventory()
    {
        Debug.Log("�κ��丮 Ŭ����");
        foreach (var slot in slots)
        {
            slot.ClearSlotItem();   // ��ü ���Ե��� ���鼭 �ϳ��� ����
        }
    }

    /// <summary>
    /// ������ �̵���Ű��
    /// </summary>
    /// <param name="from">���� ������ ID</param>
    /// <param name="to">���� ������ ID</param>
    public void MoveItem(uint from, uint to)
    {
        // from, to ���� ���� �߻� ������ 4���� ����� ��
        // from�� �ְ� to�� �ְ�
        // from�� �ְ� to�� ����
        // from�� ���� to�� �ְ� -> ���� ����Ǹ� �ȵȴ�.
        // from�� ���� to�� ���� -> ���� ����Ǹ� �ȵȴ�.

        // from�� �븮���� ���� �ε����� ������ ������� �ʴ�. �׸��� to�� �븮���� ���� �ε�����.
        if (IsValidAndNotEmptySlot(from) && IsValidSlotIndex(to))
        {
            ItemSlot fromSlot = null;
            ItemSlot toSlot = null;

            // �ε����� ���� ã��
            if (from == TempSlotID)
            {
                fromSlot = TempSlot;    // temp������ ������ �ε��� Ȯ��
            }
            else
            {
                fromSlot = slots[from]; // �ٸ� ������ �ε����� �״�� Ȱ��
            }
            if (to == TempSlotID)
            {
                toSlot = TempSlot;      // temp������ ������ �ε��� Ȯ��
            }
            else
            {
                toSlot = slots[to];     // �ٸ� ������ �ε����� �״�� Ȱ��
            }

            //�����۰� ������ ������ ���� �����Ѵ�.
            ItemData tempItemData = toSlot.SlotItemData;    // �ӽ� ����
            toSlot.AssignSlotItem(fromSlot.SlotItemData);   // to���� from�� ���� �ֱ�
            fromSlot.AssignSlotItem(tempItemData);               // from���ٰ� �ӽ÷� ������ to�� ���� �ֱ�                                
            
        }
    }

    
    /// <summary>
    /// �������� ������ �ӽ� ���Կ� ����
    /// </summary>
    /// <param name="from">�������� ���� ����</param>
    /// <param name="count">���� ������ ����</param>
    public void TempRemoveItem(uint from)
    {
        if (IsValidAndNotEmptySlot(from))  // from�� ������ �����̸�
        {
            ItemSlot slot = slots[from];
            tempSlot.AssignSlotItem(slot.SlotItemData);  // temp ���Կ� ������ ������ ������ �Ҵ�
            slot.ClearSlotItem();   // �� ���Կ� �ִ� �������� ����
        }
    }
    

    // ������ ����ϱ�
    // ������ ����ϱ�
    // ������ ����

    // �Լ�(�鿣��) --------------------------------------------------------------------------------

    /// <summary>
    /// �� ������ ã���ִ� �Լ�
    /// </summary>
    /// <returns>�� ����</returns>
    private ItemSlot FindEmptySlot()
    {
        ItemSlot result = null;

        foreach (var slot in slots)  // slots�� ���� ��ȸ�ϸ鼭
        {
            if (slot.IsEmpty())     // �� �������� Ȯ��
            {
                result = slot;      // �� �����̸� foreach break�ϰ� ����
                break;
            }
        }

        return result;
    }    

    /// <summary>
    /// index���� ������ �������� Ȯ�����ִ� �Լ�
    /// </summary>
    /// <param name="index">Ȯ���� �ε���</param>
    /// <returns>true�� ������ ����. �ƴϸ� false</returns>
    private bool IsValidSlotIndex(uint index) => (index < SlotCount) || (index == TempSlotID);
    //{
    //    return index < SlotCount;
    //}

    /// <summary>
    /// index���� ������ �����̸鼭 �������� ����ִ����� Ȯ�����ִ� �Լ�
    /// </summary>
    /// <param name="index">Ȯ���� �ε���</param>
    /// <returns>true�� ������ �����̸鼭 �����۵� �������.</returns>
    private bool IsValidAndNotEmptySlot(uint index)
    {
        ItemSlot testSlot = null;
        if (index != TempSlotID)
        {
            testSlot = slots[index];    // index�� tempSlot�� �ƴϸ� �ε����� ã��
        }
        else
        {
            testSlot = TempSlot;    // index�� tempSlot�� ��� TempSlot ����
        }

        return (IsValidSlotIndex(index) && !testSlot.IsEmpty());
    }

    /*
    /// <summary>
    /// �κ��丮 ������ �ܼ�â�� ������ִ� �Լ�
    /// </summary>
    public void PrintInventory()
    {
        // ���� �κ��丮 ������ �ܼ�â�� ����ϴ� �Լ�
        // ex) [�ް�,�ް�,�ް�,(��ĭ),���ٱ�,���ٱ�]

        string printText = "[";
        for (int i = 0; i < SlotCount - 1; i++)         // ������ ��ü6���� ��� 0~4������ �ϴ� �߰�(5���߰�)
        {
            if (slots[i].SlotItemData != null)
            {
                printText += $"{slots[i].SlotItemData.itemName}({slots[i].ItemCount})";
            }
            else
            {
                printText += "(��ĭ)";
            }
            printText += ",";
        }
        ItemSlot slot = slots[SlotCount - 1];   // ������ ���Ը� ���� ó��
        if (!slot.IsEmpty())
        {
            printText += $"{slot.SlotItemData.itemName}({slot.ItemCount})]";
        }
        else
        {
            printText += "(��ĭ)]";
        }

        //string.Join(',', ���ڿ� �迭);
        Debug.Log(printText);
    }
    */
}