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
                            text.text = "전체화면 모드";
                            break;
                        case FullScreenMode.FullScreenWindow:
                            text.text = "테두리 없는 창 모드";
                            break;
                        case FullScreenMode.Windowed:
                            text.text = "창 모드";
                            break;
                        case FullScreenMode.MaximizedWindow:    // MacOS 에서만 사용 가능
                            text.text = "전체화면 모드";
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
        backgroundImage.color = initialColor;    // 슬롯에 불투명한게 사라짐
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
