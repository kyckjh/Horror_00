using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUsableObject
{
    /// <summary>
    /// ��� ������ ������Ʈ
    /// </summary>
    /// <param name="data">������Ʈ�� ����� ������</param>
    /// <returns>������ ��� �� �κ��丮���� ������� true ��ȯ, �״�� ������ false ��ȯ</returns>
    bool Use(ItemData data);
}
