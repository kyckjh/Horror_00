using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISettableObject
{
    /// <summary>
    /// ������Ʈ�� �������� ������ �� �ִ� �Լ�
    /// </summary>
    /// <param name="data">������ ������</param>
    /// <returns>������ ��� �� �κ��丮���� ������� true ��ȯ, �״�� ������ false ��ȯ</returns>
    bool Set(ItemData data);
}
