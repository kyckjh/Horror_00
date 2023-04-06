using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // public -------------------------------------------------------------------------------------

    // �⺻ ������ ---------------------------------------------------------------------------------
    /// <summary>
    /// �� �κ��丮�� ����ϴ� �÷��̾�
    /// </summary>
    Player player;

    /// <summary>
    /// �� Ŭ������ ǥ���Ϸ��� �κ��丮
    /// </summary>
    Inventory inven;

    /// <summary>
    /// ���� ������ �θ� �� ���� ������Ʈ�� Ʈ������
    /// </summary>
    Transform slotParent;

    /// <summary>
    /// �� �κ��丮�� ������ �ִ� ����UI��
    /// </summary>
    [SerializeField]
    ItemSlotUI[] slotUIs;

    [SerializeField]
    Transform shortCutParent;

    [SerializeField]
    ShortCutUI[] shortCutUIs;

    /// <summary>
    /// ���� �ݱ�� ĵ���� �׷�
    /// </summary>
    CanvasGroup canvasGroup;

    PlayerInputActions inputActions;


    // Item����  ----------------------------------------------------------------------------------    
    /// <summary>
    /// �巡�� ���� ǥ�ÿ�( ���� id�� InvalideID�� �巡�� ������ ���� ��)
    /// </summary>
    const uint InvalidID = uint.MaxValue;

    /// <summary>
    /// �巡�װ� ���۵� ������ ID
    /// </summary>
    uint dragStartID = InvalidID;

    /// <summary>
    /// �ӽ� ����(������ �巡�׳� ������ �и��� �� ���)
    /// </summary>
    TempItemSlotUI tempItemSlotUI;
    public TempItemSlotUI TempSlotUI => tempItemSlotUI;

    // �� ���� UI --------------------------------------------------------------------------------
    /// <summary>
    /// ������ ������ â
    /// </summary>
    DetailInfoUI detail;
    public DetailInfoUI Detail => detail;

    // ��������Ʈ ----------------------------------------------------------------------------------
    public Action OnInventoryOpen;
    public Action OnInventoryClose;


    // ����Ƽ �̺�Ʈ �Լ��� -------------------------------------------------------------------------
    private void Awake()
    {
        // �̸� ã�Ƴ���
        canvasGroup = GetComponent<CanvasGroup>();
        slotParent = transform.Find("ItemSlots");
        slotUIs = slotParent.GetComponentsInChildren<ItemSlotUI>();
        tempItemSlotUI = GetComponentInChildren<TempItemSlotUI>();
        detail = GetComponentInChildren<DetailInfoUI>();

        if(shortCutParent == null)
        {
            shortCutParent = transform.parent.Find("ShortCuts");
        }
        shortCutUIs = shortCutParent.GetComponentsInChildren<ShortCutUI>();

        Button closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(Close);

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        //inputActions.ShortCut.Enable();
        //inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        //inputActions.UI.Disable();
        //inputActions.ShortCut.Disable();
    }


    private void Start()
    {
        player = GameManager.Inst.MainPlayer;   // ���� �޴������� �÷��̾� ��������
        //player.OnMoneyChange += RefreshMoney;   // �÷��̾��� Money�� ����Ǵ� ����Ǵ� ��������Ʈ�� �Լ� ���
        //RefreshMoney(player.Money);             // ù ����

        Close();    // ������ �� ������ �ݱ�
    }

    // �Ϲ� �Լ��� ---------------------------------------------------------------------------------

    /// <summary>
    /// �κ��丮�� �Է¹޾� UI�� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="newInven">�� UI�� ǥ���� �κ��丮</param>
    public void InitializeInventory(Inventory newInven)
    {
        inven = newInven;   //��� �Ҵ�

        for (int i = 0; i < inven.SlotCount; i++)
        {
            slotUIs[i].Initialize((uint)i, inven[i]);
        }
        for(int i = 0; i < shortCutUIs.Length; i++)
        {
            shortCutUIs[i].Initialize((uint)i, inven[i]);
        }

        // TempSlot �ʱ�ȭ
        tempItemSlotUI.Initialize(Inventory.TempSlotID, inven.TempSlot);    // TempItemSlotUI�� TempSlot ����
        tempItemSlotUI.Close(); // tempItemSlotUI ����ä�� �����ϱ�
        //inputActions.UI.ItemDrop.canceled += tempItemSlotUI.OnDrop;


        RefreshAllSlots();  // ��ü ����UI ����
    }

    /// <summary>
    /// ��� ������ Icon�̹����� ����
    /// </summary>
    private void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
        {
            slotUI.Refresh();
        }
    }

    /// <summary>
    /// �κ��丮 ���� �ݱ�
    /// </summary>
    public void InventoryOnOffSwitch()
    {
        if (canvasGroup.blocksRaycasts)  // ĵ���� �׷��� blocksRaycasts�� �������� ó��
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    /// <summary>
    /// �κ��丮 ����
    /// </summary>
    void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        OnInventoryOpen?.Invoke();
        UIManager.Inst.SetAimImage(false);     // ȭ�� �߾� ���� ���׶�� �����
        Cursor.lockState = CursorLockMode.Confined; // ���콺 ���̰��ϱ�
    }

    /// <summary>
    /// �κ��丮 �ݱ�
    /// </summary>
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        OnInventoryClose?.Invoke();
        UIManager.Inst.SetAimImage(true);      // ȭ�� �߾� ���� ���׶�� �ٽ� ��Ÿ����
        Cursor.lockState = CursorLockMode.Locked;   // ���콺 �Ⱥ��̰��ϱ�
    }

    // �̺�Ʈ �ý����� �������̽� �Լ��� -------------------------------------------------------------

    /// <summary>
    /// �巡�� �߿� ����(OnBeginDrag, OnEndDrag�� ����Ϸ��� �ݵ�� �ʿ��ؼ� ���� ��)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // OnBeginDrag, OnEndDrag�� ����Ϸ��� �ݵ�� �ʿ��ؼ� ���� ��

        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    tempItemSlotUI.transform.position = eventData.position;
        //}
    }

    /// <summary>
    /// �巡�� ���۽� ����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ���� ���� ó��
        {
            // tempslot�� ����� ��
            if (TempSlotUI.IsEmpty())
            {
                GameObject startObj = eventData.pointerCurrentRaycast.gameObject;   // �巡�� ������ ��ġ�� �ִ� ���� ������Ʈ ��������
                if (startObj != null)
                {
                    // �巡�� ������ ��ġ�� ���� ������Ʈ�� ������
                    ItemSlotUI slotUI = startObj.GetComponent<ItemSlotUI>();    // ItemSlotUI ������Ʈ ��������
                    if (slotUI != null)
                    {
                        // ItemSlotUI ������Ʈ�� ������ ID ����� ����
                        dragStartID = slotUI.ID;
                        inven.TempRemoveItem(dragStartID);  // �巡�� ������ ��ġ�� �������� TempSlot���� �ű�
                        
                        tempItemSlotUI.Open();              // �巡�� ������ �� TempSlot ����
                    }
                }
            }
        }
    }

    /// <summary>
    /// �巡�װ� ������ �� ����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ���� ���� ó��
        {
            if (dragStartID != InvalidID)  // �巡�װ� ���������� ���۵Ǿ��� ���� ó��
            {
                GameObject endObj = eventData.pointerCurrentRaycast.gameObject; // �巡�� ���� ��ġ�� �ִ� ���� ������Ʈ ��������
                if (endObj != null)
                {
                    // �巡�� ���� ��ġ�� ���� ������Ʈ�� ������
                    ItemSlotUI slotUI = endObj.GetComponent<ItemSlotUI>();  // ItemSlotUI ������Ʈ ��������
                    if (slotUI != null && slotUI.ItemSlot != null)  // ItemSlotUI �� �����ϰ� ItemSlotUI �� ItemSlot�� ������ ���� ��(������ ������ ������ �ƴ� ��)
                    {
                        // ItemSlotUI ������Ʈ�� ������ Inventory.MoveItem() �����Ű��
                        // TempSlot�� �������� �巡�װ� ���� ���Կ� �ű��.
                        // ���� �巡�װ� ���� ���Կ� �������� �־��� ��� �� �������� TempSlot�� �̵�
                        inven.MoveItem(Inventory.TempSlotID, slotUI.ID);

                        // �巡�װ� ���� ���Կ� �ִ� �������� dragStartID �������� �ű��
                        inven.MoveItem(Inventory.TempSlotID, dragStartID);

                        dragStartID = InvalidID;                       // �巡�� ���� id�� �� �� ���� ������ ����(�巡�װ� �������� ǥ��)
                    }
                }

                if (tempItemSlotUI.IsEmpty())
                {
                    tempItemSlotUI.Close(); // �巡�׸� ������ tempSlot�� ������� �ݱ�
                }
            }
        }
    }
}
