using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortCutUI : MonoBehaviour
{
    /// <summary>
    /// ������ ���� ���̵�
    /// </summary>
    uint id;

    /// <summary>
    /// �� ����UI���� ������ ���� ItemSlot(inventoryŬ������ ������ �ִ� ItemSlot�� �ϳ�)
    /// </summary>
    ItemSlot itemSlot;

    /// <summary>
    /// �������� Icon�� ǥ���� �̹��� ������Ʈ
    /// </summary>
    protected Image itemImage;

    protected Image backgroundImage;    // Ŀ�� ���� �÷��� �� ���������� �̹���

    Color backgroundColor = new Color(1, 1, 1, 0.2f);

    /// <summary>
    /// �� ����UI���� ������ ���� ItemSlot(�б� ����)
    /// </summary>
    public ItemSlot ItemSlot { get => itemSlot; }

    private void Awake()
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();    // ������ ǥ�ÿ� �̹��� ������Ʈ ã�Ƴ���
        backgroundImage = transform.GetChild(1).GetComponent<Image>();    // ������ ǥ�ÿ� �̹��� ������Ʈ ã�Ƴ���
        backgroundImage.color = Color.clear;
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// ShorCutUI�� �ʱ�ȭ �۾�
    /// </summary>
    /// <param name="targetSlot">�� �����̶� ����� ItemSlot</param>
    public void Initialize(uint newID, ItemSlot targetSlot)
    {
        id = newID;
        itemSlot = targetSlot;
        itemSlot.onSlotItemChange += Refresh; // ItemSlot�� �������� ����� ��� ����� ��������Ʈ�� �Լ� ���        
    }

    /// <summary>
    /// ���Կ��� ǥ�õǴ� ������ �̹��� ���ſ� �Լ�
    /// </summary>
    public void Refresh()
    {
        if (itemSlot.SlotItemData != null)
        {
            // �� ���Կ� �������� ������� ��
            itemImage.sprite = itemSlot.SlotItemData.itemIcon;  // ������ �̹��� �����ϰ�
            itemImage.color = Color.white;  // �������ϰ� �����            
        }
        else
        {
            // �� ���Կ� �������� ���� ��
            itemImage.sprite = null;        // ������ �̹��� �����ϰ�
            itemImage.color = Color.clear;  // �����ϰ� �����
            GameManager.Inst.MainPlayer.MySlotID = id;
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="_tf">true�� �������ϰ�, false�� �����ϰ�</param>
    public void Select(bool _tf)
    {
        if(_tf)
        {
            backgroundImage.color = backgroundColor;
        }
        else
        {
            backgroundImage.color = Color.clear;
        }
    }
}
