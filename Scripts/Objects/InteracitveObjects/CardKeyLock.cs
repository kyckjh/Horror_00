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
            UIManager.Inst.SetMessagePanel("�����ġ ������ �����ֽ��ϴ�");
            return false;
        }
        if (data == null)
        {
            UIManager.Inst.SetMessagePanel("�������� ���õ��� �ʾҽ��ϴ�");
            return false;
        }
        if(data.id == (uint)ItemIDCode.CardKey)
        {
            door.UnLock();
        }
        else
        {
            UIManager.Inst.SetMessagePanel("����� �� ���� �������Դϴ�");
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
