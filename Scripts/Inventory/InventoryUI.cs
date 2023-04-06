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

    // 기본 데이터 ---------------------------------------------------------------------------------
    /// <summary>
    /// 이 인벤토리를 사용하는 플레이어
    /// </summary>
    Player player;

    /// <summary>
    /// 이 클래스로 표현하려는 인벤토리
    /// </summary>
    Inventory inven;

    /// <summary>
    /// 슬롯 생성시 부모가 될 게임 오브젝트의 트랜스폼
    /// </summary>
    Transform slotParent;

    /// <summary>
    /// 이 인벤토리가 가지고 있는 슬롯UI들
    /// </summary>
    [SerializeField]
    ItemSlotUI[] slotUIs;

    [SerializeField]
    Transform shortCutParent;

    [SerializeField]
    ShortCutUI[] shortCutUIs;

    /// <summary>
    /// 열고 닫기용 캔버스 그룹
    /// </summary>
    CanvasGroup canvasGroup;

    PlayerInputActions inputActions;


    // Item관련  ----------------------------------------------------------------------------------    
    /// <summary>
    /// 드래그 시작 표시용( 시작 id가 InvalideID면 드래그 시작을 안한 것)
    /// </summary>
    const uint InvalidID = uint.MaxValue;

    /// <summary>
    /// 드래그가 시작된 슬롯의 ID
    /// </summary>
    uint dragStartID = InvalidID;

    /// <summary>
    /// 임시 슬롯(아이템 드래그나 아이템 분리할 때 사용)
    /// </summary>
    TempItemSlotUI tempItemSlotUI;
    public TempItemSlotUI TempSlotUI => tempItemSlotUI;

    // 상세 정보 UI --------------------------------------------------------------------------------
    /// <summary>
    /// 아이템 상세정보 창
    /// </summary>
    DetailInfoUI detail;
    public DetailInfoUI Detail => detail;

    // 델리게이트 ----------------------------------------------------------------------------------
    public Action OnInventoryOpen;
    public Action OnInventoryClose;


    // 유니티 이벤트 함수들 -------------------------------------------------------------------------
    private void Awake()
    {
        // 미리 찾아놓기
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
        player = GameManager.Inst.MainPlayer;   // 게임 메니저에서 플레이어 가져오기
        //player.OnMoneyChange += RefreshMoney;   // 플레이어의 Money가 변경되는 실행되는 델리게이트에 함수 등록
        //RefreshMoney(player.Money);             // 첫 갱신

        Close();    // 시작할 때 무조건 닫기
    }

    // 일반 함수들 ---------------------------------------------------------------------------------

    /// <summary>
    /// 인벤토리를 입력받아 UI를 초기화하는 함수
    /// </summary>
    /// <param name="newInven">이 UI로 표시할 인벤토리</param>
    public void InitializeInventory(Inventory newInven)
    {
        inven = newInven;   //즉시 할당

        for (int i = 0; i < inven.SlotCount; i++)
        {
            slotUIs[i].Initialize((uint)i, inven[i]);
        }
        for(int i = 0; i < shortCutUIs.Length; i++)
        {
            shortCutUIs[i].Initialize((uint)i, inven[i]);
        }

        // TempSlot 초기화
        tempItemSlotUI.Initialize(Inventory.TempSlotID, inven.TempSlot);    // TempItemSlotUI와 TempSlot 연결
        tempItemSlotUI.Close(); // tempItemSlotUI 닫은채로 시작하기
        //inputActions.UI.ItemDrop.canceled += tempItemSlotUI.OnDrop;


        RefreshAllSlots();  // 전체 슬롯UI 갱신
    }

    /// <summary>
    /// 모든 슬롯의 Icon이미지를 갱신
    /// </summary>
    private void RefreshAllSlots()
    {
        foreach (var slotUI in slotUIs)
        {
            slotUI.Refresh();
        }
    }

    /// <summary>
    /// 인벤토리 열고 닫기
    /// </summary>
    public void InventoryOnOffSwitch()
    {
        if (canvasGroup.blocksRaycasts)  // 캔버스 그룹의 blocksRaycasts를 기준으로 처리
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    /// <summary>
    /// 인벤토리 열기
    /// </summary>
    void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        OnInventoryOpen?.Invoke();
        UIManager.Inst.SetAimImage(false);     // 화면 중앙 에임 동그라미 지우기
        Cursor.lockState = CursorLockMode.Confined; // 마우스 보이게하기
    }

    /// <summary>
    /// 인벤토리 닫기
    /// </summary>
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        OnInventoryClose?.Invoke();
        UIManager.Inst.SetAimImage(true);      // 화면 중앙 에임 동그라미 다시 나타내기
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 안보이게하기
    }

    // 이벤트 시스템의 인터페이스 함수들 -------------------------------------------------------------

    /// <summary>
    /// 드래그 중에 실행(OnBeginDrag, OnEndDrag를 사용하려면 반드시 필요해서 넣은 것)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // OnBeginDrag, OnEndDrag를 사용하려면 반드시 필요해서 넣은 것

        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    tempItemSlotUI.transform.position = eventData.position;
        //}
    }

    /// <summary>
    /// 드래그 시작시 실행
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭일 때만 처리
        {
            // tempslot이 비었을 때
            if (TempSlotUI.IsEmpty())
            {
                GameObject startObj = eventData.pointerCurrentRaycast.gameObject;   // 드래그 시작한 위치에 있는 게임 오브젝트 가져오기
                if (startObj != null)
                {
                    // 드래그 시작한 위치에 게임 오브젝트가 있으면
                    ItemSlotUI slotUI = startObj.GetComponent<ItemSlotUI>();    // ItemSlotUI 컴포넌트 가져오기
                    if (slotUI != null)
                    {
                        // ItemSlotUI 컴포넌트가 있으면 ID 기록해 놓기
                        dragStartID = slotUI.ID;
                        inven.TempRemoveItem(dragStartID);  // 드래그 시작한 위치의 아이템을 TempSlot으로 옮김
                        
                        tempItemSlotUI.Open();              // 드래그 시작할 때 TempSlot 열기
                    }
                }
            }
        }
    }

    /// <summary>
    /// 드래그가 끝났을 때 실행
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭일 때만 처리
        {
            if (dragStartID != InvalidID)  // 드래그가 정상적으로 시작되었을 때만 처리
            {
                GameObject endObj = eventData.pointerCurrentRaycast.gameObject; // 드래그 끝난 위치에 있는 게임 오브젝트 가져오기
                if (endObj != null)
                {
                    // 드래그 끝난 위치에 게임 오브젝트가 있으면
                    ItemSlotUI slotUI = endObj.GetComponent<ItemSlotUI>();  // ItemSlotUI 컴포넌트 가져오기
                    if (slotUI != null && slotUI.ItemSlot != null)  // ItemSlotUI 가 존재하고 ItemSlotUI 가 ItemSlot을 가지고 있을 때(빨간색 아이템 슬롯이 아닐 때)
                    {
                        // ItemSlotUI 컴포넌트가 있으면 Inventory.MoveItem() 실행시키기
                        // TempSlot의 아이템을 드래그가 끝난 슬롯에 옮기기.
                        // 만약 드래그가 끝난 슬롯에 아이템이 있었을 경우 그 아이템은 TempSlot로 이동
                        inven.MoveItem(Inventory.TempSlotID, slotUI.ID);

                        // 드래그가 끝난 슬롯에 있던 아이템을 dragStartID 슬롯으로 옮기기
                        inven.MoveItem(Inventory.TempSlotID, dragStartID);

                        dragStartID = InvalidID;                       // 드래그 시작 id를 될 수 없는 값으로 설정(드래그가 끝났음을 표시)
                    }
                }

                if (tempItemSlotUI.IsEmpty())
                {
                    tempItemSlotUI.Close(); // 드래그를 끝내고 tempSlot이 비어지면 닫기
                }
            }
        }
    }
}
