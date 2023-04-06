using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKeyLock : MonoBehaviour, IUsableObject
{
    [SerializeField]
    Material screenMat_Normal, screenMat_Activated, screenMat_Error, panelMat_Activated;

    [SerializeField]
    Renderer screen, panel;

    [SerializeField]
    Door door;

    bool isActivate = false;

    private void Awake()
    {
        screen = transform.Find("Screen").GetComponent<Renderer>();
        panel = transform.Find("Panel").GetComponent<Renderer>();
    }

    public bool Use(ItemData data)
    {
        if(!isActivate)
        {
            UIManager.Inst.SetMessagePanel("잠금장치 전원이 꺼져있습니다");
            return false;
        }
        if (data == null)
        {
            UIManager.Inst.SetMessagePanel("아이템이 선택되지 않았습니다");
            return false;
        }
        if(data.id == (uint)ItemIDCode.CardKey)
        {
            door.UnLock();
        }
        else
        {
            UIManager.Inst.SetMessagePanel("사용할 수 없는 아이템입니다");
        }
        return false;
    }

    public void Activate()
    {
        isActivate = true;
        screen.material = screenMat_Activated;
        panel.material = panelMat_Activated;
    }
}
