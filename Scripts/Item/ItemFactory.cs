using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ Ŭ����(������)
/// </summary>
public class ItemFactory
{
    static uint itemCount = 0;   // �̶����� ������ �� ������ ����. (�� �����ۺ� ���� ���̵� �뵵�� ���)

    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="code">������ �������� ����</param>
    /// <returns>������ ���ӿ�����Ʈ</returns>
    public static GameObject MakeItem(ItemIDCode code)
    {
        GameObject obj = new GameObject();              // �� ������Ʈ ����� ((0,0,0)�� ������)
        Item item = obj.AddComponent<Item>();           // Item ������Ʈ �߰�

        item.data = GameManager.Inst.ItemData[code];    // ItemData ����
        string[] itemName = item.data.name.Split("_");  // ���� �����ϴ� ������ �°� �̸� ����
        obj.name = $"{itemName[1]}_{itemCount}";        // ���� ���̵� �߰�
        obj.tag = "Item";
        obj.layer = LayerMask.NameToLayer("Item");      // ���̾� ����

        itemCount++;    // ������ ������ ���� �������Ѽ� �ߺ��� ������ ó��

        return obj;     // �����Ϸ�� �� ����
    }

    public static GameObject MakeItem(ItemIDCode code, Transform transform)
    {
        GameObject obj = MakeItem(code);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.parent = transform;

        return obj;
    }

    /// <summary>
    /// ������ ���� ��ġ�� ��¦ ������Ű ���� �Լ�
    /// </summary>
    /// <param name="code">������ ������</param>
    /// <param name="position">������ ��ġ</param>
    /// <param name="randomNoise">false�� ��Ȯ�� position�� ����. true�� position���� ��¦ ��ġ ����</param>
    /// <returns></returns>
    public static GameObject MakeItem(ItemIDCode code, Vector3 position, bool randomNoise = false)
    {
        GameObject obj = MakeItem(code);
        if (randomNoise)
        {
            Vector2 noise = Random.insideUnitCircle * 0.5f;
            position.x += noise.x;
            position.z += noise.y;
        }
        obj.transform.position = position;

        return obj;
    }

    /// <summary>
    /// �������� ������ �����ϱ� ���� �Լ�
    /// </summary>
    /// <param name="code">������ ������</param>
    /// <param name="position">������ �������� ��ġ</param>
    /// <param name="count">������ ����</param>
    public static void MakeItems(ItemIDCode code, Vector3 position, uint count)
    {
        for (int i = 0; i < count; i++)
        {
            MakeItem(code, position, true);
        }
    }

    public static GameObject MakeItem(uint id)
    {
        return MakeItem((ItemIDCode)id);
    }

    public static GameObject MakeItem(uint id, Vector3 position, bool randomNoise = false)
    {
        return MakeItem((ItemIDCode)id, position, randomNoise);
    }

    public static void MakeItems(uint id, Vector3 position, uint count)
    {
        MakeItems((ItemIDCode)id, position, count);
    }
}
