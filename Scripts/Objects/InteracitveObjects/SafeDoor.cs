using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeDoor : MonoBehaviour, IUsableObject
{
    [SerializeField]
    [Range(0, 10)]
    float openSpeed = 2.0f;

    [SerializeField]
    Collider patientCol;


    public bool Use(ItemData data)
    {
        if (data == null)
        {
            UIManager.Inst.SetMessagePanel("�������� ���õ��� �ʾҽ��ϴ�");
            return false;
        }
        if (data.id == (uint)ItemIDCode.Key)
        {
            StartCoroutine(Open());
            Collider collider = GetComponent<Collider>();
            collider.enabled = false;
            patientCol.enabled = true;
        }
        else
        {
            UIManager.Inst.SetMessagePanel("����� �� ���� �������Դϴ�");
        }
        return false;
    }

    IEnumerator Open()
    {
        while(transform.localRotation.y > -115.0f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, -125.0f, 0), Time.deltaTime * openSpeed);
            yield return null;
        }
        yield return null;
    }
}
