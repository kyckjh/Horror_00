using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailInfoUI : MonoBehaviour
{    
    // 사용하는 컴포넌트들 --------------------------------------------------------------------------
    TextMeshProUGUI itemName;
    Image itemImage;
    TextMeshProUGUI itemDecription;
    CanvasGroup canvasGroup;

    // 기본 데이터 ---------------------------------------------------------------------------------
    /// <summary>
    /// 표시할 상세정보
    /// </summary>
    ItemData itemData;

    public ItemData ItemData { get => itemData; }
    uint itemID;
    /// <summary>
    /// 상세정보창 열고 닫는 기능을 일시 정지하기 위한 플래그(true면 열리지 않는다.)
    /// </summary>
    public bool IsPause;

    // 함수들 --------------------------------------------------------------------------------------
    /// <summary>
    /// 상세정보창 열기
    /// </summary>
    /// <param name="data">표시할 데이터</param>
    public void Open(ItemData data, uint id)
    {
        //if (!IsPause)   // pause 상태가 아닐 때만 열기
        {
            itemData = data;    // 데이터 넣고
            Refresh();          // 화면 갱신
            canvasGroup.alpha = 1;  // 알파값 조절로 on/off 결정
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            itemID = id;
        }
    }

    /// <summary>
    /// 상세 정보창 닫기
    /// </summary>
    public void Close()
    {
        //if (!IsPause)   // pause 상태가 아닐때만 닫기
        {
            itemData = null;        // 데이터 비우기
            canvasGroup.alpha = 0;  // 알파값 조절해서 보이지 않게 만들기
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Use()
    {
        GameManager.Inst.MainPlayer.MySlotID = itemID;
    }

    /// <summary>
    /// 가지고 있는 데이터 기반으로 화면 갱신
    /// </summary>
    void Refresh()
    {
        if (itemData != null)   // 데이터가 있으면 데이터 갱신
        {
            itemName.text = itemData.itemName;
            itemImage.sprite = itemData.itemIcon;
            itemDecription.text = itemData.itemDescription;
        }
    }

    // 유니티 이벤트 함수 --------------------------------------------------------------------------
    private void Awake()
    {
        itemName = transform.Find("ItemName").GetChild(1).GetComponent<TextMeshProUGUI>();
        itemImage = transform.Find("ItemImage").GetComponent<Image>();
        itemDecription = transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>(); 
        Button closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(Close);
        Button useButton = transform.Find("UseButton").GetComponent<Button>();
        useButton.onClick.AddListener(Use);
    }

    private void Start()
    {
        GameManager.Inst.InvenUI.OnInventoryClose += Close; // 인벤토리 닫을 때 같이 닫기
    }
}
