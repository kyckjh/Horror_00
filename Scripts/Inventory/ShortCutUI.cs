using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortCutUI : MonoBehaviour
{
    /// <summary>
    /// 아이템 슬롯 아이디
    /// </summary>
    uint id;

    /// <summary>
    /// 이 슬롯UI에서 가지고 있을 ItemSlot(inventory클래스가 가지고 있는 ItemSlot중 하나)
    /// </summary>
    ItemSlot itemSlot;

    /// <summary>
    /// 아이템의 Icon을 표시할 이미지 컴포넌트
    /// </summary>
    protected Image itemImage;

    protected Image backgroundImage;    // 커서 위에 올렸을 때 불투명해질 이미지

    Color backgroundColor = new Color(1, 1, 1, 0.2f);

    /// <summary>
    /// 이 슬롯UI에서 가지고 있을 ItemSlot(읽기 전용)
    /// </summary>
    public ItemSlot ItemSlot { get => itemSlot; }

    private void Awake()
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();    // 아이템 표시용 이미지 컴포넌트 찾아놓기
        backgroundImage = transform.GetChild(1).GetComponent<Image>();    // 아이템 표시용 이미지 컴포넌트 찾아놓기
        backgroundImage.color = Color.clear;
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// ShorCutUI의 초기화 작업
    /// </summary>
    /// <param name="targetSlot">이 슬롯이랑 연결된 ItemSlot</param>
    public void Initialize(uint newID, ItemSlot targetSlot)
    {
        id = newID;
        itemSlot = targetSlot;
        itemSlot.onSlotItemChange += Refresh; // ItemSlot에 아이템이 변경될 경우 실행될 델리게이트에 함수 등록        
    }

    /// <summary>
    /// 슬롯에서 표시되는 아이콘 이미지 갱신용 함수
    /// </summary>
    public void Refresh()
    {
        if (itemSlot.SlotItemData != null)
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
    /// 선택 상태 결정
    /// </summary>
    /// <param name="_tf">true면 불투명하게, false면 투명하게</param>
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
