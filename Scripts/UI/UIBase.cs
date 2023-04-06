using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    protected bool isSelected = false;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            if (isSelected)
            {
                OnSelect();
            }
            else
            {
                OnUnselect();
            }
        }
    }

    protected virtual void OnSelect()
    {
        backgroundImage.color = selectedColor;  // 슬롯이 가장 불투명해짐
    }

    protected virtual void OnUnselect()
    {
        backgroundImage.color = initialColor;   // 슬롯에 불투명한게 사라짐
    }

    protected Image backgroundImage;    // 커서 위에 올렸을 때 불투명해질 이미지

    protected Color initialColor = new Color(1, 1, 1, 0.1f);

    Color unselectedColor = new Color(1, 1, 1, 0.3f);

    Color selectedColor = new Color(1, 1, 1, 0.5f);

    protected virtual void Awake()  // 오버라이드 가능하도록 virtual 추가
    {
        backgroundImage = GetComponent<Image>();    // 아이템 표시용 이미지 컴포넌트 찾아놓기
        initialColor = backgroundImage.color;
        unselectedColor = initialColor + new Color(0, 0, 0, 0.2f); ;
        selectedColor = initialColor + new Color(0, 0, 0, 0.4f); ;
    }

    /// <summary>
    /// 슬롯위에 마우스 포인터가 들어왔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            backgroundImage.color = unselectedColor;    // 슬롯이 살짝 불투명해짐        
        }
    }

    /// <summary>
    /// 슬롯위에서 마우스 포인터가 나갔을 때
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            backgroundImage.color = initialColor;    // 슬롯에 불투명한게 사라짐
        }
    }


    /// <summary>
    /// 슬롯위에서 마우스 포인터가 움직일 때
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerMove(PointerEventData eventData)
    {

    }


    /// <summary>
    /// 슬롯을 마우스로 클릭했을 때
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        IsSelected = true;  // 이 슬롯을 선택 상태로 전환
    }
}
