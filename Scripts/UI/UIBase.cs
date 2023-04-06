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
        backgroundImage.color = selectedColor;  // ������ ���� ����������
    }

    protected virtual void OnUnselect()
    {
        backgroundImage.color = initialColor;   // ���Կ� �������Ѱ� �����
    }

    protected Image backgroundImage;    // Ŀ�� ���� �÷��� �� ���������� �̹���

    protected Color initialColor = new Color(1, 1, 1, 0.1f);

    Color unselectedColor = new Color(1, 1, 1, 0.3f);

    Color selectedColor = new Color(1, 1, 1, 0.5f);

    protected virtual void Awake()  // �������̵� �����ϵ��� virtual �߰�
    {
        backgroundImage = GetComponent<Image>();    // ������ ǥ�ÿ� �̹��� ������Ʈ ã�Ƴ���
        initialColor = backgroundImage.color;
        unselectedColor = initialColor + new Color(0, 0, 0, 0.2f); ;
        selectedColor = initialColor + new Color(0, 0, 0, 0.4f); ;
    }

    /// <summary>
    /// �������� ���콺 �����Ͱ� ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            backgroundImage.color = unselectedColor;    // ������ ��¦ ����������        
        }
    }

    /// <summary>
    /// ���������� ���콺 �����Ͱ� ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            backgroundImage.color = initialColor;    // ���Կ� �������Ѱ� �����
        }
    }


    /// <summary>
    /// ���������� ���콺 �����Ͱ� ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerMove(PointerEventData eventData)
    {

    }


    /// <summary>
    /// ������ ���콺�� Ŭ������ ��
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        IsSelected = true;  // �� ������ ���� ���·� ��ȯ
    }
}
