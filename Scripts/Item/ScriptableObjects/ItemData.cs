using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �����͸� �����ϴ� ������ ������ ����� ���ִ� ��ũ��Ʈ
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data", order = 1)]
public class ItemData : ScriptableObject    // ���� ���ϴ� �����͸� ������ �� �ִ� ������������ ������ �� �ְ� ���ִ� Ŭ����
{
    [Header("�⺻ ������")]
    public uint id = 0;                     // ������ ID
    public string itemName = "������";       // ������ �̸�
    public Sprite itemIcon;                 // ������ ������
    public GameObject prefab;               // �������� ������
    [TextArea]  // ���� �����ϰ� ����
    public string itemDescription;
}
