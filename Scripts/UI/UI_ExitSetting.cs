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
        if(set == ExitOrRestart.Exit) // 게임종료 버튼 클릭
        {
            Application.Quit();
        }
        else // 게임재시작 버튼 클릭
        {
            // 에러 방지를 위해 싱글톤 제거, 로딩 씬 제작 후에는 로딩 씬에서 제거하는게 나을수도?
            Destroy(UIManager.Inst);    
            Destroy(GameManager.Inst);

            SceneManager.LoadScene("LoadingMainScene"); // 지금은 바로 씬 재시작, 로딩 씬 제작 필요
        }
    }
}
