using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    // 기본 데이터 ---------------------------------------------------------------------------------
    /// <summary>
    /// 아이템 슬롯 아이디
    /// </summary>
    uint id;

    /// <summary>
    /// 이 슬롯UI에서 가지고 있을 ItemSlot(inventory클래스가 가지고 있는 ItemSlot중 하나)
    /// </summary>
    protected ItemSlot itemSlot;

    // 주요 인벤토리 UI 가지고 있기 -----------------------------------------------------------------

    /// <summary>
    /// 인벤토리 UI
    /// </summary>
    InventoryUI invenUI;

    /// <summary>
    /// 상세 정보창
    /// </summary>
    DetailInfoUI detailUI;

    // UI처리용 데이터 -----------------------------------------------------------------------------
    
    /// <summary>
    /// 아이템의 Icon을 표시할 이미지 컴포넌트
    /// </summary>
    protected Image itemImage;

    protected Image backgroundImage;    // 커서 위에 올렸을 때 불투명해질 이미지

    Color backgroundColor = new Color(1, 1, 1, 0.2f);




    // 프로퍼티들 ----------------------------------------------------------------------------------

    /// <summary>
    /// 아이템 슬롯 아이디(읽기 전용)
    /// </summary>
    public uint ID { get => id; }

    /// <summary>
    /// 이 슬롯UI에서 가지고 있을 ItemSlot(읽기 전용)
    /// </summary>
    public ItemSlot ItemSlot { get => itemSlot; }

    // 함수들 --------------------------------------------------------------------------------------
    protected virtual void Awake()  // 오버라이드 가능하도록 virtual 추가
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();    // 아이템 표시용 이미지 컴포넌트 찾아놓기
        backgroundImage = transform.GetChild(1).GetComponent<Image>();    // 아이템 표시용 이미지 컴포넌트 찾아놓기
        backgroundImage.color = Color.clear;
    }

    /// <summary>
    /// ItemSlotUI의 초기화 작업
    /// </summary>
    /// <param name="newID">이 슬롯의 ID</param>
    /// <param name="targetSlot">이 슬롯이랑 연결된 ItemSlot</param>
    public void Initialize(uint newID, ItemSlot targetSlot)
    {
        invenUI = GameManager.Inst.InvenUI; // 미리 찾아놓기
        detailUI = invenUI.Detail;

        id = newID;
        itemSlot = targetSlot;
        itemSlot.onSlotItemChange += Refresh; // ItemSlot에 아이템이 변경될 경우 실행될 델리게이트에 함수 등록        
    }

    /// <summary>
    /// 슬롯에서 표시되는 아이콘 이미지 갱신용 함수
    /// </summary>
    public void Refresh()
    {
        if (itemSlot == null)   // 아이템 슬롯이 없을 때
        {
            itemImage.sprite = backgroundImage.sprite;
            itemImage.color = new Color(1, 0, 0, 0.2f);     // 빨간색으로 만들기
        }
        else if( itemSlot.SlotItemData != null )
        {
            // 이 슬롯에 아이템이 들어있을 때
            itemImage.sprite = itemSlot.SlotItemData.itemIcon;  // 아이콘 이미지 설정하고
            itemImage.color = Color.white;  // 불투명하게 만들기            
        }
        else
        {
            // 이 슬롯에 아이템이 없을 때
            itemImage.sprite = null;        // 아이콘 이미지 제거하고
            itemImage.color = Color.clear;  // 투명하게 만들기
            GameManager.Inst.MainPlayer.MySlotID = id;
        }
    }

    /// <summary>
    /// 슬롯위에 마우스 포인터가 들어왔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSlot != null && itemSlot.SlotItemData != null)  // 슬롯에 아이템이 있으면
        {
            backgroundImage.color = backgroundColor;    // 슬롯이 살짝 불투명해짐
        }
    }

    /// <summary>
    /// 슬롯위에서 마우스 포인터가 나갔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemSlot != null)
        {
            backgroundImage.color = Color.clear;    // 슬롯에 불투명한게 사라짐
        }
    }

    
    /// <summary>
    /// 슬롯위에서 마우스 포인터가 움직일 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerMove(PointerEventData eventData)
    {

    }


    /// <summary>
    /// 슬롯을 마우스로 클릭했을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemSlot != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                TempItemSlotUI temp = invenUI.TempSlotUI;

                if (!temp.IsEmpty())  // temp에 ItemSlot이 들어있다 => 아이템을 덜어낸 상황이다.                
                {
                    // 들고 있던 임시 아이템을 슬롯에 넣기                
                    if (ItemSlot.IsEmpty())
                    {
                        // 클릭한 슬롯이 빈칸이다.

                        // temp에 있는 내용을 이 슬롯에 다 넣기
                        itemSlot.AssignSlotItem(temp.ItemSlot.SlotItemData);
                        temp.Close();   // temp칸 비우기
                    }
                    else
                    {
                        // 클릭한 슬롯이 빈칸이 아니다.

                        // 다른 종류의 아이템이다. => 서로 스왑
                        ItemData tempData = temp.ItemSlot.SlotItemData;
                        temp.ItemSlot.AssignSlotItem(itemSlot.SlotItemData);
                        itemSlot.AssignSlotItem(tempData);
                    }
                    //detailUI.IsPause = false;   // 상세정보창 일시정지 풀기
                }
                else
                {
                    // 그냥 클릭한 상황
                    if (!ItemSlot.IsEmpty())
                    {
                        // 이 슬롯의 아이템 데이터와 상세정보창의 아이템 데이터가 다르면
                        // (상세정보창이 닫혀있는 경우도 포함(data가 null))
                        if (itemSlot.SlotItemData != detailUI.ItemData)
                        {
                            detailUI.Open(itemSlot.SlotItemData, ID);   // 이 슬롯의 데이터 상세정보창 열기
                        }
                        else
                        {
                            // 이 슬롯의 아이템 데이터와 상세정보창의 아이템 데이터가 같으면
                            detailUI.Close();
                        }
                    }
                }
            }
        }
    }
}
