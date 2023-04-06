using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_ExitSetting : UIBase
{
    public enum ExitOrRestart
    {
        Exit,
        Restart
    }

    public ExitOrRestart set;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if(set == ExitOrRestart.Exit) // �������� ��ư Ŭ��
        {
            Application.Quit();
        }
        else // ��������� ��ư Ŭ��
        {
            // ���� ������ ���� �̱��� ����, �ε� �� ���� �Ŀ��� �ε� ������ �����ϴ°� ��������?
            Destroy(UIManager.Inst);    
            Destroy(GameManager.Inst);

            SceneManager.LoadScene("LoadingMainScene"); // ������ �ٷ� �� �����, �ε� �� ���� �ʿ�
        }
    }
}
