using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UI_Setting : UIBase
{
    [SerializeField]
    GameObject menuUI;

    GameObject obj;
    TextMeshProUGUI text;

    protected override void Awake()
    {
        base.Awake();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public enum UICategory
    {
        FullScreen = 0,
        Resolution
    }

    public UICategory category;

    private void OnEnable()
    {
        UpdateMenuName();
    }

    public void UpdateMenuName()
    {
        StartCoroutine(UpdateMenu());
    }

    IEnumerator UpdateMenu()
    {
        for (int i = 0; i < 2; i++)
        {
            switch (category)
            {
                case UICategory.FullScreen:
                    switch (Screen.fullScreenMode)
                    {
                        case FullScreenMode.ExclusiveFullScreen:
                            text.text = "��üȭ�� ���";
                            break;
                        case FullScreenMode.FullScreenWindow:
                            text.text = "�׵θ� ���� â ���";
                            break;
                        case FullScreenMode.Windowed:
                            text.text = "â ���";
                            break;
                        case FullScreenMode.MaximizedWindow:    // MacOS ������ ��� ����
                            text.text = "��üȭ�� ���";
                            break;                        
                        default:
                            break;
                    }
                    break;
                case UICategory.Resolution:
                    text.text = $"{Screen.width} * {Screen.height}";
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        backgroundImage.color = initialColor;    // ���Կ� �������Ѱ� �����
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        obj = Instantiate(menuUI, this.transform);
    }

    private void OnDisable()
    {
        if(obj != null)
            Destroy(obj);
    }
}
